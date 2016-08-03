using System;
using System.Threading;
using BayClinicCernerAmbulatory;
using System.Diagnostics;


namespace BayClinicCdrAggregationLib
{
    /// <summary>
    /// Manages the control of a worker thread to execute data aggregation 
    /// </summary>
    public class BayClinicAggregationWorkerThreadManager
    {
        private struct ThreadArguments
        {
            public String ConnectionStringName;
            public bool ClearMongo;
        }

        private bool _EndThreadSignal;
        private bool _ClearMongo;
        BayClinicCernerAmbulatoryBatchAggregator Aggregator = null;

        /// <summary>
        /// Property that encapsulates thread safe access to the end thread signal
        /// </summary>
        private bool EndThreadSignal
        {
            set
            {
                Mutx.WaitOne();
                _EndThreadSignal = value;
                Mutx.ReleaseMutex();
            }
            get
            {
                Mutx.WaitOne();
                bool ReturnVal = _EndThreadSignal;
                Mutx.ReleaseMutex();
                return ReturnVal;
            }
        }

        /// <summary>
        /// Property that encapsulates thread safe access to the end thread signal
        /// </summary>
        private bool ClearMongo
        {
            set
            {
                Mutx.WaitOne();
                _ClearMongo = value;
                Mutx.ReleaseMutex();
            }
            get
            {
                Mutx.WaitOne();
                bool ReturnVal = _ClearMongo;
                Mutx.ReleaseMutex();
                return ReturnVal;
            }
        }

        private Thread WorkerThd;
        private Mutex Mutx;

        /// <summary>
        /// Instantiates member objects
        /// </summary>
        public BayClinicAggregationWorkerThreadManager()
        {
            Mutx = new Mutex();
            WorkerThd = new Thread(ThreadMain);
        }

        /// <summary>
        /// Starts the encapsulated worker thread
        /// </summary>
        /// <param name="PgConnectionName">Name of a PostgreSQL connection string to use. If null, a value is taken from ConfigurationManager.AppSettings["CdrPostgreSQLConnectionString"]</param>
        public void StartThread(bool ClearMongoArg, String PgConnectionName = null)
        {
            String TraceFileName = "TraceLog_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";
            Trace.Listeners.Add(new TextWriterTraceListener(TraceFileName));
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
            Trace.WriteLine("Application launched " + DateTime.Now.ToString());

            EndThreadSignal = false;
            ClearMongo = ClearMongoArg;
            WorkerThd.Start( new ThreadArguments { ClearMongo = ClearMongo, ConnectionStringName = PgConnectionName } );
        }

        /// <summary>
        /// Sets the signal for the worker thread to finish current processing and end itself
        /// </summary>
        /// <param name="WaitMs">Time to wait for the thread to be ended before returning.  If not provided the function returns immediately</param>
        /// <returns>boolean indicating whether the thread is ended at the time of return</returns>
        public bool EndThread(int WaitMs = 0)
        {
            EndThreadSignal = true;
            if (Aggregator != null)
            {
                Aggregator.EndThreadSignal = true;
            }

            return WorkerThd.Join(WaitMs);
        }

        /// <summary>
        /// returns the count of patient records aggregated so far
        /// </summary>
        /// <returns></returns>
        public String GetProgressSummary()
        {
            if (Aggregator != null)
            {
                return Aggregator.NewPatientCounter.ToString() + " inserted, " + 
                       Aggregator.MergedPatientCounter.ToString() + " merged / " +
                       Aggregator.PersonCounter.ToString() + " processed";
            }

            return "";
        }

        /// <summary>
        /// Worker thread entry point, initiates processing
        /// </summary>
        /// <param name="ThreadArg"></param>
        private void ThreadMain(object ThreadArg)
        {
            ThreadArguments Args = (ThreadArguments) ThreadArg;
            bool Success;

#if true // do once
            Aggregator = new BayClinicCernerAmbulatoryBatchAggregator(Args.ConnectionStringName);
            Success = Aggregator.AggregateAllAvailablePatients(Args.ClearMongo);  // TODO for normal production the argument should be false
            Aggregator = null;

#else  // repeat
            while (!EndThreadSignal)
            {
            Aggregator = new BayClinicCernerAmbulatoryBatchAggregator(PgCxnName);
                Success = Aggregator.AggregateAllAvailablePatients(true);  // TODO for production the argument should be false
                Aggregator = null;
                if (!EndThreadSignal)
                {
                    Thread.Sleep(1 * 60 * 1000);  // milliseconds
                }
            }
#endif

        }

    }
}
