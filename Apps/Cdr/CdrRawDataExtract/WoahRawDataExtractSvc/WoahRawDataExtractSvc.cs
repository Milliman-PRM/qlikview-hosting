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

namespace WoahRawDataExtractSvc
{
    public partial class WoahRawDataExtractSvc : ServiceBase
    {
        RedoxRawDataExtractionManager RedoxExtractManager;

        public WoahRawDataExtractSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Pause enough time to attach debugger to this process
            int PauseTime = 10;
            for (int i=0; i<PauseTime ; i++)
            {
                Trace.WriteLine(String.Format("Paused {0} seconds of {1}", i, PauseTime));
                Thread.Sleep(1000);
            }

            RedoxExtractManager = new RedoxRawDataExtractionManager();
            RedoxExtractManager.StartThread();  // can override default settings, see signature
        }

        protected override void OnStop()
        {
            RedoxExtractManager.EndThread(5000);
            RedoxExtractManager = null;
        }
    }
}
