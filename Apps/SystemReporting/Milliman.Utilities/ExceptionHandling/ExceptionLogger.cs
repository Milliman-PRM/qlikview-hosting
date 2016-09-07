using System;
using System.Configuration;
using System.IO;
using System.Text;
using SystemReporting.Utilities.Email;
using C = SystemReporting.Utilities.Constants;

namespace SystemReporting.Utilities.ExceptionHandling
{
    public static class ExceptionLogger
    {
        public static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.
                                                                               MethodBase.GetCurrentMethod().DeclaringType);

        public static void WriteErrorLog(string message)
        {
            var sb = new StringBuilder();

            sb.AppendLine("System Exceptions");
            sb.AppendLine("= = = = = =");
            sb.AppendLine("Message = " + message);
            sb.AppendLine("DateTime = " + System.DateTime.Now.ToString());

            var sAttr = ConfigurationManager.AppSettings.Get("ExceptionLoggerFile");

            using (StreamWriter outfile =  new StreamWriter(ConfigurationManager.AppSettings["filename"]))
            {
                outfile.Write(sb.ToString());
            }

        }

        public static void WriteErrorLog(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("System Exception");
            sb.AppendLine("= = = = = =");
            sb.AppendLine("Exception Source = " + ex.Source.ToString());
            sb.AppendLine("Exception Target = " + ex.TargetSite.ToString());
            sb.AppendLine("Exception Message = " + ex.Message.ToString());
            sb.AppendLine("Exception StackTrace = " + ex.StackTrace.ToString());
            sb.AppendLine("Exception InnerException = " + ex.InnerException.Source.ToString());
            sb.AppendLine("DateTime = " + System.DateTime.Now.ToString());
            var sAttr = ConfigurationManager.AppSettings.Get("ExceptionLoggerFile");

            using (StreamWriter outfile = new StreamWriter(ConfigurationManager.AppSettings["filename"]))
            {
                outfile.Write(sb.ToString());
            }
        }
        public static void LogError(Exception ex, string message, string subject)
        {
            if (!string.IsNullOrEmpty(message))
            {
                log.Error(DateTime.Now.ToString("HH:mm:ss tt") + " Todays Exceptions: ~ " + "||-||" + subject + "||-||" +  message + "||-||", ex);
            }
            else
            {
                log.Error(DateTime.Now.ToString("HH:mm:ss tt") + " Todays Exceptions: ~ " + "||-||" + subject + "||-||" , ex);
            }

            //Setting error logged filter
            C.ERROR_LOGGED = true;
        }

        #region Notification
        public static void SendErrorEmail(string exceptionDir, string subject)
        {
            //send email
            SendEmail("Exception occured in Application.", subject, exceptionDir);
            C.ERROR_LOGGED = false;
        }
        public static void SendEmail(string message, string subject, string exceptionDir)
        {
            var msg = "Exception file has a new exception recorded. Please cehck the file at the file locaion "
                            + exceptionDir;
            //send email
            Notification.SendNotification(message + Environment.NewLine + msg, subject);
        }
        #endregion 
    }
}
