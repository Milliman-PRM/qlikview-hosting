using System;
using System.Net.Mail;
using System.Configuration;
using System.IO;
using log4net;

namespace SystemReporting.Utilities.Email
{
    public class Notification
    {
        //switch to using the generic logger
        public static log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void SendNotification(string message, string subject)
        {
            try
            {
                using (var email = new Email(ConfigurationManager.AppSettings["SmtpServer"]))
                {
                    var withEmail = email;
                    withEmail.From = ConfigurationManager.AppSettings["EmailFrom"];
                    withEmail.To = ConfigurationManager.AppSettings["EmailTo"];
                    withEmail.CC = ConfigurationManager.AppSettings["EmailCC"];
                    withEmail.Priority = Email.MailPriority.High;
                    withEmail.Subject = subject + "  -  "  + DateTime.Now;
                    withEmail.Body = GetMessage() + Environment.NewLine + Environment.NewLine + message + 
                                                    Environment.NewLine + Environment.NewLine + Environment.NewLine + 
                                                    Environment.NewLine + "Thank You";
                    withEmail.SendMail();
                }
            }
            catch (Exception ex)
            {
                log.Error("Notification.SendNotification. Failed to send email.", ex);
            }            
        }
        
        public static void SendNotification(string fileName, string message , string subject)
        {
            try
            {
                using (var email = new Email(ConfigurationManager.AppSettings["SmtpServer"]))
                {
                    var withEmail = email;
                    withEmail.From = ConfigurationManager.AppSettings["EmailFrom"];
                    withEmail.To = ConfigurationManager.AppSettings["EmailTo"];
                    withEmail.CC = ConfigurationManager.AppSettings["EmailCC"];
                    withEmail.Priority = Email.MailPriority.High;
                    withEmail.Subject = subject + "  -  " + DateTime.Now;
                    withEmail.Body = GetMessage() + Environment.NewLine + Environment.NewLine + message +
                                                    Environment.NewLine + Environment.NewLine + Environment.NewLine +
                                                    Environment.NewLine + "Thank You";
                    withEmail.AddAttachment(fileName);
                    withEmail.SendMail();
                }
            }
            catch (Exception ex)
            {
                log.Error("Notification.SendNotification. Failed to send email.", ex );
            }
        }

        public static string GetMessage()
        {
            var Message = new System.Text.StringBuilder();
            Message.Append(Environment.NewLine + Environment.NewLine);
            Message.Append("******************************************************************");
            Message.Append(Environment.NewLine + Environment.NewLine);
            Message.Append(" This is automated Emial for information only. Please do not Reply to this email." + Environment.NewLine);
            Message.Append("******************************************************************" + Environment.NewLine);
            Message.Append(Environment.NewLine + Environment.NewLine);
            return Message.ToString();
        }
    }
}
