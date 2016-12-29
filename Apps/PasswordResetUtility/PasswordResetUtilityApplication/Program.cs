using PasswordUtilityProcessor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PasswordResetUtilityApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log4net.Config.XmlConfigurator.Configure();
            try
            {
                //we dont accept any command line parms - must be configured via web.config
               PasswordProcessor.ExecutePasswordResetUtility();
               log.Info(DateTime.Now.ToString("HH:mm:ss tt") + " password reset task completed successfully.");
               SendEmail(PasswordProcessor.NewPasswordResetsThisIteration.ToString() + " additional users are being requested to reset passwords this iteration for a total of " + PasswordProcessor.TotalPasswordResets.ToString() + " users in a password reset state.");
               Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
               //create a logger via app.config and dump out the issue to  it
               log.Error(DateTime.Now.ToString("HH:mm:ss tt") , ex);           
            }
        }

        /// <summary>
        /// this should come from a re-use lib not be here.  However until password reset is assimilated into Millframe solution will leave to reduce dependancy
        /// on large re-use assembly from different solution.
        /// Do not trap exceptions, allow them to propagate back to caller 
        /// </summary>
        /// <param name="Msg">the message to send for status</param>
        public static void SendEmail( string Msg )
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient(System.Configuration.ConfigurationManager.AppSettings["SmtpServer"]);

            mail.From = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["EmailFrom"]);
            mail.To.Add(System.Configuration.ConfigurationManager.AppSettings["EmailTo"]);
            mail.Subject = System.Configuration.ConfigurationManager.AppSettings["PRM Password Reset Daemon"];
            mail.Body = Msg;

            SmtpServer.Port = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
            SmtpServer.Credentials = null;
            SmtpServer.EnableSsl = false;

            SmtpServer.Send(mail);

        }
    }
}
