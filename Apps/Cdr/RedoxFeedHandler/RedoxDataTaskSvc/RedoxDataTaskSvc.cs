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
        }
    
        /// <summary>
        /// When service is started or continued, this function is called, similar to an application's main() function
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            int SecondsToPauseBeforeStarting = 12;
            // Wait long enough for me to attach the debugger before launching the worker thread
            for (int i = 0; i < SecondsToPauseBeforeStarting ; i++)
            {
                Trace.WriteLine("Service is paused " + i.ToString() + " seconds, waiting for " + SecondsToPauseBeforeStarting.ToString());
                Thread.Sleep(1000);
            }

            // Launch the processing of Redox Scheduling messages from the PG database
            SchedulingProcessor = new SchedulingMsgProcessor();
            SchedulingProcessor.StartThread();

        }

        protected override void OnStop()
        {
            int StopWaitTimeMs = 5 * 1000;

            if (!SchedulingProcessor.EndThread(StopWaitTimeMs))
            {
                // TODO Handle the failure to stop the worker thread
            }

            SchedulingProcessor = null;
        }
    }
}
