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
            get {
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

        //If the QVW is present in the directory, and should not be shown, there is a QVW.OFFLINE file that will
        //be in the directory along with the QVW - this indicates not to show it
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
                if (System.IO.File.Exists(QVW) == false) //no QVW available
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

                string QVWOnline = System.IO.Path.Combine(AbsoluteProjectPath, QVName + ".qvw");
                string QVWOffline = System.IO.Path.Combine(AbsoluteProjectPath, QVName + OfflineExtension);
                if (System.IO.File.Exists(QVWOffline))
                    return "Report is present, but in an offline state - click to make available to users.";
                else if (System.IO.File.Exists(QVWOnline))
                    return "Report is not available to users - click to make report available.";
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
                if ( pi.GetSetMethod() != null)
                    pi.SetValue(PSE, Val, null);
            }
            
            return PSE;
        }
    }
}