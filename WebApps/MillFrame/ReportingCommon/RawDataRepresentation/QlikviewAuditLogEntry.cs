using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportingCommon
{
    public class QlikviewAuditLogEntry
    {
        public DateTime ServerStarted;
        public DateTime Timestamp;
        public string Document;
        public string EventType;
        public string User;
        public string Message;

        /// <summary>
        ///     QlikViewSessionLogEntry constructor sans parameters
        /// </summary>
        public QlikviewAuditLogEntry()
        {
            Clear();
        }

        /// <summary>
        ///     QlikviewAuditLogEntry constructor, initialized with the contents of a caller supplied log file line
        /// </summary>
        /// <param name="line">A log file text line to be parsed</param>
        public QlikviewAuditLogEntry(string line)
        {
            this.ParseQlikviewAuditLogLine(line);
        }
        
        /// <summary>
        ///     Clear member data.  
        /// </summary>
        /// <returns>void</returns>
        public void Clear() 
        {
            this.ServerStarted = new DateTime();
            this.Timestamp = new DateTime();
            this.Document = "";
            this.EventType = "";
            this.User = "";
            this.Message = "";
        }

        /// <summary>
        ///     Clears member data and parses the content from one QlikView log file line and set it to respective fields
        /// </summary>
        /// <param name="line">Contents of one line from a Qlikview log file</param>
        public void ParseQlikviewAuditLogLine(string line)
        {
            Clear();

            string[] fields = line.Split('\t');
            if (fields.Length < 6)
            {
                return;
            }

            // set all field values, avoiding exceptions
            DateTime.TryParse(fields[0], out this.ServerStarted);
            DateTime.TryParse(fields[1], out this.Timestamp);
            this.Document = fields[2];
            this.EventType = fields[3];
            this.User = fields[4];
            this.Message = fields[5];
        }

        /// <summary>
        ///     Parse one QlikView log file and return the complete List of entries
        /// </summary>
        /// <param name="LogFileName">Name of the log file to parse</param>
        /// <returns>List<QlikViewLogFileEntry></returns>
        public static List<QlikviewAuditLogEntry> ParseOneQlikviewAuditLog(string LogFileName, DateTime FromTime, DateTime ToTime)
        {
            List<QlikviewAuditLogEntry> ReturnList = new List<QlikviewAuditLogEntry>();

            if (IsLogFileInDateRange(LogFileName, FromTime, ToTime))
            {
                List<string> Lines;
                try
                {
                    Lines = File.ReadAllLines(LogFileName).ToList();
                }
                catch
                {
                    // TODO maybe log something
                    return ReturnList;
                }

                Lines.RemoveAt(0);

                foreach (string LogLine in Lines)
                {
                    QlikviewAuditLogEntry Entry = new QlikviewAuditLogEntry(LogLine);
                    if (Entry.Timestamp.CompareTo(FromTime) >= 0 &&
                        Entry.Timestamp.CompareTo(ToTime) <= 0)
                    {
                        ReturnList.Add(Entry);
                    }
                }
            }

            return ReturnList;
        }

        /// <summary>
        ///     Test whether a Qlikview log file is dated within the specified range based on the file name only.  
        ///     Only date is considered, not time of day. 
        /// </summary>
        /// <param name="LogFileName">The full path/name of the log file to test</param>
        /// <param name="LogFileName">The start of the defined date range</param>
        /// <param name="LogFileName">The end of the defined date range</param>
        /// <returns>bool</returns>
        private static bool IsLogFileInDateRange(string LogFileName, DateTime FromTime, DateTime ToTime)
        {
            string FileName = Path.GetFileNameWithoutExtension(LogFileName);
            string DateString = FileName.Substring(FileName.LastIndexOf('_') + 1);
            DateTime FileDate = DateTime.ParseExact(DateString, "yyyy-MM-dd", null);  // could throw
            return (FileDate >= FromTime.Date && FileDate <= ToTime.Date);
        }

    }
}
