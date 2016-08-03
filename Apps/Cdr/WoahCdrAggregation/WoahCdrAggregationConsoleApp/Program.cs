using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BayClinicCdrAggregationLib;
using System.Threading;

namespace WoahCdrAggregationConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            BayClinicAggregationWorkerThreadManager BcLib = new BayClinicAggregationWorkerThreadManager();

            BcLib.StartThread(false);

            Thread.Sleep(200);  // milliseconds
            BcLib.EndThread();
        }
    }
}
