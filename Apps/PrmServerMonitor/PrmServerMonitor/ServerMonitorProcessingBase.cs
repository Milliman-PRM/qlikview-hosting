using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrmServerMonitor
{
    public class ServerMonitorProcessingBase
    {
        TextWriterTraceListener TraceFile;

        public void EstablishTraceLog()
        {
            TraceFile = new TextWriterTraceListener("Trace_" + this.GetType().Name + "_" + DateTime.Now.ToString("yyyyMMdd-HHmmss-ffff") + ".log");
            Trace.AutoFlush = true;
            Trace.Listeners.Add(TraceFile);
            Thread.Sleep(2);  // try to prevent attempts to create multiple log files with the same date/time.  
        }

        public void CloseTraceLog()
        {
            if (TraceFile != null && Trace.Listeners.Contains(TraceFile))
            {
                TraceFile.Flush();
                Trace.Listeners.Remove(TraceFile);
                TraceFile.Close();
                TraceFile = null;
                Thread.Sleep(2);  // try to prevent attempts to create multiple log files with the same date/time.  
            }
        }

    }
}
