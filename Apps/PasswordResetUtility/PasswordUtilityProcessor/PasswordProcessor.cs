using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Profile;
using System.Web.Security;


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
                Process();
                if (!string.IsNullOrEmpty(GetDirectoryCleanUp()))
                {
                    if (String.Equals(GetDirectoryCleanUp(), "True", StringComparison.OrdinalIgnoreCase) == true)
                        DirectoryCleanUp();
                }          
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, "ExecutePasswordResetUtility || An exception occured during the processing." + args);                
            }
        }

        /// <summary>
        /// Process user based on the App.config file settings
        /// If the app.config file has the handler, it will process that function.
        /// </summary>
        /// <param name="args"></param>
        private static void Process()
        {
            if (String.Equals(GetAllUserProcessing(), "True", StringComparison.OrdinalIgnoreCase) == true)
            {
                ProcessAllUsers();
            }
            else if (!string.IsNullOrEmpty(GetSingleUserProcessing()))
            {
                ProcessUser(GetSingleUserProcessing());
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
                    return;
                }

                //if file counter has value then genrate exact number of files. If the file counter is null, then generate All files for users
                if (!string.IsNullOrEmpty(GetFileGenerateCounter()))
                {
                    // 1. get FileGenerateCounter value from configs
                    int fileGenerateCounter;
                    //make sure we have numeric value
                    var isFileGenerateCounterNumeric = int.TryParse(GetFileGenerateCounter(), out fileGenerateCounter);

                    if (isFileGenerateCounterNumeric)
                    {
                        int filesToGenerateCounter = 0;

                        // 2. find out count of files in the directory, if there are any files then we need to subtract that from the fileGenerateCounter
                        var existingFilesInDir = GetAllFileNamesFromDirectory(GetFolderPath());

                        // If there are files in dir
                        //if (existingFilesInDir != null && existingFilesInDir.Count > 0)
                        //{
                        // ******Check if the file in dir is more than or equal to the FileGenerateCounter. if files in directory are less then generate files ********//
                        if (existingFilesInDir.Count < fileGenerateCounter)
                        {
                            // 4. How many files we need to generate => *Subtract the existing files that exist in the directory and generate the "difference"
                            filesToGenerateCounter = (fileGenerateCounter - existingFilesInDir.Count);
                            // *****Check to remove users that has files in Dir *******//
                            //5. generate list of user ProviderUserKey only [generate a list of ProviderUserKey]
                            var listUsersProviderKey = Membership.GetAllUsers().OfType<MembershipUser>().Select(s => s.ProviderUserKey.ToString().ToUpper()).ToList();

                            //6. generate list of users ProviderUserKey form existing files in directory and remove the .rst extention
                            var existingFilesInDirWithoutExt = existingFilesInDir.Select(s => s.Replace(".rst", "").Trim()).ToList();

                            if (listUsersProviderKey.Count > 0)
                            {
                                //7. From the user provider key, remove the existing files provider key
                                var usersTobeProcessed = listUsersProviderKey.Except(existingFilesInDirWithoutExt).ToList();

                                //8. Convert back to Membership class 
                                var finalUserList = new List<MembershipUser>();
                                foreach (var membershipUser in usersTobeProcessed)
                                {
                                    finalUserList.Add((Membership.GetUser(new Guid(membershipUser))));
                                }

                                // 11. From the above user list [finalUserList], process users count equal to the filesCounter .. 
                                #region Example
                                // Example: Assume the fileGenerateCounter is 10 and we have existingFilesInDir equal to 4 (means there are 4 files in dir)
                                // - we need to fist figure out how many total files we need to generate [filesToGenerateCounter = 10-4=6]
                                // - we get list of exisitng files without extention [existingFilesInDirWithoutExt]
                                // - We need to exclude the existing files [6] from user list to get a clean user list [usersTobeProcessed=listUsersProviderKey-existingFilesInDirWithoutExt]
                                // - convert usersTobeProcessed provider key to MembershipUser
                                // - get the users to be processed from finalUserList for exact count as filesToGenerateCounter
                                #endregion
                                if (finalUserList.Count > 0)
                                {
                                    var userToProcessCountedList = finalUserList.OrderBy(i => i.UserName).Take(filesToGenerateCounter).ToList();
                                    if (userToProcessCountedList.Count > 0)
                                    {
                                        foreach (var item in userToProcessCountedList)
                                        {
                                            ProcessUser(item.UserName);
                                        }
                                    }
                                }
                            }
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
        /// <param name="param">todo: describe param parameter on ProcessUser</param>
        public static void ProcessUser(string param)
        {
            try
            {
                var user = Membership.GetUser(param);
                if (user == null)
                {
                    //log error and send email
                    BaseFileProcessor.LogError(null, "ProcessUser || Invalid user name. System could not find the user | " + param + " | in database. Please check user name in database and make sure user is valid.");
                    return;
                }

                //paswwrod exipration duration
                var passwordExpiresInDays = 0;

                if (!string.IsNullOrEmpty(GetPasswordExpirationDurationCounter()))
                {
                    if (int.TryParse(GetPasswordExpirationDurationCounter(), out passwordExpiresInDays))
                    {
                        //validate the user

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
                                BaseFileProcessor.LogError(null, "ProcessUser || Invalid user. System could not find the providerUserKey | " + providerUserKey + " | in database. Check the 'UserId' in [aspnet_Users] for the userName " + user.UserName + ".");
                                return;
                            }
                            WritePasswordResetFile(user);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, "ProcessUser");
            }
        }

        //#region Notification
        //private static void SendErrorEmail()
        //{
        //    if (bErrorLogged)
        //        //send email
        //        BaseFileProcessor.SendEmail("Password re-set Utility could not process user(s) information due to missing or lack of information in database. ", "Missing User Information");
        //    bErrorLogged = false;
        //}
        //#endregion

        #region File Function

        private static void WritePasswordResetFile(MembershipUser user)
        {
            //get the log directory
            if (!string.IsNullOrEmpty(GetFolderPath()))
            {
                //write file
                var userPasswordResetFileAndDirectory = Path.Combine(GetFolderPath() ,user.ProviderUserKey.ToString().ToUpper() + ".rst");
                ////Check file
                //if (File.Exists(userPasswordResetFileAndDirectory))
                //{
                //    File.Delete(userPasswordResetFileAndDirectory);
                //}
                //if file does not exist then create
                if (!File.Exists(userPasswordResetFileAndDirectory))
                {
                    //create the file name
                    var message = (DateTime.Now + " || " + "User Needs to Reset Password. User Name: " + user.UserName);
                    File.WriteAllText(userPasswordResetFileAndDirectory, message);
                }
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

                var listUsers = new List<string>();
                //get all users
                if (Membership.GetAllUsers() != null || Membership.GetAllUsers().Count > 0)
                {
                    //generate list of user by getting the ProviderUserKey only
                    listUsers = Membership.GetAllUsers().OfType<MembershipUser>().Select(s => s.ProviderUserKey.ToString().ToUpper()).ToList();
                }

                // get existing files list in dir
                var existingFilesInDir = GetAllFileNamesFromDirectory(GetFolderPath());
                if (existingFilesInDir != null && existingFilesInDir.Count > 0)
                {
                    //Now create a new list and remove the .rst extention
                    var fielsInDirWithOutExt = existingFilesInDir.Select(s => s.Replace(".rst", "").Trim()).ToList();

                    if (fielsInDirWithOutExt.Count > 0)
                    {
                        // get files to be deleted by comparing files with list of users and remove the file that does not have a matching user!
                        var fileTobeDeleted = fielsInDirWithOutExt.Except(listUsers).ToList();
                        if (fileTobeDeleted.Count > 0)
                        {
                            //loop through each file and delete
                            foreach (var file in fileTobeDeleted)
                            {
                                File.Delete(Path.Combine(GetFolderPath() , file + ".rst"));
                            }
                        }
                    }
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
                    ConfigurationManager.AppSettings["DirectoryCleanUp"] != null) ?
                    ConfigurationManager.AppSettings["DirectoryCleanUp"].ToString() :
                    string.Empty;
        }

        private static string GetSingleUserProcessing()
        {
            return (ConfigurationManager.AppSettings != null &&
                    ConfigurationManager.AppSettings["ProcessSingleUser"] != null) ?
                    ConfigurationManager.AppSettings["ProcessSingleUser"].ToString() :
                    string.Empty;
        }

        private static string GetAllUserProcessing()
        {
            return (ConfigurationManager.AppSettings != null &&
                    ConfigurationManager.AppSettings["ProcessAllUsers"] != null) ?
                    ConfigurationManager.AppSettings["ProcessAllUsers"].ToString() :
                    string.Empty;
        }

        private static string GetFileGenerateCounter()
        {
            return (ConfigurationManager.AppSettings != null &&
                    ConfigurationManager.AppSettings["FileGenerateCounter"] != null) ?
                    ConfigurationManager.AppSettings["FileGenerateCounter"].ToString() :
                    string.Empty;
        }

        private static string GetPasswordExpirationDurationCounter()
        {
            return (ConfigurationManager.AppSettings != null &&
                    ConfigurationManager.AppSettings["PasswordExpiresInDays"] != null) ?
                    ConfigurationManager.AppSettings["PasswordExpiresInDays"].ToString() :
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
