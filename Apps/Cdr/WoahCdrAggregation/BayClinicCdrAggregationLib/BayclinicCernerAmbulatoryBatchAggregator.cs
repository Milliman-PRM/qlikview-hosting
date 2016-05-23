using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CdrContext;
using CdrDbLib;
using Devart.Data.Linq;
using MongoDbWrap;

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
        private bool Initialized = false;
        private AggregationRun AggregationRun;
        private DataFeed FeedObject;
        private MongoDbConnection MongoCxn;

        public BayClinicCernerAmbulatoryBatchAggregator(String PgConnectionName = null)
        {
            if (String.IsNullOrEmpty(PgConnectionName))
            {
                PgConnectionName = PgConnectionStringName;
            }

            CdrDb = new CdrDbInterface(PgConnectionName, ConnectionArgumentType.ConnectionStringName);
            MongoCxn = new MongoDbConnection(NewBayClinicAmbulatoryMongoCredentialConfigFile, NewBayClinicAmbulatoryMongoCredentialSection);
        }

        public bool Initialize()
        {
            if (!Initialized)
            {
                AggregationRun = GetNewAggregationRun();
                Initialized = AggregationRun.dbid > 0;
            }

            ListAllPatients();

            return Initialized;
        }

        public void ListAllPatients()
        {
            List<string> x = MongoCxn.GetCollectionNames();

            Dictionary<String, String> SearchFilter = new Dictionary<String, String>
            {
                { "last_name", "{$ne: ''}" }
            };
            MongoCxn.GetDocuments("person", SearchFilter);
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
