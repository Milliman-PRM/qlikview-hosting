using System.IO;
using System;
using System.Net;
using System.Web;
using System.Net.Security;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace ClientPublisher
{
    public class Global : System.Web.HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            Log("Application Start");
            MillimanCommon.UserRepo.GetInstance();
            Log("Loaded user repo");
            MillimanCommon.SuperGroup.GetInstance(); //this will cause it to load
            Log("Loaded super groups");

            // Set the server certificate validation callback.
            ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteServerCertificate;
        }

        /// <summary>
        /// Verifies the remote Secure Sockets Layer (SSL) certificate used for authentication.
        /// </summary>
        /// <param name="sender">An object that contains state information for this validation.</param>
        /// <param name="certificate">The certificate used to authenticate the remote party.</param>
        /// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
        /// <param name="sslPolicyErrors">One or more errors associated with the remote certificate.</param>
        /// <returns>A System.Boolean value that determines whether the specified certificate is accepted for authentication.</returns>
        private static bool ValidateRemoteServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // NOTE: This is a test application with self-signed certificates, so all certificates are trusted.
            return true;
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
            string CurrentPage = Path.GetFileName(Request.PhysicalPath);

            //if ((string.Compare(CurrentPage, @"UserLogin.aspx", true) != 0) && (string.Compare(CurrentPage, @"LostPassword.aspx", true) != 0) && (string.Compare(CurrentPage, @"time.aspx", true) != 0)) //no recursive login voodoo
            //{
            //    if (string.Compare(Request.CurrentExecutionFilePathExtension, @".aspx", true) == 0)  //only protect aspx pages
            //    {

            //        try
            //        {
            //            System.Web.Security.MembershipUser MU = System.Web.Security.Membership.GetUser();
            //            if (MU == null)
            //                Response.Redirect("userlogin.aspx");
            //        }
            //        catch (Exception)
            //        {
            //            //not logged in
            //            Response.Redirect("userlogin.aspx");
            //        }

            //    }
            //}
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
            //Response.Cookies.Clear();
            //Server.ClearError();
            //System.Web.Security.FormsAuthentication.SignOut();
            ////Session.Abandon();  //don't call session abandon, causes exception
            //Server.Transfer("Userlogin.aspx");
        }

        static private ClientPublisher.TaskManager _TaskManager = null;
        public static ClientPublisher.TaskManager TaskManager
        {
            get
            {
                if (_TaskManager == null)
                {
                    _TaskManager = new ClientPublisher.TaskManager();
                }
                return _TaskManager;
            }
            set { _TaskManager = value; }
        }

    }
}