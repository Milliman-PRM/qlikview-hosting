using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;

namespace MillimanSupport
{
    public class Comm
    {
        public string Execute(string address)
        {
            try
            {
                var req = WebRequest.Create(address.ToString());
                var resp = req.GetResponse();
                var sr = new StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}