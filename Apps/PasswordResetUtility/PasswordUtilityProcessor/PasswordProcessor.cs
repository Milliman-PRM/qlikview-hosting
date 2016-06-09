using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Profile;
using System.Web.Security;
using SystemReporting.Utilities.Email;

/// <summary>
/// In order to use the Membership API we need to reference System.configuration &  System.Web.ApplicationServices
/// and we need to add the membership handler to app.config file
/// </summary>
namespace PasswordUtilityProcessor
{
    public class PasswordProcessor : BaseFileProcessor
    {
        public static void ExecutePasswordResetUtility(string args)
        {
            try
            {
                Process(args);
                if (!string.IsNullOrEmpty(GetDirectoryCleanUp()))
                {
                    DirectoryCleanUp();
                }
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, "ExecutePasswordResetUtility || " + args.ToArray());
            }
        }

        /// <summary>
        /// Process user based on the App.config file settings
        /// If the app.config file has the handler, it will process that function.
        /// </summary>
        /// <param name="args"></param>
        private static void Process(string args)
        {
            if (!string.IsNullOrEmpty(GetAllUserProcessing()))
            {
                ProcessAllUsers();
            }
            else if (!string.IsNullOrEmpty(GetUserProcessing()))
            {
                ProcessUser(args);
            }

        }

        /// <summary>
        /// Method to process all users
        /// </summary>
        public static void ProcessAllUsers()
        {
            try
            {
                //get all users
                var usersCollection = Membership.GetAllUsers();

                //if the no user found
                if (usersCollection == null || usersCollection.Count == 0)
                {
                    //log error and send email
                    BaseFileProcessor.LogError(null, "ProcessAllUsers || System could not load all users. Check if the database server is down.");
                    SendEmail();
                    return;
                }

                //if file counter has value then genrate exact number of files. If the file counter is null, then generate ALL
                if (ConfigurationManager.AppSettings["FileGenerateCounter"] != null)
                {
                    // get FileGenerateCounter value
                    var fileGenerateCounter = ConfigurationManager.AppSettings["FileGenerateCounter"].ToString();

                    // find out count of files in the dir, if there are any files then we need to subtract that from the fileGenerateCounter
                    var allFilesInDir = GetAllFileNamesFromDirectory(GetFolderPath());

                    // check if the file in dir is more than or equal to the FileGenerateCounter
                    // if files in dir are less then generate files
                    if (allFilesInDir.Count < int.Parse(fileGenerateCounter))
                    {
                        // how many files we need to generate. *Subtract the existing files that are already in the folder and generate the difference
                        int filesCounter = (int.Parse(fileGenerateCounter) - (allFilesInDir.Count));   
                       
                        // create user list of all members                   
                        var userList = Membership.GetAllUsers().OfType<MembershipUser>().ToList();

                        //TODO: Add check to remove users that has files

                        // from the above user list, process users count equal to the filesCounter .. 
                        // so if the value of filesCounter=10 and users are 300, then subtract 10 from 300 and process 10 users only
                        var userCountedList = userList.OrderBy(i => i.UserName).Take(filesCounter + 1).ToList();
                        for (int recordCount = 0; recordCount < userCountedList.Count; recordCount++)
                        {
                            ProcessUser(userCountedList[recordCount].UserName);
                        }
                    }
                }
                else
                {
                    //All files
                    foreach (MembershipUser user in usersCollection)
                    {
                        ProcessUser(user.UserName);
                    }
                }

            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, "ProcessAllUsers");
            }
        }

        /// <summary>
        /// Method to process specific user
        /// </summary>
        /// <param name="args"></param>
        public static void ProcessUser(string args)
        {
            try
            {
                var user = Membership.GetUser(args);
                if (user == null)
                {
                    //log error and send email
                    BaseFileProcessor.LogError(null, "ProcessUser || Invalid user name. Please check your user name and try again.");
                    SendEmail();
                    return;
                }

                //paswwrod exipration duration
                var passwordExpiresInDays = 0;

                if (ConfigurationManager.AppSettings["PasswordExpiresInDays"] != null)
                {
                    if (int.TryParse(ConfigurationManager.AppSettings["PasswordExpiresInDays"].ToString(), out passwordExpiresInDays))
                    {
                        //validate the user
                        if (Membership.ValidateUser(user.UserName, user.GetPassword()))
                        {
                            //Timespan gets the difference in days between Today and the last time the user changed his password(LastPasswordChangedDate) 
                            TimeSpan ts = DateTime.Today - user.LastPasswordChangedDate;

                            //Check if the TimeSpan.TotalDays is greater than the PasswordExpiresInDays setting we got from the app.config file. If true the user must change his password and we log the entry for that user 
                            if (ts.TotalDays > passwordExpiresInDays)
                            {
                                //get user unique provider Key
                                var providerUserKey = user.ProviderUserKey;
                                //check if providerUserKey exist
                                if (providerUserKey == null)
                                {
                                    //log error
                                    BaseFileProcessor.LogError(null, "ProcessUser || Invalid user. System could not find the providerUserKey " + providerUserKey + ". Check the 'UserId' in [aspnet_Users] for the userName " + user.UserName + ".");
                                    SendEmail();
                                    return;
                                }
                                WrtiePasswordResetFile(user);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, "ProcessUser");
            }
        }

        #region Notification
        private static void SendEmail()
        {
            //send email
            Notification.SendNotification("Password re-set Utility could not process user(s) information due to lack of information in database. ", "Missing User Information");
        }
        #endregion

        #region File Function
        
        private static void WrtiePasswordResetFile(MembershipUser user)
        {
            //get the log directory
            if (!string.IsNullOrEmpty(GetFolderPath()))
            {
                //write file
                var userPasswordResetFileAndDirectory = (GetFolderPath() + user.ProviderUserKey.ToString().ToUpper() + ".rst");
                //Check file
                if (File.Exists(userPasswordResetFileAndDirectory))
                {
                    File.Delete(userPasswordResetFileAndDirectory);
                }
                //if (!File.Exists(userPasswordResetFileAndDirectory)){
                //create the file name
                var message = (DateTime.Now + " || " + "User Needs to Reset Password. User Name: " + user.UserName);
                File.WriteAllText(userPasswordResetFileAndDirectory, message);
                //}
            }

        }

        /// <summary>
        /// Each time the password reset utility runs,  match every "rst" file to it's associated user account in database,  
        /// if "rst" file does not match an account,  delete the "rst" file
        /// </summary>
        private static void DirectoryCleanUp()
        {
            if (!string.IsNullOrEmpty(GetFolderPath()))
            {
                var allFilesInDir = GetAllFileNamesFromDirectory(GetFolderPath());
                var listUsers = new List<string>();

                //get all users
                if (Membership.GetAllUsers() != null || Membership.GetAllUsers().Count > 0)
                {
                    //generate list of user by getting the ProviderUserKey only
                    listUsers = Membership.GetAllUsers().OfType<MembershipUser>().Select(s=>s.ProviderUserKey.ToString().ToUpper()).ToList();
                }

                var fielsInDirWithOutExt = new List<string>();

                //Now create a new list and remove the .rst extention
                fielsInDirWithOutExt = allFilesInDir.Select(s => s.Replace(".rst","").Trim()).ToList();

                // get files to be deleted by comparing files with list of users and remove the file that does not have a matching user!
                var fileTobeDeleted = fielsInDirWithOutExt.Except(listUsers).ToList();

                //loop through each file and delete
                foreach (var f in fileTobeDeleted)
                {
                    File.Delete(GetFolderPath() + f + ".rst");
                }
            }
        }

        /// <summary>
        /// Returns list of files
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<string> GetAllFileNamesFromDirectory(string dir)
        {
            var files = new List<string>();
            var isEmpty = !System.IO.Directory.EnumerateFiles(dir).Any();
            if (!isEmpty)
            {
                var directory = new System.IO.DirectoryInfo(dir);
                files = directory.GetFiles()
                                 .OrderBy(file => file.LastWriteTime)
                                 .Select(file => file.Name).ToList();
            }
            return files;
        }

        private static string GetFolderPath()
        {
            // For Example - D:\Projects\SomeProject\SomeFolder
            return (ConfigurationManager.AppSettings != null &&
                    ConfigurationManager.AppSettings["UserProfileDirectory"] != null) ?
                    ConfigurationManager.AppSettings["UserProfileDirectory"].ToString() :
                    string.Empty;
        }

        private static string GetDirectoryCleanUp()
        {
            // For Example - D:\Projects\SomeProject\SomeFolder
            return (ConfigurationManager.AppSettings != null &&
                    ConfigurationManager.AppSettings["DirectroyCleanUp"] != null) ?
                    ConfigurationManager.AppSettings["DirectroyCleanUp"].ToString() :
                    string.Empty;
        }

        private static string GetUserProcessing()
        {
            // For Example - D:\Projects\SomeProject\SomeFolder
            return (ConfigurationManager.AppSettings != null &&
                    ConfigurationManager.AppSettings["ProcessSpecificUser"] != null) ?
                    ConfigurationManager.AppSettings["ProcessSpecificUser"].ToString() :
                    string.Empty;
        }

        private static string GetAllUserProcessing()
        {
            // For Example - D:\Projects\SomeProject\SomeFolder
            return (ConfigurationManager.AppSettings != null &&
                    ConfigurationManager.AppSettings["ProcessAllUsers"] != null) ?
                    ConfigurationManager.AppSettings["ProcessAllUsers"].ToString() :
                    string.Empty;
        }

        #endregion

        #region Custom
        /// <summary>
        /// call this method to get info about the Membership Provider
        /// </summary>
        /// <returns></returns>
        public static SqlMembershipProvider GetMembershipProvider()
        {
            var prov = new SqlMembershipProvider();
            var coll = new NameValueCollection {
                                                    { "connectionStringName", "PWUdbContextConnectionString" },
                                                    { "applicationName", "/" }
                                                };
            prov.Initialize("dbSqlMemberShipProvider", coll);
            return prov;
        }

        /// <summary>
        /// call this method to get info about the Profile Provider
        /// </summary>
        /// <returns></returns>
        public static SqlProfileProvider GetProfileProvider()
        {
            var prov = new SqlProfileProvider();
            var coll = new NameValueCollection {
                                                    { "connectionStringName", "PWUdbContextConnectionString" },
                                                    { "applicationName", "/" }
                                                };
            prov.Initialize("AspNetSqlProfileProvider", coll);
            return prov;
        }
        /// <summary>
        /// call this method to get info about the Role Provider
        /// </summary>
        /// <returns></returns>
        public static SqlRoleProvider GetRoleProvider()
        {
            var prov = new SqlRoleProvider();
            var coll = new NameValueCollection {
                                                { "connectionStringName", "PWUdbContextConnectionString" },
                                                { "applicationName", "/" }
                                                };
            prov.Initialize("AspNetSqlRoleProvider", coll);
            return prov;
        }
        #endregion
    }
}
