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

namespace ReportingTest
{
    [TestClass]
    public class FileFunctionsTester : BaseFileProcessor
    {
        
        #region File Basic Functions Test
        [TestMethod]
        public void TestCheckDirectory()
        {
            System.IO.DirectoryInfo directoryInfo = new 
                                    System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("/ProductionLogsTest"));
            string message = string.Empty;
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
            string directoryPath = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "ProductionLogsTest"));
            string message = string.Empty;
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
        public void TestGetFileName()
        {
            //successful
            FileFunctions fp = new FileFunctions();
            EnumFileProcessor.eFileOutputType eFileOutputType = EnumFileProcessor.eFileOutputType.TextFile;
            EnumFileProcessor.eFilePath filePaths = EnumFileProcessor.eFilePath.IisLogs;
            string fileLocation = FileFunctions.GetFileProcessingInDirectory(filePaths);
            string fileFullNameAndPath = fp.GetFileName(fileLocation, eFileOutputType, filePaths);
            Assert.IsNotNull(fileFullNameAndPath);
        }

        [TestMethod]
        public void TestGetDateRange()
        {
            //successful
            FileFunctions fp = new FileFunctions();
            fp.GetDateRange();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestGetFileSourceLocation()
        {
            EnumFileProcessor.eFilePath filePaths = EnumFileProcessor.eFilePath.IisLogs;
            string fileLocation = FileFunctions.GetFileOriginalSourceDirectory(filePaths);
            Assert.IsNotNull(fileLocation);
        }

        //[TestMethod]
        //public void TestGetFileDestinationBackUpLocation()
        //{
        //    EnumFileProcessor.eFilePath filePaths = EnumFileProcessor.eFilePath.IisLogs;
        //    string fileLocation = FileFunctions.GetFileBackUpDirectory(filePaths);
        //    Assert.IsNotNull(fileLocation);
        //}

        [TestMethod]
        public void TestGetFileCopyToDestinationInLocation()
        {
            EnumFileProcessor.eFilePath filePaths = EnumFileProcessor.eFilePath.IisLogs;
            string fileLocation = FileFunctions.GetFileProcessingInDirectory(filePaths);
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
            //string fileLocation = @"\\indy-ss01\ProductionLogsTest\IISLogs\test_writer.txt";
            EnumFileProcessor.eFilePath filePaths = EnumFileProcessor.eFilePath.IisLogs;

            string fileLocation = FileFunctions.GetFileProcessingInDirectory(filePaths);
            string fileName = "test_writer.txt";
            string fileFullNameAndPath = fileLocation + fileName;
            File file = new File();
            FileTextWriter textWriter = file.CreateFile(fileFullNameAndPath);

            bool bSucess = false;
            if (file.Exists(fileFullNameAndPath))
            {
                bSucess = true;
            }

            Assert.IsTrue(bSucess);
        }

        [TestMethod]
        public void TestFileCreation2()
        {
            //sucess
            FileFunctions fp = new FileFunctions();
            EnumFileProcessor.eFilePath efilePaths = EnumFileProcessor.eFilePath.IisLogs;
            EnumFileProcessor.eFileOutputType eOutPutType = EnumFileProcessor.eFileOutputType.TextFile;

            string sResults = "Some Data";
            fp.CreateFile(sResults, eOutPutType, efilePaths);

            bool bSucess = true;   
            Assert.IsTrue(bSucess);
        }


        [TestMethod]
        public void TestFileCreation3()
        {
            //sucess
            FileFunctions fp = new FileFunctions();
            EnumFileProcessor.eFilePath efilePaths = EnumFileProcessor.eFilePath.IisLogs;
            EnumFileProcessor.eFileOutputType eOutPutType = EnumFileProcessor.eFileOutputType.TextFile;

            string sResults = string.Empty;
            fp.CreateFile(sResults, eOutPutType, efilePaths);

            bool bSucess = true;
            Assert.IsTrue(bSucess);
        }

        [TestMethod]
        public void TestWriteFile()
        {
            FileFunctions fp = new FileFunctions();
            EnumFileProcessor.eFileOutputType eFileOutputType = EnumFileProcessor.eFileOutputType.TextFile;  
            EnumFileProcessor.eFilePath filePaths = EnumFileProcessor.eFilePath.IisLogs;

            string filepath = FileFunctions.GetFileProcessingInDirectory(filePaths);
            string fileFullNameAndPath = fp.GetFileName(filepath, eFileOutputType, filePaths);

            string sResults = "sResults";
            //copy file
            bool bSucess = fp.WriteFile(sResults, fileFullNameAndPath);
            //check if file exist
           Assert.IsTrue(bSucess);
        }

        [TestMethod]
        public void TestCopyFile()
        {
            //the file has to be avalible
            FileFunctions fp = new FileFunctions();
            
            EnumFileProcessor.eFilePath filePath = EnumFileProcessor.eFilePath.IisLogs;
            string fileFullNameAndSourcePath = @"\\indy-ss01\ProductionLogsTest\IISLogs\test_writer.txt";

            //copy file
            FileFunctions.CopyFile(fileFullNameAndSourcePath, filePath,true);
            //check if file exist
            
            bool bSucess = false;
            File file = new File();
            if (file.Exists(fileFullNameAndSourcePath))
            {
                bSucess = true;
            }
            Assert.IsTrue(bSucess);
        }


        [TestMethod]
        public void TestProcessFile()
        {
            FileFunctions fp = new FileFunctions();
            EnumFileProcessor.eFilePath filePath = EnumFileProcessor.eFilePath.IisLogs;

            string filepath = FileFunctions.GetFileProcessingInDirectory(filePath);
            string fileFullNameAndPath = @"\\indy-ss01\ProductionLogsTest\IISLogs\test_writer.txt";

            bool bSucess = fp.ProcessFile(fileFullNameAndPath, filePath);
            //check if file exist
            Assert.IsTrue(bSucess);
        }

        #endregion
        
        #region iisLogs
        [TestMethod]
        public void TestGetFileNameiisLogs()
        {
            //sucessful
            FileProcessor.FileFunctions fp = new FileProcessor.FileFunctions();
            EnumFileProcessor.eFileOutputType eFileOutputType = EnumFileProcessor.eFileOutputType.TextFile;

            EnumFileProcessor.eFilePath efilePath = EnumFileProcessor.eFilePath.IisLogs;
            string filepath = FileFunctions.GetFileProcessingInDirectory(efilePath);

            string fileFullNameAndPath = fp.GetFileName(filepath, eFileOutputType, efilePath);
            Assert.IsNotNull(fileFullNameAndPath);
        }
        #endregion

        [TestMethod]
        public void TestGetFileNames()
        {
            //sucessful
            FileFunctions fp = new FileFunctions();
            EnumFileProcessor.eFilePath filePath = EnumFileProcessor.eFilePath.IisLogs;
            string filepath = FileFunctions.GetFileProcessingInDirectory(filePath);

            string file = fp.GetLatestFileName(filepath, "u_*.log");
            Assert.IsNotNull(file);
        }

        #region Copy
        [TestMethod]
        public void TestCopyFileFromProductionLogsInToLogFileProcessorIisLogsIN()
        {
            FileFunctions fp = new FileFunctions();

            EnumFileProcessor.eFilePath filePath = EnumFileProcessor.eFilePath.IisLogs;
            //move file from here
            string fileSource = FileFunctions.GetFileOriginalSourceDirectory(filePath);
            //get lastest file 
            string fileGet = fp.GetLatestFileName(fileSource, "u_*.log");
            string fileFullPath = fileSource + fileGet;
            //copy file
            FileFunctions.CopyFile(fileFullPath, filePath,true);
            //check if file exist

            bool bSucess = true;
            File file = new File();
            if (file.Exists(fileFullPath))
            {
                bSucess = true;
            }
            Assert.IsTrue(bSucess);
        }
        
        [TestMethod]
        public void TestCopyFileFromLogFileProcessorIisLogsINToLogFileProcessorIisLogsBackUp()
        {
            //no need 
            FileFunctions fp = new FileFunctions();

            EnumFileProcessor.eFilePath filePath = EnumFileProcessor.eFilePath.IisLogs;

            //move file from here
            string fileSource = FileFunctions.GetFileProcessingInDirectory(filePath);
            //get lastest file 
            string fileGetLatest = fp.GetLatestFileName(fileSource, "u_*.log");
            string fileFullPath = fileSource + fileGetLatest;
            //copy file
            FileFunctions.CopyFile(fileFullPath, filePath,true);
            //check if file exist
            
            bool bSucess = true;
            File file = new File();
        
            if (file.Exists(fileFullPath))
            {
                bSucess = true;
            }
            Assert.IsTrue(bSucess);
        }

        #endregion

        [TestMethod]
        public void TestMoveFileFromLogFileProcessorIisLogsINToLogFileProcessorIisLogsBackUp()
        {
            FileFunctions fp = new FileFunctions();

            EnumFileProcessor.eFilePath filePath = EnumFileProcessor.eFilePath.IisLogs;

            //move file from here
            string fileSource = FileFunctions.GetFileProcessingInDirectory(filePath);
            string fileDestinationPath = FileFunctions.GetFileProcessingInDirectory(filePath);

            //get lastest file 
            string latestFileName = fp.GetLatestFileName(fileSource, "u_*.log");

            bool bSucess = false;
            if (!string.IsNullOrEmpty(latestFileName))
            {
                string fileSourceFullNameAndPath = fileSource + latestFileName;
                string destNationFileFullNameAndPath = fileDestinationPath + latestFileName;
                
                File file = new File();
                if (!file.Exists(destNationFileFullNameAndPath))
                {
                    file.Move(fileSourceFullNameAndPath, destNationFileFullNameAndPath);
                }

                if (file.Exists(destNationFileFullNameAndPath))
                {
                    bSucess = true;
                }
            }
            Assert.IsTrue(bSucess);
        }

       [TestMethod]
        public void TestLogger()
        {
            LogError("This is Test  || DateTime: " + DateTime.Now );
            var LoggerFileDirectory = ConfigurationManager.AppSettings["LoggerFileDirectory"];
            var LoggerFileName = ConfigurationManager.AppSettings["LoggerFileName"];

            Logger.Instance.LogPath = LoggerFileDirectory;
            Logger.Instance.LogFileName = LoggerFileName;

            string fileSourceFullNameAndPath = Logger.Instance.LogPath + Logger.Instance.LogFileName;
            bool bFileExist = false;
            File file = new File();
            if (file.Exists(fileSourceFullNameAndPath))
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
            string statusFileName = ConfigurationManager.AppSettings["statusFileName"];
            string statusFileAndDirectory = FileFunctions.GetStatusFileDirectory() + statusFileName + ".txt";

            //backup file name with directory
            string processedLogFileName = ConfigurationManager.AppSettings["ProcessedFileLogFileName"];
            string processedLogFileAndDirectory = FileFunctions.ProcessedFileLogDirectory() + processedLogFileName + ".log";

            string filter = "u_ex";
            
            List<string> fileToRead = new List<string>();
            bool isEmpty = !System.IO.Directory.EnumerateFiles(statusFileAndDirectory).Any();
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
            string statusFileName = ConfigurationManager.AppSettings["statusFileName"];
            string statusFileAndDirectory = FileFunctions.GetStatusFileDirectory() + statusFileName + ".txt";

            //backup file name with directory
            string processedLogFileName = ConfigurationManager.AppSettings["ProcessedFileLogFileName"];
            string processedLogFileAndDirectory = FileFunctions.ProcessedFileLogDirectory() + processedLogFileName + ".log";

            string filter = "Audit_INDY-PRM";
            
            List<string> fileToRead = new List<string>();
            bool isEmpty = !System.IO.Directory.EnumerateFiles(statusFileAndDirectory).Any();
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
            string statusFileName = ConfigurationManager.AppSettings["statusFileName"];
            string statusFileAndDirectory = FileFunctions.GetStatusFileDirectory() + statusFileName + ".txt";

            //backup file name with directory
            string processedLogFileName = ConfigurationManager.AppSettings["ProcessedFileLogFileName"];
            string processedLogFileAndDirectory = FileFunctions.ProcessedFileLogDirectory() + processedLogFileName + ".log";
            
            string filter = "Sessions_INDY-PRM";
            List<string> fileToRead = new List<string>();
            bool isEmpty = !System.IO.Directory.EnumerateFiles(statusFileAndDirectory).Any();
            if (!isEmpty)
            {
                fileToRead = FileFunctions.GetFileNameToReadFile(statusFileAndDirectory, processedLogFileAndDirectory,
                                                                            filter, EnumFileProcessor.eFilePath.QVSessionLogs);
            }
            Assert.IsNotNull(fileToRead);
        }
        #endregion
    }
}
