using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.Configuration;
using Polenter.Serialization;
using System.Configuration;
using System.IO;


namespace MillimanCommon
{
    public class MillimanGroupMap
    {
        public class MillimanGroups
        {
            public string MillimanGroupName { get; set; }
            public string ExernalGroupName { get; set; }
            public string FriendlyGroupName { get; set; }
            public int MaximumnUsers { get; set; }
            public MillimanGroups()
            {

            }
        }

        private Dictionary<string, MillimanGroups> _MillimanGroupDictionary = new Dictionary<string, MillimanGroups>();
        public Dictionary<string, MillimanGroups> MillimanGroupDictionary
        {
            get { return _MillimanGroupDictionary; }
            set { _MillimanGroupDictionary = value; }
        }

        private static MillimanGroupMap instance;
        private static object instance_lock = new object();
        public static MillimanGroupMap GetInstance()
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

        public MillimanGroupMap.MillimanGroups FindByExternalName(string ExternalName)
        {
            foreach (KeyValuePair<string, MillimanGroupMap.MillimanGroups> MG in MillimanGroupDictionary)
            {
                if (string.Compare(MG.Value.ExernalGroupName, ExternalName, true) == 0)
                    return MG.Value;
            }
            return null;
        }

        private string RepoFilePath { get; set; }
        private static System.IO.FileSystemWatcher FileWatcher { get; set; }

        /////
        public bool Save()
        {
            StopFileWatcher();  //we are a producer,  stop watching since we always write things
            var serializer = new SharpSerializer(false);
            if ( string.IsNullOrEmpty(RepoFilePath) )
                RepoFilePath = ConfigurationManager.AppSettings["MillimanGroupMap"]; 
            serializer.Serialize(this, RepoFilePath);
            return true;
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
            string RepoFilePath = ConfigurationManager.AppSettings["MillimanGroupMap"];
            FileInfo FI = new FileInfo(RepoFilePath);
            for (int Index = 0; Index < 30; Index++)
            {
                if (IsFileLocked(FI) == false)
                {
                    break;
                }
                System.Threading.Thread.Sleep(1000);
            }

            //this will null out the current instance and reload it with new data
            UserRepo.KillInstance();
            UserRepo.GetInstance(); //this will reload everything
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
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

        public void TestData()
        {
            string[] R = Roles.GetAllRoles();
            foreach( string S in R  )
            {
                MillimanGroups MG = new MillimanGroups();
                MG.MillimanGroupName = S;
                MG.MaximumnUsers = 0;
                MillimanGroupDictionary.Add( S, MG );
            }

        }

        public static MillimanGroupMap Load()
        {
            //un comment to generate test file
            //MillimanGroupMap MGM = new MillimanGroupMap();
            //MGM.TestData();
            //MGM.Save();

            //this gets called first
            string GroupMapFile = ConfigurationManager.AppSettings["MillimanGroupMap"];
            StartFileWatcher(GroupMapFile);
            //we need to make sure events are off while we load
            //apparently sharp deserialize mods the file in some way thus we get
            //multiple events that prompt loading
            bool EventsEnabled = FileWatcher.EnableRaisingEvents;
            FileWatcher.EnableRaisingEvents = false;
            MillimanGroupMap Repo = null;
            var serializer = new SharpSerializer(false);
            if (System.IO.File.Exists(GroupMapFile))
            {
                Repo = serializer.Deserialize(GroupMapFile) as MillimanGroupMap;
                Repo.RepoFilePath = GroupMapFile;
            }
            FileWatcher.EnableRaisingEvents = EventsEnabled; //set back to original state
            return Repo;
        }
    }
}
