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
        private AggregationRun ThisAggregationRun;
        private MongoDbConnection MongoCxn;
        private CernerReferencedCodeDictionaries ReferencedCodes;
        private MongoAggregationRunUpdater MongoRunUpdater;

        IMongoCollection<MongodbIdentifierEntity> IdentifierCollection;
        IMongoCollection<MongodbPersonEntity> PersonCollection;
        IMongoCollection<MongodbRefCodeEntity> RefCodeCollection;
        IMongoCollection<MongodbPhoneEntity> PhoneCollection;
        IMongoCollection<MongodbAddressEntity> AddressCollection;

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

        private bool InitializeRun()
        {
            bool Initialized;

            // Initialize MongoDb Collections
            IdentifierCollection = MongoCxn.Db.GetCollection<MongodbIdentifierEntity>("identifiers");
            PersonCollection = MongoCxn.Db.GetCollection<MongodbPersonEntity>("person");
            RefCodeCollection = MongoCxn.Db.GetCollection<MongodbRefCodeEntity>("referencecode");
            PhoneCollection = MongoCxn.Db.GetCollection<MongodbPhoneEntity>("phone");
            AddressCollection = MongoCxn.Db.GetCollection<MongodbAddressEntity>("address");

            Initialized = ReferencedCodes.Initialize(RefCodeCollection);
            ThisAggregationRun = GetNewAggregationRun();
            Initialized &= ThisAggregationRun.dbid > 0;

            MongoRunUpdater = new MongoAggregationRunUpdater(ThisAggregationRun.dbid, MongoCxn.Db);

            return Initialized;
        }

        public bool AggregateAllAvailablePatients()
        {
            bool OverallResult = true;
            int PatientCounter = 0;

            if (!InitializeRun())
            {
                return false;
            }

            #region temporary code
            ClearAllMongoAggregationRunNumbers();
            #endregion

            ThisAggregationRun.StatusFlags = AggregationRunStatus.InProcess;
            CdrDb.Context.SubmitChanges();
            
            FilterDefinition<MongodbPersonEntity> PatientFilterDef = Builders<MongodbPersonEntity>.Filter
                .Where(  // TODO get criteria right
                           x => x.LastName != ""           // must have a last name
                        && x.UniquePersonIdentifier != ""  // has an identifier to be referenced from other txt files
                        && !(x.LastAggregationRun > 0)     // not previously aggregated
                      );

            using (var PersonCursor = PersonCollection.Find<MongodbPersonEntity>(PatientFilterDef)/*.Project<MongodbPersonEntity>(Proj)*/.ToCursor())
            {
                while (PersonCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbPersonEntity PersonDocument in PersonCursor.Current)
                    {
                        PatientCounter++;

                        bool ThisPatientAggregationResult = AggregateOnePatient(PersonDocument);
                        OverallResult &= ThisPatientAggregationResult;
                    }
                }
            }

           ThisAggregationRun.StatusFlags = AggregationRunStatus.Complete;
            CdrDb.Context.SubmitChanges();

            Trace.WriteLine("Processed " + PatientCounter + " patients");
            return OverallResult;
        }

        private bool AggregateOnePatient(MongodbPersonEntity PersonDocument)
        {
            DateTime ParsedDateTime;
            bool OverallSuccess = true;

            Patient ThisPatient = new Patient();
            ThisPatient.NameLast = PersonDocument.LastName;
            ThisPatient.NameFirst = PersonDocument.FirstName;
            ThisPatient.NameMiddle = PersonDocument.MiddleName;
            DateTime.TryParse(PersonDocument.BirthDateTime, out ParsedDateTime);        // Will be DateTime.MinValue on parse failure
            ThisPatient.BirthDate = ParsedDateTime;
            ThisPatient.Gender = ReferencedCodes.GetCdrGenderEnum(PersonDocument.Gender);
            DateTime.TryParse(PersonDocument.DeceasedDateTime, out ParsedDateTime);        // Will be DateTime.MinValue on parse failure
            ThisPatient.DeathDate = ParsedDateTime;
            ThisPatient.Race = ReferencedCodes.RaceCodeMeanings[PersonDocument.Race];  // coded
            ThisPatient.Ethnicity = ReferencedCodes.EthnicityCodeMeanings[PersonDocument.Ethnicity];  // coded
            ThisPatient.MaritalStatus = ReferencedCodes.GetCdrMaritalStatusEnum(PersonDocument.MaritalStatus);  // coded

            // TODO maybe do a quality check on Patient before proceeding to aggregate the referencing entities.  

            #region PostgreSQL transaction to process all data for one patient
            CdrDb.Context.Connection.Open();
            CdrDb.Context.Transaction = CdrDb.Context.Connection.BeginTransaction();

            CdrDb.Context.Patients.InsertOnSubmit(ThisPatient);
            CdrDb.Context.SubmitChanges();  // TODO Is it possible that only one SubmitChanges call for the entire transaction is more efficient?  

            MongoRunUpdater.PersonIdList.Add(PersonDocument.Id);

            //            EntitySet<TelephoneNumber> _TelephoneNumbers;
            OverallSuccess &= AggregateTelephoneNumbers(PersonDocument, ThisPatient);

            //            EntitySet<PhysicalAddress> _PhysicalAddresses;
            OverallSuccess &= AggregateAddresses(PersonDocument, ThisPatient);

            //            EntitySet<PatientIdentifier> _PatientIdentifiers;
            //            EntitySet<VisitEncounter> _VisitEncounters;
            //            EntitySet<Measurement> _Measurements;
            //            EntitySet<Medication> _Medications;
            //            EntitySet<Immunization> _Immunizations;
            //            EntitySet<InsuranceCoverage> _InsuranceCoverages;
            //            EntitySet<Diagnosis> _Diagnoses;
            //            EntitySet<Problem> _Problems;

            if (OverallSuccess)
            {
                CdrDb.Context.Transaction.Commit();
                MongoRunUpdater.UpdateAll();
            }
            else
            {
                CdrDb.Context.Transaction.Rollback();
                MongoRunUpdater.Reset();
            }

            CdrDb.Context.Connection.Close();
            #endregion  PostgreSQL transaction to process all data for one patient

            return OverallSuccess;
        }

        private bool AggregateTelephoneNumbers(MongodbPersonEntity MongoPerson, Patient PgPatient)
        {
            int PhoneCounter = 0;

            FilterDefinition<MongodbPhoneEntity> PhoneFilterDef = Builders<MongodbPhoneEntity>.Filter
                .Where(
                         x => x.EntityType == "PERSON"
                      && x.EntityIdentifier == MongoPerson.UniquePersonIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      );

            using (var PhoneCursor = PhoneCollection.Find<MongodbPhoneEntity>(PhoneFilterDef).ToCursor())
            {
                while (PhoneCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbPhoneEntity PhoneDoc in PhoneCursor.Current)
                    {
                        PhoneCounter++;
                        DateTime ActiveStatusDT;
                        DateTime.TryParse(PhoneDoc.ActiveStatusDateTime, out ActiveStatusDT);

                        TelephoneNumber NewPgRecord = new TelephoneNumber
                        {
                            Number = new PhoneNumber
                            {
                                Number = PhoneDoc.PhoneNumber,
                                PhoneType = ReferencedCodes.GetCdrPhoneTypeEnum(PhoneDoc.Type)
                            },
                            Patient = PgPatient,
                            DateFirstReported = ActiveStatusDT,
                            DateLastReported = ActiveStatusDT
                        };

                        CdrDb.Context.TelephoneNumbers.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.PhoneIdList.Add(PhoneDoc.Id);
                    }
                }
            }

            return true;
        }

        private bool AggregateAddresses(MongodbPersonEntity MongoPerson, Patient PgPatient)
        {
            int AddressCounter = 0;

            FilterDefinition<MongodbAddressEntity> AddressFilterDef = Builders<MongodbAddressEntity>.Filter
                .Where(
                         x => x.EntityType == "PERSON"
                      && x.EntityIdentifier == MongoPerson.UniquePersonIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      );

            using (var AddressCursor = AddressCollection.Find<MongodbAddressEntity>(AddressFilterDef).ToCursor())
            {
                while (AddressCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbAddressEntity AddressDoc in AddressCursor.Current)
                    {
                        AddressCounter++;
                        DateTime ActiveStatusDT;
                        DateTime.TryParse(AddressDoc.ActiveStatusDateTime, out ActiveStatusDT);

                        PhysicalAddress NewPgRecord = new PhysicalAddress
                        {
                            Address = new Address
                            {
                                City = AddressDoc.CityText,
                                State = AddressDoc.StateText,
                                PostalCode = AddressDoc.ZipCode,
                                Country = AddressDoc.CountryText,
                                Line1 = AddressDoc.AddressLine1,
                                Line2 = AddressDoc.AddressLine2,
                                Line3 = AddressDoc.AddressLine3,
                                Line4 = AddressDoc.AddressLine4
                            },
                            AddressType = ReferencedCodes.GetCdrAddressTypeEnum(AddressDoc.Type),
                            patient = PgPatient,
                            DateFirstReported = ActiveStatusDT,
                            DateLastReported = ActiveStatusDT
                        };

                        CdrDb.Context.PhysicalAddresses.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.AddressIdList.Add(AddressDoc.Id);
                    }
                }
            }

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

        private void ClearAllMongoAggregationRunNumbers()
        {
            FilterDefinition<MongodbPersonEntity> PersonFilterDef = Builders<MongodbPersonEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbPersonEntity> PersonUpdateDef = Builders<MongodbPersonEntity>.Update.Unset(x => x.LastAggregationRun);
            PersonCollection.UpdateMany(PersonFilterDef, PersonUpdateDef);

            FilterDefinition<MongodbPhoneEntity> PhoneFilterDef = Builders<MongodbPhoneEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbPhoneEntity> PhoneUpdateDef = Builders<MongodbPhoneEntity>.Update.Unset(x => x.LastAggregationRun);
            UpdateResult a = PhoneCollection.UpdateMany(PhoneFilterDef, PhoneUpdateDef);

        }

    }
}
