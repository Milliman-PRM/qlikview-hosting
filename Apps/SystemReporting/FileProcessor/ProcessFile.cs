using SystemReporting.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace FileProcessor
{
    public class ProcessFile : BaseFileProcessor
    {
        public static void ExecuteProcessFile(string[] args)
        {
            try
            {
                ProcessLogfiles(args);
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, "ProcessFile: Failed processing file. || " + args.ToArray());
            }
        }
        private static void ProcessLogfiles(string[] args)
        {
            /*
             *  iIs log: u_*.log 
             *  Qlick view log: Audit_*.log
             *  Qlick view Session: Sessions_*.log
             *  C:\ProductionLogs\IISLogs\*.log
             */
            IFileProcessor proc = null;

            var newArgs = new string[2];
            if ((args != null) && (args.Length != 0) && args[0]!= "")
            {
                //check we its a file path or the file type. if file path then find out the file type
                var isHistroy = args.Any(x => x.IndexOf("ProductionLogs", StringComparison.Ordinal) > -1);
                if (isHistroy)
                {
                    var possiblePath = args[0].ToString().IndexOfAny(Path.GetInvalidPathChars()) == -1;
                    {
                        var filename = Path.GetFileName(args[0].ToString()).ToLower();

                        if (filename.IndexOf("u_ex", StringComparison.Ordinal) > -1)
                        {
                            newArgs[0] = args[0];
                            newArgs[1] = "Iis";
                        }
                        else if (filename.IndexOf("audit_", StringComparison.Ordinal) > -1)
                        {
                            newArgs[0] = args[0];
                            newArgs[1] = "Audit";
                        }
                        else if (filename.IndexOf("sessions_", StringComparison.Ordinal) > -1)
                        {
                            newArgs[0] = args[0];
                            newArgs[1] = "Session";
                        }                        
                    }
                    args = newArgs;
                }
                else
                {
                    //if we only process by file type then it will be one array element, so add one more to make it two elements
                    newArgs[0] = args[0];
                    newArgs[1] = args[0];
                    args = newArgs;
                }
            }

            for (int i = 0; i <= args.Length - 1; i += 2)
            {
                var arg1 = args[i].ToLower();
                var arg2 = args[i + 1].ToLower();
                switch (arg2.ToString())
                {
                    case "iis":
                        {
                            proc = new ProcessIisLogs(arg1);
                            proc.ProcessLogFileData(arg1);
                            break;
                        }
                    case "audit":
                        {
                            proc = new ProcessQVAuditLogs(arg1);
                            proc.ProcessLogFileData(arg1);
                            break;
                        }
                    case "session":
                        {
                            proc = new ProcessQVSessionLogs(arg1);
                            proc.ProcessLogFileData(arg1);
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("File type not supported.");
                            break;
                        }
                }
            }


        }
    }
}
