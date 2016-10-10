using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Polenter.Serialization;
using System.Configuration;

namespace MillimanCommon
{
    public class SuperGroup
    {
        /// <summary>
        /// Milliman internal groups should have 1 list entry per user, such that the client publisher
        /// will display a list of all the projects they can update
        /// However, external clients may have multiple contains associated and for each container they
        /// will get an entry on the drop down in web portal to select the container, the container will
        /// then contain the list of projects they can administer for users
        /// 
        /// Super groups are not intended to limit access for client admins, but to group the information
        /// a client admin can see
        /// 
        /// WARNING!  only 1 qvw per project is allowed for this functionality
        /// </summary>
        public enum AdminOrigin { Milliman_Internal, External_Client };

        public class SuperGroupContainer
        {
            //name of the container
            private string _ContainerName;
            public string ContainerName
            {
                get { return _ContainerName; }
                set { _ContainerName = value; }
            }

            private string _ContainerDescription;
            public string ContainerDescription
            {
                get { return _ContainerDescription; }
                set { _ContainerDescription = value; }
            }

            /// <summary>
            /// used by client admin to change from , to semicolon delimited emails
            /// </summary>
            private bool _SemiColonDelimitedEmail;
            public bool SemiColonDelimitedEmail
            {
                get { return _SemiColonDelimitedEmail; }
                set { _SemiColonDelimitedEmail = value; }
            }

            /// <summary>
            /// controled in admin panel used by client admin to show temp password textboxes instead of securelink
            /// </summary>
            private bool _AllowTempPasswordEntry;
            public bool AllowTempPasswordEntry
            {
                get { return _AllowTempPasswordEntry; }
                set { _AllowTempPasswordEntry = value; }
            }

            /// <summary>
            /// client admin accounts that can access this super group
            /// </summary>
            private List<string> _AdminUserAccounts;

            public List<string> AdminUserAccounts
            {
                get { return _AdminUserAccounts; }
                set { _AdminUserAccounts = value; }
            }

            /// <summary>
            /// publishish admin accounts that can access this super group
            /// </summary>
            private List<string> _PublisherUserAccounts;

            public List<string> PublisherUserAccounts
            {
                get { return _PublisherUserAccounts; }
                set { _PublisherUserAccounts = value; }
            }

            /// <summary>
            /// groups names that are in this super group
            /// </summary>
            private List<string> _GroupNames;
            public List<string> GroupNames
            {
                get { return _GroupNames; }
                set { _GroupNames = value; }
            }

            public SuperGroupContainer()
            {
            }

            public SuperGroupContainer(string Name, string Description, List<string> AdminAccounts, List<string> PublishingAccounts, List<string> Groups, bool SemiColonForEmails = false)
            {
                _ContainerName = Name;
                _ContainerDescription = Description;
                _SemiColonDelimitedEmail = SemiColonForEmails;
                _AdminUserAccounts = AdminAccounts;
                _PublisherUserAccounts = PublishingAccounts;
                _GroupNames = Groups;
            }
        }

        private List<SuperGroupContainer> _SuperGroupContainers;
        public List<SuperGroupContainer> SuperGroupContainers
        {
            get { return _SuperGroupContainers; }
            set { _SuperGroupContainers = value; }
        }

        /// <summary>
        /// Find all the super group entries the user has access to
        /// </summary>
        /// <param name="ClientAdminName"></param>
        /// <returns></returns>
        public List<SuperGroupContainer> GetSupersForClientAdmin(string ClientAdminName)
        {
            //Verify the settings are valid each time a user attempts access, could have
            //changed via group being removed from user
            ValidateUserForSuperGroup(ClientAdminName);

            List<SuperGroupContainer> Supers = new List<SuperGroupContainer>();
            if (SuperGroupContainers == null)
            {
                Report.Log(Report.ReportType.Error, "Super group file is missing - '" + SuperGroupFilePath + @"'");
                return Supers;
            }
            foreach (SuperGroupContainer SGC in SuperGroupContainers)
            {
                if (SGC.AdminUserAccounts != null)
                {
                    foreach (string User in SGC.AdminUserAccounts)
                    {
                        if (string.Compare(User, ClientAdminName, true) == 0)
                        {
                            Supers.Add(SGC);
                            break;
                        }
                    }
                }
            }
            return Supers;
        }

        /// <summary>
        /// Find all the super group entries the user has access to
        /// </summary>
        /// <param name="ClientAdminName"></param>
        /// <returns></returns>
        public List<SuperGroupContainer> GetSupersForPublishingAdmin(string ClientAdminName)
        {
            //Verify the settings are valid each time a user attempts access, could have
            //changed via group being removed from user
            ValidateUserForSuperGroup(ClientAdminName);

            List<SuperGroupContainer> Supers = new List<SuperGroupContainer>();
            if (SuperGroupContainers == null)
            {
                Report.Log(Report.ReportType.Error, "Super group file is missing - '" + SuperGroupFilePath + @"'");
                return Supers;
            }
            foreach (SuperGroupContainer SGC in SuperGroupContainers)
            {
                if (SGC.PublisherUserAccounts != null)
                {
                    foreach (string User in SGC.PublisherUserAccounts)
                    {
                        if (string.Compare(User, ClientAdminName, true) == 0)
                        {
                            Supers.Add(SGC);
                            break;
                        }
                    }
                }
            }
            return Supers;
        }

        /// <summary>
        /// Check to see if the user exists in all the groups in the supegroup
        /// before we show the user a menu - if we mod the supergroup we need to
        /// resave so it shows up correctly in the user admin console
        /// </summary>
        /// <param name="SuperGroup"></param>
        /// <param name="User"></param>
        private void ValidateUserForSuperGroup(string User)
        {
            //if we have not supers or I am an admin, don't scrub
            if ((SuperGroupContainers == null) || (System.Web.Security.Roles.IsUserInRole("administrator")))
                return;  //nothing to do

            bool Modified = false;
            foreach (SuperGroupContainer Current in SuperGroupContainers)
            {
                //only check a super group that has USER as part of items
                if (Current.PublisherUserAccounts.Contains(User, StringComparer.OrdinalIgnoreCase) || Current.AdminUserAccounts.Contains(User, StringComparer.OrdinalIgnoreCase))
                {
                    foreach (string GroupName in Current.GroupNames)
                    {
                        //check to see user is in EVERY role of supergroup
                        if (System.Web.Security.Roles.IsUserInRole(User, GroupName) == false)
                        {
                            Current.PublisherUserAccounts.Remove(User);
                            Current.AdminUserAccounts.Remove(User);
                            Modified = true;
                        }
                    }
                }
            }
            if ((Modified) && (SuperGroupContainers != null))
            {
                //user not in every group - we removed them so re-save
                Save();
            }
        }

        public SuperGroupContainer FindSuper(string SuperGroupName)
        {
            if (SuperGroupContainers == null)
                return null;
            foreach (SuperGroupContainer SGC in SuperGroupContainers)
            {
                if (string.Compare(SGC.ContainerName, SuperGroupName, true) == 0)
                    return SGC;
            }
            return null;
        }

        public bool DeleteSuper(string SuperGroupName)
        {
            if (SuperGroupContainers == null)
                return false;
            for (int Index = SuperGroupContainers.Count - 1; Index >= 0; Index--)
            {
                if (string.Compare(SuperGroupContainers[Index].ContainerName, SuperGroupName, true) == 0)
                {
                    SuperGroupContainers.RemoveAt(Index);
                    Save();
                    return true;
                }
            }
            return false;
        }

        #region Scaffolding


        //factory singleton
        private static SuperGroup instance;
        private static object instance_lock = new object();
        public static SuperGroup GetInstance()
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

        private string SuperGroupFilePath { get; set; }
        private static System.IO.FileSystemWatcher FileWatcher { get; set; }

        private string ID = Guid.NewGuid().ToString();


        public SuperGroup()
        {
            _SuperGroupContainers = new List<SuperGroupContainer>();
        }


        public bool Save()
        {
            StopFileWatcher();  //we are a producer,  stop watching since we always write things
            var serializer = new SharpSerializer(false);
            serializer.Serialize(this, SuperGroupFilePath);
            return true;
        }

        public static void StartFileWatcher(string SuperGroupFilePath)
        {
            //start watching if not already watching
            if (FileWatcher == null)
            {
                FileWatcher = new System.IO.FileSystemWatcher(System.IO.Path.GetDirectoryName(SuperGroupFilePath), System.IO.Path.GetFileName(SuperGroupFilePath));
                FileWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite;
                FileWatcher.Changed += FileWatcher_Changed;
                FileWatcher.EnableRaisingEvents = true;
            }
        }

        public static void FileWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            //wait for at least 30 seconds to load
            string RepoFilePath = ConfigurationManager.AppSettings["SuperGroupFilePath"];
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
            SuperGroup.KillInstance();
            SuperGroup.GetInstance(); //this will reload everything
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
            FileWatcher.EnableRaisingEvents = false;
        }

        public static SuperGroup Load()
        {
            //this gets called first
            string SuperGroupFilePath = ConfigurationManager.AppSettings["SuperGroupFilePath"];
            StartFileWatcher(SuperGroupFilePath);

            //we need to make sure events are off while we load
            //apparently sharp deserialize mods the file in some way thus we get
            //multiple events that prompt loading
            bool EventsEnabled = FileWatcher.EnableRaisingEvents;
            FileWatcher.EnableRaisingEvents = false;
            SuperGroup SuperGroupRepo = null;

            var serializer = new SharpSerializer(false);
            if (System.IO.File.Exists(SuperGroupFilePath))
            {
                SuperGroupRepo = serializer.Deserialize(SuperGroupFilePath) as SuperGroup;
                SuperGroupRepo.SuperGroupFilePath = SuperGroupFilePath;
            }
            FileWatcher.EnableRaisingEvents = EventsEnabled; //set back to original state

            //Uncomment to regenerate file
            //SuperGroup SG = new SuperGroup();
            //SuperGroup.SuperGroupContainer SGC1 = new SuperGroupContainer();
            //SGC1.ContainerName = @"New York Projects";
            //SGC1.ContainerDescription = @"Projects related to New York office";
            //SGC1.AllowTempPasswordEntry = true;
            //SGC1.SemiColonDelimitedEmail = false;
            //SGC1.AdminUserAccounts = new List<string>() { "van.nanney@milliman.com", "maggie.alston@milliman.com", "pamela.pelizarri@milliman.com", "david.pierce@milliman.com" };
            //SGC1.PublisherUserAccounts = new List<string>() { "van.nanney@milliman.com", "maggie.alston@milliman.com", "pamela.pelizarri@milliman.com", "david.pierce@milliman.com" };
            //SGC1.GroupNames = new List<string>() { "0000EXT01_NewYork_BPCI Demo Milliman", "0000EXT01_NewYork_BPCI Demo Premier", "0000EXT01_NewYork_LIVE BPCI - CAM", "0000EXT01_NewYork_LIVE BPCI - Premier", "0000EXT01_NewYork_LIVE BPCI - USPI" };
            //SG.SuperGroupContainers.Add(SGC1);

            //SuperGroup.SuperGroupContainer SGC2 = new SuperGroupContainer();
            //SGC2.ContainerName = @"Hartford Projects";
            //SGC2.ContainerDescription = @"Projects related to Hartford office";
            //SGC2.GroupNames = new List<string>() { "0000EXT01_Hartford" };
            //SGC2.AllowTempPasswordEntry = false;
            //SGC2.SemiColonDelimitedEmail = false;
            //SG.SuperGroupContainers.Add(SGC2);

            //SuperGroup.SuperGroupContainer SGC3 = new SuperGroupContainer();
            //SGC3.ContainerName = @"WOAH Projects";
            //SGC3.ContainerDescription = @"Projects related to WOAH";
            //SGC3.AllowTempPasswordEntry = false;
            //SGC3.SemiColonDelimitedEmail = false;
            //SGC3.AdminUserAccounts = new List<string>() { "van.nanney@milliman.com", "david.pierce@milliman.com", "tmuday@dochshp.com" };
            //SGC3.GroupNames = new List<string>() { "0273WOH01_Medicaid_PRCA", "0273WOH01_Medicaid_WOAH", "0273WOH01_Medicaid_ALL", "0273WOH01_Medicaid_Demo" };
            //SG.SuperGroupContainers.Add(SGC3);
            //SG.SuperGroupFilePath = SuperGroupFilePath;
            //SG.Save();
            //end file generation


            return SuperGroupRepo;
        }
        #endregion


    }
}
