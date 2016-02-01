using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MillimanSupport.HCIntelMonitoring
{
    public class MonitoringHelper
    {
        public class User
        {
            public string CUID { get; set; }
            public string Status { get; set; }
            public string LastAccess { get; set; }

            public User() { }
            public User( string _CUID, string _Status, string _LastAccess )
            {
                CUID = _CUID;
                Status = _Status;
                LastAccess = _LastAccess;
            }
        }
        public List<User> UserList { get; set; }

        public bool ReportSubsystem { get; set; }
        public bool DataSubsystem { get; set; }
        public bool UserSubsystem { get; set; }
        public bool Diskspace { get; set; }
        public bool Memory { get; set; }

        public MonitoringHelper()
        {
            ReportSubsystem = false;
            DataSubsystem = false;
            UserSubsystem = false;
            Diskspace = false;
            Memory = false;
            UserList = new List<User>();

            GetHCIntelStatus();
        }

        private string DecodeResponse(string Response)
        {
            string EncryptedString = string.Empty;
            string[] Tokens = Response.Split(new char[] {'<','>','"' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < Tokens.Length; i++)
            {
                if (string.Compare(Tokens[i], "status", true) == 0)
                {
                    EncryptedString = Tokens[i + 2];
                    break;
                }
            }
            if (EncryptedString == string.Empty)
                return "";
            MillimanCommon.AutoCrypt AC = new MillimanCommon.AutoCrypt();
            return AC.AutoDecrypt(EncryptedString);
        }

        private bool GetHCIntelStatus()
        {
            string UserList = string.Empty;
            try
            {
                string URL = "https://prm.milliman.com/prm/time.aspx?NeverFail=true";
                Comm Communications = new Comm();
                string Response = Communications.Execute(URL);
                if (string.IsNullOrEmpty(Response) == false)
                {
                    Response = DecodeResponse(Response);
                    if (Response.IndexOf("<QVServer>true") >= 0)
                        ReportSubsystem = true;
                    if (Response.IndexOf("<DBConnections>true") >= 0)
                        DataSubsystem = true;
                    if (Response.IndexOf("<DBConnections>true") >= 0)
                        UserSubsystem = true;
                    if (Response.IndexOf("<FreeMemoryPercentage>true") >= 0)
                        Memory = true;
                    if (Response.IndexOf("<FreeDiskPercentage>true") >= 0)
                        Diskspace = true;

                    int StartUsers = Response.IndexOf("<ids>");
                    int EndUsers = Response.IndexOf("</ids>");
                    int UsersLength = EndUsers - (StartUsers+5);
                    if ( UsersLength > 0 )
                        UserList = Response.Substring(StartUsers+5, UsersLength );
                }
                HttpContext.Current.Session["CUIDS"] = UserList;
            }
            catch (Exception)
            {
                UserList = string.Empty;
                HttpContext.Current.Session["CUIDS"] = "";
                return false;
            }
            return true;
        }
    }
}