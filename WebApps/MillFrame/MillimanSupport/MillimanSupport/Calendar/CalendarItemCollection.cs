using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace MillimanSupport.Calendar
{
    public class CalendarItemCollection
    {
        private static object CalendarLock = new object();

        private string QualifiedRepoName = @"c:\runtimefiles\InstalledApplications\MillimanSite\IssueSupport\Calendar.xml";
        public List<CalendarItem> EventList { get; set; }

        public CalendarItemCollection()
        {
 
        }
        public void LoadEventList()
        {
             EventList = LoadList();
        }
        private List<CalendarItem> LoadList()
        {
            List<CalendarItem> CE = null;
            if (File.Exists(QualifiedRepoName))
            {
                lock (CalendarLock)
                {
                    Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                    CE = SS.Deserialize(QualifiedRepoName) as List<CalendarItem>;
                    
                }
            }
            else
            {
                //no file so create a template file
                CE = new List<CalendarItem>();
                CE.Add(new CalendarItem("Subject", false, false, false, 10, "TimesNewRoman", System.Drawing.Color.Black, System.Drawing.Color.White, "Tooltip message", DateTime.Now, DateTime.Now));
 
                EventList = CE;
                SaveList();
            }
            return CE;
        }

        public void SaveList()
        {
            lock (CalendarLock)
            {
                Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                SS.Serialize(this.EventList, QualifiedRepoName);
                
            }
        }
    }
}