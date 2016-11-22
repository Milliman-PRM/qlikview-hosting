using System.Web.Configuration;
using System.Xml;
using System;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Collections.Generic;

namespace ClientPublisher
{
    public partial class _Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string PublishToSupergroup = CheckForLogin();
                if (string.IsNullOrEmpty(PublishToSupergroup))
                {
                    Response.Redirect("HTML/NotAuthorizedIssue.html");
                    return;
                }

                if (Membership.GetUser() == null)
                {
                    Response.Redirect("userlogin.aspx");
                    return;
                }

                Session["supergroup"] = PublishToSupergroup;
                CacheCleaner(7); // cleanup the cache

                LoadProjects(PublishToSupergroup);
            }

            //always check if I'm not authenticated, I should not be here
            if (string.IsNullOrEmpty(User.Identity.Name) == true)
            {
                Response.Redirect("HTML/NotLoggedIn.html");
            }
        }

        /// <summary>
        /// Check for login will verify the authentication cookie against the server side token to 
        /// ensure someone is not trying to break in via user emulation
        /// </summary>
        /// <returns></returns>
        private string CheckForLogin()
        {
            string Key = Request["key"];

            if (string.IsNullOrEmpty(Key) == false)
            {
                string CacheDir = System.Configuration.ConfigurationManager.AppSettings["HCIntelCache"];  //should be full path in web.config
                string CachePathFileName = System.IO.Path.Combine(CacheDir, Key);
                Session["SSOToken"] = CachePathFileName + ".xml";  //when the session expires we delete the cache file in global.asx
                MillimanCommon.CacheEntry CE = MillimanCommon.CacheEntry.Load(CachePathFileName);
                if (string.IsNullOrEmpty(User.Identity.Name) == false) //hmmm, there is a user with access, but is the the correct user or a trick
                {
                    MembershipUser CurrentLoggedInUser = Membership.GetUser();
                    string CurrentLoggedInUserID = (CurrentLoggedInUser == null ? "[notloggedin]" : CurrentLoggedInUser.ProviderUserKey.ToString());
                    if (CE != null)
                    {
                        //tokens are only good for 2 hours
                        if (CE.Expires < DateTime.Now)
                        {
                            Response.Redirect("HTML/NotAuthorizedIssue.html");
                            return string.Empty;
                        }
                        //make sure I am the origin of the token and not somebody trying to trick the system to get in via cut/paste of URLs                    
                        else if (CurrentLoggedInUserID != CE.UserKey.ToString())
                        {
                            Response.Redirect("HTML/NotAuthorizedIssue.html");
                            return string.Empty;
                        }
                        //check to see if the indicated user is online via main application
                        else if ((Membership.GetUser(CE.UserName).IsOnline == false))
                        {
                            Response.Redirect("HTML/NotLoggedIn.html");
                            return string.Empty;
                        }
                        //don't do this, causes cookie issues - if you got here you are already authenticated
                        //if (System.IO.File.Exists(CachePathFileName))  //make sure cache file exists, there is a race condition btween session end and the metatag call back, this check is a last second check against the race condition
                        //{
                        //    if (Membership.ValidateUser(CE.UserName, Membership.GetUser(CE.UserName).GetPassword()) == true)
                        //    {
                        //        FormsAuthentication.SetAuthCookie(CE.UserName, false);
                        //        Response.Redirect("Default.aspx");  //we have to bounce the page to make sure authenication takes effect
                        //    }
                        //    else
                        //    {
                        //        Response.Redirect("HTML/NotAuthorizedIssue.html");
                        //        return string.Empty;
                        //    }
                        //}
                        Session["Account"] = CE.UserName;
                        return CE.MillimanGroupName;
                    }
                    else
                    {
                        Response.Redirect("HTML/NotAuthorizedIssue.html");
                    }
                }
                else
                {
                    Response.Redirect("HTML/NotLoggedIn.html");
                    return string.Empty;
                }
            }
            return "";
        }

        /// <summary>
        /// Remove all cached credentials older than the specified date
        /// </summary>
        private void CacheCleaner(int ClearOlderThanXDays)
        {
            string CacheDir = WebConfigurationManager.AppSettings["HCIntelCache"];
            string[] AllFiles = System.IO.Directory.GetFiles(CacheDir, "*");
            foreach (string F in AllFiles)
            {
                if (System.IO.File.GetCreationTime(F).AddDays((double)ClearOlderThanXDays) < DateTime.Now)
                    System.IO.File.Delete(F);
            }
        }

        private bool IsPublishingAdmin()
        {
            MembershipUser MU = Membership.GetUser();
            if (MU != null)
            {
                string UserId = MU.ProviderUserKey.ToString();
                try
                {
                    string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["dbMyCMSConnectionString"].ConnectionString;
                    System.Data.SqlClient.SqlCommand comm = new System.Data.SqlClient.SqlCommand();
                    comm.Connection = new System.Data.SqlClient.SqlConnection(ConnectionString);
                    String sql = @"SELECT IsPublishingAdministrator from aspnet_customprofile where UserId='" + UserId.ToUpper() + "'";
                    comm.CommandText = sql;
                    comm.Connection.Open();
                    System.Data.SqlClient.SqlDataReader cursor = comm.ExecuteReader();
                    while (cursor.Read())
                    {
                        string Value = cursor["IsPublishingAdministrator"].ToString();
                        if (string.IsNullOrEmpty(Value) == false)
                            return System.Convert.ToBoolean(Value);
                    }
                    comm.Connection.Close();
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return false;
        }
        /// <summary>
        /// given the users ID,  do they have to reset thier password and enter info
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        private bool IsNewUser(string UserID)
        {
            try
            {
                string ResetFile = System.IO.Path.Combine(WebConfigurationManager.AppSettings["ResetUserInfoRoot"], UserID + ".rst");
                return System.IO.File.Exists(ResetFile);
            }
            catch (Exception)
            {

            }

            return false;
        }

        private string CreateCacheEntry(string ConnectionStringFriendlyName, string ConnectionString)
        {
            string CacheDir = WebConfigurationManager.AppSettings["HCIntelCache"];  //should be full path in web.config
            string CacheFileName = Guid.NewGuid().ToString().Replace('-', '_');
            string CachePathFileName = System.IO.Path.Combine(CacheDir, CacheFileName);
            string UserKey = Membership.GetUser().ProviderUserKey.ToString();
            MillimanCommon.CacheEntry CE = new MillimanCommon.CacheEntry(ConnectionStringFriendlyName, ConnectionString, UserKey, Membership.GetUser().UserName, "", DateTime.Now.AddDays(1.0));
            CE.Save(CachePathFileName);

            return System.IO.Path.GetFileNameWithoutExtension(CacheFileName);
        }


        /// <summary>
        /// Check to see if the currently logged in user has admin rights
        /// </summary>
        /// <returns></returns>
        private bool IAmAdministrator()
        {
            string[] UserRoles = Roles.GetRolesForUser();
            foreach (string Role in UserRoles)
            {
                if (string.Compare(Role, "Administrator", true) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// load the projects into the grid
        /// </summary>
        private void LoadProjects(string PublishToSupergroup)
        {
            //make sure we are still logged in, if not exit
            if (Membership.GetUser() == null)
            {
                Response.Redirect("userlogin.aspx");
                return;
            }

            MillimanCommon.SuperGroup.SuperGroupContainer FoundSuperGroup = null;

            //make sure is a client admin trying to run, otherwise exit
            if ((IsPublishingAdmin() == false) && (IAmAdministrator() == false))
            {
                Response.Redirect("HTML/NoneClientAdmin.html");
                return;
            }
            string[] UserRoles = MillimanCommon.UserAccessList.GetRolesForUser();
            List<MillimanCommon.SuperGroup.SuperGroupContainer> SuperGroups = MillimanCommon.SuperGroup.GetInstance().GetSupersForPublishingAdmin(Membership.GetUser().UserName);
            if (SuperGroups == null)
            {
                Response.Redirect("HTML/NoGroupsAllocated.html");
                return;
            }
            else
            {
                foreach (MillimanCommon.SuperGroup.SuperGroupContainer SGC in SuperGroups)
                {
                    if (string.Compare(PublishToSupergroup, SGC.ContainerName, true) == 0)
                    {
                        FoundSuperGroup = SGC;
                        break;
                    }
                }

                if (FoundSuperGroup == null)
                {
                    Response.Redirect("HTML/NoProjectsReady.html");
                }

            }


            int ProjectIndex = 0;
            IList<ProjectSettingsExtension> Projects = new List<ProjectSettingsExtension>();
            foreach (string Group in FoundSuperGroup.GroupNames)
            {
                List<string> GroupProjects = MillimanCommon.UserRepo.GetInstance().FindAllProjectsForRole(Group);
                if (GroupProjects != null)
                {
                    if (GroupProjects.Count > 1)
                    {
                        Response.Redirect("HTML/GroupContainsMultipleProjects.html");
                        return;
                    }
                    if (GroupProjects.Count == 1)
                    {
                        ProjectSettingsExtension ThisProject = ProjectSettingsExtension.LoadProjectExtension(GroupProjects[0]);
                        try
                        {
                            if (ThisProject != null)
                            {
                                ThisProject.ProjectIndex = ProjectIndex;
                                ProjectIndex++;
                                Projects.Add(ThisProject);
                            }

                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
            }

            if (Projects.Count == 0)
            {
                Response.Redirect("HTML/NoProjectsReady.html");
                return;
            }

            RadProjectList.DataSource = Projects;
            RadProjectList.DataBind();
            //place in session so I don't have to look up all the info again
            Session["Projects"] = Projects;


        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {

        }

        protected void RadMenu1_ItemClick(object sender, Telerik.Web.UI.RadMenuEventArgs e)
        {
            if (string.Compare(e.Item.Value.ToString(), "logout", true) == 0)
            {
                try
                {
                    Response.Redirect("UserLogin.aspx");
                }
                catch (Exception exception)
                {
                    Trace.Write("ServiceProvider", "Error on logout page", exception);
                }
            }
        }

        protected void EditProject_Click(object sender, EventArgs e)
        {
            Telerik.Web.UI.RadButton RB = sender as Telerik.Web.UI.RadButton;
            Telerik.Web.UI.RadListViewDataItem item = RB.Parent as Telerik.Web.UI.RadListViewDataItem;
            int Index = item.DataItemIndex;

            Telerik.Web.UI.RadWindowManager windowManager = new Telerik.Web.UI.RadWindowManager();
            Telerik.Web.UI.RadWindow widnow1 = new Telerik.Web.UI.RadWindow();
            widnow1.Title = "Project Edit";
            widnow1.Modal = true;
            widnow1.Width = 800;
            widnow1.Height = 600;
            widnow1.Skin = "Silk";
            //widnow1.AutoSize = true;
            widnow1.OnClientClose = "Hi";
            // Set the window properties    
            widnow1.NavigateUrl = "HTML/noprojectsready.html?Silly=" + Index.ToString();
            widnow1.ID = "Window_" + Index.ToString();
            widnow1.VisibleOnPageLoad = true; // Set this property to True for showing window from code    
            windowManager.Windows.Add(widnow1);
            this.test.Controls.Add(widnow1);

        }

        protected void ViewQVW_Click(object sender, EventArgs e)
        {
            Telerik.Web.UI.RadButton RB = sender as Telerik.Web.UI.RadButton;
            Telerik.Web.UI.RadListViewDataItem item = RB.Parent as Telerik.Web.UI.RadListViewDataItem;
            int Index = item.DataItemIndex;
            //dashboard.aspx?key=" + MillimanCommon.Utilities.ConvertStringToHex(ACL_Entity.QVRootRelativeProjectPath)

            if (Session["Projects"] != null)
            {
                IList<ProjectSettingsExtension> Projects = Session["Projects"] as List<ProjectSettingsExtension>;
                if (Index < Projects.Count)
                {
                    string QVWVirtualPath = System.IO.Path.Combine(Projects[Index].VirtualDirectory, Projects[Index].QVName + ".qvw");
                    string URL = "dashboard.aspx?key=" + MillimanCommon.Utilities.ConvertStringToHex(QVWVirtualPath);
                }
            }
        }

        /// <summary>
        /// used to reload main page via ajax and keep div windows active
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UpdatePanel_Button_Click(object sender, EventArgs e)
        {
            LoadProjects(Session["supergroup"].ToString());
        }

        protected void ToggleAvailability_Click(object sender, EventArgs e)
        {

            Telerik.Web.UI.RadButton RB = sender as Telerik.Web.UI.RadButton;
            Telerik.Web.UI.RadListViewDataItem item = RB.Parent as Telerik.Web.UI.RadListViewDataItem;
            int Index = item.DataItemIndex;//which one item I click on in the grid
            if (Session["Projects"] != null)
            {
                IList<ProjectSettingsExtension> Projects = Session["Projects"] as List<ProjectSettingsExtension>;
                if (Index < Projects.Count)
                {
                    string QVWOnline = System.IO.Path.Combine(Projects[Index].AbsoluteProjectPath, Projects[Index].QVName + ".qvw");
                    string QVWOffline = System.IO.Path.Combine(Projects[Index].AbsoluteProjectPath, Projects[Index].QVName + ".offline");

                    //initiate the project extension class
                    ProjectSettingsExtension pj = new ProjectSettingsExtension();
                    //set project path for project extension
                    pj.AbsoluteProjectPath = Projects[Index].AbsoluteProjectPath;
                    //set QVW for project extension
                    pj.QVName = Projects[Index].QVName;
                    if (System.IO.File.Exists(QVWOffline))
                    {
                        System.IO.File.Delete(QVWOffline);  //get rid of offline file, we want ot go online
                        RB.Text = ProjectSettingsExtension.IsAvailable;                        
                        RB.ToolTip = pj.AvailabilityTooltip;// "Report is available to users - click to make report unavailable.";                      
                    }
                    else
                    {
                        System.IO.File.WriteAllText(QVWOffline, System.DateTime.Now.ToString());  //create an offline file, can be empt
                        RB.Text = ProjectSettingsExtension.IsOffline;
                        RB.ToolTip = pj.AvailabilityTooltip;//"Report is present, but in an offline state - click to make it available to users.";
                    }
                }
            }
        }
    }
}