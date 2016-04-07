using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace MillimanDev
{
    public partial class Dashboard : System.Web.UI.Page
    {
        #region Authentication Settings
        // If TrustedIPs are used in c:\ProgramData\QlikTech\WebServer\config.xml this should be set to true, otherwise false
        private bool Anonymous = true;
        // If Windows Authentication is used for GetWebTicket.aspx then credentials may be specified below. If no credentials is provided, UseDefaultCredentials will be used
        private string UserName = "";
        private string Password = "";
        #endregion

        #region Redirection Settings
        // URL that the user is redirected to after a successful login. AccessPoint is usually where you want to go
        private string TryUrl = "/QlikView/";
        // URL redirected to after a failed login attempt
        private string BackUrl = "";
        // Server where the QlikView AccessPoint resides (ends with slash) - must be 127.0.0.1 to be trusted by server
        //private string AccessPointServer = "https://127.0.0.1/";
        //private string ExternalAccessPointServer = "https://hcintel.milliman.com";

        //for production server this must be HTTPS://127.0.0.1 to work
        //we have code below that will look to see if the external server name is PRM.MILLIMAN.COM and default to HTTPS if needed
        private string AccessPointServer = "http://127.0.0.1/";
        // private string ExternalAccessPointServer = "http://hcintel.cloudapp.net";

        #endregion

        #region VariableDeclarations
        // Variable declarations (DON'T CHANGE HERE)
        private string _userId = "";
        private string _userFriendlyName = "";
        private string _userGroups = "";
        private string _webTicket = "";

        private string _UserPrefix = @"Custom\";  //needed by QV to do custom authentiation
        #endregion

        public enum SystemOrigin { Milliman, ExternalSSO };
        /// <summary>
        /// Launch the document as the user
        /// </summary>
        /// <param name="_UserName">User to launch the document on behalf of</param>
        /// <param name="_QVDocument">the document to launch</param>
        /// <param name="_ContextSelections">An selections to make in the document -"chPopReport,BQ863" - set control chPopReport to patient BQ863</param>
        /// <param name="_ErrorURL">QV will nav to here when a problem occurs</param>
        /// <param name="Origin">Milliman or Covisint - needed when bookmarked items hit this page first</param>
        /// <returns></returns>
        public bool Launch(string _UserName, string _QVDocument, string _ContextSelections, string _ErrorURL, SystemOrigin Origin = SystemOrigin.Milliman)
        {
            _userId = _UserPrefix + _UserName;
            _userFriendlyName = _UserName;

            BackUrl = _ErrorURL;

            //this is just a check to see if we are on PRM production machine or not, we default to HTTP
            if (System.Configuration.ConfigurationManager.AppSettings["ExternalServerName"].ToLower().Contains("prm.milliman.com"))
                AccessPointServer = "https://127.0.0.1/";


            if (GetWebTicket() == false)
                return false; 

            if (LicenseSetup(_UserName, _QVDocument))
            {
                RedirectToQlikView(_QVDocument, System.Configuration.ConfigurationManager.AppSettings["ExternalServerName"], _ContextSelections, Origin.ToString().ToLower());
                return true;
            }
            else
            {
                MillimanCommon.Report.Log( MillimanCommon.Report.ReportType.Error,"Could not allocate license for user");
                return false;
            }

        }

        private bool LicenseSetup(string UserName, string QVDocument)
        {
            try
            {
                MillimanReportReduction.QVLicensing QVL = new MillimanReportReduction.QVLicensing();
                int MaxDocsPerDocumentCAL = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxDocsPerDocumentCAL"]);
                string[] UserRoles = MillimanCommon.UserAccessList.GetRolesForUser();
                MillimanCommon.UserAccessList ACL = new MillimanCommon.UserAccessList(UserName, UserRoles, false);

               if (ACL.ACL.Count > MaxDocsPerDocumentCAL)
                {  //give them a named user CAL for all documents
                    QVL.AssignUserNamedCAL(UserName);
                }
               else
               { //give them a document CAL for this specific document
                   QVL.AssignUserDocCAL(UserName, new System.Collections.Generic.List<string>() { QVDocument });
               }

                ACL.ACL = null;
                ACL = null;
                QVL = null;
                return true;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified error", ex);
            }
            return false;
        }

        /// <summary>
        /// Check the health of the QV server component with the dummy user "HealthMonitor";
        /// </summary>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public bool QVServerHealthCheck(out string ErrorMsg)
        {
            ErrorMsg = @"Server not accessable";
            try
            {
                _userFriendlyName = @"HealthMonitor";
                _userId = _UserPrefix + _userFriendlyName;

                if (!String.IsNullOrEmpty(_userGroups))
                    GetGroups();

                string webTicketXml = string.Format("<Global method=\"GetWebTicket\"><UserId>{0}</UserId>{1}</Global>", _userId, _userGroups);

                string result = Execute(String.Format("{0}QvAJAXZfc/GetWebTicket.aspx", AccessPointServer), "POST", webTicketXml);

                if ((string.IsNullOrEmpty(result) == false) && (result.Contains("Invalid call") == false))
                    return true;

            }
            catch (Exception ex)
            {
                ErrorMsg = ex.ToString();
            }
            return false;
        }

        /// <summary>
        /// Get webticket for specified user
        /// </summary>
        private bool GetWebTicket()
        {
            bool Results = false;

            if (String.IsNullOrEmpty(_userId))
                return Results;

            if (!String.IsNullOrEmpty(_userGroups))
                GetGroups();

            string webTicketXml = string.Format("<Global method=\"GetWebTicket\"><UserId>{0}</UserId>{1}</Global>", _userId, _userGroups);

            string result = Execute(String.Format("{0}QvAJAXZfc/GetWebTicket.aspx", AccessPointServer), "POST", webTicketXml);

            if (string.IsNullOrEmpty(result) || result.Contains("Invalid call"))
                return Results;

            XDocument doc = XDocument.Parse(result);

            _webTicket = doc.Root.Element("_retval_").Value;

            // Set friendly name cookie for AccessPoint
            if (!String.IsNullOrEmpty(_userFriendlyName))
            {
                var cookie = new HttpCookie("WelcomeName" + HttpUtility.UrlEncode(_userId)) { Value = _userFriendlyName, Path = String.Format("{0}QvAJAXZfc/", AccessPointServer) };
                Response.Cookies.Add(cookie);
            }
            Results = true;

            return Results;
        }

        private void GetGroups()
        {
            var group = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(_userGroups))
            {
                group.Append("<GroupList>");

                foreach (string value in _userGroups.Split(';'))
                {
                    group.Append("<string>");
                    group.Append(value);
                    group.Append("</string>");
                }

                group.Append("</GroupList>");
                group.Append("<GroupsIsNames>");
                group.Append("true");
                group.Append("</GroupsIsNames>");
            }
            
            _userGroups = group.ToString();
        }

        /// <summary>
        /// Multiple selection reformatting, not used
        /// </summary>
        /// <param name="selections"></param>
        /// <returns></returns>
        private string GetSelections(string selections)
        {
            var selectionCollection = new StringBuilder();

            foreach (string value in selections.Split(';'))
            {
                selectionCollection.Append("&select=");
                selectionCollection.Append(value);
            }

            return selectionCollection.ToString();
        }

        /// <summary>
        /// Redirects to QlikView after succesfull retrieval of webticket
        /// </summary>
        /// <param name="document">QlikView document to open directly, bypassing AccessPoint</param>
        /// <param name="host">QlikView host name (as found in QEMC) is required when using document parameter</param>
        /// <param name="selections">Semicolon separated list of selections, ie: LB38,Yellow;LB39,Banana (Note: Only the first selection works at the moment)</param>
        /// <param name="OriginSystem">Set to the system that is launching this -either milliman user or covisint federated user</param>
        private void RedirectToQlikView(string document = "", string host = "", string selections = "", string OriginSystem = "milliman")
        {
            //http://hcintel.cloudapp.net/qvajaxzfc/opendoc.htm?document=PRTest.qvw&select=Document\InitialContext,"427903708Z"
            //document = "PRTest.qvw";  //test code
            //string URL = ExternalAccessPointServer + "/qvajaxzfc/authenticate.aspx?type=html&try=/qvajaxzfc/opendoc.htm?document=" + document + "&back=/LoginPage.htm&webticket=" + _webTicket + "&originsystem=" + OriginSystem;
            string SelectionURLFragment = "";
            if (string.IsNullOrEmpty(selections) == false)
                SelectionURLFragment = "&select=Document\\InitialContext,\"" + selections + "\"";

            string ExternalAccessPoint = System.Configuration.ConfigurationManager.AppSettings["ExternalServerName"];
            string URL = ExternalAccessPoint + "/qvajaxzfc/authenticate.aspx?type=html&try=" + Uri.EscapeDataString("/qvajaxzfc/opendoc.htm?document=" + document + SelectionURLFragment +  "&originsystem=" + OriginSystem) + "&back=/LoginPage.htm&webticket=" + _webTicket ;

            Response.Redirect(URL);
            
            if (String.IsNullOrEmpty(document))
                Response.Redirect(string.Format("{0}QvAJAXZfc/Authenticate.aspx?type=html&webticket={1}&try={2}&back={3}&originsystem={4}", AccessPointServer, _webTicket, TryUrl, BackUrl, OriginSystem));
            else
                Response.Redirect(string.Format("../QvAJAXZfc/Authenticate.aspx?type=html&webticket={1}&try={2}&back={3}", AccessPointServer, _webTicket, Uri.EscapeDataString("/QvAJAXZfc/AccessPoint.aspx?open=&id=" + host + "%7C" + document + selections + "&client=Ajax&originsystem=" + OriginSystem), BackUrl));
        }
    }
}