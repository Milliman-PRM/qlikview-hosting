using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ReportingCommon
{
    /// <summary>
    ///     An instance represents the raw, unfiltered information from one QlikView log file entry
    ///     static functionality to read a complete log file, returning a list of instances
    /// </summary>
    public class QlikviewSessionLogEntry
    {
        public string ExeType;
        public string ExeVersion;
        public DateTime ServerStarted;
        public DateTime Timestamp;
        public string Document;
        public DateTime DocumentTimeStamp;
        public string QlikViewUser;
        public string ExitReason;
        public DateTime SessionStart;
        public TimeSpan SessionDuration;
        public double CpuSpentS;
        public Int32  BytesReceived;
        public Int32  BytesSent;
        public Int32  Calls;
        public Int32  Selections;
        private string _AuthenticatedUser;
        public string IdentifyingUser;
        public Guid   ClientMachineIdentification;
        public string SerialNumber;
        public string ClientType;
        public string ClientBuildVersion;
        public string SecureProtocol;
        public string TunnelProtocol;
        public Int32  ServerPort;
        public string ClientAddress;
        public Int32  ClientPort;
        public string CalType;
        public Int32  CalUsageCount;

        public string AuthenticatedUser
        {
            get { 
                // TODO make sure invalid values are handled properly
                return _AuthenticatedUser; 
            }
            set {
                if (value.IndexOf('@') >= 0)
                    _AuthenticatedUser = value;
            }
        }

        /// <summary>
        ///     QlikViewLogFileEntry constructor sans parameters
        /// </summary>
        public QlikviewSessionLogEntry()
        {
            Clear();
        }

        /// <summary>
        ///     QlikViewLogFileEntry constructor, initialized with the contents of a caller supplied log file line
        /// </summary>
        /// <param name="line">A log file text line to be parsed</param>
        public QlikviewSessionLogEntry(string line)
        {
            this.ParseQlikViewLogFileLine(line);
        }
        
        /// <summary>
        ///     Clear member data.  
        /// </summary>
        /// <returns>void</returns>
        public void Clear() 
        {
            this.ExeType = "";
            this.ExeVersion = "";
            this.ServerStarted = new DateTime();
            this.Timestamp = new DateTime();;
            this.Document = "";
            this.DocumentTimeStamp = new DateTime();
            this.QlikViewUser = "";
            this.ExitReason = "";
            this.SessionStart = new DateTime();
            this.SessionDuration = new TimeSpan();
            this.CpuSpentS = 0.0;
            this.BytesReceived = -1;
            this.BytesSent = -1;
            this.Calls = -1;
            this.Selections = -1;
            this._AuthenticatedUser = "";
            this.IdentifyingUser = "";
            this.ClientMachineIdentification = new Guid();  // ???
            this.SerialNumber = "";
            this.ClientType = "";
            this.ClientBuildVersion = "";
            this.SecureProtocol = "";
            this.TunnelProtocol = "";
            this.ServerPort = -1;
            this.ClientAddress = "";
            this.ClientPort = -1;
            this.CalType = "";
            this.CalUsageCount = -1;
        }

        /// <summary>
        ///     Clears member data and parses the content from one QlikView log file line and set it to respective fields
        /// </summary>
        /// <param name="line">Contents of one line from a Qlikview log file</param>
        public void ParseQlikViewLogFileLine(string line)
        {
            Clear();

            string[] fields = line.Split('\t');
            if (fields.Length >= 28)
            {
                // set all field values, avoiding exceptions
                this.ExeType = fields[0];
                this.ExeVersion = fields[1];
                DateTime.TryParse(fields[2], out this.ServerStarted);
                DateTime.TryParse(fields[3], out this.Timestamp);
                this.Document = fields[4];
                DateTime.TryParse(fields[5], out this.DocumentTimeStamp);
                this.QlikViewUser = fields[6];
                this.ExitReason = fields[7];
                DateTime.TryParse(fields[8], out this.SessionStart);
                TimeSpan.TryParse(fields[9], out this.SessionDuration);
                Double.TryParse(fields[10], out this.CpuSpentS);
                Int32.TryParse(fields[11], out this.BytesReceived);
                Int32.TryParse(fields[12], out this.BytesSent);
                Int32.TryParse(fields[13], out this.Calls);
                Int32.TryParse(fields[14], out this.Selections);
                this._AuthenticatedUser = fields[15];
                this.IdentifyingUser = fields[16];
                Guid.TryParse(fields[17], out this.ClientMachineIdentification);
                this.SerialNumber = fields[18];
                this.ClientType = fields[19];
                this.ClientBuildVersion = fields[20];
                this.SecureProtocol = fields[21];
                this.TunnelProtocol = fields[22];
                Int32.TryParse(fields[23], out this.ServerPort);
                this.ClientAddress = fields[24];
                Int32.TryParse(fields[25], out this.ClientPort);
                this.CalType = fields[26];
                Int32.TryParse(fields[27], out this.CalUsageCount);
            }
        }

        /// <summary>
        ///     Parse one QlikView log file and return the complete List of entries
        /// </summary>
        /// <param name="LogFileName">Name of the log file to parse</param>
        /// <returns>List<QlikViewLogFileEntry></returns>
        public static List<QlikviewSessionLogEntry> ParseOneQlikviewLogFile(string LogFileName, DateTime FromTime, DateTime ToTime)
        {
            List<QlikviewSessionLogEntry> ReturnList = new List<QlikviewSessionLogEntry>();

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
                    QlikviewSessionLogEntry Entry = new QlikviewSessionLogEntry(LogLine);
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
            string DateString = FileName.Substring(FileName.LastIndexOf('_')+1);
            DateTime FileDate = DateTime.ParseExact(DateString, "yyyy-MM-dd", null);  // could throw
            return (FileDate >= FromTime.Date && FileDate <= ToTime.Date);
        }
    }
}
