using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemReporting.Entities.Proxy
{
    [Serializable()]
    public class ProxyIisLog
    {
        public string UserAccessDatetime { get; set; }
        public string ClientIpAddress { get; set; }
        public string ServerIPAddress { get; set; }
        public int? PortNumber { get; set; }
        public string CommandSentMethod { get; set; }
        public string StepURI { get; set; }
        public string QueryURI { get; set; }
        public int? StatusCode { get; set; }
        public int? SubStatusCode { get; set; }
        public int? Win32StatusCode { get; set; }
        public int? ResponseTime { get; set; }
        public string UserAgent { get; set; }
        public string ClientReferer { get; set; }

        //User table
        public string User { get; set; }
        //Goes to group table
        public string Group { get; set; }
        //Need to find out where to store this
        public string EventType { get; set; }
        //Need to find out where to store this
        public string Browser { get; set; }
    }
}
