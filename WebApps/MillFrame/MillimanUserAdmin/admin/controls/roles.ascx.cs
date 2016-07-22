using System;
using System.Data;
using System.Web.Security;
using System.Web.UI.WebControls;

public partial class admin_controls_roles : System.Web.UI.UserControl
{
    #region globals

    private bool createRoleSuccess = true;

    #endregion

    #region Bind ROLES to Gridview

    private void Page_PreRender()
    {
        //string TestRole = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";
        //if (Roles.RoleExists(TestRole) == false)
        //    Roles.CreateRole(TestRole);


        // Create a DataTable and define its columns
        DataTable RoleList = new DataTable();
        RoleList.Columns.Add("Role Name");
        RoleList.Columns.Add("User Count");
        RoleList.Columns.Add("External Name");
        RoleList.Columns.Add("Group Category");
        RoleList.Columns.Add("Friendly Name");
        RoleList.Columns.Add("Maximum Number Users");
        string[] allRoles = Roles.GetAllRoles();
        var MGM = MillimanCommon.MillimanGroupMap.GetInstance();

        // Get the list of roles in the system and how many users belong to each role
        foreach (string roleName in allRoles)
        {
            MillimanCommon.MillimanGroupMap.MillimanGroups MG = null;
            if (MGM.MillimanGroupDictionary.ContainsKey(roleName))
                MG = MGM.MillimanGroupDictionary[roleName];

            string FriendlyName = (MG == null) ? string.Empty : MG.FriendlyGroupName;
            string ExternalName = (MG == null) ? string.Empty : MG.ExernalGroupName;
            int MaxUsers = (MG == null) ? 0 : MG.MaximumnUsers;
            string GroupCategory = (MG == null) ? string.Empty : MG.GroupCategory;

            int numberOfUsersInRole = Roles.GetUsersInRole(roleName).Length;
            string[] roleRow = { roleName, numberOfUsersInRole.ToString(), ExternalName, GroupCategory, FriendlyName, MaxUsers.ToString() };
            RoleList.Rows.Add(roleRow);
        }

        // Bind the DataTable to the GridView
        UserRoles.DataSource = RoleList;
        UserRoles.DataBind();

        if (createRoleSuccess)
        {
            // Clears form field after a role was successfully added.
            NewRole.Text = "";
        }
    }

    #endregion

    #region CREATE new ROLE

    // create new role
    public void AddRole(object sender, EventArgs e)
    {
        try
        {
            Msg.ForeColor = System.Drawing.Color.Black;
            Roles.CreateRole(NewRole.Text);
            Msg.Text = "The new role was added.";
            Msg.Visible = true;
            createRoleSuccess = true;
        }
        catch (Exception ex)
        {
            Msg.Text = ex.Message;
            Msg.Visible = true;
            Msg.ForeColor = System.Drawing.Color.Red;
            createRoleSuccess = false;
        }
    }

    #endregion

    #region DELETE ROLE one by one

    // delete selected role
    public void DeleteRole(object sender, CommandEventArgs e)
    {
        try
        {
            Msg.ForeColor = System.Drawing.Color.Black;
            // delete role only if no user exists in it (by adding the boolean value at the end)
            Roles.DeleteRole(e.CommandArgument.ToString(), true);
            Msg.Text = "Role '" + e.CommandArgument.ToString() + "' was DELETED.";
            Msg.Visible = true;
        }
        catch (Exception ex)
        {
            Msg.ForeColor = System.Drawing.Color.Red;
            Msg.Text = "Oops! " + ex.Message;
            Msg.Visible = true;
        }
    }

    #endregion

    #region DELETE selected ROLES

    protected void btnDeleteSelected_Click(object sender, EventArgs e)
    {
        try
        {
            Msg.ForeColor = System.Drawing.Color.Black;
            foreach (GridViewRow row in UserRoles.Rows)
            {
                CheckBox cb = (CheckBox)row.FindControl("chkRows");
                Label lbl = (Label)row.FindControl("RoleName");
                if (cb != null && cb.Checked)
                {
                    string userRole = lbl.Text.ToString();
                    Roles.DeleteRole(userRole, true);

                    this.UserRoles.DataBind();

                    Msg.Text = "ROLE(S) were sucessfully <b>DELETED</b>!";
                    Msg.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            Msg.ForeColor = System.Drawing.Color.Red;
            Msg.Text = ex.Message;
            Msg.Visible = true;
        }
    }

    #endregion

    #region highlight selected rows

    protected void UserRoles_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //-----------------------------------------------------------------------------
        // highlight row on click - IE and FF
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onclick", "ChangeRowColor(this)");
            CheckBox cb = (CheckBox)e.Row.FindControl("chkRows");
            Label lbl = (Label)e.Row.FindControl("RoleName");
            if (string.Compare(lbl.Text, "Administrator", true) == 0)
            {
                cb.Enabled = false;
                cb.ToolTip = @"Administrator group cannot be deleted";
            }
        }
        //-----------------------------------------------------------------------------
    }

    #endregion

    protected void ApplyChanges_Click(object sender, EventArgs e)
    {
        var MGM = MillimanCommon.MillimanGroupMap.GetInstance();
        foreach (GridViewRow GVR in UserRoles.Rows)
        {
            Label MillimanGroup = GVR.FindControl("RoleName") as Label;
            TextBox FriendlyName = GVR.FindControl("FriendlyName") as TextBox;
            TextBox Max = GVR.FindControl("UserLimit") as TextBox;
            TextBox GroupCategory = GVR.FindControl("txtGroupCategory") as TextBox;

            if (MGM.MillimanGroupDictionary.ContainsKey(MillimanGroup.Text) == true)
            {
                var MG = MGM.MillimanGroupDictionary[MillimanGroup.Text];
                MG.MaximumnUsers = System.Convert.ToInt32(Max.Text);
                MG.FriendlyGroupName = FriendlyName.Text;
                MG.GroupCategory = GroupCategory.Text;
            }
            else  //groups created a number of ways, make sure we have a friendly name for each
            {
                var MGNew = new MillimanCommon.MillimanGroupMap.MillimanGroups();
                MGNew.MaximumnUsers = System.Convert.ToInt32(Max.Text);
                MGNew.FriendlyGroupName = FriendlyName.Text;
                MGNew.GroupCategory = GroupCategory.Text;
                MGM.MillimanGroupDictionary.Add(MillimanGroup.Text, MGNew);
            }
            MGM.Save();
        }
    }
}