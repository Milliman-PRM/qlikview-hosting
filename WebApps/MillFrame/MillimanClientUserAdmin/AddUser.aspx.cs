using MillimanCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace MillimanClientUserAdmin
{
    public partial class AddUser : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MillimanCommon.MillimanGroupMap.MillimanGroups MG = MillimanCommon.MillimanGroupMap.GetInstance().MillimanGroupDictionary[Session["groupid"].ToString()];
                string[] Users = System.Web.Security.Roles.GetUsersInRole(Session["groupid"].ToString());
                int StandardUsers = 0;
                foreach (string User in Users)
                {
                    if (System.Web.Security.Roles.IsUserInRole(User, "Administrator") == false)
                        StandardUsers++;
                }
                string LabelTemplate = "Currently using " + StandardUsers.ToString() + " of " + MG.MaximumnUsers.ToString() + " licenses for " + MG.FriendlyGroupName;
                License.Text = LabelTemplate;

                List<UserInfo> UL = new List<UserInfo>();
                UL.Add(new UserInfo());
                RadGrid1.DataSource = UL;
                RadGrid1.DataBind();

                List<string> MasterQVWs = new List<string>();
                List<System.Dynamic.ExpandoObject> QVWItems = MillimanCommon.UserRepo.GetInstance().FindAllQVProjectsForUser("van.nanney@milliman.com", new string[] { Session["groupid"].ToString() }, false);
                string XML = string.Empty;
                if (QVWItems != null)
                {
                    RadTreeView RTV = RadPanelBar1.FindItemByValue("TreeHolder").FindControl("AccessTree") as RadTreeView;
                    if (RTV != null)
                    {
                        foreach (System.Dynamic.ExpandoObject EO in QVWItems)
                        {
                            dynamic D = EO;
                            List<string> HierarchyFiles = MillimanCommon.ReducedQVWUtilities.GetHierarchyFilenames(D.QVFilename.ToString());
                            MasterQVWs.Add(D.QVFilename.ToString());
                            foreach (string Hierarchy in HierarchyFiles)
                            {
                                string Container = "<Node Text=\"_TEXT_\" Value=\"_VALUE_\" ImageUrl=\"_IMAGE_\">_XML_</Node>";
                                Container = Container.Replace("_TEXT_", System.IO.Path.GetFileNameWithoutExtension(D.QVFilename.ToString()));
                                Container = Container.Replace("_VALUE_", D.QVFilename.ToString());
                                Container = Container.Replace("_IMAGE_", "images/rootreport.gif");
                                MillimanCommon.MillimanTreeNode MTN = MillimanCommon.MillimanTreeNode.GetMemoryTree(Hierarchy);

                                string ContainerXML = MTN.ToBindableXML();//System.IO.File.ReadAllText(Hierarchy);
                                MTN.SubNodes.Clear();
                                MTN = null;
                                Container = Container.Replace("_XML_", ContainerXML);
                                XML += Container;
                            }
                        }
                        RTV.LoadXml("<Tree>" + XML + "</Tree>");
                        LoadDownloads(MasterQVWs);

                    }
                    foreach (RadTreeNode RTN in RTV.GetAllNodes())
                    {
                        if (RTN.Level == 0)
                            RTN.Expanded = true;
                    }
                }
                else
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "User does not have access to any QVWs in group " + Session["groupid"].ToString() + " to administer users");
                }
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

        protected void RadGrid1_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (string.Compare(e.CommandName, "add", true) == 0)
            {
                List<UserInfo> UIList = ValidateUserRequests();
                bool AllGood = true;
                var errorMsg = string.Empty;
                foreach (UserInfo UIC in UIList)
                {
                    if (string.IsNullOrEmpty(UIC.ErrorMsg) == false)
                    {
                        errorMsg = errorMsg + UIC.ErrorMsg;
                        AllGood = false;
                    }
                }
                if (AllGood == false)
                {
                    RadGrid1.DataSource = UIList;
                    RadGrid1.Rebind();
                    MillimanCommon.Alert.Show(errorMsg + " To create users all errors must be corrected in the user list.  Check list items tagged with a red icon.");
                    return;
                }

                MillimanCommon.MillimanGroupMap.MillimanGroups MG = MillimanCommon.MillimanGroupMap.GetInstance().MillimanGroupDictionary[Session["groupid"].ToString()];
                string[] Users = System.Web.Security.Roles.GetUsersInRole(Session["groupid"].ToString());
                List<UserInfo> UL = ParseEmails(GridToList(RadGrid1));
                if (UL == null)
                    UL = new List<UserInfo>();

                int AvailableSlots = MG.MaximumnUsers - UL.Count();
                if (AvailableSlots <= 0)
                {
                    string NoLicenses = "Current license constraints limits '" + MG.FriendlyGroupName + "' to a maximum of " + MG.MaximumnUsers.ToString() + " users.\\n\\nFor additional users please contact PRM support for additional licenses.";
                    MillimanCommon.Alert.Show(NoLicenses);
                    return;
                }

                RadGrid1.DataSource = UL;
                RadGrid1.DataBind();

            }
            else if (string.Compare(e.CommandName, "validate", true) == 0)
            {
                List<UserInfo> UL = ValidateUserRequests();
                if (UL != null)
                {
                    RadGrid1.DataSource = UL;
                    RadGrid1.DataBind();
                }
            }
            else if (string.Compare(e.CommandName, "clear", true) == 0)
            {
                InitilizeScreen();
            }
        }
        // Page.ClientScript.RegisterClientScriptBlock(GetType(), "CloseScript", "CloseDialog()", true);

        protected void Reset_Click(object sender, EventArgs e)
        {
            InitilizeScreen();
        }
        private void InitilizeScreen()
        {
            //add an empty row
            var uInfoList = new List<UserInfo>();
            uInfoList.Add(new UserInfo("", false, false));
            RadGrid1.DataSource = uInfoList;
            RadGrid1.Rebind();

            RadTreeView AccessTree = (RadTreeView)RadPanelBar1.Items[0].Items[0].FindControl("AccessTree");

            if (AccessTree.Nodes.Count > 0)
            {
                AccessTree.SelectedNodes.Clear();
                foreach (RadTreeNode node in AccessTree.Nodes)
                {
                    if (node.Checked)
                        node.Checked = false;
                    if (node.Nodes.Count > 0)
                    {
                        foreach (RadTreeNode subNode in node.Nodes)
                        {
                            if (subNode.Checked)
                                subNode.Checked = false;
                        }
                    }
                }
            }

        }
        private List<UserInfo> GridToList(Telerik.Web.UI.RadGrid theGrid)
        {
            List<UserInfo> UI = new List<UserInfo>();
            foreach (Telerik.Web.UI.GridDataItem GDI in theGrid.Items)
            {
                TextBox AccountName = (TextBox)GDI.FindControl("AccountNameTextBox");
                CheckBox SendWelcome = (CheckBox)GDI.FindControl("SendWelcomeCheckbox");
                CheckBox DBRequired = (CheckBox)GDI.FindControl("DataAccessRequiredTextBox");

                //for some dump reason,  the items are not removed from the grid on post back
                //when deleted,  but they do not have values, so filter out delted rows with
                // logic below
                if ((string.IsNullOrEmpty(AccountName.Text) == false) ||

                    (DBRequired.Checked == true))
                {
                    UI.Add(new UserInfo(AccountName.Text,
                                         SendWelcome.Checked,
                                         DBRequired.Checked));
                }
            }
            return UI;
        }

        private string AlphaNums = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private string AccountValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-@.";
        private string EmailLocalSectionValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!#$%&'*+-/=?^_`{|}~.";
        private string EmailServerValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-.";
        private List<UserInfo> ValidateUserRequests()
        {
            List<UserInfo> UIList = GridToList(RadGrid1);
            List<string> ProcessedUserNames = new List<string>();
            foreach (UserInfo UI in UIList)
            {
                UI.SetStatus(UserInfo.StatusType.SUCCESS);
                if (ProcessedUserNames.Contains(UI.Account_Name_No_Password.ToLower()) == true)
                {
                    UI.ErrorMsg = "Account name '" + UI.Account_Name_No_Password + "' is duplicated in the list.  Please remove duplicate account names.";
                }
                ProcessedUserNames.Add(UI.Account_Name_No_Password.ToLower());

                if (string.IsNullOrEmpty(UI.Account_Name_No_Password) == true)
                {
                    UI.ErrorMsg = "Account name cannot be left empty";
                }
                else if (string.IsNullOrEmpty(UI.Account_Name_No_Password) == false)
                {

                    var isValid = MillimanHelper.ValidateUserNameInput(UI.Account_Name_No_Password);
                    if (isValid.Count > 0)
                    {
                        UI.ErrorMsg = string.Join(",", isValid.ToArray());
                        break;
                    }
                }
                //just call to check, sets error message and icon as needed
                UI.IsValidPassword();

            }

            return UIList;
        }

        private List<UserInfo> ParseEmails(List<UserInfo> UI)
        {
            List<UserInfo> NewUI = new List<UserInfo>();
            bool ParsedItems = false;
            char[] Delimiters = new char[] { ',', ';', '\r', '\n', ' ' };
            foreach (UserInfo User in UI)
            {
                string[] Tokens = User.Account_Name.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);
                if (Tokens.Count() > 1)
                {
                    ParsedItems = true;
                    foreach (string Token in Tokens)
                    {
                        NewUI.Add(new UserInfo(Token.Trim(), true, false));
                    }
                }
                else
                {
                    NewUI.Add(User);
                }
            }

            if (ParsedItems == false)  //we didn't find any multi line entries, so they clicked to add a new entry, so add one
                NewUI.Add(new UserInfo("", true, false));
            return NewUI;
        }

        private void RollBack(List<string> RemoveUsersFromRole, string Role, List<string> DeleteUsers, List<string> RemoveResetFiles)
        {
            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed adding users");
            //get rid of role for all users
            foreach (string RollbackUser in RemoveUsersFromRole)
            {
                System.Web.Security.Roles.RemoveUserFromRole(RollbackUser, Role);

            }
            //if a new user, delete account
            foreach (string NewUser in DeleteUsers)
            {
                System.Web.Security.Membership.DeleteUser(NewUser);
            }
            //get rid of reset files
            foreach (string RS in RemoveResetFiles)
                System.IO.File.Delete(RS);


        }

        protected void CreateUsers_Click(object sender, EventArgs e)
        {           
            //parse the username 
            List<UserInfo> UL = ParseEmails(GridToList(RadGrid1));
            if (UL == null)
                UL = new List<UserInfo>();
            //rebind the gird
            RadGrid1.DataSource = UL;
            RadGrid1.DataBind();

            //validate
            List<UserInfo> UI = ValidateUserRequests();
            foreach (UserInfo User in UI)
            {
                if (string.IsNullOrEmpty(User.ErrorMsg) != true)
                {
                    MillimanCommon.Alert.Show("All errors must be corrected before users can be added. Check list entries that have a red icon beside them.");
                    RadGrid1.DataSource = UI;
                    RadGrid1.DataBind();
                    return;
                }
            }

            int UserCount = 0;
            List<string> CreatedUsers = new List<string>();  //all users that were processed
            List<string> NewUsers = new List<string>();      //list of users that are new and need welcome email
            string GroupID = Session["groupid"].ToString();
            List<string> ResetFiles = new List<string>(); //save list for possible rollback
            try
            {
                if (string.IsNullOrEmpty(GroupID) != true)
                {
                    if (System.Web.Security.Roles.RoleExists(GroupID) == true)
                    {
                        List<string> UserDirectories = new List<string>(); //save list for possible rollack
                        try
                        {
                            string ProjectDir = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                            ProjectDir = System.IO.Path.Combine(ProjectDir, GroupID.Replace("_", "\\"));  //groups use underscore,  to convert to directory use slash
                            foreach (UserInfo User in UI)
                            {
                                bool AccountCreated = false;
                                System.Web.Security.MembershipUser MU = System.Web.Security.Membership.GetUser(User.Account_Name_No_Password);
                                if (MU == null)
                                {
                                    string Password = User.Password;
                                    if (string.IsNullOrEmpty(Password))
                                        Password = "@" + Guid.NewGuid().ToString();
                                    MU = System.Web.Security.Membership.CreateUser(User.Account_Name_No_Password, Password, User.Account_Name_No_Password);
                                    AccountCreated = true;
                                    NewUsers.Add(User.Account_Name_No_Password);
                                }
                                if (MU == null)
                                    throw new Exception("User account not created for: " + User.Account_Name_No_Password);
                                CreatedUsers.Add(User.Account_Name_No_Password);
                                if (System.Web.Security.Roles.IsUserInRole(User.Account_Name_No_Password, GroupID) == false)
                                    System.Web.Security.Roles.AddUserToRole(User.Account_Name_No_Password, GroupID);
                                UserCount++;
                                //create user password reset file
                                string ResetFile = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["ResetUserInfoRoot"], MU.ProviderUserKey + ".rst");
                                string WasWelcomed = User.SendWelcomeEmail == true ? "welcome" : "nowelcome";

                                //if a new account, then make them reset thier password, otherwise don't force a password reset
                                if (AccountCreated == true)
                                {
                                    System.IO.File.WriteAllText(ResetFile, MU.UserName + " added " + DateTime.Now.ToShortDateString() + " " + WasWelcomed);
                                    ResetFiles.Add(ResetFile);
                                }

                                string UserDir = MillimanCommon.ReducedQVWUtilities.GetUserDir(ProjectDir, User.Account_Name_No_Password);
                                if (System.IO.Directory.Exists(UserDir) == false)
                                {
                                    System.IO.Directory.CreateDirectory(UserDir);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            UserCount = 0;
                            RollBack(CreatedUsers, GroupID, NewUsers, ResetFiles);
                            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified", ex);
                        }
                    }
                    else
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, GroupID + " was passed in for adding user but stated role does not exist in system");
                        UserCount = 0;
                    }
                }
                else
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "No group id was passed to add users");
                    UserCount = 0;
                }
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified Error - Groups", ex);
                UserCount = 0;
            }


            try
            {
                if (UserCount > 0)
                {
                    //save all the tree info to file
                    RadTreeView AccessTree = RadPanelBar1.FindItemByValue("TreeHolder").FindControl("AccessTree") as RadTreeView;
                    RadTreeView DownloadTree = RadPanelBar1.FindItemByValue("DownloadHolder").FindControl("DownloadTree") as RadTreeView;

                    bool RequiresReduction = true;
                    MillimanCommon.TreeToFileUtilities TTFU = new MillimanCommon.TreeToFileUtilities();
                    if (TTFU.SaveSettings(MillimanCommon.ProjectSettings.Load(Session["ProjectPath"].ToString()), AccessTree, DownloadTree, CreatedUsers, out RequiresReduction) == false)
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to save settings");

                    bool AllNodesSelected = false;
                    List<MillimanCommon.TreeToFileUtilities.ReductionSelections> Selections = TTFU.GetAccessSelectionsForReduction(AccessTree, out AllNodesSelected);

                    MillimanReportReduction.QVWReductionProcessor QVWRepProc = new MillimanReportReduction.QVWReductionProcessor();

                    if (QVWRepProc.ProcessUsers(GroupID, CreatedUsers, Selections, AllNodesSelected) == false)
                    {

                        string UserList = string.Empty;
                        foreach (string S in CreatedUsers)
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
                            QVWList += "'" + RS.QualafiedQVW + "'";
                        }

                        RollBack(CreatedUsers, GroupID, NewUsers, ResetFiles);

                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to reduced " + QVWList + " for users " + UserList);
                        MillimanCommon.Alert.Show("An error occurred processing the request - no user accounts were created.");

                        return; //show a message and exit
                    }
                    else
                    {
                        //it worked - send out emails to those designated
                        foreach (UserInfo U in UI)
                        {
                            if (CreatedUsers.Contains(U.Account_Name_No_Password))
                            {
                                //only send emails to "NEW" users that do not have an account
                                if ((U.SendWelcomeEmail) && (NewUsers.Contains(U.Account_Name_No_Password)) && (U.HasPassword == false)) //if a password was provided don't send an email
                                {
                                    //if email checked - send email
                                    //string SecureLink = MillimanCommon.SecureLink.CreateSecureLink(System.Web.Security.Membership.GetUser().UserName, U.Account_Name, System.Web.Security.Membership.GetUser(U.Account_Name).ProviderUserKey.ToString());
                                    //VWN should load an email template and send
                                    string EmailTemplatesDir = Server.MapPath("~/email_templates");
                                    string EmailBody = System.IO.File.ReadAllText(System.IO.Path.Combine(EmailTemplatesDir, "welcome.htm"));
                                    string Body = MillimanCommon.MillimanEmail.EmailMacroProcessor(EmailBody, U.Account_Name_No_Password, System.Web.Security.Membership.GetUser(U.Account_Name_No_Password).ProviderUserKey.ToString(), System.Web.Security.Membership.GetUser().UserName);

                                    MillimanCommon.MillimanEmail ME = new MillimanCommon.MillimanEmail();
                                    string From = System.Configuration.ConfigurationManager.AppSettings["SupportEmail"];
                                    string Title = System.Configuration.ConfigurationManager.AppSettings["SupportEmailWelcome"];
                                    ME.Send(U.Account_Name_No_Password, From, Body, Title, true, false);
                                }
                            }
                        }
                    }

                    AddUser.Promote(GroupID, CreatedUsers);

                    string Msg = "User accounts(s):\\n\\n";
                    foreach (string User in CreatedUsers)
                        Msg += User + "\\n";
                    Msg += "\\n have been added to the system.";

                    string script = "<script type=\"text/javascript\">CloseDialog('" + Msg + "');</script>";
                    //string script = "<script type=\"text/javascript\">CloseDialog('" + Msg + "');</script>";
                    // Gets the executing web page
                    Page page = HttpContext.Current.CurrentHandler as Page;
                    // Checks if the handler is a Page and that the script isn't allready on the Page
                    if (page != null && !page.ClientScript.IsStartupScriptRegistered("CloseMe"))
                    {
                        page.ClientScript.RegisterStartupScript(typeof(AddUser), "CloseMe", script);
                    }

                }
                else
                {
                    MillimanCommon.Alert.Show("An error occurred processing the request - no user accounts were created.");
                }
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified error - publisher", ex);
            }

        }

        /// <summary>
        /// Rename all "_tmp" files a current
        /// </summary>
        /// <param name="Group"></param>
        /// <param name="UserList"></param>
        static public void Promote(string Group, List<string> UserList)
        {
            string QVRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
            string Dir = System.IO.Path.Combine(QVRoot, Group.Replace('_', '\\'));  //replace underscroes in group name with back slash
            string[] TempFiles = System.IO.Directory.GetFiles(Dir, "*.*_tmp", System.IO.SearchOption.AllDirectories);
            string OriginalFile = string.Empty;
            foreach (string S in TempFiles)
            {
                OriginalFile = S.Replace("_tmp", "");
                System.IO.File.Delete(OriginalFile);
                System.IO.File.Move(S, OriginalFile);
            }

            MillimanReportReduction.QVWReductionProcessor.AuthorizeAllQVWs();
        }

        /// <summary>
        /// Delete all "_tmp" files
        /// </summary>
        /// <param name="Group"></param>
        static public void RollBack(string Group)
        {
            string QVRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
            string Dir = System.IO.Path.Combine(QVRoot, Group);
            string[] TempFiles = System.IO.Directory.GetFiles(Dir, "*.*_tmp");
            foreach (string S in TempFiles)
                System.IO.File.Delete(S);
        }
        ////SelectionsPerQVW ==
        ////[0]: "0|C:\\inetpub\\wwwroot\\InstalledApplications\\MillimanSite\\QVDocuments\\Demo\\DemoProject.qvw|DemoProject"
        ////[1]: "1|mem_report_hier_2|exc_mem_report_hier_2|Provider Location City 0001"
        ////[2]: "1|mem_report_hier_2|exc_mem_report_hier_2|Provider Location City 0002"
        ////[3]: "2|mem_report_hier_3|exc_mem_report_hier_3|Provider Location Name 0002"
        ////[4]: "2|mem_report_hier_3|exc_mem_report_hier_3|Provider Location Name 0007"


    }
}