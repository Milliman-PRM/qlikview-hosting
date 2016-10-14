using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ionic.Zip;

namespace ClientPublisher
{
    public class PublisherUtilities
    {
        static public string MakeSafeForFileSystemName( string Name )
        {
            string ValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-";
            string ValidFSN = string.Empty;
            foreach (char Item in Name)
            {
                if (ValidChars.Contains(Item))
                    ValidFSN += Item;
                else
                    ValidFSN += '_'; //replace all those other chars with underscore
            }
            return ValidFSN;
        }

        /// <summary>
        /// based on parameters, get and/or create working directory for user
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="ProjectName"></param>
        /// <returns></returns>
        static public string GetWorkingDirectory(string UserName, string ProjectName )
        {

            string ClientPubDocuments = System.Web.Configuration.WebConfigurationManager.AppSettings["ClientPublisherQVDocuments"];
            string ScrubbedUserName = PublisherUtilities.MakeSafeForFileSystemName(UserName);
            string ScrubbedProjectName = PublisherUtilities.MakeSafeForFileSystemName(ProjectName);
            string WorkingDirectory = System.IO.Path.Combine(ClientPubDocuments, ScrubbedUserName, ScrubbedProjectName);
            if (System.IO.Directory.Exists(WorkingDirectory) == false)
                System.IO.Directory.CreateDirectory(WorkingDirectory);

            return System.IO.Directory.Exists(WorkingDirectory) ? WorkingDirectory : "";
        }

        /// <summary>
        /// Dump a backup into a zip that is password protected
        /// </summary>
        /// <param name="DirToBackup">Full path of dir to backup</param>
        /// <param name="SaveZipToDir">Directory on where the backup should be written</param>
        /// <returns></returns>
        static public string CreateBackupSet(string DirToBackup, string SaveZipToDir, System.IO.SearchOption Options = System.IO.SearchOption.TopDirectoryOnly )
        {

            try
            {
                System.IO.Directory.CreateDirectory(SaveZipToDir);

                string ZID = System.Guid.NewGuid().ToString("N");
                using (ZipFile zip = new ZipFile())
                {
                    zip.Password = "M" + ZID;
                    zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                    string[] AllFiles = System.IO.Directory.GetFiles(DirToBackup, "*.*", Options);

                    string[] InvalidExtension = new string[] { ".meta", ".shared", ".zip" };
                    foreach (string S in AllFiles)
                    {
                        string Extension = System.IO.Path.GetExtension(S);
                        if (InvalidExtension.Contains(Extension.ToLower()) == false)
                        {
                            if (Extension.ToLower().IndexOf("_tmp") == -1)
                            {
                                zip.AddFile(S, "");
                            }
                        }
                    }
                    zip.Save(System.IO.Path.Combine(SaveZipToDir, ZID + ".zip"));
                }
                return ZID;
            }
            catch (Exception)
            {

            }

            return "";
        }

        /// <summary>
        /// Restore a backup and clean away _tmp files
        /// </summary>
        /// <param name="DirToRestoreFromBackup">The directory we need restored</param>
        /// <param name="BackupSet">The backup zip fully qualified</param>
        /// <returns></returns>
        static public bool RestoreBackupSet(string DirToRestoreFromBackup, string BackupSet, System.IO.SearchOption Options = System.IO.SearchOption.TopDirectoryOnly)
        {
            try
            {
                if (System.IO.File.Exists(BackupSet))
                {
                    string Pswd = "M" + System.IO.Path.GetFileNameWithoutExtension(BackupSet);
                    string[] AllFiles = System.IO.Directory.GetFiles(DirToRestoreFromBackup, "*.*", Options);
                    foreach (string S in AllFiles)
                    {
                        string Extension = System.IO.Path.GetExtension(S);
                        if (Extension.ToLower().IndexOf("_tmp") != -1)
                        {
                            System.IO.File.Delete(S);
                        }
                    }

                    using (ZipFile zip = ZipFile.Read(BackupSet))
                    {
                        zip.Password = Pswd;
                        zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                        foreach (ZipEntry e in zip)
                        {
                            e.Extract(DirToRestoreFromBackup, ExtractExistingFileAction.OverwriteSilently);
                        }
                    }
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }


    }
}