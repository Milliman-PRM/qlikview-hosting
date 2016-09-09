using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Web.Configuration;

public partial class admin_controls_create_user_with_role : System.Web.UI.UserControl
{
    #region page_load - get roles and databind it to role list

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            // Reference the SpecifyRolesStep WizardStep
            WizardStep SpecifyRolesStep = RegisterUserWithRoles.FindControl("SpecifyRolesStep") as WizardStep;

            // Reference the RoleList CheckBoxList
            CheckBoxList RoleList = SpecifyRolesStep.FindControl("RoleList") as CheckBoxList;

            // Bind the set of roles to RoleList
            RoleList.DataSource = Roles.GetAllRoles();
            RoleList.DataBind();

            // find add focus to user name textbox
            TextBox UserName = CreateUserWizardStep1.ContentTemplateContainer.FindControl("UserName") as TextBox;
            UserName.Focus();

            LoginType_SelectedIndexChanged(null, null);
        }
    }

    #endregion

    #region add user to role

    protected void RegisterUserWithRoles_ActiveStepChanged(object sender, EventArgs e)
    {
        // Have we JUST reached the Complete step?
        if (RegisterUserWithRoles.ActiveStep.Title == "Complete")
        {
            // Reference the SpecifyRolesStep WizardStep
            WizardStep SpecifyRolesStep = RegisterUserWithRoles.FindControl("SpecifyRolesStep") as WizardStep;

            // Reference the RoleList CheckBoxList
            CheckBoxList RoleList = SpecifyRolesStep.FindControl("RoleList") as CheckBoxList;

            // Add the checked roles to the just-added user
            foreach (ListItem li in RoleList.Items)
            {
                if (li.Selected)
                {
                    Roles.AddUserToRole(RegisterUserWithRoles.UserName, li.Text);
                }
            }
        }
       
    }

    #endregion

    #region do not show newly created user as online

    // this code has been depricated as the number of online users now work 
    // with the global.asax file. It won't hurt anything though.
    protected void RegisterUserWithRoles_CreatedUser(object sender, EventArgs e)
    {
        RadioButtonList RBL = RegisterUserWithRoles.CreateUserStep.ContentTemplateContainer.FindControl("LoginType") as RadioButtonList;
        CheckBox DBRequired = RegisterUserWithRoles.CreateUserStep.ContentTemplateContainer.FindControl("DBAccess") as CheckBox;
        TextBox UserName = CreateUserWizardStep1.ContentTemplateContainer.FindControl("UserName") as TextBox;
        TextBox Pswd = CreateUserWizardStep1.ContentTemplateContainer.FindControl("Password") as TextBox;

        // do not show newly created user as Online
        MembershipUser muser = Membership.GetUser(RegisterUserWithRoles.UserName);
        muser.LastActivityDate = DateTime.Parse("1/1/1800");  //set user last activity to Jan 1, 1800
        
        if (( RBL != null ) && (string.Compare(RBL.SelectedValue,MillimanCommon.Predefined.ExternalLoginType, true) == 0 ))
        {
            muser.ChangePassword(Pswd.Text, MillimanCommon.Predefined.DefaultExternalPassword);
        }

        Membership.UpdateUser(muser);

        //create a user profile, this info can be regurtated from the DB using the httpcontext profile - make sure web.config is setup corretly :-)
        if ((RBL != null) && (DBRequired != null) && (UserName != null) && (string.IsNullOrEmpty(UserName.Text) == false))
        {
            ProfileCommon p = (ProfileCommon)ProfileCommon.Create(UserName.Text, true);
            p.AccessOptions.AccessType = RBL.SelectedValue;
            p.AccessOptions.DBRequired = DBRequired.Checked;
            p.AccessOptions.MustChangePassword = true;
            p.Save();
        }

        try
        {
            //only right out a rest file for Milliman users - never for covisint
            if (string.Compare(RBL.SelectedValue, MillimanCommon.Predefined.MillimanLoginType, true) == 0)
            {
                string ResetFile = System.IO.Path.Combine(ConfigurationManager.AppSettings["ResetUserInfoRoot"], muser.ProviderUserKey + ".rst");
                System.IO.File.WriteAllText(ResetFile, muser.UserName + " added " + DateTime.Now.ToShortDateString());
            }
        }
        catch (Exception)
        {
            muser.IsApproved = false;
            muser.Comment = "Account error on creation";
            Membership.UpdateUser(muser);
        }
    }

    #endregion
    protected void LoginType_SelectedIndexChanged(object sender, EventArgs e)
    {
        TextBox Pswd = CreateUserWizardStep1.ContentTemplateContainer.FindControl("Password") as TextBox;
        TextBox ConfPswd = CreateUserWizardStep1.ContentTemplateContainer.FindControl("ConfirmPassword") as TextBox;
        RadioButtonList RBL = RegisterUserWithRoles.CreateUserStep.ContentTemplateContainer.FindControl("LoginType") as RadioButtonList;

        Pswd.Enabled = (RBL.SelectedIndex == 1);
        ConfPswd.Enabled = (RBL.SelectedIndex == 1);
        Pswd.BackColor =  Pswd.Enabled ? System.Drawing.Color.White : System.Drawing.Color.LightGray;
        ConfPswd.BackColor = Pswd.BackColor;
        Pswd.Text = Pswd.Enabled ? "" : "[NOT REQUIRED]";
        ConfPswd.Text = Pswd.Text;
        Pswd.TextMode = Pswd.Enabled ? TextBoxMode.Password : TextBoxMode.SingleLine;
        ConfPswd.TextMode = Pswd.TextMode;
    }
    protected void RegisterUserWithRoles_CreatingUser(object sender, LoginCancelEventArgs e)
    {
        
    }
}