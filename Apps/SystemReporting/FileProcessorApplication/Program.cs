using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessorApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                DisplayInfo();
                if (args.Length == 0)
                {
                    //args = "Audit";
                    //args = "Session";
                    string[] ar1 = new string[] { "Iis" };
                    args = ar1;
                }                
                if (CheckArgs(args))
                {

                    FileProcessor.ProcessFile.ExecuteProcessFile(args);
                }
                else
                {
                    DisplayUsage();
                }
                Environment.ExitCode = 0;

            }
            catch (Exception ex)
            {
                FileProcessor.BaseFileProcessor.LogError(ex, "ProcessFile: Failed processing file. || " + args.ToArray());
            }
        }

        private static bool CheckArgs(string[] args)
        {
            bool ReturnValue = false;
            if (args.Length > 0)
                ReturnValue = true;

            return ReturnValue;
        }

        private static void DisplayInfo()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = "Reporting Application Milliman Inc.";
            
            // Get rid of the scroll bars by making the buffer the same size as the window
            Console.Clear();
            Console.SetWindowSize(65, 20);
            Console.BufferWidth = 65;
            Console.BufferHeight = 20;

            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
            Console.SetCursorPosition(left, top);

            Console.Write(DateTime.Now + Environment.NewLine);
            Console.WriteLine("----------------------------------------");
            Console.Write("Reporting Application in Progress." + Environment.NewLine);
            Console.Write("Press any key to continue" + Environment.NewLine);
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Attention: Reporting Application Running");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("----------------------------------------");

            //// Restore previous position
            //Console.SetCursorPosition(left, top);
        }

        private static void DisplayUsage()
        {
            Console.WriteLine("Pass in Report name as command line arguments ");
            Console.WriteLine("for usage to process specific data.");
            Console.WriteLine(String.Empty);
            Console.WriteLine("Ex: FileProcessorApplication.exe Iis ");
            Console.WriteLine("--------------------------------------------------");
        }
    }
}
