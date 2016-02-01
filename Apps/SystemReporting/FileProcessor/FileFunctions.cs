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

namespace FileProcessor
{
    public class FileFunctions
    {
        #region Properties
        private DateTime _dStartDate;
        private DateTime _dEndDate;
        private DateTime _dDateTimeReceived = DateTime.Today;
        private EnumFileProcessor.eFileOutputType _eFileOutputTypes;
        private EnumFileProcessor.eFilePath _eFilePath;
        #endregion

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
        public static string GetFileProcessingInDirectory(EnumFileProcessor.eFilePath eFilePath)
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
        public static string ProcessedFileLogDirectory()
        {
            return (ConfigurationManager.AppSettings["ProcessedFileLogDirectory"]);
        }        

        public static List<string> GetFileToReadFromStatusFile(string filter, EnumFileProcessor.eFilePath eFilePath)
        {
            //status file name with directory
            string statusFileName = ConfigurationManager.AppSettings["statusFileName"];
            string statusFileAndDirectory = GetStatusFileDirectory() + statusFileName + ".txt";

            //backup file name with directory
            string processedLogFileName = ConfigurationManager.AppSettings["ProcessedFileLogFileName"];
            string processedLogFileAndDirectory = ProcessedFileLogDirectory() + processedLogFileName + ".log";

            List<string> fileToRead = new List<string>();
            if (!string.IsNullOrEmpty(statusFileAndDirectory))
            {
                //get file name
                fileToRead = GetFileNameToReadFile(statusFileAndDirectory, processedLogFileAndDirectory, filter, eFilePath);
            }
            return fileToRead;
        }

        public static List<string> GetFileNameToReadFile(string statusFileAndDirectory, string processedLogFileAndDirectory, string filter, EnumFileProcessor.eFilePath eFilePath)
        {
            if (string.IsNullOrEmpty(statusFileAndDirectory))
                return null;

            //Read status File
            List<string> listStatusFileLines = System.IO.File.ReadAllLines(statusFileAndDirectory).ToList();
            //Read Processed Log File lines at the back up location
            List<string> listProcessedFileLines = System.IO.File.ReadAllLines(processedLogFileAndDirectory).ToList();

            //Get the Source location
            var sourceDirectory = GetFileOriginalSourceDirectory(eFilePath);

            var listFilesAtSourceLocation = new List<string>();
            //file names that are unique among source and dest
            var listFilesDifferenceBWSB = new List<string>();
            var listFinalFilesToBeProcessed = new List<string>();

            bool isEmpty = !System.IO.Directory.EnumerateFiles(System.IO.Path.GetDirectoryName(sourceDirectory)).Any();
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
                var newerFileNamesInStatusList = listStatusFileLines.Where(f => f.StartsWith(filter) ||
                                                            f.Contains("Newer")).Select(s => s.Replace('\t', ' '));

                //validate if the file exist in list that matches the filter
                var validateFilterFileExist = newerFileNamesInStatusList.Any(x => x.IndexOf(filter) > -1);
                if (validateFilterFileExist)
                {
                    //#region Status File FileNames 
                    //get list of all the file names that has Newer and matches filter
                    var fileNameToProcessFromStatus = newerFileNamesInStatusList.Where(a => a.Contains(filter)
                                                                && a.Contains("Newer"))
                                                                .Select(split=> split.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                                                                .Last();
                    //get the list of file names only
                    var finalFileToProcess = fileNameToProcessFromStatus.Where(s=>s.Contains(filter)).ToList();

                    //match the above list with difference and which ever file(s) names match, process those                               
                    var matching = from s in finalFileToProcess where listFilesDifferenceBWSB.Any(r => s.StartsWith(r)) select s;

                    listFinalFilesToBeProcessed = finalFileToProcess.Where(x => listFilesDifferenceBWSB.Contains(x)).ToList();

                }
                else
                {
                    listFinalFilesToBeProcessed = new List<string>();
                }

            }
            return listFinalFilesToBeProcessed;
        }

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

        #region Common

        #region Create File
        /// <summary>
        /// Creates a file using the results
        /// </summary>
        /// <param name="sResults"></param>
        /// <param name="eOutPutType"></param>
        /// <param name="eFilePaths"></param>
        public void CreateFile(string sResults, EnumFileProcessor.eFileOutputType eOutPutType,
                                                            EnumFileProcessor.eFilePath eFilePath)
        {
            GetDateRange();

            //get the root directory
            string fileLocation = GetFileOriginalSourceDirectory(eFilePath);
            string filFullNamePath = GetFileName(fileLocation, eOutPutType, eFilePath);

            if (filFullNamePath.Length > 0)
            {
                if (sResults.Length > 0)
                {
                    ProcessFile(sResults, filFullNamePath);
                }
                else
                {
                    ProcessFile(filFullNamePath, eFilePath);
                }
            }

        }

        public string GetFileName(string fileLocation, EnumFileProcessor.eFileOutputType eOutputType,
                                                                    EnumFileProcessor.eFilePath eFilePath)
        {
            _eFileOutputTypes = (EnumFileProcessor.eFileOutputType)Enum.Parse(typeof(EnumFileProcessor.eFileOutputType), eOutputType.ToString());

            string nameFormat = string.Empty;
            switch (_eFileOutputTypes)
            {
                case EnumFileProcessor.eFileOutputType.ExcelFile:
                    nameFormat = GetNameAndDateFormat(fileLocation, eOutputType, eFilePath);
                    break;
                case EnumFileProcessor.eFileOutputType.CsvFile:
                    nameFormat = GetNameAndDateFormat(fileLocation, eOutputType, eFilePath);
                    break;
                case EnumFileProcessor.eFileOutputType.TextFile:
                    nameFormat = GetNameAndDateFormat(fileLocation, eOutputType, eFilePath);
                    break;
                default:
                    throw new ApplicationException("Output type not supported");
            }

            return (string.Format(nameFormat));
        }

        public string GetFileName(string fileLocation, EnumFileProcessor.eFilePath eFilePath)
        {

            string nameFormat = string.Empty;
            switch (eFilePath)
            {
                case EnumFileProcessor.eFilePath.IisLogs:
                    nameFormat = GetNameAndDateFormat(fileLocation, eFilePath);
                    break;
                case EnumFileProcessor.eFilePath.QVAuditLogs:
                    nameFormat = GetNameAndDateFormat(fileLocation, eFilePath);
                    break;
                case EnumFileProcessor.eFilePath.QVSessionLogs:
                    nameFormat = GetNameAndDateFormat(fileLocation, eFilePath);
                    break;
                default:
                    throw new ApplicationException("Output type not supported");
            }

            return (string.Format(nameFormat));
        }

        public string GetNameAndDateFormat(string fileLocation)
        {
            string todaysDate = DateTime.Now.ToString("yyyyMMdd_hhmm");
            //get the file back up location
            string nameFormat = fileLocation + "_S_" + todaysDate;

            return (string.Format(nameFormat, _dDateTimeReceived));
        }

        /// <summary>
        /// returns the name format
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <param name="eOutputType"></param>
        /// <returns></returns>
        public string GetNameAndDateFormat(string fileLocation, EnumFileProcessor.eFileOutputType eOutputType,
                                                                    EnumFileProcessor.eFilePath eFilePath)
        {
            GetDateRange();
            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("yyyyMMdd_hhmm");

            _eFileOutputTypes = (EnumFileProcessor.eFileOutputType)Enum.Parse(typeof(EnumFileProcessor.eFileOutputType),
                                                   eOutputType.ToString());

            _eFilePath = (EnumFileProcessor.eFilePath)Enum.Parse(typeof(EnumFileProcessor.eFilePath), eFilePath.ToString());

            string fileType = string.Empty;
            switch (_eFilePath)
            {
                case EnumFileProcessor.eFilePath.IisLogs:
                    //get the file back up location
                    fileType = "IisL";
                    break;
                case EnumFileProcessor.eFilePath.QVAuditLogs:
                    fileType = "QVA";
                    break;
                case EnumFileProcessor.eFilePath.QVSessionLogs:
                    fileType = "QVS";
                    break;
            }

            string nameFormat = string.Empty;
            switch (_eFileOutputTypes)
            {
                case EnumFileProcessor.eFileOutputType.CsvFile:
                    //get the file back up location
                    nameFormat = fileLocation + "_" + fileType + "_" + todaysDate + ".csv";
                    break;
                case EnumFileProcessor.eFileOutputType.ExcelFile:
                    nameFormat = fileLocation + "_" + fileType + "_" + todaysDate + ".xls";
                    break;
                case EnumFileProcessor.eFileOutputType.TextFile:
                    nameFormat = fileLocation + "_" + fileType + "_" + todaysDate + ".txt";
                    break;
            }

            return (string.Format(nameFormat, _dDateTimeReceived));
        }

        /// <summary>
        /// returns the name format
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <param name="eOutputType"></param>
        /// <returns></returns>
        public string GetNameAndDateFormat(string fileLocation, EnumFileProcessor.eFilePath eFilePath)
        {
            GetDateRange();
            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("yyyyMMdd_hhmm");

            _eFilePath = (EnumFileProcessor.eFilePath)Enum.Parse(typeof(EnumFileProcessor.eFilePath), eFilePath.ToString());

            string nameFormat = string.Empty;
            switch (_eFilePath)
            {
                case EnumFileProcessor.eFilePath.IisLogs:
                    //get the file back up location
                    nameFormat = fileLocation + "_" + "IisL" + "_" + todaysDate;
                    break;
                case EnumFileProcessor.eFilePath.QVAuditLogs:
                    nameFormat = fileLocation + "_" + "QVA" + "_" + todaysDate + "_";
                    break;
                case EnumFileProcessor.eFilePath.QVSessionLogs:
                    nameFormat = fileLocation + "_" + "QVS" + "_" + todaysDate + "_";
                    break;
            }
            return (string.Format(nameFormat, _dDateTimeReceived));
        }

        /// <summary>
        /// Function that completely process a file and it creates if does not exist
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool ProcessFile(string sResults, string filFullNamePath)
        {
            bool bSucess = false;
            string retVal = string.Empty;
            File file = new File();

            if (file.Exists(filFullNamePath))
            {
                file.Delete(filFullNamePath);
            }
            using (System.IO.FileStream fs = new System.IO.FileStream(filFullNamePath, System.IO.FileMode.OpenOrCreate,
                                                                    System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
            using (System.IO.StreamWriter str = new System.IO.StreamWriter(fs))
            {
                str.BaseStream.Seek(0, System.IO.SeekOrigin.End);
                str.Write(sResults);
                str.Flush();
            }

            System.IO.File.AppendAllText(filFullNamePath, sResults);
            string readtext = System.IO.File.ReadAllText(filFullNamePath);

            if (file.Exists(filFullNamePath))
                bSucess = true;

            return bSucess;
        }

        /// <summary>
        /// Function that completely process a file if it exists, reads it, writes and copies it
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool ProcessFile(string filFullNamePath, EnumFileProcessor.eFilePath eFilePath)
        {
            bool bSucess = false;
            string retVal = string.Empty;

            File file = new File();
            if (file.Exists(filFullNamePath))
            {
                //start reading the file. i have used Encoding 1256
                System.IO.StreamReader sr = new System.IO.StreamReader(filFullNamePath);
                retVal = sr.ReadToEnd(); // getting the entire text from the file.
                sr.Close();

                bSucess = WriteFile(retVal, filFullNamePath);
                if (bSucess)
                {
                    CopyFile(filFullNamePath, eFilePath, true);
                }
            }

            return bSucess;
        }

        /// <summary>Description :This routine creates text file based 
        ///on the string of data retrieved from databse 
        ///</summary> 
        public bool WriteFile(string sResults, string filFullNamePath)
        {
            File file = new File();

            //create the file name
            FileTextWriter textWriter = file.CreateFile(filFullNamePath);

            string outputValue = sResults;
            textWriter.WriteLine(outputValue);
            textWriter.Close();

            //if the file is there then
            bool returnValue = false;
            if (outputValue.Length > 0)
                returnValue = true;

            return returnValue;
        }

        /// <summary>
        /// Method to check if the directory exist then copy file. If the file already exist then delete that one and move the recent
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        public static void CopyFile(string fileFullNameWithSourcePath, EnumFileProcessor.eFilePath eFilePath, bool overwrite)
        {
            string destinationDirectory = string.Empty;
            var destination = GetFileProcessingInDirectory(eFilePath);

            File file = new File();

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

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!file.Exists(destinationDirectory))
            {
                file.Delete(destinationDirectory);
            }
            // To copy a file to another location and 
            // overwrite the destination file if it already exists.
            if (file.Exists(fileFullNameWithSourcePath))
            {
                file.Delete(destinationDirectory);
            }
            using (System.IO.FileStream fs = new System.IO.FileStream(fileFullNameWithSourcePath, System.IO.FileMode.OpenOrCreate,
                                                                   System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
                file.Copy(fileFullNameWithSourcePath, destinationDirectory, overwrite);
        }

        #endregion

        public void GetDateRange()
        {
            var dateValue = ConfigurationManager.AppSettings["OverrideDate"];
            _dDateTimeReceived = (!string.IsNullOrEmpty(dateValue)) ? (DateTime.Parse(dateValue.ToString())) : DateTime.Today;
            //get Yesterday date
            var yesterday = DateTime.Today.AddDays(-1);
            _dStartDate = yesterday;//string.Format(DateTime.Parse(_dDateTimeReceived.AddDays(-1)), "MM/dd/yy");
            _dEndDate = _dStartDate.AddDays(1);
        }

        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        
        /// <summary>
        /// return all files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string[] GetFileNames(string path, string filter)
        {
            string[] files = System.IO.Directory.GetFiles(path, filter);
            for (int i = 0; i < files.Length; i++)
                files[i] = System.IO.Path.GetFileName(files[i]);
            return files;
        }

        /// <summary>
        /// Returns latest file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public string GetLatestFileName(string path, string filter)
        {
            bool isEmpty = !System.IO.Directory.EnumerateFiles(path).Any();
            var myFile = (String)null;
            var file = (String)null;
            if (!isEmpty)
            {
                var directory = new System.IO.DirectoryInfo(path);
                myFile = directory.GetFiles()
                                .OrderByDescending(f => f.LastWriteTime)
                                .OrderBy(f => f.Length)
                                .First().ToString();
                file = myFile.ToString();
            }
            return file;
        }

        /// <summary>
        /// Get latest file if more than one exist
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="filePth"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static System.IO.FileInfo GetFileFromDirectory(string filePath, string filter)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            string fileName = System.IO.Path.GetFileName(filePath);
            string dir = System.IO.Path.GetDirectoryName(filePath);
            var latestFile = new System.IO.DirectoryInfo(dir)
                            .EnumerateFiles(filter, System.IO.SearchOption.TopDirectoryOnly)
                            //.Where(file => file.Name.Contains(filter))
                            .OrderByDescending(file => file.LastWriteTime)
                            .First();
            long len = latestFile.Length;

            return latestFile;
        }
        #endregion
    }

}
