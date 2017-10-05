using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Net.Mail;

namespace SystemBackup
{
    public class BackupProcessor
    {
        //  string Test = ConfigurationManager.AppSettings["SQLCMD"];

        private string BackUpDirectory { get; set; }
        public string MasterZipName { get; set; }

        private string _TempDir;

        public string TempDir
        {
            get
            {
                if (string.IsNullOrEmpty(_TempDir))
                {                     
                    _TempDir = ConfigurationManager.AppSettings["TemporaryBackupCache"];
                    System.IO.Directory.CreateDirectory(_TempDir);
                }
                return _TempDir;
            }
            set { _TempDir = value; }
        }

        public bool CreateBackupDir()
        {
            string tempDirectory = Path.Combine(TempDir, System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            if (System.IO.Directory.Exists(tempDirectory))
            {
                BackUpDirectory = tempDirectory;
                return true;
            }
            return false;
        }

        public bool DatabaseBackup()
        {
            //create the script
            string BackupTemplate = @"BACKUP DATABASE _DATABASENAME_ TO  DISK = N'_BACKUPNAME_' WITH NOFORMAT, INIT,  NAME = N'_DATABASENAME_ Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";
            BackupTemplate = BackupTemplate.Replace("_DATABASENAME_", ConfigurationManager.AppSettings["DatabaseName"]);
            string BackupName = DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Year.ToString() + "-" + ConfigurationManager.AppSettings["DatabaseName"] + ".bak";
            string SQLBackup = System.IO.Path.Combine(BackUpDirectory, BackupName);
            BackupTemplate = BackupTemplate.Replace("_BACKUPNAME_", SQLBackup);
            string ScriptFile = Path.Combine(TempDir, System.Guid.NewGuid().ToString("N")) + ".sql";
            System.IO.File.WriteAllText(ScriptFile, BackupTemplate);

            Trace.WriteLine("Database backing up initially to file: " + SQLBackup);
            //create the backup process launcher
            string SQLLauncherArgs = @"-S (local)\SQLExpress -i _SCRIPTFILE_";
            SQLLauncherArgs = SQLLauncherArgs.Replace("_SCRIPTFILE_", ScriptFile);
            string CMDSQL = ConfigurationManager.AppSettings["SQLCMD"];

            try
            {
                System.Diagnostics.Process Proc = new System.Diagnostics.Process();
                Proc.StartInfo.Arguments = SQLLauncherArgs;
                Proc.StartInfo.CreateNoWindow = true;
                Proc.StartInfo.FileName = CMDSQL;
                Proc.StartInfo.UseShellExecute = false;
                Proc.Start();
                Proc.WaitForExit(1000 * 60 * 30); //wait max 30 mins
                int ExitCode = Proc.ExitCode;
                Proc.Close();
                System.IO.File.Delete(ScriptFile);  //cleanup, get rid of script file

                if (System.IO.File.Exists(SQLBackup))
                {
                    string RootDir = ConfigurationManager.AppSettings["RootDirToBackup"];
                    string BackupDir = System.IO.Path.Combine(RootDir, ConfigurationManager.AppSettings["DatabaseBackupDir"]);
                    Directory.CreateDirectory(BackupDir);
                    string DB_Backup_File = System.IO.Path.Combine(BackupDir, ConfigurationManager.AppSettings["DatabaseBackupName"]);
                    System.IO.File.Delete(DB_Backup_File);

                    Trace.WriteLine("Database backup copying to file: " + DB_Backup_File);
                    System.IO.File.Copy(SQLBackup, DB_Backup_File);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception:\r\n" + e.Message + "\r\n" + e.StackTrace);
            }

            //go ahead and return if file exists, will be included in full backup even if copy failed
            return System.IO.File.Exists(SQLBackup);  //if this does not exist it did not backup

        }

        public bool BackupSystem()
        {
            bool Status = false;
            try
            {
                string RootBackupDir = ConfigurationManager.AppSettings["RootDirToBackup"];
                string ExcludeDirs = ConfigurationManager.AppSettings["ExcludeDirectories"];
                string CreateBackupInDirectory = ConfigurationManager.AppSettings["CreateBackupInDirectory"];
                Directory.CreateDirectory(CreateBackupInDirectory);

                List<string> BackupDirs = GetBackupDirs(RootBackupDir, ExcludeDirs);

                foreach (string Dir in BackupDirs)
                {
                    string ZipDir = System.IO.Path.Combine(BackUpDirectory, System.IO.Path.GetFileName(Dir)) + ".zip";
                    Trace.WriteLine("System backup of directory " + Dir + " to zip file: " + ZipDir);
                    System.IO.Compression.ZipFile.CreateFromDirectory(Dir, ZipDir);
                }

                MasterZipName = DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Year.ToString() + "- PRM SystemBackup.zip";
                string MasterZip = System.IO.Path.Combine(CreateBackupInDirectory, MasterZipName);
                if (System.IO.File.Exists(MasterZip))
                    System.IO.File.Delete(MasterZip);
                Trace.WriteLine("Creating master zip file " + MasterZip);
                System.IO.Compression.ZipFile.CreateFromDirectory(BackUpDirectory, MasterZip, System.IO.Compression.CompressionLevel.NoCompression,false);

                Status = true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception caught in BackupProcessor::BackupSystem():\r\n" + e.Message + "\r\n" + e.StackTrace);
            }

            try
            {   //cleanup
                Trace.WriteLine("Removing directory " + BackUpDirectory);
                System.IO.Directory.Delete(BackUpDirectory, true);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception caught while removing directory " + BackUpDirectory + ":\r\n" + e.Message + "\r\n" + e.StackTrace);
            }

            return Status;
        }

        private List<string> GetBackupDirs(string RootDirectory, string ExcludeDirs)
        {
            string[] Excludes = ExcludeDirs.ToLower().Split(new char[] { '~' });
            DirectoryInfo directory = new DirectoryInfo(RootDirectory);
            DirectoryInfo[] directories = directory.GetDirectories();

            List<string> Candidates = new List<string>();
            foreach (DirectoryInfo DI in directories)
            {
                if (Excludes.Contains(DI.Name.ToLower()) == false)
                    Candidates.Add( DI.FullName );
            }
            return Candidates;
        }

        public void SendEmail(string Msg, MailPriority EmailPriority = MailPriority.Normal )
        {
            try
            {
                string EmailAddress = ConfigurationManager.AppSettings["StatusEmail"];
                string SMTPServer = ConfigurationManager.AppSettings["SmtpServer"];
                int Port = System.Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);

                MailMessage objeto_mail = new MailMessage();
                SmtpClient client = new SmtpClient();
                client.Port = Port;
                client.Host = SMTPServer;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = true;
                //client.Credentials = new System.Net.NetworkCredential("user", "Password");
                objeto_mail.From = new MailAddress("PRMSysBackup@milliman.com");
                objeto_mail.To.Add(new MailAddress(EmailAddress));
                objeto_mail.Subject = "PRM System Backup Daemon Status";
                objeto_mail.Body = Msg;
                objeto_mail.Priority = EmailPriority;

                Trace.WriteLine("Sending email notification to address " + EmailAddress);
                client.Send(objeto_mail);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception caught while sending email notification:\r\n" + e.Message + "\r\n" + e.StackTrace);
            }
        }

        public void Cleaner()
        {
            int RetainForDays = System.Convert.ToInt32(ConfigurationManager.AppSettings["RetentionDays"]);
            string CreateBackupInDirectory = ConfigurationManager.AppSettings["CreateBackupInDirectory"];

            DateTime DeletionDate = DateTime.Now.AddDays(RetainForDays * -1);
            var files = new DirectoryInfo(CreateBackupInDirectory).GetFiles();
            foreach (var file in files)
            {
                if (DateTime.UtcNow - file.CreationTimeUtc > TimeSpan.FromDays(RetainForDays))
                {
                    Trace.WriteLine("Cleaner deleting file " + file.FullName);
                    File.Delete(file.FullName);
                }
            }
        }
    }
}
