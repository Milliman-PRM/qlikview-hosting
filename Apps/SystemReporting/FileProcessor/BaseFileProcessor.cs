using SystemReporting.Controller;
using SystemReporting.Controller.BusinessLogic.Controller;
using SystemReporting.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor
{
    /// <summary>
    /// creating an interface
    /// </summary>
    public interface IFileProcessor
    {
        void ProcessFileData(string args);
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
        #region Error Log
        public static void LogError(Exception ex, string message)
        {
            if (ex != null && !string.IsNullOrEmpty(message))
            {
                LogExAndErr( ex, message);
            }
            else if (ex == null & (!string.IsNullOrEmpty(message)))
            {
                LogError(message);
            }
            else if (ex!=null & (string.IsNullOrEmpty(message)))
            {
                LogError(ex);
            }
        }        
        public static void LogError(Exception ex)
        {
            var LoggerFileDirectory = ConfigurationManager.AppSettings["LoggerFileDirectory"];
            var LoggerFileName = ConfigurationManager.AppSettings["LoggerFileName"];
            Logger.Instance.LogPath = LoggerFileDirectory;
            Logger.Instance.LogFileName = LoggerFileName;
            Logger.WriteLine(DateTime.Now + " Todays Exceptions: ~ " +  "Exception Message: " + ex.Message.ToString() + "||-||" 
                                          +  "Exception Trace : " + ex.StackTrace + "||-||"
                                          +  "Exception Target: " + ex.TargetSite.ToString() + "||-||"
                                          +  "Exception Source: " + ex.Source.ToString()
                                          + Environment.NewLine);
        }
        public static void LogError(string message)
        {
            var LoggerFileDirectory = ConfigurationManager.AppSettings["LoggerFileDirectory"];
            var LoggerFileName = ConfigurationManager.AppSettings["LoggerFileName"];
            Logger.Instance.LogPath = LoggerFileDirectory;
            Logger.Instance.LogFileName = LoggerFileName;
            Logger.WriteLine(DateTime.Now + " Todays Exceptions: ~ "   + "||-||"
                                          + " Exception Message: " + message
                                          + Environment.NewLine);
        }
        public static void LogExAndErr(Exception ex, string message)
        {
            var LoggerFileDirectory = ConfigurationManager.AppSettings["LoggerFileDirectory"];
            var LoggerFileName = ConfigurationManager.AppSettings["LoggerFileName"];
            Logger.Instance.LogPath = LoggerFileDirectory;
            Logger.Instance.LogFileName = LoggerFileName;
            Logger.WriteLine(DateTime.Now + " Todays Exceptions: ~ " + "||-||"
                                          + message  + "||-||"
                                          + "Exception Message: " + ex.Message.ToString()   + "||-||"
                                          + "Exception Trace : " + ex.StackTrace   + "||-||"
                                          + "Exception Target: " + ex.TargetSite.ToString()  + "||-||"
                                          + "Exception Source: " + ex.Source.ToString()
                                          + Environment.NewLine);
        }
        #endregion

        #region FileProcessed
        public static void LogProcessedFile(string message)
        {
            var LoggerFileDirectory = ConfigurationManager.AppSettings["ProcessedFileLogDirectory"];
            var LoggerFileName = ConfigurationManager.AppSettings["ProcessedFileLogFileName"];
            Logger.Instance.LogPath = LoggerFileDirectory;
            Logger.Instance.LogFileName = LoggerFileName;
            Logger.WriteLine(DateTime.Now + " ProcessedFileName:~ " + message + Environment.NewLine);
        }
        #endregion
    }
    #endregion
}
