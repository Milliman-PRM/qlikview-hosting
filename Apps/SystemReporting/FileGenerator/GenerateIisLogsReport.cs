using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using SystemReporting.Entities.Models;
using SystemReporting.Utilities;
using SystemReporting.Utilities.ExceptionHandling;

namespace ReportFileGenerator
{
    public class GenerateIisLogsReport : ControllerAccess
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
                        //ProcessReportGenerateForReportName(startDateValue, endDateValue, reportName, outputType, fileNameWithFolderPath);
                        break;
                    case Enumerations.eReportType.User:
                        ProcessReportGenerateForUserName(startDateValue, endDateValue, reportName, outputType, fileNameWithFolderPath);
                        break;
                    default:
                        throw new ApplicationException("Output type not supported");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogError(ex, "Exception Raised in Method GenerateReport. ", "GenerateIisLogsReport Exceptions");
            }
        }

        public static void ProcessReportGenerateForGroupName(DateTime? startDateValue, DateTime? endDateValue,
                                                                string reportName, string outputType, string fileNameWithFolderPath)
        {
            var ca = new ControllerAccess();
            var list = ca.ControllerIisLog.GetIisLogListForGroup(startDateValue.Value.ToString(),
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

        public static void ProcessReportGenerateForUserName(DateTime? startDateValue, DateTime? endDateValue,
                                                                string reportName, string outputType, string fileNameWithFolderPath)
        {
            var ca = new ControllerAccess();
            var list = ca.ControllerIisLog.GetIisLogListForUser(startDateValue.Value.ToString(),
                                                                    endDateValue.Value.ToString(),
                                                                    reportName);

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

        private static void GenerateExcel(List<IisLog> list, string fileNameWithFolderPath)
        {
            var dt = ExtensionMethods.ToDataTable(list);
            if (dt != null)
            {
                //somehow the system adds thre columns like dt.Columns[21]	{ListAuditLog},dt.Columns[22]	{ListSessionLog},dt.Columns[20]	{ListIisLog} and then by removing the iis log all rest are remove
                dt.Columns.Remove("ListIisLog");
                dt.Columns.Remove("ListSessionLog");
                dt.Columns.Remove("ListAuditLog");
                dt.Columns.Remove("Description");

                var file = string.Empty;
                if (string.IsNullOrEmpty(fileNameWithFolderPath))
                {
                    //Save Back To File
                    file = GetFileDirectroy() + "Iis" + "_" + DateTime.Now.ToString("MMdd_hhmm") + ".xls";
                }
                else
                {
                    file = fileNameWithFolderPath;
                }

                var ds = new DataSet();
                //Add the table to the data set
                ds.Tables.Add(dt);

                var HtmlBody = ExtensionMethods.ExportDatatableToHtml(dt);
                File.WriteAllText(file, HtmlBody);
            }
        }

        private static void GenerateCSV(List<IisLog> list, string fileNameWithFolderPath)
        {
            var resultsList = new List<string>();

            //Date/Time,QVW,Action,User,Action Activity
            foreach (IisLog curData in list)
                resultsList.Add(
                                    (curData.UserAccessDatetime.HasValue ? curData.UserAccessDatetime.Value.ToString() : "NULL").ToString() + "," +
                                    (!string.IsNullOrEmpty(curData.EventType) ? curData.EventType.ToString() : "NULL").ToString() + "," +
                                    (!string.IsNullOrEmpty(curData.UserAgent) ? curData.UserAgent.ToString() : "NULL").ToString() + "," +
                                    (!string.IsNullOrEmpty(curData.User.UserName) ? curData.User.UserName.ToString() : "NULL").ToString() + "," +
                                    (!string.IsNullOrEmpty(curData.Group.GroupName) ? curData.Group.GroupName.ToString() : "NULL").ToString() + "," +
                                    (!string.IsNullOrEmpty(curData.QueryURI) ? curData.QueryURI.ToString() : "NULL").ToString()
                                );

            var file = string.Empty;
            if (string.IsNullOrEmpty(fileNameWithFolderPath))
            {
                //Save Back To File
                file = GetFileDirectroy() + "Iis" + "_" + DateTime.Now.ToString("MMdd_hhmm") + ".csv";
            }
            else
            {
                file = fileNameWithFolderPath;
            }

            //write file
            if (!File.Exists(file))
                File.WriteAllLines(file, resultsList.ToArray());

            //now add the header
            var path = file;
            string str;
            using (StreamReader sreader = new StreamReader(path))
            {
                str = sreader.ReadToEnd();
            }

            File.Delete(path);
            using (StreamWriter swriter = new StreamWriter(path, false))
            {
                str = "Date/Time,EventType,UserAgent,UserName,GroupName,QueryURI" + Environment.NewLine + str;
                swriter.Write(str);
            }
        }

        /// <summary>
        /// generate csv file for selected columns only
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fileNameWithFolderPath"></param>
        public static void GenerateCSVFileForGroupName(List<IisLog> list, string fileNameWithFolderPath)
        {
            var resultsList = new List<string>();

            //Date/Time,QVW,Action,User,Action Activity
            foreach (IisLog curData in list)
                resultsList.Add(
                                    (curData.UserAccessDatetime.HasValue ? curData.UserAccessDatetime.Value.ToString() : "NULL").ToString() + "," +
                                    (!string.IsNullOrEmpty(curData.EventType) ? curData.EventType.ToString() : "NULL").ToString() + "," +
                                    (!string.IsNullOrEmpty(curData.UserAgent) ? curData.UserAgent.ToString() : "NULL").ToString() + "," +
                                    (!string.IsNullOrEmpty(curData.User.UserName) ? curData.User.UserName.ToString() : "NULL").ToString() + "," +
                                    (!string.IsNullOrEmpty(curData.Group.GroupName) ? curData.Group.GroupName.ToString() : "NULL").ToString() + "," +
                                    (!string.IsNullOrEmpty(curData.QueryURI) ? curData.QueryURI.ToString() : "NULL").ToString()
                                );

            var file = string.Empty;
            if (string.IsNullOrEmpty(fileNameWithFolderPath))
            {
                //Save Back To File
                file = GetFileDirectroy() + "Iis" + "_" + DateTime.Now.ToString("MMdd_hhmm") + ".csv";
            }
            else
            {
                file = fileNameWithFolderPath;
            }

            //write file
            if (!File.Exists(file))
                File.WriteAllLines(file, resultsList.ToArray());

            //now add the header
            var path = file;
            string str;
            using (StreamReader sreader = new StreamReader(path))
            {
                str = sreader.ReadToEnd();
            }

            File.Delete(path);
            using (StreamWriter swriter = new StreamWriter(path, false))
            {
                str = "Date/Time,EventType,UserAgent,UserName,GroupName,QueryURI" + Environment.NewLine + str;
                swriter.Write(str);
            }
        }

        /// <summary>
        /// generate excel file for the full entity properties
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fileNameWithFolderPath"></param>
        /// <summary>
        /// generate excel file for the full entity properties
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fileNameWithFolderPath"></param>
        public static void GenerateExcelFileForGroupName(List<IisLog> list, string fileNameWithFolderPath)
        {
            GenerateExcel(list, fileNameWithFolderPath);
        }

        public static void GenerateTxtFileForGroupName(List<IisLog> list, string fileNameWithFolderPath)
        {
            throw new NotImplementedException("Program function is not implemented.");
        }



        #region Report By ReportName
        public static void GenerateCSVFileForReportName(List<IisLog> list, string fileNameWithFolderPath)
        {
            GenerateCSV(list, fileNameWithFolderPath);
        }

        public static void GenerateExcelFileForReportName(List<IisLog> list, string fileNameWithFolderPath)
        {
            GenerateExcel(list, fileNameWithFolderPath);
        }

        public static void GenerateTxtFileForReportName(List<IisLog> list, string fileNameWithFolderPath)
        {
            throw new NotImplementedException("Program function is not implemented.");
        }
        #endregion

        #region Report By UserName
        public static void GenerateCSVFileForUserName(List<IisLog> list, string fileNameWithFolderPath)
        {
            GenerateCSV(list, fileNameWithFolderPath);
        }

        public static void GenerateExcelFileForUserName(List<IisLog> list, string fileNameWithFolderPath)
        {
            GenerateExcel(list, fileNameWithFolderPath);
        }

        public static void GenerateTxtFileForUserName(List<IisLog> list, string fileNameWithFolderPath)
        {
            throw new NotImplementedException("Program function is not implemented.");
        }
        #endregion

        private static string GetFileDirectroy()
        {
            // For Example - D:\Projects\SomeProject\SomeFolder
            return (ConfigurationManager.AppSettings != null &&
                    ConfigurationManager.AppSettings["GenerateIisLogsReportDirectory"] != null) ?
                    ConfigurationManager.AppSettings["GenerateIisLogsReportDirectory"].ToString() :
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
