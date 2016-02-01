using System.IO;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Net;
using System.Web;
using System.Net.Security;
using System.Configuration;
using System.Net.Mail;
using SendGridMail;
using SendGridMail.Transport;

namespace MillimanSupport
{
    public class Global : System.Web.HttpApplication
    {
        private string SPKeyFile = string.Empty;
        private const string SPKeyPassword = "HCIntel";

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

            if ((string.Compare(CurrentPage, @"UserLogin.aspx", true) != 0) && (string.Compare(CurrentPage, @"LostPassword.aspx", true) != 0) && (string.Compare(CurrentPage, @"time.aspx", true) != 0) && (string.Compare(CurrentPage, @"diagnose.aspx", true) != 0) && (string.Compare(CurrentPage, @"time.aspx", true) != 0) && (string.Compare(CurrentPage, @"MillimanRelay.asmx", true) != 0)) //no recursive login voodoo
            {
                if (string.Compare(Request.CurrentExecutionFilePathExtension, @".aspx", true) == 0)  //only protect aspx pages
                {
                    string Shorty = "false";
                   if ( (Shorty == "true") || (HttpContext.Current.User == null) || (HttpContext.Current.User.Identity.IsAuthenticated == false))
                    {
                       Response.Redirect(@"UserLogin.aspx");
                    }
                }
            }
        }

        //probably want to use a logging framework for for now
        public enum LogLevel { Critial_Error, Error, Warning, Info, Debug };
        public static void Log(string Msg, LogLevel Level = LogLevel.Info)
        {
            try
            {
                string LogFile = Path.Combine( HttpContext.Current.Server.MapPath("~"), ConfigurationManager.AppSettings["Logfile"]);
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

        public enum FromType { CLOUD_SUPPORT, MILLIMAN_SUPPORT, TICKETING_SUPPORT };
        public static void SendEmail(string To, FromType From, string Body)
        {
            try
            {
                SendGrid myMessage = SendGrid.GetInstance();
                myMessage.AddTo(To);
                if (From == FromType.MILLIMAN_SUPPORT)
                    myMessage.From = new MailAddress("hcintel.support@Milliman.com", "HCIntel Support");
                else if (From == FromType.CLOUD_SUPPORT)
                    myMessage.From = new MailAddress("moitoring@hcintel.cloudapp.net", "Monitoring Support");
                else if (From == FromType.TICKETING_SUPPORT)
                    myMessage.From = new MailAddress("ticket@hcintel.cloudapp.net", "Ticket Support");

                myMessage.Subject = myMessage.From.DisplayName;
                myMessage.Html = Body;

                // Create credentials, specifying your user name and password.
                var credentials = new NetworkCredential("azure_914bd8e14f88e8ae1175362418c43c34@azure.com", "bvbsyxsw");

                // Create an SMTP transport for sending email.
                var transportSMTP = SMTP.GetInstance(credentials);

                // Send the email.
                transportSMTP.Deliver(myMessage);

            }
            catch (Exception ex)
            {
                Log("Failed to send email " + Body + " of type " + From.ToString() + " to " + To + " Error:" + ex.ToString(), LogLevel.Error);
            }
        }

    }
}