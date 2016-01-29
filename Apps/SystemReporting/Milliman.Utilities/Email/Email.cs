using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Utilities.Email
{
    /// <summary>
    /// http://referencesource.microsoft.com/#System/net/System/Net/mail/Message.cs,0800189429387e13
    /// </summary>
    public class Email
    {
        private MailMessage m;
        private Attachment attachmentItem = null;
        private AttachmentCollection attachmentsList = null;

        public Email()
        {
            m = new MailMessage();
            m.Sender = m.From = new MailAddress(ConfigurationManager.AppSettings["SendMailMessagesFromAddress"]);
            m.IsBodyHtml = true;
        }

        public bool isHTML
        {
            set
            {
                m.IsBodyHtml = value;
            }
        }

        public string to
        {
            set
            {
                m.To.Add(value);
            }
        }

        public string from
        {
            set
            {
                m.Sender = m.From = new MailAddress(value);
            }
        }

        public string subject
        {
            set
            {
                m.Subject = value;
            }
        }

        public string body
        {
            get
            {
                return body;
            }
            set
            {
                m.Body = value;
            }
        }

        public MailPriority priority
        {
            get
            {
                return (((int)priority == -1) ? MailPriority.Normal : priority);
            }
            set
            {
                m.Priority = (value);
            }
        }

        public Attachment attachment
        {
            get{
                return attachmentItem;
            }
        }

        public AttachmentCollection attachments
        {
            get { return attachmentsList; }
        }

        public void sendEmail()
        {
            try
            {
                SmtpClient sc = new SmtpClient();
                sc.Host = ConfigurationManager.AppSettings["SendMailSTMPHostAddress"];
                sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                sc.UseDefaultCredentials = false;
                sc.Port = 25;
                sc.Credentials = new System.Net.NetworkCredential(
                        ConfigurationManager.AppSettings["SendMailSMTPUserName".ToString()],
                        ConfigurationManager.AppSettings["SendMailSMTPUserPassword".ToString()]
                );
                sc.Send(m);
            }
            catch (Exception)
            {
                //Log error somehow
            }
        }
    }
}
