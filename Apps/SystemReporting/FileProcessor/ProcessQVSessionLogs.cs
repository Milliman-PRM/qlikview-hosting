using ReportingCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using SystemReporting.Entities.Models;
using SystemReporting.Utilities;

namespace FileProcessor
{
    public class ProcessQVSessionLogs : ControllerAccess, IFileProcessor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args"></param>
        public ProcessQVSessionLogs(string args) { }

        /// <summary>
        /// Process the iis files. Args are only the file type like iis. 
        /// The file will be picked up by method GetFileFromDirectory
        /// </summary>
        /// <param name="args"></param>
        public void ProcessLogFileData(string args)
        {
            try
            {
                if (args.Length > 0)
                {
                    var filter = "Sessions_INDY-PRM";
                    var efilePath = EnumFileProcessor.eFilePath.QVSessionLogs;
                    // ProductionLogsTest\IISLogs\
                    var sourceDirectory = new DirectoryInfo(FileFunctions.GetFileOriginalSourceDirectory(efilePath));

                    //LogFileProcessor\IN
                    var destinationInDirectory = new DirectoryInfo(FileFunctions.GetFileProcessingInDirectory());

                    if (sourceDirectory.Exists)
                    {
                        if (args.IndexOf("productionlogs", StringComparison.Ordinal) > -1)
                        {
                            var filename = Path.GetFileName(args);
                            ProcessLogFileMove(efilePath, sourceDirectory, destinationInDirectory, filename);
                        }
                        else
                        {
                            var listFileToProcess = FileFunctions.GetFileToReadFromStatusFile(filter, efilePath);
                            if (listFileToProcess.Count > 0)
                            {
                                foreach (var file in listFileToProcess)
                                {
                                    ProcessLogFileMove(efilePath, sourceDirectory, destinationInDirectory, file);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, "Class ProcessQVSessionLogs. Method ProcessFileData.");
            }
        }

        private bool ProcessLogFileMove(EnumFileProcessor.eFilePath efilePath, DirectoryInfo sourceDirectory,
                                                                    DirectoryInfo destinationInDirectory, string file)
        {
            var blnSucessful = false;
            var fileNameWithsourceDirectory = (sourceDirectory + file);
            if (File.Exists(fileNameWithsourceDirectory))
            {
                FileFunctions.CopyFile(sourceDirectory + file, efilePath, true);
                blnSucessful = ProcessLogFile(destinationInDirectory + file);
                if (blnSucessful)
                {
                    File.Delete(destinationInDirectory + file);
                    BaseFileProcessor.LogProcessedFile(file);
                }
            }
            return blnSucessful;
        }

        /// <summary>
        /// This method process file by first parsing it, and then generating the proxy entity
        /// </summary>
        /// <param name="fileNameWithDirectory">todo: describe fileNameWithDirectory parameter on ProcessLogFile</param>
        public bool ProcessLogFile(string fileNameWithDirectory)
        {
            var fileInfo = new FileInfo(fileNameWithDirectory);
            var ff = new FileFunctions();

            var blnSucessful = false;
            try
            {
                var listLogFile = ParseLogFile(fileInfo.ToString());

                if (listLogFile != null & listLogFile.Count > 0)
                {
                    var listProxyLogs = new List<ProxySessionLog>();
                    //Entity
                    var proxyLogEntry = new ProxySessionLog();

                    //Time Zone is false for session
                    var UseDaylightSavings = false;
                    var serverTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

                    foreach (var entry in listLogFile)
                    {                        
                        proxyLogEntry.ExitReason = (!string.IsNullOrEmpty(entry.ExitReason)) ? entry.ExitReason.Trim() : string.Empty;

                        if (entry.SessionStart != null)
                            proxyLogEntry.SessionStartDateTime = entry.SessionStart.ToString();

                        if (entry.SessionStart != null)
                        {
                            if (!UseDaylightSavings)
                                proxyLogEntry.UserAccessDatetime = TimeZoneInfo.ConvertTimeToUtc(entry.SessionStart, serverTimeZone).ToString();
                        }

                        if (entry.SessionDuration != null)
                            proxyLogEntry.SessionDuration = entry.SessionDuration.ToString();

                        proxyLogEntry.CpuSpentS = entry.CpuSpentS != 0 ? entry.CpuSpentS : 0.0;

                        //remove character in begging of email
                        if ((!string.IsNullOrEmpty(entry.IdentifyingUser)) && (entry.IdentifyingUser != "Identifying user"))
                        {
                            proxyLogEntry.User = (!string.IsNullOrEmpty(entry.ClientType)) ? entry.IdentifyingUser.Replace(@"Custom\", "").Replace(@"custom\", "") : string.Empty;
                        }

                        proxyLogEntry.ClientType = (!string.IsNullOrEmpty(entry.ClientType)) ? entry.ClientType.Trim() : string.Empty;
                        proxyLogEntry.ClientAddress = (!string.IsNullOrEmpty(entry.ClientAddress)) ? entry.ClientAddress.Trim() : string.Empty;
                        proxyLogEntry.CalType = (!string.IsNullOrEmpty(entry.CalType)) ? entry.CalType.Trim() : string.Empty;
                        proxyLogEntry.CalUsageCount = entry.CalUsageCount != 0 ? entry.CalUsageCount : 0;

                        proxyLogEntry.IsReduced = entry.Document.IndexOf(@"\reducedcachedqvws", StringComparison.Ordinal) > -1;
                        //**************************************************************************************************//
                        //GetAll the QVDoc Keys as it can be more than one
                        var repositoryUrls = ConfigurationManager.AppSettings.AllKeys
                                                     .Where(key => key.StartsWith("qvDocMultipleRoot", StringComparison.Ordinal))
                                                     .Select(key => ConfigurationManager.AppSettings[key])
                                                     .ToArray();
                        //loop through each and match with document. If a match is found then use that entry as root path
                        var group = "";
                        var docNameNoPath = "";
                        foreach (var item in repositoryUrls)
                        {
                            if (entry.Document.ToLower().Contains(item.ToLower()))
                            {
                                group = QlikviewEventBase.GetGroup(entry.Document, item);
                                docNameNoPath = entry.Document.ToLower().Replace(item.ToLower(), "");
                                break;
                            }
                        }

                        proxyLogEntry.Group = (!string.IsNullOrEmpty(group)) ? group : string.Empty;
                        //**************************************************************************************************//
                        proxyLogEntry.Report = QlikviewEventBase.GetReportName(entry.Document);

                        proxyLogEntry.Document = (!string.IsNullOrEmpty(docNameNoPath)) ? docNameNoPath.ToUpper().Trim() : string.Empty;

                        proxyLogEntry.SessionLength = entry.SessionDuration.ToString();
                        proxyLogEntry.SessionEndReason = QlikviewSessionEvent.GetExitReason(entry.ExitReason).ToString();
                        proxyLogEntry.Browser = QlikviewSessionEvent.GetBrowser(entry.ClientType);
                        //add entry to list
                        listProxyLogs.Add(proxyLogEntry);
                        proxyLogEntry = new ProxySessionLog();
                    }
                    //process the list
                    blnSucessful = ControllerSessionLog.ProcessLogs(listProxyLogs);
                }
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, " Class ProcessQVSessionLogs. Method ProcessLogFile while sending the data to controller.");
            }

            return blnSucessful;
        }

        /// <summary>
        /// Parse log file and returns the list
        /// </summary>
        /// <param name="filefullName"></param>
        /// <returns></returns>
        public static List<QlikviewSessionLogEntry> ParseLogFile(string filefullName)
        {
            var fileLines = new List<string>();
            var listLogFile = new List<QlikviewSessionLogEntry>();

            try
            {
                fileLines = File.ReadAllLines(filefullName).ToList();
                if (fileLines != null && fileLines.Count > 0)
                    fileLines.RemoveAt(0);//removes the first line

                foreach (string line in fileLines)
                {
                    var entry = new QlikviewSessionLogEntry(line);
                    listLogFile.Add(entry);
                }
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, " Class ProcessQVSessionLogs. Method ParseFile. File name. " + filefullName);
            }
            return listLogFile;
        }
    }
}
