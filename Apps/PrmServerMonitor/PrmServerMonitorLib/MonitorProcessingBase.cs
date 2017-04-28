/*
 * CODE OWNERS: Tom Puckett, Ben Wyatt
 * OBJECTIVE: A class containing common functionality for all dedicated task processing classes.  
 * DEVELOPER NOTES: All dedicated processing classes should be derived from this base to uniformly inherit common functionality
 */

using System;
using System.Diagnostics;
using System.Threading;

namespace PrmServerMonitorLib
{
    public class MonitorProcessingBase
    {
        TextWriterTraceListener TraceFile;
        bool LifetimeTrace = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LifetimeTraceArg"></param>
        protected MonitorProcessingBase(bool LifetimeTraceArg)
        {
            LifetimeTrace = LifetimeTraceArg;
        }

        /// <summary>
        /// Opens a file based trace log destination named for the calling derived class.  
        /// </summary>
        public void EstablishTraceLogFile()
        {
            if (TraceFile == null)
            {
                TraceFile = new TextWriterTraceListener("Trace_" + this.GetType().Name + "_" + DateTime.Now.ToString("yyyyMMdd-HHmmss-ffff") + ".log");
                Trace.AutoFlush = true;
                Trace.Listeners.Add(TraceFile);
                Thread.Sleep(2);  // Prevent attempts to create multiple log files with the same date/time stamp in the name.  
            }
        }

        /// <summary>
        /// Closes a previously opened file based trace log destination.  
        /// </summary>
        public void CloseTraceLogFile(bool ForceCloseTrace = false)
        {
            if (!LifetimeTrace || ForceCloseTrace)
            {
                if (TraceFile != null && Trace.Listeners.Contains(TraceFile))
                {
                    TraceFile.Flush();
                    Trace.Listeners.Remove(TraceFile);
                    TraceFile.Close();
                    TraceFile = null;
                    Thread.Sleep(2);  // try to prevent attempts to create multiple log files with the same date/time.  
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

    }
}
