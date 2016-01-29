using Milliman.Entities.Proxy;
using ReportingCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor
{
    public class ProcessQVAuditLogs : ControllerAccess, IFileProcessor
    {
        DateTime _dateTimeReceived = new DateTime();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args"></param>
        public ProcessQVAuditLogs(string args)
        {
            _dateTimeReceived = DateTime.Now;
        }

        /// <summary>
        /// Process the files. 
        /// </summary>
        /// <param name="args"></param>
        public void ProcessFileData(string args)
        {
            bool blnSucessful = false;
            try
            {
                if (args.Length > 0)
                {
                    EnumFileProcessor.eFilePath efilePath = EnumFileProcessor.eFilePath.QVAuditLogs;
                    //Move from ProductionLogsTest\QVLogs\
                    var dirInfo = new DirectoryInfo(ConfigurationManager.AppSettings["QVAuditLogsS"]);
                    //ProductionLogsTest\QVLogs\
                    var sourceDirectory = new DirectoryInfo(FileFunctions.GetFileOriginalSourceDirectory(efilePath));

                    //Move To   LogFileProcessor\IN\QVLogs\
                    var destInDirectory = FileFunctions.GetFileProcessingInDirectory(efilePath);
                    //backUp    LogFileProcessor\BackUp\QVLogs
                    var backUpDirectory = FileFunctions.GetFileBackUpDirectory(efilePath);

                    string filter = "Audit_INDY-PRM";
                    if (sourceDirectory.Exists)
                    {
                        List<string> listFileToProcess = FileFunctions.GetFileToReadFromStatusFile(filter, efilePath);

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
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, "Class ProcessQVAuditLogs. Method ProcessFileData.");
            }
        }

        /// <summary>
        /// This method process file by first parsing it, and then generating the proxy entity
        /// </summary>
        /// <param name="args"></param>
        public bool ProcessLogFile(string fileFullNameWithSourcePath)
        {
            FileInfo fileInfo = new FileInfo(fileFullNameWithSourcePath);
            FileFunctions ff = new FileFunctions();

            bool blnSucessful = false;
            try
            {
                List<QlikviewAuditLogEntry> listLogFile = ParseLogFile(fileInfo.ToString());

                if (listLogFile != null & listLogFile.Count > 0)
                {
                    List<ProxyAuditLog> listProxyLogs = new List<ProxyAuditLog>();
                    //Entity
                    ProxyAuditLog proxyLogEntry = new ProxyAuditLog();

                    //Time Zone
                    bool UseDaylightSavings = true;
                    //local time zone of the server
                    TimeZoneInfo serverTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.Id);
                    DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, serverTimeZone);

                    foreach (var entry in listLogFile)
                    {
                        if (entry.ServerStarted != null)
                        {
                            proxyLogEntry.ServerStarted = entry.ServerStarted.ToString("MM/dd/yy HH:mm:ss");
                        }

                        if (entry.Timestamp != null)
                        {
                            if (UseDaylightSavings)
                            {
                                proxyLogEntry.Timestamp = TimeZoneInfo.ConvertTimeToUtc(entry.Timestamp, serverTimeZone).ToString("MM/dd/yy HH:mm:ss");
                            }
                            else
                            {
                                proxyLogEntry.Timestamp = (entry.Timestamp - serverTimeZone.BaseUtcOffset).ToString("MM/dd/yy HH:mm:ss");
                            }
                        }

                        proxyLogEntry.Document = (!string.IsNullOrEmpty(entry.Document)) ? entry.Document.Trim() : string.Empty;
                        proxyLogEntry.EventType = (!string.IsNullOrEmpty(entry.EventType)) ? entry.EventType.Trim() : string.Empty;

                        var user = entry.User.Replace(@"Custom\", "").Replace(@"custom\", "");
                        proxyLogEntry.UserName = (!string.IsNullOrEmpty(user)) ? user.Trim() : "UnKnown User";

                        proxyLogEntry.Message = (!string.IsNullOrEmpty(entry.Message)) ? entry.Message.Trim() : string.Empty;

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
                            if (entry.Document.ToLower().Contains(item.ToLower()))
                            {
                                group = QlikviewEventBase.GetGroup(entry.Document, item);
                            }
                        }

                        proxyLogEntry.Group = (!string.IsNullOrEmpty(group)) ? group : string.Empty;
                        //**************************************************************************************************//

                        var report = QlikviewEventBase.GetReportName(entry.Document);
                        proxyLogEntry.Report = (!string.IsNullOrEmpty(report)) ? report : string.Empty;

                        proxyLogEntry.IsReduced = entry.Document.IndexOf(@"\reducedcachedqvws") > 0;

                        //add entry to list
                        listProxyLogs.Add(proxyLogEntry);
                        proxyLogEntry = new ProxyAuditLog();
                    }
                    //process the list
                    blnSucessful = ControllerAuditLog.ProcessLogs(listProxyLogs);
                    if (blnSucessful)
                    {
                        BaseFileProcessor.LogError(null, "ProcessQVAuditLogs: Successfully processed fileInfo " + fileFullNameWithSourcePath);
                    }
                    else
                    {
                        BaseFileProcessor.LogError(null, "ProcessQVAuditLogs: Failed processing fileInfo " + fileFullNameWithSourcePath);
                    }
                }
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, " Class ProcessQVAuditLogs. Method ProcessLogFile while sending the data to controller.");
            }

            return blnSucessful;
        }

        /// <summary>
        /// Parse log file and returns the list
        /// </summary>
        /// <param name="filefullName"></param>
        /// <returns></returns>
        public static List<QlikviewAuditLogEntry> ParseLogFile(string filefullName)
        {
            List<string> fileLines = new List<string>();
            List<QlikviewAuditLogEntry> listLogFile = new List<QlikviewAuditLogEntry>();

            try
            {
                fileLines = File.ReadAllLines(filefullName).ToList();
                if (fileLines != null && fileLines.Count > 0)
                    fileLines.RemoveAt(0);//removes the first line

                foreach (string line in fileLines)
                {
                    QlikviewAuditLogEntry entry = new QlikviewAuditLogEntry(line);
                    listLogFile.Add(entry);
                }
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, " Class ProcessQVAuditLogs. Method ParseFile. File name. " + filefullName);
            }
            return listLogFile;
        }
    }
}
