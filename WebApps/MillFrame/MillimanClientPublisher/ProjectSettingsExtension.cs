using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ClientPublisher
{
    public class ProjectSettingsExtension : MillimanCommon.ProjectSettings
    {


        /// <summary>
        /// Extended so icon source can be utilzied via binding
        /// </summary>
        public string QVIconReflector
        {
            get
            {
                string Key = MillimanCommon.Utilities.ConvertStringToHex(System.IO.Path.Combine(AbsoluteProjectPath, QVThumbnail));
                return "reflector.ashx?key=" + Key;
            }
        }

        public string QVLauncher
        {
            get
            {
                string QVWVirtualPath = System.IO.Path.Combine(VirtualDirectory, QVName + ".qvw");
                string URL = "dashboard.aspx?key=" + MillimanCommon.Utilities.ConvertStringToHex(QVWVirtualPath);
                return URL;
            }
        }

        //to take a QVW offline, we change it's extension from QVW to OFFLINE,  thus there are 3 possible states
        //A QVW is present, so we are online and want to take offline
        //A OFFLINE file is present(really QVW extension renamed) an we want to take ONline
        //no QVW or OFFLine file exists, thus we are just OFFLINE waiting for a QVW to be uploaded(group is not displayed in publisher for this condition)
        static public string NotAvailable = "Offline";
        static public string IsAvailable = "Take Offline";  //set text on button to take it offline, since it is available
        static public string IsOffline = "Take Online";     //QVW is there, but in an offline state
        static public string OfflineExtension = ".OFFLINE";  //QVW has an extension of OFFLINE instead of QVW

        public string Availability
        {
            get
            {
                string QVW = System.IO.Path.Combine(AbsoluteProjectPath, QVName + ".qvw");
                string QVWOfflineFile = System.IO.Path.Combine(AbsoluteProjectPath, QVName + OfflineExtension);
                if ((System.IO.File.Exists(QVW) == false) && (System.IO.File.Exists(QVWOfflineFile) == false)) //no QVW available
                    return NotAvailable;
                else if (System.IO.File.Exists(QVWOfflineFile)) //offline file is available, so dont show QVW to end users
                    return IsOffline;
                else
                    return IsAvailable; //no QVWOfflineFile exists
            }
        }
        public string AvailabilityTooltip
        {
            get
            {
                if (string.Compare(Availability,IsOffline,true) == 0 )
                    return "Report is present, but in an offline state - click to make available to users.";
                else if (string.Compare(Availability, IsAvailable, true) == 0)
                    return "Report is available to users - click to take report offline.";
                else
                    return "Report is not present on server - a new report must be uploaded before state can be changed.";
            }
        }
        private int _ProjectIndex;

        public int ProjectIndex
        {
            get { return _ProjectIndex; }
            set { _ProjectIndex = value; }
        }

        static public ProjectSettingsExtension LoadProjectExtension(string Project)
        {
            //this need to be refactored,  the issue is we needed to extend the project setting class
            //to work well with binding,  however it has an
            ProjectSettingsExtension PSE = new ProjectSettingsExtension();
            MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(Project);
            Type Source = typeof(MillimanCommon.ProjectSettings);
            System.Reflection.PropertyInfo[] properties = Source.GetProperties();

            foreach (System.Reflection.PropertyInfo pi in properties)
            {
                System.Diagnostics.Debug.WriteLine(pi.ToString());
                object Val = pi.GetValue(PS, null);
                if (pi.GetSetMethod() != null)
                    pi.SetValue(PSE, Val, null);
            }

            return PSE;
        }
    }
}