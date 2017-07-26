using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanCommon
{
    public static class Report
    {
        private static string LogFile = @"Logs\Report.txt";

        public enum ReportType { Debug, Info, Warning, Error, GiveUp };
        public static void Log(ReportType _ReportType, string Msg, Exception ex = null)
        {
            try
            {
                string _Msg = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").PadRight(25) + _ReportType.ToString().PadRight(10) + Msg;
                if (ex != null)
                    _Msg += "\r\nException trace: " + ex.ToString();
            }
            catch (Exception)  //eat exceptions
            {
            }
        }
    }
}
