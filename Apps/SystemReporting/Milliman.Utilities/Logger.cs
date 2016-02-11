using System;
using System.Configuration;
using System.IO;
using SystemReporting.Utilities.Email;

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
            //Logger.WriteLine("Logs started: " + " || " + DateTime.Now);
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
    }
}
