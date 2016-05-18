using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CdrDbLib;
using CdrContext;

namespace BayClinicCdrAggregationLib
{
    public class BayClinicCernerAmbulatoryExtractAggregator
    {
        private bool _EndThreadSignal;

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
        private CdrDbInterface CdrDb;
        private const String FeedIdentity = "BayClinicCernerAmbulatoryExtract";
        private const String PgConnectionStringName = "Cdr_WoahConnection";

        /// <summary>
        /// Instantiates member objects
        /// </summary>
        public BayClinicCernerAmbulatoryExtractAggregator()
        {
            EndThreadSignal = false;
            WorkerThd = new Thread(ThreadMain);
            Mutx = new Mutex();
        }

        /// <summary>
        /// Starts the encapsulated worker thread
        /// </summary>
        public void StartThread(String PgConnectionName = PgConnectionStringName)
        {
            CdrDb = new CdrDbInterface(PgConnectionName, ConnectionArgumentType.ConnectionStringName);

            DataFeed Feed = CdrDb.EnsureFeed(FeedIdentity);
            long x = Feed.dbid;  // test

            WorkerThd.Start();
        }

        /// <summary>
        /// Sets the signal for the worker thread to finish current processing and end itself
        /// </summary>
        /// <param name="WaitMs">Time to wait for the thread to be ended before returning.  If not provided the function returns immediately</param>
        /// <returns>boolean indicating whether the thread is ended at the time of return</returns>
        public bool EndThread(int WaitMs = 0)
        {
            EndThreadSignal = true;

            return WorkerThd.Join(WaitMs);
        }

        private void ThreadMain(object ThreadArg)
        {
            DataFeed Feed = CdrDb.EnsureFeed(FeedIdentity);
            long x = Feed.dbid;  // test

            while (!EndThreadSignal)
            {
                Thread.Sleep(3000);
                Patient NewPatient = new Patient
                {
                    NameLast = "xyz"
                };
                
            }
        }

    }
}
