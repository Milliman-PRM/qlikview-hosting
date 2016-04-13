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
    /// <summary>
    /// Implements and interfaces to a worker thread that processes tasks based on scheduling messages from the Redox task queue
    /// </summary>
    public class SchedulingMsgProcessor
    {
        // instance variable declarations
        int SecondsPauseBetweenQuery = 5;
        Thread Thd;
        RedoxCacheDbInterface Db;
        Mutex Mutx;
        bool EndThreadSignal;
        RedoxQueryInterface RedoxQueryIfc;
        String OutputFolderName = @"C:\RedoxFeedHandler\RedoxFeedTest";

        /// <summary>
        /// Constructor, instantiates / initializes member resources.  
        /// </summary>
        public SchedulingMsgProcessor()   // Constructor
        {
            Thd = new Thread(ThreadMain);
            RedoxQueryIfc = new RedoxQueryInterface();
            Mutx = new Mutex();

            Directory.CreateDirectory(OutputFolderName);
        }

        /// <summary>
        /// public method that causes the encapsulated worker thread to start running
        /// </summary>
        /// <returns></returns>
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

                Db = new RedoxCacheDbInterface(CxStr);

                Thd.Start();  // if no argument is expected by thread entry point
                //Thd.Start(4500);  // if argument is expected
            }

            return Thd.ManagedThreadId;
        }

        /// <summary>
        /// public method that attempts to end the encapsulated worker thread gracefully within a specified duration, then aborts the thread if still running
        /// </summary>
        /// <param name="StopWaitTimeMs"></param>
        /// <returns>bool indicating whether the thread is alive at the time of return</returns>
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

            return !IsThreadAlive;
        }

        /// <summary>
        /// The entry point for the worker thread, contains the thread's main logic
        /// </summary>
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
                    // TODO Should there be a conditional test on S.EventType so selective handling or filtering is possible?

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

        /// <summary>
        /// public property that returns boolean representing alive state of the encapsulated worker thread
        /// </summary>
        public bool IsThreadAlive
        {
            get
            {
                return Thd.IsAlive;
            }
        }

        /// <summary>
        /// Generates a Json query object intended for Redox for a clinical summary document based on an existing scheduling message
        /// </summary>
        /// <param name="S"></param>
        /// <returns>Query object to be serialized into the body of a web request to Redox</returns>
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
                        new JProperty("ID", S.SourceFeed.QueryDestinationId.ToString()),
                        new JProperty("Name", S.SourceFeed.QueryDestinationName)
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

        /// <summary>
        /// Encapsulates the query execution to Redox for a clinical summary document
        /// </summary>
        /// <param name="QueryPayloadObject">The entire query body</param>
        /// <param name="IndentedCcd">The clinical summary returned by the query</param>
        /// <returns>bool indicating success of the query</returns>
        private bool GetClinicalSumamryResponseObject(JObject QueryPayloadObject, out JObject IndentedCcd)
        {
            try
            {
                IndentedCcd = RedoxQueryIfc.QueryForClinicalSummary(QueryPayloadObject);
            }
            catch (Exception /*e*/)
            {
                IndentedCcd = new JObject();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes a Scheduling record from persistence
        /// </summary>
        /// <param name="S">Represents the record to be removed</param>
        /// <returns>boolean indicating success of the remove operation</returns>
        private bool RemoveTask(Scheduling S)
        {
            return Db.RemoveSchedulingRecord(S);
        }

        /// <summary>
        /// Generates an appropriate name for a file in which a clinical summary json document will be stored
        /// </summary>
        /// <param name="S"></param>
        /// <param name="ClinicalSummary">The entire clinical summary JObject as received from Redox</param>
        /// <returns>A file name with extension, but no path</returns>
        private String CreateClinicalSummaryFileName(Scheduling S, JObject ClinicalSummary)
        {
            JProperty ClinicalSummaryMeta = ClinicalSummary.Property("Meta");
            JObject Transmission = ClinicalSummaryMeta.Value["Transmission"].Value<JObject>();
            String Name = ClinicalSummaryMeta.Value["DataModel"].Value<String>().Replace(" ","");
            Name += "_";
            Name += S.SourceFeedName;
            Name += "_";
            Name += Transmission.Value<String>("ID");  // TODO Maybe this should be Message ID instead of Transmission ID
            Name = Name.Replace(" ", "");
            Name = Path.ChangeExtension(Name, "json");

            return Name;
        }
    }
}
