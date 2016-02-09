using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SystemReporting.Utilities.Email
{
    /// <summary>
    /// Email Class - a wrapper class for encapsulating standard SMTP mail calls.
    /// </summary>
    public class Email : IDisposable
    {
        #region Properties
        private System.Net.Mail.MailMessage _MailMessage;
        private System.Collections.Specialized.StringCollection _TempFileList;
        private string _SMTPServer;

        private string _To;
        private string _CC;
        private string _Bcc;
        private string _From;
        private string _ReplyTo;

        private bool _Disposed;
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        internal Email()
        {
            _MailMessage = new System.Net.Mail.MailMessage();
            _TempFileList = new System.Collections.Specialized.StringCollection();
        }

        /// <summary>
        /// Constructor to allow specifying an SMTP Server
        /// </summary>
        /// <param name="emailServer">The SMTP server that mail calls will be made through</param>
        internal Email(string emailServer)
            : this() // call the default constructor first
        {
            _SMTPServer = emailServer;
        }

        /// <summary>
        /// Enum that matches up against the System.Net.Mail.MailPriority enum.
        /// </summary>
        public enum MailPriority
        {
            Normal = 0,
            Low = 1,
            High = 2
        }

        /// <summary>
        /// To recipient.
        /// </summary>
        public string To
        {
            get { return _To; }
            set { _To = value; }
        }

        /// <summary>
        /// Carbon Copy recipients.
        /// </summary>
        public string CC
        {
            get { return _CC; }
            set { _CC = value; }
        }

        /// <summary>
        /// Blind Carbon Copy recipients.
        /// </summary>
        public string Bcc
        {
            get { return _Bcc; }
            set { _Bcc = value; }
        }

        /// <summary>
        /// From Address
        /// </summary>
        public string From
        {
            get { return _From; }
            set { _From = value; }
        }

        /// <summary>
        /// Reply To Address
        /// </summary>
        public string ReplyTo
        {
            get { return _ReplyTo; }
            set { _ReplyTo = value; }
        }

        /// <summary>
        /// Priority: Normal, Low, or High.
        /// </summary>
        public MailPriority Priority
        {
            get { return (MailPriority)_MailMessage.Priority; }
            set { _MailMessage.Priority = (System.Net.Mail.MailPriority)value; }
        }

        /// <summary>
        /// Subject Line
        /// </summary>
        public string Subject
        {
            get { return _MailMessage.Subject; }
            set { _MailMessage.Subject = value; }
        }

        /// <summary>
        /// Whether or not the body is encoded as plain text or HTML.
        /// </summary>
        public bool IsBodyHtml
        {
            get { return _MailMessage.IsBodyHtml; }
            set { _MailMessage.IsBodyHtml = value; }
        }

        /// <summary>
        /// The body of the message.
        /// </summary>
        public string Body
        {
            get { return _MailMessage.Body; }
            set { _MailMessage.Body = value; }
        }

        /// <summary>
        /// Adds an attachment to the mail message.
        /// </summary>
        /// <param name="attachmentPath">File path to the attachment</param>
        public void AddAttachment(string attachmentPath)
        {
            this.AddAttachment(attachmentPath, false);
        }

        /// <summary>
        /// Adds an attachment to the mail message.  This is an overload of AddAttachment.
        /// </summary>
        /// <param name="attachmentPath">Absolute path to the attachment</param>
        /// <param name="isTempFile">Whether or not to delete the file after the message has been sent.</param>
        public void AddAttachment(string attachmentPath, bool isTempFile)
        {
            using (var attachment = new Attachment(attachmentPath))
            {
                _MailMessage.Attachments.Add(attachment);
                if (isTempFile)
                {
                    _TempFileList.Add(attachmentPath);
                }
            }
        }

        /// <summary>
        /// This method takes the properties of the mail object and builds and
        /// email message to send to the SMTP server.
        /// </summary>
        public void SendMail()
        {
            SetupAddresses(_To, _MailMessage.To);
            SetupAddresses(_CC, _MailMessage.CC);
            SetupAddresses(_Bcc, _MailMessage.Bcc);
            _MailMessage.From = new System.Net.Mail.MailAddress(_From);

            if (_ReplyTo != null && _ReplyTo != String.Empty)
            {
                _MailMessage.ReplyToList.Add(new MailAddress(_ReplyTo));
            }

            System.Net.Mail.SmtpClient client;
            if (_SMTPServer == null)
            {
                client = new SmtpClient();
            }
            else
            {
                client = new SmtpClient(_SMTPServer);
            }
            client.Credentials = (System.Net.ICredentialsByHost)System.Net.CredentialCache.DefaultCredentials;
            client.Send(_MailMessage);
            client.Dispose();
        }

        /// <summary>
        /// Implementation of IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the mail message object and deletes any attachments that
        /// are regarded as temporary files.
        /// </summary>
        /// <param name="disposing">todo: describe disposing parameter on Dispose</param>
        private void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    _MailMessage.Dispose();
                    foreach (string attachment in _TempFileList)
                    {
                        if (System.IO.File.Exists(attachment))
                        {
                            System.IO.File.Delete(attachment);
                        }
                    }
                }
                _Disposed = true;
            }
        }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~Email()
        {
            Dispose(false);
        }

        /// <summary>
        /// Takes a list of email addresses that are separated by semicolins and puts them into
        /// an MailAddressCollection object that System.Net.Mail expects.
        /// </summary>
        /// <param name="addressList">List of email addresses</param>
        /// <param name="addressField">The address collection to put addresses into</param>
        private static void SetupAddresses(string addressList, System.Net.Mail.MailAddressCollection addressField)
        {
            if (addressList != null && addressList.Trim() != String.Empty)
            {
                foreach (string address in addressList.Split(new Char[] { ';' }))
                {
                    if (address != String.Empty)
                    {
                        addressField.Add(address);
                    }
                }
            }
        }
        /// <summary>
        /// This method clear all the email properties.
        /// </summary>
        public void Clear()
        {
            _Bcc = null;
            _CC = null;
            _From = null;
            _To = null;
            _ReplyTo = null;
            _MailMessage = new System.Net.Mail.MailMessage();
            _TempFileList = new System.Collections.Specialized.StringCollection();
        }
    }
}
