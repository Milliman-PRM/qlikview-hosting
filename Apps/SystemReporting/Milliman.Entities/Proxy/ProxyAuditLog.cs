using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemReporting.Entities.Proxy
{
    [Serializable()]
    public class ProxyAuditLog
    {
        public string UserAccessDatetime { get; set; }
        public string Document { get; set; }
        public string EventType { get; set; }
        public string Message { get; set; }

        //User table
        public string User { get; set; }
        //Goes to group table
        public string Group { get; set; }
        //Goes to report table
        public string Report { get; set; }
        public bool IsReduced { get; set; }
    }
}
