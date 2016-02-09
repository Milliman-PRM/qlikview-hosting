using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemReporting.Utilities;
using SystemReporting.Utilities.File;
using SystemReporting.Utilities.ExceptionHandling;
using System.Security.Cryptography;
using SystemReporting.Controller;
using SystemReporting.Controller.BusinessLogic.Controller;
using SystemReporting.Utilities.Email;

namespace FileProcessor
{
    public class FileFunctions
    {
        #region File Direcotries and Locations
        /// <summary>
        /// This method retrieves the location where the files will be copied from to be processed
        /// </summary>
        /// <param name="eFilePath"></param>
        /// <returns>\\ProductionLogsTest\\eFilePath\\</returns>
        public static string GetFileOriginalSourceDirectory(EnumFileProcessor.eFilePath eFilePath)
        {
            // ProductionLogsTest\IISLogs\
            var appIISLogs = ConfigurationManager.AppSettings["IISLogsS"];
            var appMiscLogs = ConfigurationManager.AppSettings["MiscLogsS"];
            var appQVAuditLogs = ConfigurationManager.AppSettings["QVAuditLogsS"];
            var appQVSessionLogs = ConfigurationManager.AppSettings["QVSessionLogsS"];

            string source = string.Empty;
            //get the directory name
            switch (eFilePath)
            {
                case EnumFileProcessor.eFilePath.IisLogs:
                    source = appIISLogs;
                    break;
                case EnumFileProcessor.eFilePath.QVAuditLogs:
                    source = appQVAuditLogs;
                    break;
                case EnumFileProcessor.eFilePath.QVSessionLogs:
                    source = appQVSessionLogs;
                    break;
            }
            return (string.Format(source));
        }

        /// <summary>
        /// Returns the destination folder
        /// </summary>
        /// <returns>LogFileProcessor\IN\</returns>
        public static string GetFileProcessingInDirectory()
        {
            return (ConfigurationManager.AppSettings["ProcessingInDir"]);
        }

        /// <summary>
        /// Returns the status file location directory
        /// </summary>
        /// <returns>\ProductionLogsTest\SyncStatus\</returns>
        public static string GetStatusFileDirectory()
        {
            return (ConfigurationManager.AppSettings["FileStatus"]);
        }

        /// <summary>
        /// Returns the processed log file location directory
        /// </summary>
        /// <returns>\LogFileProcessor\BackUp\</returns>
        public static string GetProcessedFileLogDirectory()
        {
            return (ConfigurationManager.AppSettings["ProcessedFileLogDirectory"]);
        }

        /// <summary>
        /// Returns Processed file name
        /// </summary>
        /// <returns></returns>
        public static string GetProcessedFileLogFileName()
        {
            return (ConfigurationManager.AppSettings["ProcessedFileLogFileName"]);
        }

        #region ErrorLogger
        public static string GetLoggerFileDirectory()
        {
            return ConfigurationManager.AppSettings["LoggerFileDirectory"];
        }
        public static string GetLoggerFileName()
        {
            return ConfigurationManager.AppSettings["LoggerFileName"];
        }
        #endregion

        #endregion

        #region Get File Name
        /// <summary>
        /// Returns list of files
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<string> GetAllFileNamesFromDirectory(string dir, string filter)
        {
            List<string> files = new List<string>();
            bool isEmpty = !System.IO.Directory.EnumerateFiles(dir).Any();
            if (!isEmpty)
            {
                var directory = new System.IO.DirectoryInfo(dir);
                files = directory.GetFiles("*" + filter + "*")
                                 .Where(file => file.Name.StartsWith(filter))
                                 .OrderBy(file => file.LastWriteTime)
                                 .Select(file => file.Name).ToList();

            }

            return files;
        }

        /// <summary>
        /// Get list of files from status file
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="eFilePath"></param>
        /// <returns></returns>
        public static List<string> GetFileToReadFromStatusFile(string filter, EnumFileProcessor.eFilePath eFilePath)
        {
            //status file name with directory
            var statusFileName = ConfigurationManager.AppSettings["statusFileName"];
            var statusFileAndDirectory = (GetStatusFileDirectory() + statusFileName + ".txt");

            //backup file name with directory
            var processedLogFileName = ConfigurationManager.AppSettings["ProcessedFileLogFileName"];
            var processedLogFileAndDirectory = (GetProcessedFileLogDirectory() + processedLogFileName + ".log");

            var fileToRead = new List<string>();
            if (!string.IsNullOrEmpty(statusFileAndDirectory))
            {
                //get file name
                fileToRead = GetFileNameToReadFile(statusFileAndDirectory, processedLogFileAndDirectory, filter, eFilePath);
            }
            return fileToRead;
        }

        /// <summary>
        /// Compare the files and return the name of non-processed file
        /// </summary>
        /// <param name="statusFileAndDirectory"></param>
        /// <param name="processedLogFileAndDirectory"></param>
        /// <param name="filter"></param>
        /// <param name="eFilePath"></param>
        /// <returns></returns>
        public static List<string> GetFileNameToReadFile(string statusFileAndDirectory, string processedLogFileAndDirectory, string filter, EnumFileProcessor.eFilePath eFilePath)
        {
            if (string.IsNullOrEmpty(statusFileAndDirectory))
                return null;

            //Read status File
            var listStatusFileLines = System.IO.File.ReadAllLines(statusFileAndDirectory).ToList();

            //Read Processed Log File lines at the back up location
            FileCheck(processedLogFileAndDirectory);
            var listProcessedFileLines = System.IO.File.ReadAllLines(processedLogFileAndDirectory).ToList();

            //Get the Source location
            var sourceDirectory = (GetFileOriginalSourceDirectory(eFilePath));

            var listFilesAtSourceLocation = new List<string>();
            //file names that are unique among source and dest
            var listFilesDifferenceBWSB = new List<string>();
            var listFinalFilesToBeProcessed = new List<string>();

            var isEmpty = !System.IO.Directory.EnumerateFiles(System.IO.Path.GetDirectoryName(sourceDirectory)).Any();
            if (!isEmpty)
            {
                //get list of files at source location
                listFilesAtSourceLocation = GetAllFileNamesFromDirectory(sourceDirectory, filter);

                //get list of files name from the processed file at back up location
                var listProcessedFilesInProcessFileLog = listProcessedFileLines.Select(s => s.Split('~').Last().Trim())
                                                                .Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

                //List of files different in back up file and source location
                listFilesDifferenceBWSB = listFilesAtSourceLocation.Union(listProcessedFilesInProcessFileLog)
                                                           .Except(listFilesAtSourceLocation.Intersect(listProcessedFilesInProcessFileLog))
                                                           .ToList();

                //get the values that has the newer file name in status file
                var newerFileNamesInStatusList = listStatusFileLines.Where(f => f.StartsWith(filter, StringComparison.Ordinal) ||
                                                                        f.Contains("Newer"))
                                                                    .Select(s => s.Replace('\t', ' ')).ToList();

                //validate if the file exist in list that matches the filter
                var validateFilterFileExist = newerFileNamesInStatusList.Any(x => x.IndexOf(filter, StringComparison.Ordinal) > -1);
                if (validateFilterFileExist)
                {
                    //get all the file names that has Newer
                    var fileNameToProcess = newerFileNamesInStatusList.Where(a => a.Contains(filter)
                                                                            && a.Contains("Newer"))
                                                                            .ToList();
                    //Remove all the Newer
                    for (int i = 0; i < fileNameToProcess.Count; i++)
                        fileNameToProcess[i] = fileNameToProcess[i].Replace("Newer", "").Trim();

                    //match the above list with difference and which ever file(s) names match, process those                               
                    var matching = from s in fileNameToProcess
                                   where listFilesDifferenceBWSB.Any(r => s.Contains(r))
                                   select s;

                    listFinalFilesToBeProcessed = fileNameToProcess.Where(x => listFilesDifferenceBWSB.Contains(x)).Distinct().ToList();

                }
                else
                {
                    listFinalFilesToBeProcessed = new List<string>();
                }

            }
            return listFinalFilesToBeProcessed;
        }

        /// <summary>
        /// Method to recreate file if does not exist
        /// </summary>
        /// <param name="fileNameAndDirectoryPath"></param>
        public static void FileCheck(string fileNameAndDirectoryPath)
        {
            var file = new File();
            if (!file.Exists(fileNameAndDirectoryPath))
            {
                file.CreateFile(fileNameAndDirectoryPath);
                Notification.SendNotification("New Processed file for the recording of sucessfully exectuted file is created. " + Environment.NewLine +
                                                    "Please review the new file at file location: " + fileNameAndDirectoryPath,
                                                    System.IO.Path.GetFileName(fileNameAndDirectoryPath));
            }                
        }

        #endregion

        #region Common       

        /// <summary>
        /// Method to check if the directory exist then copy file. If the file already exist then delete that one and move the recent
        /// </summary>
        /// <param name="fileFullNameWithSourcePath"></param>
        /// <param name="eFilePath"></param>
        /// <param name="overwrite"></param>
        public static void CopyFile(string fileFullNameWithSourcePath, EnumFileProcessor.eFilePath eFilePath, bool overwrite)
        {
            var destinationDirectory = string.Empty;
            var destination = GetFileProcessingInDirectory();

            var file = new File();

            switch (eFilePath)
            {
                case EnumFileProcessor.eFilePath.IisLogs:
                    //Move file from its current location to the destination that is defined
                    destinationDirectory = destination + file.GetFileName(fileFullNameWithSourcePath);
                    break;
                case EnumFileProcessor.eFilePath.QVAuditLogs:
                    //Move file from its current location to the destination that is defined
                    destinationDirectory = destination + file.GetFileName(fileFullNameWithSourcePath); ;
                    break;
                case EnumFileProcessor.eFilePath.QVSessionLogs:
                    //Move file from its current location to the destination that is defined
                    destinationDirectory = destination + file.GetFileName(fileFullNameWithSourcePath); ;
                    break;
            }

            // To copy a folder's contents to a new location: Create a new target folder, if necessary.
            if (!file.Exists(destinationDirectory))
            {
                file.Delete(destinationDirectory);
            }
            // To copy a file to another location and overwrite the destination file if it already exists.
            if (file.Exists(fileFullNameWithSourcePath))
            {
                file.Delete(destinationDirectory);
            }
            using (System.IO.FileStream fs = new System.IO.FileStream(fileFullNameWithSourcePath, System.IO.FileMode.OpenOrCreate,
                                                                   System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
                file.Copy(fileFullNameWithSourcePath, destinationDirectory, overwrite);
        }
                
        #endregion                     
    }

}
