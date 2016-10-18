using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.Security;
using Telerik.Web.UI;

public partial class admin_controls_admin_reports : System.Web.UI.UserControl
{
    protected override void OnLoad(EventArgs e)
    {
        if (!IsPostBack)
        {
            //init for setup
            ReportType_SelectedIndexChanged(null, null);
        }
        base.OnLoad(e);
        checkResults(0);
    }
    
    protected void Generate_Click(object sender, EventArgs e)
    {
        if (string.Compare(ReportType.SelectedValue, "UserQVWS", true) == 0)
        {
            GenerateUserToQVWs( UserSelections.Text );
        }
        else if (string.Compare(ReportType.SelectedValue, "UserGroup", true) == 0)
        {
            GenerateUserToQVWs(UserSelections.Text, true);
        }
        else if (string.Compare(ReportType.SelectedValue, "Group", true) == 0)
        {
            GroupCollection(UserSelections.Text);
        }
        else if (string.Compare(ReportType.SelectedValue, "QVWSUser", true) == 0)
        {
            QVWsToUser(UserSelections.Text);
        }
        else if (string.Compare(ReportType.SelectedValue, "QVWSGroup", true) == 0)
        {
            QVWsToUser(UserSelections.Text, true);
        }
    }

    protected void QVWsToUser(string QVW, bool HideUsers = false)
    {
        MillimanCommon.UserRepo UR = MillimanCommon.UserRepo.GetInstance();
        Report.Nodes.Clear();
        //add the user as the root
        RadTreeNode RootNode = new RadTreeNode(QVW);
        RootNode.ImageUrl = "~/images/qlikview16x16.png";
        Report.Nodes.Add(RootNode);

        List<string> UserList = new List<string>();
        foreach (MembershipUser MU in Membership.GetAllUsers())
        {
            foreach (string Role in Roles.GetRolesForUser(MU.UserName))
            {
                 List<System.Dynamic.ExpandoObject> QVWItems = UR.FindAllQVProjectsForUser(MU.UserName, new string[] { Role });
                 if (QVWItems != null)
                 {
                     foreach (System.Dynamic.ExpandoObject EO in QVWItems)
                     {
                         dynamic EOD = EO;
                         if (EOD.QVFilename.IndexOf(QVW) != -1)
                         {
                             string AdjustedItem = MU.UserName + " [" + EOD.Role + "]";
                             if (HideUsers)
                                 AdjustedItem = EOD.Role;
                             AdjustedItem = AdjustedItem.Replace("[[", "[");//[DIRECT] gets two set of brackets so replace
                             AdjustedItem = AdjustedItem.Replace("]]", "]");//[DIRECT] gets two set of brackets so replace 
                             if (UserList.Contains(AdjustedItem) == false)
                                 UserList.Add(AdjustedItem);
                         }
                     }
                 }
            }
        }

        foreach (string UL in UserList)
        {
            RadTreeNode RTN = new RadTreeNode(UL);
            RTN.ImageUrl = "~/images/user-identity-icon_16.png";
            if ( HideUsers )
                RTN.ImageUrl = "~/images/Buzz-Box-icon.png";
            RootNode.Nodes.Add(RTN);
        }
        Report.ExpandAllNodes();
        checkResults(Report.GetAllNodes().Count);
    }

    protected void GroupCollection(string GroupName)
    {
        MillimanCommon.UserRepo UR = MillimanCommon.UserRepo.GetInstance();
        Report.Nodes.Clear();
        //add the user as the root
        RadTreeNode RootNode = new RadTreeNode(GroupName);
        RootNode.ImageUrl = "~/images/Buzz-Box-icon.png";
        Report.Nodes.Add(RootNode);

        List<string> QVWs = new List<string>();
        foreach (MembershipUser MU in Membership.GetAllUsers())
        {
            //check to see if user is in Role
            if (Roles.IsUserInRole(MU.UserName, GroupName) == true)
            {  //user is in role, so get accessable files
                List<System.Dynamic.ExpandoObject> QVWItems = UR.FindAllQVProjectsForUser(MU.UserName, new string[] { GroupName });
                if (QVWItems != null)
                {
                    RadTreeNode UserNode = new RadTreeNode(MU.UserName);
                    UserNode.ImageUrl = "~/images/user-identity-icon_16.png";
                    RootNode.Nodes.Add(UserNode);
                    foreach (System.Dynamic.ExpandoObject EO in QVWItems)
                    {
                        dynamic EOD = EO;
                        string AdjustedPath = AdjustPathToDocumentRelative(EOD.QVFilename);
                        if (QVWs.Contains(AdjustedPath) == false)
                            QVWs.Add(AdjustedPath);
                    }
                }
            }
        }

        foreach (string QV in QVWs)
        {
            RadTreeNode ReportNode = new RadTreeNode(QV);
            ReportNode.ImageUrl = "~/images/qlikview16x16.png";
            RootNode.Nodes.Add(ReportNode);
        }
        Report.ExpandAllNodes();
        checkResults(Report.GetAllNodes().Count);
    }

    protected void GenerateUserToQVWs(string UserName, bool HideQVWs = false)
    {
        MillimanCommon.UserRepo UR = MillimanCommon.UserRepo.GetInstance();

        List<System.Dynamic.ExpandoObject> QVWItems =  UR.FindAllQVProjectsForUser(UserName, Roles.GetRolesForUser(UserName));
        Report.Nodes.Clear();
        //add the user as the root
        RadTreeNode RootNode = new RadTreeNode(UserName);
        RootNode.ImageUrl = "~/images/user-identity-icon_16.png";
        Report.Nodes.Add(RootNode);
        //add the groups to the root
        foreach (string RoleName in Roles.GetRolesForUser(UserName))
        {
            RadTreeNode RTN = new RadTreeNode(RoleName);
            RTN.ImageUrl = "~/images/Buzz-Box-icon.png";
            RootNode.Nodes.Add(RTN);
        }
        Report.ExpandAllNodes();
        //only show groups
        if (HideQVWs)
        {
            checkResults(0);
            return;
        }

        if (QVWItems != null)
        {
            RadTreeNode Current = null;
            foreach (System.Dynamic.ExpandoObject EO in QVWItems)
            {
                dynamic EOD = EO;
                //find the group node, should be at level 1
                Current = RootNode.Nodes.FindNodeByText(EOD.Role);
                if (Current == null)
                {    //didnt find that group name, create one and add it, but mark it since there is a group in the
                    //rules.xml that is out of sync with membership api
                    Current = new RadTreeNode( EOD.Role );
                    if (string.Compare(EOD.Role, "[direct]", true) != 0)
                        Current.Text = EOD.Role + "[rogue]";
                    Current.ImageUrl = "~/images/Buzz-Box-icon.png";
                    if (string.Compare(EOD.Role, "[direct]", true) == 0) //direct access
                        Current.ImageUrl = "~/images/Arrow-symbolic-link-icon16x16.png";
                    RootNode.Nodes.Add(Current);
                }
                //add the QVW to group
                RadTreeNode QVW = new RadTreeNode(AdjustPathToDocumentRelative(EOD.QVFilename));
                QVW.ImageUrl = "~/images/qlikview16x16.png";
                Current.Nodes.Add(QVW);
            }
        }
        Report.ExpandAllNodes();
        checkResults(Report.GetAllNodes().Count);
    }

    protected string AdjustPathToDocumentRelative(string Path)
    {
        string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
        if ((DocumentRoot.EndsWith(@"\") == false) && (DocumentRoot.EndsWith(@"/") == false))
            DocumentRoot += "/";

        return Path.Substring(DocumentRoot.Length);
    }

    public void checkResults(int? counter)
    {
        if (counter.HasValue && counter.Value>0)
        {
            divResults.Visible = true;
            lblTotalRecords.Text = Report.GetAllNodes().Count.ToString();
            
        }
        else
        {
            lblTotalRecords.Text = "0";
            divResults.Visible = false;
            updPanlRecords.Update();
        }
    }
    protected void ReportType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if ((string.Compare(ReportType.SelectedValue, "UserQVWS", true) == 0) || (string.Compare(ReportType.SelectedValue, "UserGroup", true) == 0))
        {
            SelectionLabel.InnerText = "Users: ";
            UserSelections.Items.Clear();
            foreach (MembershipUser MU in Membership.GetAllUsers())
            {
                UserSelections.Items.Add(new ListItem(MU.UserName));
            }
        }
        else if (string.Compare(ReportType.SelectedValue, "Group", true) == 0) 
        {
            SelectionLabel.InnerText = "Groups: ";
            UserSelections.Items.Clear();
            foreach (string Role in Roles.GetAllRoles())
            {
                UserSelections.Items.Add(new ListItem(Role));
            }
        }
        else if ((string.Compare(ReportType.SelectedValue, "QVWSUser", true) == 0) || (string.Compare(ReportType.SelectedValue, "QVWSGroup", true) == 0))
        {
            SelectionLabel.InnerText = "QVWs: ";
            UserSelections.Items.Clear();
            MillimanCommon.UserRepo UR = MillimanCommon.UserRepo.GetInstance();
            List<string> QVWs = UR.GetAllQVWs();
            foreach (string QV in QVWs)
            {
                UserSelections.Items.Add(new ListItem(QV));
            }
        }
    }

}