using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
