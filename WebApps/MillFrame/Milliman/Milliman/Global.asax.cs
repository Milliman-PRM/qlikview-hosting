using System.IO;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Net;
using System.Web;
using System.Net.Security;
using System.Configuration;

namespace MillimanDev
{
    public class Global : System.Web.HttpApplication
    {
        private string SPKeyFile = string.Empty;
        private const string SPKeyPassword = "HCIntel";

        private string IdPCertFile = string.Empty;

        public const string SPCertKey = "SPCertKey";
        public const string IdPCertKey = "IdPCertKey";

        public const string DefaultCovisintPassword = "covisint_federated_user";
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

        /// <summary>
        /// Loads the certificate file.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="fileName">The certificate file name.</param>
        /// <param name="password">The password for this certificate file.</param>
        private void LoadCertificate(string cacheKey, string fileName, string password)
        {
            X509Certificate2 cert = new X509Certificate2(fileName, password, X509KeyStorageFlags.MachineKeySet);

            Application[cacheKey] = cert;
        }

        /// <summary>
        /// Covisint has multiple certs so we need to loop over the web config 
        /// and pull all the cert files and load them
        ///    <add key="CovisintCert_1" value="SSOConfigure\HCO-Prod-cert.crt" />
        ///    <add key="CovisintCert_2" value="SSOConfigure\tibcert.crt" />
        /// </summary>
        /// <param name="RootCacheKey"></param>
        private void LoadAllCovisintCerts(string RootCacheKey)
        {
            int Index = 1;
            while (true)
            {
                string KeyName = RootCacheKey + @"_" + Index.ToString();
                if (ConfigurationManager.AppSettings[KeyName] == null)
                    return;

                IdPCertFile = Path.Combine(Server.MapPath("~"), ConfigurationManager.AppSettings[KeyName]);
                if (File.Exists(IdPCertFile) == true)
                    LoadCertificate(KeyName, IdPCertFile, "");

                Index++;
            }
        }

        void Application_Start(object sender, EventArgs e)
        {
            Log("Application Start");

            MillimanCommon.UserRepo.GetInstance(); //this will cause it to load

            // Set the server certificate validation callback.
            ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteServerCertificate;

           // LoadAllCovisintCerts("CovisintCert");
            //code below is replaced,  need to load multiple certs
            //IdPCertFile = Path.Combine(Server.MapPath("~"), ConfigurationManager.AppSettings["CovisintCert"]);
            //// Load the IdP cert file.
            //LoadCertificate(IdPCertKey, IdPCertFile, null);

            //SPKeyFile = Path.Combine(Server.MapPath("~"), ConfigurationManager.AppSettings["HCIntelPFX"]);
            //// Load the SP cert file.
            //LoadCertificate(SPCertKey, SPKeyFile, SPKeyPassword);


            string MDFile = Path.Combine(Server.MapPath("~"), ConfigurationManager.AppSettings["SSOMetaData"]);
            SSOConfiguration.LoadMetadataFromFile(MDFile);
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

            //temp patch until email issue is fixed
            if (string.Compare(CurrentPage, @"emaildameon.asmx", true) == 0)
            {
                return;
            }
            //end email patch

            if ((string.Compare(CurrentPage, @"UserLogin.aspx", true) != 0) && (string.Compare(CurrentPage, @"AssertionService.aspx", true) != 0) && (string.Compare(CurrentPage, @"LostPassword.aspx", true) != 0) && (string.Compare(CurrentPage, @"time.aspx", true) != 0)) //no recursive login voodoo
            {
                if (string.Compare(Request.CurrentExecutionFilePathExtension, @".aspx", true) == 0)  //only protect aspx pages
                {

                    try
                    {
                        System.Web.Security.MembershipUser MU = System.Web.Security.Membership.GetUser();

                    }
                    catch (Exception)
                    {
                    }
                   if ( (HttpContext.Current.User == null) || (HttpContext.Current.User.Identity.IsAuthenticated == false))
                    {
                        if ((string.Compare(CurrentPage, @"dashboard.aspx", true) == 0) && ( Request.RawUrl.ToLower().IndexOf("dashboardid") != -1))
                        {
                            string DashboardID = GetParameter(Request.RawUrl, @"dashboardid");
                            string PatientID = GetParameter(Request.RawUrl, @"enterpriseid");
                            Log("DashboardID=" + DashboardID);
                            string PatientQuery = string.IsNullOrEmpty(PatientID) ? string.Empty : @"&patientid=" + PatientID;
                            if (string.IsNullOrEmpty(DashboardID) == false)
                                Response.Redirect(@"UserLogin.aspx?mode=sso_redirect&dashboardid=" + DashboardID + PatientQuery);
                            else
                                Response.Redirect(@"UserLogin.aspx");
                        }
                        else
                        {
                            Response.Redirect(@"UserLogin.aspx");
                        }
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
            Server.Transfer("Userlogin.aspx");
        }

  
    }
}