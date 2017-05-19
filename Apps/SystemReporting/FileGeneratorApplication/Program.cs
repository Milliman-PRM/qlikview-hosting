using ReportFileGenerator;
using System;
using System.Configuration;
using SystemReporting.Utilities.ExceptionHandling;
using C = SystemReporting.Utilities.Constants;

namespace ReportFileGeneratorApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                DisplayInfo();
                //Lets only process//
                var process = new string[] { "Process" };
                args = process;
                if (CheckArgs(args))
                {
                    GenerateReportFile.ExecuteReportFileGenerate(args[0]);
                }
                if (C.ERROR_LOGGED)
                {
                    ExceptionLogger.SendErrorEmail(GetExceptionDirectory(), "Report File Generator Application");
                }
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogError(ex, "Exception Raised in Method Main.", "Program Exceptions");
            }
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
