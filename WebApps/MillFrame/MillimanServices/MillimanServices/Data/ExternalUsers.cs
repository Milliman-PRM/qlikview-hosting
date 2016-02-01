using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Milliman.Data
{
    public class ExternalUsers
    {
        public class ExternalReport
        {
            private string _OrginGroupName;
            public string OrginGroupName
            {
                get { return _OrginGroupName; }
                set
                {
                    _OrginGroupName = value;
                    IsDirty = true;
                }
            }
            private string _ReportID;
            public string ReportID
            {
                get { return _ReportID; }
                set
                {
                    _ReportID = value;
                    IsDirty = true;
                }
            }
            private List<string> _ProviderIDs;
            public List<string> ProviderIDs
            {
                get { return _ProviderIDs; }
                set
                {
                    _ProviderIDs = value;
                    IsDirty = true;
                }
            }

            [Polenter.Serialization.ExcludeFromSerialization]
            public bool IsDirty { get; set; }

            public ExternalReport()
            {
                ProviderIDs = new List<string>();
                IsDirty = false;
            }

            public ExternalReport Clone()
            {
                ExternalReport ER = new ExternalReport();
                ER.ReportID = ReportID;
                ER.ProviderIDs = new List<string>();
                foreach (string PI in ProviderIDs)
                    ER.ProviderIDs.Add(PI);

                return ER;
            }
        }
        public class ExternalUser
        {
            private string _Owner;  //NextGen, Covisint, etc... not really needed just sanity checking
            public string Owner
            {
                get { return _Owner; }
                set
                {
                    _Owner = value;
                    _IsDirty = true;
                }
            }
            private string _UserName;
            public string UserName
            {
                get { return _UserName; }
                set
                {
                    _UserName = value;
                    _IsDirty = true;
                }
            }
            private string _UserID; //membership api id of user for sanity checking
            public string UserID
            {
                get { return _UserID; }
                set
                {
                    _UserID = value;
                    _IsDirty = true;
                }
            }
            private Dictionary<string, ExternalReport> _Reports;
            public Dictionary<string, ExternalReport> Reports
            {
                get { return _Reports; }
                set
                {
                    _Reports = value;
                    _IsDirty = true;
                }
            }
            private bool _IsDirty;

            [Polenter.Serialization.ExcludeFromSerialization]
            public bool IsDirty
            {
                get
                {
                    if (_IsDirty)
                        return true;
                    foreach (KeyValuePair<string, ExternalReport> ER in Reports)
                    {
                        if (ER.Value.IsDirty)
                            return true;
                    }
                    return false;
                }
                set
                {
                    _IsDirty = value;
                    //since flag is global to ExternalReports - make children the same
                    foreach (KeyValuePair<string, ExternalReport> ExReport in Reports)
                        ExReport.Value.IsDirty = value;
                }
            }

            public ExternalUser()
            {
                Reports = new Dictionary<string, ExternalReport>();
                _IsDirty = false;
            }
        }

        private Dictionary<string, ExternalUser> _Users = new Dictionary<string, ExternalUser>();

        public Dictionary<string, ExternalUser> Users
        {
            get { return _Users; }
            set { _Users = value; }
        }

        //added these to make sure access to our dictionary is threadsafe
        private static object useraccess_lock = new object();
        public bool ContainsKey(string UserName)
        {
            lock (useraccess_lock)
            {
                return Users.ContainsKey(UserName);
            }
        }

        public void AddUser(ExternalUser EX)
        {
            lock (useraccess_lock)
            {
                EX.IsDirty = true;
                Users.Add(EX.UserName, EX);
            }
        }

        public void RemoveUser(string UserName)
        {
            lock (useraccess_lock)
            {
                Users.Remove(UserName);
            }
        }
        public ExternalUser GetUser(string UserName)
        {
            lock (useraccess_lock)
            {
                return Users[UserName];
            }
        }
        //factory singleton
        private static ExternalUsers instance;
        private static object instance_lock = new object();
        public static ExternalUsers GetInstance()
        {
            lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
            {
                if (instance == null)
                    instance = Load();
                return instance;
            }
        }
        public static void KillInstance()
        {
            lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
            {
                if (instance != null)
                    instance = null;
            }
        }
        //end singleton

        private string RepoFilePath { get; set; }
        private static System.IO.FileSystemWatcher FileWatcher { get; set; }

        public bool Save()
        {
            StopFileWatcher();  //we are a producer,  stop watching since we always write things
            var serializer = new Polenter.Serialization.SharpSerializer(false);
            string RepoFilePath = System.Configuration.ConfigurationManager.AppSettings["ExternalUsers"];
            string RepoDirectory = Path.Combine(Path.GetDirectoryName(RepoFilePath), Path.GetFileNameWithoutExtension(RepoFilePath));

            if (Directory.Exists(RepoDirectory) == false)
                Directory.CreateDirectory(RepoDirectory);

            lock (useraccess_lock)
            {
                foreach (KeyValuePair<string, ExternalUser> User in _Users)
                {
                    if (User.Value.IsDirty)
                    {
                        User.Value.IsDirty = false;
                        string QualifiedFile = Path.Combine(RepoDirectory, MillimanCommon.Utilities.ConvertStringToHex(User.Value.UserName) + ".xuser");
                        if (File.Exists(QualifiedFile))
                            File.Delete(QualifiedFile);
                        serializer.Serialize(User.Value, QualifiedFile);
                    }
                }
            }
            return true;
        }

        //public void TestData(bool SaveData = true)
        //{
        //    ExternalUser EU1 = new ExternalUser();
        //    EU1.UserName = "van.nanney@milliman.com";
        //    EU1.UserID = "12345";
        //    EU1.Owner = "NextGen";
        //    EU1.Reports = new Dictionary<string, ExternalReport>();
        //    ExternalReport ER1 = new ExternalReport();
        //    ER1.ReportID = "CARECOORD";
        //    ER1.OrginGroupName = "xxx-xxxx-xxxx-xxxx";
        //    ER1.ProviderIDs = new List<string>();
        //    ER1.ProviderIDs.Add("provider_8");
        //    ER1.ProviderIDs.Add("provider_12");
        //    ER1.ProviderIDs.Add("provider_15");
        //    EU1.Reports.Add(ER1.ReportID, ER1);
        //    _Users.Add(EU1.UserName, EU1);

        //    ExternalUser EU2 = new ExternalUser();
        //    EU2.UserName = "hobbes.nanney@milliman.com";
        //    EU2.UserID = "876554";
        //    EU2.Owner = "NextGen";
        //    EU2.Reports = new Dictionary<string, ExternalReport>();
        //    ExternalReport ER2 = new ExternalReport();
        //    ER2.ReportID = "CARECOORD";
        //    ER2.OrginGroupName = "yyy-yyyy-yyyy-yyyy";
        //    ER2.ProviderIDs = new List<string>();
        //    ER2.ProviderIDs.Add("provider_1");
        //    ER2.ProviderIDs.Add("provider_2");
        //    EU2.Reports.Add(ER2.ReportID, ER2);
        //    _Users.Add(EU2.UserName, EU2);

        //    if (SaveData)
        //        Save();
        //}

        public static ExternalUsers Load()
        {
            string RepoFilePath = System.Configuration.ConfigurationManager.AppSettings["ExternalUsers"];
            string RepoDirectory = Path.Combine(Path.GetDirectoryName(RepoFilePath), Path.GetFileNameWithoutExtension(RepoFilePath));

            if (Directory.Exists(RepoDirectory) == false)
                Directory.CreateDirectory(RepoDirectory);

            ExternalUsers EUS = new ExternalUsers();
            ExternalUser EU = null;
            var serializer = new Polenter.Serialization.SharpSerializer(false);

            string[] AllUserFiles = Directory.GetFiles(RepoDirectory);
            foreach (string UserFile in AllUserFiles)
            {
                EU = serializer.Deserialize(UserFile) as ExternalUser;
                if (EU == null)
                {
                    Directory.Delete(UserFile);
                }
                else
                {
                    //make sure something coming from disk isnt marked as dirty
                    EU.IsDirty = false;
                    foreach ( KeyValuePair<string, ExternalReport> ER in EU.Reports)
                        ER.Value.IsDirty = false;

                    EUS.AddUser(EU);
                }
            }

            return EUS;
        }

        //public static ExternalUsers Load()
        //{
        //    //uncomment to generate templates
        //    //ExternalUsers EU = new ExternalUsers();
        //    //EU.RepoFilePath = System.Configuration.ConfigurationManager.AppSettings["ExternalUsers"];
        //    //EU.TestData();

        //    //this gets called first
        //    string RepoFilePath = System.Configuration.ConfigurationManager.AppSettings["ExternalUsers"];
        //    StartFileWatcher(RepoFilePath);
        //    //we need to make sure events are off while we load
        //    //apparently sharp deserialize mods the file in some way thus we get
        //    //multiple events that prompt loading
        //    bool EventsEnabled = FileWatcher.EnableRaisingEvents;
        //    FileWatcher.EnableRaisingEvents = false;
        //    ExternalUsers Repo = null;
        //    var serializer = new Polenter.Serialization.SharpSerializer(false);
        //    if (System.IO.File.Exists(RepoFilePath))
        //    {
        //        Repo = serializer.Deserialize(RepoFilePath) as ExternalUsers;
        //        Repo.RepoFilePath = RepoFilePath;
        //    }
        //    FileWatcher.EnableRaisingEvents = EventsEnabled; //set back to original state
        //    return Repo;
        //}

        public static void StartFileWatcher(string RepoFilePath)
        {
            //start watching if not already watching
            if (FileWatcher == null)
            {
                FileWatcher = new System.IO.FileSystemWatcher(System.IO.Path.GetDirectoryName(RepoFilePath), "*.xuser");
                FileWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite;
                FileWatcher.Changed += FileWatcher_Changed;
                FileWatcher.EnableRaisingEvents = true;
            }
        }

        public static void FileWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            //this only happens when something external mods the directory, so we can afford to kill and renew
            KillInstance();
            GetInstance(); //this wil reload the us

        }

        public static void StopFileWatcher()
        {
            if (FileWatcher != null)
                FileWatcher.EnableRaisingEvents = false;
        }
    }
}