using SystemReporting.Controller.BusinessLogic.Controller;

namespace SystemReporting.Controller
{
    public static class ControllerContext
    {
        private static AuditLogController _auditLogController;
        /// <summary>
        /// Gets a lazy-loaded reference to the controller to handle client-related matters.
        /// </summary>
        public static AuditLogController ControllerAuditLog
        {
            get
            {
                if (_auditLogController == null)
                    _auditLogController = ControllerFactory.Create<AuditLogController>();
                return _auditLogController;
            }
        }

        private static IisLogController _iisLogController;
        /// <summary>
        /// Gets a lazy-loaded reference to the controller to handle client-related matters.
        /// </summary>
        public static IisLogController ControllerIisLog
        {
            get
            {
                if (_iisLogController == null)
                    _iisLogController = ControllerFactory.Create<IisLogController>();
                return _iisLogController;
            }
        }

        private static SessionLogController _sessionLogController;
        /// <summary>
        /// Gets a lazy-loaded reference to the controller to handle client-related matters.
        /// </summary>
        public static SessionLogController ControllerSessionLog
        {
            get
            {
                if (_sessionLogController == null)
                    _sessionLogController = ControllerFactory.Create<SessionLogController>();
                return _sessionLogController;
            }
        }


        private static CommonController _commonController;
        /// <summary>
        /// Gets a lazy-loaded reference to the controller to handle client-related matters.
        /// </summary>
        public static CommonController ControllerCommon
        {
            get
            {
                if (_commonController == null)
                    _commonController = ControllerFactory.Create<CommonController>();
                return _commonController;
            }
        }
    }
}
