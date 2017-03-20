using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace SystemBackup
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            BackupProcessor BP = new BackupProcessor();
            DateTime Start = DateTime.Now;
            Trace.AutoFlush = true;
            TextWriterTraceListener Listener = new TextWriterTraceListener("log_" + Start.ToString("yyyyMMdd-HHmmss") + ".txt");
            Trace.Listeners.Add(Listener);

            if (BP.CreateBackupDir())
            {
                if (BP.DatabaseBackup())
                {
                    if (BP.BackupSystem())
                    {
                        System.TimeSpan DTO = DateTime.Now - Start;
                        BP.SendEmail(BP.MasterZipName + " was successfully archived.(" + ((int)DTO.TotalMinutes).ToString() + "min)");
                        BP.Cleaner();
                    }
                    else
                    {
                        BP.SendEmail("Backup of PRM files FAILED!", System.Net.Mail.MailPriority.High);
                        //error failed to backup
                    }
                }
                else
                {
                    BP.SendEmail("Backup of PRM database FAILED!", System.Net.Mail.MailPriority.High);
                }
            }
            else
            {
                BP.SendEmail("Backup FAILED to due to access issue of backup assembly area!", System.Net.Mail.MailPriority.High);
            }

            Trace.Listeners.Remove(Listener);
        }
    }
}
