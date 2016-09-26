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
            public string GroupCategory { get; set; }
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

        public bool Save()
        {
            StopFileWatcher();  //we are a producer,  stop watching since we always write things
            var serializer = new SharpSerializer(false);
            if (string.IsNullOrEmpty(RepoFilePath))
                RepoFilePath = ConfigurationManager.AppSettings["MillimanGroupMap"];
            serializer.Serialize(this, RepoFilePath);
            return true;
        }

        /// <summary>
        /// Allocate and start a file watcher
        /// </summary>
        /// <param name="RepoFilePath"></param>
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

        /// <summary>
        /// We have triggered that a change happened, so reprocess the group map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void FileWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            StopFileWatcher();  //stop looking we have already been signaled for a change

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

            //following two lines corrects the issue of the group map not reloading
            //kill this instance of the group map
            KillInstance();
            //rebuild and reload it
            GetInstance();

            //this will null out the current instance and reload it with new data
            UserRepo.KillInstance();
            UserRepo.GetInstance(); //this will reload everything
        }

        /// <summary>
        /// Look to see if the file is locked by another process that may be writing it
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Turn off the file watcher for now
        /// </summary>
        public static void StopFileWatcher()
        {
            if (FileWatcher != null)
                FileWatcher.EnableRaisingEvents = false;
        }

        public static MillimanGroupMap Load()
        {
            //this gets called first
            string GroupMapFile = ConfigurationManager.AppSettings["MillimanGroupMap"];
            StartFileWatcher(GroupMapFile); // this will create the watcher if necessary
            //we need to make sure events are off while we load
            StopFileWatcher();  //don't let it run while we are loading from file

            //start loading the group map
            MillimanGroupMap Repo = null;
            var serializer = new SharpSerializer(false);
            if (System.IO.File.Exists(GroupMapFile))
            {
                Repo = serializer.Deserialize(GroupMapFile) as MillimanGroupMap;
                Repo.RepoFilePath = GroupMapFile;
            }
            FileWatcher.EnableRaisingEvents = true;
            return Repo;
        }
    }
}
