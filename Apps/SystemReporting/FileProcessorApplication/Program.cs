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
                if (args.Length == 0)
                {
                    //args = "Audit";
                    //args = "Session";
                    string[] ar1 = new string[] { "Iis" };
                    args = ar1;
                }
                DisplayInfo();
                if (CheckArgs(args))
                {
                    Console.WriteLine("Processing data for file: {0}", args);
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
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Attention: Test Application Running");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("--------------------------------------");
        }

        private static void DisplayUsage()
        {
            Console.WriteLine("Pass in Report name and report type pairs as command line arguments ");
            Console.WriteLine("for usage to process specific data.");
            Console.WriteLine(String.Empty);
            Console.WriteLine("Ex: FileProcessorApplication.exe Iis fileName");
            Console.WriteLine("--------------------------------------------------");
        }
    }
}
