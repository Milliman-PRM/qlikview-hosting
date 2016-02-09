using SystemReporting.Entities.Proxy;
using ReportingCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemReporting.Utilities;

namespace FileProcessor
{
    public class ProcessQVAuditLogs : ControllerAccess, IFileProcessor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args"></param>
        public ProcessQVAuditLogs(string args) { }

        /// <summary>
        /// Process the files. 
        /// </summary>
        /// <param name="args"></param>
        public void ProcessFileData(string args)
        {
            var blnSucessful = false;
            try
            {
                if (args.Length > 0)
                {
                    var filter = "Audit_INDY-PRM";
                    var efilePath = EnumFileProcessor.eFilePath.QVAuditLogs;

                    //ProductionLogsTest\QVLogs\
                    var sourceDirectory = new DirectoryInfo(FileFunctions.GetFileOriginalSourceDirectory(efilePath));

                    //Move To   LogFileProcessor\IN\
                    var destinationInDirectory = FileFunctions.GetFileProcessingInDirectory();
                                       
                    if (sourceDirectory.Exists)
                    {
                        var listFileToProcess = FileFunctions.GetFileToReadFromStatusFile(filter, efilePath);

                        if (listFileToProcess.Count <=0)
                        {
                            Console.WriteLine("No files exist to Process.");
                        }
                        foreach (var file in listFileToProcess)
                        {
                            Console.WriteLine("Processing file:  {0}.", file);
                            var fileNameWithsourceDirectory = sourceDirectory + file;
                            if (File.Exists(fileNameWithsourceDirectory))
                            {
                                FileFunctions.CopyFile(sourceDirectory + file, efilePath, true);
                                blnSucessful = ProcessLogFile(destinationInDirectory + file);
                                if (blnSucessful)
                                {
                                    File.Delete(destinationInDirectory + file);
                                    foreach (var item in listFileToProcess)
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
            catch (Exception ex)
            {
                Logger.LogError(ex, "Class ProcessQVAuditLogs. Method ProcessFileData.");
            }
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
                    var listProxyLogs = new List<ProxyAuditLog>();
                    //Entity
                    var proxyLogEntry = new ProxyAuditLog();

                    //Time Zone for aduit log is true
                    var UseDaylightSavings = true;
                    //local time zone of the server
                    var serverTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.Id);
                    var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, serverTimeZone);

                    foreach (var entry in listLogFile)
                    {
                        if (entry.Timestamp != null)
                        {
                            if (UseDaylightSavings)
                            {
                                proxyLogEntry.UserAccessDatetime = TimeZoneInfo.ConvertTimeToUtc(entry.Timestamp, serverTimeZone).ToString("MM/dd/yy HH:mm:ss");
                            }
                            else
                            {
                                proxyLogEntry.UserAccessDatetime = (entry.Timestamp - serverTimeZone.BaseUtcOffset).ToString("MM/dd/yy HH:mm:ss");
                            }
                        }
                        proxyLogEntry.Document = (!string.IsNullOrEmpty(entry.Document)) ? entry.Document.Trim() : string.Empty;
                        proxyLogEntry.EventType = (!string.IsNullOrEmpty(entry.EventType)) ? entry.EventType.Trim() : string.Empty;

                        var user = entry.User.Replace(@"Custom\", "").Replace(@"custom\", "");
                        proxyLogEntry.User = (!string.IsNullOrEmpty(user)) ? user.Trim() : "UnKnown User";
                        proxyLogEntry.Message = (!string.IsNullOrEmpty(entry.Message)) ? entry.Message.Trim() : string.Empty;

                        //**************************************************************************************************//
                        //GetAll the QVDoc Keys as it can be more than one
                        var repositoryUrls = ConfigurationManager.AppSettings.AllKeys
                                                     .Where(key => key.StartsWith("qvDocMultipleRoot", StringComparison.Ordinal))
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

                        proxyLogEntry.IsReduced = entry.Document.IndexOf(@"\reducedcachedqvws", StringComparison.Ordinal) > -1;

                        //add entry to list
                        listProxyLogs.Add(proxyLogEntry);
                        proxyLogEntry = new ProxyAuditLog();
                    }
                    //process the list
                    blnSucessful = ControllerAuditLog.ProcessLogs(listProxyLogs);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, " Class ProcessQVAuditLogs. Method ProcessLogFile while sending the data to controller.");
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
            var fileLines = new List<string>();
            var listLogFile = new List<QlikviewAuditLogEntry>();

            try
            {
                fileLines = File.ReadAllLines(filefullName).ToList();
                if (fileLines != null && fileLines.Count > 0)
                    fileLines.RemoveAt(0);//removes the first line

                foreach (string line in fileLines)
                {
                    var entry = new QlikviewAuditLogEntry(line);
                    listLogFile.Add(entry);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, " Class ProcessQVAuditLogs. Method ParseFile. File name. " + filefullName);
            }
            return listLogFile;
        }
    }
}
