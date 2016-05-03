using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace MillimanDev2.Announcements
{
    public class Announcement
    {
        public enum AudienceType { All, SpecificUser, SpecificGroup, SpecificQVFile };

        private AudienceType _IntendedAudience;

        public Announcement()
        {

        }

        public AudienceType IntendedAudience
        {
            get { return _IntendedAudience; }
            set { _IntendedAudience = value; }
        }
        private DateTime _PublishedOn;

        public DateTime PublishedOn
        {
            get { return _PublishedOn; }
            set { _PublishedOn = value; }
        }
        private DateTime _ExpiresOn;  //no long displayed and is removed from list

        public DateTime ExpiresOn
        {
            get { return _ExpiresOn; }
            set { _ExpiresOn = value; }
        }
        private string _Icon;

        public string Icon
        {
            get { return _Icon; }
            set { _Icon = value; }
        }
        private string _Message;

        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        public string ToMessage()
        {
            StringBuilder SB = new StringBuilder(@"<tr>");
            if (string.IsNullOrEmpty(Icon) == true)
            {
                SB.Append(@"<td style='border-style:solid;border-color:#336666; border-width:1px 1px 1px 1px' title='Published On: " + PublishedOn.ToLongDateString() + "'>");
                SB.Append(Message);
                SB.Append(@"</td>");
            }
            else
            {
                SB.Append(@"<td style='border-style:solid;border-color:#336666; border-width:1px 1px 1px 1px' title='Published On: " + PublishedOn.ToLongDateString() + "'>");
                
                SB.Append(@"<table>");
                SB.Append(@"<tr>");
                SB.Append(@"<td>");
                SB.Append(@"<img src='" + Icon + "'/>");
                SB.Append("</td>");
                SB.Append(@"<td style='text-align:left'>");
                SB.Append(Message);
                SB.Append(@"</td");
                SB.Append(@"</tr>");
                SB.Append(@"</table>");
                SB.Append(@"</td>");
            }
            SB.Append(@"</tr>");
            return SB.ToString();
        }

    }

    public class Announcements
    {
        private static string AnnouncementsFile = "Announcements.xml";

        private List<Announcement> _CurrentAnnouncements = new System.Collections.Generic.List<Announcement>();  //always create the list

        public Announcements()
        {

        }

        public List<Announcement> CurrentAnnouncements
        {
            get { return _CurrentAnnouncements; }
            set { _CurrentAnnouncements = value; }
        }

        public void Save()
        {
            try
            {
                string PathFilename = System.Web.HttpContext.Current.Server.MapPath(@"Announcements\" + AnnouncementsFile);
                if (System.IO.File.Exists(PathFilename))
                {
                    System.IO.File.Delete(PathFilename);
                }
                Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                SS.Serialize(this, PathFilename);
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "", ex);
            }
        }

        /// <summary>
        /// load an instance of anno9uncements
        /// </summary>
        /// <returns></returns>
        static public Announcements Load()
        {
            try
            {
                string PathFilename = System.Web.HttpContext.Current.Server.MapPath(@"Announcements\" + AnnouncementsFile);
                if (System.IO.File.Exists(PathFilename))
                {
                    Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                    Announcements _Announcements = SS.Deserialize(PathFilename) as Announcements;
                    AnnouncementCleaner(ref _Announcements);
                    return  _Announcements;
                }
            }
            catch (Exception ex)
            {
              MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "", ex);
            }
            return null;
        }

        /// <summary>
        /// Remove all the entries that have expired and save back to file
        /// </summary>
        /// <param name="_Announcements"></param>
        static public void AnnouncementCleaner( ref Announcements _Announcements )
        {
            bool Updated = false;
            if (_Announcements == null)
                return;

            for (int Index = _Announcements.CurrentAnnouncements.Count - 1; Index >= 0; Index--)
            {
                if (DateTime.Now > _Announcements.CurrentAnnouncements[Index].ExpiresOn)
                {
                    _Announcements.CurrentAnnouncements.RemoveAt(Index);
                    Updated = true;
                }
            }

            if (Updated)
                _Announcements.Save();  //save changes back
        }

        /// <summary>
        /// call to create a template with 2 entries
        /// </summary>
        static public void CreateTemplate()
        {
            Announcements Template = new Announcements();
            Announcement Announce1 = new Announcement();
            Announce1.IntendedAudience = Announcement.AudienceType.All;
            Announce1.ExpiresOn = DateTime.Now.AddMonths(1);
            Announce1.Icon = @"Info-blue.png";
            Announce1.Message = @"I need a server with a bit more horsepower.";
            Announce1.PublishedOn = DateTime.Now;
            Template.CurrentAnnouncements.Add(Announce1);

            Announcement Announce2 = new Announcement();
            Announce2.IntendedAudience = Announcement.AudienceType.All;
            Announce2.ExpiresOn = DateTime.Now.AddMonths(2);
            Announce2.Icon = @"warning-blue.png";
            Announce2.Message = @"What is that crunching sound? <br>  I think we need an overhaul or at least a trip to the shop.";
            Announce2.PublishedOn = DateTime.Now;
            Template.CurrentAnnouncements.Add(Announce2);

            Template.Save();
        }
    }
}