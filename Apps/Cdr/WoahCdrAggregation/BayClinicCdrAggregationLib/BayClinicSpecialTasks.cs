using System;
using System.Configuration;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CdrContext;
using CdrDbLib;
using MongoDbWrap;
using MongoDB.Driver;
using MongoDB.Bson;

namespace BayClinicCernerAmbulatory
{
    public class BayClinicSpecialTasks
    {
        private const String FeedIdentity = "bayclinictest";  // TODO move this to config shared between aggregation and extraction applications
        MongoDbConnection MongoCxn;
        CdrDbInterface CdrDb;

        // TODO Some of these should be arguments from the caller, or sourced another way? 
        private String PgConnectionStringName = ConfigurationManager.AppSettings["CdrPostgreSQLConnectionString"];
        private String NewBayClinicEmrMongoCredentialConfigFile = ConfigurationManager.AppSettings["NewBayClinicEmrMongoCredentialConfigFile"];
        private String NewBayClinicEmrMongoCredentialSection = ConfigurationManager.AppSettings["NewBayClinicEmrMongoCredentialSection"];

        IMongoCollection<MongodbDiagnosisEntity> DiagnosisCollection;
        IMongoCollection<MongodbReferenceTerminologyEntity> ReferenceTerminologyCollection;

        /// <summary>
        /// Initializes member instance variables to support this aggregation run
        /// </summary>
        /// <returns>boolean indicating success</returns>
        private bool InitializeMongoConnection()
        {
            MongoCxn = new MongoDbConnection();
            // Mongo.DbSuffix is optional
            if (!Environment.ExpandEnvironmentVariables("<%ephi_username%><%ephi_password%><%Mongo.UserDomain%><%Mongo.Host%>").Contains("<%"))
            {
                // all required environment exists
                if (MongoCxn.InitializeWithEnvironment(FeedIdentity))
                {
                    Trace.WriteLine("MongoCxn.InitializeWithEnvironment complete");
                }
                else
                {
                    Trace.WriteLine("MongoCxn.InitializeWithEnvironment failed");
                }
            }
            else
            {
                // TODO validate config file?
                if (MongoCxn.InitializeWithIni(NewBayClinicEmrMongoCredentialConfigFile, NewBayClinicEmrMongoCredentialSection))
                {
                    Trace.WriteLine("MongoCxn.InitializeWithIni complete");
                }
                else
                {
                    Trace.WriteLine("MongoCxn.InitializeWithIni failed");
                }
            }

            if (MongoCxn.GetCollectionNames().Count > 1)
            {
                Trace.WriteLine("MongoDB connection opened, collection(s) found");
                return true;
            }
            else
            {
                Trace.WriteLine("MongoDB connection failed");
                return false;
            }
        }

        private bool InitializePgConnection()
        {
            String EphiUserName = Environment.GetEnvironmentVariable("ephi_username");  // should be available if launched from Jenkins
            String EphiPassword = Environment.GetEnvironmentVariable("ephi_password");  // should be available if launched from Jenkins

            if (!Environment.ExpandEnvironmentVariables("<%ephi_username%><%ephi_password%><%Pg.Host%><%Pg.Db%>").Contains("<%"))
            {
                String PgHost = Environment.GetEnvironmentVariable("Pg.Host");  // should be available if launched from Jenkins
                String PgDb = Environment.GetEnvironmentVariable("Pg.Db");  // should be available if launched from Jenkins
                String PgPort = Environment.GetEnvironmentVariable("Pg.Port");  // optional
                String PgSchema = Environment.GetEnvironmentVariable("Pg.Schema");  // optional

                String PgConnectionString = "User Id=" + EphiUserName.ToLower() +
                                           ";password=" + EphiPassword +
                                           ";Host=" + PgHost +
                                           ";Port=" + (String.IsNullOrEmpty(PgPort) ? "5432" : PgPort) +
                                           ";Database=" + PgDb +
                                           ";Initial Schema=" + (String.IsNullOrEmpty(PgSchema) ? "public" : PgSchema);
                //Trace.WriteLine("PostgreSQL connection string is: " + PgConnectionString);
                CdrDb = new CdrDbInterface(PgConnectionString, ConnectionArgumentType.ConnectionString);
            }
            else
            {
                // TODO validate _PgConnectionName
                CdrDb = new CdrDbInterface(PgConnectionStringName, ConnectionArgumentType.ConnectionStringName);
            }

            try  // Validate the context connection
            {
                int RecordCount = CdrDb.Context.AggregationRuns.Count();
                Trace.WriteLine("PostgreSQL connection complete to database " + CdrDb.Context.Connection.Database + " on host " + CdrDb.Context.Connection.DataSource);
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("PostgreSQL connection failed: " + e.Message);
                return false;
            }
        }

        private bool InitializeRun()
        {
            bool Initialized = true;

            Initialized &= InitializeMongoConnection();
            Initialized &= InitializePgConnection();

            // Initialize MongoDb Collections
            DiagnosisCollection = MongoCxn.Db.GetCollection<MongodbDiagnosisEntity>("diagnosis");
            ReferenceTerminologyCollection = MongoCxn.Db.GetCollection<MongodbReferenceTerminologyEntity>("referenceterminology");

            //CsvWriter = new StreamWriter("Performance_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".csv");
            //CsvWriter.AutoFlush = true;

            Trace.WriteLine("Populating diagnosis ICD-10-CM code meanings.");

            return Initialized;
        }

        public void AddAllIcd10CodeMeanings()
        {
            InitializeRun();

            var Icd10Query = CdrDb.Context.Diagnoses.Where(x => 
                                                                x.DiagCode.CodeSystem == "ICD-10-CM"
                                                             && x.DiagCode.CodeMeaning == ""
                                                          ).Take(100);

            while (CdrDb.Context.Diagnoses.Count(x => x.DiagCode.CodeSystem == "ICD-10-CM" && x.DiagCode.CodeMeaning == "") > 0)
            {
                foreach (Diagnosis DiagnosisRecord in Icd10Query)
                {
                    var y = ReferenceTerminologyCollection.Find(x =>
                                                                     x.Terminology == "95538641"
                                                                  && x.Code == DiagnosisRecord.DiagCode.Code
                                                               );
                    String TerminologyText = y.FirstOrDefault().Text;
                    DiagnosisRecord.DiagCode.CodeMeaning = TerminologyText;
                }
                CdrDb.Context.SubmitChanges();
            }

        }
    }
}
