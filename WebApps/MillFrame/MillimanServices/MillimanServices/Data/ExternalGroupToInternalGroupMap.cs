using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milliman.Data
{
    public class ExternalGroupToInternalGroupMap
    {
       
        public class GroupMap
        {
            public string ExternalID { get; set; }
            public string InternalID { get; set; }
            public string ExternalName { get; set; } //NextGen, Covisint, etc... not really needed just sanity checking
            public string InternalName { get; set; }
            public string ExternalNameDesc { get; set; } //membership api id of user for sanity checking
            public string InternalNameDesc { get; set; } //membership api id of user for sanity checking
            public string ExternalGroupOwner { get; set; }

            public GroupMap()
            {
            }

            public GroupMap(string _ExternalID, string _InternalID, string _ExternalName, string _ExternalNameDesc, string _InternalName, string _InternalNameDesc, string _ExternalGroupOwer)
            {
                ExternalID = _ExternalID;
                InternalID = _InternalID;
                ExternalName = _ExternalName;
                ExternalNameDesc = _ExternalNameDesc;
                InternalName = _InternalName;
                InternalNameDesc = _InternalNameDesc;
                ExternalGroupOwner = _ExternalGroupOwer;
            }
        }

        public List<GroupMap> GroupsMap { get; set; }

        public ExternalGroupToInternalGroupMap()
        {
            GroupsMap = new List<GroupMap>();
        }

        public string FindInternalGroup( string ExternalGroupID )
        {
            string RetVal = string.Empty;
            foreach( GroupMap GM in GroupsMap )
            {
                if ( string.Compare(GM.ExternalID, ExternalGroupID, true ) == 0 )
                {
                    RetVal = GM.InternalID;
                    break;
                }
            }
            return RetVal;
        }

        public string FindInternalGroup(Guid ExternalGroupID)
        {
            return FindInternalGroup(ExternalGroupID.ToString());
        }

        public string FindExternalGroup(string InternalGroupID)
        {
            string RetVal = string.Empty;
            foreach (GroupMap GM in GroupsMap)
            {
                if (string.Compare(GM.InternalID, InternalGroupID, true) == 0)
                {
                    RetVal = GM.ExternalID;
                    break;
                }
            }
            return RetVal;
        }

        public Guid FindExternalGroupAsGUID(string InternalGroupID)
        {
            string ExGroupID = FindExternalGroup(InternalGroupID);
            if (string.IsNullOrEmpty(ExGroupID) == false)
            {
                Guid G = Guid.Parse(ExGroupID);
                return G;
            }
            throw new Exception("Could not convert map value to GUID for external entity call");
        }
        //factory singleton
        private static ExternalGroupToInternalGroupMap instance;
        private static object instance_lock = new object();
        public static ExternalGroupToInternalGroupMap GetInstance()
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
            serializer.Serialize(this, RepoFilePath);
            return true;
        }

        public void TestData(bool SaveData = true)
        {
            GroupsMap.Add(new GroupMap("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",  "0032Alpha01", "Alpha X", "", "", "", "NextGen"));
            GroupsMap.Add(new GroupMap("baaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",  "0032Beta01", "Beta X", "", "", "", "NextGen"));
            GroupsMap.Add(new GroupMap("caaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",   "0032Gamma01", "Gamma X", "", "", "", "NextGen"));

            if (SaveData)
                Save();
        }

        public static ExternalGroupToInternalGroupMap Load()
        {
            //uncomment to generate test file
            //ExternalGroupToInternalGroupMap G = new ExternalGroupToInternalGroupMap();
            //G.RepoFilePath = System.Configuration.ConfigurationManager.AppSettings["ExternalGroupMap"];
            //G.TestData();


            //this gets called first
            string RepoFilePath = System.Configuration.ConfigurationManager.AppSettings["ExternalGroupMap"];
            StartFileWatcher(RepoFilePath);
            //we need to make sure events are off while we load
            //apparently sharp deserialize mods the file in some way thus we get
            //multiple events that prompt loading
            bool EventsEnabled = FileWatcher.EnableRaisingEvents;
            FileWatcher.EnableRaisingEvents = false;
            ExternalGroupToInternalGroupMap Repo = null;
            var serializer = new Polenter.Serialization.SharpSerializer(false);
            if (System.IO.File.Exists(RepoFilePath))
            {
                Repo = serializer.Deserialize(RepoFilePath) as ExternalGroupToInternalGroupMap;
                Repo.RepoFilePath = RepoFilePath;
            }
            FileWatcher.EnableRaisingEvents = EventsEnabled; //set back to original state
            return Repo;
        }

        public static void StartFileWatcher(string RepoFilePath)
        {
            //start watching if not already watching
            if (FileWatcher == null)
            {
                FileWatcher = new System.IO.FileSystemWatcher(System.IO.Path.GetDirectoryName(RepoFilePath), System.IO.Path.GetFileName(RepoFilePath));
                FileWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite;
                FileWatcher.Changed += FileWatcher_Changed;
                FileWatcher.EnableRaisingEvents = true;
            }
        }

        public static void FileWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            //wait for at least 30 seconds to load
            string RepoFilePath = System.Configuration.ConfigurationManager.AppSettings["ExternalGroupMap"];
            System.IO.FileInfo FI = new System.IO.FileInfo(RepoFilePath);
            for (int Index = 0; Index < 30; Index++)
            {
                if (IsFileLocked(FI) == false)
                {
                    break;
                }
                System.Threading.Thread.Sleep(1000);
            }

            //this will null out the current instance and reload it with new data
            ExternalSystemGroups.KillInstance();
            ExternalSystemGroups.GetInstance(); //this will reload everything
        }

        public static bool IsFileLocked(System.IO.FileInfo file)
        {
            System.IO.FileStream stream = null;

            try
            {
                stream = file.Open(System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
            }
            catch (System.IO.IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static void StopFileWatcher()
        {
            if ( FileWatcher != null )
                FileWatcher.EnableRaisingEvents = false;
        }
    }
}