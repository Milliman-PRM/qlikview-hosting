using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Milliman.Common;

namespace QvReportReductionLib
{
    public class ProcessManager
    {
        private bool _StopSignal;  // wrapped by the thread safe StopSignal property
        private Object ObjectStateLock = null;
        private Thread MainServiceWorkerThread = null;

        private string RootPath = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProcessManager()
        {
            ObjectStateLock = new object();

            StopSignal = false;
            MainServiceWorkerThread = new Thread(WorkerThreadMain);
        }

        /// <summary>
        /// Thread safe access to the stop signal
        /// </summary>
        private bool StopSignal
        {
            get
            {
                bool ReturnVal;
                lock (ObjectStateLock) { ReturnVal = _StopSignal; }
                return ReturnVal;
            }
            set
            {
                lock (ObjectStateLock) { _StopSignal = value; }
            }
        }

        public bool ThreadAlive
        {
            get {
                return (MainServiceWorkerThread != null && (MainServiceWorkerThread.ThreadState == System.Threading.ThreadState.Running || MainServiceWorkerThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
                       ? true : false;
            }
        }

        public bool Start(ProcessManagerConfiguration ProcessConfig)
        {
            Trace.WriteLine("Starting the processing of class " + this.GetType().Name);

            MainServiceWorkerThread.Start(ProcessConfig);
            return true;
        }

        /// <summary>
        /// Entry point intended for the main application to request this object to gracefully stop all processing under its control
        /// Note this is not called on the worker thread so any member variable also accessed by the worker thread must be protected
        /// </summary>
        /// <param name="WaitMs"></param>
        /// <returns></returns>
        public bool Stop(int WaitMs = 0)
        {
            StopSignal = true;

            if (WaitMs > 0)
            {
                // TODO Wait as long as WaitMs for the thread to stop
                Thread.Sleep(WaitMs);
            }

            return !ThreadAlive;
        }

        private void WorkerThreadMain(Object Arg)
        {
            ProcessManagerConfiguration ProcessConfig = Arg as ProcessManagerConfiguration;

            Trace.WriteLine("In " + this.GetType().Name + ".WorkerThreadMain()");
            while (!StopSignal)
            {
                foreach (string TaskFolderName in Directory.EnumerateDirectories(ProcessConfig.RootPath)
                                                           .Where(d => ProcessManager.FolderContainsAReductionRequest(d))
                                                           .OrderBy(d => Directory.GetLastWriteTime(d))
                                                           .Take(ProcessConfig.MaxConcurrentTasks - 1))
                {
                    {
                        Trace.WriteLine("Found task in folder " + TaskFolderName);
                    }
                }

                Thread.Sleep(2000);
            }
        }

        public static bool FolderContainsAReductionRequest(string FolderName)
        {
            bool ReturnVal = true;  // start true because of the &= logic below

            ReturnVal &= File.Exists(Path.Combine(FolderName, "request_complete.txt"));
            ReturnVal &= !File.Exists(Path.Combine(FolderName, "processing_complete.txt"));
            ReturnVal &= !File.Exists(Path.Combine(FolderName, "delete_me.txt"));

            // Other things to look for, should cause false return if found.  
            //  For each job executing by QV server a “*_running.txt” file will be created
            //  For each job completed by QV server a “*_completed.txt” file will be created

            return ReturnVal;
        }
    }
}