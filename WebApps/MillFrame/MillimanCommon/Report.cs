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
                string _Msg = System.DateTime.Now.ToLongTimeString().PadRight(30) + _ReportType.ToString().PadRight(10) + Msg + @"\r\n";
                if (ex != null)
                    _Msg += "Exception trace: " + ex.ToString();

                System.IO.File.AppendAllText(System.Web.HttpContext.Current.Server.MapPath(LogFile), _Msg);
                string Delimit = string.Empty;
                System.IO.File.AppendAllText(System.Web.HttpContext.Current.Server.MapPath(LogFile), Delimit.PadRight(40, '-'));
            }
            catch (Exception)  //eat exceptions
            {
            }
        }
    }
}
