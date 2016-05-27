using System;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CdrContext;
using CdrDbLib;
using Devart.Data.Linq;
using MongoDbWrap;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace BayClinicCernerAmbulatory
{
    class BayClinicCernerAmbulatoryBatchAggregator
    {
        private const String FeedIdentity = "BayClinicCernerAmbulatoryExtract";
        // TODO maybe some of these should be arguments from the caller, or handled another way? 
        private String PgConnectionStringName = ConfigurationManager.AppSettings["CdrPostgreSQLConnectionString"];
        private String NewBayClinicAmbulatoryMongoCredentialConfigFile = ConfigurationManager.AppSettings["NewBayClinicAmbulatoryMongoCredentialConfigFile"];
        private String NewBayClinicAmbulatoryMongoCredentialSection = ConfigurationManager.AppSettings["NewBayClinicAmbulatoryMongoCredentialSection"];

        private CdrDbInterface CdrDb;
        private DataFeed FeedObject;
        private AggregationRun AggregationRun;
        private MongoDbConnection MongoCxn;
        private CernerReferencedCodeDictionaries ReferencedCodes;

        private bool Initialized = false;

        IMongoCollection<MongodbIdentifierEntity> IdentifierCollection;
        IMongoCollection<MongodbPersonEntity> PersonCollection;
        IMongoCollection<MongodbRefCodeEntity> RefCodeCollection;
        IMongoCollection<MongodbPhoneEntity> PhoneCollection;

        public BayClinicCernerAmbulatoryBatchAggregator(String PgConnectionName = null)
        {
            if (String.IsNullOrEmpty(PgConnectionName))
            {
                PgConnectionName = PgConnectionStringName;
            }

            CdrDb = new CdrDbInterface(PgConnectionName, ConnectionArgumentType.ConnectionStringName);
            MongoCxn = new MongoDbConnection(NewBayClinicAmbulatoryMongoCredentialConfigFile, NewBayClinicAmbulatoryMongoCredentialSection);
            ReferencedCodes = new CernerReferencedCodeDictionaries();
        }

        public bool Initialize()
        {
            if (!Initialized)
            {
                bool Success;
                AggregationRun = GetNewAggregationRun();
                Initialized = AggregationRun.dbid > 0;

                // Initialize MongoDb Collections
                IdentifierCollection = MongoCxn.Db.GetCollection<MongodbIdentifierEntity>("identifiers");
                PersonCollection = MongoCxn.Db.GetCollection<MongodbPersonEntity>("person");
                RefCodeCollection = MongoCxn.Db.GetCollection<MongodbRefCodeEntity>("referencecode");
                PhoneCollection = MongoCxn.Db.GetCollection<MongodbPhoneEntity>("phone");

                Success = ReferencedCodes.Initialize(RefCodeCollection);
            }

            return Initialized;
        }

        public bool AggregateAllAvailablePatients()
        {
            bool OverallResult = true;
            int PatientCounter = 0;

            if (Initialized)
            {
                //ProjectionDefinition<MongodbPersonEntity> Proj = Builders<MongodbPersonEntity>.Projection.Exclude("_id");
                FilterDefinition<MongodbPersonEntity> PatientFilterDef = Builders<MongodbPersonEntity>.Filter
                    .Where(
                             x => x.LastName != ""           // must have a last name
                          && x.UniquePersonIdentifier != ""  // has an identifier to be referenced from other txt files
                          && !(x.LastAggregationRun > 0)     // not previously aggregated
                          );

                using (var PersonCursor = PersonCollection.Find<MongodbPersonEntity>(PatientFilterDef)/*.Project<MongodbPersonEntity>(Proj)*/.ToCursor())
                {
                    while (PersonCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                    {
                        foreach (MongodbPersonEntity PersonRecord in PersonCursor.Current)
                        {
                            PatientCounter++;

                            bool ThisPatientAggregationResult = AggregateOnePatient(PersonRecord);
                            OverallResult &= ThisPatientAggregationResult;

                            if (ThisPatientAggregationResult)
                            {
                                FilterDefinition<MongodbPersonEntity> FilterDef = Builders<MongodbPersonEntity>.Filter.Where(x => x.Id == PersonRecord.Id);
                                UpdateDefinition<MongodbPersonEntity> UpdateDef = Builders<MongodbPersonEntity>.Update.Set(p => p.LastAggregationRun, AggregationRun.dbid);
                                //PersonCollection.UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this person record has been aggregated
                            }
                        }
                    }
                }
                Trace.WriteLine("Processed " + PatientCounter + " patients");
            }

            return OverallResult;
        }

        private bool AggregateOnePatient(MongodbPersonEntity PersonRecord)
        {
            DateTime ParsedDateTime;
            Patient Patient = new Patient();

            Patient.NameLast = PersonRecord.LastName;
            Patient.NameFirst = PersonRecord.FirstName;
            Patient.NameMiddle = PersonRecord.MiddleName;
            DateTime.TryParse(PersonRecord.BirthDateTime, out ParsedDateTime);        // Could be DateTime.MinValue on failure
            Patient.BirthDate = ParsedDateTime;
            Patient.Gender = ReferencedCodes.GetCdrGenderEnum(PersonRecord.Gender);
            DateTime.TryParse(PersonRecord.DeceasedDateTime, out ParsedDateTime);        // Could be DateTime.MinValue on failure
            Patient.DeathDate = ParsedDateTime;
            Patient.Race = ReferencedCodes.RaceCodeMeanings[PersonRecord.Race];  // coded
            Patient.Ethnicity = ReferencedCodes.EthnicityCodeMeanings[PersonRecord.Ethnicity];  // coded
            Patient.MaritalStatus = ReferencedCodes.GetCdrMaritalStatusEnum(PersonRecord.MaritalStatus);  // coded

            //            EntitySet<TelephoneNumber> _TelephoneNumbers;

            //            EntitySet<PatientIdentifier> _PatientIdentifiers;
            //            EntitySet<PhysicalAddress> _PhysicalAddresses;
            //            EntitySet<VisitEncounter> _VisitEncounters;
            //            EntitySet<Measurement> _Measurements;
            //            EntitySet<Medication> _Medications;
            //            EntitySet<Immunization> _Immunizations;
            //            EntitySet<InsuranceCoverage> _InsuranceCoverages;
            //            EntitySet<Diagnosis> _Diagnoses;
            //            EntitySet<Problem> _Problems;

            CdrDb.Context.Patients.InsertOnSubmit(Patient);
            CdrDb.Context.SubmitChanges();

            return true;
        }

        private bool AggregateTelephoneNumbers(MongodbPersonEntity MongoPerson, Patient PgPatient)
        {
            int PhoneCounter = 0;
            FilterDefinition<MongodbPhoneEntity> PhoneFilterDef = Builders<MongodbPhoneEntity>.Filter
                .Where(
                         x => x.EntityType == "PERSON"
                      && x.EntityIdentifier == MongoPerson.UniquePersonIdentifier
                      );

            using (var PhoneCursor = PhoneCollection.Find<MongodbPhoneEntity>(PhoneFilterDef).ToCursor())
            {
                while (PhoneCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbPhoneEntity PhoneRecord in PhoneCursor.Current)
                    {
                        PhoneCounter++;

                        bool ThisPatientAggregationResult = AggregateOnePatient(PersonRecord);
                        OverallResult &= ThisPatientAggregationResult;

                        if (ThisPatientAggregationResult)
                        {
                            FilterDefinition<MongodbPersonEntity> FilterDef = Builders<MongodbPersonEntity>.Filter.Where(x => x.Id == PersonRecord.Id);
                            UpdateDefinition<MongodbPersonEntity> UpdateDef = Builders<MongodbPersonEntity>.Update.Set(p => p.LastAggregationRun, AggregationRun.dbid);
                            //PersonCollection.UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this person record has been aggregated
                        }
                    }
                }
            }

            TelephoneNumber T = new TelephoneNumber
            {
                Number = new PhoneNumber { Number = "", PhoneType = PhoneType.Home },
                Patient = PgPatient
            };

            CdrDb.Context.TelephoneNumbers.InsertOnSubmit(T);

            return true;
        }

        private AggregationRun GetNewAggregationRun()
        {
            FeedObject = CdrDb.EnsureFeed(FeedIdentity);

            AggregationRun NewRun = new AggregationRun
            {
                DataFeeddbid = FeedObject.dbid,
                StatusFlags = AggregationRunStatus.RunNumberReserved
            };

            CdrDb.Context.AggregationRuns.InsertOnSubmit(NewRun);
            try
            {
                CdrDb.Context.SubmitChanges();
            }
            catch (Exception e)
            {
                string x = e.Message;
            }

            return NewRun;
        }

    }
}
