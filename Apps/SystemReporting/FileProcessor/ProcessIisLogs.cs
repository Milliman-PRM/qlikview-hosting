using ReportingCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemReporting.Controller;
using SystemReporting.Controller.BusinessLogic.Controller;
using SystemReporting.Entities.Proxy;
using SystemReporting.Utilities;

namespace FileProcessor
{
    public class ProcessIisLogs : ControllerAccess, IFileProcessor
    {
        public ProcessIisLogs(string args) {  }

        /// <summary>
        /// Process the iis files. Args are only the file type like iis. 
        /// The file will be picked up by method GetFileFromDirectory
        /// </summary>
        /// <param name="args"></param>
        public void ProcessFileData(string args)
        {
            bool blnSucessful = false;
            try
            {
                if (args.Length > 0)
                {
                    string filter = "u_ex";
                    EnumFileProcessor.eFilePath efilePath = EnumFileProcessor.eFilePath.IisLogs;
                    
                    // ProductionLogsTest\IISLogs\
                    var sourceDirectory = new DirectoryInfo(FileFunctions.GetFileOriginalSourceDirectory(efilePath));

                    //LogFileProcessor\IN
                    var destinationInDirectory = new DirectoryInfo(FileFunctions.GetFileProcessingInDirectory(efilePath));
                                        
                    if (sourceDirectory.Exists)
                    {
                        List<string> listFileToProcess = FileFunctions.GetFileToReadFromStatusFile(filter, efilePath);
                        if (listFileToProcess.Count <= 0)
                        {
                            Console.WriteLine("No files exist to Process.");
                        }

                        if (listFileToProcess.Count > 0)
                        {
                            foreach (var file in listFileToProcess)
                            {
                                Console.WriteLine("Processing file:  {0}." , file);
                                string fileNameWithsourceDirectory = (sourceDirectory + file);
                                if (File.Exists(fileNameWithsourceDirectory))
                                {
                                    FileFunctions.CopyFile(sourceDirectory + file, efilePath, true);
                                    blnSucessful = ProcessLogFile(destinationInDirectory + file);
                                    if (blnSucessful)
                                    {
                                        File.Delete(destinationInDirectory + file);
                                        foreach(var item in listFileToProcess)
                                        {
                                            BaseFileProcessor.LogProcessedFile(item);
                                        }                                        
                                    }
                                    else
                                    {
                                        Logger.LogError(null, "ProcessIisLogs: Failed processing fileInfo || " + fileNameWithsourceDirectory);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, " Class ProcessIisLogs. Method ProcessFileData.");
            }
        }

        /// <summary>
        /// This method process file by first parsing it, and then generating the proxy entity
        /// </summary>
        /// <param name="args"></param>
        public bool ProcessLogFile(string fileNameWithDirectory)
        {
            FileInfo fileInfo = new FileInfo(fileNameWithDirectory);
            if (fileInfo == null)
            {
                Logger.LogError(null, "FileInfo missing. Can not process iisFile " + fileNameWithDirectory);
                return false;
            }

            bool blnSucessful = false;
            try
            {
                List<IisLogFileEntry> listLogFile = ParseFile(fileInfo.ToString());
                if (listLogFile != null & listLogFile.Count > 0)
                {
                    //list
                    var listProxyLogs = new List<ProxyIisLog>();
                    //Entity
                    var proxyLogEntry = new ProxyIisLog();
                    foreach (var entry in listLogFile)
                    {
                        //find what type of event 
                        ReportingCommon.IisAccessEvent.IisEventType eventType = IisAccessEvent.GetEventType(entry);
                        proxyLogEntry.EventType = eventType.ToString();

                        //Do not process a record that is Unknown event or that does not have a user name associated with it
                        if ((eventType == IisAccessEvent.IisEventType.IIS_UNKNOWN_EVENT) || (entry.HasUserName() == false))
                            continue;

                        if (entry.TimeStamp != null)
                        {
                            proxyLogEntry.UserAccessDatetime = entry.TimeStamp.ToString();
                        }
                        proxyLogEntry.ClientIpAddress = (!string.IsNullOrEmpty(entry.ClientIp)) ? entry.ClientIp.Trim() : string.Empty;
                        proxyLogEntry.User = (!string.IsNullOrEmpty(entry.UserName)) ? entry.UserName.Trim() : string.Empty;
                        proxyLogEntry.ServerIPAddress = (!string.IsNullOrEmpty(entry.ServerIp)) ? entry.ServerIp.Trim() : string.Empty;
                        proxyLogEntry.PortNumber = entry.Port != 0 ? entry.Port : 0;
                        proxyLogEntry.CommandSentMethod = (!string.IsNullOrEmpty(entry.Method)) ? entry.Method.Trim() : string.Empty;
                        proxyLogEntry.StepURI = (!string.IsNullOrEmpty(entry.UriStem)) ? entry.UriStem.Trim() : string.Empty;

                        //Group
                        proxyLogEntry.QueryURI = (!string.IsNullOrEmpty(entry.UriQuery)) ? entry.UriQuery.Trim() : string.Empty;
                        //**************************************************************************************************//
                        //GetAll the QVDoc Keys as it can be more than one
                        var repositoryUrls = ConfigurationManager.AppSettings.AllKeys
                                                                 .Where(key => key.StartsWith("qvDocMultipleRoot"))
                                                                 .Select(key => ConfigurationManager.AppSettings[key])
                                                                 .ToArray();
                        //loop through each and match with document. If a match is found then use that entry as root path
                        var group = "";
                        foreach (var item in repositoryUrls)
                        {
                            group = IisAccessEvent.DecodeUriQuery(entry.UriQuery, item);
                            if (group != null)
                                break;
                        }
                        proxyLogEntry.Group = (!string.IsNullOrEmpty(group)) ? group : string.Empty;
                        //**************************************************************************************************//
                        proxyLogEntry.StatusCode = entry.Status != 0 ? entry.Status : 0;
                        proxyLogEntry.SubStatusCode = entry.Substatus != 0 ? entry.Substatus : 0;
                        proxyLogEntry.Win32StatusCode = entry.Win32Status != 0 ? entry.Win32Status : 0;
                        proxyLogEntry.ResponseTime = entry.TimeTaken != 0 ? entry.TimeTaken : 0;

                        //Browser 
                        proxyLogEntry.UserAgent = (!string.IsNullOrEmpty(entry.UserAgent)) ? entry.UserAgent.Trim() : string.Empty;
                        if (!string.IsNullOrEmpty(entry.UserAgent))
                        {
                            var browser = IisAccessEvent.GetBrowserFromAgentString(entry.UserAgent);
                            proxyLogEntry.Browser = (!string.IsNullOrEmpty(browser)) ? browser : string.Empty;
                        }

                        //TODO - Something special needs to happen for this
                        proxyLogEntry.ClientReferer = (!string.IsNullOrEmpty(entry.Referer)) ? entry.Referer.Trim() : string.Empty;

                        //add entry to list
                        listProxyLogs.Add(proxyLogEntry);
                        //start new line
                        proxyLogEntry = new ProxyIisLog();
                    }

                    //sort the list
                    var listProxyLogsOrdered = listProxyLogs.OrderBy(a => a.EventType)
                                                            .ThenBy(b => b.User)
                                                            .ThenByDescending(c => c.Group);

                    //give me distinct latest and last records based on the two fields UN (if there is any) & Event
                    var listProxyLogsDistinctRecords = listProxyLogsOrdered.GroupBy(x => new { x.EventType, x.User, x.Group })
                                                                        .Select(y => y.First())
                                                                        .OrderBy(x => x.User).ThenByDescending(x => x.EventType)
                                                                        .Distinct();

                    var listProxyExcludeDups = listProxyLogsDistinctRecords.GroupBy(x => new { x.User, x.EventType })
                                                                        .Select(y => y.First())
                                                                        .OrderBy(x => x.User).ThenByDescending(x => x.EventType)
                                                                        .Distinct();

                    List<ProxyIisLog> listProxyLogsFinal = listProxyExcludeDups.ToList();
                    //process the list
                    blnSucessful = ControllerIisLog.ProcessLogs(listProxyLogsFinal);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, " Class ProcessIisLogs. Method ProcessLogFile while sending the data to controller.");
            }

            return blnSucessful;
        }

        /// <summary>
        ///     Parse one log file and return the complete List of entries
        /// </summary>
        /// <param name="LogFileName">The name of the log file to be parsed</param>
        /// <param name="LogFileName">The start of the defined date range</param>
        /// <param name="LogFileName">The end of the defined date range</param>
        /// <returns>List<IisLogFileEntry></returns>
        public static List<IisLogFileEntry> ParseFile(string filefullName)
        {
            var fileLines = new List<string>();
            var listLogFile = new List<IisLogFileEntry>();

            string[] supportedIisVersion = { "8.5" };

            try
            {
                fileLines = File.ReadAllLines(filefullName).ToList();
                string thisIisVersion = fileLines[0].Substring(fileLines[0].IndexOfAny("0123456789".ToCharArray()));

                if (supportedIisVersion.Contains(thisIisVersion))
                {
                    // Strip out comment lines
                    fileLines = fileLines.SkipWhile(x => x.StartsWith("#")).ToList();
                    if (fileLines.Count > 0)
                    {
                        foreach (string row in fileLines)
                        {
                            IisLogFileEntry entry = new IisLogFileEntry(row);
                            listLogFile.Add(entry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, " Class ProcessIisLogs. Method ParseFile. File name. " + filefullName);
            }
            return listLogFile;
        }
    }
}
