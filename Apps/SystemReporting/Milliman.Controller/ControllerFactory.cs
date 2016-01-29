using Milliman.Controller.BusinessLogic.Controller;
using System;

namespace Milliman.Controller
{
    public static class ControllerFactory
    {
        public static T Create<T>() where T : IController
        {
            IController castedValue;
            Type type = typeof(T);
            if (type == typeof(AuditLogController))
                castedValue = new AuditLogController();
            else if (type == typeof(IisLogController))
                castedValue = new IisLogController();
            else if (type == typeof(SessionLogController))
                castedValue = new SessionLogController();
            else if (type == typeof(CommonController))
                castedValue = new CommonController();
            else
                throw new NotImplementedException("Unknown/Not Implemented Controller Type");

            return (T)castedValue;
        }
    }
}
