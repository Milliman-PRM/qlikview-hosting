using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MillimanSignature
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;


        enum CommandType { Get, Update, Delete, Help };

        static private string GetArgs(string[] Args, int StartWithIndex)
        {
            string Result = "";
            for (int Index = StartWithIndex; Index < Args.Length; Index++)
            {
                if (string.IsNullOrEmpty(Result) == false)
                    Result += " ";
                Result += Args[Index];
            }
            return Result;
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            bool CommandLineRun = false;
            string File = "";
            string Key = "";
            string Value = "";
            CommandType Action = CommandType.Help;


            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (string.Compare(args[i], "-file", true) == 0)
                    {
                        File = args[i + 1];
                        CommandLineRun = true;
                    }
                    else if (string.Compare(args[i], "-get", true) == 0)
                    {
                        Action = CommandType.Get;
                        Key = GetArgs(args, i + 1);
                        CommandLineRun = true;
                        break;
                    }
                    else if (string.Compare(args[i], "-update", true) == 0)
                    {
                        Action = CommandType.Update;
                        Key = args[i + 1];
                        Value = GetArgs(args, i + 2);
                        CommandLineRun = true;
                        break;
                    }
                    else if (string.Compare(args[i], "-delete", true) == 0)
                    {
                        Action = CommandType.Delete;
                        Key = GetArgs(args, i + 1);
                        CommandLineRun = true;
                        break;
                    }
                    else if (string.Compare(args[i], "-help", true) == 0)
                    {
                        Action = CommandType.Help;
                        CommandLineRun = true;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                Action = CommandType.Help;
                CommandLineRun = true;
            }

            if (CommandLineRun == false)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SignView());
            }
            else
            {
                // redirect console output to parent process;
                // must be before any calls to Console.WriteLine()
                AttachConsole(ATTACH_PARENT_PROCESS);

                if (Action == CommandType.Get)
                {
                    try
                    {
                        if (System.IO.File.Exists(File) == true)
                        {
                            MillimanCommon.XMLFileSignature Signature = new MillimanCommon.XMLFileSignature(File);
                            if (Signature.SignatureDictionary.ContainsKey(Key) == true)
                            {
                                Console.WriteLine(Signature.SignatureDictionary[Key]);
                                Environment.Exit(0);
                            }
                            else
                            {
                                Console.WriteLine("[KEY NOT FOUND]");
                                Environment.Exit(1);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("[UNSPECIFIED EXCEPTION]");
                        Environment.Exit(2);
                    }
                }
                else if (Action == CommandType.Update)
                {
                    try
                    {
                        if (System.IO.File.Exists(File) == true)
                        {
                            MillimanCommon.XMLFileSignature Signature = new MillimanCommon.XMLFileSignature(File);
                            if (Signature.SignatureDictionary.ContainsKey(Key) == true)
                            {
                                Signature.SignatureDictionary[Key] = Value;
                            }
                            else
                            {
                                Signature.SignatureDictionary.Add(Key, Value);
                            }
                            Signature.SaveChanges();
                            Console.WriteLine("[SUCCESS]");
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("[UNSPECIFIED EXCEPTION]");
                        Environment.Exit(4);
                    }
                }
                else if (Action == CommandType.Delete)
                {
                    try
                    {
                        if (System.IO.File.Exists(File) == true)
                        {
                            MillimanCommon.XMLFileSignature Signature = new MillimanCommon.XMLFileSignature(File);
                            if (Signature.SignatureDictionary.ContainsKey(Key) == true)
                            {
                                Signature.SignatureDictionary.Remove(Key);
                            }
                            Signature.SaveChanges();
                            Console.WriteLine("[SUCCESS]");
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("[UNSPECIFIED EXCEPTION]");
                        Environment.Exit(5);
                    }
                }
                else if (Action == CommandType.Help)
                {
                    Console.WriteLine("MillimanSignature.exe -file QualifiedPathFilename -get Key");
                    Console.WriteLine("MillimanSignature.exe -file QualifiedPathFilename -update Key Value");
                    Console.WriteLine("MillimanSignature.exe -file QualifiedPathFilename -delete Key");
                    Console.WriteLine("MillimanSignature.exe -file QualifiedPathFilename -help");
                    Environment.Exit(0);
                }
            }
        }
    }
}
