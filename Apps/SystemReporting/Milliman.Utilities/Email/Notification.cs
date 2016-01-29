using System;
using System.Net.Mail;
using System.Configuration;
using System.IO;

namespace Milliman.Utilities.Email
{
    public class Notification
    {
        #region Variabls
        protected string _ToReciepients = null;
        protected string _Subject = null;
        protected string _TemplatesPath = null;
        protected bool _debugmode = false;
        #endregion

        /// <summary>
        /// The Constructor Function
        /// </summary>
        /// <param name="EmailHeaderSubject">Email Header Subject</param>
        /// <param name="TemplatesPath">Emails Files Templates</param>
        public Notification(string toAddress, string emailHeaderSubject, string templatesPath)
        {
            _ToReciepients = toAddress;
            _Subject = emailHeaderSubject;
            _TemplatesPath = templatesPath;
        }
        #region Email File Format
        /// <summary>
        /// This function will read the content of a file name
        /// </summary>
        /// <param name="FileName">File Name</param>
        /// <returns>String: Containing the Entire content of the file</returns>
        protected string ReadEmailFile(string FileName)
        {
            string retVal = null;
            try
            {
                //setting the file name path
                string path = _TemplatesPath + FileName;

                File.File file = new File.File();
                //check if the file exists in the location.
                if (!file.Exists(path))
                    throw new Exception("Could Not Find the file : " + FileName + " in the location " + _TemplatesPath); // throw an exception here.


                //start reading the file. i have used Encoding 1256 to support arabic text also.
                StreamReader sr = new StreamReader(@path, System.Text.Encoding.GetEncoding(1256));
                retVal = sr.ReadToEnd(); // getting the entire text from the file.
                sr.Close();
            }


            catch (Exception ex)
            {
                throw new Exception("Error Reading File." + ex.Message);
            }
            return retVal;
        }

        /// <summary>
        /// This function will return the default email header specified in the "email_header.txt"
        /// </summary>
        /// <returns>String: Contains the entire text of the "email_header.txt"</returns>
        protected string emailheader()
        {
            File.File file = new File.File();
            string retVal = null;
            if (file.Exists(_TemplatesPath + "email_header.txt"))
            {
                retVal = ReadEmailFile("email_header.txt");
            }
            else
                throw new Exception("you should have a file called 'email_header.txt' in the location :" + _TemplatesPath);
            return retVal;
        }

        /// <summary>
        /// This function will return the default email footer specified in the "email_footer.txt"
        /// </summary>
        /// <returns>String: Contains the entire text of the "email_footer.txt"</returns>
        protected string emailfooter()
        {
            File.File file = new File.File();
            string retVal = null;
            if (file.Exists(_TemplatesPath + "email_footer.txt"))
                retVal = ReadEmailFile("email_footer.txt");
            else
                throw new Exception("you should have a file called 'email_footer.txt' in the location :" + _TemplatesPath);

            return retVal;
        }
        #endregion
        #region Email Notification With Reciepients from Configs or DB

        public void SendNotification(string Message, string EmailErr)
        {
            Email Email = new Email();

            string sMessage = GetMessage(Message);

            var _email = Email;
            _email.from = ConfigurationManager.AppSettings["FromAddress"];
            _email.to = ConfigurationManager.AppSettings["ErrorAddress"];
            _email.priority = MailPriority.High;
            _email.subject = "No Report Created" + DateTime.Now.ToLongDateString();
            _email.body = sMessage.ToString() + Message;
            _email.sendEmail();

        }

        public void SendNotification(string Message, string EmailErr, string Filename)
        {
            Email Email = new Email();
            string sMessage = GetMessage(Message);

            var _email = Email;
            _email.from = ConfigurationManager.AppSettings["FromAddress"];
            _email.to = ConfigurationManager.AppSettings["ErrorAddress"];
            _email.priority = MailPriority.High;
            _email.subject = Filename + DateTime.Now.ToLongDateString();
            _email.body = sMessage.ToString() + Message;
            _email.sendEmail();
        }

        public void SendNotification(string FileName)
        {
            Email Email = new Email();
            string sMessage = GetMessage(string.Empty);
            Attachment attachment = new Attachment(FileName);

            var _email = Email;
            _email.from = ConfigurationManager.AppSettings["FromAddress"];
            _email.to = ConfigurationManager.AppSettings["ErrorAddress"];
            _email.priority = MailPriority.High;
            _email.subject = "No Report Created" + DateTime.Now.ToLongDateString();
            _email.body = sMessage.ToString();
            _email.attachment.Name = FileName + DateTime.Now.ToLongDateString() + ".msg";
            _email.attachments.Add(attachment);
            _email.sendEmail();
        }

        public string GetMessage(string sMessage)
        {

            System.Text.StringBuilder Message = new System.Text.StringBuilder();
            Message.Append(Environment.NewLine + Environment.NewLine);
            Message.Append("**************************************************************************************");
            Message.Append(Environment.NewLine + Environment.NewLine);
            Message.Append(" This is automated Emial. Please do not Reply." + Environment.NewLine);
            Message.Append(" If you have questions, please call us." + Environment.NewLine);
            Message.Append("******************************************************************" + Environment.NewLine);
            Message.Append(Environment.NewLine + Environment.NewLine);
            Message.Append(Environment.NewLine + Environment.NewLine);
            Message.Append(" Please review the file." + Environment.NewLine);

            string ReturnValue = null;
            ReturnValue = Message.ToString();

            return ReturnValue;
        }

        #endregion

        #region Email Notification With Reciepients

        /// <summary>
        /// this function will send email. it will read the mail setting from the web.config
        /// </summary>
        /// <param name="SenderEmail">Sender Email ID</param>
        /// <param name="SenderName">Sender Name</param>
        /// <param name="Recep">Recepient Email ID</param>
        /// <param name="cc">CC ids</param>
        /// <param name="email_title">Email Subject</param>
        /// <param name="email_body">Email Body</param>
        protected void SendEmail(string SenderEmail, string SenderName, string Recep, string cc, string email_title, string email_body)
        {
            // creating email message
            MailMessage msg = new MailMessage();
            msg.IsBodyHtml = true;// email body will allow html elements

            // setting the Sender Email ID
            msg.From = new MailAddress(SenderEmail, SenderName);

            // adding the Recepient Email ID
            msg.To.Add(Recep);

            // add CC email ids if supplied.
            if (!string.IsNullOrEmpty(cc))
                msg.CC.Add(cc);

            //setting email subject and body
            msg.Subject = email_title;
            msg.Body = email_body;

            //create a Smtp Mail which will automatically get the smtp server details from web.config mailSettings section
            SmtpClient SmtpMail = new SmtpClient();

            // sending the message.
            SmtpMail.Send(msg);
        }

        /// <summary>
        /// This function will send the email notification by reading the email template and substitute the arguments
        /// </summary>
        /// <param name="EmailTemplateFile">Email Template File</param>
        /// <param name="SenderEmail">Sender Email</param>
        /// <param name="SenderName">Sender Name</param>
        /// <param name="RecepientEmail">Recepient Email ID</param>
        /// <param name="CC">CC IDs</param>
        /// <param name="Subject">EMail Subject</param>
        /// <param name="Args">Arguments</param>
        /// <returns>String: Return the body of the email to be send</returns>
        public string SendNotificationEmail(string EmailTemplateFile, string SenderEmail, string SenderName, 
                                            string RecepientEmail, string CC, string Subject, params string[] Args)
        {
            string retVal = null;

            //reading the file
            string FileContents = ReadEmailFile(EmailTemplateFile);

            //conactinate the email Header  and Email Body and Email Footer
            string emailBody = emailheader() + FileContents + emailfooter();

            //setting formatting the string
            retVal = string.Format(emailBody, Args);

            try
            {
                //check if we are in debug mode or not. to send email
                if (!_debugmode)
                    SendEmail(SenderEmail, SenderName, RecepientEmail, CC,
                                    (!string.IsNullOrEmpty(Subject) ? Subject : _Subject), retVal);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retVal;
        }
   
        #endregion

    }
}
