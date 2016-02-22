using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReportingCommon
{
    /// <summary>
    /// This class represents a user's usage of Qlikview functionality, inferred from an instance of class QlikviewLogFileEntry.  
    /// </summary>
    public class QlikviewSessionEvent : QlikviewEventBase
    {
        public string Report { get; set; }
        public TimeSpan SessionLength { get; set; }
        public QvSessionEndReason SessionEndReason { get; set; }
        public bool IsReduced { get; set; }

        /// <summary>
        /// Translates this instance's value of the reason enumeration into a string representation
        /// </summary>
        /// <returns></returns>
        public string GetReasonString()
        {
            switch (this.SessionEndReason)
            {
                case QvSessionEndReason.SESSIONTIMEOUT:
                    return "Timeout";
                case QvSessionEndReason.CLIENTCLOSE:
                    return "ClientClose";
                case QvSessionEndReason.OTHER:
                    return "Other";
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// Constructor, inferring member variable values from an instance of class QlikViewLogFileEntry
        /// </summary>
        /// <param name="LogEntry"></param>
        /// <param name="QlikServerTimeZone">A TimezoneInfo object representing the time zone of the QlikView Server that generated the log file</param>
        /// <param name="UseDaylightSavings">A flag indicating whether to convert timestamps with or without consideration of daylight savings time</param>
        /// <param name="QvDocumentRootPath">The root path of QlikView documents, as expected to be contained in paths of QVW file names in log files</param>
        public QlikviewSessionEvent(QlikviewSessionLogEntry LogEntry, TimeZoneInfo QlikServerTimeZone, bool UseDaylightSavings, string QvDocumentRootPath)
        {
            if (QlikServerTimeZone.SupportsDaylightSavingTime && !UseDaylightSavings)
            {
                TimeStamp = LogEntry.SessionStart - QlikServerTimeZone.BaseUtcOffset;
            }
            else
            {
                TimeStamp = TimeZoneInfo.ConvertTimeToUtc(LogEntry.SessionStart, QlikServerTimeZone);
            }
            IsReduced = LogEntry.Document.IndexOf(@"\reducedcachedqvws") > 0;
            Group = GetGroup(LogEntry.Document, QvDocumentRootPath);
            Report = GetReportName(LogEntry.Document);
            User = LogEntry.IdentifyingUser.Replace(@"Custom\", "").Replace(@"custom\", "");
            SessionLength = LogEntry.SessionDuration;
            SessionEndReason = GetExitReason(LogEntry.ExitReason);
            Browser = GetBrowser(LogEntry.ClientType);
        }

        /// <summary>
        ///     Extracts the browser type and version from the raw ClientType string
        /// </summary>
        /// <param name="ClientType">Client type string as read from the Qlikview log file</param>
        /// <returns>String of browser type</returns>
        public static string GetBrowser(string ClientType)
        {
            string BrowserType = "Unknown";
            string BrowserVersion = "Unknown";

            string[] ClientTypeParts = ClientType.Split(' ');

            if (ClientTypeParts.Length > 4)
            {
                string[] BrowserParts = ClientTypeParts[3].Split('.');
                if (!string.IsNullOrEmpty(BrowserParts[1]))
                {
                    BrowserType = BrowserParts[1].ToUpper();
                }
                if (!string.IsNullOrEmpty(ClientTypeParts[4]))
                {
                    BrowserVersion = ClientTypeParts[4];
                }
            }
            return BrowserType + " " + BrowserVersion;
        }

        /// <summary>
        ///     Infers an exit reason enumeration value
        /// </summary>
        /// <param name="ReasonText">Reason text string as read from the Qlikview log file</param>
        /// <returns>enum QvExitReason</returns>
        public static QvSessionEndReason GetExitReason(string ReasonText)
        {
            if (!string.IsNullOrEmpty(ReasonText))
            {
                if (ReasonText.Contains("Socket closed by client"))
                {
                    return QvSessionEndReason.CLIENTCLOSE;
                }
                if (ReasonText.Contains("Session expired after idle time"))
                {
                    return QvSessionEndReason.SESSIONTIMEOUT;
                }
                return QvSessionEndReason.OTHER;
            }
            return QvSessionEndReason.UNKNOWN;
        }

    }

}
