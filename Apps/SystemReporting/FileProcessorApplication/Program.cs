using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemReporting.Utilities;

namespace FileProcessorApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                DisplayInfo();
                //TO DO: For Testing Only
                //args = ProcessFileFromUserInput(args);
                if (CheckArgs(args))
                {
                    FileProcessor.ProcessFile.ExecuteProcessFile(args);
                }
                else
                {
                   var argsAll = new string[] { "Iis", "Audit", "Session" };
                    for (int i = 0; i < argsAll.Length; i++)
                    {
                        Console.WriteLine("Processing...... {0} ",argsAll[i] );
                        FileProcessor.ProcessFile.ExecuteProcessFile(new string[]{ argsAll[i] });
                    }
                }
                Environment.ExitCode = 0;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ProcessFile: Failed processing file. || " + args.ToArray());
            }
        }

        private static string[] ProcessFileFromUserInput(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("The system is not Auto sept up for process to execute. Please select the process you want to Execute and press Enter:");
                
                var listArgs = new List<string>();
                string choice;
                if (console_present)
                {
                    Console.WriteLine("1. Iis Log");
                    Console.WriteLine("2. Audit Log");
                    Console.WriteLine("3. Session Log");
                }
                choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": // Do Something
                        listArgs.Add("Iis");
                        break;
                    case "2": //Do that
                        listArgs.Add("Audit");
                        break;
                    case "3":
                        listArgs.Add("Session");
                        break;
                }

                Console.WriteLine("Please wait till process for {0} log file completes. ", choice.ToString());
                Console.WriteLine("----------------------------------------");
                args = listArgs.ToArray();
            }

            return args;
        }

        private static bool CheckArgs(string[] args)
        {
            var ReturnValue = false;
            if (args.Length > 0)
                ReturnValue = true;

            return ReturnValue;
        }

        // Property:
        public static bool? _console_present;
        public static bool console_present
        {
            get
            {
                if (_console_present == null)
                {
                    _console_present = true;
                    try { var window_height = Console.WindowHeight; }
                    catch (Exception ex)
                    { _console_present = false; }
                }
                return _console_present.Value;
            }
        }

        private static void DisplayInfo()
        {
            if (console_present)
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Title = "Reporting Application Milliman Inc.";

                // Get rid of the scroll bars by making the buffer the same size as the window
                Console.Clear();

                var left = Console.CursorLeft;
                var top = Console.CursorTop;
                Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
                Console.SetCursorPosition(left, top);

                Console.Write(DateTime.Now + Environment.NewLine);
                Console.WriteLine("----------------------------------------");
                Console.Write("Reporting Application in Progress." + Environment.NewLine);
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("Attention: Reporting Application Running");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("----------------------------------------");
            }
        }

        private static void DisplayUsage()
        {
            if (console_present)
            {
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("Pass in Report name as command line arguments ");
                Console.WriteLine("for usage to process specific data.");
                Console.WriteLine(String.Empty);
                Console.WriteLine("Ex: FileProcessorApplication.exe Iis ");
                Console.WriteLine("--------------------------------------------------");
            }
        }
    }
}
