using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ReportingCommon
{
    /// <summary>
    /// Intended library API class to provide aggregation, sorting, and filtering of log file information about PRM user activity
    /// </summary>
    public class AggregatedStatistics
    {
        /// <summary>
        /// High level static API function obtains the filtered, sorted list of events and writes the output to a designated file
        /// </summary>
        /// <param name="IisLogFilePath"></param>
        /// <param name="QlikLogFilePath"></param>
        /// <param name="FromTime"></param>
        /// <param name="ToTime"></param>
        /// <param name="OutputFileName"></param>
        /// <param name="QlikServerTimeZone">A TimezoneInfo object representing the time zone of the QlikView Server that generated the log file</param>
        /// <param name="UseDaylightSavings">A flag indicating whether to convert timestamps with or without consideration of daylight savings time</param>
        /// <param name="QvDocRoot">The root path of QlikView documents, as expected to be contained in paths of QVW file names in log files</param>
        /// <returns></returns>
        public static bool WriteAggregatedIisQlikviewStatistics(string IisLogFilePath, string QlikLogFilePath, DateTime FromTime, DateTime ToTime, string OutputFileName, TimeZoneInfo QlikServerTimeZone, bool UseDaylightSavings, string QvDocRoot, bool SearchSessionLogs, bool SearchAuditLogs)
        {
            const string OutputFormat =
                "{0,-19} \t" +  // Timestamp
                "{1,-54} \t" +  // User
                "{2,-56} \t" +  // Group
                "{3,-22} \t" +  // Event
                "{4,-48} \t" +  // Report
                "{5, -8} \t" +  // Session Length
                "{6,-11} \t" +  // Reason/Type
                "{7,-20} \t" +  // Browser
                "{8,-20} ";     // Detail

            StreamWriter OutStream = null;
            try
            {
                OutStream = new StreamWriter(OutputFileName, false);

                List<UserEventBase> ResultList = GetAggregatedIisQlikviewStatistics(IisLogFilePath, QlikLogFilePath, FromTime, ToTime, QlikServerTimeZone, UseDaylightSavings, QvDocRoot, SearchSessionLogs, SearchAuditLogs);

                string OutputLine = string.Format(OutputFormat + OutStream.NewLine, "Timestamp", "User", "Group", "Event", "Report", "Duration", "Type", "Browser", "Detail");
                OutStream.Write(OutputLine);
                foreach (UserEventBase Event in ResultList)
                {
                    string[] OutputFields = GetOutputFields(Event);
                    OutputLine = string.Format(OutputFormat + OutStream.NewLine, OutputFields);
                    OutStream.Write(OutputLine);
                }

                OutStream.Close();
            }
            catch (Exception e)
            {
                if (OutStream != null)
                {
                    OutStream.Write("Exception during processing:" + OutStream.NewLine +
                                    e.Message + OutStream.NewLine +
                                    e.StackTrace);
                    OutStream.Close();
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// returns an ordered sequence of strings to be outout, The output is specialized for each 
        /// type of event supported, and is matched to the format string value stored in OutputFormat
        /// </summary>
        /// <param name="Event">An instance of any class derived from UserEventBase</param>
        /// <returns></returns>
        public static string[] GetOutputFields(UserEventBase Event)
        {
            string[] ReturnList = new string[9];

            switch (Event.GetType().ToString())
            {
                case "ReportingCommon.IisAccessEvent":
                    ReturnList[0] = ((IisAccessEvent)Event).TimeStamp.ToString("MM/dd/yyyy HH:mm:ss");
                    ReturnList[1] = ((IisAccessEvent)Event).User;
                    ReturnList[2] = ((IisAccessEvent)Event).Group;
                    ReturnList[3] = ((IisAccessEvent)Event).EventType.ToString();
                    ReturnList[4] = "";
                    ReturnList[5] = "";
                    ReturnList[6] = "";
                    ReturnList[7] = ((IisAccessEvent)Event).Browser;
                    ReturnList[8] = "";
                    break;

                case "ReportingCommon.QlikviewSessionEvent":
                    ReturnList[0] = ((QlikviewSessionEvent)Event).TimeStamp.ToString("MM/dd/yyyy HH:mm:ss");
                    ReturnList[1] = ((QlikviewSessionEvent)Event).User;
                    ReturnList[2] = ((QlikviewSessionEvent)Event).Group;
                    ReturnList[3] = "Qlikview Session Event";
                    ReturnList[4] = ((QlikviewSessionEvent)Event).Report;
                    ReturnList[5] = ((QlikviewSessionEvent)Event).SessionLength.ToString();
                    ReturnList[6] = ((QlikviewSessionEvent)Event).GetReasonString();
                    ReturnList[7] = ((QlikviewSessionEvent)Event).Browser;
                    ReturnList[8] = "";
                    if (((QlikviewSessionEvent)Event).IsReduced) ReturnList[4] = "* " + ReturnList[4];
                    break;

                case "ReportingCommon.QlikviewAuditEvent":
                    ReturnList[0] = ((QlikviewAuditEvent)Event).TimeStamp.ToString("MM/dd/yyyy HH:mm:ss");
                    ReturnList[1] = ((QlikviewAuditEvent)Event).User;
                    ReturnList[2] = ((QlikviewAuditEvent)Event).Group;
                    ReturnList[3] = "Qlikview Audit Event";
                    ReturnList[4] = ((QlikviewAuditEvent)Event).Report;
                    ReturnList[5] = "";
                    ReturnList[6] = ((QlikviewAuditEvent)Event).EventType;
                    ReturnList[7] = "";
                    ReturnList[8] = ((QlikviewAuditEvent)Event).Message;
                    if (((QlikviewAuditEvent)Event).IsReduced) ReturnList[4] = "* " + ReturnList[4];
                    break;

                default:
                    ReturnList[0] = "";
                    ReturnList[1] = "";
                    ReturnList[2] = "";
                    ReturnList[3] = "Unsupported event type";
                    ReturnList[4] = "";
                    ReturnList[5] = "";
                    ReturnList[6] = "";
                    ReturnList[7] = "";
                    break;
            }

            return ReturnList;
        }

        /// <summary>
        /// High level static API function returns the filtered, sorted list of events
        /// </summary>
        /// <param name="IisLogFilePath"></param>
        /// <param name="QlikLogFilePath"></param>
        /// <param name="FromTime"></param>
        /// <param name="ToTime"></param>
        /// <param name="QlikServerTimeZone">A TimezoneInfo object representing the time zone of the QlikView Server that generated the log file</param>
        /// <param name="UseDaylightSavings">A flag indicating whether to convert timestamps with or without consideration of daylight savings time</param>
        /// <param name="QvDocRoot">The root path of QlikView documents, as expected to be contained in paths of QVW file names in log files</param>
        /// <returns></returns>
        public static List<UserEventBase> GetAggregatedIisQlikviewStatistics(string IisLogFilePath, string QlikLogFilePath, DateTime FromTime, DateTime ToTime, TimeZoneInfo QlikServerTimeZone, bool UseDaylightSavings, string QvDocRoot, bool SearchSessionLogs, bool SearchAuditLogs)
        {
            List<UserEventBase> ReturnList = new List<UserEventBase>();

            // Qlikview Session logs
            if (SearchSessionLogs && Directory.Exists(QlikLogFilePath))
            {
                foreach (string FoundFile in Directory.GetFiles(QlikLogFilePath, "Sessions_*.log"))
                {
                    List<QlikviewSessionLogEntry> Logs = QlikviewSessionLogEntry.ParseOneQlikviewLogFile(FoundFile, FromTime, ToTime);
                    foreach (QlikviewSessionLogEntry Entry in Logs)
                    {
                        if (Entry.Timestamp.CompareTo(FromTime) >= 0 &&
                            Entry.Timestamp.CompareTo(ToTime) <= 0)
                        {
                            ReturnList.Add(new QlikviewSessionEvent(Entry, QlikServerTimeZone, UseDaylightSavings, QvDocRoot));
                        }
                    }
                }
            }
            // Qlikview Audit logs
            if (SearchAuditLogs && Directory.Exists(QlikLogFilePath))
            {
                foreach (string FoundFile in Directory.GetFiles(QlikLogFilePath, "Audit_*.log"))
                {
                    List<QlikviewAuditLogEntry> Logs = QlikviewAuditLogEntry.ParseOneQlikviewAuditLog(FoundFile, FromTime, ToTime);
                    foreach (QlikviewAuditLogEntry Entry in Logs)
                    {
                        if (Entry.Timestamp.CompareTo(FromTime) >= 0 &&
                            Entry.Timestamp.CompareTo(ToTime) <= 0)
                        {
                            ReturnList.Add(new QlikviewAuditEvent(Entry, QlikServerTimeZone, UseDaylightSavings, QvDocRoot));
                        }
                    }
                }
            }

            // Collect (add) events from all found IIS logs
            if (Directory.Exists(IisLogFilePath))
            {
                foreach (string FoundFile in Directory.GetFiles(IisLogFilePath, "u_*.log"))
                {
                    List<IisLogFileEntry> Logs = IisLogFileEntry.ParseOneIisLogFile(FoundFile, FromTime, ToTime);
                    foreach (IisLogFileEntry Entry in Logs)
                    {
                        if (Entry.TimeStamp.CompareTo(FromTime) >= 0 &&
                            Entry.TimeStamp.CompareTo(ToTime) <= 0 &&
                            Entry.HasUserName())
                        {
                            ReturnList.Add(new IisAccessEvent(Entry, QvDocRoot));
                        }
                    }
                }
            }

            // Sort all events using comparison rules defined in class UserEventComparer.  Must use OrderBy because
            // Sort method uses unstable sorting, does not preserve prior order of elements that compare equal
            ReturnList = ReturnList.OrderBy(a => a, new UserEventComparer()).ToList();

            // Remove all but the last redundant IIS_LAST_ACCESS event
            HashSet<string> ActiveIisUserSessions = new HashSet<string>();
            for (int Counter = ReturnList.Count-1 ; Counter >= 0 ; Counter--) 
            {
                if (ReturnList[Counter].GetType() == typeof(IisAccessEvent))
                {
                    switch (((IisAccessEvent)ReturnList[Counter]).EventType)
                    {
                        case IisAccessEvent.IisEventType.IIS_LOGIN:
                            ActiveIisUserSessions.Remove(((IisAccessEvent)ReturnList[Counter]).User);
                            break;
                        
                        case IisAccessEvent.IisEventType.IIS_LAST_ACCESS:
                            if (ActiveIisUserSessions.Contains(((IisAccessEvent)ReturnList[Counter]).User))
                            {
                                ReturnList.RemoveAt(Counter);
                            }
                            else
                            {
                                ActiveIisUserSessions.Add(((IisAccessEvent)ReturnList[Counter]).User);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            return ReturnList;
        }

    }
}
