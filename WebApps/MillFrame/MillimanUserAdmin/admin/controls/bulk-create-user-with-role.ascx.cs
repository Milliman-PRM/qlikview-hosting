using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using System.Collections.Generic;

public partial class bulk_admin_controls_create_user_with_role : System.Web.UI.UserControl
{
    #region page_load - get roles and databind it to role list

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            
            //// Reference the SpecifyRolesStep WizardStep
            //WizardStep SpecifyRolesStep = RegisterUserWithRoles.FindControl("SpecifyRolesStep") as WizardStep;

            //// Reference the RoleList CheckBoxList
            //CheckBoxList RoleList = SpecifyRolesStep.FindControl("RoleList") as CheckBoxList;

            //// Bind the set of roles to RoleList
            //RoleList.DataSource = Roles.GetAllRoles();
            //RoleList.DataBind();

            //// find add focus to user name textbox
            //TextBox UserName = CreateUserWizardStep1.ContentTemplateContainer.FindControl("UserName") as TextBox;
            //UserName.Focus();

            //LoginType_SelectedIndexChanged(null, null);

            //List<string> T = new List<string>() { "one", "two", "three", "four", "00032FAI01_WINDOWX", "1234567890", "abcdef", "aaaa", "bbbb", "ccccccc", "eeeee", "ffff", "g", "h", "I", "j", "k" };
            Groups.DataSource = Roles.GetAllRoles();
            Groups.DataBind();

            //add an empty row
            List<UserInfo> UI = new List<UserInfo>();
            string TempPassword = PasswordGenerator.Generate("@");
            UI.Add(new UserInfo("", false, false));
            RadGrid1.DataSource = UI;
            RadGrid1.DataBind();

            List<int> DateValueSelectionValues = new List<int>() { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,60,90,120,150,180,210,240,270,300,330 };
            DateValue.DataSource = DateValueSelectionValues;
            DateValue.DataBind();
            DateValue.SelectedIndex = 0;
            //ViewState["RadGridDataSource"] = UI;
        }
    }

    #endregion

    #region add user to role

    protected void RegisterUserWithRoles_ActiveStepChanged(object sender, EventArgs e)
    {
        //// Have we JUST reached the Complete step?
        //if (RegisterUserWithRoles.ActiveStep.Title == "Complete")
        //{
        //    // Reference the SpecifyRolesStep WizardStep
        //    WizardStep SpecifyRolesStep = RegisterUserWithRoles.FindControl("SpecifyRolesStep") as WizardStep;

        //    // Reference the RoleList CheckBoxList
        //    CheckBoxList RoleList = SpecifyRolesStep.FindControl("RoleList") as CheckBoxList;

        //    // Add the checked roles to the just-added user
        //    foreach (ListItem li in RoleList.Items)
        //    {
        //        if (li.Selected)
        //        {
        //            Roles.AddUserToRole(RegisterUserWithRoles.UserName, li.Text);
        //        }
        //    }
        //}
       
    }

    #endregion

    #region do not show newly created user as online

    // this code has been depricated as the number of online users now work 
    // with the global.asax file. It won't hurt anything though.
    protected void RegisterUserWithRoles_CreatedUser(object sender, EventArgs e)
    {
        //RadioButtonList RBL = RegisterUserWithRoles.CreateUserStep.ContentTemplateContainer.FindControl("LoginType") as RadioButtonList;
        //CheckBox DBRequired = RegisterUserWithRoles.CreateUserStep.ContentTemplateContainer.FindControl("DBAccess") as CheckBox;
        //TextBox UserName = CreateUserWizardStep1.ContentTemplateContainer.FindControl("UserName") as TextBox;
        //TextBox Pswd = CreateUserWizardStep1.ContentTemplateContainer.FindControl("Password") as TextBox;

        //// do not show newly created user as Online
        //MembershipUser muser = Membership.GetUser(RegisterUserWithRoles.UserName);
        //muser.LastActivityDate = DateTime.Parse("1/1/1800");  //set user last activity to Jan 1, 1800
        
        //if (( RBL != null ) && (string.Compare(RBL.SelectedValue,MillimanCommon.Predefined.CovisintLoginType, true) == 0 ))
        //{
        //    muser.ChangePassword(Pswd.Text, MillimanCommon.Predefined.DefaultCovisintPassword);
        //}

        //Membership.UpdateUser(muser);

        ////create a user profile, this info can be regurtated from the DB using the httpcontext profile - make sure web.config is setup corretly :-)
        //if ((RBL != null) && (DBRequired != null) && (UserName != null) && (string.IsNullOrEmpty(UserName.Text) == false))
        //{
        //    ProfileCommon p = (ProfileCommon)ProfileCommon.Create(UserName.Text, true);
        //    p.AccessOptions.AccessType = RBL.SelectedValue;
        //    p.AccessOptions.DBRequired = DBRequired.Checked;
        //    p.AccessOptions.MustChangePassword = true;
        //    p.Save();
        //}

        //try
        //{
        //    //only right out a rest file for Milliman users - never for covisint
        //    if (string.Compare(RBL.SelectedValue, MillimanCommon.Predefined.MillimanLoginType, true) == 0)
        //    {
        //        string ResetFile = System.IO.Path.Combine(ConfigurationManager.AppSettings["ResetUserInfoRoot"], muser.ProviderUserKey + ".rst");
        //        System.IO.File.WriteAllText(ResetFile, muser.UserName + " added " + DateTime.Now.ToShortDateString());
        //    }
        //}
        //catch (Exception)
        //{
        //    muser.IsApproved = false;
        //    muser.Comment = "Account error on creation";
        //    Membership.UpdateUser(muser);
        //}
    }

    #endregion
    protected void LoginType_SelectedIndexChanged(object sender, EventArgs e)
    {
        //TextBox Pswd = CreateUserWizardStep1.ContentTemplateContainer.FindControl("Password") as TextBox;
        //TextBox ConfPswd = CreateUserWizardStep1.ContentTemplateContainer.FindControl("ConfirmPassword") as TextBox;
        //RadioButtonList RBL = RegisterUserWithRoles.CreateUserStep.ContentTemplateContainer.FindControl("LoginType") as RadioButtonList;

        //Pswd.Enabled = (RBL.SelectedIndex == 1);
        //ConfPswd.Enabled = (RBL.SelectedIndex == 1);
        //Pswd.BackColor =  Pswd.Enabled ? System.Drawing.Color.White : System.Drawing.Color.LightGray;
        //ConfPswd.BackColor = Pswd.BackColor;
        //Pswd.Text = Pswd.Enabled ? "" : "[NOT REQUIRED]";
        //ConfPswd.Text = Pswd.Text;
        //Pswd.TextMode = Pswd.Enabled ? TextBoxMode.Password : TextBoxMode.SingleLine;
        //ConfPswd.TextMode = Pswd.TextMode;
    }
    protected void RegisterUserWithRoles_CreatingUser(object sender, LoginCancelEventArgs e)
    {
        
    }

    private UserInfo GetUserInfoFromString(string Info)
    {
        try
        {
            UserInfo NewUser = new UserInfo();

            string[] UserTokens = Info.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            NewUser.Account_Name = UserTokens[0].Trim();
             
            if (UserTokens.Length > 1)
                NewUser.SendWelcomeEmail = System.Convert.ToBoolean(UserTokens[1]);
            return NewUser;
        }
        catch (Exception)
        {

        }
        return null;
    }

    protected void Submit_Click(object sender, EventArgs e)
    {
        List<UserInfo> UsersList = new List<UserInfo>();
        int Failed = 0;
        if (string.IsNullOrEmpty(UserList.Text) == false)
        {
            UserList.Text = UserList.Text.Replace("\t", ",");
            string[] UserRows = UserList.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string NewUserInfo in UserRows)
            {
                UserInfo NewUser = GetUserInfoFromString(NewUserInfo);
                if (NewUser != null)
                {
                    UsersList.Add(NewUser);
                }
                else
                {
                    Failed++;
                }
            }

            if (Failed == 0)
            {
                List<UserInfo> Current = GridToList(RadGrid1);
                if (Current != null)
                    UsersList.AddRange(Current);

                RadGrid1.DataSource = UsersList;

                RadGrid1.Rebind();
                UserPanel.Expanded = false;
                UserList.Text = "";

                EnableDisablePasswordColumns();
            }
            else
            {
                MillimanCommon.Alert.Show("Some items did not parse correctly");
            }
        }
        else
        {
            MillimanCommon.Alert.Show("No user information has been entered");
        }
    }

    protected void RadGrid1_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        string S = e.CommandName;
        List<UserInfo> UI = null;
        if (string.Compare(e.CommandName, "add", true) == 0)
        {
            UI = GridToList(RadGrid1);

            UI.Add(new UserInfo("",false,false));
            RadGrid1.DataSource = UI;
            RadGrid1.Rebind();
        }
        else if (string.Compare(e.CommandName, "Validate", true) == 0)
        {
            List<UserInfo> UIList = ValidateUserRequests();
            RadGrid1.DataSource = UIList;
            RadGrid1.Rebind();
           
        }
        else if (string.Compare(e.CommandName, "Clear", true) == 0)
        {
            UI = new List<UserInfo>();
            string TempPassword = PasswordGenerator.Generate("@");
            UI.Add(new UserInfo("", false, false));
            RadGrid1.DataSource = UI;
            RadGrid1.Rebind();

        }
        //else if (string.Compare(e.CommandName, "Create", true) == 0)
        //{
        //    List<UserInfo> UIList = ValidateUserRequests();
        //    bool AllGood = true;
        //    foreach (UserInfo UIC in UIList)
        //    {
        //        if (string.IsNullOrEmpty(UIC.ErrorMsg) == false)
        //            AllGood = false;
        //    }
        //    if (AllGood == false)
        //    {
        //        RadWindowManager.RadAlert("To create users all errors must be corrected in the user list.  Check list items tagged with a red icon.", null, null, "User List Errors", "confirmCallBackFn");
        //        return;
        //    }

        //    string Results = CreateUsersFromList(UIList);
        //    RadWindowManager.RadAlert(Results, null, null, "User List Errors", "confirmCallBackFn");
        //    RadGrid1.DataSource = UIList;
        //    RadGrid1.Rebind();
        //}
        else if (string.Compare(e.CommandName, "Autocomplete", true) == 0)
        {
            //RadWindowManager.RadConfirm("Are you sure you wish to <br><br><table><tr><td>cell 1</td><td>cell 2</td></tr><tr><td>cell 3</td><td>cell 4</td></tr></table>", "", 300, 100, null, "Summary", "");
            UI = AutoCompleteType();
            RadGrid1.DataSource = UI;
            RadGrid1.Rebind();
        }
    }

    private string CreateUsersFromList(List<UserInfo> UIList, out string CSVResults)
    {
        string ReturnMessage = UIList.Count.ToString() + " user account(s) were created successfully.\\n\\nCheck the 'CSV User List' for an easy cut/paste version of the updates.";
        CSVResults = string.Empty;
        try
        {
            //create a list of all roles
            List<string> RequestedRoles = new List<string>();
            foreach (ListItem LI in Groups.Items)
            {
                if (LI.Selected)
                    RequestedRoles.Add(LI.Text);
            }

            bool IsMillimanLogin = UserType.Items[0].Selected;
            bool WillAccountExpire = AccountExpires.Checked;
            DateTime? AccountExpiresOn = DateTime.MaxValue;
            if (WillAccountExpire)
                AccountExpiresOn = DatePicker.SelectedDate;
            
            foreach (UserInfo UI in UIList)
            {
                string Password = Guid.NewGuid().ToString();
                MembershipUser MU = Membership.CreateUser(UI.Account_Name, Password, UI.Account_Name);
                
                //create a csv list to pass back
                CSVResults += UI.Account_Name;
    
                CSVResults += "\n";

                MU.LastActivityDate = DateTime.MinValue;
                MU.LastLoginDate = DateTime.MinValue;
                
                if (MU != null)
                {
                    if (RequestedRoles.Count > 0)
                        Roles.AddUserToRoles(UI.Account_Name, RequestedRoles.ToArray());

                    //create a profile for the user
                    ProfileCommon p = (ProfileCommon)ProfileCommon.Create(UI.Account_Name, true);
                    p.AccessOptions.AccessType = IsMillimanLogin ? MillimanCommon.Predefined.MillimanLoginType : MillimanCommon.Predefined.CovisintLoginType;
                    p.AccessOptions.DBRequired = UI.DataAccess_Required;
                    p.AccessOptions.MustChangePassword = true;
                    p.Save();

                    try
                    {
                        //only right out a rest file for Milliman users - never for covisint
                        if (IsMillimanLogin)
                        {
                            string ResetFile = System.IO.Path.Combine(ConfigurationManager.AppSettings["ResetUserInfoRoot"], MU.ProviderUserKey + ".rst");
                            System.IO.File.WriteAllText(ResetFile, MU.UserName + " added " + DateTime.Now.ToShortDateString());
                        }
                    }
                    catch (Exception)
                    {
                        MU.IsApproved = false;
                        MU.Comment = "Account error on creation";
                        Membership.UpdateUser(MU);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ReturnMessage = "An issue occured when attempt to add users - no users were added.";
            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to add users", ex);
            //something went wrong back out
            foreach (UserInfo UI in UIList)
            {
                if (Membership.FindUsersByName(UI.Account_Name).Count > 0)
                    Membership.DeleteUser(UI.Account_Name);
            }
            CSVResults = string.Empty;
        }

        if (string.IsNullOrEmpty(CSVResults) == false)
        {  //accounts were created so send out welcome emails for those selected
            foreach (UserInfo UI in UIList)
            {
                if (UI.SendWelcomeEmail)
                {
                    MillimanCommon.MillimanEmail ME = new MillimanCommon.MillimanEmail();
                    string EmailTemplatesDir = Server.MapPath("~/email_templates");
                    string EmailBody = System.IO.File.ReadAllText(System.IO.Path.Combine(EmailTemplatesDir, "welcome.htm"));
                    string Body = MillimanCommon.MillimanEmail.EmailMacroProcessor(EmailBody, UI.Account_Name, Membership.GetUser(UI.Account_Name).ProviderUserKey.ToString(), Membership.GetUser().UserName);
                    ME.Send(UI.Account_Name, ConfigurationManager.AppSettings["SupportEmail"], Body, ConfigurationManager.AppSettings["SupportEmailWelcome"], true, false);
                }
            }
        }

        return ReturnMessage;
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

                (DBRequired.Checked == true ) )
            {
                UI.Add(new UserInfo(AccountName.Text,
                                     SendWelcome.Checked,
                                     DBRequired.Checked));
            }
        }
        return UI;
    }

    protected void UserType_SelectedIndexChanged(object sender, EventArgs e)
    {
        List<UserInfo> GridContents = GridToList(RadGrid1);
        //process grid here if needed for validation
        RadGrid1.DataSource = GridContents;
        RadGrid1.Rebind();

        EnableDisablePasswordColumns();
    }

    private void EnableDisablePasswordColumns()
    {

    }
    protected void AccountExpires_CheckedChanged(object sender, EventArgs e)
    {
            DateValue.Enabled = AccountExpires.Checked;
            DateType.Enabled = AccountExpires.Checked;
            DatePicker.Enabled = AccountExpires.Checked;

            if (AccountExpires.Checked == false)
            {
                DateValue.SelectedIndex = 0;
                DateType.SelectedIndex = 0;
                DatePicker.SelectedDate = null;
            }
 
    }
    protected void DateValue_SelectedIndexChanged(object sender, EventArgs e)
    {
        CalculateExpireDate();
    }
    protected void DateType_SelectedIndexChanged(object sender, EventArgs e)
    {
        CalculateExpireDate();
    }

    private void CalculateExpireDate()
    {
        DateTime Calculated = DateTime.Now;
        if (string.Compare(DateType.SelectedValue, "year", true) == 0)
           Calculated = Calculated.AddYears( System.Convert.ToInt32(DateValue.SelectedValue));
        else if (string.Compare(DateType.SelectedValue, "month", true) == 0)
            Calculated = Calculated.AddMonths(System.Convert.ToInt32(DateValue.SelectedValue));
        else if (string.Compare(DateType.SelectedValue, "week", true) == 0)
            Calculated = Calculated.AddDays(System.Convert.ToInt32(DateValue.SelectedValue) * 7);
        else if (string.Compare(DateType.SelectedValue, "day", true) == 0)
            Calculated = Calculated.AddDays(System.Convert.ToInt32(DateValue.SelectedValue));
        DatePicker.SelectedDate = Calculated;
    }

    private List<UserInfo> AutoCompleteType()
    {
        List<UserInfo> UIList = GridToList(RadGrid1);
  
        return UIList;
    }

    //private string AlphaNums = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private string AccountValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-@.";
    //private string EmailLocalSectionValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!#$%&'*+-/=?^_`{|}~.";
    //private string EmailServerValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-.";
    private List<UserInfo> ValidateUserRequests()
    {
        List<UserInfo> UIList = GridToList(RadGrid1);
        foreach (UserInfo UI in UIList)
        {
            UI.SetStatus(UserInfo.StatusType.SUCCESS);
            if (string.IsNullOrEmpty(UI.Account_Name) == true)
            {
                UI.ErrorMsg = "Account name cannot be left empty";
            }
            else if (string.IsNullOrEmpty(UI.Account_Name) == false)
            {
                foreach (char C in UI.Account_Name)
                {
                    if (AccountValidChars.Contains(C.ToString()) == false)
                    {
                        UI.ErrorMsg = "Account names may only contains alpha-numeric characters and the special characters '_-@.'";
                        break;
                    }
                }
                if (Membership.FindUsersByName(UI.Account_Name).Count != 0)
                {
                    UI.ErrorMsg = "A user with this account name already exists - duplicates may not be added.";
                }
            }
            
            //if (string.IsNullOrEmpty(UI.Email) == true)
            //{
            //    UI.ErrorMsg = "Email field cannot be left empty";
            //}
            if (string.IsNullOrEmpty(UI.Account_Name) == false)
            {
                if (UI.Account_Name.Contains(".") == false)
                {
                    UI.ErrorMsg = "Email address does not contain a '.' character.";
                }
                if (UI.Account_Name.Contains("@") == false)
                {
                    UI.ErrorMsg = "Email address does not contain a '@' character.";
                }
                bool isEmail = System.Text.RegularExpressions.Regex.IsMatch(UI.Account_Name.ToLower(), @"\A(?:[a-z0-9!#$%&amp;'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&amp;'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
                if (isEmail == false)
                {
                    UI.ErrorMsg = "Email address format does not match standards protocal RFC 2822 format - please report to system administrator";
                }
                else if (Membership.FindUsersByEmail(UI.Account_Name).Count != 0)
                {
                    UI.ErrorMsg = "A user with this address already exists - duplicates may not be added.";
                }
            }
            
            //if (UI.Password != UI.Confirm_Password)
            //{
            //    UI.ErrorMsg = "Password and confirm password do not match.";
            //}
            //else if (UI.Password.Length < 8)
            //{
            //    UI.ErrorMsg = "Passwords must have a length of at least 8.";
            //}
            //else
            //{
            //    bool ContainsOneSpecialChar = false;
            //    foreach (char C in UI.Password)
            //    {
            //        if (AlphaNums.Contains(C.ToString()) == false)
            //            ContainsOneSpecialChar = true;
            //    }
            //    if (ContainsOneSpecialChar == false)
            //    {
            //        UI.ErrorMsg = "Password must contain at least one special character";
            //    }
            //}
        }

        return UIList;
    }
    protected void CreateNewUsers_Click(object sender, ImageClickEventArgs e)
    {
        List<UserInfo> UIList = ValidateUserRequests();
        bool AllGood = true;
        foreach (UserInfo UIC in UIList)
        {
            if (string.IsNullOrEmpty(UIC.ErrorMsg) == false)
                AllGood = false;
        }
        if (AllGood == false)
        {
            MillimanCommon.Alert.Show( "To create users all errors must be corrected in the user list.  Check list items tagged with a red icon.");
            RadGrid1.DataSource = UIList;
            RadGrid1.Rebind();
            return;
        }

        string CSVUsers = string.Empty;
        string Results = CreateUsersFromList(UIList, out CSVUsers);
        UserList.Text = "[CSV Format]\n" + CSVUsers;
        UserList.Text += "\n\n[Excel Format]\n" + CSVUsers.Replace(",", "\t");

        MillimanCommon.Alert.Show(Results);

    }
}