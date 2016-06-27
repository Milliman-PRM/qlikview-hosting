using log4net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using SystemReporting.Utilities.Email;
using C = PasswordUtilityProcessor.Constants;

namespace PasswordUtilityProcessor
{
    public class BaseFileProcessor
    {
        //switch to using the generic logger
        public static log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BaseFileProcessor()  {  }
        
        #region Error Log     
        //Logs error. It will create file at the directory if file does not exist   
        public static void LogError(Exception ex, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                log.Error("||-||" + message + "||-|| ", ex);                
            }
            else
            {
                log.Error("||-||", ex);
            }

            //Setting error logged filter
            C.ERROR_LOGGED = true;
        }
        public static void SendEmail(string message, string subject)
        {
            var msg = "Exception file has a new exception recorded. Please cehck the file at the file locaion "
                            + ConfigurationManager.AppSettings["ExceptionLoggerDirectory"];
            //send email
            Notification.SendNotification(message + Environment.NewLine + msg,"Password Reset Utility");
        }
        #endregion
    }

    /// <summary>
    /// Creating this constant class
    /// </summary>
    public static class Constants
    {
        //everytime an error is recorded, this will be set
        public static bool ERROR_LOGGED = false;    
    }
}
