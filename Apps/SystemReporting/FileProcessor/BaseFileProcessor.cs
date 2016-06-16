using SystemReporting.Controller;
using SystemReporting.Controller.BusinessLogic.Controller;
using SystemReporting.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemReporting.Utilities.File;
using SystemReporting.Utilities.Email;

namespace FileProcessor
{
    /// <summary>
    /// creating an interface
    /// </summary>
    public interface IFileProcessor
    {
        void ProcessLogFileData(string args);
    }

    #region IControllerAccessible Members
    /// <summary>
    /// This class will be called where ever we need to use the controller for this project
    /// </summary>
    public class ControllerAccess : IControllerAccessible
    {
        #region IControllerAccessible Members

        private AuditLogController _auditLogController;
        /// <summary>
        /// Gets a lazy-loaded reference to the controller to handle client-related matters.
        /// </summary>
        public AuditLogController ControllerAuditLog
        {
            get
            {
                if (_auditLogController == null)
                    _auditLogController = ControllerFactory.Create<AuditLogController>();
                return _auditLogController;
            }
        }

        private IisLogController _iisLogController;
        /// <summary>
        /// Gets a lazy-loaded reference to the controller to handle client-related matters.
        /// </summary>
        public IisLogController ControllerIisLog
        {
            get
            {
                if (_iisLogController == null)
                    _iisLogController = ControllerFactory.Create<IisLogController>();
                return _iisLogController;
            }
        }

        private SessionLogController _sessionLogController;
        /// <summary>
        /// Gets a lazy-loaded reference to the controller to handle client-related matters.
        /// </summary>
        public SessionLogController ControllerSessionLog
        {
            get
            {
                if (_sessionLogController == null)
                    _sessionLogController = ControllerFactory.Create<SessionLogController>();
                return _sessionLogController;
            }
        }

        private CommonController _commonController;
        /// <summary>
        /// Gets a lazy-loaded reference to the controller to handle client-related matters.
        /// </summary>
        public CommonController ControllerCommon
        {
            get
            {
                if (_commonController == null)
                    _commonController = ControllerFactory.Create<CommonController>();
                return _commonController;
            }
        }
        #endregion
    }
    #endregion

    #region Common Methods
    /// <summary>
    /// Base class to be inherited by other classes and this class has common methods
    /// </summary>
    public class BaseFileProcessor
    {
        //switch to using the generic logger
        public static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.
                                                                               MethodBase.GetCurrentMethod().DeclaringType);
        #region FileProcessed

        /// <summary>
        /// Record the file that was just processed. It will also check if the file exist
        /// If the file does not, then create file and send email
        /// </summary>
        /// <param name="message"></param>
        public static void LogProcessedFile(string message)
        {
            Console.WriteLine("Processed file: {0}", message);
            var fMessage = DateTime.Now + " ProcessedLogFileName:~ " + message;
            var filePath = FileFunctions.GetProcessedFileLogDirectory() + FileFunctions.GetProcessedFileLogFileName() + ".log";
            FileFunctions.FileCheck(filePath);           
            var aFile = new System.IO.FileStream(filePath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
            var sw = new System.IO.StreamWriter(aFile);
            sw.WriteLine(fMessage);
            sw.Close();
            aFile.Close();
        }
        #endregion

        #region Error Log     
        //Logs error. It will create file at the directory if file does not exist   
        public static void LogError(Exception ex, string message, bool sendEmail)
        {
            if (!string.IsNullOrEmpty(message))
            {
                log.Error(DateTime.Now + " Todays Exceptions: ~ " + "||-||" + message + "||-||", ex);
            }
            else
            {
                log.Error(DateTime.Now + " Todays Exceptions: ~ " + "||-||", ex);
            }

            if (sendEmail)
                SendEmail(message,"");
        }
        public static void SendEmail(string message, string subject)
        {
            Notification.SendNotification("Exception file has a new exception recorded. Please cehck the file at the file locaion "
            + ConfigurationManager.AppSettings["ExceptionFileDirectory"].ToString(),
                "System Reporting");
        }
        #endregion        
    }
    #endregion
}
