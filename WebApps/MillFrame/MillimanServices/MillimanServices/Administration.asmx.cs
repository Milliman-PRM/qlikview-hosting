using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using MillimanReportReduction;

namespace MillimanServices
{
    /// <summary>
    /// This interface defines the services as exposed via internet to the client.  We use soap transport to maintain max
    /// compatability will all remote clients both window and unix based.
    /// 
    /// Note: all calls made to the method "AddSystemMsg" are added to a queue that is visible via the external debugview page. 
    /// Clients can log into the site and monitor the debugview to determine the parameters passed, processing status and
    /// general debug info needed when calling our remote services. When this service is moved to production the debugview will
    /// not be active and will optimized away by the release mode compiler
    /// 
    /// Note: any routine that has an error return can call the GetLastError routine to get a readable, expanded error message
    /// 
    /// </summary>
    [WebService(Namespace = "http://prm.milliman.com/services/")]
    [WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    //[System.Web.Script.Services.ScriptService]
    public class Administration : System.Web.Services.WebService
    {

        private const string ERROR_TAG = "SERVICEERROR";
        public enum StatusEnum { ENABLED, DISABLED , ALL };

        //credentiasls used by external entity for access, each external entity will have different credentials so we can
        //determine from login who is calling, NextGen, Covisint, other....
        public SystemCredentials Credentials;

        /// <summary>
        /// Credentials are provided via SOAP injections, and are eventually passed to this routine,  if the system account
        /// is not validate we ignore the request
        /// </summary>
        /// <param name="Credentials"></param>
        /// <todo>For now we store the system account in the web.config,  someday should be in a system DB</todo>
        /// <returns></returns>
        private bool ValidateUser(SystemCredentials Credentials)
        {

            int Index = 0;
            Credentials.Owner = string.Empty;
            while (true)
            {
                string UserID = System.Configuration.ConfigurationManager.AppSettings["SystemID_" + Index.ToString()];
                if (string.IsNullOrEmpty(UserID))
                    break;
                string Pswd = System.Configuration.ConfigurationManager.AppSettings["Password_" + Index.ToString()];
                string Owner = System.Configuration.ConfigurationManager.AppSettings["Owner_" + Index.ToString()];
                if (string.Compare(Credentials.UserName, UserID, true) == 0)
                {
                    if (string.Compare(Credentials.UserPassword, Pswd, true) == 0)
                    {
                        Credentials.Owner = Owner;
                        return true;
                    }
                }
                Index++;
            }
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "System account " + Credentials.UserName + " could not be validated, please check the account name and password.");
            return false;
        }

        [System.Web.Services.Protocols.SoapHeader("Credentials")]
        [WebMethod(EnableSession = false, Description = "Get an error message for the last error encountered.")]
        public string GetLastError()
        {
            string ErrorMsg = @"No error found.";
            try
            {
                lock (Milliman.Global.ClientActionLock)
                {
                    List<Milliman.Global.SystemMsg> Messages = Milliman.Global.GetClientActions();
                    for (int Index = Messages.Count - 1; Index >= 0; Index--)
                    {
                        if (Messages[Index].MType == Milliman.Global.MsgType.ERROR)
                        {
                            ErrorMsg = Messages[Index].Msg;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.ToString();
                Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, ErrorMsg);
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Service-GetLastError", ex);
            }
            return ErrorMsg;
        }

        /// <summary>
        /// Called to enable or disable a group of user as identified by the clientid
        /// </summary>
        /// <param name="AdminEmail">Used for auditing</param>
        /// <param name="ClientID">NextGen client id - must be mapped to Milliman ID</param>
        /// <param name="ClientName">Free form text client human readable name</param>
        /// <param name="Enable">enable or disable</param>
        /// <returns></returns>
        [System.Web.Services.Protocols.SoapHeader("Credentials")]
        [WebMethod(EnableSession = false, Description = "Enable/disable client users access to reports")]
        public bool ClientAccess( string AdminEmail,
                                  Guid ClientID,
                                  string ClientName,
                                  bool Enable)
        {
            string MethodName = "ClientAccess";
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(" + AdminEmail + "," + ClientID.ToString() + "," + ClientName + "," + Enable.ToString() + ")");
            bool ReturnValue = false;
            try
            {
                if (ValidateUser(Credentials) == true)
                {
                    //processing content goes here
                    if (string.IsNullOrEmpty(AdminEmail) == true)
                    {
                        Milliman.Global.AddSystemMsg( Milliman.Global.MsgType.ERROR, "AdminEmail parameter cannot be left empty.");
                    }
                    else if (Milliman.DebugHelper.IsValidExternallyOwnedGroup(ClientID.ToString(), Credentials.Owner) == false)
                    {
                        //do nothing, helper reports problem
                    }
                    //mark it as requested
                    Milliman.Data.ExternalSystemGroups.GetInstance().NextGenAssociatedGroups[ClientID.ToString()].Enabled = Enable;
                    Milliman.Data.ExternalSystemGroups.GetInstance().Save();  //persist it
                    ReturnValue = true;
                }
            }
            catch (Exception ex)
            {
                Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, MethodName + " error:<br> " + ex.ToString());
            }
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + " returned " + ReturnValue.ToString());
            return ReturnValue;
        }

        ///// <summary>
        ///// Returned in list from GetClients to show client status, must be cleanly structured and serializable to be used
        ///// via SOAP.  Used to help daemons sync external systems with Milliman system
        ///// </summary>
        [System.Serializable]
        public class ClientID_NameMap
        {
            public Guid ClientID { get; set; }     //NextGen ID
            public string ClientName { get; set; } //human readable name
            public bool Enabled { get; set; }      //enabled/disabled on Milliman server

            public ClientID_NameMap()
            {
                ClientID = Guid.Empty;
                ClientName = string.Empty;
                Enabled = false;
            }

            public ClientID_NameMap(Guid _ClientID, string _ClientName, bool _Enabled)
            {
                ClientID = _ClientID;
                ClientName = _ClientName;
                Enabled = _Enabled;
            }
        }

        /// <summary>
        /// Get a list of all clients that have been added to the Milliman server
        /// </summary>
        /// <param name="AdminEmail">used for auditing</param>
        /// <param name="ClientStatus">allows geting a list of ALL clients, ENABLED clients or DISABLED clients</param>
        /// <returns> a list of ClientID_NameMap objects filtered on input criteria</returns>
        [System.Web.Services.Protocols.SoapHeader("Credentials")]
        [WebMethod(EnableSession = false, Description = "Return a list of all clients that meet the specified criteria")]
        public List<ClientID_NameMap> GetClients(string AdminEmail,
                                                 StatusEnum ClientStatus)
        {
            string MethodName = "GetClients";
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(" + AdminEmail + "," + ClientStatus.ToString() + ")");
            List<ClientID_NameMap> ReturnValue = null;
            try
            {
                if (ValidateUser(Credentials) == true)
                {
                    //processing content goes here
                    if (string.IsNullOrEmpty(AdminEmail) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "AdminEmail parameter cannot be left empty.");
                    }
                    ReturnValue = new List<ClientID_NameMap>();
                    Milliman.Data.ExternalSystemGroups ESG = Milliman.Data.ExternalSystemGroups.GetInstance();
                    foreach (KeyValuePair<string, Milliman.Data.ExternalSystemGroups.ExternalSystemGroup> KVP in ESG.NextGenAssociatedGroups)
                    {
                        if ( ( KVP.Value.Enabled ) && (ClientStatus == StatusEnum.ENABLED ) )
                            ReturnValue.Add(new ClientID_NameMap(new Guid(KVP.Key), KVP.Value.Name, KVP.Value.Enabled));
                        else if ( ( KVP.Value.Enabled == false ) && (ClientStatus == StatusEnum.DISABLED ) )
                            ReturnValue.Add(new ClientID_NameMap(new Guid(KVP.Key), KVP.Value.Name, KVP.Value.Enabled));
                        else if ( ClientStatus == StatusEnum.ALL )
                            ReturnValue.Add(new ClientID_NameMap(new Guid(KVP.Key), KVP.Value.Name, KVP.Value.Enabled));
                    }
                }
            }
            catch (Exception ex)
            {
                Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, MethodName + " error:<br> " + ex.ToString());
            }
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + " returned " + ReturnValue.Count.ToString() + " client entries");
            return ReturnValue;
        }

        /// <summary>
        /// Find the correct master report in the group by using the predefinded ReportID
        /// </summary>
        /// <param name="GroupName">Milliman group name</param>
        /// <param name="ReportID">represents which report to access</param>
        /// <todo>To support multiple report types, like CCR and cost model,  we need to sign reports with a signature that
        /// indicates the type of report,  that can then be matched with this ID to ensure the correct report is being
        /// served.  For now we only support 1 report type per group for external entities,  the CCR report</todo>
        /// <WARNING>see todo above - potential future bug</WARNING>
        /// <returns>Fully master report with qualafied path</returns>
        private string FindQualafiedMasterQVW(string GroupName, string ReportID)
        {
            //we have 2 IDs for CCR,  one for NextGen and the other kept for backward compat for Covisint code
            if ( (string.Compare(ReportID,"CARECOORD",true) == 0) || (string.Compare(ReportID,"POPULATION",true) == 0) )
            {
                MillimanCommon.UserRepo Repo = MillimanCommon.UserRepo.GetInstance();
                List<string> Projects = Repo.FindAllQVProjectsForRole(GroupName);
                //for initial mapped projects we only support 1 QVW within the group,  if external entities need to access
                //multiple projects then we need to sign the QVWs with a type id that can associate this info
                if (Projects.Count() == 1)
                    return Projects[0];
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Current mapping to allow external entities to access QVWs vis SSO using admin API only supports 1 qvw per group.  However this group '" + GroupName + "' contains " + Projects.Count().ToString() + " projects");
            }
            return "";
        }
        /// <summary>
        /// Creates the directory the reduced qvw will reside in, and returns its name and qualified path.
        /// </summary>
        /// <param name="MasterQVW">Qualafied path report name of master qvw</param>
        /// <returns>Fully qualafied path to reduced QVW, or empty string on failure</returns>
        private string GetNameAndCreateDir(string MasterQVW, string UserID)
        {
            string RetVal = string.Empty;
            try
            {
                string ReducedPathAndName = MillimanCommon.ReducedQVWUtilities.GetUsersReducedQVWName(MasterQVW, UserID);
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ReducedPathAndName));
                return ReducedPathAndName;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "GetNameAndCreateDir", ex);
            }
            return RetVal;
        }
        /// <summary>
        /// Reduce a report down based on the reportid,  group,  provider ids and configured data model field
        /// </summary>
        /// <param name="AuditUserID">used for auditing</param>
        /// <param name="AccountName">user account that the report is being reduced for</param>
        /// <param name="GroupName">Milliman group that will house the reduced report and user</param>
        /// <param name="ProviderIDs">NextGen Ids provided as criteria for reduction</param>
        /// <param name="ReportID">Id provided to determine what type of report,  CCR, cost model....</param>
        /// <notes>The QVW field to reduce on is loaded via the "DataModelReductionKey" located in the web.config, when
        /// moving toward reduction of differnt types of reports,  the reduction key should be housed in the signature</notes>
        /// <returns>true for success, false otherwise</returns>
        protected bool ReduceReport(string AuditUserID, string AccountName, string GroupName, List<string> ProviderIDs, string ReportID)
        {
            QVWReducer Reduce = new QVWReducer();
            Reduce.QualifiedQVWNameToReduce = FindQualafiedMasterQVW(GroupName, ReportID );
            Reduce.QualifiedReducedQVWName = GetNameAndCreateDir(Reduce.QualifiedQVWNameToReduce, AccountName);
            string DataModelReductionKey = System.Configuration.ConfigurationManager.AppSettings["DataModelReductionKey"];
            Reduce.Variables = new List<NVPairs>();
            foreach( string ProviderID in ProviderIDs )
                Reduce.Variables.Add(new NVPairs(DataModelReductionKey, ProviderID));
            Reduce.TaskID = Guid.NewGuid();
            Reduce.DeleteTaskOnCompletion = true;
            Reduce.TaskName = AuditUserID + ":" + AccountName;
            Reduce.TaskDescription = AuditUserID + " reducing for " + AccountName + " on report " + System.IO.Path.GetFileName(Reduce.QualifiedReducedQVWName);

            return Reduce.ReduceBlocking();
        }
        /// <summary>
        /// this routine emulates the production system and allows testing in a more realistic manner - the real reduction
        /// code cannot be run against NextGen because the data has not been passed to Milliman for report generation and 
        /// then cycled back into the system for testing,  as a result all data passed in by NextGen is completly out of sync with
        /// data housed in the test QVW,  thuse impossible to reduce.  This routine emulates reduction on a small subset of
        /// data made known to NextGen for simple testing of adding users, reduction and then access via SSO
        /// </summary>
        /// <param name="AccountName"></param>
        /// <param name="QualafiedQVW"></param>
        /// <param name="ProviderIDs"></param>
        protected bool EmulateProduction(string AccountName, string GroupName, List<string> ProviderIDs)
        {
            string QualafiedQVW = System.IO.Path.Combine(Server.MapPath("TestQVWs"), "Care Coordinator Report.qvw");
    
            foreach (string PI in ProviderIDs)
            {
                string Provider = PI;
                //empty provider means give access to all
                if (string.IsNullOrEmpty(Provider) == true)
                    Provider = "Care Coordinator Report";

                string TestQVW = System.IO.Path.Combine(Server.MapPath("TestQVWs"), Provider + ".qvw");
                if (System.IO.File.Exists(TestQVW))
                {
                    //copy over as reduced file
                    string QVWPath = System.IO.Path.GetDirectoryName(QualafiedQVW);
                    string ValidUserID = MillimanCommon.Utilities.ConvertStringToHex(AccountName);
                    string ReducedQVW = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"], GroupName, "ReducedUserQVWs", ValidUserID, System.IO.Path.GetFileName(QualafiedQVW));
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ReducedQVW));
                    if (System.IO.File.Exists(ReducedQVW))
                        System.IO.File.Delete(ReducedQVW);
                    System.IO.File.Copy(TestQVW, ReducedQVW);
                    Random R = new Random();
                    System.Threading.Thread.Sleep(8000 + R.Next(1, 5));

                    //check to see if the copied versione xists
                    if (System.IO.File.Exists(ReducedQVW) == false)
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to copy file - " + TestQVW + " to " + ReducedQVW);
                        return false;
                    }
                }
                else
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Test QVW does not exist - " + TestQVW);
                    return false; //baseline file did not exist
                }
            }

            //this isn't exactly correctly, but the most bad that can happend is 1 document is authorized
            try
            {
                MillimanReportReduction.QVSAPI QV = new MillimanReportReduction.QVSAPI();
                bool Verified = false;
                QV.AuthorizeAllDocuments("", out Verified);
                if (Verified == false)
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to authorize with QV server ");
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Authorization failure", ex);
            }

            return true;  
        }

        /// <summary>
        /// Add a user to the Milliman system from the NextGen system such that the user's account is associated with a report 
        /// ( possibly reduced ) and is accessable via SSO
        /// </summary>
        /// <param name="AdminEmail">used for auditing</param>
        /// <param name="ClientID">NextGen client id</param>
        /// <param name="UserName">Name of user requesting access</param>
        /// <param name="ReportID">Type of report the user needs access to</param>
        /// <param name="ProviderIDs">Providers the report should be limited to</param>
        /// <param name="Enable">By default a user can be enabled or disabled</param>
        /// <returns></returns>
        [System.Web.Services.Protocols.SoapHeader("Credentials")]
        [WebMethod(EnableSession = false, Description = "Allows a user to be added for a client, specific report and specific provider ids")]
        public bool AddUser(string AdminEmail,
                                  Guid ClientID,
                                  string UserName,
                                  string ReportID,
                                  List<string> ProviderIDs,
                                  bool Enable)
        {
            string MethodName = "AddUser";
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(" + AdminEmail + "," + ClientID.ToString() + "," + UserName + "," + ReportID + "," + Milliman.DebugHelper.ParamterListHelper(ProviderIDs) + "," + Enable.ToString() + ")");
            bool ReturnValue = false;
            bool IsNewUser = false;
            try
            {
                if (ValidateUser(Credentials) == true)
                {
                    //processing content goes here
                    if (string.IsNullOrEmpty(AdminEmail) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "AdminEmail parameter cannot be left empty.");
                    }
                    else if (Milliman.DebugHelper.IsValidExternallyOwnedGroup(ClientID.ToString(), Credentials.Owner) == false)
                    {
                        //do nothing, helper reports problem
                    }
                    else if (string.IsNullOrEmpty(UserName) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Username parameter cannot be left empty.");
                    }
                    else if (string.IsNullOrEmpty(ReportID) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "ReportID parameter cannot be left empty.");
                    }

                    bool TestMode = System.Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["TestMode"]);

                    //check to see if user already exists in membership api, if so, it's not an error, this is called once for each report they should access
                    Milliman.Data.ExternalGroupToInternalGroupMap EGTIGM = Milliman.Data.ExternalGroupToInternalGroupMap.GetInstance(); 
                    MembershipUser MU = Membership.GetUser(UserName);
                    if ( MU == null )
                    {
                        string EP = System.Configuration.ConfigurationManager.AppSettings["ExternalUserSSOPassword"];
                        if (TestMode == false)
                            EP = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

                        MU = Membership.CreateUser(UserName, EP, UserName);
                        //have to use the mapping class and map external group id to internal group id
                        Roles.AddUserToRole(UserName, EGTIGM.FindInternalGroup(ClientID));
                        IsNewUser = true;
                    }
                    Milliman.Data.ExternalUsers EU = Milliman.Data.ExternalUsers.GetInstance();
                    if (EU != null)
                    {
                       Milliman.Data.ExternalUsers.ExternalUser ExUser = null;
                       if ( EU.ContainsKey(UserName) == false )
                        {
                            ExUser = new Milliman.Data.ExternalUsers.ExternalUser();
                            ExUser.UserName = UserName;
                            ExUser.UserID = MU.ProviderUserKey.ToString();
                            ExUser.Owner = Credentials.Owner;
                            ExUser.Reports = new Dictionary<string,Milliman.Data.ExternalUsers.ExternalReport>();
                            EU.AddUser(ExUser);
                            //only save if user info has changed 
                            EU.Save();
                        }
                        ExUser = EU.GetUser(UserName);
                        if ( ExUser.Reports.ContainsKey(ReportID) == false )
                        {
                            Milliman.Data.ExternalUsers.ExternalReport ExReport = new Milliman.Data.ExternalUsers.ExternalReport();
                            ExReport.OrginGroupName = EGTIGM.FindInternalGroup(ClientID);
                            foreach ( string PI in ProviderIDs )
                                ExReport.ProviderIDs.Add(PI);
                            ExReport.ReportID = ReportID;
                            ExUser.Reports.Add(ReportID, ExReport);
                        }
                    }
                    //add rule for user to access report ( add to role, must map guid id to internal milliman id )

                    if (Roles.IsUserInRole(UserName, EGTIGM.FindInternalGroup(ClientID)) == false)
                        Roles.AddUserToRole(UserName, EGTIGM.FindInternalGroup(ClientID));

                    bool Created = false;
                    
                    //if in testmode we run the emulator, this so we can do basic testing without need of QVWs with consistent data
                    if (TestMode)
                        Created = EmulateProduction(UserName, EGTIGM.FindInternalGroup(ClientID), ProviderIDs);
                    else //otherwise QVW should have consistent data since all data originated from NextGen clients
                        Created = ReduceReport(AdminEmail, UserName, EGTIGM.FindInternalGroup(ClientID), ProviderIDs, ReportID);

                    //set user to enabled/disabled
                    MU.IsApproved = Enable;

                    Membership.UpdateUser(MU);

                    ReturnValue = Created;
                }
            }
            catch (Exception ex)
            {
                Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, MethodName + "(User:" + UserName + ")" + " error:<br> " + ex.ToString());
                //attempt to rollback the membership api
                if (IsNewUser && (Membership.GetUser(UserName) != null))
                {
                    Membership.DeleteUser(UserName);
                }
            }
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(User:" + UserName + ")" + " returned " + ReturnValue.ToString());
            return ReturnValue;
        }

        /// <summary>
        /// Modify the users access to change report types of data the user has access to ( providerIDs )
        /// </summary>
        /// <param name="AdminEmail">used for auditing</param>
        /// <param name="ClientID">NextGen client id</param>
        /// <param name="UserName">user name to modify access to</param>
        /// <param name="ReportID">the requested report type</param>
        /// <param name="ProviderIDs">list of provider ids to limit access to</param>
        /// <returns></returns>
        [System.Web.Services.Protocols.SoapHeader("Credentials")]
        [WebMethod(EnableSession = false, Description = "Allows a users providers ids to be changed associated with a report")]
        public bool ModifyUserProviders(string AdminEmail,
                                  Guid ClientID,
                                  string UserName,
                                  string ReportID,
                                  List<string> ProviderIDs)
        {
            string MethodName = "ModifyUserProviders";
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(" + AdminEmail + "," + ClientID.ToString() + "," + UserName + "," + ReportID + "," + Milliman.DebugHelper.ParamterListHelper(ProviderIDs) + ")");
            bool ReturnValue = false;
            try
            {
                if (ValidateUser(Credentials) == true)
                {
                    Milliman.Data.ExternalGroupToInternalGroupMap EGTIGM = Milliman.Data.ExternalGroupToInternalGroupMap.GetInstance(); 

                    //processing content goes here
                    if (string.IsNullOrEmpty(AdminEmail) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "AdminEmail parameter cannot be left empty.");
                    }
                    else if (Milliman.DebugHelper.IsValidExternallyOwnedGroup(ClientID.ToString(), Credentials.Owner) == false)
                    {
                        //do nothing, helper reports problem
                    }
                    else if (string.IsNullOrEmpty(UserName) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Username parameter cannot be left empty.");
                    }
                    else if (Membership.GetUser(UserName) == null)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Specified user was not found in membership");
                    }
                    else if (Roles.IsUserInRole(UserName, EGTIGM.FindInternalGroup(ClientID)) == false)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Specified user is not associated with client id");
                    }
                    else if (string.IsNullOrEmpty(ReportID) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "ReportID parameter cannot be left empty.");
                    }

                    //process it here - emptylist of providers means everything
                    Milliman.Data.ExternalUsers EU = Milliman.Data.ExternalUsers.GetInstance();
                    if ((EU != null) && (EU.ContainsKey(UserName)))
                    {
                        Milliman.Data.ExternalUsers.ExternalUser ExUser = EU.GetUser(UserName);
                        if (ExUser.Reports.ContainsKey(ReportID))
                        {
                            Milliman.Data.ExternalUsers.ExternalReport exReport = ExUser.Reports[ReportID];
                            exReport.ProviderIDs.Clear();
                            //re-generate report HERE
                            //request reduction on report - emptylist of providers means everything
                            bool TestMode = System.Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["TestMode"]);

                            bool Created = false;

                            //if in testmode we run the emulator, this so we can do basic testing without need of QVWs with consistent data
                            if (TestMode)
                                Created = EmulateProduction(UserName, EGTIGM.FindInternalGroup(ClientID), ProviderIDs);
                            else //otherwise QVW should have consistent data since all data originated from NextGen clients
                                Created = ReduceReport(AdminEmail, UserName, EGTIGM.FindInternalGroup(ClientID), ProviderIDs, ReportID);

                            foreach (string PI in ProviderIDs)
                                exReport.ProviderIDs.Add(PI);

                            EU.Save();
                        }
                        else  //error
                        {
                            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Requested to change user/report provider ids,  but report id was not found.  Is user associated with report?");
                            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Requested to change user/report provider ids,  but report id was not found.  Is user associated with report?");
                        }
                    }
                    else
                    {  //error condition
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Requested to change user/report provider ids,  but user is not tagged as external.  Has user been associated with clientID?");
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Requested to change user/report provider ids,  but user is not tagged as external.  Has user been associated with clientID?");
                    }
                    ReturnValue = true;
                }
            }
            catch (Exception ex)
            {
                Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, MethodName + "(User:" + UserName + ")" + " error:<br> " + ex.ToString());
            }
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(User:" + UserName + ")" + " returned " + ReturnValue.ToString());
            return ReturnValue;
        }

        /// <summary>
        /// Remove the users access to a report
        /// </summary>
        /// <param name="AdminEmail">used for auditing purposes</param>
        /// <param name="ClientID">NextGen client id</param>
        /// <param name="UserName">account to remove</param>
        /// <param name="ReportID">report id</param>
        /// <returns>true for success, false otherwise</returns>
        [System.Web.Services.Protocols.SoapHeader("Credentials")]
        [WebMethod(EnableSession = false, Description = "Remove a user's access to a report")]
        public bool RemoveUserReportAccess( string AdminEmail,
                                            Guid ClientID,
                                            string UserName,
                                            string ReportID )
        {
            string MethodName = "RemoveUserReportAccess";
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(" + AdminEmail + "," + ClientID.ToString() + "," + UserName + "," + ReportID + ")");
            bool ReturnValue = false;
            try
            {
                if (ValidateUser(Credentials) == true)
                {
                    Milliman.Data.ExternalGroupToInternalGroupMap EGTIGM = Milliman.Data.ExternalGroupToInternalGroupMap.GetInstance(); 

                    //processing content goes here
                    if (string.IsNullOrEmpty(AdminEmail) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "AdminEmail parameter cannot be left empty.");
                    }
                    else if (Milliman.DebugHelper.IsValidExternallyOwnedGroup(ClientID.ToString(), Credentials.Owner) == false)
                    {
                        //do nothing, helper reports problem
                    }
                    else if (string.IsNullOrEmpty(UserName) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Username parameter cannot be left empty.");
                    }
                    else if (Membership.GetUser(UserName) == null)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Specified user was not found in membership");
                    }
                    else if (Roles.IsUserInRole( UserName, EGTIGM.FindInternalGroup(ClientID)) == false)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Specified user is not associated with client id");
                    }
                    else if (string.IsNullOrEmpty(ReportID) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "ReportID parameter cannot be left empty.");
                    }

                    //process it here - emptylist of providers means everything
                    //process it here - emptylist of providers means everything
                    Milliman.Data.ExternalUsers EU = Milliman.Data.ExternalUsers.GetInstance();
                    if ((EU != null) && (EU.ContainsKey(UserName)))
                    {
                        Milliman.Data.ExternalUsers.ExternalUser ExUser = EU.GetUser(UserName);
                        if (ExUser.Reports.ContainsKey(ReportID))
                        {
                            ExUser.Reports.Remove(ReportID);
                            EU.Save();
                        }
                        else  //error
                        {
                            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Requested to remove report access from user,  but report id was not found.  Is user associated with report?");
                            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Requested to remove report access from user,  but report id was not found.  Is user associated with report?");
                        }
                    }
                    else
                    {  //error condition
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Requested to remove report access from user,  but user is not tagged as external.  Has user been associated with clientID?");
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Requested to remove report access from user,  but user is not tagged as external.  Has user been associated with clientID?");
                    }
                    ReturnValue = true;
                }
            }
            catch (Exception ex)
            {
                Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, MethodName + "(User:" + UserName + ")" + " error:<br> " + ex.ToString());
            }
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(User:" + UserName + ")" + " returned " + ReturnValue.ToString());
            return ReturnValue;
        }

        /// <summary>
        /// Disable a specific user's account
        /// </summary>
        /// <param name="AdminEmail">used for auditing</param>
        /// <param name="ClientID">NextGen client ID</param>
        /// <param name="UserName">User to disable</param>
        /// <returns>true on success, false otherwise</returns>
        [System.Web.Services.Protocols.SoapHeader("Credentials")]
        [WebMethod(EnableSession = false, Description = "Disable the user's account")]
        public bool DisableUser(string AdminEmail,
                                  Guid ClientID,
                                  string UserName)
        {
            string MethodName = "DisableUser";
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(" + AdminEmail + "," + ClientID.ToString() + "," + UserName +  ")");
            bool ReturnValue = false;
            try
            {
                if (ValidateUser(Credentials) == true)
                {
                    Milliman.Data.ExternalGroupToInternalGroupMap EGTIGM = Milliman.Data.ExternalGroupToInternalGroupMap.GetInstance(); 

                    //processing content goes here
                    if (string.IsNullOrEmpty(AdminEmail) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "AdminEmail parameter cannot be left empty.");
                    }
                    else if (Milliman.DebugHelper.IsValidExternallyOwnedGroup(ClientID.ToString(), Credentials.Owner) == false)
                    {
                        //do nothing, helper reports problem
                    }
                    else if (string.IsNullOrEmpty(UserName) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Username parameter cannot be left empty.");
                    }
                    else if (Membership.GetUser(UserName) == null)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Specified user was not found in membership");
                    }
                    else if (Roles.IsUserInRole(UserName, EGTIGM.FindInternalGroup(ClientID)) == false)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Specified user is not associated with client id");
                    }

                    //process it here - emptylist of providers means everything
                    MembershipUser MU = Membership.GetUser(UserName);
                    if (MU != null)
                    {
                        MU.IsApproved = false;
                        Membership.UpdateUser(MU);
                    }
                    else  //error missing user
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Requested to disable user,  but user is not tagged as external.  Has user been associated with clientID?");
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Requested to disable from user,  but user is not tagged as external.  Has user been associated with clientID?");

                    }
                    ReturnValue = true;
                }
            }
            catch (Exception ex)
            {
                Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, MethodName + "(User:" + UserName + ")" + " error:<br> " + ex.ToString());
            }
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(User:" + UserName + ")" + " returned " + ReturnValue.ToString());
            return ReturnValue;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="AdminEmail">used for auditing </param>
        /// <param name="ClientID">NextGen client id</param>
        /// <param name="UserName">Name of user to delete</param>
        /// <returns>true on success, false otherwise</returns>
        [System.Web.Services.Protocols.SoapHeader("Credentials")]
        [WebMethod(EnableSession = false, Description = "Delete the user's account")]
        public bool DeleteUser(string AdminEmail,
                                  Guid ClientID,
                                  string UserName)
        {
            string MethodName = "DeleteUser";
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(" + AdminEmail + "," + ClientID.ToString() + "," + UserName + ")");
            bool ReturnValue = false;
            try
            {
                if (ValidateUser(Credentials) == true)
                {
                    Milliman.Data.ExternalGroupToInternalGroupMap EGTIGM = Milliman.Data.ExternalGroupToInternalGroupMap.GetInstance(); 

                    //processing content goes here
                    if (string.IsNullOrEmpty(AdminEmail) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "AdminEmail parameter cannot be left empty.");
                    }
                    else if (Milliman.DebugHelper.IsValidExternallyOwnedGroup(ClientID.ToString(), Credentials.Owner) == false)
                    {
                        //do nothing, helper reports problem
                    }
                    else if (string.IsNullOrEmpty(UserName) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Username parameter cannot be left empty.");
                    }
                    else if (Membership.GetUser(UserName) == null)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Specified user was not found in membership");
                    }
                    else if (Roles.IsUserInRole(UserName, EGTIGM.FindInternalGroup(ClientID)) == false)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Specified user is not associated with client id");
                    }
                    //process it here - emptylist of providers means everything
                    MembershipUser MU = Membership.GetUser(UserName);
                    if (MU != null)
                    {
                        Membership.DeleteUser(UserName);
                        Milliman.Data.ExternalUsers EU = Milliman.Data.ExternalUsers.GetInstance();
                        if (EU != null)
                        {
                            if (EU.ContainsKey(UserName))
                            {
                                EU.RemoveUser(UserName);
                                EU.Save();
                            }
                        }
                    }
                    else  //error missing user
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Requested to delete user,  but user was not found in membership");
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Requested to delete user,  but user was not found in membership");
                    }
                    //process it here - emptylist of providers means everything
                    ReturnValue = true;
                }
            }
            catch (Exception ex)
            {
                Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, MethodName + "(User:" + UserName + ")" + " error:<br> " + ex.ToString());
            }
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(User:" + UserName + ")" + " returned " + ReturnValue.ToString());
            return ReturnValue;
        }
        
        /// <summary>
        /// Provide a clean structure to serialze back to external entity that shows the association of provider ids to a 
        /// report as stored on the Milliman server
        /// </summary>
        [System.Serializable]
        public class Report
        {
            public string ReportID;
            public List<string> ProviderIDs;  //NextGen IDs

            public void TestData()
            {
                ReportID = "CARECOORD";
                ProviderIDs = new List<string>();
                for (int Index = 0; Index < 3; Index++)
                {
                    ProviderIDs.Add("Provider_" + Index.ToString());
                }
            }
        }
        /// <summary>
        /// Provide a clean structure for serializing back to external enity that shows the relationship of a user account
        /// as associated with a NextGen client id,  and reports they can access
        /// </summary>
        [System.Serializable]
        public class MillimanUser
        {
            public string UserName;
            public Guid ClientID;  //NextGen ID
            public bool Enabled;
            public List<Report> Reports;

            public void TestData()
            {
                UserName = "qbert";
                ClientID = Guid.NewGuid();
                Enabled = true;
                Reports = new List<Report>();
                Reports.Add(new Report());
                Reports[0].TestData();
                Reports.Add(new Report());
                Reports[1].TestData();
            }
        }

        /// <summary>
        /// Get a list of all users for the NextGen client id along with associated report/provider ids
        /// </summary>
        /// <param name="AdminEmail">used for auditing</param>
        /// <param name="ClientID">NextGen client id</param>
        /// <param name="UserStatus">Get a filtered list based on ALL users, ENABLED users, DISABLED users</param>
        /// <returns>List of Milliman users as specified by filtering criteria</returns>
        [System.Web.Services.Protocols.SoapHeader("Credentials")]
        [WebMethod(EnableSession = false, Description = "Get a list of all users for clients that match the criteria specified")]
        public List<MillimanUser> GetUsers(string AdminEmail,
                                           List<Guid> ClientID,
                                           StatusEnum UserStatus)
        {
            string MethodName = "GetUsers";
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(" + AdminEmail + "," + Milliman.DebugHelper.ParamterListHelper(ClientID) + "," + UserStatus.ToString() + ")");
            List<MillimanUser> ReturnValue = null;
            try
            {
                if (ValidateUser(Credentials) == true)
                {
                    //processing content goes here
                    if (string.IsNullOrEmpty(AdminEmail) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "AdminEmail parameter cannot be left empty.");
                    }
                    else if (ClientID == null)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "At least one ClientID must be specified as a parameter");
                    }

                    bool AllGroupsValid = true;
                    foreach (Guid S in ClientID)
                    {
                        if (Milliman.DebugHelper.IsValidExternallyOwnedGroup(S.ToString(), Credentials.Owner) == false)
                        {
                            AllGroupsValid = false;
                        }
                    }

                    if (AllGroupsValid)
                    {
                        Milliman.Data.ExternalGroupToInternalGroupMap EGTIGM = Milliman.Data.ExternalGroupToInternalGroupMap.GetInstance(); 

                        ReturnValue = new List<MillimanUser>();
                        foreach (Guid _ClientID in ClientID)
                        {
                            string[] AllForClient = Roles.FindUsersInRole(EGTIGM.FindInternalGroup(_ClientID), "%");
                            foreach (string UserName in AllForClient)
                            {
                                MillimanUser MU = FindUser(AdminEmail, _ClientID, UserName);
                                if (MU != null)
                                {
                                    if (UserStatus == StatusEnum.ALL)
                                        ReturnValue.Add(MU);
                                    else if ((UserStatus == StatusEnum.DISABLED) && (MU.Enabled == false))
                                        ReturnValue.Add(MU);
                                    else if ((UserStatus == StatusEnum.ENABLED) && (MU.Enabled == true))
                                        ReturnValue.Add(MU);
                                }
                                else
                                {
                                    MillimanCommon.Report.Log( MillimanCommon.Report.ReportType.Warning, "User " + UserName + " was found in membership but did not have a corresponding external account description.  Possible system sync error");
                                }
                            }
                        }
                    }  
                }
            }
            catch (Exception ex)
            {
                Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, MethodName + " error:<br> " + ex.ToString());
            }
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + " returned " + ReturnValue.Count.ToString() + " users");
            return ReturnValue;
        }

        /// <summary>
        /// Find a specific user based on the criteria specified
        /// </summary>
        /// <param name="AdminEmail">used for auditing</param>
        /// <param name="ClientID">NextGen client id</param>
        /// <param name="UserName">User to search for</param>
        /// <returns>A Milliman user class instance populated status as stored by the Milliman server</returns>
        [System.Web.Services.Protocols.SoapHeader("Credentials")]
        [WebMethod(EnableSession = false, Description = "Find the user based on User ID")]
        public MillimanUser FindUser(string AdminEmail,
                                     Guid   ClientID,
                                     string UserName )
        {
            string MethodName = "FindUser";
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(" + AdminEmail + "," + UserName + ")");
            MillimanUser ReturnValue = null;
            try
            {
                Milliman.Data.ExternalGroupToInternalGroupMap EGTIGM = Milliman.Data.ExternalGroupToInternalGroupMap.GetInstance();

                if (ValidateUser(Credentials) == true)
                {
                    //processing content goes here
                    if (string.IsNullOrEmpty(AdminEmail) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "AdminEmail parameter cannot be left empty.");
                    }
                    else if (string.IsNullOrEmpty(UserName) == true)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Username parameter cannot be left empty.");
                    }
                    else if (Membership.GetUser(UserName) == null)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Specified user was not found in membership");
                    }
                    else if (Roles.IsUserInRole(UserName, EGTIGM.FindInternalGroup(ClientID)) == false)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Specified user is not associated with client id");
                    }
                    else
                    {
                        //process here
                        Milliman.Data.ExternalUsers EU = Milliman.Data.ExternalUsers.GetInstance();
                        MembershipUser MU = Membership.GetUser(UserName);
                        if ((MU != null) && (EU != null))
                        {
                            if (EU.ContainsKey(UserName))
                            {
                                Milliman.Data.ExternalUsers.ExternalUser ExUser = EU.GetUser(UserName);
                                if (ExUser != null)
                                {
                                    ReturnValue = new MillimanUser();
                                    ReturnValue.ClientID = ClientID;
                                    ReturnValue.Enabled = MU.IsApproved;
                                    ReturnValue.UserName = UserName;
                                    ReturnValue.Reports = new List<Report>();
                                    foreach (KeyValuePair<string, Milliman.Data.ExternalUsers.ExternalReport> ER in ExUser.Reports)
                                    {
                                        Report R = new Report();
                                        R.ReportID = ER.Key;
                                        R.ProviderIDs = new List<string>();
                                        foreach (string PI in ER.Value.ProviderIDs)
                                            R.ProviderIDs.Add(PI);
                                        ReturnValue.Reports.Add(R);
                                    }
                                }
                                else
                                {  //external user not found
                                    Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "Requested to find user,  but user was not found with external user attribute");
                                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Requested to find user,  but user was not found with external user attribute");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, MethodName + " error:<br> " + ex.ToString());
            }
            if ( ReturnValue != null )
                Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + " returned user information" );
            else
                Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + " returned without finding user");

            return ReturnValue;
        }

        /// <summary>
        /// Used to see if server is alive, only method that does not currently require credentials
        /// </summary>
        /// <returns></returns>
        [System.Web.Services.Protocols.SoapHeader("Credentials")]
        [WebMethod(EnableSession = false, Description = "Check the Milliman servers accessability")]
        public bool Ping()
        {
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, Milliman.DebugHelper.MethodHelper("Ping (SystemAccount=" + Credentials.UserName + ")"));

            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, "Ping returned true<br>");
            return true;
        }

        /// <summary>
        /// Get a list of valid report IDs ( for now it's CARECOORD and POPULATION which both point to Care Coordinator Reports
        /// </summary>
        /// <param name="ClientID">NextGen client id</param>
        /// <returns>return a list of valid report IDs based on the NextGen client id</returns>
        [System.Web.Services.Protocols.SoapHeader("Credentials")]
        [WebMethod(EnableSession = false, Description = "Get a list of all report identifiers available for a client")]
        public List<string> GetValidReportIDs(Guid ClientID)
        {
            string MethodName = "GetValidReportIDs";
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + "(" + ClientID.ToString() + ")");
            List<string> ReturnValue = null;
            try
            {
                if (ValidateUser(Credentials) == true)
                {
                    //processing content goes here
                    if (ClientID == Guid.Empty)
                    {
                        Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, "ClientID parameter cannot be left empty.");
                    }
                    else
                    {
                        //process here
                        ReturnValue = new List<string>() ;
                        //should always be at least one
                        string ReportType = System.Configuration.ConfigurationManager.AppSettings["ReportType_0"];
                        int Index = 1;
                        while (string.IsNullOrEmpty(ReportType) == false)
                        {
                            ReturnValue.Add(ReportType);
                            ReportType = System.Configuration.ConfigurationManager.AppSettings["ReportType_" + Index.ToString()];
                            Index++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.ERROR, MethodName + " error:<br> " + ex.ToString());
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, MethodName, ex);
            }
            Milliman.Global.AddSystemMsg(Milliman.Global.MsgType.INFO, MethodName + " returned " + ReturnValue.Count.ToString() + " report ids " + Milliman.DebugHelper.ParamterListHelper(ReturnValue));
            return ReturnValue;
        }

    }
}
