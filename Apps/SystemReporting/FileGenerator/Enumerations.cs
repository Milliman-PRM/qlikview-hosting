using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportFileGenerator
{
    public class Enumerations
    {
        #region "   ReportType"
        public enum eReportType
        {
            Group,
            User,
            Report
        }
        #endregion

        #region "   LogType"
        public enum eLogType
        {
            iIS,
            Audit,
            Session
        }
        #endregion

        #region "   OutputType"
        public enum eOutputType
        {
            EXCEL,
            CSV,
            TEXT
        }
        #endregion

    }
}
