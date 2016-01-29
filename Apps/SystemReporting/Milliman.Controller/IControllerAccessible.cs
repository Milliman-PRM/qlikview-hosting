using Milliman.Controller.BusinessLogic.Controller;

namespace Milliman.Controller
{
    public interface IControllerAccessible
    {
        AuditLogController ControllerAuditLog { get; }
        IisLogController ControllerIisLog { get; }
        SessionLogController ControllerSessionLog { get; }
        CommonController ControllerCommon { get; }
    }
}
