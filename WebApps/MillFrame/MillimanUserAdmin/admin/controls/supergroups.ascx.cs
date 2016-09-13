using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.Security;
using Telerik.Web.UI;

public partial class admin_controls_supergroups : System.Web.UI.UserControl
{
    protected override void OnLoad(EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["ALLCLIENTADMINS"] = null;
            Session["ALLPUBLISHERADMINS"] = null;
            Session["ALLROLES"] = null;

            ActiveInterface(false);

            MillimanCommon.SuperGroup SG = MillimanCommon.SuperGroup.GetInstance();
            foreach (MillimanCommon.SuperGroup.SuperGroupContainer SGC in SG.SuperGroupContainers)
            {
                SuperGroups.Items.Add(SGC.ContainerName);
            }
        }
        base.OnLoad(e);
    }

    private void ActiveInterface( bool IsActive )
    {
        Description.Enabled = IsActive;
        UseCommaDelimited.Enabled = IsActive;
        SmartLinkOn.Enabled = IsActive;
        RemoveGroup.Enabled = IsActive;
        AddToSuperGroup.Enabled = IsActive;
        RemoveAdmin.Enabled = IsActive;
        AddAdmin.Enabled = IsActive;
        RemovePublisher.Enabled = IsActive;
        AddPublisher.Enabled = IsActive;
        AllGroups.Enabled = IsActive;
        AllAdmins.Enabled = IsActive;
        AllPublishers.Enabled = IsActive;
        if ( IsActive == false )
        {
            GroupsInSuper.Items.Clear();
            PublishingUsers.Items.Clear();
            ClientAdminUsers.Items.Clear();
            AllPublishers.Items.Clear();
            AllAdmins.Items.Clear();
            Description.Text = "";
        }
    }


    protected void SuperGroups_SelectedIndexChanged(object sender, EventArgs e)
    {
        string Selected = SuperGroups.SelectedValue;
        MillimanCommon.SuperGroup.SuperGroupContainer SelectedSGC = null;
        MillimanCommon.SuperGroup SG = MillimanCommon.SuperGroup.GetInstance();
        foreach (MillimanCommon.SuperGroup.SuperGroupContainer SGC in SG.SuperGroupContainers)
        {
            if (string.Compare(SGC.ContainerName, Selected) == 0)
            {
                SelectedSGC = SGC;
                break;
            }
        }
        if (SelectedSGC != null)
        {
            ActiveInterface(true);

            SelectedSuperGroup.Text = SelectedSGC.ContainerName ;

            Description.Text = SelectedSGC.ContainerDescription;

            ClientAdminUsers.DataSource = SelectedSGC.AdminUserAccounts;
            ClientAdminUsers.DataBind();

            PublishingUsers.DataSource = SelectedSGC.PublisherUserAccounts;
            PublishingUsers.DataBind();

            GroupsInSuper.DataSource = SelectedSGC.GroupNames;
            GroupsInSuper.DataBind();

            UseCommaDelimited.Checked = SelectedSGC.SemiColonDelimitedEmail;
            SmartLinkOn.Checked = !SelectedSGC.AllowTempPasswordEntry;

            AddGroups(SelectedSGC.GroupNames);

            List<string> ClientAdmins = null;
            List<string> PublisherAdmins = null;
            if ( GetValidAdminsForGroups( SelectedSGC.GroupNames, out ClientAdmins, out PublisherAdmins))
            {
                MutuallyExclusiveLists(SelectedSGC.AdminUserAccounts, ref ClientAdmins);  //remove redundanate selections
                MutuallyExclusiveLists(SelectedSGC.PublisherUserAccounts, ref PublisherAdmins);
                AllAdmins.DataSource = ClientAdmins;
                AllAdmins.DataBind();
                AllPublishers.DataSource = PublisherAdmins;
                AllPublishers.DataBind();
            }
        }
    }

    /// <summary>
    /// Add all the groups to the list, unless they have already
    /// been selected as part of supergroup
    /// </summary>
    /// <param name="GroupsAlreadyAdded"></param>
    /// <returns></returns>
    private bool AddGroups(List<string> GroupsAlreadyAdded)
    {
        AllGroups.Items.Clear();
        MillimanCommon.UserRepo Repo = MillimanCommon.UserRepo.GetInstance();
        string[] AllRoles = Roles.GetAllRoles();

        foreach (string Role in AllRoles)
        {
            if ( (string.Compare(Role, "administrator", true)) != 0 && (GroupsAlreadyAdded.Contains(Role) == false) )
            {
                //the role can only have 1 QVW in it, or we will have issues in publisher and client admin
                if (Repo.FindAllQVProjectsForRole(Role).Count == 1)
                {
                    AllGroups.Items.Add(Role);
                }
            }
        }
        return true;
    }

    /// <summary>
    /// get all the accounts marked for client admin/publishing admins, with groups that qualify
    /// </summary>
    /// <param name="GroupNames"></param>
    /// <param name="ClientAdmins"></param>
    /// <param name="PublisherAdmins"></param>
    /// <returns></returns>
    private bool GetValidAdminsForGroups(List<string> GroupNames, out List<string> ClientAdmins, out List<string> PublisherAdmins )
    {
        if ( GroupNames.Count == 0 )
        {   //no groups, just give back empty lists
            ClientAdmins = new List<string>();
            PublisherAdmins = new List<string>();
            return true;
        }
        if ( GetAllClientUserAndPublisherAdmins(out ClientAdmins, out PublisherAdmins ))
        {
            foreach( string Group in GroupNames )
            {
                for( int Index = ClientAdmins.Count-1; Index >= 0; Index--)
                {
                    //admins are always good :-)
                    if (Roles.IsUserInRole(ClientAdmins[Index], "administrator") == false )
                    {
                        if (Roles.IsUserInRole(ClientAdmins[Index], Group) == false)
                            ClientAdmins.RemoveAt(Index);
                    }
                }
                for (int Index = PublisherAdmins.Count - 1; Index >= 0; Index--)
                {
                    //admins are always good :-)
                    if (Roles.IsUserInRole(PublisherAdmins[Index], "administrator") == false)
                    {
                        if (Roles.IsUserInRole(PublisherAdmins[Index], Group) == false)
                            PublisherAdmins.RemoveAt(Index);
                    }
                }
            }
            return true;
        }

        return false ;
    }


    /// <summary>
    /// Look though all the users and get a list of all client admins and publishers
    /// This can be optimized to be much faster.....
    /// </summary>
    /// <param name="ClientAdmins"></param>
    /// <param name="PublisherAdmins"></param>
    /// <returns></returns>
    private bool GetAllClientUserAndPublisherAdmins(out List<string> ClientAdmins, out List<string> PublisherAdmins)
    {
        if ((Session["ALLCLIENTADMINS"] != null) && (Session["ALLPUBLISHERADMINS"] != null))
        {
            ClientAdmins = Session["ALLCLIENTADMINS"] as List<string>;
            PublisherAdmins = Session["ALLPUBLISHERADMINS"] as List<string>;
            return true;
        }

        bool ClientResults = GetAdmins(AdminType.User, out ClientAdmins);
        bool PublisherResults = GetAdmins(AdminType.Publisher, out PublisherAdmins);

        if (( ClientResults == false ) || (PublisherResults == false))
        {
            ClientAdmins = new List<string>();
            PublisherAdmins = new List<string>();
            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to retrieve publisher and client admins from DB");
        }

        //to save on processing time put these in session to cache for now
        Session["ALLCLIENTADMINS"] = ClientAdmins;
        Session["ALLPUBLISHERADMINS"] = PublisherAdmins;

        return true;
    }
    /// <summary>
    /// Apply the updates the user has selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Update_Click(object sender, EventArgs e)
    {
        //save all changes for group
        if ( string.IsNullOrEmpty(SuperGroups.SelectedValue))
            return;

        MillimanCommon.SuperGroup SG = MillimanCommon.SuperGroup.GetInstance();
        MillimanCommon.SuperGroup.SuperGroupContainer SGC = SG.FindSuper(SuperGroups.SelectedValue);
        if (SGC == null)
        {
            SGC = new MillimanCommon.SuperGroup.SuperGroupContainer();
            SG.SuperGroupContainers.Add(SGC);
        }
        SGC.ContainerName = SuperGroups.SelectedValue;
        SGC.ContainerDescription = Description.Text;
        SGC.AllowTempPasswordEntry = !SmartLinkOn.Checked;
        SGC.SemiColonDelimitedEmail = !UseCommaDelimited.Checked;
        SGC.AdminUserAccounts = ListItemsToStringList(ClientAdminUsers);
        SGC.PublisherUserAccounts = ListItemsToStringList(PublishingUsers);
        SGC.GroupNames = ListItemsToStringList(GroupsInSuper);

        string Msg = "Super groups have been updated successfully";
        if (SG.Save() == false)
            Msg = "Super groups failed to updated. Please check with a system administrator.";
        MillimanCommon.Alert.Show(Msg);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void AddNewSuperGroup_Click(object sender, EventArgs e)
    {
        string SGName = NewSuperGroupName.Text;
        if ( string.IsNullOrEmpty(SGName))
        {
            MillimanCommon.Alert.Show("A super group name was not entered");
            NewSuperGroupName.Focus();
            return;
        }
        MillimanCommon.SuperGroup SG = MillimanCommon.SuperGroup.GetInstance();
        bool Found = false;
        foreach (ListItem SuperName in SuperGroups.Items)
        {
            if (string.Compare(SuperName.Value, SGName, true) == 0)
            {
                Found = true;
            }
        }
        if ( Found )
        {
            MillimanCommon.Alert.Show("'" + SGName + "' is already in use. Please choose another name.");
            NewSuperGroupName.Text = "";
        }
        else
        {
            SuperGroups.Items.Add(SGName);
            SuperGroups.SelectedIndex = SuperGroups.Items.IndexOf(new ListItem(SGName));
            SelectedSuperGroup.Text = "<center>" + SGName + "</center>";
            NewSuperGroupName.Text = "";
            Description.Text = "";
            SmartLinkOn.Checked = true;
            UseCommaDelimited.Checked = false;
            GroupsInSuper.Items.Clear();
            ClientAdminUsers.Items.Clear();
            PublishingUsers.Items.Clear();
            AllAdmins.Items.Clear();
            AllPublishers.Items.Clear();
            AddGroups(new List<string>());
            ActiveInterface(true);
        
        }
    }
    protected void DeleteSuperGroup_Click(object sender, EventArgs e)
    {
        if (SuperGroups.SelectedIndex >= 0)
        {
            if (string.IsNullOrEmpty(SuperGroups.SelectedValue))
                return;

            SuperGroups.Items.RemoveAt(SuperGroups.SelectedIndex);

            string DeleteGroup = SuperGroups.SelectedValue;
            MillimanCommon.SuperGroup SG = MillimanCommon.SuperGroup.GetInstance();

            MillimanCommon.SuperGroup.SuperGroupContainer SGC = SG.FindSuper(DeleteGroup);
            if (SGC != null) //tis not an error if null, may be a new item not saved in list yet
                SG.DeleteSuper(DeleteGroup);

            ActiveInterface(false);
        }
    }

    /// <summary>
    /// Remove a group from the super group, re-add to all groups
    /// and update candidate groups for admin and publishing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RemoveGroup_Click(object sender, EventArgs e)
    {
        if (GroupsInSuper.SelectedIndex >= 0)
        {
            string SelectedGroup = GroupsInSuper.SelectedValue;
            if (string.IsNullOrEmpty(SelectedGroup))
                return;  //do nothing, nothing is selected

            GroupsInSuper.Items.RemoveAt(GroupsInSuper.Items.IndexOf(new ListItem(SelectedGroup)));
            AllGroups.Items.Add(SelectedGroup);

            List<string> ListItems = new List<string>();
            foreach (ListItem LI in GroupsInSuper.Items)
                ListItems.Add(LI.Value);

            List<string> ClientAdmins = null;
            List<string> PublisherAdmins = null;
            if (GetValidAdminsForGroups(ListItems, out ClientAdmins, out PublisherAdmins))
            {
                AllAdmins.DataSource = ClientAdmins;
                AllAdmins.DataBind();
                AllPublishers.DataSource = PublisherAdmins;
                AllPublishers.DataBind();
            }
        }
    }
    protected void RemoveAdmin_Click(object sender, EventArgs e)
    {
        if (ClientAdminUsers.SelectedIndex >= 0)
        {
            AllAdmins.Items.Add(ClientAdminUsers.SelectedValue);
            ClientAdminUsers.Items.RemoveAt(ClientAdminUsers.SelectedIndex);
        }
    }
    protected void RemovePublisher_Click(object sender, EventArgs e)
    {
        if (PublishingUsers.SelectedIndex >= 0)
        {
            AllPublishers.Items.Add(PublishingUsers.SelectedValue);
            PublishingUsers.Items.RemoveAt(PublishingUsers.SelectedIndex);
        }
    }
    protected void AddToSuperGroup_Click(object sender, EventArgs e)
    {
        if (AllGroups.SelectedIndex >= 0)
        {
            string SelectedGroup = AllGroups.SelectedValue;
            if (string.IsNullOrEmpty(SelectedGroup))
                return;  //do nothing, nothing is selected
            AllGroups.Items.RemoveAt(AllGroups.SelectedIndex); //remove entry from AllGroups

            GroupsInSuper.Items.Add(SelectedGroup);
            List<string> ListItems = new List<string>();
            foreach (ListItem LI in GroupsInSuper.Items)
                ListItems.Add(LI.Value);

            List<string> ClientAdmins = null;
            List<string> PublisherAdmins = null;
            if (GetValidAdminsForGroups(ListItems, out ClientAdmins, out PublisherAdmins))
            {
                AllAdmins.DataSource = ClientAdmins;
                AllAdmins.DataBind();
                AllPublishers.DataSource = PublisherAdmins;
                AllPublishers.DataBind();
            }
        }
    }
    protected void AddAdmin_Click(object sender, EventArgs e)
    {
        if (AllAdmins.SelectedIndex >= 0)
        {
            string Admin = AllAdmins.SelectedValue;
            if (ClientAdminUsers.Items.IndexOf(new ListItem(Admin)) == -1)
            {
                ClientAdminUsers.Items.Add(Admin);
                AllAdmins.Items.RemoveAt(AllAdmins.SelectedIndex);  //remove from all list
            }
        }
    }
    protected void AddPublisher_Click(object sender, EventArgs e)
    {
        if (AllPublishers.SelectedIndex >= 0)
        {
            string Publisher = AllPublishers.SelectedValue;
            if (PublishingUsers.Items.IndexOf(new ListItem(Publisher)) == -1)
            {
                PublishingUsers.Items.Add(Publisher);
                AllPublishers.Items.RemoveAt(AllPublishers.SelectedIndex);  //remove from all pubs list
            }
        }
    }

    private List<string> ListItemsToStringList( ListBox LB)
    {
        if (LB == null)
            return null;
        List<string> Contents = new List<string>();
        foreach (ListItem LI in LB.Items)
            Contents.Add(LI.Value);

        return Contents;
    }

    enum AdminType { User, Publisher }
    private bool GetAdmins( AdminType Admin,out List<string> AdminAccounts)
    {
        AdminAccounts = new List<string>();
        //select aspnet_users.UserName from aspnet_users, aspnet_CustomProfile where (aspnet_users.UserId = aspnet_CustomProfile.UserId) AND (aspnet_CustomProfile.IsPublishingAdministrator = 'true')
        try
        {
            string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["dbMyCMSConnectionString"].ConnectionString;
            System.Data.SqlClient.SqlCommand comm = new System.Data.SqlClient.SqlCommand();
            comm.Connection = new System.Data.SqlClient.SqlConnection(ConnectionString);
            String sql = @"select aspnet_users.UserName from aspnet_users, aspnet_CustomProfile where (aspnet_users.UserId = aspnet_CustomProfile.UserId) AND (aspnet_CustomProfile.IsClientAdministrator = 'true')";
            if (Admin == AdminType.Publisher)
                sql = @"select aspnet_users.UserName from aspnet_users, aspnet_CustomProfile where (aspnet_users.UserId = aspnet_CustomProfile.UserId) AND (aspnet_CustomProfile.IsPublishingAdministrator = 'true')";
            comm.CommandText = sql;
            comm.Connection.Open();
            System.Data.SqlClient.SqlDataReader cursor = comm.ExecuteReader();
            while (cursor.Read())
            {
                string Value = cursor["UserName"].ToString();

                if (string.IsNullOrEmpty(Value) == false)
                    AdminAccounts.Add(Value);
            }
            comm.Connection.Close();
            return true;
        }

        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        return false;
    }

    /// <summary>
    /// ensure items in 'IfInThisList' do not exit in other list
    /// </summary>
    /// <param name="IfInThisList"></param>
    /// <param name="RemoveFromThisList"></param>
    private void MutuallyExclusiveLists( List<string> IfInThisList, ref List<string> RemoveFromThisList)
    {
        if ((IfInThisList != null) && (RemoveFromThisList != null))
        {
            foreach (string S in IfInThisList)
            {
                if (RemoveFromThisList.Contains(S))
                    RemoveFromThisList.RemoveAt(RemoveFromThisList.IndexOf(S));
            }
        }
    }

    protected void Sorted_PreRender(object sender, EventArgs e)
    {
        ListBox LB = sender as ListBox;
        if (LB != null)
        {
            System.Collections.SortedList sorted = new System.Collections.SortedList();
            string Selected = LB.SelectedValue;

            foreach (ListItem ll in LB.Items)
            {
                sorted.Add(ll.Text, ll.Value);
            }

            LB.Items.Clear();

            foreach (String key in sorted.Keys)
            {
                LB.Items.Add(new ListItem(key, sorted[key].ToString()));
            }
            if (string.IsNullOrEmpty(Selected) == false)
                LB.SelectedValue = Selected;
        }
    }
}