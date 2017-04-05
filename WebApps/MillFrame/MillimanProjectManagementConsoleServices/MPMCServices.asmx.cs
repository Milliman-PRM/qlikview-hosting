using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.IO;
using System.IO.Compression;
using Ionic.Zip;

namespace MillimanProjectManagementConsoleServices
{
    /// <summary>
    /// Summary description for MPMCServices
    /// </summary>
    [WebService(Namespace = "http://HCIntel.Cloudapp.net/MPMCS")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MPMCServices : System.Web.Services.WebService
    {
        //we only accept conntections from these IP addresses
        private List<string> IPs = null;
        private bool IsValidConnection()
        {
            //return true;
            //load one if not already loaded
            if (IPs == null)
            {
                IPs = new List<string>();
                string ValidIPS = System.Configuration.ConfigurationManager.AppSettings["ValidIPs"];
                IPs.AddRange(ValidIPS.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList());
            }
            return (IPs.Contains( HttpContext.Current.Request.UserHostAddress ) || (IPs.Contains(HttpContext.Current.Request.Url.Host)) );
        }

        /// <summary>
        /// Verify the users account and password and they must be in the
        /// "administrators" group
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Pswd"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public bool ValidUser( string Name, string Pswd )
        {
            if (IsValidConnection())
            {
                if ( (string.IsNullOrEmpty(Name)) || (string.IsNullOrEmpty(Pswd)) )
                    return false;

                MembershipUserCollection MUC = Membership.FindUsersByName(Name);
                if (MUC.Count == 0)
                    return false;
                MillimanCommon.AutoCrypt AC = new MillimanCommon.AutoCrypt();
                if (Membership.ValidateUser(Name, AC.AutoDecrypt(Pswd)) == true)
                    return Roles.IsUserInRole(Name, "Administrator");
            }
            return false;
        }

        /// <summary>
        /// Get all the groups available
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public List<string> GetGroups()
        {
            if (IsValidConnection())
            {
                return Roles.GetAllRoles().ToList();
            }
            return new List<string>();
        }

        /// <summary>
        /// create a group and return a status message
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string CreateGroup(string Name)
        {
            if (IsValidConnection())
            {
                if (Roles.RoleExists(Name) == true)
                    return "Group already exists.";
                try
                {
                    Roles.CreateRole(Name);
                    return "Group '" + Name + "' created.";
                }
                catch (Exception)
                {
                    return "Group '" + Name + "' could not be created - ensure the group name does not contain special characters.";
                }
            }
            return "";
        }

        /// <summary>
        /// delete a group and return a status message
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string DeleteGroup(string Name)
        {
            if (IsValidConnection())
            {
                if (Roles.RoleExists(Name) == true)
                {
                    try
                    {
                        string[] AllUsersForRole = Roles.GetUsersInRole(Name);
                        if ( (AllUsersForRole != null ) && (AllUsersForRole.Count() != 0) )
                        {
                            return "Group '" + Name + "' could not be deleted due to " + AllUsersForRole.Count().ToString() + " users in the role";
                        }

                        if (Roles.DeleteRole(Name) == true)
                            return "Group '" + Name + "' has been deleted";
                        else
                            return "Group '" + Name + "' could not be deleted";

                    }
                    catch (Exception)
                    {
                        return "Group '" + Name + "' could not be created - ensure the group name does not contain special characters.";
                    }
                }
                else
                {
                    return "Group '" + Name + "' does not exist";
                }
            }
            return "";
        } 

   

        /// <summary>
        /// Push item to server
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string PutBinarySlice(string VirtualPath, byte[] Item, long ItemLength, bool DeleteBeforeWrite, bool CreateDirectory)
        {
            if (IsValidConnection())
            {
                try
                {
                    string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                    //DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));

                    string UploadingFile = System.IO.Path.Combine(DocumentRoot, VirtualPath);
                    if (CreateDirectory)
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(UploadingFile));
                    }
                    if (DeleteBeforeWrite)
                    {
                        System.IO.File.Delete(UploadingFile);
                    }

                    using (var fileStream = new FileStream(UploadingFile, FileMode.Append, FileAccess.Write, FileShare.None))
                    using (var bw = new BinaryWriter(fileStream))
                    {
                        bw.Write(Item, 0, (int)ItemLength);
                    }

                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
            return "";
        }

        private bool RenameFile(string From, string To)
        {
            if (System.IO.File.Exists(From))
            {
                System.IO.File.Delete(To);
                System.IO.File.Move(From, To);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Dump a backup into a zip that is password protected
        /// </summary>
        /// <param name="DirToBackup">Full path of dir to backup</param>
        /// <param name="SaveZipToDir">Directory on where the backup should be written</param>
        /// <returns></returns>
        public string CreateBackupSet(string DirToBackup, string SaveZipToDir)
        {

            try
            {
                System.IO.Directory.CreateDirectory(SaveZipToDir);

                string ZID = System.Guid.NewGuid().ToString("N");
                using (ZipFile zip = new ZipFile())
                {
                    zip.Password = "M" + ZID;
                    zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                    string[] AllFiles = System.IO.Directory.GetFiles(DirToBackup);
                    string[] InvalidExtension = new string[] { ".meta", ".shared" };
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
        public bool RestoreBackupSet(string DirToRestoreFromBackup, string BackupSet)
        {
            try
            {
                if (System.IO.File.Exists(BackupSet))
                {
                    string Pswd = "M" + System.IO.Path.GetFileNameWithoutExtension(BackupSet);
                    string[] AllFiles = System.IO.Directory.GetFiles(DirToRestoreFromBackup);
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

        /// <summary>
        /// Copy all the icons back to download data dir
        /// </summary>
        /// <param name="DataDir"></param>
        private void CopyIconDownloadImages(string DataDir)
        {
            //get the images directory
            try
            {
                string ImageDir = Server.MapPath("Images");
                string[] AllImages = System.IO.Directory.GetFiles(ImageDir, "*-icon.png", SearchOption.TopDirectoryOnly);
                foreach (string IconFile in AllImages)
                {
                    System.IO.File.Copy(IconFile, DataDir);
                }
            }
            catch (Exception ex)
            {
               MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to copy icon file to download dir", ex);
            }
        }

    
        /// <summary>
        /// How does this work
        /// We always upload specific files that will have a _tmp extention
        ///     project file
        ///     thumbnail file
        ///     user guide ( if exists )
        /// We always upload the contents of the download directory, the prod and ss01 dirs should stay in sync thus we delete
        /// each time and rebuild
        /// If we upload the master QVW,  then we delete the contents cache and user dirs and rebuild from tmp
        /// Note: this has a design flaw it that multiple QVWs that are client admined within the same group can delete files 
        /// that are needed between the instances
        /// 
        /// </summary>
        /// <param name="PS"></param>
        /// <returns></returns>
        private bool Update(MillimanCommon.ProjectSettings PS)
        {
            string QVWFile = System.IO.Path.GetDirectoryName(PS.LoadedFrom);
            //create a directory to hold historical items, items appened with same guid are related
            string HistoryDir = System.IO.Path.Combine(QVWFile, "history");
            System.IO.Directory.CreateDirectory(HistoryDir);
            bool ContainsMasterQVW = false;
            try
            {
                string ZID = CreateBackupSet(PS.LoadedFromPath, System.IO.Path.Combine(PS.LoadedFromPath, "history"));
                if (string.IsNullOrEmpty(ZID) == false)
                {
                    try
                    {
                        string[] AllFiles = System.IO.Directory.GetFiles(PS.LoadedFromPath, "*", SearchOption.AllDirectories);
                        //look to see if master QVW uploaded, if so we must process reduced files too
                        string[] TMPFiles = System.IO.Directory.GetFiles(PS.LoadedFromPath, "*_tmp", SearchOption.TopDirectoryOnly);
                        string MasterQVWName = PS.ProjectName.ToLower() + ".qvw_tmp";
                        foreach (string ItemName in TMPFiles)
                        {
                            if (ItemName.ToLower().EndsWith(MasterQVWName) )
                            {
                                ContainsMasterQVW = true;
                                break;
                            }
                        }
                        //clear out the user downloads, they will be re-uploaded
                        string DataDir = System.IO.Path.Combine(QVWFile, PS.ProjectName + "_data");
                        if (System.IO.Directory.Exists(DataDir) == false)
                        {
                            System.IO.Directory.CreateDirectory(DataDir);
                        }

                        //PATCH:  we need the icons in the directory, so recopy them
                        CopyIconDownloadImages(DataDir);

                        string[] DataDirEntries = System.IO.Directory.GetFiles(DataDir, "*", SearchOption.TopDirectoryOnly);
                        foreach (string DDE in DataDirEntries)
                        {   //get rid of all NON tmp files they are old,  we just uploaded the TMPs.
                            string Extension = System.IO.Path.GetExtension(DDE).ToLower();
                            if (Extension.EndsWith("_tmp") == false)
                                System.IO.File.Delete(DDE);
                        }

                        foreach (string AF in AllFiles)
                        {
                            if (System.IO.Path.GetExtension(AF).IndexOf("_tmp") != -1)
                            {
                                string NoTempInExtension = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(AF), System.IO.Path.GetFileNameWithoutExtension(AF) + System.IO.Path.GetExtension(AF).Replace("_tmp", ""));
                                RenameFile(AF, NoTempInExtension);
                            }
                            else if ( ContainsMasterQVW ) //new files all end in _tmp, thus delete the old
                            {
                                //get rid of all cached files that are no longer needed
                                if ( AF.ToLower().Contains("\\reducedcachedqvws\\") || AF.ToLower().Contains("/reducedcachedqvws/"))
                                {
                                    if ( AF.ToLower().Contains(".qvw.shared") == false )  //don't get rid of bookmark files, the new QVW have same name and thus reuse the bookmarks
                                        System.IO.File.Delete(AF);  //old file get rid of it
                                }
                            }
                        }

                        return true;
                    }
                    catch (Exception ex)
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to update project - rolled back", ex);
                        //roll back
                    }

                    RestoreBackupSet(PS.LoadedFromPath, System.IO.Path.Combine(PS.LoadedFromPath, @"history\" + ZID + @".zip"));
                }
                else
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Could not create backup set when uploading project");
                }
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified exception when uploading project", ex);
            }
            
            return false;
        }

        /// <summary>
        /// Get item from server
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string ProcessProject(string VirtualPath)
        {
            string Status = "";
            if (IsValidConnection())
            {

                string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
               // DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));

                string UploadingFile = System.IO.Path.Combine(DocumentRoot, VirtualPath);
                MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(UploadingFile + "_tmp");  //uploaded as temp file

                if (PS == null)
                {
                    return @"Error: Server could not load project file - " + UploadingFile;
                }
                try
                {
                    if (Update(PS) == true)
                    {
                        Status += "\nProject updated successfully";
                    }
                    else
                    {
                        Status += "\nError: Project failed to update";
                    }
                }
                catch (Exception Ex)
                {
                    return "Error: " + Ex.ToString();
                }

                bool Verified = false;
                MillimanReportReduction.QVSAPI QV = new MillimanReportReduction.QVSAPI();
                //we should give up to 10 seconds for the server to recognize the QVW so we
                //can authenticate it
                for (int Index = 0; Index < 10; Index++)
                {
                    Status = QV.AuthorizeAllDocuments(PS.QVName, out Verified);
                    if ((Verified) || (string.IsNullOrEmpty(Status) == false ))
                        break;
                    System.Threading.Thread.Sleep(1000);
                }
                if (string.IsNullOrEmpty(PS.Groups) == false)
                {
                    try
                    {
                        System.Web.Configuration.AuthorizationRule newRule = new System.Web.Configuration.AuthorizationRule(System.Web.Configuration.AuthorizationRuleAction.Allow);
                        string[] GroupItems = PS.Groups.Split(new char[] { '~' });
                        foreach (string Group in GroupItems)
                        {
                            newRule.Roles.Add(Group);
                        }
                        bool RequiresPatientContext = false;
                        string SpecialCommand = "";
                        //put the QVW in the rules files QVW, project file and thumbnail all have same name
                        string QVVirtualPath = System.IO.Path.Combine(PS.VirtualDirectory, PS.QVName + ".qvw");
                        MillimanCommon.UserRepo.GetInstance().AddAuthorizationConfiguration(QVVirtualPath, newRule, RequiresPatientContext, SpecialCommand);
                        MillimanCommon.UserRepo.GetInstance().Save();
                    }
                    catch (Exception ex)
                    {
                        Status += "\nError: " + ex.ToString();
                    }

                }
                else
                {
                    Status += "\nProject was successfully published(but not associated to any groups)";
                }
            }
            return Status;
        }

        /// <summary>
        /// Get item from server
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public byte[] Get(string VirtualDirItem)
        {
            if (IsValidConnection())
            {
                string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                // DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));

                string RequestedFile = System.IO.Path.Combine(DocumentRoot, VirtualDirItem);
                if (File.Exists(RequestedFile))
                {
                    return System.IO.File.ReadAllBytes(RequestedFile);
                }
            }
            return null;
        }

        [WebMethod(EnableSession = true)]
        public List<string> GetAllProjectFiles(string VirtualDirectory)
        {
            List<string> VirtualList = new List<string>();
            if (IsValidConnection())
            {
                string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                // DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));
                string Root = System.IO.Path.Combine(DocumentRoot, VirtualDirectory);
                if (Directory.Exists(Root))
                {
                    string[] AllFiles = System.IO.Directory.GetFiles(Root, "*", SearchOption.AllDirectories);
                    foreach (string File in AllFiles)
                    {
                        if ( (File.ToLower().Contains("history") == false) && (File.ToLower().EndsWith(".zip") == false) &&
                             (File.ToLower().EndsWith(".meta") == false) && (File.ToLower().EndsWith(".shared") == false) )
                        {
                            string Virtual = File.Substring(DocumentRoot.Length);
                            if ( (Virtual.StartsWith("\\")) || (Virtual.StartsWith("/")) )
                                Virtual = Virtual.Substring(1);
                            VirtualList.Add(Virtual);
                        }
                    }
                }
            }
            return VirtualList;
        }

        /// <summary>
        /// Get a text file from the server
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string GetTextFile(string VirtualDirItem)
        {
            if (IsValidConnection())
            {
                string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                // DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));

                string RequestedFile = System.IO.Path.Combine(DocumentRoot, VirtualDirItem);
                if (File.Exists(RequestedFile))
                {
                    return System.IO.File.ReadAllText(RequestedFile);
                }
            }
            return null;
        }

        /// <summary>
        /// Calculate the hash if found
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string GetHash(string XMLQualifiedDirItem)
        {
            if (IsValidConnection())
            {
                string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                string AdjustedPath = System.IO.Path.Combine(DocumentRoot, XMLQualifiedDirItem);
                if (System.IO.File.Exists(AdjustedPath))
                    return MillimanCommon.Utilities.CalculateMD5Hash(AdjustedPath, true);
            }
            return "";
        }

        /// <summary>
        /// Pass in a list of virtual dir qualified files to get hash codes
        /// </summary>
        /// <param name="Files"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public List<string> GetFileListHash(List<string> VirtualQualifedFiles)
        {
            List<string> HashList = new List<string>();
            foreach (string F in VirtualQualifedFiles)
            {
                HashList.Add(MillimanCommon.Utilities.CalculateMD5Hash(F, true));
            }
            return HashList;
        }

        [WebMethod(EnableSession = true)]
        public string GetIndexsAndSelections(string VirtualPathAndProject, out bool ProjectMissing, out bool IsErrorCondition)
        {
            IsErrorCondition = false;
            ProjectMissing = false;
            try
            {
                Dictionary<string, string> IndexAndSelections = new Dictionary<string, string>();
                if (IsValidConnection())
                {
                    string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                    string AdjustedPath = System.IO.Path.Combine(DocumentRoot, VirtualPathAndProject);
                    if (System.IO.File.Exists(AdjustedPath) == false)
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Could not find project: " + AdjustedPath);
                        IsErrorCondition = true;
                        ProjectMissing = true;
                        return "Could not find project '" + VirtualPathAndProject + "'";
                    }

                    //include from project if autoinclusions are required
                    MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(AdjustedPath);
                    IndexAndSelections.Add("AutoInclude", PS.AutomaticInclusion.ToString());

                    //locate all the previous hierarchies and return
                    string HierRootName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(AdjustedPath), System.IO.Path.GetFileNameWithoutExtension(AdjustedPath)) + ".hierarchy_";
                    for (int Index = 0; true; Index++)
                    {
                        string HierName = HierRootName + Index.ToString();
                        if (System.IO.File.Exists(HierName))
                        {
                            IndexAndSelections.Add(HierName.Substring(DocumentRoot.Length+1), System.IO.File.ReadAllText(HierName));
                        }
                        else
                        {
                            break;
                        }
                    }

                    //get all selection files
                    string Path = System.IO.Path.GetDirectoryName(AdjustedPath);
                    string Filename = System.IO.Path.GetFileNameWithoutExtension(AdjustedPath) + ".selections";
                    string[] SelectionFiles = System.IO.Directory.GetFiles(Path, Filename, SearchOption.AllDirectories);
                    foreach (string SF in SelectionFiles)
                    {
                        IndexAndSelections.Add(SF.Substring(DocumentRoot.Length+1), System.IO.File.ReadAllText(SF));
                    }

                    //get all the redirect files too
                    string[] RedirectFiles = System.IO.Directory.GetFiles(Path, "*.redirect", SearchOption.AllDirectories);
                    foreach (string RF in RedirectFiles)
                    {
                        IndexAndSelections.Add(RF.Substring(DocumentRoot.Length + 1), System.IO.File.ReadAllText(RF));
                    }
                }

                Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);

                System.IO.MemoryStream MS = new MemoryStream();
                SS.Serialize(IndexAndSelections, MS);
                MS.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(MS);
                string XML = reader.ReadToEnd();

                return XML;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified error", ex);
                IsErrorCondition = true;
            }
            return "";
        }
    }
}
