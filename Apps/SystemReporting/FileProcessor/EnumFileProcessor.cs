using System.ComponentModel;

namespace FileProcessor
{
    public class EnumFileProcessor
    {
        #region "   OutputType"
        public enum eFileOutputType
        {
            ExcelFile,
            CsvFile,
            TextFile
        }
        #endregion

        #region "   FilePath"
        public enum eFilePath
        {
            IisLogs,
            QVAuditLogs,
            QVSessionLogs
        }
        #endregion

        #region "   FileType"
        public enum eFileType
        {
            IisLogs,
            QVAuditLogs,
            QVSessionLogs
        }
        #endregion

        #region BrowserType
        public enum eBrowserType
        {
            gecko,
            msie,
            android,
            safari,
            firefox,
            chrome,
            android_mobile,
            safari_mobile
        }
        #endregion


    }
}
