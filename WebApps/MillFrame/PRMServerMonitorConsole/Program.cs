/*
 * CODE OWNERS: Tom Puckett, Ben Wyatt
 * OBJECTIVE: A console application to be launched from a system service such as Zabbix agent
 * DEVELOPER NOTES: Intentions for the console application include no log file and numeric output to a specific request identified through a command line keyword
 */

using System.Diagnostics;
using PrmServerMonitor;

namespace PRMServerMonitorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            TraceListener ConsoleListener = new ConsoleTraceListener();
            Trace.Listeners.Clear();
            Trace.Listeners.Add(ConsoleListener);

            // Handle each command line argument as appropriate
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
        }
    }
}