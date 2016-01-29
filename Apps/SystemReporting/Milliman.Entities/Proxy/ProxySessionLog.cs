using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Milliman.Entities.Models
{
    [Serializable()]
    public class ProxySessionLog
    {
        public string Document { get; set; }
        public string ExitReason { get; set; }
        public string SessionStartTime { get; set; }
        public string SessionDuration { get; set; }
        public string SessionEndReason { get; set; }
        public double? CpuSpentS { get; set; }
        public string IdentifyingUser { get; set; }
        public string ClientType { get; set; }
        public string ClientAddress { get; set; }
        public string CalType { get; set; }
        public int? CalUsageCount { get; set; }

        //Goes to group table
        public string Group { get; set; }
        public string Report { get; set; }
        //Need to find out where to store this
        public string EventType { get; set; }
        //Need to find out where to store this
        public string Browser { get; set; }
        public bool IsReduced { get; set; }
        public string SessionLength { get; set; }
    }
}
