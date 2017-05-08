/*
 * CODE OWNERS: Tom Puckett, Ben Wyatt
 * OBJECTIVE: A console application to be launched from a system service such as Zabbix agent
 * DEVELOPER NOTES: Intentions for the console application include no log file and numeric output to a specific request identified through a command line keyword
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using PrmServerMonitorLib;

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
                            OrphanQlikTaskRemover Worker = new OrphanQlikTaskRemover("localhost");
                            Worker.RemoveOrphanTasks(false);
                        }
                        break;

                    case "managecals":
                        {
                            QlikviewCalManager Worker = new QlikviewCalManager("localhost");
                            Worker.EnumerateAllCals(true);
                        }
                        break;

                    case "reportnamedcalassigned":
                        {
                            QlikviewCalManager Worker = new QlikviewCalManager("localhost");
                            Worker.ReportCalStatistic(QlikviewCalManager.CalStatisticEnum.NamedCalAssigned, false);
                        }
                        break;

                    case "reportnamedcallimit":
                        {
                            QlikviewCalManager Worker = new QlikviewCalManager("localhost");
                            Worker.ReportCalStatistic(QlikviewCalManager.CalStatisticEnum.NamedCalLimit, false);
                        }
                        break;

                    case "reportdocumentcalassigned":
                        {
                            QlikviewCalManager Worker = new QlikviewCalManager("localhost");
                            Worker.ReportCalStatistic(QlikviewCalManager.CalStatisticEnum.DocumentCalAssigned, false);
                        }
                        break;

                    case "reportdocumentcallimit":
                        {
                            QlikviewCalManager Worker = new QlikviewCalManager("localhost");
                            Worker.ReportCalStatistic(QlikviewCalManager.CalStatisticEnum.DocumentCalLimit, false);
                        }
                        break;

                    case "deleteoldestdoccals":
                        {
                            int SelectLimit;
                            int MinimumAgeToDelete;
                            bool AllowDeleteOfUndatedDocCals;

                            #region Read configured values
                            int.TryParse(ConfigurationManager.AppSettings["MaxDocumentCALsToDelete"], out SelectLimit);  // zero if not configured

                            int.TryParse(ConfigurationManager.AppSettings["MinimumDocCalAgeHoursToDelete"], out MinimumAgeToDelete);  // zero if not configured
                            MinimumAgeToDelete = Math.Max(MinimumAgeToDelete, 48);  // never go lower than this minimum value

                            bool.TryParse(ConfigurationManager.AppSettings["AllowDeleteOfUndatedDocCals"], out AllowDeleteOfUndatedDocCals);  // false if not configured
                            #endregion

                            QlikviewCalManager Worker = new QlikviewCalManager("localhost");
                            List<DocCalEntry> AllDocCals = Worker.EnumerateDocumentCals(false, SelectLimit, AllowDeleteOfUndatedDocCals, MinimumAgeToDelete);

                            Trace.WriteLine(string.Format("Based on configured threshold, {0} document CALs should be deleted", AllDocCals.FindAll(e => e.DeleteFlag).Count));

                            foreach (DocCalEntry Entry in AllDocCals.FindAll(e => e.DeleteFlag))
                            {
                                Worker.RemoveOneDocumentCal(Entry.UserName, Entry.RelativePath, Entry.DocumentName, false);
                            }
                        }
                        break;

                    case "deleteoldestnamedcals":
                        {
                            int MaxNamedCALsToDelete;
                            int MinimumNamedCalAgeHoursToDelete;
                            bool AllowDeleteOfUndatedNamedCals;

                            #region Read configured values
                            int.TryParse(ConfigurationManager.AppSettings["MaxNamedCALsToDelete"], out MaxNamedCALsToDelete);  // zero if not configured

                            int.TryParse(ConfigurationManager.AppSettings["MinimumNamedCalAgeHoursToDelete"], out MinimumNamedCalAgeHoursToDelete);  // zero if not configured
                            MinimumNamedCalAgeHoursToDelete = Math.Max(MinimumNamedCalAgeHoursToDelete, 48);  // never go lower than this minimum value

                            bool.TryParse(ConfigurationManager.AppSettings["AllowDeleteOfUndatedNamedCals"], out AllowDeleteOfUndatedNamedCals);  // false if not configured
                            #endregion

                            QlikviewCalManager Worker = new QlikviewCalManager("localhost");
                            List<NamedCalEntry> AllNamedCals = Worker.EnumerateNamedCals(MaxNamedCALsToDelete, AllowDeleteOfUndatedNamedCals, MinimumNamedCalAgeHoursToDelete, false);

                            Trace.WriteLine(string.Format("Based on configured threshold, {0} named CALs should be deleted", AllNamedCals.FindAll(e => e.DeleteFlag).Count));

                            foreach (NamedCalEntry Entry in AllNamedCals.FindAll(e => e.DeleteFlag))
                            {
                                Worker.RemoveOneNamedCal(Entry.UserName, false);
                            }
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