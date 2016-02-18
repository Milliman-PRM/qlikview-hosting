using log4net;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace SystemReporting.Utilities.ExceptionHandling
{
    public class SystemExceptionHandling
    {
        #region Properties

        /// <summary>
        /// declared at class level in all classes requiring logging.
        /// </summary>
        public static readonly ILog logger = LogManager.GetLogger(typeof(SystemExceptionHandling));

        private static string environmentName = ConfigurationManager.AppSettings["SystemEnvironmentName"];

        public static string Source
        {
            get
            {
                if (string.IsNullOrEmpty(environmentName))
                    environmentName = "LogFileProcessor";

                return environmentName;
            }
        }
        public const string LogName = "System";
        public const string MachineName = ".";

        #endregion

        #region Methods

        public static void WriteInfo(string message)
        {
            EventLog.WriteEntry(Source, message, EventLogEntryType.Information);
            logger.Info(message);
        }

        public static void WriteException(Exception exception)
        {
            try
            {
                Trace.WriteLine(Environment.NewLine + "************************************************************************************");
                Trace.WriteLine(System.DateTime.Now.ToString());
                Trace.Flush();

                if (exception != null)
                {
                    var message = new StringBuilder();
                    FormatErrorMessage(exception, message);

                    if (exception.Data.Contains("AdditionalMessage"))
                        message.AppendLine("Additional Message: " + exception.Data["AdditionalMessage"].ToString());

                    exception = exception.InnerException;
                    message.AppendLine("InnerException: ");
                    while (exception != null)
                    {
                        FormatErrorMessage(exception, message);
                        exception = exception.InnerException;
                    }
                    Trace.Write(message.ToString());
                    Trace.Flush();

                    EventLog.WriteEntry(Source, message.ToString(), EventLogEntryType.Error);
                    logger.Error(message.ToString(), exception);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                Trace.Flush();
                System.Console.WriteLine(e.Message);
            }
        }

        #endregion

        #region Helper Methods

        private static void FormatErrorMessage(Exception exception, StringBuilder message)
        {
            if (!string.IsNullOrEmpty(exception.Message))
                message.AppendLine("Message").AppendLine(exception.Message).AppendLine();

            if (!string.IsNullOrEmpty(exception.Source))
                message.AppendLine("Source").AppendLine(exception.Source).AppendLine();

            if (exception.TargetSite != null)
                message.AppendLine("Target Site").AppendLine(exception.TargetSite.ToString()).AppendLine();

            if (!string.IsNullOrEmpty(exception.StackTrace))
                message.AppendLine("Stack Trace").AppendLine(exception.StackTrace).AppendLine();

            message.AppendLine("ToString()").AppendLine(exception.ToString()).AppendLine();
        }

        #endregion

    }
}
