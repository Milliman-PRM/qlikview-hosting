using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanReportReduction
{
    /// <summary>
    /// This class is used to cache reduced QVWS such that user can share QVWs loaded by the
    /// QVW server, this will increase the load capabilities of the server using QV server
    /// </summary>
    public class QVWCaching
    {
        private string QualifiedMasterQVW { get; set; }
        private string QualifiedProjectPath { get; set; }
        private string QualifiedReducedPath { get; set; }

        private string QualifiedCachedQVWsPath { get; set; }

        public QVWCaching(string _QualifiedMasterQVW)
        {
            QualifiedMasterQVW = _QualifiedMasterQVW;
            QualifiedProjectPath = System.IO.Path.GetDirectoryName(QualifiedMasterQVW);
            QualifiedReducedPath = System.IO.Path.Combine(QualifiedProjectPath, "ReducedUserQVWs");
            QualifiedCachedQVWsPath = System.IO.Path.Combine(QualifiedProjectPath, "ReducedCachedQVWs");

            if (System.IO.Directory.Exists(QualifiedReducedPath) == false)
                System.IO.Directory.CreateDirectory(QualifiedReducedPath);
            if (System.IO.Directory.Exists(QualifiedCachedQVWsPath) == false)
                System.IO.Directory.CreateDirectory(QualifiedCachedQVWsPath);
        }

        /// <summary>
        /// Find a cached reduced QVW based on the selection made by the user
        /// </summary>
        /// <param name="QualifiedReductionSelectionFilename">Qualfied path and filename of the selections file on disk</param>
        /// <returns>Full qualified path and QVW filename OR empty string if not found</returns>
        public string FindReducedQVW(string QualifiedReductionSelectionFilename)
        {
            long FileSize = new System.IO.FileInfo(QualifiedReductionSelectionFilename).Length;
            string FileSizePrefix = FileSize.ToString();
            FileSizePrefix = FileSizePrefix.Substring(0, FileSizePrefix.Length-2) + "*";
            string SearchFilename = "*_" + FileSizePrefix + ".cacheindex";
            /// Care_Coordinator_Report_39*

            string[] CandidateFiles = System.IO.Directory.GetFiles(QualifiedCachedQVWsPath, SearchFilename, System.IO.SearchOption.AllDirectories);
            if (CandidateFiles.Length > 0)
            {
                //at least one file has the correct length settings
                //get the selection file MD5 and compare with contents of files
               string SelectionHash = MillimanCommon.Utilities.CalculateMD5Hash(QualifiedReductionSelectionFilename, true);
               foreach (string File in CandidateFiles)
               {
                   //file format  QVW|MD5Hash
                   string FileContents = System.IO.File.ReadAllText(File);
                   string[] FileItems = FileContents.Split(new char[] { '|' });
                   if (string.Compare(SelectionHash, FileItems[1], true) == 0)
                       return FileItems[0];
               }
            }
            return ""; ///didnt find anything
        
        }

        /// <summary>
        /// Copies the QualifiedReducedQVWFilename to the cache and adds the index file
        /// </summary>
        /// <param name="QualifiedReductionSelectionFilename">Selections made by user for reduction file</param>
        /// <param name="QualifiedReducedQVWFilename">QVW to be copied to cache</param>
        /// <returns>Referenced QVW in cache</returns>
        public string MoveNewQVWToCache(string QualifiedReductionSelectionFilename, string QualifiedReducedQVWFilename, string UseGUID = "")
        {
            string QVWQuid = UseGUID;
            if ( string.IsNullOrEmpty(QVWQuid))
                QVWQuid = System.Guid.NewGuid().ToString("N");
            string CachedQVWFilename = System.IO.Path.Combine(QualifiedCachedQVWsPath, QVWQuid + ".qvw");
 
            string SelectionMD5 = MillimanCommon.Utilities.CalculateMD5Hash(QualifiedReductionSelectionFilename, true);
            long SelectionFilenameLength = new System.IO.FileInfo(QualifiedReductionSelectionFilename).Length;
            string CandidateFileName = System.IO.Path.Combine(QualifiedCachedQVWsPath, System.IO.Path.GetFileNameWithoutExtension(QualifiedReductionSelectionFilename) + "_" + SelectionFilenameLength.ToString() + ".cacheindex");
 
            int Index = 0;
            //sometimes there will be difference selection critera, but the selection file will be same lenght, this loop create a unique name
            while (System.IO.File.Exists(CandidateFileName) == true)
            {
                CandidateFileName = System.IO.Path.Combine(QualifiedCachedQVWsPath, System.IO.Path.GetFileNameWithoutExtension(QualifiedReductionSelectionFilename) + "_" + SelectionFilenameLength.ToString() + "_" + Index.ToString() + ".cacheindex");
                Index++;
 
            }
            System.IO.File.Copy(QualifiedReducedQVWFilename, CachedQVWFilename);
            System.IO.File.Delete(QualifiedReducedQVWFilename);  //we need to get rid of it, since it reduced in in the user's dir, but it has been copied to cache

            System.IO.File.WriteAllText(CandidateFileName, CachedQVWFilename + "|" + SelectionMD5);
            return CachedQVWFilename;
        }


        //public void ProcessOld(string QualifiedRootDir)  //D:\InstalledApplications\PRM\QVDocuments\0273WOH01\Medicaid\WOAH
        //{
        //    string[] SelectionFiles = System.IO.Directory.GetFiles(QualifiedRootDir, "*.selections", System.IO.SearchOption.AllDirectories);
        //    foreach (string SelectionFile in SelectionFiles)
        //    {
        //        string Cached = FindReducedQVW(SelectionFile);
        //        if (string.IsNullOrEmpty(Cached))
        //        {
        //            string QualifiedQVW = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SelectionFile), "Care Coordinator Report.qvw");
        //            string CachedQVW = MoveNewQVWToCache(SelectionFile, QualifiedQVW);
        //            string Redirector = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SelectionFile), "[REFERENCE].redirect");
        //            System.IO.File.WriteAllText(Redirector, CachedQVW);

        //        }
        //        else
        //        {
        //            string Redirector = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SelectionFile), "[REFERENCE].redirect");
        //            System.IO.File.WriteAllText(Redirector, Cached);
        //        }
        //    }
        //}

        /// <summary>
        /// Write a redirector to the correct location, accepts absolute path and makes relative to QV doc root
        /// </summary>
        /// <param name="MasterQVW">Master QVW</param>
        /// <param name="User">User account name</param>
        /// <param name="RedirectTo">QVW to redirect to, typically in cache</param>
        /// <returns>true on success, false otherwise</returns>
        static public bool WriteQVWRedirector(string MasterQVW, string User, string RedirectTo, bool AsTemp = false)
        {
            string ReducedQVWName = MillimanCommon.ReducedQVWUtilities.GetUsersReducedQVWName(MasterQVW, User);
            string RedirectFilename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ReducedQVWName), "[REFERENCE].redirect");
            if (AsTemp)
                RedirectFilename += "_new";

            if (System.IO.File.Exists(RedirectFilename))
                System.IO.File.Delete(RedirectFilename);

            //need to make redirectfile relative to QV root and not starting with slash
            string QVRoot = System.Web.Configuration.WebConfigurationManager.AppSettings["QVDocumentRoot"];
            if (RedirectTo.Contains(':') == true) //must be absolute path, otherwise just use it
            {
                RedirectTo = RedirectTo.Substring(QVRoot.Length);
                if (RedirectTo.StartsWith("\\") || RedirectTo.StartsWith("/"))
                    RedirectTo = RedirectTo.Substring(1);
            }
            System.IO.File.WriteAllText(RedirectFilename, RedirectTo);
            return true;
        }

        /// <summary>
        /// returns the path to the file in the cache, relative path
        /// </summary>
        /// <param name="MasterQVW"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        static public string ReadQVWRedirector(string MasterQVW, string User)
        {
            string ReducedQVWName = MillimanCommon.ReducedQVWUtilities.GetUsersReducedQVWName(MasterQVW, User);
            string RedirectFilename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ReducedQVWName), "[REFERENCE].redirect");
            if (System.IO.File.Exists(RedirectFilename))
            {
                return System.IO.File.ReadAllText(RedirectFilename);
            }
            return "";
        }
        static public bool RedirectorExists(string MasterQVW, string User)
        {
            string ReducedQVWName = MillimanCommon.ReducedQVWUtilities.GetUsersReducedQVWName(MasterQVW, User);
            string RedirectFilename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ReducedQVWName), "[REFERENCE].redirect");
            return System.IO.File.Exists(RedirectFilename);
        }
    }
}
