using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Configuration;


namespace MillimanSupport.Issues
{
    public class Issue
    {
        public enum Status { OPEN, PENDING, CLOSED, REMOVED };
        public int MillimanTicketID { get; set; }
        public string FiledOn { get; set; }
        public string FiledByAccount { get; set; }
        public string NotificationAccount { get; set; }
        public string CovisintTicketID { get; set; }
        public string CovisintUser { get; set; }
        public string Description { get; set; }
        public Status IssueStatus { get; set; }
        public string Resolution { get; set; }
        public string AttachmentFile { get; set; }
        public bool   SendMailNotificationRequired { get; set; }

        public Issue() { }
        public Issue(int _MillimanTicketID, string _FiledByAccount, string _NotificationAccount, string _CovisintTicketID, string _CovisintUser, string _Description, string _AttachmentFile)
        {
            MillimanTicketID = _MillimanTicketID;
            FiledByAccount = _FiledByAccount;
            NotificationAccount = _NotificationAccount;
            CovisintTicketID = _CovisintTicketID;
            CovisintUser = _CovisintUser;
            Description = _Description;
            AttachmentFile = _AttachmentFile;
            IssueStatus = Status.OPEN;
            FiledOn = DateTime.Now.ToString();
            SendMailNotificationRequired = false; 
        }

        public bool SendEmail(string To, MillimanSupport.Global.FromType From)
        {
            string Template = ConfigurationManager.AppSettings["EmailBodyFile"];
            string body = System.IO.File.ReadAllText( Template);
            body = body.Replace("_DATETIME_", DateTime.Now.ToString());
            body = body.Replace("_FILEDON_", FiledOn);
            body = body.Replace("_FILEDBYACCOUNT_", FiledByAccount);
            body = body.Replace("_MILLIMANTICKETID_", MillimanTicketID.ToString());
            body = body.Replace("_COVISINTTICKETID_", CovisintTicketID);
            if ( string.IsNullOrEmpty(CovisintUser) )
                body = body.Replace("_COVISINTUSER_", "[Not Provided]");
            else
                body = body.Replace("_COVISINTUSER_", CovisintUser);
            body = body.Replace("_ISSUESTATUS_", IssueStatus.ToString());
            if (string.IsNullOrEmpty(Resolution))
                body = body.Replace("_DESCRIPTION_", Description);
            else
                body = body.Replace("_DESCRIPTION_", Description + "<br><br>Resolution:" + Resolution); //tack on a resolution if we have one

            MillimanSupport.Global.SendEmail(To, From, body);
            return true;
        }
    }
}