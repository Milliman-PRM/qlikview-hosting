using SystemReporting.Controller.BusinessLogic.Controller;
using SystemReporting.Service;
using SystemReporting.Utilities;
using System;
using System.Configuration;


namespace SystemReporting.Controller
{
    /// <summary>
    /// Base class in hierarchy
    /// </summary>
    [Serializable]
    public abstract class ControllerBase : IController, IControllerAccessible
    {
        public ControllerBase() { }

        #region Registering Controllers
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

        #region Error Logger

        public static void LogError(Exception ex)
        {
            var LoggerFileDirectory = ConfigurationManager.AppSettings["LoggerFileDirectory"];
            var LoggerFileName = ConfigurationManager.AppSettings["LoggerFileName"];
            Logger.Instance.LogPath = LoggerFileDirectory;
            Logger.Instance.LogFileName = LoggerFileName;
            Logger.WriteLine("An exception:   Exception Message: " + ex.Message.ToString() +
                                             "Exception Trace : " + ex.StackTrace +
                                             "Exception Target: " + ex.TargetSite.ToString() +
                                             "Exception Source: " + ex.Source.ToString());
        }

        public static void LogError(string message)
        {
            var LoggerFileDirectory = ConfigurationManager.AppSettings["LoggerFileDirectory"];
            var LoggerFileName = ConfigurationManager.AppSettings["LoggerFileName"];
            Logger.Instance.LogPath = LoggerFileDirectory;
            Logger.Instance.LogFileName = LoggerFileName;
            Logger.WriteLine("Exception Message: " + message);
        }

        #endregion
    }

}
