using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ReportingCommon
{
    /// <summary>
    ///     An instance represents the raw, unfiltered information from one IIS log file entry
    ///     static functionality to read a complete log file, returning a list of instances
    /// </summary>
    public class IisLogFileEntry
    {
        public DateTime TimeStamp { get; set; }
        public string ServerIp { get; set; }
        public string Method { get; set; }
        public string UriStem { get; set; }
        public string UriQuery { get; set; }
        public int Port { get; set; }
        private string _UserName;
        public string ClientIp { get; set; }
        public string UserAgent { get; set; }
        public string Referer { get; set; }
        public int Status { get; set; }
        public int Substatus  { get; set; }
        public int Win32Status  { get; set; }
        public int TimeTaken  { get; set; }

        public string UserName
        {
            get { 
                if (_UserName.IndexOf('@') >= 0)
                    return _UserName;
                else
                    return "";
            }
            set
            {
                if (value.IndexOf('@') >= 0)
                {
                    _UserName = value;
                }
            }
        }

        /// <summary>
        ///     Contructor, ensure members are initialized clear.  
        /// </summary>
        public IisLogFileEntry()
        {
            Clear();
        }

        /// <summary>
        ///     Contructor, ensure members are initialized by parsing the provided string from the log file.  
        /// </summary>
        /// <param name="line">The raw contents of one line as read from an IIS log file</param>
        public IisLogFileEntry(string line)
        {
            ParseIisLogFileLine(line);
        }

        /// <summary>
        /// Tests whether this log message instance represents a user login action.  
        /// </summary>
        /// <returns></returns>
        public bool IsLogin()
        {
            return
                    (string.Compare(Method.ToLower(), "get", true) == 0) &&
                    (string.Compare(UriStem.ToLower(), "/prm/default.aspx", true) == 0) &&
                    (Port == 443) &&
                    (string.Compare(Referer.ToLower(), "https://prm.milliman.com/prm/userlogin.aspx", true) == 0) &&
                    (Status == 200);
        }

        /// <summary>
        /// Tests whether this log message instance contains a valid user name.  
        /// </summary>
        /// <returns></returns>
        public bool HasUserName()
        {
            return (this._UserName.IndexOf("@") >= 0);
        }

        /// <summary>
        /// Initializes all data members to an unset state.  
        /// </summary>
        public void Clear() 
        {
            this._UserName = "";
            this.ServerIp = "";
            this.UriStem = "";
            this.UriQuery = "";
            this.Port = -1;
            this.ClientIp = "";
            this.TimeStamp = new DateTime();
            this.UserAgent = "";
            this.Referer = "";
            this.Status = -1;
            this.Method = "";
            this.Substatus = -1;
            this.Win32Status = -1;
            this.TimeTaken = -1;
        }

        /// <summary>
        ///     Parse one line from the IIS log file and set member variables accordingly.
        /// </summary>
        /// <param name="line">A string with the contents of one line of as read from an IIS log file</param>
        public void ParseIisLogFileLine(string line)
        {
            Clear();

            string[] fields = line.Split(' ');
            if (!line.StartsWith("#") && fields.Length >= 15)
            {
                this.TimeStamp = Convert.ToDateTime(fields[0] + " " + fields[1]);
                this.ServerIp = fields[2];
                this.Method = fields[3];
                this.UriStem = fields[4];
                this.UriQuery = fields[5];
                this.Port = Convert.ToInt32(fields[6]);
                this._UserName = fields[7];
                this.ClientIp = fields[8];
                this.UserAgent = fields[9];
                this.Referer = fields[10];
                this.Status = Convert.ToInt32(fields[11]);
                this.Substatus = Convert.ToInt32(fields[12]);
                this.Win32Status = Convert.ToInt32(fields[13]);
                this.TimeTaken = Convert.ToInt32(fields[14]);
            }
        }

        /// <summary>
        ///     Parse one IIS log file and return the complete List of entries
        /// </summary>
        /// <param name="LogFileName">The name of the log file to be parsed</param>
        /// <param name="LogFileName">The start of the defined date range</param>
        /// <param name="LogFileName">The end of the defined date range</param>
        /// <returns>List<IisLogFileEntry></returns>
        public static List<IisLogFileEntry> ParseOneIisLogFile(string LogFileName, DateTime FromTime, DateTime ToTime)
        {
            string[] SupportedIisVersions = { "8.5" };
            List<IisLogFileEntry> ReturnList = new List<IisLogFileEntry>();

            if (IsIisLogFileInDateRange(LogFileName, FromTime, ToTime))
            {
                List<string> Lines = File.ReadAllLines(LogFileName).ToList();

                string ThisIisVersion = Lines[0].Substring(Lines[0].IndexOfAny("0123456789".ToCharArray()));
                if (SupportedIisVersions.Contains(ThisIisVersion))
                {
                    // Strip out comment lines
                    Lines = Lines.SkipWhile(x => x.StartsWith("#")).ToList();

                    foreach (string LogLine in Lines)
                    {
                        IisLogFileEntry Entry = new IisLogFileEntry(LogLine);
                        if (Entry.TimeStamp.CompareTo(FromTime) >= 0 &&
                            Entry.TimeStamp.CompareTo(ToTime) <= 0)
                        {
                            ReturnList.Add(Entry);
                        }
                    }
                }

            }

            return ReturnList;
        }

        /// <summary>
        ///     Test whether an IIS log file is dated within the specified range based on the file name only.  
        ///     Only date is considered, not time of day. 
        /// </summary>
        /// <param name="LogFileName">The full path/name of the log file to test</param>
        /// <param name="LogFileName">The start of the defined date range</param>
        /// <param name="LogFileName">The end of the defined date range</param>
        /// <returns>bool</returns>
        private static bool IsIisLogFileInDateRange(string IisLogFileName, DateTime FromTime, DateTime ToTime)
        {
            string FileName = Path.GetFileNameWithoutExtension(IisLogFileName);
            DateTime FileDate = DateTime.ParseExact(FileName, "'u_ex'yyMMdd", null);  // could throw
            return (FileDate >= FromTime.Date && FileDate <= ToTime.Date);
        }
    }

}
