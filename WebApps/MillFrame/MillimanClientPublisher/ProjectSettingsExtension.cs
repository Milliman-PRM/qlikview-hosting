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