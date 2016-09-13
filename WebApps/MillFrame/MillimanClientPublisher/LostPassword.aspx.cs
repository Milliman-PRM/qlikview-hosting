using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace ClientPublisher
{
    public partial class LostPassword : System.Web.UI.Page
    {
        private string WizardLabelText = "To reset your password enter your&nbsp;";
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UserName.Value = "";
                     
                SetState();
                lblErrorMessage.Visible = false;
            }
        }

        private void SetState()
        {
            int step = System.Convert.ToInt32(Step.Value);
            switch ( step )
            {
                case 0:   wizardlabel.Text = WizardLabelText;
                          txtUserName.ToolTip = " Email";
                          txtUserName.Text = UserName.Value;
                          break;
                case 1:   wizardlabel.Text = Membership.FindUsersByName(UserName.Value)[UserName.Value].PasswordQuestion;
                          txtUserName.ToolTip = "";
                          txtUserName.Text = "";
                          txtUserName.Focus();
                          break;
                case 2:   wizardlabel.Visible = false;
                          txtUserName.Visible = false;
                          lblErrorMessage.Text ="Password reset information has been emailed.";
                          lblErrorMessage.Visible = true;
                          break;
            }
        }

        private string ResetPassword(string EmailAddress)
        {
            //from here I could get the current password and place in encrypted cache so a link
            //could be added to the email to abort this password change by setting the password
            //back to the original value
            string NewPswd = "@" + System.Guid.NewGuid().ToString().Substring(0, 8);
            MembershipUserCollection MUC = Membership.FindUsersByName(EmailAddress);
            MembershipUser MU = MUC[EmailAddress];
            string PSWDEncrypted = MillimanCommon.Utilities.ConvertStringToHex(MU.GetPassword());
            if (MU.ChangePassword(MU.GetPassword(), NewPswd) == true)
            {
                string ResetFile = System.IO.Path.Combine(ConfigurationManager.AppSettings["ResetUserInfoRoot"], MU.ProviderUserKey + ".rst");
                System.IO.File.WriteAllText(ResetFile, MU.UserName + " added " + DateTime.Now.ToShortDateString() + "~" + PSWDEncrypted);
                return NewPswd;
            }
            return "";
        }

        private bool AllowSendEmail(string EmailAddress)
        {
            try
            {
                MembershipUserCollection MUC = Membership.FindUsersByName(EmailAddress);
                if (MUC.Count > 0)
                {
                    MembershipUser MU = MUC[EmailAddress];
                    string ResetFile = System.IO.Path.Combine(ConfigurationManager.AppSettings["ResetUserInfoRoot"], MU.ProviderUserKey + ".rst");
                    //if the file is older than 30 mins delete it, so it will get reset
                    var TimeCheck = new System.IO.FileInfo(ResetFile);
                    if (DateTime.UtcNow - TimeCheck.LastWriteTimeUtc > TimeSpan.FromMinutes(30))
                        System.IO.File.Delete(ResetFile);
                    return System.IO.File.Exists(ResetFile) == false; //no reset file, send an email
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        private void SendEmail(string EmailAddress)
        {
            //bug fix 1299 - don't allow resent of email on refresh, or multiple clicks, only send an email
            //once every 30 mins
            if (AllowSendEmail(EmailAddress) == false)
                return;
            //end bug fix 1299

            ResetPassword(EmailAddress);

            string EmailTemplatesDir = Server.MapPath("~/email_templates");
            string EmailBody = System.IO.File.ReadAllText(System.IO.Path.Combine(EmailTemplatesDir, "resetpassword.htm"));
            string Body = MillimanCommon.MillimanEmail.EmailMacroProcessor(EmailBody, EmailAddress, System.Web.Security.Membership.GetUser(EmailAddress).ProviderUserKey.ToString(), System.Web.Security.Membership.GetUser(EmailAddress).UserName);

            MillimanCommon.MillimanEmail ME = new MillimanCommon.MillimanEmail();
            string Title = System.Configuration.ConfigurationManager.AppSettings["EmailSubject"];
            string ProductName = System.Configuration.ConfigurationManager.AppSettings["ProductName"];
            string From = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"];
            ME.Send(EmailAddress, From, Body, Title, true, false);

            return;
            
            //MailMessage msg = new MailMessage();

            //string from = ConfigurationManager.AppSettings["EmailFrom"]; // "hcintel.support@milliman.com";
            //string to = EmailAddress;
            //string subject = ConfigurationManager.AppSettings["EmailSubject"]; //"Milliman Healthcare Password Recovery";
            //string bodyFile = ConfigurationManager.AppSettings["EmailBodyFile"];
            //string YourSMTPServer =  ConfigurationManager.AppSettings["SmtpServer"]; //"relay-2.attens.com";
            //int Port = System.Convert.ToInt32( ConfigurationManager.AppSettings["SmtpPort"]);

            //string NewPswd = ResetPassword(EmailAddress);
            //string body = System.IO.File.ReadAllText( MapPath( bodyFile ));
            //body = body.Replace("_EMAIL_", EmailAddress);
            //body = body.Replace("_NEWPASSWORD_", NewPswd );
            //body = body.Replace("_DATETIME_", DateTime.Now.ToString() );

            //msg.From = new MailAddress(from);
            //msg.To.Add(to);
            //msg.Subject = subject;
            //msg.Body = body;
            //msg.IsBodyHtml = true;
            //msg.Priority = MailPriority.High;

            //SmtpClient smtpClient = new SmtpClient(YourSMTPServer, Port);

            //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //string SMTPAccount = ConfigurationManager.AppSettings["SmtpUserName"];
            //string SMTPPassword = ConfigurationManager.AppSettings["SmtpPassword"];

            //if ( string.IsNullOrEmpty(SMTPAccount) == false )
            //    smtpClient.Credentials = new NetworkCredential(SMTPAccount, SMTPPassword);

            ////smtpClient.EnableSsl = true;  //if you use SSL then true

            //if ( (msg.To.Count > 0) && ( string.IsNullOrEmpty(NewPswd) == false ))
            //{
            //    try
            //    {
            //        smtpClient.Send(msg);
            //    }
            //    catch (Exception ex)
            //    {
            //        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.GiveUp, "Failed to send password reset to " + EmailAddress, ex);
            //    }
            //    //patched since ATT smtp does not work correctly 
            //    if (AzureCloudRelay(EmailAddress, body) == false)
            //    {
            //        try
            //        {
            //            LogResetEmail(EmailAddress, body);
            //        }
            //        catch (Exception ex)
            //        {
            //            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.GiveUp, "Failed to send password reset to " + EmailAddress, ex);
            //        }
            //    }
            //}
        }

        //private bool AzureCloudRelay(string EmailAddress, string body)
        //{
        //    try
        //    {
        //        MillimanSMTPRelay.MillimanRelaySoapClient SMTP = new MillimanSMTPRelay.MillimanRelaySoapClient();
        //        if (SMTP != null)
        //        {
        //            string From = ConfigurationManager.AppSettings["EmailFrom"];
        //            string Subject = ConfigurationManager.AppSettings["EmailSubject"];
        //            string CC_Config = ConfigurationManager.AppSettings["CC"];
        //            string BlindCC_Config = ConfigurationManager.AppSettings["BCC"];

        //            MillimanSMTPRelay.ArrayOfString CC = null;
        //            MillimanSMTPRelay.ArrayOfString BCC = null;
        //            if (CC_Config != null)
        //            {
        //                string[] CCItems = CC_Config.Split(new char[] { '~' });
        //                CC = new MillimanSMTPRelay.ArrayOfString();
        //                CC.AddRange(CCItems);
        //            }

        //            if (BlindCC_Config != null)
        //            {
        //                string[] BCCItems = BlindCC_Config.Split(new char[] { '~' });
        //                BCC = new MillimanSMTPRelay.ArrayOfString();
        //                BCC.AddRange(BCCItems);
        //            }

        //            MillimanSMTPRelay.ArrayOfString To = new MillimanSMTPRelay.ArrayOfString();
        //            To.Add(EmailAddress);
        //            SMTP.Relay(To, From, Subject, body, true, CC, BCC);

        //            //send a notification to support email
        //            MillimanSMTPRelay.ArrayOfString Support = new MillimanSMTPRelay.ArrayOfString();
        //            Support.Add(From);
        //            SMTP.Relay(Support, "PRM_Server@milliman.com", "Info", "User " + EmailAddress + " initiated lost password recovery.", false, null, null);
        //            SMTP.Close();
        //            SMTP = null;
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Warning, "Could not connect to Azure email service - " + ex.ToString());
        //    }
        //    return false;
        //}

        private void LogResetEmail(string ToAddress,
                                    string EmailBody)
        {
            try
            {
                string SMTPMailPickupDir = ConfigurationManager.AppSettings["SmtpMailPickup"];
                if (string.IsNullOrEmpty(SMTPMailPickupDir) == false)
                {
                    string NewMailID = System.Guid.NewGuid().ToString("N");
                    string ToIndex = System.IO.Path.Combine(SMTPMailPickupDir, NewMailID + ".to");
                    string BodyFile = System.IO.Path.Combine(SMTPMailPickupDir, NewMailID + ".email");
                    System.IO.File.WriteAllText(ToIndex, ToAddress);
                    System.IO.File.WriteAllText(BodyFile, EmailBody);
                }
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Warning, "Failed to log reset password", ex);
            }
        }

        protected void Next_Click(object sender, ImageClickEventArgs e)
        {
            int step = System.Convert.ToInt32(Step.Value);
            int Trys = System.Convert.ToInt32(UserTrys.Value);
            lblErrorMessage.Visible = false;
            if (step == 0)
            {  //check email
                if (string.IsNullOrEmpty(txtUserName.Text) == true)
                {
                    lblErrorMessage.Text = "An email address must be entered to continue.";
                    lblErrorMessage.Visible = true;
                    Trys++;
                    if (Trys == 3)
                        Response.Redirect("userlogin.aspx"); // too many attempts
                    return;
                }
                if (Membership.FindUsersByName(txtUserName.Text).Count == 0)
                {
                    lblErrorMessage.Text = "Your information could not be verified.";
                    lblErrorMessage.Visible = true;
                    Trys++;
                    if (Trys == 3)
                        Response.Redirect("userlogin.aspx"); // too many attempts
                    return;
                }
                UserName.Value = txtUserName.Text; //store to look at later
                Trys = 0;
            }
            else if (step == 1)
            { //challenge question
                if (string.IsNullOrEmpty(txtUserName.Text.Trim()) == true)
                {
                    lblErrorMessage.Text = "An answer must be entered to continue.";
                    lblErrorMessage.Visible = true;
                    Trys++;
                    if (Trys == 3)
                        Response.Redirect("userlogin.aspx"); // too many attempts
                    return;
                }
                string Answer = Membership.FindUsersByName(UserName.Value)[UserName.Value].Comment;

                if ((string.IsNullOrEmpty(Answer) == true) || (string.Compare(Membership.FindUsersByName(UserName.Value)[UserName.Value].Comment.Trim(), txtUserName.Text.Trim(), true) != 0))
                {
                    lblErrorMessage.Text = "Your information could not be verified.";
                    lblErrorMessage.Visible = true;
                    Trys++;
                    if (Trys == 3)
                        Response.Redirect("userlogin.aspx"); // too many attempts
                    return;
                }
                else
                {
                    SendEmail(UserName.Value);
                }
            }
            else if (step == 2)
            { //display message
                Response.Redirect("userlogin.aspx");
            }

            step++;
            Step.Value = (step >= 2) ? "2" : step.ToString();
            SetState();
        }
    }
}