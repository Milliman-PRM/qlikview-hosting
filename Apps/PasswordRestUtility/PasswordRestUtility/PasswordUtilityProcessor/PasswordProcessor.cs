using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
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
        public static void ExecutePasswordRestUtility(string args)
        {
            try
            {
                Process(args);
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, "ExecutePasswordRestUtility || " + args.ToArray());
            }
        }
        private static void Process(string args)
        {
            if ((args != null) && (args.Length != 0) && (args != ""))
            {
                //check if all
                if (args.Contains("All") == true)
                {
                    ProcessAllUsers();
                }
                else
                {
                    ProcessUser(args);
                }
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

                foreach (MembershipUser user in usersCollection)
                {
                    ProcessUser(user.UserName);
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
                    return;
                }

                //paswwrod exipration duration
                var passwordExpiresInDays = int.Parse(ConfigurationManager.AppSettings["PasswordExpiresInDays"].ToString());

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
                            BaseFileProcessor.LogError(null, "ProcessUser || Invalid user. System could not find the providerUserKey " + providerUserKey + ". Check the 'UserId' in [aspnet_Users] for the userName " + user.UserName + ".");
                            return;
                        }
                        WrtiePasswordResetFile(user, providerUserKey.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, "ProcessUser");
            }
        }

        private static void WrtiePasswordResetFile(MembershipUser user, string providerUserKey)
        {
            //get the log directory
            var userProfileDirectory = ConfigurationManager.AppSettings["UserProfileDirectory"];
            //write file
            var userPasswordRestFileAndDirectory = (userProfileDirectory + providerUserKey.ToUpper() + ".rst");
            //Check file
            if (File.Exists(userPasswordRestFileAndDirectory))
            {
                File.Delete(userPasswordRestFileAndDirectory);
            }
            //create the file name
            var message = (DateTime.Now + " || " + "User Needs to Reset Password. User Name: " + user.UserName);
            File.WriteAllText(userPasswordRestFileAndDirectory, message);
        }

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
            prov.Initialize("AspNetSqlMembershipProvider", coll);
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
