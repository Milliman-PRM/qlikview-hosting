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
        public static string _exceptionLoggerFileDirectory = FileFunctions.GetExceptionLoggerFileDirectory();
        public static string _exceptionLoggerFileName = FileFunctions.GetExceptionLoggerFileName();

        #region FileProcessed

        /// <summary>
        /// Record the file that was just processed. It will also check if the file exist
        /// If the file does not, then create file and send email
        /// </summary>
        /// <param name="message"></param>
        public static void LogProcessedFile(string message)
        {
            Console.WriteLine("Processed successfully file: {0}", message);
            Logger.Instance.LogPath = FileFunctions.GetProcessedFileLogDirectory();
            Logger.Instance.LogFileName = FileFunctions.GetProcessedFileLogFileName();
            FileFunctions.FileCheck(Logger.Instance.LogPath + Logger.Instance.LogFileName);
            Logger.WriteLine(DateTime.Now + " ProcessedLogFileName:~ " + message + Environment.NewLine);
        }
        #endregion

        #region Error Log     
        //Logs error. It will create file at the directory if file does not exist   
        public static void LogError(Exception ex, string message)
        {
            if (ex != null && !string.IsNullOrEmpty(message))
            {
                LogExAndErr(ex, message);
            }
            else if (ex == null & (!string.IsNullOrEmpty(message)))
            {
                LogError(message);
            }
            else if (ex != null & (string.IsNullOrEmpty(message)))
            {
                LogError(ex);
            }
        }
        public static void LogError(Exception ex)
        {
            Logger.Instance.LogPath = _exceptionLoggerFileDirectory;
            Logger.Instance.LogFileName = _exceptionLoggerFileName;
            Logger.WriteLine(DateTime.Now + " Todays Exceptions: ~ " + "Exception Message: " + ex.Message.ToString() + "||-||"
                                          + "Exception Trace : " + ex.StackTrace + "||-||"
                                          + "Exception Target: " + ex.TargetSite.ToString() + "||-||"
                                          + "Exception Source: " + ex.Source.ToString()
                                          + Environment.NewLine);
        }
        public static void LogError(string message)
        {
            Logger.Instance.LogPath = _exceptionLoggerFileDirectory;
            Logger.Instance.LogFileName = _exceptionLoggerFileName;
            Logger.WriteLine(DateTime.Now + " Todays Exceptions: ~ " + "||-||"
                                          + " Exception Message: " + message
                                          + Environment.NewLine);
        }
        public static void LogExAndErr(Exception ex, string message)
        {
            Logger.Instance.LogPath = _exceptionLoggerFileDirectory;
            Logger.Instance.LogFileName = _exceptionLoggerFileName;
            Logger.WriteLine(DateTime.Now + " Todays Exceptions: ~ " + "||-||"
                                          + message + "||-||"
                                          + "Exception Message: " + ex.Message.ToString() + "||-||"
                                          + "Exception Trace : " + ex.StackTrace + "||-||"
                                          + "Exception Target: " + ex.TargetSite.ToString() + "||-||"
                                          + "Exception Source: " + ex.Source.ToString()
                                          + Environment.NewLine);
        }
        #endregion        
    }
    #endregion
}
