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

                int TotalLicense = System.Convert.ToInt32( Session["totalusers"] );
                int TotalReports = System.Convert.ToInt32(Session["totalgroups"]);
                string LabelTemplate = "Currently using " + TotalLicense.ToString() + " licenses for " + TotalReports.ToString() + " reports";
                License.Text = LabelTemplate;

                List<UserInfo> UL = new List<UserInfo>();
                for (int Index = 0; Index < 10; Index++)
                {
                    UL.Add(new UserInfo());
                }
                RadGrid1.DataSource = UL;
                RadGrid1.DataBind();

                MillimanCommon.SuperGroup.SuperGroupContainer SGC = Session["supergroup"] as MillimanCommon.SuperGroup.SuperGroupContainer;

                foreach (string Group in SGC.GroupNames)
                    ReportList.Items.Add(Group);

                if (SGC.AllowTempPasswordEntry)
                {
                    RadGrid1.MasterTableView.GetColumn("AccountNameText").HeaderStyle.Width = new Unit(45.0, UnitType.Percentage);
                    RadGrid1.MasterTableView.GetColumn("PasswordText").HeaderStyle.Width = new Unit(45.0, UnitType.Percentage);
                    RadGrid1.MasterTableView.GetColumn("PasswordText").Visible = true;
                    RadGrid1.MasterTableView.GetColumn("SendWelcome").Visible = false;

                    Msg.Text = "'" + System.Web.Security.Membership.GetUser().UserName + "' will recieve an email with the contents of this window.";
                }
                RadGrid1.MasterTableView.GetColumn("DataAccessRequiredText").Visible = false;
            }
        }


        protected void RadGrid1_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (string.Compare(e.CommandName, "add", true) == 0)
            {
                List<UserInfo> UL = ParseEmails(GridToList(RadGrid1));
                if (UL == null)
                    UL = new List<UserInfo>();

                //limit grid to always 10 for now
                while (UL.Count > 10)
                    UL.RemoveAt(UL.Count - 1);
                while (UL.Count < 10)
                    UL.Add(new UserInfo());

                RadGrid1.DataSource = UL;
                RadGrid1.DataBind();

            }
            else if (string.Compare(e.CommandName, "validate", true) == 0)
            {
                List<UserInfo> UL = ValidateUserRequests();
                
                //limit grid to always 10 for now
                while (UL.Count > 10)
                    UL.RemoveAt(UL.Count - 1);
                while (UL.Count < 10)
                    UL.Add(new UserInfo());

                if (UL != null)
                {
                    RadGrid1.DataSource = UL;
                    RadGrid1.DataBind();
                }
            }
            else if (string.Compare(e.CommandName, "clear", true) == 0)
            {
                List<UserInfo> UL = new List<UserInfo>();

                //limit grid to always 10 for now
                while (UL.Count > 10)
                    UL.RemoveAt(UL.Count - 1);
                while (UL.Count < 10)
                    UL.Add(new UserInfo());

                if (UL != null)
                {
                    UL.Add(new UserInfo());
                    RadGrid1.DataSource = UL;
                    RadGrid1.DataBind();
                }
            }
        }
        // Page.ClientScript.RegisterClientScriptBlock(GetType(), "CloseScript", "CloseDialog()", true);

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

       // private string AlphaNums = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
       private string AccountValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-@.";
       // private string EmailLocalSectionValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!#$%&'*+-/=?^_`{|}~.";
       // private string EmailServerValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-.";
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
                    foreach (char C in UI.Account_Name_No_Password)
                    {
                        if (AccountValidChars.Contains(C.ToString()) == false)
                        {
                            UI.ErrorMsg = "Account names may only contain alpha-numeric characters and the special characters '_-@.'";
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(UI.Account_Name_No_Password) == false)
                {
                    if (UI.Account_Name_No_Password.Contains(".") == false)
                    {
                        UI.ErrorMsg = "Email address does not contain a '.' character.";
                    }
                    if (UI.Account_Name_No_Password.Contains("@") == false)
                    {
                        UI.ErrorMsg = "Email address does not contain a '@' character.";
                    }
                    bool isEmail = System.Text.RegularExpressions.Regex.IsMatch(UI.Account_Name_No_Password.ToLower(), @"\A(?:[a-z0-9!#$%&amp;'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&amp;'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
                    if (isEmail == false)
                    {
                        UI.ErrorMsg = "Email address format does not match standards protocal RFC 2822 format - please report to system administrator";
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
                string[] Tokens = User.Account_Name.Split( Delimiters, StringSplitOptions.RemoveEmptyEntries);
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

        private void RollBack(List<string> RemoveUsersFromRole, List<string> DeleteUsers, List<string> RemoveResetFiles, List<Tuple<string,string>> UserGroupRollback)
        {
            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed adding users");
            //get rid of role for all users
            //if a new user, delete account, cannot exist in any other groups
            foreach (string NewUser in DeleteUsers)
            {
                System.Web.Security.Membership.DeleteUser(NewUser);
            }
            foreach( Tuple<string,string> UserGroupMap in UserGroupRollback)
            {
                System.Web.Security.Roles.RemoveUserFromRole(UserGroupMap.Item1, UserGroupMap.Item2);
            }
            //get rid of reset files
            foreach (string RS in RemoveResetFiles)
                System.IO.File.Delete(RS);

        }

        protected void CreateUsers_Click(object sender, EventArgs e)
        {
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
            List<string> ResetFiles = new List<string>(); //save list for possible rollback
            List<Tuple<string,string>> UserGroupRollback = new List<Tuple<string,string>>(); //save list for possible rollback
            string EmailBody = string.Empty;
            try
            {
                try
                {
                    
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

                            foreach( ListItem LI in ReportList.Items)
                            {
                                if ( LI.Selected )
                                {
                                    if ( System.Web.Security.Roles.IsUserInRole( User.Account_Name_No_Password, LI.Value) == false )
                                    {
                                        System.Web.Security.Roles.AddUserToRole(User.Account_Name_No_Password, LI.Value);
                                        EmailBody += User.Account_Name_No_Password + "|" + User.Password + "|NEW:" + LI.Value;
                                        UserGroupRollback.Add(Tuple.Create(User.Account_Name_No_Password, LI.Value));
                                    }
                                    else
                                    {
                                        EmailBody += User + "|" + User.Password + "|EXISTING:" + LI.Value;
                                    }
                                }
                            }

                        }
                        if (MU == null)
                            throw new Exception("User account not created for: " + User.Account_Name_No_Password);
                        CreatedUsers.Add(User.Account_Name_No_Password);

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
                    }
                }
                catch (Exception ex)
                {
                    UserCount = 0;
                    RollBack(CreatedUsers, NewUsers, ResetFiles, UserGroupRollback);
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified", ex);
                }

            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified Error - Groups", ex);
                UserCount = 0;
            }


            try
            {
                MillimanCommon.SuperGroup.SuperGroupContainer SGC = Session["supergroup"] as MillimanCommon.SuperGroup.SuperGroupContainer;

                if (UserCount > 0)
                {
                    if (SGC.AllowTempPasswordEntry == false)
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
                                    EmailBody = System.IO.File.ReadAllText(System.IO.Path.Combine(EmailTemplatesDir, "welcome.htm"));
                                    string Body = MillimanCommon.MillimanEmail.EmailMacroProcessor(EmailBody, U.Account_Name_No_Password, System.Web.Security.Membership.GetUser(U.Account_Name_No_Password).ProviderUserKey.ToString(), System.Web.Security.Membership.GetUser().UserName);

                                    MillimanCommon.MillimanEmail ME = new MillimanCommon.MillimanEmail();
                                    string From = System.Configuration.ConfigurationManager.AppSettings["SupportEmail"];
                                    string Title = System.Configuration.ConfigurationManager.AppSettings["SupportEmailWelcome"];
                                    ME.Send(U.Account_Name_No_Password, From, Body, Title, true, false);
                                }
                            }
                        }


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

                        MillimanCommon.MillimanEmail ME = new MillimanCommon.MillimanEmail();
                        ME.Send(System.Web.Security.Membership.GetUser().UserName, "PRM.Support@milliman.com", EmailBody, "PRM Created Users", true);

                        string Msg = "User accounts(s):\\n\\n";
                        foreach (string User in CreatedUsers)
                            Msg += User + "\\n";
                        Msg += "\\n have been added to the system.\\n\\nAn email has been dispatched to you with the account information.";

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



     
    }
}