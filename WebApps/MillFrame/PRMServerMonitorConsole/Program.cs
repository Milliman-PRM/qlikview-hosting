using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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