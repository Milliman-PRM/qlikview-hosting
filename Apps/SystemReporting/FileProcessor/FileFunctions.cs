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
        public static string GetExceptionLoggerFileDirectory()
        {
            return ConfigurationManager.AppSettings["ExceptionFileDirectory"];
        }
        public static string GetExceptionLoggerFileName()
        {
            return ConfigurationManager.AppSettings["ExceptionFileName"];
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
            var files = new List<string>();
            var isEmpty = !System.IO.Directory.EnumerateFiles(dir).Any();
            if (!isEmpty)
            {
                var directory = new System.IO.DirectoryInfo(dir);
                files = directory.GetFiles("*" + filter + "*")
                                 .Where(file => file.Name.StartsWith(filter, StringComparison.Ordinal))
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
            var processedLogFileAndDirectory = (GetProcessedFileLogDirectory() + GetProcessedFileLogFileName());

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
            var listProcessedFileLines = System.IO.File.ReadAllLines(processedLogFileAndDirectory + ".log").ToList();

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
                                                                                        .Where(s => !string.IsNullOrWhiteSpace(s)
                                                                                        && !s.Contains("New File Created"))
                                                                                        .Distinct().ToList();

                //List of files different in back up file and source location
                listFilesDifferenceBWSB = listFilesAtSourceLocation.Union(listProcessedFilesInProcessFileLog)
                                                                    .Except(listFilesAtSourceLocation.Intersect(listProcessedFilesInProcessFileLog))
                                                                    .Distinct().ToList();


                //get the values that has the new file name in status file and split the list
                var newFileNamesInStatusList = listStatusFileLines.Where(f => f.Any(r => f.Contains(filter)) &&
                                                                        f.Contains("New File") ||
                                                                        f.Contains("Newer"))
                                                                    .Select(s => s.Replace('\t', ' ')).ToList();

                //Remove all the New File phrase
                for (int i = 0; i < newFileNamesInStatusList.Count; i++)
                    newFileNamesInStatusList[i] = newFileNamesInStatusList[i].Replace("Newer", "").Trim();

                //Remove all the New File phrase
                for (int i = 0; i < newFileNamesInStatusList.Count; i++)
                    newFileNamesInStatusList[i] = newFileNamesInStatusList[i].Replace("New File", "").Trim();

                //get the values that has the new file name in status file and split the list
                var filesNotToProcess = newFileNamesInStatusList.Where(f => f.StartsWith(filter, StringComparison.Ordinal)).ToList();

                //validate if the file exist in list that matches the filter
                var validateFilterFileExist = filesNotToProcess.Any(x => x.IndexOf(filter, StringComparison.Ordinal) > -1);
                if (validateFilterFileExist)
                {
                    //if a file exist in processed and it is listed again in status as new or newer then send email
                    var fileExistForReProcessing = from s in listProcessedFilesInProcessFileLog
                                                   where filesNotToProcess.Any(r => s.Contains(r))
                                                   select s;

                    if (fileExistForReProcessing.ToList().Count>0)
                    {
                        var displyList = string.Join(",", fileExistForReProcessing.ToArray());
                        Notification.SendNotification("File(s) " + displyList + " has been modified that has already been processed - updated content of file will not be added to database. " 
                                                          , "Previously processed file listed in Status File");
                    }

                    //files not to process   
                    //now find the files in difference that matches the above list and remove those
                    var matchingFileToBeRemoved = from s in listFilesDifferenceBWSB
                                                  where filesNotToProcess.Any(r => s.Contains(r))
                                                  select s;

                    listFinalFilesToBeProcessed = listFilesDifferenceBWSB.Except(matchingFileToBeRemoved)
                                                        .Where(f => f.Contains(filter)).ToList();

                }
                else
                {
                    listFinalFilesToBeProcessed = new List<string>();
                }
            }
            return listFinalFilesToBeProcessed;
        }

        /// <summary>
        /// Method to re-create file if does not exist
        /// </summary>
        /// <param name="fileNameAndDirectoryPath"></param>
        public static void FileCheck(string fileNameAndDirectoryPath)
        {
            //if the file name does not have .log as extention then add
            if (!fileNameAndDirectoryPath.Contains(".log"))
            {
                var file = fileNameAndDirectoryPath + ".log";
                fileNameAndDirectoryPath = "";
                fileNameAndDirectoryPath = file;
            }

            if (!File.Exists(fileNameAndDirectoryPath))
            {
                //create the file name
                var message = (DateTime.Now + "||" + "=================== Log Created Started ===================");
                ////create the file name
                var textWriter = File.CreateFile(fileNameAndDirectoryPath);
                textWriter.WriteLine(message);
                textWriter.Close();
                Notification.SendNotification("New file for the recording of sucessfully exectuted log file is created. " + Environment.NewLine +
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

            switch (eFilePath)
            {
                case EnumFileProcessor.eFilePath.IisLogs:
                    //Move file from its current location to the destination that is defined
                    destinationDirectory = destination + File.GetFileName(fileFullNameWithSourcePath);
                    break;
                case EnumFileProcessor.eFilePath.QVAuditLogs:
                    //Move file from its current location to the destination that is defined
                    destinationDirectory = destination + File.GetFileName(fileFullNameWithSourcePath); ;
                    break;
                case EnumFileProcessor.eFilePath.QVSessionLogs:
                    //Move file from its current location to the destination that is defined
                    destinationDirectory = destination + File.GetFileName(fileFullNameWithSourcePath); ;
                    break;
            }

            // To copy a folder's contents to a new location: Create a new target folder, if necessary.
            if (!File.Exists(destinationDirectory))
            {
                File.Delete(destinationDirectory);
            }
            // To copy a file to another location and overwrite the destination file if it already exists.
            if (File.Exists(fileFullNameWithSourcePath))
            {
                File.Delete(destinationDirectory);
            }
            using (System.IO.FileStream fs = new System.IO.FileStream(fileFullNameWithSourcePath, System.IO.FileMode.OpenOrCreate,
                                                                   System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
                File.Copy(fileFullNameWithSourcePath, destinationDirectory, overwrite);
        }

        /// <summary>
        /// This method will delete files if there are any
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<string> GetAllFilesInProcessingDirectroyThatAreNotDeleted()
        {
            //LogFileProcessor\IN
            var destinationInDirectory = new System.IO.DirectoryInfo(FileFunctions.GetFileProcessingInDirectory());

            var files = new List<string>();
            var isEmpty = !System.IO.Directory.EnumerateFiles(destinationInDirectory.ToString()).Any();
            if (!isEmpty)
            {
                var fileList = System.IO.Directory.GetFiles(destinationInDirectory.ToString());
                foreach (var file in fileList)
                {
                    if (file.ToLower().IndexOf("u_ex", StringComparison.Ordinal) > -1)
                    {
                        BaseFileProcessor.LogError(null,DateTime.Now + "||" + "=== File is deleted from processing directory. ===" + file);
                        System.IO.File.Delete(file);
                    }
                    if (file.ToLower().IndexOf("audit_", StringComparison.Ordinal) > -1)
                    {
                        BaseFileProcessor.LogError(null,DateTime.Now + "||" + "=== File is deleted from processing directory. ===" + file);
                        System.IO.File.Delete(file);
                    }
                    if (file.ToLower().IndexOf("sessions_", StringComparison.Ordinal) > -1)
                    {
                        BaseFileProcessor.LogError(null,DateTime.Now + "||" + "=== File is deleted from processing directory. ===" + file);
                        System.IO.File.Delete(file);
                    }
                }                

            }

            return files;
        }
        #endregion
    }

}
