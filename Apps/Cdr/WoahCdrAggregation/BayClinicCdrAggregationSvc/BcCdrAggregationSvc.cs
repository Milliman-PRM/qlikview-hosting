using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BayClinicCdrAggregationLib;

namespace BayClinicCdrAggregationSvc
{
    public partial class BcCdrAggregationSvc : ServiceBase
    {
        BayClinicCernerAmbulatoryExtractAggregator LibInstance = new BayClinicCernerAmbulatoryExtractAggregator();

        public BcCdrAggregationSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            for (int DelayCount = 0; DelayCount < 9; DelayCount++)
            {
                Thread.Sleep(1000);
            }
            LibInstance.StartThread();
            bool Ended = LibInstance.EndThread();
        }

        protected override void OnStop()
        {
        }
    }
}
