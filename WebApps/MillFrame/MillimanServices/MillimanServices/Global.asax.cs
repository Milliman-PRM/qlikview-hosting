using System.IO;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Net;
using System.Web;
using System.Net.Security;
using System.Configuration;

namespace Milliman
{
    public class Global : System.Web.HttpApplication
    {
        public enum MsgType { ERROR, WARNING, INFO };
        public class SystemMsg
        {
            public MsgType MType { get; set; }
            public string Msg { get; set; }

            public SystemMsg(MsgType _MType, string _Msg)
            {
                MType = _MType;
                string Time = System.DateTime.Now.ToString().ToUpper();
                string MS = System.DateTime.Now.Millisecond.ToString();
                Time = Time.Replace("AM", MS);
                Time = Time.Replace("PM", MS);
                Msg = (Time).PadRight(35) + _Msg;
            }
        }
        static private System.Collections.Generic.List<SystemMsg> ClientActions = new System.Collections.Generic.List<SystemMsg>();

        static public Object ClientActionLock = new Object();
        static public System.Collections.Generic.List<SystemMsg> GetClientActions()
        {
            return ClientActions;
        }
        static public void ClearActions()
        {
            lock ( ClientActionLock)
            {
                ClientActions.Clear();
            }
        }
        static public void AddSystemMsg( MsgType _MType,
                                         string  _Msg )
        {
            lock (ClientActionLock)
            {
                while (ClientActions.Count > 25)
                    ClientActions.RemoveAt(0);

                ClientActions.Add( new SystemMsg(_MType, _Msg));
            }
        }
        

        void Application_Start(object sender, EventArgs e)
        {
            Log("Application Start");

        }

        public static string GetParameter(string RawURL, string Parameter)
        {
            string[] Items = RawURL.Split(new char[] { '=', '&', '?' });
            int DashboardIndex = -1;
            for (int Index = 0; Index < Items.Length; Index++)
            {
                if (string.Compare(Items[Index], Parameter, true) == 0)
                {
                    if (Index + 1 < Items.Length)
                        DashboardIndex = Index + 1;
                    break;
                }
            }
            return DashboardIndex == -1 ? string.Empty : Items[DashboardIndex];
        }

        void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            string CurrentPage = Path.GetFileName( Request.PhysicalPath);

            if ((string.Compare(CurrentPage, @"UserLogin.aspx", true) != 0) && (string.Compare(CurrentPage, @"LostPassword.aspx", true) != 0) && (string.Compare(CurrentPage, @"time.aspx", true) != 0) && (string.Compare(CurrentPage, @"administration.asmx", true) != 0)) //no recursive login voodoo
            {
                if (string.Compare(Request.CurrentExecutionFilePathExtension, @".aspx", true) == 0)  //only protect aspx pages
                {
                   if ( (HttpContext.Current.User == null) || (HttpContext.Current.User.Identity.IsAuthenticated == false))
                    {
                        Response.Redirect(@"UserLogin.aspx");
                    }
                }
            }
        }

        //probably want to use a logging framework for for now
        public enum LogLevel { Critial_Error, Error, Warning, Info, Debug };
        public void Log(string Msg, LogLevel Level = LogLevel.Info)
        {
            try
            {
                string LogFile = Path.Combine(Server.MapPath("~"), ConfigurationManager.AppSettings["Logfile"]);
                string Info = DateTime.Now.ToString().PadRight(20, ' ') + Level.ToString().PadRight(15, ' ') + Msg + "\r\n";
                System.IO.File.AppendAllText(LogFile, Info);
            }
            catch (Exception)
            {
                //do nothing, we are the error handler
            }
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Get the exception object.
            Exception exc = Server.GetLastError();
            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "", exc);
            Response.Cookies.Clear();
            Server.ClearError();
            Server.Transfer("login.aspx");
        }

    }
}