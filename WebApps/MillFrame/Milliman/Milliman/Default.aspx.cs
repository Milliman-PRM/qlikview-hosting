using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using System.Xml;
using System;
using System.Text;
using ComponentPro.Saml2;
using System.Web;
using System.Web.Security;
using System.Collections;
using System.Collections.Generic;

namespace MillimanDev
{
    public partial class _Default : System.Web.UI.Page
    {
        const string NoFilter = "---------------";

        protected void Page_Load(object sender, EventArgs e)
        {
            //MillimanReportReduction.QVWCaching QVWC = new MillimanReportReduction.QVWCaching(@"D:\InstalledApplications\PRM\QVDocuments\0273WOH01\Medicaid\WOAH\Care Coordinator Report.qvw");
            //QVWC.ProcessOld(@"D:\InstalledApplications\PRM\QVDocuments\0273WOH01\Medicaid\WOAH");


            if (!IsPostBack)
            {
                if (Membership.GetUser() == null)
                {
                    Response.Redirect("userlogin.aspx");
                    return;
                }

                CacheCleaner(7); // cleanup the cache

                if (IsNewUser(Membership.GetUser().ProviderUserKey.ToString()) == true)
                {
                    Response.Redirect("profile.aspx?newuser=true");
                    return;
                }

                string MenuXML = System.IO.File.ReadAllText(Server.MapPath("~/MainMenuConfiguration/MainMenu.xml"));
                RadMenu1.LoadXml(ProcessForPublisherAdmin( ProcessForClientAdmin(MenuXML) ));

                LoadAnnouncements();
                LoadProducts();

                string[] UserRoles = MillimanCommon.UserAccessList.GetRolesForUser();
                if (UserRoles.Length > 1)
                {  //show filter options
                    FilterLabel.Visible = true;
                    Groups.Visible = true;
                    Groups.Items.Add(NoFilter); //add the don't filter anything string
                    foreach (string S in UserRoles)
                        Groups.Items.Add(S);
                    Groups.SelectedIndex = 0;
                }

                string FilterUsers = WebConfigurationManager.AppSettings["ShowFilter"].ToLower();
                if ((FilterUsers == null) || (FilterUsers.IndexOf(Membership.GetUser().UserName.ToLower()) == -1))
                { //only filter user can see the groups which are really CC codes - only makes sense to Milliman employees
                    FilterLabel.Visible = false;
                    Groups.Visible = false;
                }
            }
            else
            {
                //this will correct the issue of postback for end-users for not for admins with "filter-by" permissions
                if (Groups.SelectedItem == null) 
                {
                    LoadProducts();
                }
            }
        }

        private string ProcessForPublisherAdmin(string XML)
        {
            string ReplacementTag = "_INSERT_PUBLISHER_ADMIN_ITEM_";
            bool IsPublisher = GetAdminByType(AdminType.Publisher);
            if (IsPublisher == false)
            {
                return XML.Replace(ReplacementTag, "");
            }

            MillimanCommon.SuperGroup SG = MillimanCommon.SuperGroup.GetInstance();
            List<MillimanCommon.SuperGroup.SuperGroupContainer> MyItems = SG.GetSupersForPublishingAdmin( Membership.GetUser().UserName);
            if (MyItems.Count == 0)
            {
                return XML.Replace(ReplacementTag, "");  //they do not have anything setup via global admin 
            }
            else if (MyItems.Count == 1)
            {
                string MenuItem = CreateMenuItem("Publish Content", "Publish new report content", Membership.GetUser().UserName, true);
                return XML.Replace(ReplacementTag, MenuItem);
            }
            else
            {
                string SubMenus = string.Empty;
                string MainEntry = @"<Item Text='Publish Content' Width='200px' LeftLogo='images/user-group-icon.png' ToolTip='_TOOLTIP_'><Group Flow='Vertical'>_ITEMS_</Group></Item>";
                foreach (MillimanCommon.SuperGroup.SuperGroupContainer SGC in MyItems)
                {
                    string SubMenu = CreateMenuItem("Publish content for " + SGC.ContainerName, SGC.ContainerDescription, SGC.ContainerName, true);
                    SubMenus += SubMenu;
                }
                MainEntry = MainEntry.Replace("_ITEMS_", SubMenus);
                MainEntry = MainEntry.Replace("_TOOLTIP_", "Administer users and control access rights.");
                return XML.Replace(ReplacementTag, MainEntry);
            }
        }

        /// <summary>
        /// Check to see if user is supposed to be able to admin thier own users,  if not
        /// remove the option on the menu to launch client admin interface, otherwise look into adding options for
        /// admin.  It is possible they are setup to admin clients, but not in a group, in this case remove the
        /// menu option
        /// </summary>
        /// <param name="XML"></param>
        /// <returns></returns>
        private string ProcessForClientAdmin(string XML)
        {
            //<Item Text='User Administration' Width='200px' LeftLogo="images/user-group-icon.png">
            //  <Group Flow='Vertical'>
            //    <Item Text='Administer users for Francisan CIR' Value='CIR' PostBack='true'/>
            //    <Item Text='Administer users for Francisan NIR' Value='NIR' />
            //    <Item Text='Administer users for Francisan WIR' Value='WIR'/>
            //  </Group>
            //</Item> 
            string ReplacementTag = "_INSERT_CLIENT_ADMIN_ITEM_";
            bool IsUserAdmin = GetAdminByType( AdminType.User);
            if (IsUserAdmin == false)
            {
                return XML.Replace(ReplacementTag, "");
            }
            else
            {
                MillimanCommon.SuperGroup SG = MillimanCommon.SuperGroup.GetInstance();

                 System.Collections.Generic.List<MillimanCommon.SuperGroup.SuperGroupContainer> SuperGroups = SG.GetSupersForClientAdmin(Membership.GetUser().UserName);

                string MainEntry = @"<Item Text='User Administration' Width='200px' LeftLogo='images/user-group-icon.png' ToolTip='_TOOLTIP_'><Group Flow='Vertical'>_ITEMS_</Group></Item>";

                string[] MyCurrentRoles = MillimanCommon.UserAccessList.GetRolesForUser();
                System.Collections.Generic.List<string> MyRoles = new System.Collections.Generic.List<string>(MyCurrentRoles);
                //don't use "Administrator" role - is build in default role
                for (int Index = 0; Index < MyRoles.Count; Index++)
                {
                    if (string.Compare(MyRoles[Index], "administrator", true) == 0)
                    {
                        MyRoles.RemoveAt(Index);
                        break;
                    }
                }
                if (SuperGroups.Count == 1)
                {
                    string SubMenu = CreateMenuItem("User Administration", "Administer users for " + SuperGroups[0].ContainerName, SuperGroups[0].ContainerName);
                    return XML.Replace(ReplacementTag, SubMenu);
                }
                else if (SuperGroups.Count > 1)
                {
                    string SubMenus = string.Empty;
                    foreach (MillimanCommon.SuperGroup.SuperGroupContainer SGC in SuperGroups)
                    {
                        string SubMenu = CreateMenuItem("Administer users for " + SGC.ContainerName, SGC.ContainerDescription, SGC.ContainerName);
                        SubMenus += SubMenu;
                    }
                    MainEntry = MainEntry.Replace("_ITEMS_", SubMenus);
                    MainEntry = MainEntry.Replace("_TOOLTIP_", "Administer users and control access rights.");
                    return XML.Replace(ReplacementTag, MainEntry);
                }
                else //am client admin but not in a group - get rid of menu option
                {
                    return XML.Replace(ReplacementTag, "");
                }
            }
        }

        //private string ProcessForClientAdmin(string XML)
        //{
        //    //<Item Text='User Administration' Width='200px' LeftLogo="images/user-group-icon.png">
        //    //  <Group Flow='Vertical'>
        //    //    <Item Text='Administer users for Francisan CIR' Value='CIR' PostBack='true'/>
        //    //    <Item Text='Administer users for Francisan NIR' Value='NIR' />
        //    //    <Item Text='Administer users for Francisan WIR' Value='WIR'/>
        //    //  </Group>
        //    //</Item> 
        //    string ReplacementTag = "_INSERT_CLIENT_ADMIN_ITEM_";
        //    bool IsUserAdmin = GetAdminByType(AdminType.User);
        //    if (IsUserAdmin == false)
        //    {
        //        return XML.Replace(ReplacementTag, "");
        //    }
        //    else
        //    {
        //        MillimanCommon.MillimanGroupMap MGM = MillimanCommon.MillimanGroupMap.GetInstance();

        //        string MainEntry = @"<Item Text='User Administration' Width='200px' LeftLogo='images/user-group-icon.png' ToolTip='_TOOLTIP_'><Group Flow='Vertical'>_ITEMS_</Group></Item>";

        //        string[] MyCurrentRoles = MillimanCommon.UserAccessList.GetRolesForUser();
        //        System.Collections.Generic.List<string> MyRoles = new System.Collections.Generic.List<string>(MyCurrentRoles);
        //        //don't use "Administrator" role - is build in default role
        //        for (int Index = 0; Index < MyRoles.Count; Index++)
        //        {
        //            if (string.Compare(MyRoles[Index], "administrator", true) == 0)
        //            {
        //                MyRoles.RemoveAt(Index);
        //                break;
        //            }
        //        }
        //        string FriendlyGroupName = "????";
        //        if (MyRoles.Count == 1)
        //        {
        //            FriendlyGroupName = MGM.MillimanGroupDictionary[MyRoles[0]].FriendlyGroupName;
        //            string SubMenu = CreateMenuItem("User Administration", "Administer users for " + FriendlyGroupName, MyRoles[0]);
        //            //don't show if no users are allowed
        //            if (MGM.MillimanGroupDictionary[MyRoles[0]].MaximumnUsers > 0)
        //                return XML.Replace(ReplacementTag, SubMenu);
        //            return "";
        //        }
        //        else if (MyRoles.Count > 1)
        //        {
        //            string SubMenus = string.Empty;
        //            foreach (string Role in MyRoles)
        //            {
        //                if (MGM.MillimanGroupDictionary.ContainsKey(Role) == true)
        //                {  //if we do not have a friendly name dont show a menu item
        //                    FriendlyGroupName = MGM.MillimanGroupDictionary[Role].FriendlyGroupName;
        //                    if (string.IsNullOrEmpty(FriendlyGroupName) == false)
        //                    {
        //                        string SubMenu = CreateMenuItem("Administer users for " + FriendlyGroupName, "", Role);
        //                        if (MGM.MillimanGroupDictionary[Role].MaximumnUsers > 0)
        //                            SubMenus += SubMenu;
        //                    }
        //                }
        //            }
        //            MainEntry = MainEntry.Replace("_ITEMS_", SubMenus);
        //            MainEntry = MainEntry.Replace("_TOOLTIP_", "Administer users and control access rights.");
        //            return XML.Replace(ReplacementTag, MainEntry);
        //        }
        //        else //am client admin but not in a group - get rid of menu option
        //        {
        //            return XML.Replace(ReplacementTag, "");
        //        }
        //    }
        //}
        private string CreateMenuItem(string DisplayText, string ToolTip, string ParameterItem, bool LaunchPublisher = false)
        {
            string SubEntry = @"<Item Text='_GROUPFRIENDLYNAME_' NavigateUrl='_URL_' Target='_blank' ToolTip='_TOOLTIP_' ImageUrl='images/User-Group-icon.png'/>";
            if ( LaunchPublisher )
                SubEntry = @"<Item Text='_GROUPFRIENDLYNAME_' NavigateUrl='_URL_' Target='_blank' ToolTip='_TOOLTIP_' ImageUrl='images/upload.png'/>";
            string CacheDir = WebConfigurationManager.AppSettings["HCIntelCache"];  //should be full path in web.config
            string CacheFileName = Guid.NewGuid().ToString().Replace('-', '_');
            string CachePathFileName = System.IO.Path.Combine(CacheDir, CacheFileName);
            SubEntry = SubEntry.Replace("_GROUPFRIENDLYNAME_", DisplayText);
            SubEntry = SubEntry.Replace("_TOOLTIP_", ToolTip);
            MillimanCommon.CacheEntry CE = new MillimanCommon.CacheEntry(Membership.GetUser().ProviderUserKey.ToString(), Membership.GetUser().UserName, ParameterItem, DateTime.Now.AddHours(2.0));
            CE.Save(CachePathFileName);
            string ClientAdminApplication = WebConfigurationManager.AppSettings["ClientAdminApp"];
            if ( LaunchPublisher )
                ClientAdminApplication = WebConfigurationManager.AppSettings["ClientPublishingApp"];
            string Launch = ClientAdminApplication + "?key=" + System.IO.Path.GetFileNameWithoutExtension(CachePathFileName);
            SubEntry = SubEntry.Replace("_URL_", Launch);
            return SubEntry;
        }

        /// <summary>
        /// Remove all cached credentials older than the specified date
        /// </summary>
        private void CacheCleaner(int ClearOlderThanXDays )
        {
            string CacheDir = WebConfigurationManager.AppSettings["HCIntelCache"];
            string[] AllFiles = System.IO.Directory.GetFiles(CacheDir, "*");
            foreach (string F in AllFiles)
            {
                if (System.IO.File.GetCreationTime(F).AddDays((double)ClearOlderThanXDays) < DateTime.Now)
                    System.IO.File.Delete(F);
            }
        }

        private enum AdminType {  User, Publisher }
        private bool GetAdminByType( AdminType Admin)
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
                    String sql = @"SELECT IsClientAdministrator from aspnet_customprofile where UserId='" + UserId.ToUpper() + "'";
                    if ( Admin == AdminType.Publisher)
                        sql = @"SELECT IsPublishingAdministrator from aspnet_customprofile where UserId='" + UserId.ToUpper() + "'";
                    comm.CommandText = sql;
                    comm.Connection.Open();
                    System.Data.SqlClient.SqlDataReader cursor = comm.ExecuteReader();
                    while (cursor.Read())
                    {
                        string Value = string.Empty;
                        if ( Admin == AdminType.User )
                            Value = cursor["IsClientAdministrator"].ToString();
                        else
                            Value = cursor["IsPublishingAdministrator"].ToString();

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
                string ResetFile = System.IO.Path.Combine( WebConfigurationManager.AppSettings["ResetUserInfoRoot"], UserID + ".rst" );
                return System.IO.File.Exists(ResetFile);
            }
            catch (Exception)
            {

            }
  
            return false;
        }

        private string CreateCacheEntry(string ConnectionStringFriendlyName, string ConnectionString )
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
        /// This is bad- but don't have time to correct it for now
        /// </summary>
        /// <param name="ACL_Entity"></param>
        private void LoadProduct(MillimanCommon.UserAccessList.UserAccess ACL_Entity)
        {
 
            string DBBrowser = WebConfigurationManager.AppSettings["DatabaseBrowserURL"]; //full url to browser page
            string QVDocumentRoot = WebConfigurationManager.AppSettings["QVDocumentRoot"];

            string NoDatabase = "<img src='images/nodatabase.png' title='Direct database access is not available.' style='width:15px;height:15px;border-style:none;vertical-align:middle' />";
            string DataBase = "<a href='_DBBROWSERURL_' target='_blank'><img src='images/database.png' title='Database _DATABASENAME_ is available.' style='width:15px;height:15px;border-style:none;vertical-align:middle' /></a>";

            System.Web.UI.WebControls.TableRow ProductRow = new System.Web.UI.WebControls.TableRow();
            System.Web.UI.WebControls.TableCell ProductCell = new System.Web.UI.WebControls.TableCell();
            //ProductCell.Text = " <table cellspacing='0'><tr><td align='right' style='background-image:url(images/header.gif);border:1px solid gray'><a href='dashboard.aspx?dashboardid=POPULATION'> <img src='images/nodatabase.png' title='Direct database access is not available.' style='width:15px;height:15px;border-style:none;vertical-align:middle' /> </a></td></tr><tr><td style='border:1px solid gray' ><a href='dashboard.aspx?dashboardid=POPULATION' target='_blank'>  <img src='Css/populationreport.gif' style='border-style:none'></img></a></td></tr></table>";
            ProductCell.Text = " <table cellspacing='0'><tr><td align='right' style='background-image:url(images/header.gif);border:1px solid gray'> _DOWNLOAD1_ _DOWNLOAD2_ _DOWNLOAD3_ _DOWNLOAD4_ _DOWNLOAD5_ _DOWNLOAD6_ _DBACCESS_</td></tr><tr><td style='border:1px solid gray' align='middle'  ><a href='_DASHBOARD_' target='_blank' onclick='return _ENABLED_;' ><img src='imagereflector.aspx?key=_THUMBNAIL_' title='Click to launch - _REPORTNAME_ ' style='border-style:none'></img></a></td></tr></table>";
            //ProductCell.Text = " <table cellspacing='0'><tr><td align='right' style='background-image:url(images/header.gif);border:1px solid gray'> _DBACCESS_ </td></tr><tr><td style='border:1px solid gray' align='middle'  ><a href='_DASHBOARD_' target='_blank'><img src='imagereflector.aspx?key=_THUMBNAIL_' title='Click to launch - _REPORTNAME_ ' style='border-style:none'></img></a></td></tr></table>";
            
            // #37 if I am admin let me always see a QVW reduced if available, master otherwise - but I will show special icon
            bool AmAdministrator = IAmAdministrator();
            if (AmAdministrator)
                if (string.IsNullOrEmpty(ACL_Entity.QVReducedRelativeProjectPath) == true ) //show ADMIN the master QVW to admin
                    ProductCell.Text = ProductCell.Text.Replace("_DASHBOARD_", @"dashboard.aspx?key=" + MillimanCommon.Utilities.ConvertStringToHex(ACL_Entity.QVRootRelativeProjectPath));
                else  //show ADMIN redcued version
                    ProductCell.Text = ProductCell.Text.Replace("_DASHBOARD_", @"dashboard.aspx?key=" + MillimanCommon.Utilities.ConvertStringToHex(ACL_Entity.QVReducedRelativeProjectPath));
            else  
                ProductCell.Text = ProductCell.Text.Replace("_DASHBOARD_", @"dashboard.aspx?key=" + MillimanCommon.Utilities.ConvertStringToHex(ACL_Entity.QVReducedRelativeProjectPath));

            if (ACL_Entity.ReducedVersionNotAvailable)
            {
                if (AmAdministrator) // #37 show a special icon
                    ProductCell.Text = ProductCell.Text.Replace("_THUMBNAIL_", MillimanCommon.Utilities.ConvertStringToHex(System.IO.Path.Combine(HttpContext.Current.Request.MapPath(HttpContext.Current.Request.ApplicationPath), "images/reportnotavailableADMINOVERIDE.gif")));
                else
                    ProductCell.Text = ProductCell.Text.Replace("_THUMBNAIL_", MillimanCommon.Utilities.ConvertStringToHex(System.IO.Path.Combine(HttpContext.Current.Request.MapPath(HttpContext.Current.Request.ApplicationPath), "images/reportnotavailable.gif")));
            }
            else
            {
                ProductCell.Text = ProductCell.Text.Replace("_THUMBNAIL_", MillimanCommon.Utilities.ConvertStringToHex(System.IO.Path.Combine(ACL_Entity.ProjectSettings.AbsoluteProjectPath, ACL_Entity.ProjectSettings.QVThumbnail)));
            }
            // #37 allow global admin to launch master, even if no reduced version available
            string MakeClickable = (!ACL_Entity.ReducedVersionNotAvailable).ToString().ToLower();
            if (AmAdministrator)
                MakeClickable = "true";

            ProductCell.Text = ProductCell.Text.Replace("_ENABLED_", MakeClickable );

            //look to see if there is a tooltip, if so use it
            string Tooltip = System.IO.Path.GetFileNameWithoutExtension(ACL_Entity.ProjectSettings.ProjectName);
            if (string.IsNullOrEmpty(ACL_Entity.ProjectSettings.QVTooltip) == false)
                Tooltip = ACL_Entity.ProjectSettings.QVTooltip;

            ProductCell.Text = ProductCell.Text.Replace("_REPORTNAME_", Tooltip);
            ProductCell.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
            if (string.IsNullOrEmpty(ACL_Entity.ProjectSettings.DBConnectionString) == true)
            {
                ProductCell.Text = ProductCell.Text.Replace("_DBACCESS_", NoDatabase);
            }
            else
            {
                string CacheKey = CreateCacheEntry(ACL_Entity.ProjectSettings.FriendlyDBName, ACL_Entity.ProjectSettings.DBConnectionString);
                DataBase = DataBase.Replace("_DBBROWSERURL_", DBBrowser + @"?key=" + CacheKey);
                DataBase = DataBase.Replace("_DATABASENAME_", ACL_Entity.ProjectSettings.FriendlyDBName);
                ProductCell.Text = ProductCell.Text.Replace("_DBACCESS_", DataBase);
            }

            //check for custom downloads
            if ( Membership.GetUser() != null )
            {
               string DownloadItemTemplate = "<a href='reflector.ashx?key=_KEY_' target='_blank'><img src='_ICON_' title='_TOOLTIP_' style='width:15px;height:15px;border-style:none;vertical-align:middle' /></a>";

               string AccountName = Membership.GetUser().UserName;
               MillimanCommon.CustomUserDownloads CUD = MillimanCommon.CustomUserDownloads.GetInstance();
               int DownloadIndex = 1; //yes start at 1
               string QVWRelativePath = string.IsNullOrEmpty(ACL_Entity.QVReducedRelativeProjectPath) ? ACL_Entity.QVRootRelativeProjectPath : ACL_Entity.QVReducedRelativeProjectPath;
               foreach (MillimanCommon.CustomUserDownloads.CustomDownloads CD in CUD.GetUserSpecficDownloads(AccountName, ACL_Entity.QVRootRelativeProjectPath))
               {
                   string ReplacementLabel = "_DOWNLOAD" + DownloadIndex.ToString() + "_";
                   DownloadIndex++;
                   string NewDownloadItem = DownloadItemTemplate.Replace("_KEY_", MillimanCommon.Utilities.ConvertStringToHex( System.IO.Path.Combine(QVDocumentRoot, CD.VirtualItemPath)));
                   //string IconReflector = "reflector.ashx?key=" + MillimanCommon.Utilities.ConvertStringToHex( System.IO.Path.Combine(QVDocumentRoot,CD.VirtualItemIcon));
                   string IconReflector = "reflector.ashx?key=" + MillimanCommon.Utilities.ConvertStringToHex(System.IO.Path.Combine(QVDocumentRoot, CD.VirtualItemIcon));
                   //NewDownloadItem = NewDownloadItem.Replace("_ICON_", CD.VirtualItemIcon);
                   
                   //this allows backward compatabiltiy as we transform to new framework
                   if (( CD.VirtualItemIcon.ToLower().Contains(@"images/")) || ( CD.VirtualItemIcon.ToLower().Contains(@"images\")))
                       NewDownloadItem = NewDownloadItem.Replace("_ICON_", CD.VirtualItemIcon);  //new method contains all icons in IMAGES dir
                   else
                       NewDownloadItem = NewDownloadItem.Replace("_ICON_",IconReflector);  //old way, icons were located with documents


                   NewDownloadItem = NewDownloadItem.Replace("_TOOLTIP_", CD.Tooltip);
                   ProductCell.Text = ProductCell.Text.Replace(ReplacementLabel, NewDownloadItem);
                   //_DOWNLOAD1_ _DOWNLOAD2_ _DOWNLOAD3_ _DOWNLOAD4_ _DOWNLOAD5_ _DOWNLOAD6_
               }
            }
            //replace any custom downloads with transparnt icon
            for (int DownloadIndex = 1; DownloadIndex <= 6; DownloadIndex++)
            {
                string TransparentIcon = "<img src='~/images/transparent16x16.gif'  style='width:15px;height:15px;border-style:none;vertical-align:middle;visibility:hidden' />";
                ProductCell.Text = ProductCell.Text.Replace("_DOWNLOAD" + DownloadIndex.ToString() + "_", TransparentIcon);
            }
            //end special download

            ProductRow.Cells.Add(ProductCell);

            //add description
            System.Web.UI.WebControls.TableCell Desc = new System.Web.UI.WebControls.TableCell();
            Desc.Text = ACL_Entity.ProjectSettings.QVDescription;
            Desc.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left;
            //Add a link for the users manual
            if (string.IsNullOrEmpty(ACL_Entity.ProjectSettings.UserManual) == false)
            {
                Desc.Text += @"<br><br>";
                Desc.Text += "<center> <a href='Reflector.ashx?key=_KEY_&nofile=true' target='_blank' title='Click to view' > <img src='images/document-icon.png' align='middle' border='0' hspace='5'  /> View User Guide </a> </center>";
                Desc.Text = Desc.Text.Replace("_KEY_", MillimanCommon.Utilities.ConvertStringToHex( System.IO.Path.Combine( ACL_Entity.ProjectSettings.AbsoluteProjectPath,  ACL_Entity.ProjectSettings.UserManual )));
            }
            ProductRow.Cells.Add(Desc);

            Products.Rows.Add(ProductRow);

            //add title
            System.Web.UI.WebControls.TableRow TitleRow = new System.Web.UI.WebControls.TableRow();
            System.Web.UI.WebControls.TableCell TitleCell = new System.Web.UI.WebControls.TableCell();
            TitleCell.Text = System.IO.Path.GetFileNameWithoutExtension(ACL_Entity.ProjectSettings.QVProject); //TODO = should be a project name
            TitleCell.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;
            TitleCell.Width = 200;
            TitleCell.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
            TitleCell.CssClass = "tableRow";
            TitleRow.Cells.Add(TitleCell);

            //add empty cell
            System.Web.UI.WebControls.TableCell EmptyCell = new System.Web.UI.WebControls.TableCell();
            EmptyCell.Text = "&nbsp;";
            EmptyCell.CssClass = "tableRow";
            TitleRow.Cells.Add(EmptyCell);

            Products.Rows.Add(TitleRow);
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

        private void LoadProducts()
        {
            //make sure we are still logged in, if not exit
            if (Membership.GetUser() == null)
            {
                Response.Redirect("userlogin.aspx");
                return;
            }
            string[] UserRoles = MillimanCommon.UserAccessList.GetRolesForUser();
            if (UserRoles != null)
            {
                MillimanCommon.UserAccessList ACL = new MillimanCommon.UserAccessList(Membership.GetUser().UserName, UserRoles, false);
                if (ACL.ACL.Count == 0)
                {
                    MembershipUser MUT = Membership.GetUser();
                }
                foreach (MillimanCommon.UserAccessList.UserAccess Access in ACL.ACL)
                {
                    LoadProduct(Access);
                }
            }
        }

        private void LoadAnnouncements()
        {
            MillimanDev2.Announcements.Announcements AllAnnouncements = MillimanDev2.Announcements.Announcements.Load();
            if (AllAnnouncements.CurrentAnnouncements.Count > 0)
            {
                FirstAndOnlyRow.Cells[1].Visible = true;
                string AnnoucementCellContents = @"<div style='border:solid; border-width:1px;width: 350px;border-color:#336666;vertical-align:top;'><div style='line-height:25px; height:25px;vertical-align:top;background-color:#fafafa'><center><b>Announcements</b></center></div><table style='border-color: #336666; width: 350px; background-color:#fafafa'  cellpadding='15' cellspacing='3' border='0'>";
                string AnnouncementCellEnd = @"</table></div>";
                StringBuilder SB = new StringBuilder(AnnoucementCellContents);
                foreach (MillimanDev2.Announcements.Announcement Anncouncement in AllAnnouncements.CurrentAnnouncements)
                {
                    SB.Append(Anncouncement.ToMessage());
                }
                SB.Append(AnnouncementCellEnd);
                
               FirstAndOnlyRow.Cells[1].Text = SB.ToString();
            }
            else
            {
                FirstAndOnlyRow.Cells[1].Visible = false;
            }
        }


        protected void btnLogout_Click(object sender, EventArgs e)
        {
        
        }

        protected void Groups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Groups.SelectedItem.Text.IndexOf(NoFilter) == 0)
            {
                LoadProducts();
            }
            else
            {
                string[] UserRoles = new string[] { Groups.SelectedItem.Text };
                MillimanCommon.UserAccessList ACL = new MillimanCommon.UserAccessList(Membership.GetUser().UserName, UserRoles, false);
                foreach (MillimanCommon.UserAccessList.UserAccess Access in ACL.ACL)
                {
                    LoadProduct(Access);
                }
            }
        }

        protected void RadMenu1_ItemClick(object sender, Telerik.Web.UI.RadMenuEventArgs e)
        {
            if (string.Compare(e.Item.Value.ToString(), "logout", true) == 0)
            {
                try
                {
                    System.Threading.Thread.Sleep(2000); //seleep 2 seconds, to give other handlers chance to complete
                    // Logout locally.
                    System.Web.Security.FormsAuthentication.SignOut();
                    Session.Abandon();
                    if (this.Session["milliman"] == null)
                    {
                        // Create a logout request.
                        LogoutRequest logoutRequest = new LogoutRequest();
                        logoutRequest.Issuer = new Issuer(Util.GetAbsoluteUrl(this, "~/"));
                        logoutRequest.NameId = new NameId(Context.User.Identity.Name);

                        // Send the logout request to the IdP over HTTP redirect.
                        string logoutUrl = SSOConfiguration.IdPLogoutIdProviderUrl;
                        X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.SPCertKey];
                        logoutRequest.Redirect(Response, logoutUrl, string.Empty, x509Certificate.PrivateKey, "Sha1");
                    }
                    else
                    {
                        Response.Redirect("UserLogin.aspx");
                    }
                }
                catch (Exception exception)
                {
                    Trace.Write("ServiceProvider", "Error on logout page", exception);
                }
            }
        }

      
    }
}