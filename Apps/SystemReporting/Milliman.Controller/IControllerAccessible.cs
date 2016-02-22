using SystemReporting.Controller.BusinessLogic.Controller;

namespace SystemReporting.Controller
{
    public interface IControllerAccessible
    {
        AuditLogController ControllerAuditLog { get; }
        IisLogController ControllerIisLog { get; }
        SessionLogController ControllerSessionLog { get; }
        CommonController ControllerCommon { get; }
    }
}
