using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Net;
using System.Net.Mail;
using SendGridMail;
using SendGridMail.Transport;

namespace MillimanSupport
{
    /// <summary>
    /// Summary description for MillimanRelay
    /// </summary>
    [WebService(Namespace = "http://PRM.Milliman.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MillimanRelay : System.Web.Services.WebService
    {
        //we only accept conntections from these IP addresses
        private List<string> IPs = null;
        private bool IsValidConnection()
        {
            if (IPs == null)
            {
                IPs = new List<string>();
                string ValidIPS = System.Configuration.ConfigurationManager.AppSettings["ValidIPs"];
                IPs.AddRange(ValidIPS.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList());
            }
            return (IPs.Contains(HttpContext.Current.Request.UserHostAddress) || (IPs.Contains(HttpContext.Current.Request.Url.Host)));
        }

        [WebMethod(EnableSession = true)]
        public bool Relay(string[] To, string From, string Subject, string Body, bool IsHTML = true, string[] CC = null, string[] BCC = null)
        {
            try
            {
                SendGrid myMessage = SendGrid.GetInstance();
                myMessage.AddTo(To);
                myMessage.From = new MailAddress(From, From);
                myMessage.Subject = Subject;
                if (IsHTML)
                    myMessage.Html = Body;
                else
                    myMessage.Text = Body;

                if (CC != null)
                    myMessage.AddCc(CC);
                if (BCC != null)
                    myMessage.AddBcc(BCC);

                // Create credentials, specifying your user name and password.
                var credentials = new NetworkCredential("azure_914bd8e14f88e8ae1175362418c43c34@azure.com", "bvbsyxsw");

                // Create an SMTP transport for sending email.
                var transportSMTP = SMTP.GetInstance(credentials);

                // Send the email.
                transportSMTP.Deliver(myMessage);

                return true;
                
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Warning, ex.ToString());
            }
            return false;
        }

    }
}
