using ReportingCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milliman.Controller;
using Milliman.Controller.BusinessLogic.Controller;
using Milliman.Entities.Proxy;

namespace FileProcessor
{
    public class ProcessIisLogs : ControllerAccess, IFileProcessor
    {
        DateTime _dateTimeReceived = new DateTime();
        public ProcessIisLogs(string args)
        {
            _dateTimeReceived = DateTime.Now;
        }

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
                    EnumFileProcessor.eFilePath efilePath = EnumFileProcessor.eFilePath.IisLogs;

                    //Move from ProductionLogsTest\IISLogs\
                    var dirInfo = new DirectoryInfo(ConfigurationManager.AppSettings["IISLogsS"]);
                    //ProductionLogsTest\IISLogs\
                    var sourceDirectory = new DirectoryInfo(FileFunctions.GetFileOriginalSourceDirectory(efilePath));

                    //Move To   LogFileProcessor\IN\IISLogs\
                    var destInDirectory = FileFunctions.GetFileProcessingInDirectory(efilePath);
                    //backUp    LogFileProcessor\BackUp\IISLogs
                    var backUpDirectory = FileFunctions.GetFileBackUpDirectory(efilePath);

                    string filter = "u_ex";
                    if (sourceDirectory.Exists)
                    {                        
                        List<string> listFileToProcess = FileFunctions.GetFileToReadFromStatusFile(filter, efilePath);

                        if (listFileToProcess.Count > 0)
                        {
                            foreach (var file in listFileToProcess)
                            {
                                string fileFullNameWithSourcePath = dirInfo + file;
                                if (File.Exists(fileFullNameWithSourcePath))
                                {
                                    FileFunctions.CopyFile(dirInfo + file, efilePath, true);
                                    blnSucessful = ProcessLogFile(destInDirectory + file);
                                    if (blnSucessful)
                                        File.Move(destInDirectory + file, backUpDirectory + file);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, " Class ProcessIisLogs. Method ProcessFileData.");
            }
        }

        /// <summary>
        /// This method process file by first parsing it, and then generating the proxy entity
        /// </summary>
        /// <param name="args"></param>
        public bool ProcessLogFile(string fileFullNameWithSourcePath)
        {
            FileInfo fileInfo = new FileInfo(fileFullNameWithSourcePath);
            if (fileInfo == null)
            {
                BaseFileProcessor.LogError(null, "FileInfo missing. Can not process iisFile " + fileFullNameWithSourcePath);
                return false;
            }

            bool blnSucessful = false;
            try
            {
                List<IisLogFileEntry> listLogFile = ParseFile(fileInfo.ToString());
                if (listLogFile != null & listLogFile.Count > 0)
                {
                    //list
                    List<ProxyIisLog> listProxyLogs = new List<ProxyIisLog>();
                    //Entity
                    ProxyIisLog proxyLogEntry = new ProxyIisLog();
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
                            proxyLogEntry.LogCreateDate = entry.TimeStamp.ToString("MM/dd/yy");
                            proxyLogEntry.LogCreateTime = entry.TimeStamp.ToString("HH:mm:ss");
                        }
                        proxyLogEntry.ClientIpAddress = (!string.IsNullOrEmpty(entry.ClientIp)) ? entry.ClientIp.Trim() : string.Empty;
                        proxyLogEntry.UserName = (!string.IsNullOrEmpty(entry.UserName)) ? entry.UserName.Trim() : string.Empty;
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
                                                            .ThenBy(b => b.UserName)
                                                            .ThenByDescending(c => c.Group);

                    //give me distinct latest and last records based on the two fields UN (if there is any) & Event
                    var listProxyLogsDistinctRecords = listProxyLogsOrdered.GroupBy(x => new { x.EventType, x.UserName, x.Group })
                                                                        .Select(y => y.First())
                                                                        .OrderBy(x => x.UserName).ThenByDescending(x => x.EventType)
                                                                        .Distinct();

                    var listProxyExcludeDups = listProxyLogsDistinctRecords.GroupBy(x => new { x.UserName, x.EventType })
                                                                        .Select(y => y.First())
                                                                        .OrderBy(x => x.UserName).ThenByDescending(x => x.EventType)
                                                                        .Distinct();

                    List<ProxyIisLog> listProxyLogsFinal = listProxyExcludeDups.ToList();
                    //process the list
                    blnSucessful = ControllerIisLog.ProcessLogs(listProxyLogsFinal);
                    if (blnSucessful)
                    {
                        BaseFileProcessor.LogError(null, "ProcessIisLogs: Successfully processed fileInfo ||" + fileFullNameWithSourcePath);
                    }
                    else
                    {
                        BaseFileProcessor.LogError(null, "ProcessIisLogs: Failed processing fileInfo || " + fileFullNameWithSourcePath);
                    }
                }
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, " Class ProcessIisLogs. Method ProcessLogFile while sending the data to controller.");
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
            List<IisLogFileEntry> listLogFile = new List<IisLogFileEntry>();
            string[] supportedIisVersion = { "8.5" };

            try
            {
                List<string> fileLines = File.ReadAllLines(filefullName).ToList();
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
                BaseFileProcessor.LogError(ex, " Class ProcessIisLogs. Method ParseFile. File name. " + filefullName);
            }
            return listLogFile;
        }
    }
}
