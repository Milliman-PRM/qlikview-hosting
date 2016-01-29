using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemReporting.Entities.Proxy
{
    [Serializable()]
    public class ProxyAuditLog
    {
        public string ServerStarted { get; set; }
        public string Timestamp { get; set; }
        public string Document { get; set; }
        public string EventType { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }

        //Goes to group table
        public string Group { get; set; }
        //Goes to report table
        public string Report { get; set; }
        public bool IsReduced { get; set; }
    }
}
