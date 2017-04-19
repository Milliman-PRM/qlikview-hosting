using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrmServerMonitor
{
    static class Program
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length == 0)
            { // Run as GUI
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                return 0;
            }
            else if (args.Any(a => a.ToLower() == "console"))
            { // run background process (invoke with "start /wait <exename> [args]" because the OS does not wait for gui applications to return)
                AttachConsole(-1);

                TraceListener ConsoleListener = new ConsoleTraceListener();
                Trace.Listeners.Clear();
                Trace.Listeners.Add(ConsoleListener);

                foreach (string Arg in args)
                {
                    switch (Arg.ToLower())
                    {
                        case "orphantaskremoval":
                            {
                                OrphanQlikTaskRemover Worker = new OrphanQlikTaskRemover();
                                Worker.RemoveOrphanTasks();
                            }
                            break;

                        case "managecals":
                            {
                                QlikviewCalManager Worker = new QlikviewCalManager();
                                Worker.EnumerateAllCals(true);
                            }
                            break;

                        case "reportnamedcalassigned":
                            {
                                QlikviewCalManager Worker = new QlikviewCalManager();
                                Worker.ReportCalStatistic(QlikviewCalManager.CalStatisticField.NamedCalAssigned);
                            }
                            break;

                        case "reportnamedcallimit":
                            {
                                QlikviewCalManager Worker = new QlikviewCalManager();
                                Worker.ReportCalStatistic(QlikviewCalManager.CalStatisticField.NamedCalLimit);
                            }
                            break;

                        case "reportdocumentcalassigned":
                            {
                                QlikviewCalManager Worker = new QlikviewCalManager();
                                Worker.ReportCalStatistic(QlikviewCalManager.CalStatisticField.DocumentCalAssigned);
                            }
                            break;

                        case "reportdocumentcallimit":
                            {
                                QlikviewCalManager Worker = new QlikviewCalManager();
                                Worker.ReportCalStatistic(QlikviewCalManager.CalStatisticField.DocumentCalLimit);
                            }
                            break;

                        // NOTE: Only include cases for finite operations.  No ongoing monitoring activities
                        case "somethingelse":
                            {
                            }
                            break;

                        default:
                            break;
                    }
                }

                Trace.Listeners.Remove(ConsoleListener);
                ConsoleListener = null;
                FreeConsole();

                return 0;
            }
            else  // nothing to be done
            {
                return 2;
            }

        }
    }
}
