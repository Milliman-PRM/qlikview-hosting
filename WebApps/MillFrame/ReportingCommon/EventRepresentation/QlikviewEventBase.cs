using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportingCommon
{
    public class QlikviewEventBase : UserEventBase
    {
        public enum QvSessionEndReason
        {
            SESSIONTIMEOUT,
            CLIENTCLOSE,
            OTHER,
            UNKNOWN
        }

        /// <summary>
        ///     Gets the user's group name from the full Document path
        /// </summary>
        /// <param name="Document">The raw document string as read from the log file</param>
        /// <param name="QvDocumentRootPath">The root path for QVW files on the server</param>
        /// <returns>group name</returns>
        public string GetGroup(string Document, string QvDocumentRootPath)
        {
            string ReturnValue = "Unknown Group";

            string directoryPath = Path.GetDirectoryName(Document);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                ReturnValue = directoryPath.ToUpper().IndexOf(QvDocumentRootPath.ToUpper()) >= 0 ? 
                    directoryPath.Substring(QvDocumentRootPath.Length) :
                    directoryPath;
                ReturnValue = ReturnValue.TrimStart(new char[] { '\\' });
                ReturnValue = ReturnValue.ToUpper();
                ReturnValue = ReturnValue.Replace(@"\", "_");
                ReturnValue = ReturnValue.Replace(@"_REDUCEDCACHEDQVWS", "");
            }

            return ReturnValue;
        }

        /// <summary>
        ///     Gets the report name from the full document path
        /// </summary>
        /// <param name="Document">The document string read from the Qlikview log file</param>
        /// <returns>group name</returns>
        public string GetReportName(string Document)
        {
            string reportName = Path.GetFileNameWithoutExtension(Document);
            if (!String.IsNullOrEmpty(reportName))
            {
                return reportName.ToUpper();
            }
            else
            {
                return "NO REPORT";
            }
        }

    }
}
