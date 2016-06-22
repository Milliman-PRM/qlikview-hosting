using SystemReporting.Controller;
using SystemReporting.Controller.BusinessLogic.Controller;

namespace ReportFileGenerator
{
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
}
