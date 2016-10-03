using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using System.Collections.Generic;
using Telerik.Web.UI;

public partial class bulk_admin_controls_create_user_with_role : System.Web.UI.UserControl
{
    #region page_load - get roles and databind it to role list

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //add an empty row
            List<UserInfo> UI = new List<UserInfo>();
            string TempPassword = PasswordGenerator.Generate("@");
            UI.Add(new UserInfo("", false, false));
            RadGrid1.DataSource = UI;
            RadGrid1.DataBind();
        }
    }

    #endregion

    protected void Submit_Click(object sender, EventArgs e)
    {
        List<UserInfo> UsersList = new List<UserInfo>();
        int Failed = 0;
        if (string.IsNullOrEmpty(UserList.Text) == false)
        {
            var userListFinal = string.Empty;
            if (UserList.Text.Contains("[CSV Format]") || UserList.Text.Contains("[Excel Format]"))
                userListFinal = UserList.Text.Replace("[CSV Format]", string.Empty).Replace("[Excel Format]", string.Empty);
            else
                userListFinal = UserList.Text;

            UserList.Text = userListFinal.Replace("\t", ",");
            var UserRows = UserList.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string NewUserInfo in UserRows)
            {
                var NewUser = GetUserInfoFromString(NewUserInfo);
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
                var Current = new List<UserInfo>();
                Current = GridToList(RadGrid1);
                if (Current != null)
                    UsersList.AddRange(Current);

                RadGrid1.DataSource = UsersList;
                RadGrid1.Rebind();

                UserPanel.Expanded = false;

                UserList.Text = "";
                updPanelUserList.Update();
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


    protected void CreateNewUsers_Click(object sender, ImageClickEventArgs e)
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
        string CSVUsers = string.Empty;
        string Results = CreateUsersFromList(UIList, out CSVUsers);
        UserList.Text = "[CSV Format]\n" + CSVUsers;
        UserList.Text += "\n\n[Excel Format]\n" + CSVUsers.Replace(",", "\t");

        MillimanCommon.Alert.Show(Results);
    }

    protected void UserType_SelectedIndexChanged(object sender, EventArgs e)
    {
        List<UserInfo> GridContents = GridToList(RadGrid1);
        //process grid here if needed for validation
        RadGrid1.DataSource = GridContents;
        RadGrid1.Rebind();
    }

    protected void RadGrid1_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        string S = e.CommandName;
        List<UserInfo> UI = null;
        if (string.Compare(e.CommandName, "add", true) == 0)
        {
            UI = GridToList(RadGrid1);
            UI.Add(new UserInfo("", false, false));
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
            ////add an empty row
            //var uInfoList = new List<UserInfo>();
            //uInfoList.Add(new UserInfo("", false, false));
            //RadGrid1.DataSource = uInfoList;
            //RadGrid1.Rebind();

            ////clear control
            //ctrlUserRoles.LoadUserRoles();
            //updPanelUserRoles.Update();

            //UserList.Text = string.Empty;
            //updPanelUserList.Update();

            InitilizeScreen();
        }
        else if (string.Compare(e.CommandName, "Autocomplete", true) == 0)
        {
            UI = AutoCompleteType();
            RadGrid1.DataSource = UI;
            RadGrid1.Rebind();
        }
    }

    private void InitilizeScreen()
    {
        //select the first option
        UserType.SelectedIndex = 0;
        updPanelUserType.Update();
        //clear control
        ctrlUserRoles.LoadUserRoles();
        updPanelUserRoles.Update();

        //Clear all grid items
        foreach (GridDataItem item in RadGrid1.Items)
        {
            var AccountNameText = (TextBox)item["AccountNameText"].FindControl("AccountNameTextBox");
            var SendWelcome = (CheckBox)item["SendWelcome"].FindControl("SendWelcomeCheckbox");
            var DataAccessRequiredText = (CheckBox)item["DataAccessRequiredText"].FindControl("DataAccessRequiredTextBox");
            AccountNameText.Text = "";
            SendWelcome.Checked = false;
            DataAccessRequiredText.Checked = false;
        }
        //reset grid
        //add an empty row
        var uInfoList = new List<UserInfo>();
        uInfoList.Add(new UserInfo("", false, false));
        RadGrid1.DataSource = uInfoList;
        RadGrid1.Rebind();

        //this will work if panel is added to ajaaax contol id for grid
        RadPanelItem items = new RadPanelItem();
        items = RadPanelBar1.FindItemByValue("UserPanel");
        items.Expanded = false;
        UserPanel.Expanded = false;
        
        UserList.Text = string.Empty;
        updPanelUserList.Update();
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

    private string CreateUsersFromList(List<UserInfo> UIList, out string CSVResults)
    {
        string ReturnMessage = UIList.Count.ToString() + " user account(s) were created successfully.\\n\\nCheck the 'CSV User List' for an easy cut/paste version of the updates.";
        CSVResults = string.Empty;
        try
        {
            var userRoles = ctrlUserRoles.CheckedRoles;
            bool IsMillimanLogin = UserType.Items[0].Selected;

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
                    if (userRoles.Count > 0)
                        Roles.AddUserToRoles(UI.Account_Name, userRoles.ToArray());

                    //create a profile for the user
                    ProfileCommon p = (ProfileCommon)ProfileCommon.Create(UI.Account_Name, true);
                    p.AccessOptions.AccessType = IsMillimanLogin ? MillimanCommon.Predefined.MillimanLoginType : MillimanCommon.Predefined.ExternalLoginType;
                    p.AccessOptions.DBRequired = UI.DataAccess_Required;
                    p.AccessOptions.MustChangePassword = true;
                    p.Save();

                    InitilizeScreen();

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
            if ((string.IsNullOrEmpty(AccountName.Text) == false) || (DBRequired.Checked == true))
            {
                UI.Add(new UserInfo(AccountName.Text,
                                     SendWelcome.Checked,
                                     DBRequired.Checked));
            }
        }
        return UI;
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
        }

        return UIList;
    }

}