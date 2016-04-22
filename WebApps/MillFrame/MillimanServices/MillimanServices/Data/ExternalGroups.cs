using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milliman.Data
{
    /// <summary>
    /// This class houses a map that reflects which group are externally accessable
    /// </summary>
    public class ExternalSystemGroups
    {
        public class ExternalSystemGroup
        {
            public string Owner { get; set; }  //NextGen, Covisint, etc....
            public string Name { get; set; }
            public string Description { get; set; }
            public bool Enabled { get; set; }
            public string InternalGroupID { get; set; }

            public ExternalSystemGroup()
            {

            }
        }

        private Dictionary<string, ExternalSystemGroup> _NextGenAssociatedGroups = new Dictionary<string, ExternalSystemGroup>();

        public Dictionary<string, ExternalSystemGroup> NextGenAssociatedGroups
        {
            get { return _NextGenAssociatedGroups; }
            set { _NextGenAssociatedGroups = value; }
        }

        /// <summary>
        /// help functions to map to and from external/internal ids
        /// </summary>
        /// <param name="ExternalGroupName"></param>
        /// <returns></returns>
        public string FindInternalGroup(string ExternalGroupName)
        {
            string RetVal = string.Empty;
            if ( NextGenAssociatedGroups.ContainsKey(ExternalGroupName) )
                 RetVal = NextGenAssociatedGroups[ExternalGroupName].InternalGroupID;
            return RetVal;
        }

        public string FindInternalGroup(Guid ExternalGroupName)
        {
            return FindInternalGroup(ExternalGroupName.ToString());
        }

        public string FindExternalGroup(string InternalGroupName)
        {
            string RetVal = string.Empty;
            foreach ( KeyValuePair<string,ExternalSystemGroup> ESG in NextGenAssociatedGroups )
            {
                if (string.Compare(ESG.Value.InternalGroupID, InternalGroupName, true) == 0)
                {
                    RetVal = ESG.Key;
                    break;
                }
            }
            return RetVal;
        }

        public Guid FindExternalGroupAsGUID(string InternalGroupName)
        {
            string ExGroupName = FindExternalGroup(InternalGroupName);
            if (string.IsNullOrEmpty(ExGroupName) == false)
            {
                Guid G = Guid.Parse(ExGroupName);
                return G;
            }
            throw new Exception("Could not convert map value to GUID for external entity call");
        }

        //factory singleton
        private static ExternalSystemGroups instance;
        private static object instance_lock = new object();
        public static ExternalSystemGroups GetInstance()
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
            ExternalSystemGroup ESG = new ExternalSystemGroup();
            ESG.Name = "xxx-xxxx-xxxx-xxxx";
            ESG.Owner = "NextGen";
            ESG.Enabled = true;
            ESG.InternalGroupID = "0032Alpha01";
            NextGenAssociatedGroups.Add(ESG.Name, ESG);

            ExternalSystemGroup ESG2 = new ExternalSystemGroup();
            ESG2.Name = "yyy-yyyy-yyyy-yyyy";
            ESG2.Owner = "NextGen";
            ESG2.Enabled = false;
            ESG2.InternalGroupID = "0032Beta01";
            NextGenAssociatedGroups.Add(ESG2.Name, ESG2);

            ExternalSystemGroup ESG3 = new ExternalSystemGroup();
            ESG3.Name = "zzz-zzzz-zzzz-zzzz";
            ESG3.Owner = "NextGen";
            ESG3.Enabled = false;
            ESG3.InternalGroupID = "0032Gamma01";

            NextGenAssociatedGroups.Add(ESG3.Name, ESG3);

            if (SaveData)
                Save();

        }

        public static ExternalSystemGroups Load()
        {
            //uncomment to generate templates

            //ExternalSystemGroups ESG = new ExternalSystemGroups();
            //ESG.RepoFilePath = System.Configuration.ConfigurationManager.AppSettings["ExternalGroups"];
            //ESG.TestData();

            //this gets called first
            string RepoFilePath = System.Configuration.ConfigurationManager.AppSettings["ExternalGroups"];
            StartFileWatcher(RepoFilePath);
            //we need to make sure events are off while we load
            //apparently sharp deserialize mods the file in some way thus we get
            //multiple events that prompt loading
            bool EventsEnabled = FileWatcher.EnableRaisingEvents;
            FileWatcher.EnableRaisingEvents = false;
            ExternalSystemGroups Repo = null;
            var serializer = new Polenter.Serialization.SharpSerializer(false);
            if (System.IO.File.Exists(RepoFilePath))
            {
                Repo = serializer.Deserialize(RepoFilePath) as ExternalSystemGroups;
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
            string RepoFilePath = System.Configuration.ConfigurationManager.AppSettings["ExternalGroups"];
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