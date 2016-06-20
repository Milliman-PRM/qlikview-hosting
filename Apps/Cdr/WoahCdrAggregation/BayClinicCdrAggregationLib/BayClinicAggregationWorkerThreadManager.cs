using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BayClinicCernerAmbulatory;

namespace BayClinicCdrAggregationLib
{
    /// <summary>
    /// Manages the control of a worker thread to execute data aggregation 
    /// </summary>
    public class BayClinicAggregationWorkerThreadManager
    {
        private bool _EndThreadSignal;
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
        public void StartThread(String PgConnectionName = null)
        {
            EndThreadSignal = false;
            WorkerThd.Start(PgConnectionName);
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
        public long GetNumberOfPatientsDone()
        {
            if (Aggregator != null)
            {
                return Aggregator.PatientCounter;
            }

            return 0;
        }

        /// <summary>
        /// Worker thread entry point, initiates processing
        /// </summary>
        /// <param name="ThreadArg"></param>
        private void ThreadMain(object ThreadArg)
        {
            String PgCxnName = (String)ThreadArg;
            bool Success;

#if true // do once
            Aggregator = new BayClinicCernerAmbulatoryBatchAggregator(PgCxnName);
            Success = Aggregator.AggregateAllAvailablePatients(true);  // TODO for production the argument should be false
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
