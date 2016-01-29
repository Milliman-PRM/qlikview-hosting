using SystemReporting.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor
{
    public class ProcessFile:BaseFileProcessor
    {
        public static void ExecuteProcessFile(string[] args)
        {
            try
            {
                string arg = args[0].ToString();
                if (!string.IsNullOrEmpty(arg))
                    ProcessLogfiles(arg);                
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, "ProcessFile: Failed processing file. || " + args.ToArray());
            }
        }
        private static void ProcessLogfiles(string args)
        {
            /*
             *  iIs log: u_*.log 
             *  Qlick view log: Audit_*.log
             *  Qlick view Session: Sessions_*.log
             */
            IFileProcessor proc = null;

            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Processing data for file: {0}", args);
            Console.WriteLine("----------------------------------------");

            //this can be empty
            string fullFilePath = string.Empty;
            switch (args)
            {
                case "Iis":
                    proc = new ProcessIisLogs(args);
                    proc.ProcessFileData(args);
                    break;
                case "Audit":
                    proc = new ProcessQVAuditLogs(args);
                    proc.ProcessFileData(args);
                    break;
                case "Session":
                    proc = new ProcessQVSessionLogs(args);
                    proc.ProcessFileData(args);
                    break;
                default:
                    Console.WriteLine("File type not supported.");
                    break;
            }
        }
    }
}
