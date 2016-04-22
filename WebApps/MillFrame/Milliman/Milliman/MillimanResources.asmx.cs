using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace ServiceProvider
{
    /// <summary>
    /// Summary description for MillimanResources
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MillimanResources : System.Web.Services.WebService
    {
        private string MakeResource(string Title, string Description, string AbsoluteURL)
        {
            
            string  Res = "<resource>";
            Res += "<title>";
            Res += Title;
            Res += "</title>";
            Res += "<description>";
            Res += Description;
            Res += "</description>";
            Res += "<url>";
            Res += AbsoluteURL;
            Res += "</url>";
            Res += "</resource>";
            return Res;
        }

        [WebMethod]
        public string AvailableResources( string CovUserID )
        {
            //VWN:http://stackoverflow.com/questions/14933477/client-authentication-via-x509-certificates-in-asp-net
            HttpClientCertificate HCC =  HttpContext.Current.Request.ClientCertificate;
            var X509 = new System.Security.Cryptography.X509Certificates.X509Certificate2(HCC.Certificate);
           
            //look up CovUserID for user
            string Res = "<resources>";
            Res += MakeResource("Population Report", "General population analytics", "http://localhost:3125/dashboard.aspx?dashboardid=POPULATION");
            Res += MakeResource("Cost Model", "Cost model analytics", "http://localhost:3125/dashboard.aspx?dashboardid=COST_MODELS");
            Res += "</resources>";
            return Res;
        }
    }
}
