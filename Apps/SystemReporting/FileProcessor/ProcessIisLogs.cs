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
        public ProcessIisLogs(string args) { }

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
                    var filter = "u_ex";
                    var efilePath = EnumFileProcessor.eFilePath.IisLogs;

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
                BaseFileProcessor.LogError(ex, " Class ProcessIisLogs. Method ProcessFileData.",true);
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
            if (fileInfo == null)
            {
                BaseFileProcessor.LogError(null, "FileInfo missing. Can not process iisFile " + fileNameWithDirectory, true);
                return false;
            }

            var blnSucessful = false;
            try
            {
                var listLogFile = ParseFile(fileInfo.ToString());
                if (listLogFile != null & listLogFile.Count > 0)
                {
                    //list
                    var listProxyLogs = new List<ProxyIisLog>();
                    //Entity
                    var proxyLogEntry = new ProxyIisLog();
                    foreach (var entry in listLogFile)
                    {
                        //find what type of event 
                        var eventType = IisAccessEvent.GetEventType(entry);
                        proxyLogEntry.EventType = eventType.ToString();

                        //Do not process a record that is Unknown event or that does not have a user name associated with it

                        if ((eventType == IisAccessEvent.IisEventType.IIS_UNKNOWN_EVENT) || (entry.HasUserName() == false))
                            continue;

                        if (entry.TimeStamp != null)
                        {
                            proxyLogEntry.UserAccessDatetime = entry.TimeStamp.ToString();
                        }
                        proxyLogEntry.ClientIpAddress = (!string.IsNullOrEmpty(entry.ClientIp)) ? entry.ClientIp.Trim() : string.Empty;
                        //User
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
                                                                 .Where(key => key.StartsWith("qvDocMultipleRoot", StringComparison.CurrentCulture))
                                                                 .Select(key => ConfigurationManager.AppSettings[key])
                                                                 .ToArray();
                        //loop through each and match with document. If a match is found then use that entry as root path
                        var group = "";
                        foreach (var item in repositoryUrls)
                        {
                            group = IisAccessEvent.DecodeUriQuery(entry.UriQuery, item);
                            if (group != null && group != "")
                            {
                                if (group.ToLower().IndexOf("installedapplications", StringComparison.Ordinal) > -1)
                                {
                                    //go back to loop.
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (group != null && group != "")
                        {
                            //if the group still has the installedapplicaiton then ignore
                            if (group.ToLower().IndexOf("installedapplications", StringComparison.Ordinal) > -1)
                                group = "";

                            //substring(start postions,find last index of("_REDUCEDUSERQVWS) and remove everything after that occurance of _REDUCEDUSERQVWS
                            if (group.IndexOf("_REDUCEDUSERQVWS", StringComparison.Ordinal) > -1)
                                group = group.Substring(0, group.LastIndexOf("_REDUCEDUSERQVWS"));
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

                    #region IISLogIns and IISLastAccessRecords ONly
                    // DO NOT SORT BY, ORDER BY or GROUP BY. The iis data is in specific order.. use as is
                    var pfinalList = listProxyLogs.ToList();                   

                    var hashActiveIisUserSessions = new HashSet<string>();
                    for (var i = pfinalList.Count - 1; i >= 0; i--)
                    {
                        switch ((pfinalList[i]).EventType)
                        {
                            case "IIS_LOGIN":
                                {
                                    hashActiveIisUserSessions.Remove(pfinalList[i].User);
                                    break;
                                }
                            case "IIS_LAST_ACCESS":
                                {
                                    if (hashActiveIisUserSessions.Contains(pfinalList[i].User))
                                    {
                                        pfinalList.RemoveAt(i);
                                    }
                                    else
                                    {
                                        hashActiveIisUserSessions.Add(pfinalList[i].User);
                                    }
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                    #endregion
            
                    var listProxyLogsFF = pfinalList;
                    var listProxyLogsFinal = listProxyLogsFF.ToList();
                    if (listProxyLogsFinal.Count() == 0)
                    {
                        blnSucessful = true;//nothing to process
                    }
                    else
                    {
                        //process the list
                        blnSucessful = ControllerIisLog.ProcessLogs(listProxyLogsFinal);
                    }
                }
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, " Class ProcessIisLogs. Method ProcessLogFile while sending the data to controller. File " + fileNameWithDirectory,true);
            }

            return blnSucessful;
        }

        /// <summary>
        ///     Parse one log file and return the complete List of entries
        /// </summary>
        /// <param name="filefullName">todo: describe filefullName parameter on ParseFile</param>
        /// <returns>List<IisLogFileEntry></returns>
        public static List<IisLogFileEntry> ParseFile(string filefullName)
        {
            var fileLines = new List<string>();
            var listLogFile = new List<IisLogFileEntry>();

            string[] supportedIisVersion = { "8.5" };

            try
            {
                fileLines = File.ReadAllLines(filefullName).ToList();
                var thisIisVersion = fileLines[0].Substring(fileLines[0].IndexOfAny("0123456789".ToCharArray()));

                if (supportedIisVersion.Contains(thisIisVersion))
                {
                    // Strip out comment lines
                    fileLines = fileLines.SkipWhile(x => x.StartsWith("#", StringComparison.Ordinal)).ToList();
                    if (fileLines.Count > 0)
                    {
                        foreach (string row in fileLines)
                        {
                            var entry = new IisLogFileEntry(row);
                            listLogFile.Add(entry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, " Class ProcessIisLogs. Method ParseFile. File name. " + filefullName, true);
            }
            return listLogFile;
        }
    }
}
