using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportingCommon
{
    public class QlikviewAuditEvent : QlikviewEventBase
    {
        public DateTime ServerStarted { get; set; }
        public string Report { get; set; }
        // Group
        public string EventType { get; set; }
        public string Message { get; set; }
        public bool IsReduced { get; set; }

            /// <summary>
        /// Constructor, inferring member variable values from an instance of class QlikviewAuditLogEntry
        /// </summary>
        /// <param name="LogEntry"></param>
        /// <param name="QlikServerTimeZone">A TimezoneInfo object representing the time zone of the QlikView Server that generated the log file</param>
        /// <param name="UseDaylightSavings">A flag indicating whether to convert timestamps with or without consideration of daylight savings time</param>
        /// <param name="QvDocumentRootPath">The root path of QlikView documents, as expected to be contained in paths of QVW file names in log files</param>
        public QlikviewAuditEvent(QlikviewAuditLogEntry LogEntry, TimeZoneInfo QlikServerTimeZone, bool UseDaylightSavings, string QvDocumentRootPath)
        {
            if (UseDaylightSavings)
            {
                TimeStamp = TimeZoneInfo.ConvertTimeToUtc(LogEntry.Timestamp, QlikServerTimeZone);
            }
            else
            {
                TimeStamp = LogEntry.Timestamp - QlikServerTimeZone.BaseUtcOffset;
            }
            Group = GetGroup(LogEntry.Document, QvDocumentRootPath);
            User = LogEntry.User.Replace(@"Custom\", "").Replace(@"custom\", "");

            ServerStarted = LogEntry.ServerStarted;
            Report = GetReportName(LogEntry.Document);
            EventType = LogEntry.EventType;
            Message = LogEntry.Message;
            IsReduced = LogEntry.Document.IndexOf(@"\reducedcachedqvws") > 0;
        }

    }
}
