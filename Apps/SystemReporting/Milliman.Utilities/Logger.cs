using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemReporting.Utilities
{
    public class Logger
    {
        public static Logger Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_SyncRoot)
                    {
                        if (_Instance == null)
                            _Instance = new Logger();
                    }
                }

                return _Instance;
            }
        }
        private static volatile Logger _Instance;
        private static object _SyncRoot = new Object();

        public StreamWriter Writer { get; set; }
        public string LogPath { get; set; }
        public string LogFileName { get; set; }
        public string LogFileExtension { get; set; }
        public string LogFile { get { return LogFileName + LogFileExtension; } }
        public string LogFullPath { get { return Path.Combine(LogPath, LogFile); } }
        public bool LogExists { get { return System.IO.File.Exists(LogFullPath); } }
                
        private Logger()
        {
            //LogFileName = DateTime.Now + "_" + LoggerFileName;
            LogFileExtension = ".log";
            LogPath = ConfigurationManager.AppSettings["LoggerFileDirectory"];
            LogFileName = ConfigurationManager.AppSettings["LoggerFileName"];
            
            //Logger.WriteLine("Logs started" + " | " + DateTime.Now);
        }
                
        public void WriteLineToLog(String inLogMessage)
        {
            WriteToLog(inLogMessage + Environment.NewLine);
        }

        public void WriteToLog(String inLogMessage)
        {
            if (!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);

            if (Writer == null)
                Writer = new StreamWriter(LogFullPath, true);

            Writer.Write(inLogMessage);
            Writer.Flush();
        }

        public static void WriteLine(String inLogMessage)
        {
            Instance.WriteLineToLog(inLogMessage);
        }

        public static void Write(String inLogMessage)
        {
            Instance.WriteToLog(inLogMessage);
        }

        #region Error Log
        public static void LogError(Exception ex, string message)
        {
            if (ex != null && !string.IsNullOrEmpty(message))
            {
                LogExAndErr(ex, message);
            }
            else if (ex == null & (!string.IsNullOrEmpty(message)))
            {
                LogError(message);
            }
            else if (ex != null & (string.IsNullOrEmpty(message)))
            {
                LogError(ex);
            }
        }

        public static void LogError(Exception ex)
        {
            Console.WriteLine("Exception Occurred. Check the Exception Log. Message : ", ex.Message.ToString());
            //Logger.Instance.LogPath = FileFunctions.GetLoggerFileDirectory();
            //Logger.Instance.LogFileName = FileFunctions.GetLoggerFileName();
            Logger.WriteLine(DateTime.Now + " Todays Exceptions: ~ " + "Exception Message: " + ex.Message.ToString() + "||-||"
                                          + "Exception Trace : " + ex.StackTrace + "||-||"
                                          + "Exception Target: " + ex.TargetSite.ToString() + "||-||"
                                          + "Exception Source: " + ex.Source.ToString()
                                          + Environment.NewLine);
        }
        public static void LogError(string message)
        {
            Console.WriteLine("Exception Occurred. Check the Exception Log. Message : ", message);
            Logger.WriteLine(DateTime.Now + " Todays Exceptions: ~ " + "||-||"
                                          + " Exception Message: " + message
                                          + Environment.NewLine);
        }
        public static void LogExAndErr(Exception ex, string message)
        {
            Console.WriteLine("Exception Occurred. Check the Exception Log. Message : ", message);
            Logger.WriteLine(DateTime.Now + " Todays Exceptions: ~ " + "||-||"
                                          + message + "||-||"
                                          + "Exception Message: " + ex.Message.ToString() + "||-||"
                                          + "Exception Trace : " + ex.StackTrace + "||-||"
                                          + "Exception Target: " + ex.TargetSite.ToString() + "||-||"
                                          + "Exception Source: " + ex.Source.ToString()
                                          + Environment.NewLine);
        }
        #endregion
    }
}
