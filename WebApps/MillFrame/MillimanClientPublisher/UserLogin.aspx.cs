using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using System;
using System.Web;
using System.Web.Security;



namespace ClientPublisher
{
    public partial class UserLogin : System.Web.UI.Page
    {
         protected override void OnLoad(System.EventArgs e)
        {           

            base.OnLoad(e);

            txtUserName.Focus();
         
            string error = Request.QueryString[Util.ErrorVarName];
            if (error == null)
                error = string.Empty;

            // Display any error message resulting from a failed login if any.
            lblErrorMessage.Text = error;

            if (Request.RawUrl.ToLower().IndexOf("secureid=") != -1)
            {
                string AdminName = string.Empty;
                string UserName = string.Empty;
                string UserID = string.Empty;
                DateTime CertCreated = DateTime.MinValue;
                if (MillimanCommon.SecureLink.IsSecureLinkValid(Request["SecureID"], out AdminName, out UserName, out UserID, out CertCreated) == true)
                {
                    MembershipUserCollection Users = Membership.FindUsersByName(UserName);
                    if ((Users == null) || (Users.Count == 0) || (Users.Count > 1))
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Secure link cert recieved but no users of that name appear in membership - " + UserName);
                        Response.Redirect("HTML/SecureLinkIssue.html");
                        return;
                    }
                    MembershipUser User = Users[UserName];
                    //User should never be null if we got this far
                    if (string.Compare(User.ProviderUserKey.ToString(), UserID, true) != 0)
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Secure link cert recieved but user id did not match with the name that appears in membership - " + UserName + ":" + UserID);
                        Response.Redirect("HTML/SecureLinkIssue.html");
                        return;

                    }
                    if (Membership.ValidateUser(UserName, User.GetPassword()))
                    {
                        this.Session["milliman"] = "Yes, I am!";
                        Session["patientid"] = "";
                        MembershipUserCollection MUC = Membership.FindUsersByName(UserName);
                        MembershipUser MU = MUC[UserName];

                        //there should be a reset file too, if not they are trying to use smart link more than once
                        string ResetFile = System.IO.Path.Combine(WebConfigurationManager.AppSettings["ResetUserInfoRoot"], UserID + ".rst");
                        if (System.IO.File.Exists(ResetFile))
                        {
                            FormsAuthentication.SetAuthCookie(MU.UserName, false);
                            Response.Redirect("default.aspx");
                        }
                        else
                            Response.Redirect("HTML/SecureLinkUsed.html");
                    }
                }
                else
                {  //secure link is not valid or has expired
                    Response.Redirect("HTML/SecureLinkIssue.html");
                }
            }
            else if (Membership.GetUser() != null)
            {
                Response.Redirect("default.aspx");  //already logged in
            }
        }

        //local authentication
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (Membership.ValidateUser(txtUserName.Text, txtPassword.Text))
            {
                MembershipUserCollection MUC = Membership.FindUsersByName(txtUserName.Text);
                MembershipUser MU = MUC[txtUserName.Text];

                string PasswordExpirationDaysString = WebConfigurationManager.AppSettings["PasswordExpirationDays"];
                int PasswordExpirationDays;
                if (int.TryParse(PasswordExpirationDaysString, out PasswordExpirationDays) == false)
                {
                    PasswordExpirationDays = 90;  // default
                }
                TimeSpan ConfiguredPasswordExpiration = new TimeSpan(PasswordExpirationDays, 0, 0, 0);
                TimeSpan TimeSinceLastPasswordChange = DateTime.Now - MU.LastPasswordChangedDate;

                if (TimeSinceLastPasswordChange > ConfiguredPasswordExpiration)
                {
                    // Password is expired, handle that.
                    Session["passwordagedays"] = (int)(TimeSinceLastPasswordChange.TotalDays);
                    Response.Redirect("PasswordExpired.aspx");
                    return;
                }
                else
                {
                    // OK to treat as authenticated session
                    this.Session["milliman"] = "Yes, I am!";
                    Session["patientid"] = "";
                    FormsAuthentication.SetAuthCookie(MU.UserName, false);

                    Response.Redirect("default.aspx");
                    return;
                }
            }
            else
            {
                lblErrorMessage.Text = "A valid user name and password is required for access!";
            }
        }

        /// <summary>
        /// Publisher does not maintain state across runs, so scub directory and force
        /// user to start again
        /// </summary>
        private void CleanupUserLocalDirectory()
        {
           string UserDirectory =  PublisherUtilities.GetWorkingDirectory(txtUserName.Text, "");
           if ( string.IsNullOrEmpty(UserDirectory) == false )
           {
               //we want to empty the directory for a clean start, easy way is to delete and re-create

               try
               {   //sometimes this throws an exception
                   System.IO.Directory.Delete(UserDirectory, true);
               }
               catch (Exception)
               {
                   System.Threading.Thread.Sleep(100);  //maybe OS has not really delete all yet, give it a little time
               }

               //if we cannot re-create, let it throw an exception
               System.IO.Directory.CreateDirectory(UserDirectory);
               if (System.IO.Directory.Exists(UserDirectory))
               {
                   //we always want to start clean, so check to make sure
                   string[] AllFiles = System.IO.Directory.GetFiles(UserDirectory);
                   if (AllFiles.Length != 0)
                   {
                       Response.Redirect("HTML/CleanupIssue.html");
                   }
               }
           }
        }
    }
}