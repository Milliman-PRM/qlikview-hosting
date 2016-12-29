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
    public class PasswordProcessor
    {
        //incremented for each user requested for password reset this iteration
        public static int NewPasswordResetsThisIteration = 0;
        public static int TotalPasswordResets = 0;

        /// <summary>
        /// Main entry point for checking user accounts for password reset, all configuration items come from a config
        /// file,  error conditions are propagated back to user via exceptions
        /// </summary>
        public static void ExecutePasswordResetUtility()
        {
 
            //first just call to get configuration items, if there is an item with invalid configuration this will throw an exception and halt any processing
            //each routine will check the types and values, so no point in looking at return values here
            GetRSTFolderPath();
            GetDirectoryCleanUp();
            GetRSTFileGenerateCounter();
            GetPasswordExpirationDurationCounter();

            //if we got to here, all the inputs are valid so process
            Process();

            //go ahead and call directory cleanup - it will not do anything if configuration item DirectoryCleanUp is set to FALSE
            DirectoryCleanUp();
   
        }

        /// <summary>
        ///process the users to see if there password should be expired, the max number of password resets that may be done is controlled via
        ///the RSTFileCounter config item - if the number of RST files is already greater than this ceiling this processing will do nothing.
        ///To process all users set the RSTFileCounter to a large number ie 1000000
        /// </summary>
        public static void Process()
        {
            //get all users
            MembershipUserCollection usersCollection = Membership.GetAllUsers();

            //if the no user found
            if (usersCollection == null || usersCollection.Count == 0)
            {
                throw new Exception("Failed to access database instance to obtain user list.");
            }
            //create a list of user provide IDs that are already reset from files in password reset folder
            List<string> UsersAlreadyReset = System.IO.Directory.GetFiles(GetRSTFolderPath(), "*.RST").Where(s => s.EndsWith(".rst")).Select(s => System.IO.Path.GetFileNameWithoutExtension(s).Trim()).ToList();
            TotalPasswordResets = UsersAlreadyReset.Count();
            if (GetRSTFileGenerateCounter() <= UsersAlreadyReset.Count())
                return;  //we are done, already reached limit of number of users to reset

            foreach( MembershipUser MU in usersCollection )
            {
                if (CheckPasswordExpired(MU) && (UsersAlreadyReset.Contains(MU.ProviderUserKey.ToString(),StringComparer.OrdinalIgnoreCase) == false))
                {
                    NewPasswordResetsThisIteration++;  //increment for each user requested this iteration
                    TotalPasswordResets++;
                    WritePasswordResetFile(MU);
                    UsersAlreadyReset.Add(MU.ProviderUserKey.ToString());
                    if (UsersAlreadyReset.Count() >= GetRSTFileGenerateCounter())
                        return; //we are done
                }
            }
        }

        /// <summary>
        /// Check to see if the user has changed thier password within the required time
        /// </summary>
        /// <param name="paramUser"></param>
        /// <returns></returns>
        public static bool CheckPasswordExpired(MembershipUser paramUser)
        {
            var isPasswordExpired = false;
            //paswwrod exipration duration
            var passwordExpiresInDays = GetPasswordExpirationDurationCounter();

            //Timespan gets the difference in days between Today and the last time the user changed his password(LastPasswordChangedDate) 
            //TimeSpan ts = DateTime.Today - paramUser.LastPasswordChangedDate;
            var totalDaysPasswordChanged = (DateTime.Today - paramUser.LastPasswordChangedDate).TotalDays;
            //Check if the TimeSpan.TotalDays is greater than the PasswordExpiresInDays setting we got from the app.config file. If true the user must change his password and we log the entry for that user 
            if (totalDaysPasswordChanged > passwordExpiresInDays)
            {
                isPasswordExpired = true;
            }

            return isPasswordExpired;
        }
        #region File Function

        /// <summary>
        /// Write the RST file for the user - named using the membership provider ID
        /// </summary>
        /// <param name="paramUser"></param>
        private static void WritePasswordResetFile(MembershipUser paramUser)
        {
            //write file
            var userPasswordResetFileAndDirectory = Path.Combine(GetRSTFolderPath(), paramUser.ProviderUserKey.ToString().ToUpper() + ".rst");

            //if file does not exist then create
            if (!File.Exists(userPasswordResetFileAndDirectory))
            {
                //create the file name
                var message = (DateTime.Now + " || " + "User Needs to Reset Password. User Name: " + paramUser.UserName);
                File.WriteAllText(userPasswordResetFileAndDirectory, message);
            }
        }

        /// <summary>
        /// Each time the password reset utility runs,  match every "rst" file to it's associated user account in database,  
        /// if "rst" file does not match an account,  delete the "rst" file
        /// </summary>
        private static void DirectoryCleanUp()
        {
            if (GetDirectoryCleanUp() == false)
                return; //we are not supposed to run so return

            var listUsers = new List<string>();
            //get all users
            if (Membership.GetAllUsers() != null || Membership.GetAllUsers().Count > 0)
            {
                //generate list of user by getting the ProviderUserKey only
                listUsers = Membership.GetAllUsers().OfType<MembershipUser>().Select(s => s.ProviderUserKey.ToString().ToUpper()).ToList();
            }

            // get existing files list in dir
            var existingFilesInDir = GetAllRSTFileNamesFromDirectory(GetRSTFolderPath());
            if (existingFilesInDir != null && existingFilesInDir.Count > 0)
            {
                //Now create a new list and remove the .rst extention
                var fielsInDirWithOutExt = existingFilesInDir.Where(s => s.EndsWith(".rst")).Select(s => s.Replace(".rst", "").Trim()).ToList();

                if (fielsInDirWithOutExt.Count > 0)
                {
                    // get files to be deleted by comparing files with list of users and remove the file that does not have a matching user!
                    var fileTobeDeleted = fielsInDirWithOutExt.Except(listUsers).ToList();
                    if (fileTobeDeleted.Count > 0)
                    {
                        //loop through each file and delete
                        foreach (var file in fileTobeDeleted)
                        {
                            File.Delete(Path.Combine(GetRSTFolderPath(), file + ".rst"));
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Returns list of all RST files
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<string> GetAllRSTFileNamesFromDirectory(string dir)
        {
            var files = new List<string>();
            var isEmpty = !System.IO.Directory.EnumerateFiles(dir).Any();
            if (!isEmpty)
            {
                var directory = new System.IO.DirectoryInfo(dir);
                files = directory.GetFiles()
                                 .Where(s => s.Extension.EndsWith(".rst"))
                                 .OrderBy(file => file.LastWriteTime)
                                 .Select(file => file.Name).ToList();
            }
            return files;
        }

        /// <summary>
        /// Get the folder that contains the RST files
        /// </summary>
        /// <returns></returns>
        private static string GetRSTFolderPath()
        {
            // For Example - D:\Projects\SomeProject\SomeFolder
            string Value =  (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings["UserProfileDirectory"] != null) ? ConfigurationManager.AppSettings["UserProfileDirectory"].ToString() : string.Empty;
            if (string.IsNullOrEmpty(Value) || string.IsNullOrWhiteSpace(Value))
                throw new Exception("Verify the configuration item 'UserProfileDirectory' is present and has a value that represents an existing directory.");
            if ( System.IO.Directory.Exists(Value) == false )
                throw new Exception("The configuration item 'UserProfileDirectory' is pointing to a NON-EXISTANT directory, please correct.");
            return Value;
        }

        /// <summary>
        /// Returns a value to determine if we should do cleanup - meaning RST files that exist for user not in database are deleted from disk
        /// </summary>
        /// <returns></returns>
        private static bool GetDirectoryCleanUp()
        {
            try
            {
                string Value = (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings["DirectoryCleanUp"] != null) ? ConfigurationManager.AppSettings["DirectoryCleanUp"].ToString() : string.Empty;
                return System.Convert.ToBoolean(Value);
            }
            catch (Exception)
            {
                throw new Exception("Verify the configuration item 'DirectoryCleanUp' is present and has a value of true/false");
            }
        }

        /// <summary>
        /// Get the max number of RST files allowed in the system for this run
        /// </summary>
        /// <returns></returns>
        private static int GetRSTFileGenerateCounter()
        {
            try
            {
                string Value = (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings["RSTFileCounter"] != null) ? ConfigurationManager.AppSettings["RSTFileCounter"].ToString() : string.Empty;
                return System.Convert.ToInt32(Value);
            }
            catch (Exception)
            {
                throw new Exception("Verify the configuration item 'RSTFileCounter' is present and has a numeric value 0 or greater");
            }
        }

        /// <summary>
        /// Get the expiration date in days of how long a user's password is GOOD for, if password change date is older than this, they must change it
        /// </summary>
        /// <returns></returns>
        private static int GetPasswordExpirationDurationCounter()
        {
            try
            {
                string Value = (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings["PasswordExpiresInDays"] != null) ? ConfigurationManager.AppSettings["PasswordExpiresInDays"].ToString() : string.Empty;
                return System.Convert.ToInt32(Value);
            }
            catch (Exception)
            {
                throw new Exception("Verify the configuration item 'PasswordExpiresInDays' is present and has a numeric value 0 or greater");
            }
        }


        #endregion

     
    }
}
