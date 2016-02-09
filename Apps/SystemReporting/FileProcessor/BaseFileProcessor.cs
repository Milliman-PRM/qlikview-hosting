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
        #region FileProcessed
        public static void LogProcessedFile(string message)
        {
            Console.WriteLine("Processed successfully file: {0}", message);
            Logger.Instance.LogPath = FileFunctions.GetProcessedFileLogDirectory();
            Logger.Instance.LogFileName = FileFunctions.GetProcessedFileLogFileName();

            FileFunctions.FileCheck(Logger.Instance.LogPath + Logger.Instance.LogFileName);
            Logger.WriteLine(DateTime.Now + " ProcessedFileName:~ " + message + Environment.NewLine);
        }

        #endregion
    }
    #endregion
}
