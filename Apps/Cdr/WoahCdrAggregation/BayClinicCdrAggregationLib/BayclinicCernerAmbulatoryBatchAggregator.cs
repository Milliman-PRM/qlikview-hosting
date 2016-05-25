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

                Success = ReferencedCodes.Initialize(RefCodeCollection);
            }

            return Initialized;
        }

        public bool AggregateAllAvailablePatients()
        {
            bool Result = true;

            if (Initialized)
            {
                ProjectionDefinition<MongodbPersonEntity> Proj = Builders<MongodbPersonEntity>.Projection.Exclude("_id");
                Expression<Func<MongodbPersonEntity, bool>> PersonFilter = (x => x.LastName != "" && x.UniquePersonIdentifier != "");

                int PatientCounter = 0;
                //using (var cursor = PersonCollection.Find<MongodbPersonEntity>(x => x.LastName != "" && x.UniquePersonIdentifier != "")
                using (var cursor = PersonCollection.Find<MongodbPersonEntity>(PersonFilter)
                                                    //.Project<MongodbPersonEntity>(Proj)
                                                    .ToCursor())
                {
                    while (cursor.MoveNext())
                    {
                        foreach (MongodbPersonEntity PersonRecord in cursor.Current)
                        {
                            PatientCounter++;
                            Result &= AggregateOnePatient(PersonRecord);
                        }
                    }
                }
                Trace.WriteLine("Processed " + PatientCounter + " patients");
            }

            return Result;
        }

        private bool AggregateOnePatient(MongodbPersonEntity PersonRecord)
        {
            Patient Patient = new Patient();
            Patient.NameLast = PersonRecord.LastName;
            Patient.NameFirst = PersonRecord.FirstName;
            Patient.NameMiddle = PersonRecord.MiddleName;
            Patient.BirthDate = DateTime.Parse(PersonRecord.BirthDateTime);
            Patient.Gender = ReferencedCodes.GetCdrGenderEnum(PersonRecord.Gender);
            Patient.DeathDate = DateTime.Parse(PersonRecord.DeceasedDateTime);
            Patient.Race = PersonRecord.Race;  // coded
            Patient.Ethnicity = PersonRecord.Ethnicity;  // coded
            Patient.MaritalStatus = PersonRecord.MaritalStatus;  // coded

//            EntitySet<PatientIdentifier> _PatientIdentifiers;
//            EntitySet<PhysicalAddress> _PhysicalAddresses;
//            EntitySet<TelephoneNumber> _TelephoneNumbers;
//            EntitySet<VisitEncounter> _VisitEncounters;
//            EntitySet<Measurement> _Measurements;
//            EntitySet<Medication> _Medications;
//            EntitySet<Immunization> _Immunizations;
//            EntitySet<InsuranceCoverage> _InsuranceCoverages;
//            EntitySet<Diagnosis> _Diagnoses;
//            EntitySet<Problem> _Problems;



            /*
                    //private long _dbid;
                    //private string _NameLast;
                    //private string _NameFirst;
                    //private string _NameMiddle;
                    //private System.Nullable<System.DateTime> _BirthDate;
                    //private System.Nullable<Gender> _Gender;
                    //private System.Nullable<System.DateTime> _DeathDate;
                    //private string _Race;
                    private string _Ethnicity;
                    private System.Nullable<MaritalStatus> _MaritalStatus;
                    */


            return true;
        }

        public async void ListAllPatients()
        {
            BsonDocument Filter = new BsonDocument();
            ProjectionDefinition<MongodbPersonEntity> Proj = Builders<MongodbPersonEntity>.Projection.Exclude("_id");

            IMongoCollection<MongodbIdentifierEntity> IdentifierCollection = MongoCxn.Db.GetCollection<MongodbIdentifierEntity>("identifiers");
            IMongoCollection<MongodbPersonEntity> PersonCollection = MongoCxn.Db.GetCollection<MongodbPersonEntity>("person");

            int Counter = 0;
            using (var cursor = PersonCollection.Find<MongodbPersonEntity>(x => x.LastName != "" && x.UniquePersonIdentifier != "")
                                                .Project<MongodbPersonEntity>(Proj)
                                                .ToCursor())
            {
                while (await cursor.MoveNextAsync())
                {
                    foreach (MongodbPersonEntity Person in cursor.Current)
                    {
                        String PersonIdentifier = Person.UniquePersonIdentifier;

                        List<MongodbIdentifierEntity> IdentifierRecords = IdentifierCollection.Find(x => x.EntityType == "PERSON" && x.EntityIdentifier == PersonIdentifier).ToList();
                        //String TraceMessage = "For patient identifier " + Person.UniquePersonIdentifier + " there are " + IdentifierRecords.Count + " Records: ";
                        foreach (MongodbIdentifierEntity IdRecord in IdentifierRecords) {
                            Counter++;
                            //TraceMessage += IdRecord.Identifier + ", ";
                        }
                        //Trace.WriteLine(TraceMessage);
                    }
                }
            }
            Trace.WriteLine("Found identifiers: " + Counter);
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
