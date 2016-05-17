using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CdrContext;

namespace BayClinicCdrAggregationLib
{
    public class BcCdrAggregationLib
    {
        private bool EndThreadSignal;
        private Thread WorkerThd;
        private Mutex Mutx;
        private CdrDataContext CdrContext;

        /// <summary>
        /// Instantiates member objects
        /// </summary>
        public BcCdrAggregationLib()
        {
            EndThreadSignal = false;
            WorkerThd = new Thread(ThreadMain);
            Mutx = new Mutex();
            String CxnStr = "";
            CdrContext = new CdrDataContext(CxnStr);
        }

        /// <summary>
        /// Starts the encapsulated worker thread
        /// </summary>
        public void StartThread()
        {
            WorkerThd.Start();
        }

        /// <summary>
        /// Sets the signal for the worker thread to finish current processing and end itself
        /// </summary>
        /// <param name="WaitMs">Time to wait for the thread to be ended before returning.  If not provided the function returns immediately</param>
        /// <returns>boolean indicating whether the thread is ended at the time of return</returns>
        public bool EndThread(int WaitMs = 0)
        {
            Mutx.WaitOne();
            EndThreadSignal = true;
            Mutx.ReleaseMutex();

            return WorkerThd.Join(WaitMs);
        }

        private void ThreadMain()
        {
            while (!EndThreadSignal)
            {
                Thread.Sleep(1000);


            }
        }

    }
}
