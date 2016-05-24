using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Net.Mail;
using System.Configuration;

namespace MillimanDev2
{
    /// <summary>
    /// Summary description for EmailDameon
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class EmailDameon : System.Web.Services.WebService
    {
        [WebMethod]
        public string EmailReady(string Token, out string EmailTo)
        {
            EmailTo = "";

            try
            {
                string TrustedMailRelayService = ConfigurationManager.AppSettings["TrustedMailRelayService"];
                if (string.IsNullOrEmpty(TrustedMailRelayService) == false)
                {
                    if (string.Compare(TrustedMailRelayService, HttpContext.Current.Request.UrlReferrer.ToString(), true) != -1)
                        return "";  ///no idea who this is calling me
                }
                string SMTPMailPickupDir = ConfigurationManager.AppSettings["SmtpMailPickup"];
                string[] AllToFiles = System.IO.Directory.GetFiles(SMTPMailPickupDir, "*.to");
                if ((AllToFiles != null) && (AllToFiles.Count() > 0))
                {
                    string BodyFile = AllToFiles[0].Replace(".to", ".email");
                    string Body = System.IO.File.ReadAllText(BodyFile);
                    EmailTo = System.IO.File.ReadAllText(AllToFiles[0]);

                    System.IO.File.Delete(AllToFiles[0]);
                    System.IO.File.Delete(BodyFile);

                    return Body;
                }
            }
            catch (Exception)
            {  //do nothing just don't crash
            }

            return "";
        }
    }
}
