using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Configuration;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using System.Collections.Generic;

public partial class Default : System.Web.UI.Page 
{
    public class UserItem
    {
        public string AccountName { get; set; }
        public string Status { get; set; }
        public string Tooltip { get; set; }
        public string Notes { get; set; }

        public bool IsAdmin { get; set; }

        public UserItem(string _AccountName, string _Status, string _Tooltip, string _Notes = "", bool _IsAdmin = false)
        {
            AccountName = _AccountName;
            Status = _Status;
            Tooltip = _Tooltip;
            Notes = _Notes;
            IsAdmin = _IsAdmin;
        }
    }

    /// <summary>
    /// given the users ID,  do they have to reset thier password and enter info
    /// </summary>
    /// <param name="UserID"></param>
    /// <returns></returns>
    private string UserStatus(string UserID, out string Tooltip )
    {
        Tooltip = string.Empty;
        try
        {
            string ResetFile = System.IO.Path.Combine(System.Web.Configuration.WebConfigurationManager.AppSettings["ResetUserInfoRoot"], UserID + ".rst");
            if (System.IO.File.Exists(ResetFile) == true)
            {
                string Contents = System.IO.File.ReadAllText(ResetFile);
                if (Contents.ToLower().Contains("nowelcome") == true)
                {
                    Tooltip = "User has not been sent a 'Welcome' email with information on how to access the PRM system. Click 'Reset Password' to email link.";
                    return "<center>No Welcome</center>";
                }
                else if (Contents.ToLower().Contains("welcome") == true)
                {
                    Tooltip = "User was sent a 'Welcome' email with access information but has never logged in";
                    return "<center>User has never logged in</center>";
                }
                Tooltip = "User requested password reset, but has not accessed PRM system via emailed link";
                return "<center>Password Reset</center>";
            }
        }
        catch (Exception)
        {

        }

        return "";
    }

    private List<UserItem> MembershipToUserItems(List<string> GroupNames)
    {
        List<string> AllUserList = new List<string>();
        List<UserItem> UI = new List<UserItem>();
        foreach (string GroupName in GroupNames)
        {
            string[] Users = Roles.GetUsersInRole(GroupName);
            foreach (string User in Users)
            {
                if (Roles.IsUserInRole(User, "Administrator") == true)
                    continue;

                if (AllUserList.Contains(User))
                    continue;

                AllUserList.Add(User);

                MembershipUser MU = Membership.GetUser(User);
                string Tooltip = "";
                string _Status = UserStatus(MU.ProviderUserKey.ToString(), out Tooltip);
                string Status = string.Empty;
                if (MU.IsApproved == false)
                {
                    Status = "<center>Suspended</center>";
                    Tooltip = "User account was suspended to prevent the user from logging into the PRM system.";
                }
                else if (MU.IsLockedOut)
                {
                    Status = "<center>Locked</center>";
                    Tooltip = "User account is locked due to an excess of login attempts using an incorrect password. User account will unlock itself within 10 minutes of locking.";
                }

                else if ((string.IsNullOrEmpty(Status)) && (string.IsNullOrEmpty(_Status) == false))
                {
                    Status = _Status;
                }
                else
                {
                    Status = "<center>Active</center>";
                    Tooltip = "User is active and last accessed the system on " + MU.LastActivityDate.ToString();
                }
                UI.Add(new UserItem(User, Status, Tooltip, GetAdminNote(User)));
            }
        }
        return AddGlobalUserAdmins(UI);
    }

    /// <summary>
    /// If I am logged in as a global admin, show me in the list with superman icon
    /// </summary>
    /// <param name="CurrentUsers"></param>
    /// <returns></returns>
    private List<UserItem> AddGlobalUserAdmins(List<UserItem> CurrentUsers)
    {
        if (Roles.IsUserInRole("Administrator"))
        {
            string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
            List<string> AdminUserList = new List<string>();
            MillimanCommon.SuperGroup.SuperGroupContainer SGC = Session["supergroup"] as MillimanCommon.SuperGroup.SuperGroupContainer;
            string[] Users = Roles.GetUsersInRole("Administrator");
            foreach (string Group in SGC.GroupNames)
            {
                string ProjectPath = System.IO.Path.Combine(DocumentRoot, Group.Replace('_', '\\'));
                ProjectPath = System.IO.Path.Combine(ProjectPath, "ReducedUserQVWs");
              
                foreach (string User in Users)
                {
                    //dont add underlying "admin" account
                    if ( (string.Compare(User, "admin", true) != 0) && (AdminUserList.Contains(User) == false))
                    {
                        AdminUserList.Add(User);
                        string Tooltip = "This user only visible if logged in as global administrator.";
                        string Status = "<center>NA</center>";
                        CurrentUsers.Add(new UserItem(User, Status, Tooltip, "", true));
                        //Verify users have a directory created for reduced QVWs
                        string UserDir = System.IO.Path.Combine(ProjectPath, MillimanCommon.Utilities.ConvertStringToHex(User));
                        if (System.IO.Directory.Exists(UserDir) == false)
                            System.IO.Directory.CreateDirectory(UserDir);
                    }
                }
            }
        }
        return CurrentUsers;
    }

    private void SetUIBehavior()
    {
        //if only 1 report and not reduceable, then user admin only(not right hand pane)
        //if multiple reports but none reducable then check boxs on right hand are visible, but no reduction tree
        //if any report is reducable, show all

        Dictionary<string, bool> Reduceable = Session["reductiondictionary"] as Dictionary<string, bool>;
        MillimanCommon.SuperGroup.SuperGroupContainer SGC = Session["supergroup"] as MillimanCommon.SuperGroup.SuperGroupContainer;

        bool ContainsReducableQVW = false;
        foreach( KeyValuePair<string,bool> KVP in Reduceable)
        {
            if (KVP.Value == true)
            {
                ContainsReducableQVW = true;
                break;
            }
        }

        if (( SGC.GroupNames.Count == 1 ) && (ContainsReducableQVW == false) )
        {
            RadPane2.Visible = false;
        }
        else if (( SGC.GroupNames.Count > 1 ) && (ContainsReducableQVW == false) )
        {
            RadPanelBar1.Visible = false;
            AutomaticInclusion.Visible = false;
            ShowCheckedOnly.Visible = false;
            ReportList.Enabled = true;
        }
    }
    /// <summary>
    /// this is a temp fix, we should be passed in the project name here, but instead we just get a group id
    /// so for now we only allow 1 project in the group to have a reducable QVW, we have to find that project
    /// in the group and place that in session for reporting
    /// </summary>
    private enum FailedBecause {  NO_PROJECTS, MULTIPLE_PROJECTS, NOT_SET }
    private bool SetReducableProjectInSession(out FailedBecause FailureReason )
    {
        FailureReason = FailedBecause.NOT_SET;

        string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
        string QualifiedPath = System.IO.Path.Combine(DocumentRoot, Session["supergroup"].ToString().Replace('_','\\'));
        string[] Projects = System.IO.Directory.GetFiles(QualifiedPath, "*.hciprj");
        if (( Projects == null ) || (Projects.Length == 0 ))
        {
            FailureReason = FailedBecause.NO_PROJECTS;
            return false;
        }
        if (Projects.Length > 1)
        {
            FailureReason = FailedBecause.MULTIPLE_PROJECTS;
            return false;
        }

        MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(Projects[0]);

        //bad form but best place to do this for now
        AutomaticInclusion.Checked = PS.AutomaticInclusion; //update UI with project settings for auto inclusion

        string QVWName = System.IO.Path.Combine(PS.LoadedFromPath, PS.QVName + ".qvw");
        if (System.IO.File.Exists(QVWName))
        {
            MillimanCommon.XMLFileSignature XMLFS = new MillimanCommon.XMLFileSignature(QVWName);
            if (XMLFS.SignatureDictionary.ContainsKey("can_emit"))
            {
                Session["ProjectPath"] = Projects[0];
                return true;
            }
        }
        return false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        
        if (!IsPostBack)
        {
            string AdminThisSupergroup = CheckForLogin();
            if (string.IsNullOrEmpty(AdminThisSupergroup))
            {
                Response.Redirect("NotAuthorizedIssue.html");
                return;
            }

            ////if more than one project in group stop now, don't allow an ambigious request result in PHI leak
            //FailedBecause FailureReason = FailedBecause.NOT_SET;
            //bool FoundProject = SetReducableProjectInSession( out FailureReason );
            //if (FoundProject == false)
            //{
            //    if (FailureReason == FailedBecause.NO_PROJECTS)
            //        Response.Redirect("noprojects.html");
            //    else if (FailureReason == FailedBecause.MULTIPLE_PROJECTS)
            //        Response.Redirect("multipleprojects.html");
            //    return;  //halt processing
            //}

            ApplyChangesButton.Enabled = false;

            MillimanCommon.SuperGroup.SuperGroupContainer SuperGroup = MillimanCommon.SuperGroup.GetInstance().FindSuper(AdminThisSupergroup);
            Session["supergroup"] = SuperGroup;

            List<UserItem> UI = MembershipToUserItems(SuperGroup.GroupNames);
            UI.Sort((user1, user2) => user1.AccountName.CompareTo(user2.AccountName));
            UserGrid.DataSource = UI;
            UserGrid.DataBind();

            int TotalLicense = 0;
            //update report view and license block
            foreach (string Group in SuperGroup.GroupNames)
            {
                string[] GroupUsers = Roles.GetUsersInRole(Group);
                TotalLicense += GroupUsers.Length;
                ListItem NewItem = new ListItem(Group.Substring(Group.LastIndexOf('_') + 1) + "  (" + GroupUsers.Length.ToString() + " License in Use)", Group);
                ReportList.Items.Add(NewItem);
            }

            //create a dictionary of the groups in the super group that have reducable QVWs  
            Dictionary<string, bool> Reducable = new Dictionary<string, bool>();
            string Msg = string.Empty;
            int Index = 0;
            foreach (string Group in SuperGroup.GroupNames)
            {
                Reducable.Add(Group, IsReducable(Group, out Msg));
                if (string.IsNullOrEmpty(Msg) == false)
                    ReportList.Items[Index].Attributes["title"] = Msg;
                Index++;
            }
            Session["reductiondictionary"] = Reducable;  //make available for future calls

            ReportList.Enabled = false;

            SetUIBehavior();



            string LabelTemplate = "Currently using " + TotalLicense.ToString() + " licenses for " + SuperGroup.GroupNames.Count.ToString() + " reports";
            Session["totalusers"] = TotalLicense;
            Session["totalgroups"] = SuperGroup.GroupNames.Count;

            LicenseMessage.Text = LabelTemplate;

            //if (MillimanCommon.MillimanGroupMap.GetInstance().MillimanGroupDictionary.ContainsKey(Session["supergroup"].ToString()) == false)
            //{
            //    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Group " + Session["supergroup"] + " not found in external map.");
            //}

            if (Membership.GetUser() != null)
            {
                UserMessage.Text = "Welcome! " + Membership.GetUser().UserName;
            }
            else
            {
                Response.Redirect("NotLoggedIn.html");
                return;
            }

            string XML = string.Empty;
            //GROUPNAME, QVW, Hiearchy, Reduceable
            foreach (string Group in SuperGroup.GroupNames)
            {
                string QVW = string.Empty;
                string Hierarchy = string.Empty;
                bool IsReduceable = false;
                if (GetParameters(Group, out QVW, out Hierarchy, out IsReduceable))
                {
                    string Container = "<Node Text=\"_TEXT_\" Value=\"_VALUE_\" ImageUrl=\"_IMAGE_\">_XML_</Node>";
                    Container = Container.Replace("_TEXT_", System.IO.Path.GetFileNameWithoutExtension(QVW));
                    Container = Container.Replace("_VALUE_", QVW);
                    Container = Container.Replace("_IMAGE_", "images/rootreport.gif");

                    if (string.IsNullOrEmpty(Hierarchy) == false)
                    {
                        MillimanCommon.MillimanTreeNode MTN = MillimanCommon.MillimanTreeNode.GetMemoryTree(Hierarchy);

                        string ContainerXML = MTN.ToBindableXML();//System.IO.File.ReadAllText(Hierarchy);
                        MTN.SubNodes.Clear();
                        MTN = null;

                        Container = Container.Replace("_XML_", ContainerXML);
                        XML += Container;  //append nodes
                    }
                    else
                    {
                        string SingleNodeContainer = "<Node Text=\"_TEXT_\" Value=\"_VALUE_\" ImageUrl=\"_IMAGE_\"></Node>";
                        SingleNodeContainer = SingleNodeContainer.Replace("_TEXT_", System.IO.Path.GetFileNameWithoutExtension(QVW));
                        SingleNodeContainer = SingleNodeContainer.Replace("_VALUE_", QVW);
                        SingleNodeContainer = SingleNodeContainer.Replace("_IMAGE_", "images/rootreport.gif");
                        XML = SingleNodeContainer + XML; //prepend single nodes
                    }
                }
            }
            RadTreeView RTV = RadPanelBar1.FindItemByValue("TreeHolder").FindControl("AccessTree") as RadTreeView;
            RTV.LoadXml("<Tree>" + XML + "</Tree>");

            //    return;

            //    List<string> MasterQVWs = new List<string>();
            //    List<System.Dynamic.ExpandoObject> QVWItems = MillimanCommon.UserRepo.GetInstance().FindAllQVProjectsForUser(Membership.GetUser().UserName, new string[] { Session["supergroup"].ToString() }, false);
            //    string XML = string.Empty;
            //    if (QVWItems != null)
            //    {
            //        RadTreeView RTV = RadPanelBar1.FindItemByValue("TreeHolder").FindControl("AccessTree") as RadTreeView;
            //        if (RTV != null)
            //        {
            //            foreach (System.Dynamic.ExpandoObject EO in QVWItems)
            //            {
            //                dynamic D = EO;
            //                List<string> HierarchyFiles = MillimanCommon.ReducedQVWUtilities.GetHierarchyFilenames(D.QVFilename.ToString());
            //                MasterQVWs.Add(D.QVFilename.ToString());
            //                foreach (string Hierarchy in HierarchyFiles)
            //                {
            //                    string Container = "<Node Text=\"_TEXT_\" Value=\"_VALUE_\" ImageUrl=\"_IMAGE_\">_XML_</Node>";
            //                    Container = Container.Replace("_TEXT_", System.IO.Path.GetFileNameWithoutExtension(D.QVFilename.ToString()));
            //                    Container = Container.Replace("_VALUE_", D.QVFilename.ToString());
            //                    Container = Container.Replace("_IMAGE_", "images/rootreport.gif");

            //                    MillimanCommon.MillimanTreeNode MTN = MillimanCommon.MillimanTreeNode.GetMemoryTree(Hierarchy);

            //                    string ContainerXML = MTN.ToBindableXML();//System.IO.File.ReadAllText(Hierarchy);
            //                    MTN.SubNodes.Clear();
            //                    MTN = null;

            //                    Container = Container.Replace("_XML_", ContainerXML);
            //                    XML += Container;
            //                }
            //            }
            //            RTV.LoadXml("<Tree>" + XML + "</Tree>");

            //            ////sort tree per request
            //            ////SortCollection(RTV.Nodes);
            //            //foreach (RadTreeNode item in RTV.GetAllNodes())
            //            //{
            //            //    if (item.Nodes.Count > 0)
            //            //    {
            //            //        SortCollection(item.Nodes);
            //            //    }
            //            //}

            //           // LoadDownloads(MasterQVWs);
            //        }
            //    }
            //    else
            //    {
            //        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "User does not have access to any QVWs in group " + Session["supergroup"].ToString() + " to administer users");
            //    }
            //}

            System.Collections.ArrayList selectedItems = null;
            if (Session["selecteditems"] != null)
                selectedItems = (System.Collections.ArrayList)Session["selecteditems"];
            else
                selectedItems = new System.Collections.ArrayList();

            if (selectedItems.Count == 0)
                AccessTable.Enabled = false;
        }
    }
    /// <summary>
    /// Returns a groupid if valid user
    /// </summary>
    /// <returns></returns>
    private string CheckForLogin()
    {
        //if (Membership.GetUser() == null)
        //{
        //    //short circuit for development
        //    string Me = "van.nanney@milliman.com";
        //    bool Res = Membership.ValidateUser(Me, Membership.GetUser(Me).GetPassword());
        //    FormsAuthentication.SetAuthCookie(Me, true);
        //    Response.Redirect("Default.aspx");
        //}
        //return "Test No Reduction";

        string Key = Request["key"];
     
        if (string.IsNullOrEmpty(Key) == false)
        {
            string CacheDir = System.Configuration.ConfigurationManager.AppSettings["HCIntelCache"];  //should be full path in web.config
            string CachePathFileName = System.IO.Path.Combine(CacheDir, Key);
            Session["SSOToken"] = CachePathFileName + ".xml";  //when the session expires we delete the cache file in global.asx
            MillimanCommon.CacheEntry CE = MillimanCommon.CacheEntry.Load(CachePathFileName);
            if (string.IsNullOrEmpty(User.Identity.Name) == true) //if users is already authenticated, skip this check
            {
                if (CE != null)
                {
                    if (CE.Expires < DateTime.Now)
                    {
                        Response.Redirect("NotAuthorizedIssue.html");
                        return string.Empty;
                    }
                    else if ((Membership.GetUser(CE.UserName).ProviderUserKey.ToString() != CE.UserKey.ToString()) && (Membership.GetUser(CE.UserName).IsOnline == false))
                    {
                        Response.Redirect("NotAuthorizedIssue.html");
                        return string.Empty;
                    }

                    if (System.IO.File.Exists(CachePathFileName))  //make sure cache file exists, there is a race condition btween session end and the metatag call back, this check is a last second check against the race condition
                    {
                        if (Membership.ValidateUser(CE.UserName, Membership.GetUser(CE.UserName).GetPassword()) == true)
                        {
                            FormsAuthentication.SetAuthCookie(CE.UserName, false);
                            Response.Redirect("Default.aspx");  //we have to bounce the page to make sure authenication takes effect
                        }
                        else
                        {
                            Response.Redirect("NotAuthorizedIssue.html");
                            return string.Empty;
                        }
                    }
                    return CE.MillimanGroupName;
                }
                else
                {
                    Response.Redirect("NotAuthorizedIssue.html");
                }
            }
            else
            {
                return CE.MillimanGroupName;
            }
        }
        return "";
    }

    /// <summary>
    /// Not sure this is relevant now 
    /// </summary>
    private void UpdateLicenseMessage()
    {
        try
        {
            MillimanCommon.MillimanGroupMap.MillimanGroups MG = MillimanCommon.MillimanGroupMap.GetInstance().MillimanGroupDictionary[Session["supergroup"].ToString()];
            string[] Users = Roles.GetUsersInRole(Session["supergroup"].ToString());

            int StandardUsers = 0;
            foreach (string User in Users)
            {
                //dont count admin people these are typically milliman persons
                if (Roles.IsUserInRole(User, "Administrator") == false)
                    StandardUsers++;  
            }
            string LabelTemplate = "Currently using " + StandardUsers.ToString() + " of " + MG.MaximumnUsers.ToString() + " licenses for " + MG.FriendlyGroupName;
            LicenseMessage.Text = LabelTemplate;

            if (StandardUsers >= MG.MaximumnUsers)
            {
                string Msg = "Licensing constrains the maximum number of users to " + MG.MaximumnUsers.ToString() + ".  For more users contact PRM support for to increase the license limit.";
                Session["LicenseLimitReached"] = Msg;
            }
            else
            {
                Session["LicenseLimitReached"] = null;
            }
        }
        catch (Exception ex)
        {
            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Error", ex);
            Response.Redirect("unspecifiederror.html");
        }
    }

    private void UpdateAccessView()
    {
        System.Collections.ArrayList selectedItems = null;
        if (Session["selecteditems"] != null)
            selectedItems = (System.Collections.ArrayList)Session["selecteditems"];
        else
            selectedItems = new System.Collections.ArrayList();

        if (selectedItems.Count >= 1)
        {
            AccessTable.Enabled = true;
            LoadViewsFor(selectedItems[0].ToString());
        }
        else
        {
            AccessTable.Enabled = false;
        }
    }

    private void LoadDownloads(List<string> MasterQVWs)
    {
        RadTreeView Downloads = RadPanelBar1.FindItemByValue("DownloadHolder").FindControl("DownloadTree") as RadTreeView;
        string XML = string.Empty;
        if (Downloads != null)
        {
            foreach (string MasterQVW in MasterQVWs)
            {
                string ContainerXML = MillimanCommon.DownloadDescriptions.AllDescriptionsToXML(MasterQVW);
                XML += ContainerXML;
            }
            Downloads.LoadXml("<Tree>" + XML + "</Tree>");
        }
    }

    protected void RadGrid1_ItemCommand(object sender, GridCommandEventArgs e)
    {
        //make sure we have not timed out
        if ((Session["supergroup"] == null) || (Session["supergroup"].ToString() == ""))
        {
            Response.Redirect("NotAuthorizedIssue.html");
            return;
        }

        System.Collections.ArrayList selectedItems;
        if (Session["selecteditems"] == null)
        {
            selectedItems = new System.Collections.ArrayList();
            Session["selecteditems"] = selectedItems;
        }
        else
        {
            selectedItems = (System.Collections.ArrayList)Session["selecteditems"];
            if (selectedItems.Count == 1)  //only do this if one item is selected
            {
                //don't allow reset, suspend or delete to affect ADMINS
                if (Roles.IsUserInRole(selectedItems[0].ToString(), "Administrator") == true)
                {
                   if ( (string.Compare(e.CommandName,"reset",true) == 0) ||
                        (string.Compare(e.CommandName, "suspend", true) == 0) ||
                        (string.Compare(e.CommandName,"delete",true) == 0) )
                    return; //if we are a global admin dont allow any of this to affect us
                }
            }
        }

        if (string.Compare(e.CommandName, "reset", true) == 0)
        {
            if ((UserGrid.SelectedItems != null) && (UserGrid.SelectedItems.Count > 0))
            {
                selectedItems.Clear();
                for (int Index = 0; Index < UserGrid.SelectedItems.Count; Index++)
                {
                    try
                    {
                        Telerik.Web.UI.GridDataItem GDI = UserGrid.SelectedItems[Index] as GridDataItem;  //should be only 1
                        Label AccountName = (Label)GDI.FindControl("AccountNameTextBox");
                        //create user password reset file
                        MembershipUser MU = Membership.GetUser(AccountName.Text);
                        string ResetFile = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["ResetUserInfoRoot"], MU.ProviderUserKey + ".rst");
                        MU.ChangePassword(MU.GetPassword(), "@" + Guid.NewGuid().ToString());
                        System.IO.File.WriteAllText(ResetFile, MU.UserName + " added " + DateTime.Now.ToShortDateString());
                        Membership.UpdateUser(MU);
                        selectedItems.Add(MU.UserName);
                        //send email
                        string EmailTemplatesDir = Server.MapPath("~/email_templates");
                        string EmailBody = System.IO.File.ReadAllText(System.IO.Path.Combine(EmailTemplatesDir, "resetpassword.htm"));
                        string Body = MillimanCommon.MillimanEmail.EmailMacroProcessor(EmailBody, AccountName.Text, System.Web.Security.Membership.GetUser(AccountName.Text).ProviderUserKey.ToString(), System.Web.Security.Membership.GetUser().UserName);

                        MillimanCommon.MillimanEmail ME = new MillimanCommon.MillimanEmail();
                        string From = System.Configuration.ConfigurationManager.AppSettings["SupportEmail"];
                        string Title = System.Configuration.ConfigurationManager.AppSettings["SupportEmailWelcome"];
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Info, "Sending password reset for:" + AccountName.Text);
                        ME.Send(AccountName.Text, From, Body, Title, true, false);
                    }
                    catch (Exception ex)
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified error", ex);
                    }
                }
            }
            else
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Info, "Reset password, but not list item selected");
            }

        }
        else if (string.Compare(e.CommandName, "suspend", true) == 0)
        {
            if ((UserGrid.SelectedItems != null) && (UserGrid.SelectedItems.Count > 0))
            {
                selectedItems.Clear();
                for (int Index = 0; Index < UserGrid.SelectedItems.Count; Index++)
                {
                    Telerik.Web.UI.GridDataItem GDI = UserGrid.SelectedItems[Index] as GridDataItem;  //should be only 1
                    Label AccountName = (Label)GDI.FindControl("AccountNameTextBox");
                    MembershipUser MU = Membership.GetUser(AccountName.Text);
                    MU.IsApproved = !MU.IsApproved;
                    Membership.UpdateUser(MU);
                    selectedItems.Add(MU.UserName);
                }
            }
        }
        else if (string.Compare(e.CommandName, "delete", true) == 0)
        {
            if ((UserGrid.SelectedItems != null) && (UserGrid.SelectedItems.Count > 0))
            {
                selectedItems.Clear();
                for (int Index = 0; Index < UserGrid.SelectedItems.Count; Index++)
                {
                    Telerik.Web.UI.GridDataItem GDI = UserGrid.SelectedItems[Index] as GridDataItem;  //should be only 1
                    string GroupID = Session["supergroup"].ToString();
                    Label AccountName = (Label)GDI.FindControl("AccountNameTextBox");
                    //don't allow the admin to delete thier on account, just skip it
                    if (string.Compare(AccountName.Text, Membership.GetUser().UserName, true) == 0)
                        continue;

                    string[] UserRoles = Roles.GetRolesForUser(AccountName.Text);
                    //a user could be in more than one role, if so, just remove them from this role, but leave thier loging
                    //they will need to login for other role
                    if (UserRoles.Length > 1)
                    {
                        if (Roles.IsUserInRole(AccountName.Text, GroupID))  //make sure they are in the role, or an error will occur, some roles are implied
                            Roles.RemoveUserFromRole(AccountName.Text, GroupID);
                    }
                    else  //else delete thier account
                    {
                        if (Roles.IsUserInRole(AccountName.Text, GroupID))//make sure they are in the role, or an error will occur, some roles are implied
                            Membership.DeleteUser(AccountName.Text);
                    }
                    //always cleanup this stuff, is specifid for the user and this project
                    string ProjectDirectory = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"], GroupID.Replace("_","\\"));
                    string UserDirectory = MillimanCommon.ReducedQVWUtilities.GetUserDir(ProjectDirectory, AccountName.Text);
                    if ( System.IO.Directory.Exists( UserDirectory ))  //if they have a dir, kill it
                        System.IO.Directory.Delete(UserDirectory, true);
                    selectedItems.Add(AccountName.Text);
                }
            }
        }
        else if (string.Compare(e.CommandName, "rowclick", true) == 0)
        {
            e.Item.Selected = true;
            if ((UserGrid.SelectedItems != null) && (UserGrid.SelectedItems.Count > 0))
            {
                //List<string> SelectedUsers = new List<string>();
                selectedItems.Clear();
                for (int Index = 0; Index < UserGrid.SelectedItems.Count; Index++)
                {

                    Telerik.Web.UI.GridDataItem GDI = UserGrid.SelectedItems[Index] as GridDataItem;  //should be only 1
                    Label AccountName = (Label)GDI.FindControl("AccountNameTextBox");
                    //SelectedUsers.Add(AccountName.Text);
                    selectedItems.Add(AccountName.Text);
                }
                //uncheck all report list items
                foreach( ListItem LI in ReportList.Items)
                    LI.Selected = false;
                //process on users
                UpdateAccessView();  //do this to make views switch to new user
                //if only showing checked, re-calc it
                if (ShowCheckedOnly.Checked)
                    ShowCheckedOnly_CheckedChanged(ShowCheckedOnly, null);
            }
        }
            //row double click is not ACTIVE at this time
        else if (string.Compare(e.CommandName, "rowdoubleclick", true) == 0)
        {
            e.Item.Selected = true;
            if ((UserGrid.SelectedItems != null) && (UserGrid.SelectedItems.Count > 0))
            {
                //List<string> SelectedUsers = new List<string>();
                string SingleItem = string.Empty;
                for (int Index = 0; Index < UserGrid.SelectedItems.Count; Index++)
                {
                    Telerik.Web.UI.GridDataItem GDI = UserGrid.SelectedItems[Index] as GridDataItem;  //should be only 1
                    Label AccountName = (Label)GDI.FindControl("AccountNameTextBox");
                    if (selectedItems.Contains(AccountName.Text) == false)
                        SingleItem = AccountName.Text;
                    
                }
                selectedItems.Clear();
                selectedItems.Add(SingleItem);
                //process on users
                UpdateAccessView();  //do this to make views switch to new user
                //if only showing checked, re-calc it
                if (ShowCheckedOnly.Checked)
                    ShowCheckedOnly_CheckedChanged(ShowCheckedOnly, null);
            }
        }
        else if (string.Compare(e.CommandName, "launchqvw", true) == 0)
        {
            int TrueRowIndex = e.Item.RowIndex - 2 ;  //first two rows are headers, so sub off 2
            Telerik.Web.UI.GridDataItem GDI = UserGrid.Items[TrueRowIndex] as GridDataItem;  //should be only 1
            Label AccountName = (Label)GDI.FindControl("AccountNameTextBox");

            // public string FindQualifedQVProject(string UserName, string QVProjectName, string[] Roles )
            MillimanCommon.UserRepo Repo = MillimanCommon.UserRepo.GetInstance();
            string QVW = Repo.FindQualifedQVProject(AccountName.Text, System.IO.Path.GetFileName(Session["ProjectPath"].ToString()), new string[] { Session["supergroup"].ToString() });
            if (string.IsNullOrEmpty(QVW) == false)
            {
                //even though we looked up the qvw for a user, we want to display via our account so we always use a named
                //license for client admins
                string ClientAdminAccount = Membership.GetUser().UserName;
                if ( Launch( ClientAdminAccount, QVW, "", "") == false )
                    MillimanCommon.Alert.Show("User '" + AccountName.Text + "' has an associated QVW but it failed launch.");
            }
            else
            {
                Response.Redirect("NoReducedVersion.html");
            }


        }
        //cannot rebind especially on delete since this causes the radgrid to crash
        //List<UserItem> UI = MembershipToUserItems(Session["supergroup"].ToString());
        //UserGrid.DataSource = UI;
        //UserGrid.DataBind();
        if (string.Compare(e.CommandName, "delete", true) == 0)
            Response.Redirect(Request.RawUrl);
        else
        {
            MillimanCommon.SuperGroup.SuperGroupContainer SuperGroup = Session["supergroup"] as MillimanCommon.SuperGroup.SuperGroupContainer;

            List<UserItem> UI = MembershipToUserItems(SuperGroup.GroupNames);
            UI.Sort((user1, user2) => user1.AccountName.CompareTo(user2.AccountName));
            UserGrid.DataSource = UI;
            UserGrid.DataBind();
        }
    }

    protected void NotesTextBox_TextChanged(object sender, EventArgs e)
    {
        //make sure we have not timed out
        if ((Session["supergroup"] == null) || (Session["supergroup"].ToString() == ""))
        {
            Response.Redirect("NotAuthorizedIssue.html");
            return;
        }

        TextBox T = sender as TextBox;

        string AccountNameItem = T.ClientID.Replace("NotesTextBox", "AccountNameTextBox");
        Label Test = FindControlInGrid(UserGrid, AccountNameItem) as Label;
        if (Test != null)
        {
            SetAdminNote(Test.Text, T.Text);
        }
    }

    protected void UserGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem item = (GridDataItem)e.Item;
            int index = item.ItemIndex;
            TextBox txtbx = (TextBox)item["NotesText"].FindControl("NotesTextBox");
            txtbx.Attributes.Add("OnClick", "return Select('" + index + "');");
            
        }
        else if (e.Item is GridCommandItem)
        {
            GridCommandItem item = (GridCommandItem)e.Item;
            LinkButton LB = (LinkButton)item.FindControl("Add");
            if (LB != null)
            {
                if (Session["LicenseLimitReached"] != null)
                {
                    LB.Enabled = false;
                    LB.ToolTip = Session["LicenseLimitReached"].ToString();
                    LB.OnClientClick = "";
                }
                else
                {
                    LB.Enabled = true;
                    LB.ToolTip = "Add new users";
                    LB.OnClientClick = "OpenAddUser(); return false;";
                }
            }
        }
    }

    protected System.Web.UI.WebControls.WebControl FindControlInGrid(Telerik.Web.UI.RadGrid ParentGrid, string SearchID)
    {
        foreach (GridItem item in ParentGrid.MasterTableView.Items)
        {
            if (item is GridDataItem)
            {
                GridDataItem dataItem = (GridDataItem)item;
                WebControl MyControl = dataItem.FindControl(SearchID) as WebControl;
                if (MyControl != null)
                    return MyControl;
                foreach (TableCell Cell in dataItem.Cells)
                {
                    foreach (Control C in Cell.Controls)
                    {
                        System.Diagnostics.Debug.WriteLine(C.ID);
                        if (string.Compare(C.ClientID, SearchID, true) == 0)
                            return C as WebControl;
                    }
                }
            }
        }
        return null;
    }
    protected void UserGrid_PreRender(object sender, EventArgs e)
    {
        if (Session["selecteditems"] != null)
        {
            System.Collections.ArrayList selectedItems = (System.Collections.ArrayList)Session["selecteditems"];
            Int16 stackIndex;
            for (stackIndex = 0; stackIndex <= selectedItems.Count - 1; stackIndex++)
            {
                string curItem = selectedItems[stackIndex].ToString();
                foreach (GridItem item in UserGrid.MasterTableView.Items)
                {
                    if (item is GridDataItem)
                    {
                        GridDataItem dataItem = (GridDataItem)item;
                        Label AccountName = (Label)dataItem.FindControl("AccountNameTextBox");

                        if ( selectedItems.Contains( AccountName.Text) )
                        {
                            dataItem.Selected = true;
                            //dataItem.BackColor = System.Drawing.Color.AliceBlue;
                            //break;
                        }
                    }
                }
            }
        }

    }
  
    protected void ApplyChangesButton_Click(object sender, EventArgs e)
    {
        //make sure we have not timed out
        if ((Session["supergroup"] == null) || (Session["supergroup"].ToString() == ""))
        {
            Response.Redirect("NotAuthorizedIssue.html");
            return;
        }

        if ((UserGrid.SelectedItems != null) && (UserGrid.SelectedItems.Count > 0))
        {
            List<string> Accounts = new List<string>();
            for (int Index = 0; Index < UserGrid.SelectedItems.Count; Index++)
            {

                Telerik.Web.UI.GridDataItem GDI = UserGrid.SelectedItems[Index] as GridDataItem;  //should be only 1
                Label AccountName = (Label)GDI.FindControl("AccountNameTextBox");
                Accounts.Add(AccountName.Text);
            }

            RadTreeView AccessTree = RadPanelBar1.FindItemByValue("TreeHolder").FindControl("AccessTree") as RadTreeView;
            RadTreeView DownloadTree = RadPanelBar1.FindItemByValue("DownloadHolder").FindControl("DownloadTree") as RadTreeView;

            bool RequiresReduction = true;
            MillimanCommon.TreeToFileUtilities TTFU = new MillimanCommon.TreeToFileUtilities();
            if (TTFU.SaveSettings(MillimanCommon.ProjectSettings.Load(Session["ProjectPath"].ToString()),AccessTree, DownloadTree, Accounts, out RequiresReduction) == false)
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to save settings");

            if (RequiresReduction == false)
            { //just change download settings, does not require we reduce
                string Msg = "Account(s):<br/><br/>";
                if (Accounts.Count == 1)
                    Msg = "Account:<br/><br/>";
                foreach (string User in Accounts)
                    Msg += User + "<br/>&nbsp;&nbsp;&nbsp;";
                Msg += "<br/> User download selections saved.";

                //VWN provide more info what we did
                //MillimanCommon.Alert.Show(Msg, true);
                RadWindowManager.RadAlert(Msg, 300, null, "Accounts Modified", "");
                return;
            }

            bool AllNodesSelected = false;
            List<MillimanCommon.TreeToFileUtilities.ReductionSelections> Selections = TTFU.GetAccessSelectionsForReduction(AccessTree, out AllNodesSelected);
            MillimanReportReduction.QVWReductionProcessor QVWRepProc = new MillimanReportReduction.QVWReductionProcessor();
            string GroupID = Session["supergroup"].ToString();

            if (QVWRepProc.ProcessUsers(GroupID, Accounts, Selections, AllNodesSelected) == false)
            {
                string UserList = string.Empty;
                foreach (string S in Accounts)
                {
                    if (string.IsNullOrEmpty(UserList) == false)
                        UserList += ",";
                    UserList += S;
                }
                string QVWList = string.Empty;
                foreach (MillimanCommon.TreeToFileUtilities.ReductionSelections RS in Selections)
                {
                    if (string.IsNullOrEmpty(QVWList) == false)
                        QVWList += ",";
                    QVWList += "\"" + System.IO.Path.GetFileName( RS.QualafiedQVW) + "\"";
                }
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to reduced " + QVWList + " for users " + UserList);

                string ErrorMsg = "Failed to reduce report(s):<br/>" + QVWList.Replace(",", "<br/>") + "<br/><br/> for user(s):<br/>" + UserList.Replace(",", "<br/>") + "<br><br>Report access has not been modified.";
                RadWindowManager.RadAlert(ErrorMsg, 300, null, "Report Generation Error", "");
            }
            else
            {

                string Msg = "Account(s):<br/><br/>";
                if ( Accounts.Count == 1 )
                    Msg = "Account:<br/><br/>";
                foreach (string User in Accounts)
                    Msg += User + "<br/>&nbsp;&nbsp;&nbsp;";
                Msg += "<br/> Access rights were modified as specified.";

                //VWN provide more info what we did
                //MillimanCommon.Alert.Show(Msg, true);
                RadWindowManager.RadAlert(Msg, 300, null, "Accounts Modified", "");
            }
            //Dictionary<string, List<string>> AccessSaveSelections = null;
            //List<string> SelectedItems = GetAccessSelections(out AccessSaveSelections);
            
            ////issue reduction on selectedItems - HERE 

            //foreach( string Account in Accounts )
            //{
            //    //store access save selections
            //    foreach( KeyValuePair<string, List<string>> Selection in AccessSaveSelections )
            //    {
            //        string SaveTo = MillimanCommon.ReducedQVWUtilities.ReducedQVW(Selection.Key, Account);
            //        SaveTo = SaveTo.ToLower().Replace(".qvw", ".selections");
            //        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(SaveTo));
            //        System.IO.File.Delete(SaveTo);
            //        Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
            //        SS.Serialize(Selection.Value, SaveTo);
            //    }
                
            //    Dictionary<string, List<string>> DownloadSaveSelections = null;
            //    List<string> DownloadItems = GetDownloadSelections(out DownloadSaveSelections);
            //    //store access save selections
            //    foreach (KeyValuePair<string, List<string>> Selection in DownloadSaveSelections)
            //    {
            //        string SaveTo = MillimanCommon.ReducedQVWUtilities.ReducedQVW(Selection.Key, Account);
            //        string SaveToDownloads = SaveTo.ToLower().Replace(".qvw", ".downloads");
            //        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(SaveToDownloads));
            //        System.IO.File.Delete(SaveToDownloads);
            //        Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
            //        SS.Serialize(Selection.Value, SaveToDownloads);

            //        //update custom user downloads
            //        MillimanCommon.CustomUserDownloads CUD = MillimanCommon.CustomUserDownloads.GetInstance();
            //        CUD.ClearItems(Account, MillimanCommon.QVWUtilities.AbsoluteToVirtualFromQVDocRoot(SaveTo));
            //        string Icon;
            //        string Mime;
            //        string Tooltip;
            //        foreach (string SelectedDownloads in Selection.Value)
            //        {
            //            string[] SDTokens = SelectedDownloads.Split(new char[] { '|' });
            //            string DownloadItem = SDTokens[1];
            //            string RelativeDownloadItem = MillimanCommon.QVWUtilities.AbsoluteToVirtualFromQVDocRoot(DownloadItem);
            //            Icon = MillimanCommon.ImageUtilities.GetIcon(DownloadItem, out Mime);
            //            Tooltip = SDTokens[SDTokens.Length - 1];
                  
            //            CUD.AddorUpdateItem(Account, MillimanCommon.QVWUtilities.AbsoluteToVirtualFromQVDocRoot(SaveTo), RelativeDownloadItem, Icon, Mime, Tooltip);
            //        }
            //    }
            //}

        }      
    }


    private bool LoadViewsFor(string Account)
    {

        //update checks on access tree
        RadTreeView RTV = RadPanelBar1.FindItemByValue("TreeHolder").FindControl("AccessTree") as RadTreeView;
        RTV.UncheckAllNodes();
        foreach (RadTreeNode RTN in RTV.Nodes)
        {
            //update access tree
            string QVW = RTN.Value;
            string AccessReadFrom = MillimanCommon.ReducedQVWUtilities.GetUsersReducedQVWName(QVW, Account);
            AccessReadFrom = AccessReadFrom.ToLower().Replace(".qvw", ".selections");
            if (System.IO.File.Exists(AccessReadFrom))
            {
                Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                List<string> Selections = SS.Deserialize(AccessReadFrom) as List<string>;
                FindAndCheckNodes(RTN, Selections);

                if ( Selections.Count > 0 )
                {
                    string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                    string GroupName = System.IO.Path.GetDirectoryName(QVW);
                    GroupName = GroupName.Substring(DocumentRoot.Length+1).Replace('\\', '_');
                    ReportList.Items.FindByValue(GroupName).Selected = true;
                }
            }
        }
        return true;
    }

    private bool UpdateDownloadChecks(RadTreeNode StartingNode,
                                     List<string> Selections)
    {
        bool AllFound = true;
        foreach (string S in Selections)
        {
            string[] Tokens = S.Split(new char[] { '|' });
            string Value = Tokens[2];
            bool Found = false;
            foreach (RadTreeNode RTN in StartingNode.Nodes)
            {
                if (string.Compare(RTN.Text, Value, true) == 0)
                {
                    RTN.Checked = true;
                    Found = false;
                    break;
                }
            }
            if (Found == false)
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Download item '" + Value + "' not found for QVW '" + StartingNode.Value + "'");

            AllFound = AllFound && Found;
        }

        return AllFound;
    }
    private bool FindAndCheckNodes(RadTreeNode StartingNode,
                                  List<string> Selections)
    {

        foreach( string S in Selections )
        {
            if (FindAndCheckNode(StartingNode, S) == false)
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to find node '" + S + "' in QVW tree '" + StartingNode.Value + "'");
        }
        return true;
    }

    private bool FindAndCheckNode(RadTreeNode StartingNode,
                                   string Selection)
    {
        string[] Tokens = Selection.Split(new char[] { '|' });
        RadTreeNode Current = StartingNode;
        for (int Index = 1; Index < Tokens.Length; Index++)
        {
            bool Found = false;
            foreach (RadTreeNode RTN in Current.Nodes)
            {
                if (string.Compare(RTN.Text, Tokens[Index], true) == 0)
                {
                    Current = RTN;
                    Found = true;
                    break;
                }
            }
            if (Found == false)  //didnt forlorn node
                return false;
        }
        Current.Checked = true;
        return true;
    }

    protected void ShowCheckedOnly_CheckedChanged(object sender, EventArgs e)
    {
        //make sure we have not timed out
        if ((Session["supergroup"] == null) || (Session["supergroup"].ToString() == ""))
        {
            Response.Redirect("NotAuthorizedIssue.html");
            return;
        }

        RadTreeView RTV = RadPanelBar1.FindItemByValue("TreeHolder").FindControl("AccessTree") as RadTreeView;

        CheckBox CB = sender as CheckBox;

        List<RadTreeNode> AccessTree = RTV.GetAllNodes() as List<RadTreeNode>;
        foreach (RadTreeNode RTN in AccessTree)
        {
            if (CB.Checked)
                RTN.Visible = RTN.Checked;
            else
                RTN.Visible = true;
        }

        if (CB.Checked)
        {
            RTV.ExpandAllNodes();
        }
    }

    private string GetAdminNote(string UserAccount )
    {
        MembershipUser MU = Membership.GetUser(UserAccount);
        string Note = string.Empty;
        if (MU != null)
        {
            string UserId = MU.ProviderUserKey.ToString();
            try
            {
                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["dbMyCMSConnectionString"].ConnectionString;
                System.Data.SqlClient.SqlCommand comm = new System.Data.SqlClient.SqlCommand();
                comm.Connection = new System.Data.SqlClient.SqlConnection(ConnectionString);
                String sql = @"SELECT AdminNote from aspnet_customprofile where UserId='" + UserId.ToUpper() + "'";
                comm.CommandText = sql;
                comm.Connection.Open();
                System.Data.SqlClient.SqlDataReader cursor = comm.ExecuteReader();
                while (cursor.Read())
                {
                    Note =  cursor["AdminNote"].ToString();
                }
                comm.Connection.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        return Note;
    }

    private bool SetAdminNote(string UserAccount, string Note)
    {
        bool ReturnValue = false;
        MembershipUser MU = Membership.GetUser(UserAccount);
        if (MU != null)
        {
            string UserId = MU.ProviderUserKey.ToString();
            try
            {
                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["dbMyCMSConnectionString"].ConnectionString;
                System.Data.SqlClient.SqlCommand comm = new System.Data.SqlClient.SqlCommand();
                comm.Connection = new System.Data.SqlClient.SqlConnection(ConnectionString);
                String sql = @"update aspnet_customprofile set AdminNote='" + Note + "' where UserId='" + UserId.ToUpper() + "'";
                comm.CommandText = sql;
                comm.Connection.Open();
                ReturnValue = (comm.ExecuteNonQuery() == 1);
                comm.Connection.Close();

                //if update failed we need to create the row, it does not exist
                if (ReturnValue == false)
                {
                    sql = "insert into aspnet_CustomProfile (UserId, LastUpdatedDate, AdminNote) VALUES('" + UserId.ToUpper() + "', CURRENT_TIMESTAMP, '" + Note + "')";
                    System.Data.SqlClient.SqlCommand Insertcomm = new System.Data.SqlClient.SqlCommand();
                    Insertcomm.Connection = new System.Data.SqlClient.SqlConnection(ConnectionString);
                    Insertcomm.CommandText = sql;
                    Insertcomm.Connection.Open();
                    ReturnValue = (Insertcomm.ExecuteNonQuery() == 1);
                    Insertcomm.Connection.Close();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        return ReturnValue;
    }

    /// <summary>
    /// Write a file out that will designate auto include nodes at same level
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void AutomaticInclusion_CheckedChanged(object sender, EventArgs e)
    {
        //make sure we have not timed out
        if ((Session["supergroup"] == null) || (Session["supergroup"].ToString() == ""))
        {
            Response.Redirect("NotAuthorizedIssue.html");
            return;
        }

        string ProjectPath = Session["ProjectPath"].ToString();
        MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(ProjectPath);
        PS.AutomaticInclusion = AutomaticInclusion.Checked;
        PS.Save();

    }

    /// <summary>
    /// This routine sets the target to launch QVW into new tab or window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void UserGrid_DataBound(object sender, EventArgs e)
    {
        foreach (GridDataItem item in UserGrid.Items)
        {
            if (item.ItemType == GridItemType.Item || item.ItemType == GridItemType.AlternatingItem)
            {
                System.Web.UI.WebControls.ImageButton imgBtn = (System.Web.UI.WebControls.ImageButton)item["LaunchQVW"].Controls[0];
               
                if (imgBtn != null)
                {
                    //have to set document target to blank to launch new window, and then a timer to reset form back to self, otherwise clicking
                    //another row in the table will launch into another window
                    imgBtn.Attributes.Add("onclick", "document.forms[0].target= '_blank';setTimeout(function(){ document.forms[0].target= '_self';  }, 1000);return true;");
                }
            }
        }

       
    }

    private bool IsReducable(string Group, out string Msg)
    {
        Msg = string.Empty;
        string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
        string DirectoryToReview = System.IO.Path.Combine(DocumentRoot, Group.Replace('_','\\'));
        string[] QVW = System.IO.Directory.GetFiles(DirectoryToReview, "*.qvw", System.IO.SearchOption.TopDirectoryOnly);
        if ( QVW.Length != 1)
        {  //error condition, is ambigious 
            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Directory '" + DirectoryToReview + "' contains multiple master QVWs, by group defination should only contain 1 QVW");
            Msg = "Ambigious report - group contains multiple reports.";
            return false;
        }
        else
        {
            string CanEmitKey = "can_emit";
            MillimanCommon.XMLFileSignature signature = new MillimanCommon.XMLFileSignature(QVW[0]);
            if ( signature.SignatureDictionary.ContainsKey(CanEmitKey))
            {
                return System.Convert.ToBoolean(signature.SignatureDictionary[CanEmitKey]);
            }
            return false;
        }
    }
   
    /// <summary>
    /// Return qualfiied filenames for QVWFile and HierarchyFile
    /// </summary>
    /// <param name="Group"></param>
    /// <param name="QVWFile"></param>
    /// <param name="HierarchyFile"></param>
    /// <param name="IsReduceable"></param>
    /// <returns></returns>
    private bool GetParameters( string Group, out string QVWFile, out string HierarchyFile, out bool IsReduceable)
    {
        QVWFile = string.Empty;
        HierarchyFile = string.Empty;
        IsReduceable = false;

        Dictionary<string, bool> Reduceable = Session["reductiondictionary"] as Dictionary<string, bool>;
        MillimanCommon.SuperGroup.SuperGroupContainer SGC = Session["supergroup"] as MillimanCommon.SuperGroup.SuperGroupContainer;
        
        string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
        string DirectoryToReview = System.IO.Path.Combine(DocumentRoot, Group.Replace('_', '\\'));

        string[] QVWFiles = System.IO.Directory.GetFiles(DirectoryToReview, "*.qvw", System.IO.SearchOption.TopDirectoryOnly);
        if (QVWFiles.Length != 1)
            return false;

        IsReduceable = Reduceable[Group];
        QVWFile = QVWFiles[0];
        if (IsReduceable == true)
        {
            HierarchyFile = QVWFile.ToLower().Replace(".qvw", ".hierarchy_0");
            if (System.IO.File.Exists(HierarchyFile) == false)
                return false;
        }
        return true;
    }
}
