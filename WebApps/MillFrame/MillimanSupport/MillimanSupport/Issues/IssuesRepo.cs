using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace MillimanSupport.Issues
{
    public class IssuesRepo
    {
        private static object IssueLock = new object();

        private string QualifiedRepoName = @"c:\runtimefiles\InstalledApplications\MillimanSite\IssueSupport\Issues.xml";
        public List<Issue> IssueList { get; set; }

        public IssuesRepo( )
        {
 
        }
        public void LoadIssueRepo()
        {
             IssueList = LoadList();
        }
        private List<Issue> LoadList()
        {
            List<Issue> CE = null;
            if (File.Exists(QualifiedRepoName))
            {
                lock (IssueLock)
                {
                    Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                    CE = SS.Deserialize(QualifiedRepoName) as List<Issue>; 
                }

                foreach (Issue Current in CE)
                {
                    if (Current.SendMailNotificationRequired)
                    {
                        Current.SendEmail(Current.NotificationAccount, Global.FromType.MILLIMAN_SUPPORT);
                        Current.SendMailNotificationRequired = false;
                        IssueList = CE;
                        SaveList();
                    }
                }
            }
            else
            {
                //no file so create a template file
                CE = new List<Issue>();
                CE.Add(new Issue(1000, "Account", "Notify", "TicketID", "CUID", "Description", "Attachments"));
                CE.Add(new Issue(1001, "Account", "Notify", "TicketID", "CUID", "Description", "Attachments"));
 
                IssueList = CE;
                SaveList();
            }
            return CE;
        }

        public void SaveList()
        {
            lock (IssueLock)
            {
                Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                SS.Serialize(this.IssueList, QualifiedRepoName); 
            }
        }

        public int NextTicketID()
        {
            int MaxTicketID = 0;
            foreach (Issue Current in IssueList)
            {
                if (Current.MillimanTicketID > MaxTicketID)
                    MaxTicketID = Current.MillimanTicketID;
            }
            return MaxTicketID + 1;
        }
    }
}