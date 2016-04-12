using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RedoxCacheDbLib;
using RedoxCacheDbContext;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedoxDataTaskSvc
{
    public class SchedulingMsgProcessor
    {
        int SecondsPauseBetweenQuery = 5;
        Thread Thd;
        RedoxCacheInterface Db;
        Mutex Mutx;
        bool EndThreadSignal;
        QueryInterface I;
        String OutputFolderName = @"C:\RedoxFeedHandler\RedoxFeedTest";

        public SchedulingMsgProcessor()   // Constructor
        {
            Thd = new Thread(ThreadMain);
            I = new QueryInterface();
            Mutx = new Mutex();

            Directory.CreateDirectory(OutputFolderName);
        }

        public int StartThread()
        {
            if (!Thd.IsAlive)
            {
                Mutx.WaitOne();
                EndThreadSignal = false;
                Mutx.ReleaseMutex();

                string CxStr = Environment.MachineName == "IN-PUCKETTT" ?
                    ConfigurationManager.ConnectionStrings["RedoxCacheContextConnectionStringPort5433"].ConnectionString :
                    ConfigurationManager.ConnectionStrings["RedoxCacheContextConnectionStringPort5432"].ConnectionString;

                Db = new RedoxCacheInterface(CxStr);

                Thd.Start();  // if no argument is expected by thread entry point
                //Thd.Start(4500);  // if argument is expected
            }

            return Thd.ManagedThreadId;
        }

        public bool EndThread(int StopWaitTimeMs)
        {
            Mutx.WaitOne();
            EndThreadSignal = true;
            Mutx.ReleaseMutex();

            Thd.Join(StopWaitTimeMs);
            if (Thd.IsAlive)
            {
                Thd.Abort();
            }

            return !Thd.IsAlive;
        }

        private void ThreadMain()
        // for argument, signature would be:> private void ThreadMain(Object Obj)   and pass value in Thd.Start(xyz)
        {
            Mutx.WaitOne();

            // The worker thread begins here
            for ( ; !EndThreadSignal ; )
            {
                Mutx.ReleaseMutex();

                List<Scheduling> Messages = Db.GetSchedulingRecords(true, 2);  // Normally second arg should be 1 (default value)
                Trace.WriteLine("Query found " + Messages.Count.ToString() + " records from database query");

                foreach (Scheduling S in Messages)
                {
                    JObject QueryPayloadObject, ClinicalSummaryObject;

                    QueryPayloadObject = GetClinicalSummaryQueryObject(S);

// TODO Temporary debug output
                    using (StreamWriter W = new StreamWriter(Path.Combine(@"C:\RedoxFeedHandler\RedoxFeedTest", "Query.txt")))
                    {
                        W.WriteLine(JsonConvert.SerializeObject(QueryPayloadObject, Formatting.Indented));
                    }

                    bool Success = GetClinicalSumamryResponseObject(QueryPayloadObject, out ClinicalSummaryObject);

                    if (Success) 
                    {
                        String OutputFileName = CreateClinicalSummaryFileName(S, ClinicalSummaryObject);

                        using (StreamWriter Writer = new StreamWriter(Path.Combine(OutputFolderName, OutputFileName)))
                        {
                            //string ClinicalSummaryString = JsonConvert.SerializeObject(ClinicalSummaryObject,Formatting.Indented);
                            JObject FileContentObj = new JObject(new JProperty("Scheduling", JObject.Parse(S.Content)), new JProperty("ClinicalSummary", ClinicalSummaryObject));
                            Writer.Write(JsonConvert.SerializeObject(FileContentObj,Formatting.Indented ));
                        }

                        RemoveTask(S);
                    }
                }

                Thread.Sleep(SecondsPauseBetweenQuery * 1000);

                Mutx.WaitOne();  // need this so the for loop test condition (!EndThreadSignal) can evaluate safely
            }

            Mutx.ReleaseMutex();
        }

        public bool IsThreadAlive
        {
            get
            {
                return Thd.IsAlive;
            }
        }

        private JObject GetClinicalSummaryQueryObject(Scheduling S)
        {
            String QueryPatientId;

            JObject DocJson = JObject.Parse(S.Content);
            // or   DocJson = JsonConvert.DeserializeObject<JObject>(ContentString);

            IEnumerable<JObject> PatientIdentifiersArray = DocJson.Property("Patient").Value["Identifiers"].Values<JObject>();
            foreach (JObject IdentifierObj in PatientIdentifiersArray)
            {
                if (IdentifierObj.Value<String>("IDType") == S.SourceFeed.BestIdType)
                {
                    QueryPatientId = IdentifierObj.Value<String>("ID");
                    break;
                }
                // TODO What if an ID with IDType of BestIdType is not found, how does this affect the query?
            }

            // build Json query object
            JProperty MetaPrpt = new JProperty("Meta", new JObject(
                new JProperty("DataModel", "Clinical Summary"),
                new JProperty("EventType", "Query"),
                new JProperty("EventDateTime", DateTime.UtcNow),
/*TODO update*/ new JProperty("Test", true),
                new JProperty("Destinations", new JObject[] {
                    new JObject(
#if true // This is the permanent code
                        new JProperty("ID", S.SourceFeed.QueryDestinationId.ToString()),
                        new JProperty("Name", S.SourceFeed.QueryDestinationName)
#else
/*TODO replace this*/   new JProperty("ID", "ef9e7448-7f65-4432-aa96-059647e9b357"),
/*TODO replace this*/   new JProperty("Name", "Clinical Summary Endpoint")
#endif              
                    ) 
                })
            ));
            
            JProperty PatientPrpt = new JProperty("Patient", new JObject(
                new JProperty("Identifiers", new JObject[] {
                    new JObject(
#if false
                        new JProperty("ID", QueryPatientId),
                        new JProperty("IDType", S.SourceFeed.BestIdType)
#else
/*TODO replace this*/   new JProperty("ID", "ffc486eff2b04b8^^^&1.3.6.1.4.1.21367.2005.13.20.1000&ISO"),    // TODO change this for production
/*TODO replace this*/   new JProperty("IDType", "NIST")     // TODO change this for production
                        // This is a hard coded test patient query copied from the Redox web site
#endif
                    )
                })
            ));

            JObject QueryPayloadObject = new JObject(MetaPrpt, PatientPrpt);

            return QueryPayloadObject;
        }

        private bool GetClinicalSumamryResponseObject(JObject QueryPayloadObject, out JObject IndentedCcd)
        {
            try
            {
                IndentedCcd = I.QueryForClinicalSummary(QueryPayloadObject);
            }
            catch (Exception /*e*/)
            {
                IndentedCcd = new JObject();
                return false;
            }

            return true;
        }

        private bool RemoveTask(Scheduling S)
        {
            return Db.RemoveSchedulingRecord(S);
        }

        private String CreateClinicalSummaryFileName(Scheduling S, JObject ClinicalSummary)
        {
            JProperty ClinicalSummaryMeta = ClinicalSummary.Property("Meta");
            JObject Transmission = ClinicalSummaryMeta.Value["Transmission"].Value<JObject>();
            String Name = ClinicalSummaryMeta.Value["DataModel"].Value<String>().Replace(" ","");
            Name += "_";
            Name += S.SourceFeedName.Replace(" ","");
            Name += "_";
            Name += Transmission.Value<String>("ID");  // TODO Maybe this should be Message ID instead of Transmission ID
            Name = Path.ChangeExtension(Name, "json");

            return Name;
        }
    }
}
