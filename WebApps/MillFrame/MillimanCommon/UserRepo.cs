using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.Configuration;
using Polenter.Serialization;
using System.Configuration;
using System.IO;

namespace MillimanCommon
{


    /// <summary>
    /// Summary description for UserRepo
    /// </summary>
    public partial class UserRepo
    {
        //factory singleton
        private static UserRepo instance;
        private static object instance_lock = new object();
        public static UserRepo GetInstance()
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

        private string ID = Guid.NewGuid().ToString();

        public class QVEntity
        {
            private string _QualifiedPathName;
            public string QualifiedPathName
            {
                get { return _QualifiedPathName; }
                set { _QualifiedPathName = value; }
            }

            private bool _RequiresPatientContext = false;
            public bool RequiresPatientContext
            {
                get { return _RequiresPatientContext; }
                set { _RequiresPatientContext = value; }
            }

            private string _SpecialParams;
            public string SpecialParams
            {
                get { return _SpecialParams; }
                set { _SpecialParams = value; }
            }

            private bool _IsRole = false;
            public bool IsRole
            {
                get { return _IsRole; }
                set { _IsRole = value; }
            }
            private bool _IsDirectory = false;
            public bool IsDirectory
            {
                get { return _IsDirectory; }
                set { _IsDirectory = value; }
            }
            public QVEntity() { }
            public QVEntity(string PathFilename, bool PatientContextRequired, string SpecialParameters, bool EntityIsRole, bool EntityIsDirectory = false)
            {
                QualifiedPathName = PathFilename.Replace('/', '\\'); 
                RequiresPatientContext = PatientContextRequired;
                SpecialParams = SpecialParameters;
                IsRole = EntityIsRole;
                IsDirectory = EntityIsDirectory;
            }

        }

        //a list of all files we can access per user

        private Dictionary<string, List<QVEntity>> UserToQVMap = new Dictionary<string, List<QVEntity>>(StringComparer.CurrentCultureIgnoreCase);  //user ids to QV files
        public Dictionary<string, List<QVEntity>> UserMap
        {
            get { return UserToQVMap; }
            set { UserToQVMap = new Dictionary<string, List<QVEntity>>(value, StringComparer.CurrentCultureIgnoreCase); }
        }

        //a list of all files within a role
        private Dictionary<string, List<QVEntity>> RoleToQVMap = new Dictionary<string, List<QVEntity>>(StringComparer.CurrentCultureIgnoreCase);  //map roles to QV files
        public Dictionary<string, List<QVEntity>> RoleMap
        {
            get { return RoleToQVMap; }
            set { RoleToQVMap = new Dictionary<string, List<QVEntity>>(value, StringComparer.CurrentCultureIgnoreCase); }
        }

        public UserRepo()
        {

           
        }


        #region Infrastructure
        private bool AddIfNotFound(string EntityName, StringCollection Items, Dictionary<string, List<QVEntity>> Collection, bool RequiresPatientContext, string SpecialParms, bool IsRole)
        {
            foreach (string ItemName in Items)
            {
                //make sure we have any entry
                List<QVEntity> QVs = null;
                if (Collection.ContainsKey(ItemName))
                {
                    QVs = Collection[ItemName];
                }
                else
                {
                    QVs = new List<QVEntity>();
                    Collection.Add(ItemName, QVs);
                }

                //now look to make sure we do not already have this in the list
                bool Found = false;
                foreach (QVEntity QV in QVs)
                {
                    if (string.Compare(EntityName.Replace('/','\\'), QV.QualifiedPathName.Replace('/','\\'), true) == 0)
                    {
                        Found = true;
                        break;
                    }
                }
                if (Found == false)
                    QVs.Add(new QVEntity(EntityName.Replace('/','\\'), RequiresPatientContext, SpecialParms, IsRole));
            }
            return true;
        }

        /// <summary>
        /// This routine will consume a directory or path/filename and its configuration xml 
        /// </summary>
        /// <param name="IsRole"></param>
        /// <param name="EntityName"></param>
        /// <param name="ConfigQualifiedPathName"></param>
        /// <returns></returns>
        public bool AddAuthorizationConfiguration(string EntityName, AuthorizationRule theRule, bool RequiresPatientContext, string SpecialParms)
        {
            bool WasSuccessfull = AddIfNotFound(EntityName, theRule.Users, UserToQVMap, RequiresPatientContext, SpecialParms, false);
            WasSuccessfull &= AddIfNotFound(EntityName, theRule.Roles, RoleToQVMap, RequiresPatientContext, SpecialParms, true);
            return WasSuccessfull;
        }

        public AuthorizationRuleCollection GetAuthorizationConfiguration(string EntityName)
        {
            EntityName = EntityName.Replace('/', '\\');  //make sure all back slashes, sometimes forward slashes creep in
            AuthorizationRuleCollection ARC = new AuthorizationRuleCollection();
            AuthorizationRule AR = null;  //we always allow, everything denied by default
            foreach (KeyValuePair<string, List<QVEntity>> Entity in UserToQVMap)
            {
                if (Entity.Value != null)
                {
                    foreach (QVEntity QV in Entity.Value)
                    {
                        if ((QV.IsRole == false) && (string.Compare(EntityName, QV.QualifiedPathName, true) == 0))
                        {
                            AR = new AuthorizationRule(AuthorizationRuleAction.Allow);
                            AR.Users.Add(Entity.Key);
                            ARC.Add(AR);
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, List<QVEntity>> Entity in RoleToQVMap)
            {
                foreach (QVEntity QV in Entity.Value)
                {
                    
                    if ((QV.IsRole == true) && (string.Compare(EntityName, QV.QualifiedPathName, true) == 0))
                    {
                        AR = new AuthorizationRule(AuthorizationRuleAction.Allow);
                        AR.Roles.Add(Entity.Key);
                        ARC.Add(AR);
                    }
                }
            }

            return ARC;
        }

        private bool GenericDeleteItem(string QVEntity, string DeletionItem, Dictionary<string, List<QVEntity>> Items)
        {
            foreach (KeyValuePair<string, List<QVEntity>> KVP in Items)
            {
                if (string.Compare(KVP.Key, DeletionItem) == 0)
                {
                    for (int Index = 0; Index < KVP.Value.Count; Index++)
                    {
                        QVEntity QVE = KVP.Value[Index];
                        if (string.Compare(QVEntity.Replace('/', '\\'), QVE.QualifiedPathName.Replace('/', '\\'), true) == 0)
                        {
                            KVP.Value.RemoveAt(Index);
                            break;
                        }
                    }
                }
            }
            return true;
        }
        public bool DeleteItem(string QVEntity, string User, string Role, bool IsRole)
        {
            if (IsRole)
            {
                return GenericDeleteItem(QVEntity, Role, RoleToQVMap);
            }
            return GenericDeleteItem(QVEntity, User, UserToQVMap);
        }

        public bool Save()
        {
            StopFileWatcher();  //we are a producer,  stop watching since we always write things
            var serializer = new SharpSerializer(false);
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
            string RepoFilePath = ConfigurationManager.AppSettings["RepoFilePath"];
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
            FileWatcher.EnableRaisingEvents = false;
        }

        private static bool CleanRepo( Dictionary<string, List<QVEntity>> Segment )
        {
            string QVRoot = ConfigurationManager.AppSettings["QVDocumentRoot"];
            bool UserMapModfied = false;
            List<string> RemoveKeys = new List<string>();
            foreach (KeyValuePair<string, List<QVEntity>> QV in Segment)
            {
                //new method, removes invalid entries
                for (int Index = QV.Value.Count - 1; Index >= 0; Index--)
                {
                    string QualafiedQVW = System.IO.Path.Combine(QVRoot, QV.Value[Index].QualifiedPathName);
                    if (System.IO.File.Exists(QualafiedQVW) == false)
                    {
                        QV.Value.RemoveAt(Index);
                        UserMapModfied = true;
                    }
                    else //valid make sure we point all slashes same direction
                        QV.Value[Index].QualifiedPathName = QV.Value[Index].QualifiedPathName.Replace('/', '\\');
                }
                if (QV.Value.Count == 0)
                    RemoveKeys.Add(QV.Key);
            }

            //get rid of empty key values, not needed anymore
            foreach (string Key in RemoveKeys)
               Segment.Remove(Key);
            
            
            return UserMapModfied;
        }

        public static UserRepo Load()
        {
            //this gets called first
            string RepoFilePath = ConfigurationManager.AppSettings["RepoFilePath"];
            StartFileWatcher(RepoFilePath);
            //we need to make sure events are off while we load
            //apparently sharp deserialize mods the file in some way thus we get
            //multiple events that prompt loading
            bool EventsEnabled = FileWatcher.EnableRaisingEvents;
            FileWatcher.EnableRaisingEvents = false;
            UserRepo Repo = null;

            var serializer = new SharpSerializer(false);
            if (System.IO.File.Exists(RepoFilePath))
            {
                Repo = serializer.Deserialize(RepoFilePath) as UserRepo;
                bool UserMapModfied = CleanRepo(Repo.UserMap);
                bool RoleMapModified = CleanRepo(Repo.RoleMap);

                Repo.RepoFilePath = RepoFilePath;

                if (UserMapModfied || RoleMapModified)
                {
                    Repo.Save();  //save cleaned version back out
                }
                //old method, just corrected slashes
                //foreach (QVEntity QVE in QV.Value)
                //{
                //    QVE.QualifiedPathName = QVE.QualifiedPathName.Replace('/', '\\');
                //}
                //}
                //foreach (KeyValuePair<string, List<QVEntity>> QV in Repo.RoleMap)
                //{
                //    foreach (QVEntity QVE in QV.Value)
                //    {
                //        QVE.QualifiedPathName = QVE.QualifiedPathName.Replace('/', '\\');
                //    }
                //}
                //end forward slash replacement
            }
            FileWatcher.EnableRaisingEvents = EventsEnabled; //set back to original state
            return Repo;
        }
        
        #endregion
    }
}
