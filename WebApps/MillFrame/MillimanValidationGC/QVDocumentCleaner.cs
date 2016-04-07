using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMValidationGC
{
    public class QVDocumentCleaner
    {
        private const string GCDirectory = "_garbagecollected";
        private string _DocumentFolder;

        public string DocumentFolder
        {
            get { return _DocumentFolder; }
            set { _DocumentFolder = value; }
        }
        public QVDocumentCleaner(string DocumentFolder)
        {
            _DocumentFolder = DocumentFolder;

            AllFiles = new List<string>();
            RequiredFiles = new List<string>();
            NonRequiredFiles = new List<string>();
        }

        public List<string> AllFiles { get; set; }

        public List<string> RequiredFiles { get; set; }

        public List<string> NonRequiredFiles { get; set; }

        /// <summary>
        /// Get a list of all the files in the directory with the sepecified extension, but excluding other extensions
        /// </summary>
        /// <param name="WithExtension"></param>
        /// <param name="ExcludingExtension"></param>
        /// <returns></returns>
        private List<string> Getfiles(string WithExtension, string ExcludingExtension, System.IO.SearchOption Search)
        {
            List<string> Files = System.IO.Directory.GetFiles(_DocumentFolder, WithExtension, Search).ToList();
            if (string.IsNullOrEmpty(ExcludingExtension) == false)
            {
                if (ExcludingExtension.StartsWith(@".") == false)
                    ExcludingExtension = "." + ExcludingExtension;
                for (int Index = Files.Count - 1; Index >= 0; Index--)
                {
                    if (string.Compare(ExcludingExtension, System.IO.Path.GetExtension(Files[Index]), true) == 0)
                        Files.RemoveAt(Index);
                }
            }
            return Files;
        }

        public bool IsProjectDirectory()
        {
            List<string> Projects = Getfiles("*.hciprj", "", System.IO.SearchOption.TopDirectoryOnly);
            return Projects.Count > 0;
        }

        public bool Process(bool AutoDelete = false)
        {
            List<string> Projects = Getfiles("*.hciprj", "", System.IO.SearchOption.TopDirectoryOnly);
            if (Projects.Count == 0)
                return true;  //this is not a dir that has an projects 

            AllFiles = Getfiles("*","hciprj", System.IO.SearchOption.AllDirectories);
            RequiredFiles = new List<string>();

            foreach (string Project in Projects)
            {
                ProcessProject(Project, AllFiles, RequiredFiles);
 
            }
            //we have a collective list of all files needed, create a list of what is NOT needed
            foreach (string File in AllFiles)
            {
                bool Required = false;
                foreach( string RequiredEntry in RequiredFiles)
                {
                    if ( string.Compare(RequiredEntry, File, true) == 0 )
                    {
                        Required = true;
                        break;
                    }
                }

                if (Required == false)
                {
                    NonRequiredFiles.Add(File);
                }
            }


            //what a boat load of crap just because we do not have access to the .Net 4.5 zip compressions  :-(
            if (AutoDelete && (NonRequiredFiles.Count > 0))
            {
                return Cleanup();
            }
            return true;
        }

        public bool Cleanup()
        {
            try
            {
                if (NonRequiredFiles.Count == 0)
                    return true; //nothing to do

                string QVRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                string tempDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
                System.IO.Directory.CreateDirectory(tempDirectory);

                //create a path report file, this shows the original locaation of each item deleted
                string MyPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N"));
                System.IO.Directory.CreateDirectory(MyPath);
                string PathFile = System.IO.Path.Combine(MyPath, "GC_File_Paths.txt");

                NonRequiredFiles.Add(PathFile);  //add to list to be copied over

                //copy to temp dir
                foreach (string File in NonRequiredFiles)
                {
                    if (string.Compare(File, PathFile, true) != 0) //dont write an entry fo the path file itself
                        System.IO.File.AppendAllText(PathFile, File + Environment.NewLine);
                    System.IO.File.Copy(File, System.IO.Path.Combine(tempDirectory, System.IO.Path.GetFileName(File)));
                }
                string GCFolder = System.IO.Path.Combine(_DocumentFolder, GCDirectory);
                System.IO.Directory.CreateDirectory(GCFolder);
                string GCZip = System.IO.Path.Combine(GCFolder, "GCedON_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".zip");
                System.IO.Compression.ZipFile.CreateFromDirectory(tempDirectory, GCZip, System.IO.Compression.CompressionLevel.Optimal, false);

                if (System.IO.File.Exists(GCZip))
                {
                    foreach (string File in NonRequiredFiles)
                        System.IO.File.Delete(File);

                    //get rid of empty dirs
                    string[] Dirs = System.IO.Directory.GetDirectories(_DocumentFolder, "*", System.IO.SearchOption.AllDirectories);
                    foreach (string Dir in Dirs)
                    {
                        if (System.IO.Directory.GetFiles(Dir).Count() == 0)
                            System.IO.Directory.Delete(Dir);
                    }
                }
                System.IO.Directory.Delete(tempDirectory, true);
                return true;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "GC-unspecified error", ex);
            }
            return false;
        }


        /// <summary>
        /// If the file exists, add to list of files to keep, if the root is empty, then
        /// Item is already fully qualfied
        /// </summary>
        /// <param name="RootPath"></param>
        /// <param name="Item"></param>
        /// <param name="RequiredFiles"></param>
        private void AddIfExists(string RootPath, string Item, List<string> RequiredFiles)
        {
            string QualfieddPathName = Item;
            if ( string.IsNullOrEmpty(RootPath) == false )
                QualfieddPathName =  System.IO.Path.Combine(RootPath, Item);
            if (System.IO.File.Exists(QualfieddPathName))
                RequiredFiles.Add(QualfieddPathName);
        }

        private bool UserExistsFromPath(string QualafiedItem)
        {
            if (QualafiedItem.ToLower().Contains(@"\reduceduserqvws\"))
            {
                string[] Tokens = QualafiedItem.Split(new char[] { '\\' });
                string UserName = string.Empty;
                for (int Index = 0; Index < Tokens.Count(); Index++)
                {
                    if (string.Compare(Tokens[Index], "reduceduserqvws", true) == 0)
                    {
                        UserName = MillimanCommon.Utilities.ConvertHexToString(Tokens[Index + 1]);
                        break;
                    }
                }
                return System.Web.Security.Membership.GetUser(UserName) != null;
            }
            return false;
        }
        private void ProcessProject(string Project, List<string> AllFiles, List<string> AllRequiredFiles)
        {
            try
            {
                MillimanCommon.CustomUserDownloads CUD = MillimanCommon.CustomUserDownloads.GetInstance();
                if ((CUD == null) || (CUD.LoadedSuccessfully == false))
                    throw new Exception("GC-Custom user downloads did not load.");

                MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(Project);
                if (PS == null)
                    throw new Exception("GC-Project " + Project + " failed to load");

                MillimanCommon.UserRepo Repo = MillimanCommon.UserRepo.GetInstance();
                if (Repo == null)
                    throw new Exception("GC-Could not load user repo");

                string QVRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                if (string.IsNullOrEmpty(QVRoot))
                    throw new Exception("GC-QVRoot is empty or null");

                string ProjectQVW = System.IO.Path.Combine(QVRoot, PS.VirtualDirectory, PS.ProjectName + ".qvw");
                bool CanEmit = false;

                if (Repo.IsQVWUsed(ProjectQVW) == true)
                {
                    MillimanCommon.XMLFileSignature XMLFS = new MillimanCommon.XMLFileSignature(ProjectQVW);
                    if (XMLFS.SignatureDictionary.ContainsKey("can_emit"))
                        CanEmit = System.Convert.ToBoolean(XMLFS.SignatureDictionary["can_emit"]);

                    AllRequiredFiles.Add(Project);
                    AllRequiredFiles.Add(ProjectQVW);
                    AllRequiredFiles.Add(ProjectQVW + ".Shared");
                    AllRequiredFiles.Add(ProjectQVW + ".Meta");
                    if (string.IsNullOrEmpty(PS.QVThumbnail) == false)
                    {
                        AddIfExists(PS.AbsoluteProjectPath, PS.QVThumbnail, AllRequiredFiles);
                    }
                    if (string.IsNullOrEmpty(PS.UserManual) == false)
                    {
                        AddIfExists(PS.AbsoluteProjectPath, PS.UserManual, AllRequiredFiles);
                    }
                    foreach (string File in AllFiles)
                    {
                        if (CanEmit)
                        {
                            string[] ValidEmitExtension = new string[] { "map_", "hierarcy_", "concept_" };
                            string Extension = System.IO.Path.GetExtension(File).ToLower();
                            foreach (string ValidExt in ValidEmitExtension)
                            {
                                if (Extension.Contains(ValidExt))
                                    AddIfExists("", File, AllRequiredFiles);
                            }
                        }
                        //keep all the history
                        if (File.ToLower().Contains(@"\history\"))
                            AddIfExists("", File, AllRequiredFiles);
                        //add previous GC zips as required, don't want to get rid of them
                        if (File.ToLower().Contains(@"\" + GCDirectory + @"\"))
                            AddIfExists("", File, AllRequiredFiles);

                        if (File.ToLower().Contains(@"\reduceduserqvws\"))
                        {
                            //if the user no longer exists, we don't need this file anymore
                            if (UserExistsFromPath(File))
                                AddIfExists("", File, AllRequiredFiles);
                        }
                        //technically this is incorrect,  however there is a design flaw that forces to accept all files
                        //in the cache and not do GC
                        if (File.ToLower().Contains(@"\reducedcachedqvws\"))
                            AddIfExists("", File, AllRequiredFiles);

                        if ((File.ToLower().Contains(@"_data\")) || (File.ToLower().Contains(@"\data\")))
                        {
                            string VirtualItem = File.Substring(QVRoot.Length);
                            if (VirtualItem.StartsWith(@"\") || (VirtualItem.StartsWith(@"/")))
                                VirtualItem = VirtualItem.Substring(1);
                            if (CUD.ItemIsReferenced(VirtualItem))
                                AddIfExists("", File, AllRequiredFiles);
                        }
                    }
                }
                else
                {
                    //since this is not used,  add the project file to the file list so it will be garbage collected too
                    AllFiles.Add(Project);
                }
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, ex.Message, ex);
                RequiredFiles = AllFiles;  //make all files required, don't go any GC if something went wrong
            }
 
            
        }
    }
}
