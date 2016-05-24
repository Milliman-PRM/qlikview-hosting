using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SystemReporting.Service;
using SystemReporting.Entities.Models;
using SystemReporting.Controller;
using SystemReporting.Data.Repository;
using SystemReporting.Controller.BusinessLogic.Controller;
using System.Collections.Generic;
using SystemReporting.Utilities.File;
using FileProcessor;
using System.Web;
using System.Configuration;
using SystemReporting.Utilities;
using System.Linq;
using System.IO;

namespace ReportingTest
{
    [TestClass]
    public class FileFunctionsTester : BaseFileProcessor
    {

        #region File Basic Functions Test
        /// <summary>
        /// This routine creates text file based on the string of data retrieved 
        /// <param name="sResults"></param>
        /// <param name="filFullNamePath"></param>
        /// <returns></returns>
        [TestMethod]
        public void TestWriteFile()
        {
            string sResults, filFullNamePath = string.Empty;

            sResults = "This is test";
            filFullNamePath = @"C:\ProductionLogsTest\LogFileProcessor\Logger";           


            //create the file name
            var textWriter = SystemReporting.Utilities.File.File.CreateFile(filFullNamePath);

            var outputValue = sResults;
            textWriter.WriteLine(outputValue);
            textWriter.Close();

            //if the file is there then
            var returnValue = false;
            if (outputValue.Length > 0)
                returnValue = true;

            Assert.IsTrue(returnValue);
        }
       
        /// <summary>
        /// Function that completely process a file and it creates if does not exist
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TestProcessFile()
        {
            string sResults, filFullNamePath = string.Empty;

            sResults = "This is test";
            filFullNamePath = @"C:\ProductionLogs\LogFileProcessor\TestLogger";

            var bSucess = false;
            var retVal = string.Empty;

            if (SystemReporting.Utilities.File.File.Exists(filFullNamePath))
            {
                SystemReporting.Utilities.File.File.Delete(filFullNamePath);
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
            var readtext = System.IO.File.ReadAllText(filFullNamePath);

            if (SystemReporting.Utilities.File.File.Exists(filFullNamePath))
                bSucess = true;

            Assert.IsTrue(bSucess);
        }

        /// <summary>
        /// Function that completely process a file if it exists, reads it, writes and copies it
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TestProcessFile2()
        {
            //Pass var: string filFullNamePath, EnumFileProcessor.eFilePath eFilePath
            var filFullNamePath = string.Empty;
            var eFilePath = EnumFileProcessor.eFilePath.IisLogs;

            filFullNamePath = @"C:\ProductionLogs\LogFileProcessor\TestLogger";

            var bSucess = false;
            var retVal = string.Empty;

            if (SystemReporting.Utilities.File.File.Exists(filFullNamePath))
            {
                //start reading the file. i have used Encoding 1256
                var sr = new System.IO.StreamReader(filFullNamePath);
                retVal = sr.ReadToEnd(); // getting the entire text from the file.
                sr.Close();

                bSucess = SystemReporting.Utilities.File.File.WriteFile(retVal, filFullNamePath);
                if (bSucess)
                {
                    SystemReporting.Utilities.File.File.Copy(filFullNamePath, eFilePath.ToString(), true);
                }
            }

            Assert.IsTrue(bSucess);
        }

        [TestMethod]
        public void TestCheckDirectory()
        {
            var directoryInfo = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("/ProductionLogsTest"));
            var message = string.Empty;
            if (directoryInfo != null)
            {
                message = "alert('Directory already exists.');";
            }
            else
            {
                message = "alert('Directory does not exists.');";
            }
            Assert.IsNotNull(message);
        }

        [TestMethod]
        public void TestCheckDirectory2()
        {
            var directoryPath = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "ProductionLogsTest"));
            var message = string.Empty;
            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }
            else
            {
                message = "alert('Directory already exists.');";
            }
            Assert.IsNotNull(message);
        }



        [TestMethod]
        public void TestGetFileSourceLocation()
        {
            var filePaths = EnumFileProcessor.eFilePath.IisLogs;
            var fileLocation = FileFunctions.GetFileOriginalSourceDirectory(filePaths);
            Assert.IsNotNull(fileLocation);
        }

        [TestMethod]
        public void TestGetFileCopyToDestinationInLocation()
        {
            var filePaths = EnumFileProcessor.eFilePath.IisLogs;
            var fileLocation = FileFunctions.GetFileProcessingInDirectory();
            Assert.IsNotNull(fileLocation);
        }
        #endregion

        #region File Process Test

        [TestMethod]
        public void TestFileCreation()
        {
            //This test creates a file with a name fileName 
            //sucess
            //string testFilePath = "C:\\Users\\afsheen.khan\\Desktop\\Informaiton\\test_writer.txt";

            //sucess
            string fileLocation = @"\\indy-ss01\ProductionLogs\IISLogs\";
            // var fileLocation = FileFunctions.GetFileProcessingInDirectory();
             var fileName = "test_writer.txt";
            var fileFullNameAndPath = fileLocation + fileName;
            var textWriter = SystemReporting.Utilities.File.File.CreateFile(fileFullNameAndPath);

            var sourceDirectory = new DirectoryInfo(fileLocation);
            var fi = new System.IO.FileInfo(fileLocation);
            if (!ReferenceEquals(sourceDirectory, null))
            {
                Assert.IsTrue(true);
            }

            var bSucess = false;
            if (SystemReporting.Utilities.File.File.Exists(fileFullNameAndPath))
            {
                bSucess = true;
            }

            Assert.IsTrue(bSucess);
        }

        [TestMethod]
        public void TestCopyFile()
        {
            //the file has to be avalible
            var fp = new FileFunctions();

            var filePath = EnumFileProcessor.eFilePath.IisLogs;
            var fileFullNameAndSourcePath = @"\\indy-ss01\ProductionLogsTest\IISLogs\test_writer.txt";

            //copy file
            FileFunctions.CopyFile(fileFullNameAndSourcePath, filePath,true);
            //check if file exist

            var bSucess = false;
            if (SystemReporting.Utilities.File.File.Exists(fileFullNameAndSourcePath))
            {
                bSucess = true;
            }
            Assert.IsTrue(bSucess);
        }

        #endregion       
        
       [TestMethod]
        public void TestLogger()
        {            
            //BaseFileProcessor.LogError(ex,"This is Test  || DateTime: " + DateTime.Now,"" );
            var LoggerFileDirectory = ConfigurationManager.AppSettings["LoggerFileDirectory"];
            var LoggerFileName = ConfigurationManager.AppSettings["LoggerFileName"];

            Logger.Instance.LogPath = LoggerFileDirectory;
            Logger.Instance.LogFileName = LoggerFileName;

            var fileSourceFullNameAndPath = Logger.Instance.LogPath + Logger.Instance.LogFileName;
            var bFileExist = false;
            if (SystemReporting.Utilities.File.File.Exists(fileSourceFullNameAndPath))
            {
                bFileExist = true;
            }
            Assert.IsTrue(bFileExist);
        }

        #region Logs
        [TestMethod]
        public void TestGetFileNameiisLogsFromStatus()
        {
            //status file name with directory
            var statusFileName = ConfigurationManager.AppSettings["statusFileName"];
            var statusFileAndDirectory = FileFunctions.GetStatusFileDirectory() + statusFileName + ".txt";

            //backup file name with directory
            var processedLogFileName = ConfigurationManager.AppSettings["ProcessedFileLogFileName"];
            var processedLogFileAndDirectory = FileFunctions.GetProcessedFileLogDirectory() + processedLogFileName + ".log";

            var filter = "u_ex";

            var fileToRead = new List<string>();
            var isEmpty = !System.IO.Directory.EnumerateFiles(statusFileAndDirectory).Any();
            if (!isEmpty)
            {
                fileToRead = FileFunctions.GetFileNameToReadFile(statusFileAndDirectory, processedLogFileAndDirectory, filter, EnumFileProcessor.eFilePath.IisLogs);
            }
            Assert.IsNotNull(fileToRead);
        }
        [TestMethod]
        public void TestGetFileNameAuditLogsFromStatus()
        {

            //status file name with directory
            var statusFileName = ConfigurationManager.AppSettings["statusFileName"];
            var statusFileAndDirectory = FileFunctions.GetStatusFileDirectory() + statusFileName + ".txt";

            //backup file name with directory
            var processedLogFileName = ConfigurationManager.AppSettings["ProcessedFileLogFileName"];
            var processedLogFileAndDirectory = FileFunctions.GetProcessedFileLogDirectory() + processedLogFileName + ".log";

            var filter = "Audit_INDY-PRM";

            var fileToRead = new List<string>();
            var isEmpty = !System.IO.Directory.EnumerateFiles(statusFileAndDirectory).Any();
            if (!isEmpty)
            {
                fileToRead = FileFunctions.GetFileNameToReadFile(statusFileAndDirectory, processedLogFileAndDirectory, 
                                                                                            filter, EnumFileProcessor.eFilePath.QVAuditLogs);
            }
            Assert.IsNotNull(fileToRead);
        }
        [TestMethod]
        public void TestGetFileNameSessionLogsFromStatus()
        {
            //status file name with directory
            var statusFileName = ConfigurationManager.AppSettings["statusFileName"];
            var statusFileAndDirectory = FileFunctions.GetStatusFileDirectory() + statusFileName + ".txt";

            //backup file name with directory
            var processedLogFileName = ConfigurationManager.AppSettings["ProcessedFileLogFileName"];
            var processedLogFileAndDirectory = FileFunctions.GetProcessedFileLogDirectory() + processedLogFileName + ".log";

            var filter = "Sessions_INDY-PRM";
            var fileToRead = new List<string>();
            var isEmpty = !System.IO.Directory.EnumerateFiles(statusFileAndDirectory).Any();
            if (!isEmpty)
            {
                fileToRead = FileFunctions.GetFileNameToReadFile(statusFileAndDirectory, processedLogFileAndDirectory,
                                                                            filter, EnumFileProcessor.eFilePath.QVSessionLogs);
            }
            Assert.IsNotNull(fileToRead);
        }
        #endregion
        
        //public string GetFileName(string fileLocation, EnumFileProcessor.eFilePath eFilePath)
        //{

        //    string nameFormat = string.Empty;
        //    switch (eFilePath)
        //    {
        //        case EnumFileProcessor.eFilePath.IisLogs:
        //            nameFormat = GetNameAndDateFormat(fileLocation, eFilePath);
        //            break;
        //        case EnumFileProcessor.eFilePath.QVAuditLogs:
        //            nameFormat = GetNameAndDateFormat(fileLocation, eFilePath);
        //            break;
        //        case EnumFileProcessor.eFilePath.QVSessionLogs:
        //            nameFormat = GetNameAndDateFormat(fileLocation, eFilePath);
        //            break;
        //        default:
        //            throw new ApplicationException("Output type not supported");
        //    }

        //    return (string.Format(nameFormat));
        //}

        //public string GetNameAndDateFormat(string fileLocation)
        //{
        //    string todaysDate = DateTime.Now.ToString("yyyyMMdd_hhmm");
        //    //get the file back up location
        //    string nameFormat = fileLocation + "_S_" + todaysDate;

        //    return (string.Format(nameFormat, DateTime.Now));
        //}
        //#region Create File
        ///// <summary>
        ///// Creates a file using the results
        ///// </summary>
        ///// <param name="sResults"></param>
        ///// <param name="eOutPutType"></param>
        ///// <param name="eFilePaths"></param>
        //public void CreateFile(string sResults, EnumFileProcessor.eFileOutputType eOutPutType,
        //                                                    EnumFileProcessor.eFilePath eFilePath)
        //{
        //    //GetDateRange();

        //    //get the root directory
        //    string fileLocation = GetFileOriginalSourceDirectory(eFilePath);
        //    string filFullNamePath = GetFileName(fileLocation, eOutPutType, eFilePath);

        //    if (filFullNamePath.Length > 0)
        //    {
        //        if (sResults.Length > 0)
        //        {
        //            ProcessFile(sResults, filFullNamePath);
        //        }
        //        else
        //        {
        //            ProcessFile(filFullNamePath, eFilePath);
        //        }
        //    }

        //}

        //public string GetFileName(string fileLocation, EnumFileProcessor.eFileOutputType eOutputType,
        //                                                            EnumFileProcessor.eFilePath eFilePath)
        //{
        //    _eFileOutputTypes = (EnumFileProcessor.eFileOutputType)Enum.Parse(typeof(EnumFileProcessor.eFileOutputType), eOutputType.ToString());

        //    string nameFormat = string.Empty;
        //    switch (_eFileOutputTypes)
        //    {
        //        case EnumFileProcessor.eFileOutputType.ExcelFile:
        //            nameFormat = GetNameAndDateFormat(fileLocation, eOutputType, eFilePath);
        //            break;
        //        case EnumFileProcessor.eFileOutputType.CsvFile:
        //            nameFormat = GetNameAndDateFormat(fileLocation, eOutputType, eFilePath);
        //            break;
        //        case EnumFileProcessor.eFileOutputType.TextFile:
        //            nameFormat = GetNameAndDateFormat(fileLocation, eOutputType, eFilePath);
        //            break;
        //        default:
        //            throw new ApplicationException("Output type not supported");
        //    }

        //    return (string.Format(nameFormat));
        //}

        ///// <summary>
        ///// returns the name format
        ///// </summary>
        ///// <param name="fileLocation"></param>
        ///// <param name="eOutputType"></param>
        ///// <returns></returns>
        //public string GetNameAndDateFormat(string fileLocation, EnumFileProcessor.eFileOutputType eOutputType,
        //                                                            EnumFileProcessor.eFilePath eFilePath)
        //{
        //    //GetDateRange();
        //    DateTime date = DateTime.Now;
        //    string todaysDate = date.ToString("yyyyMMdd_hhmm");

        //    _eFileOutputTypes = (EnumFileProcessor.eFileOutputType)Enum.Parse(typeof(EnumFileProcessor.eFileOutputType),
        //                                           eOutputType.ToString());

        //    _eFilePath = (EnumFileProcessor.eFilePath)Enum.Parse(typeof(EnumFileProcessor.eFilePath), eFilePath.ToString());

        //    string fileType = string.Empty;
        //    switch (_eFilePath)
        //    {
        //        case EnumFileProcessor.eFilePath.IisLogs:
        //            //get the file back up location
        //            fileType = "IisL";
        //            break;
        //        case EnumFileProcessor.eFilePath.QVAuditLogs:
        //            fileType = "QVA";
        //            break;
        //        case EnumFileProcessor.eFilePath.QVSessionLogs:
        //            fileType = "QVS";
        //            break;
        //    }

        //    string nameFormat = string.Empty;
        //    switch (_eFileOutputTypes)
        //    {
        //        case EnumFileProcessor.eFileOutputType.CsvFile:
        //            //get the file back up location
        //            nameFormat = fileLocation + "_" + fileType + "_" + todaysDate + ".csv";
        //            break;
        //        case EnumFileProcessor.eFileOutputType.ExcelFile:
        //            nameFormat = fileLocation + "_" + fileType + "_" + todaysDate + ".xls";
        //            break;
        //        case EnumFileProcessor.eFileOutputType.TextFile:
        //            nameFormat = fileLocation + "_" + fileType + "_" + todaysDate + ".txt";
        //            break;
        //    }

        //    return (string.Format(nameFormat, DateTime.Now));
        //}

    }
}
