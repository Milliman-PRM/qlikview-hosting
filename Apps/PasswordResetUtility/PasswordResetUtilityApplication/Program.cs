using PasswordUtilityProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C = PasswordUtilityProcessor.Constants;

namespace PasswordResetUtilityApplication
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
                    PasswordProcessor.ExecutePasswordResetUtility(args[0]);
                }
                SendErrorEmail();
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                BaseFileProcessor.LogError(ex, "Main || Failed processing. || " + args);
            }
        }

        #region Notification
        private static void SendErrorEmail()
        {
            //if error logged filter is true then send email
            if (C.ERROR_LOGGED)
                //send email
                BaseFileProcessor.SendEmail("Password re-set Utility could not process user(s) information due to missing or lack of information in database. ", "Missing User Information");
            C.ERROR_LOGGED = false;
        }
        #endregion

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
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Title = "Password Reset Utility";

                // Get rid of the scroll bars by making the buffer the same size as the window
                Console.Clear();

                var left = Console.CursorLeft;
                var top = Console.CursorTop;
                Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
                Console.SetCursorPosition(left, top);

                Console.Write(DateTime.Now + Environment.NewLine);
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("     Password Reset Utility in Progress     ");
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
                Console.WriteLine("Application and userEmail");
                Console.WriteLine("Application only ");
                Console.WriteLine(String.Empty);
                Console.WriteLine("Ex: PasswordResetUtilityApplication.exe  'abs@somthing.com' ");
                Console.WriteLine("Ex: PasswordResetUtilityApplication.exe");
                Console.WriteLine("--------------------------------------------------");
            }
        }
    }
}
