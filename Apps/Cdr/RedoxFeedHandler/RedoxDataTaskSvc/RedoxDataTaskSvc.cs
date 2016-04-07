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

namespace RedoxDataTaskSvc
{
    public partial class RedoxDataTaskSvc: ServiceBase
    {
        SchedulingMsgProcessor SchedulingProcessor;

        public RedoxDataTaskSvc()
        {
            InitializeComponent();

            SchedulingProcessor = new SchedulingMsgProcessor();
        }

        protected override void OnStart(string[] args)
        {
            // Wait for debugger attachment
            for (int i=0; i<12; i++)
            {
                Thread.Sleep(1000);
            }

            SchedulingProcessor.StartThread();
        }

        protected override void OnStop()
        {
            int StopWaitTimeMs = 5000;

            if (SchedulingProcessor.EndThread(StopWaitTimeMs))
            {

            }
        }
    }
}
