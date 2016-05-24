using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ProcessGlobal
{
    /// <summary>
    /// Contains globally scoped resources that are intended to be available as singleton in multiple projects of a process
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// Source name must be registered before using this (as when a service is installed).  Suggested usage: 
        /// <para>using System.Diagnostics;</para>
        /// <para>using ProcessGlobal; (project reference to ProcessGlobal)</para>
        /// <para>Global.EventLog = new EventLog("Application", ".", SourceName);</para>
        /// <para>Global.EventLog.WriteEntry(String Msg, EventLogEntryType, EventId, Category);</para>
        /// </summary>
        public static EventLog EventLog;
    }
}
