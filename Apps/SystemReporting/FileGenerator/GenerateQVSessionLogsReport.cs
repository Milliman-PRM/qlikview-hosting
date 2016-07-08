using FileProcessor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using SystemReporting.Entities.Models;
using SystemReporting.Utilities;
using SystemReporting.Utilities.ExceptionHandling;

namespace ReportFileGenerator
{
    public class GenerateQVSessionLogsReport: ControllerAccess
    {
        public static void GenerateReport(DateTime? startDateValue, DateTime? endDateValue,
                                                    string reportType, string outputType, string logReportName, string fileNameWithFolderPath)
        {
            try
            {
                var reportName = (!string.IsNullOrEmpty(logReportName) ? logReportName : GetLogReportName());

                var reportTypeEum = (Enumerations.eReportType)Enum.Parse(typeof(Enumerations.eReportType), reportType);
                switch (reportTypeEum)
                {
                    case Enumerations.eReportType.Group:
                        ProcessReportGenerateForGroupName(startDateValue, endDateValue, reportName, outputType, fileNameWithFolderPath);
                        break;
                    case Enumerations.eReportType.Report:
                        ProcessReportGenerateForReportName(startDateValue, endDateValue, reportName, outputType, fileNameWithFolderPath);
                        break;
                    case Enumerations.eReportType.User:
                        ProcessReportGenerateForUserName(startDateValue, endDateValue, reportName, outputType, fileNameWithFolderPath);
                        break;
                    default:
                        throw new ApplicationException("Output type not supported");
                }
            }
            catch(Exception ex)
            {
                 ExceptionLogger.LogError(ex, "Exception Raised in Method GenerateReport. ", "GenerateQVSessionLogsReport Exceptions");
            }
        }        

        public static void ProcessReportGenerateForGroupName(DateTime? startDateValue, DateTime? endDateValue,
                                                                string reportName, string outputType, string fileNameWithFolderPath)
        {
            var ca = new ControllerAccess();
            var list = ca.ControllerSessionLog.GetSessionLogListForGroup(startDateValue.Value.ToString(),
                                                                        endDateValue.Value.ToString(),
                                                                        reportName);

            var outputTypeEum = (Enumerations.eOutputType)Enum.Parse(typeof(Enumerations.eOutputType), outputType);
            switch (outputTypeEum)
            {
                case Enumerations.eOutputType.CSV:
                    GenerateCSVFileForGroupName(list, fileNameWithFolderPath);
                    break;
                case Enumerations.eOutputType.EXCEL:
                    GenerateExcelFileForGroupName(list, fileNameWithFolderPath);
                    break;
                case Enumerations.eOutputType.TEXT:
                    GenerateTxtFileForGroupName(list, fileNameWithFolderPath);
                    break;
                default:
                    throw new ApplicationException("Output type not supported");
            }

        }
        public static void ProcessReportGenerateForReportName(DateTime? startDateValue, DateTime? endDateValue,
                                                                string reportName, string outputType, string fileNameWithFolderPath)
        {
            var ca = new ControllerAccess();
            var list = ca.ControllerSessionLog.GetSessionLogListForReport(startDateValue.Value.ToString(),
                                                                    endDateValue.Value.ToString(), reportName);

            var outputTypeEum = (Enumerations.eOutputType)Enum.Parse(typeof(Enumerations.eOutputType), outputType);
            switch (outputTypeEum)
            {
                case Enumerations.eOutputType.CSV:
                    GenerateCSVFileForReportName(list, fileNameWithFolderPath);
                    break;
                case Enumerations.eOutputType.EXCEL:
                    GenerateExcelFileForReportName(list, fileNameWithFolderPath);
                    break;
                case Enumerations.eOutputType.TEXT:
                    GenerateTxtFileForReportName(list, fileNameWithFolderPath);
                    break;
                default:
                    throw new ApplicationException("Output type not supported");
            }
        }
        public static void ProcessReportGenerateForUserName(DateTime? startDateValue, DateTime? endDateValue,
                                                                string reportName, string outputType, string fileNameWithFolderPath)
        {
            var ca = new ControllerAccess();
            var list = ca.ControllerSessionLog.GetSessionLogListForUser(startDateValue.Value.ToString(),
                                                                    endDateValue.Value.ToString(), reportName);

            var outputTypeEum = (Enumerations.eOutputType)Enum.Parse(typeof(Enumerations.eOutputType), outputType);
            switch (outputTypeEum)
            {
                case Enumerations.eOutputType.CSV:
                    GenerateCSVFileForUserName(list, fileNameWithFolderPath);
                    break;
                case Enumerations.eOutputType.EXCEL:
                    GenerateExcelFileForUserName(list, fileNameWithFolderPath);
                    break;
                case Enumerations.eOutputType.TEXT:
                    GenerateTxtFileForUserName(list, fileNameWithFolderPath);
                    break;
                default:
                    throw new ApplicationException("Output type not supported");
            }
        }
        private static void GenerateExcel(List<SessionLog> list, string fileNameWithFolderPath)
        {
            var dt = ExtensionMethods.ToDataTable(list);
            if (dt != null)
            {
                dt.Columns.Remove("ListIisLog");
                dt.Columns.Remove("ListSessionLog");
                dt.Columns.Remove("ListAuditLog");
                dt.Columns.Remove("ReportDesctiption");
                dt.Columns.Remove("Description");

                var file = string.Empty;
                if (string.IsNullOrEmpty(fileNameWithFolderPath))
                {
                    //Save Back To File
                    file = GetFileDirectroy() + "SessionLog" + "_" + DateTime.Now.ToString("MMdd_hhmm") + ".xls";
                }
                else
                {
                    file = fileNameWithFolderPath;
                }

                var HtmlBody = ExtensionMethods.ExportDatatableToHtml(dt);
                File.WriteAllText(file, HtmlBody);
            }
        }

        private static void GenerateCSV(List<SessionLog> list, string fileNameWithFolderPath)
        {
            var resultsList = new List<string>();

            //Date/Time,QVW,QVW Close Reason,User Session Length (HH:MM:SS),User,Browser
            foreach (SessionLog curData in list)
                resultsList.Add(
                                (curData.UserAccessDatetime.HasValue ? curData.UserAccessDatetime.Value.ToString() : "NULL").ToString() + "," +
                                (!string.IsNullOrEmpty(curData.Document) ? curData.Document.ToString() : "NULL").ToString() + "," +
                                (!string.IsNullOrEmpty(curData.SessionEndReason) ? curData.SessionEndReason.ToString() : "NULL").ToString() + "," +
                                (!string.IsNullOrEmpty(curData.SessionDuration) ? curData.SessionDuration.ToString() : "NULL").ToString() + "," +
                                (!string.IsNullOrEmpty(curData.User.UserName) ? curData.User.UserName.ToString() : "NULL").ToString() + "," +
                                curData.Browser.ToString()
                                );

            //Save Back To File
            var file = string.Empty;
            if (string.IsNullOrEmpty(fileNameWithFolderPath))
            {
                //Save Back To File
                file = GetFileDirectroy() + "Session" + "_" + DateTime.Now.ToString("MMdd_hhmm") + ".csv";
            }
            else
            {
                file = fileNameWithFolderPath;
            }

            //write file
            if (!File.Exists(file))
                File.WriteAllLines(file, resultsList.ToArray());

            //now add the header
            string path = file;
            string str;
            using (StreamReader sreader = new StreamReader(path))
            {
                str = sreader.ReadToEnd();
            }

            //delete file
            File.Delete(path);
            using (StreamWriter swriter = new StreamWriter(path, false))
            {
                //add header
                str = "Date/Time,QVW,QVW Close Reason,User Session Length (HH:MM:SS),User,Browser" + Environment.NewLine + str;
                swriter.Write(str);
            }
        }

        #region Report By GroupName
        /// <summary>
        /// generate csv file for selected columns only
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fileNameWithFolderPath"></param>
        public static void GenerateCSVFileForGroupName(List<SessionLog> list, string fileNameWithFolderPath)
        {
            GenerateCSV(list, fileNameWithFolderPath);
        }
               
        /// <summary>
        /// generate excel file for the full entity properties
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fileNameWithFolderPath"></param>
        public static void GenerateExcelFileForGroupName(List<SessionLog> list, string fileNameWithFolderPath)
        {
            GenerateExcel(list, fileNameWithFolderPath);
        }
          
        public static void GenerateTxtFileForGroupName(List<SessionLog> list, string fileNameWithFolderPath)
        {
            throw new NotImplementedException("Program function is not implemented.");
        }

        #endregion

        #region Report By ReportName
        public static void GenerateCSVFileForReportName(List<SessionLog> list, string fileNameWithFolderPath)
        {
            GenerateCSV(list, fileNameWithFolderPath);
        }

        public static void GenerateExcelFileForReportName(List<SessionLog> list, string fileNameWithFolderPath)
        {
            GenerateExcel(list, fileNameWithFolderPath);
        }

        public static void GenerateTxtFileForReportName(List<SessionLog> list, string fileNameWithFolderPath)
        {
            throw new NotImplementedException("Program function is not implemented.");
        }
        #endregion

        #region Report By UserName
        public static void GenerateCSVFileForUserName(List<SessionLog> list, string fileNameWithFolderPath)
        {
            GenerateCSV(list, fileNameWithFolderPath);
        }

        public static void GenerateExcelFileForUserName(List<SessionLog> list, string fileNameWithFolderPath)
        {
            GenerateExcel(list, fileNameWithFolderPath);
        }

        public static void GenerateTxtFileForUserName(List<SessionLog> list, string fileNameWithFolderPath)
        {
            throw new NotImplementedException("Program function is not implemented.");
        }
        #endregion

        private static string GetFileDirectroy()
        {
            // For Example - D:\Projects\SomeProject\SomeFolder
            return (ConfigurationManager.AppSettings != null &&
                    ConfigurationManager.AppSettings["GenerateQVSessionLogsReportDirectory"] != null) ?
                    ConfigurationManager.AppSettings["GenerateQVSessionLogsReportDirectory"].ToString() :
                    string.Empty;
        }

        private static string GetLogReportName()
        {
            // For Example - D:\Projects\SomeProject\SomeFolder
            return (ConfigurationManager.AppSettings != null &&
                    ConfigurationManager.AppSettings["LogReportName"] != null) ?
                    ConfigurationManager.AppSettings["LogReportName"].ToString() :
                    string.Empty;
        }
    }
}
