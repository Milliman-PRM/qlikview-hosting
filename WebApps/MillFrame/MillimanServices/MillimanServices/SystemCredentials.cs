using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MillimanServices
{
    public class SystemCredentials : System.Web.Services.Protocols.SoapHeader
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string AccessToken { get; set; }
        public string Owner { get; set; }
    }

}