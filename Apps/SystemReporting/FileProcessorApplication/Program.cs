using FileProcessor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using SystemReporting.Utilities.ExceptionHandling;
using C = SystemReporting.Utilities.Constants;

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
                    ProcessFile.ExecuteProcessFile(args);
                }
                else
                {
                    var argsProcessAll = new string[] { "Iis", "Audit", "Session" };
                    for (int i = 0; i < argsProcessAll.Length; i++)
                    {
                        ProcessFile.ExecuteProcessFile(new string[] { argsProcessAll[i] });
                    }
                }
                if (C.ERROR_LOGGED)
                {
                    ExceptionLogger.SendErrorEmail(GetExceptionDirectory(),"System Reporting Exceptions");
                }               
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogError(ex, "ProcessFile: Failed processing file. || " + args.ToArray(), "Program File Processor Application");
            }
        }

        private static string[] ProcessFileFromUserInput(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Select the process you want to Execute and press Enter:");

                var listArgs = new List<string>();
                string choice;
                if (console_present)
                {
                    Console.WriteLine("1. Iis Log");
                    Console.WriteLine("2. Audit Log");
                    Console.WriteLine("3. Session Log");
                    Console.WriteLine("4. File Path");
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
                    case "4":
                        string file;
                        Console.WriteLine("Enter file path and name.");
                        file = Console.ReadLine();
                        listArgs.Add(file);
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
                    catch (Exception)
                    { _console_present = false; }
                }
                return _console_present.Value;
            }
        }

        private static void DisplayInfo()
        {
            if (console_present)
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Title = "Reporting Application";

                // Get rid of the scroll bars by making the buffer the same size as the window
                Console.Clear();

                var left = Console.CursorLeft;
                var top = Console.CursorTop;
                Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
                Console.SetCursorPosition(left, top);

                Console.Write(DateTime.Now + Environment.NewLine);
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("     Reporting Application in Progress      ");
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("--------------------------------------------");
            }
        }

        private static void DisplayUsage()
        {
            if (console_present)
            {
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("Processing Instruction. Choose following metods... ");
                Console.WriteLine("Application and file type");
                Console.WriteLine("Application only ");
                Console.WriteLine("Application and fully qualified file path ");
                Console.WriteLine(String.Empty);
                Console.WriteLine("Ex: FileProcessorApplication.exe  Iis ");
                Console.WriteLine("Ex: FileProcessorApplication.exe");
                Console.WriteLine("Ex: FileProcessorApplication.exe   C:\\ProductionLogs\\IISLogs\\u_ex151002.log  ");
                Console.WriteLine("--------------------------------------------------");
            }
        }

        private static string GetExceptionDirectory()
        {
            // For Example - D:\Projects\SomeProject\SomeFolder
            return (ConfigurationManager.AppSettings != null &&
                    ConfigurationManager.AppSettings["ExceptionFileDirectory"] != null) ?
                    ConfigurationManager.AppSettings["ExceptionFileDirectory"].ToString() :
                    string.Empty;
        }
    }
}
