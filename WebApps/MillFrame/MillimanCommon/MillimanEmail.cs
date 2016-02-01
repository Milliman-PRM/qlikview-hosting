using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanCommon
{
    public class MillimanEmail
    {
        public void Send( string To,  string From, string Body, string Subject, bool HighPriorty = true, bool Async = true )
        {
            try
            {
                System.Net.Mail.MailMessage objeto_mail = new System.Net.Mail.MailMessage();
                bool SendDebugEmail = System.Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["SendDebugEmail"]);
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.Port = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                client.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                client.Timeout = 120000;
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = true;
                if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["SmtpUserName"]) == false)
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["SmtpUserName"], System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"]);
                }
                    
                objeto_mail.From = new System.Net.Mail.MailAddress(From);
                objeto_mail.To.Add(new System.Net.Mail.MailAddress(To));


                if (SendDebugEmail)
                {
                    string DebugEmailAddress = System.Configuration.ConfigurationManager.AppSettings["EmailDebugAccount"];
                    objeto_mail.Bcc.Add(new System.Net.Mail.MailAddress(DebugEmailAddress));
                }

                objeto_mail.Subject = Subject;
                objeto_mail.IsBodyHtml = true;
                objeto_mail.Priority = HighPriorty ? System.Net.Mail.MailPriority.High : System.Net.Mail.MailPriority.Normal;
                objeto_mail.Body = Body;
                if (Async)
                {
                    client.SendCompleted += client_SendCompleted;
                    client.SendAsync(objeto_mail, To);
                }
                else
                {
                    client.Send(objeto_mail);
                }
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(Report.ReportType.Error, "Unspecified", ex);
            }
        }

        void client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MillimanCommon.Report.Log(Report.ReportType.Error, "Failed to send email to: " + e.UserState.ToString(), e.Error);
                string SupportEmail = System.Configuration.ConfigurationManager.AppSettings["SupportEmail"];
                if (string.IsNullOrEmpty(SupportEmail))
                    Send(SupportEmail, "PRM Server", "Failed to send email to: " + e.UserState.ToString(), "PRM Send Email Issue", true, false);
            }
        }

        static public string EmailMacroProcessor(string EmailBody, string Account, string AccountID, string AdminEmail)
        {
            //VWN:  MUST change back to MILLIMAN when production
            string SiteName = System.Configuration.ConfigurationManager.AppSettings["ProductName"];

            string NewEmailBody = EmailBody.Replace("<%username%>", Account);
            NewEmailBody = NewEmailBody.Replace("<%date%>", DateTime.Now.ToLongDateString());
            NewEmailBody = NewEmailBody.Replace("<%datetime%>", DateTime.Now.ToString());

            System.Web.Security.MembershipUser muser = System.Web.Security.Membership.GetUser(Account);
            string ResetFile = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["ResetUserInfoRoot"], muser.ProviderUserKey + ".rst");
            //go ahead and write out reset file
            System.IO.File.WriteAllText(ResetFile, muser.UserName + " added " + DateTime.Now.ToShortDateString());

            string SecureLink = MillimanCommon.SecureLink.CreateSecureLink(AdminEmail, Account, AccountID);
            string SecureLinkHref = "<a href='_URL_'>Click here to start using PRM.</a>";
            string Href = System.Configuration.ConfigurationManager.AppSettings["ExternalServerName"] + "/" + SiteName +"/UserLogin.aspx?secureid=" + SecureLink;
            SecureLinkHref = SecureLinkHref.Replace("_URL_", Href);
            NewEmailBody = NewEmailBody.Replace("<%securelink%>", SecureLinkHref);
            NewEmailBody = NewEmailBody.Replace("<%millimanlogo%>", "");
            NewEmailBody = NewEmailBody.Replace("<%hcintellogo%>", System.Configuration.ConfigurationManager.AppSettings["ExternalServerName"] + "/" + SiteName + "/PRMLogo_height80.png");
            NewEmailBody = NewEmailBody.Replace("<%siteurl%>", System.Configuration.ConfigurationManager.AppSettings["ExternalServerName"]  );
            int SecureLinkLifeSpan = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SecureLinkLifeSpan"]);
            string LinkExpires = DateTime.Now.AddDays(SecureLinkLifeSpan).ToString();
            LinkExpires += " " + TimeZoneName(DateTime.Now);
            NewEmailBody = NewEmailBody.Replace("<%securelinkexpires%>", LinkExpires );

            return NewEmailBody;
        }

        /// <summary>
        /// Get the timezone abreviation, if the server is ever place where they do no observe daylight saving time, this 
        /// will give a wierd abreviation, so watch out for Arizona and Indiana
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        static public string TimeZoneName(DateTime dt)
        {
            String sName = TimeZone.CurrentTimeZone.IsDaylightSavingTime(dt)
                ? TimeZone.CurrentTimeZone.DaylightName
                : TimeZone.CurrentTimeZone.StandardName;

            String sNewName = "";
            String[] sSplit = sName.Split(new char[] { ' ' });
            foreach (String s in sSplit)
                if (s.Length >= 1)
                    sNewName += s.Substring(0, 1);

            return sNewName;
        }
    }
}
