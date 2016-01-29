using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemReporting.Utilities.ExceptionHandling
{
    public static class ExceptionLogger
    {
        public static void WriteErrorLog(string message)
        {
            StringBuilder sb = new StringBuilder();

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
    }
}
