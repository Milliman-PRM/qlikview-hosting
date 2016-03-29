using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLSConfigurationCommon
{
    static public class PostgresqlUtilities
    {
        /// <summary>
        /// Find the name of the schema in the web.config file and return its name
        /// </summary>
        /// <param name="WebConfigContents">The contents of a web.config file</param>
        /// <returns></returns>
        static public string FindSchemaNameInWebConfig(string WebConfigContents )
        {
            string SchemaName = string.Empty;
            string SearchToken = "initial schema";
            int StartIndex = WebConfigContents.ToLower().IndexOf(SearchToken) + SearchToken.Length;
            int QuotesEndIndex = WebConfigContents.IndexOf('"', StartIndex);
            int SemicolonEndIndex = WebConfigContents.IndexOf(';', StartIndex);
            int EndIndex = QuotesEndIndex;
            if ( (SemicolonEndIndex != -1) &&(SemicolonEndIndex < QuotesEndIndex))  //could be ';' or '"' delimited
                EndIndex = SemicolonEndIndex;
            string CandidateName = WebConfigContents.Substring(StartIndex+1, EndIndex - StartIndex);
            //a bit brute force, but still a fast way to check
            string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJLKMNOPQRSTUVWXYZ-_0123456789";
            foreach (char C in CandidateName)
            {
                if (validChars.Contains(C) == true)
                    SchemaName += C;
            }

            return SchemaName;
        }

        /// <summary>
        /// Call this routine to backup local postres, or remote postgres configured
        /// to use MD5 authentication ( username and password set in DB )
        /// </summary>
        /// <param name="ServerAddress"></param>
        /// <param name="ServerPort"></param>
        /// <param name="DatabaseName"></param>
        /// <param name="SchemaName"></param>
        /// <param name="UserName"></param>
        /// <param name="UserPassword"></param>
        /// <param name="QualifiedFilePathForPGDump"></param>
        /// <param name="QualifiedBackupFilePath"></param>
        /// <returns></returns>
        static public bool CreatePostreSQLBackup(string ServerAddress,
                                              string ServerPort,
                                              string DatabaseName,
                                              string SchemaName,
                                              string UserName,
                                              string UserPassword,
                                              string QualifiedFilePathForPGDump,
                                              string QualifiedBackupFilePath)
        {
            return CreatePostgresqlBackup(ServerAddress, ServerPort, DatabaseName, SchemaName, UserName, UserPassword, QualifiedFilePathForPGDump, QualifiedBackupFilePath, false);
        }

        /// <summary>
        /// Call this routine to backup an instance of postres using AD/LDAP authentication,
        /// the calling machine must have a user logged in with an active profile
        /// </summary>
        /// <param name="ServerAddress"></param>
        /// <param name="ServerPort"></param>
        /// <param name="DatabaseName"></param>
        /// <param name="SchemaName"></param>
        /// <param name="QualifiedFilePathForPGDump"></param>
        /// <param name="QualifiedBackupFilePath"></param>
        /// <returns></returns>
        static public bool CreatePostreSQLBackup(string ServerAddress,
                                                   string ServerPort,
                                                   string DatabaseName,
                                                   string SchemaName,
                                                   string QualifiedFilePathForPGDump,
                                                   string QualifiedBackupFilePath)
        {
            return CreatePostgresqlBackup(ServerAddress, ServerPort, DatabaseName, SchemaName, "", "", QualifiedFilePathForPGDump, QualifiedBackupFilePath, true);
        }

        /// <summary>
        /// Call this routine to restore a postres instance using MD5 authentication, like localhost
        /// </summary>
        /// <param name="ServerAddress"></param>
        /// <param name="ServerPort"></param>
        /// <param name="DatabaseName"></param>
        /// <param name="SchemaName"></param>
        /// <param name="UserName"></param>
        /// <param name="UserPassword"></param>
        /// <param name="QualifiedFilePathForPGRestore"></param>
        /// <param name="QualifiedBackupFilePath"></param>
        /// <returns></returns>
        static public bool RestorePostreSQLFromBackup(string ServerAddress,
                                         string ServerPort,
                                         string DatabaseName,
                                         string SchemaName,
                                         string UserName,
                                         string UserPassword,
                                         string QualifiedFilePathForPGRestore,
                                         string QualifiedBackupFilePath)
        {
            return RestorePostgresqlBackup(ServerAddress, ServerPort, DatabaseName, SchemaName, UserName, UserPassword, QualifiedFilePathForPGRestore, QualifiedBackupFilePath, false);
        }

        /// <summary>
        /// Use this routine to restore a database instance using AD/LDAP authentcation, calling
        /// instance must have a user logged in with an active profile
        /// </summary>
        /// <param name="ServerAddress"></param>
        /// <param name="ServerPort"></param>
        /// <param name="DatabaseName"></param>
        /// <param name="SchemaName"></param>
        /// <param name="QualifiedFilePathForPGRestore"></param>
        /// <param name="QualifiedBackupFilePath"></param>
        /// <returns></returns>
        static public bool RestorePostreSQLFromBackup(string ServerAddress,
                                                   string ServerPort,
                                                   string DatabaseName,
                                                   string SchemaName,
                                                   string QualifiedFilePathForPGRestore,
                                                   string QualifiedBackupFilePath)
        {
            return RestorePostgresqlBackup(ServerAddress, ServerPort, DatabaseName, SchemaName, "", "", QualifiedFilePathForPGRestore, QualifiedBackupFilePath, true);
        }
        static private bool CreatePostgresqlBackup( string ServerAddress,
                                              string ServerPort,
                                              string DatabaseName,
                                              string SchemaName,
                                              string UserName,
                                              string UserPassword,
                                              string QualifiedFilePathForPGDump,
                                              string QualifiedBackupFilePath,
                                              bool   UseSSPI = false )
        {
            //C:/Program Files/PostgreSQL/9.5/bin\pg_dump.exe --host indy-pgsql02 --port 5432 --username "van.nanney" --no-password  --format custom --section pre-data --section data --section post-data --no-privileges --verbose --no-unlogged-table-data --file "E:\PGBackups\rmrrdb_20160316.backup" --schema "rmrrdb_20160316" "Roche_Medicare_Reimbursement_Develop"
            string Pswd = "SET PGPASSWORD=" + UserPassword;
            string PGDump = "\"" + QualifiedFilePathForPGDump + "\"" + " --host " + ServerAddress + " --port " + ServerPort + " --username \"" + UserName + "\" --no-password --format custom --section pre-data --section data --section post-data --no-privileges --verbose --no-unlogged-table-data --file \"" + QualifiedBackupFilePath + "\" --schema \"" + SchemaName + "\"  \"" + DatabaseName + "\"";
            if ( UseSSPI )
                PGDump = "\"" + QualifiedFilePathForPGDump + "\""  + " --host " + ServerAddress + " --port " + ServerPort + " --no-password --format custom --section pre-data --section data --section post-data --no-privileges --verbose --no-unlogged-table-data --file \"" + QualifiedBackupFilePath + "\" --schema \"" + SchemaName + "\"  \"" + DatabaseName + "\"";
            string BatchFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.Guid.NewGuid().ToString("N") + ".bat");
            System.IO.File.AppendAllText(BatchFile, Pswd);
            System.IO.File.AppendAllText(BatchFile, System.Environment.NewLine);
            System.IO.File.AppendAllText(BatchFile, PGDump);
            System.Diagnostics.Process processDB = System.Diagnostics.Process.Start(BatchFile);
            do
            {//dont perform anything
            }
            while (!processDB.HasExited);
            {
                System.Threading.Thread.Sleep(500);
            }
            System.IO.File.Delete(BatchFile);
            return true;
        }
        static private bool RestorePostgresqlBackup(string ServerAddress,
                                               string ServerPort,
                                               string DatabaseName,
                                               string SchemaName,
                                               string UserName,
                                               string UserPassword,
                                               string QualifiedFilePathForPGRestore,
                                               string QualifiedBackupFilePath,
                                               bool UseSSPI = false)
        {
            // C:/ Program Files / PostgreSQL / 9.5 / bin\pg_restore.exe--host localhost --port 5432--username "postgres"--dbname "Roche_Medicare_Reimbursement_Develop"--no - password--verbose "E:\PGBackups\rmrrdb_20160316.backup"
            string Pswd = "SET PGPASSWORD=" + UserPassword;
            string PGDump = "\"" + QualifiedFilePathForPGRestore + "\"" + " --host " + ServerAddress + " --port " + ServerPort + " --username \"" + UserName + "\"  --dbname \"" + DatabaseName + "\" --no-password --verbose --clean \"" + QualifiedBackupFilePath + "\"";
            if (UseSSPI)
                PGDump = "\"" + QualifiedFilePathForPGRestore + "\"" + " --host " + ServerAddress + " --port " + ServerPort + "  --dbname \"" + DatabaseName + "\" --no-password --verbose --clean \"" + QualifiedBackupFilePath + "\"";
            string BatchFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.Guid.NewGuid().ToString("N") + ".bat");
            System.IO.File.AppendAllText(BatchFile, Pswd);
            System.IO.File.AppendAllText(BatchFile, System.Environment.NewLine);
            System.IO.File.AppendAllText(BatchFile, PGDump);
            System.Diagnostics.Process processDB = System.Diagnostics.Process.Start(BatchFile);
            do
            {//dont perform anything
            }
            while (!processDB.HasExited);
            {
                System.Threading.Thread.Sleep(500);
            }
            System.IO.File.Delete(BatchFile);
            return true;
        }

        static private string GetConnectionString()
        {
            string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            return ConnectionString;
        }

        /// <summary>
        /// Retrieve a list of schemas,  check the schema black list and
        /// remove any entries that are blacklisted
        /// </summary>
        /// <returns></returns>
        static public System.Data.DataTable GetSchemas()
        {
            List<string> SchemaList = new List<string>();
            //select schema_name, schema_owner from information_schema.schemata where LOWER(schema_owner)=LOWER('brad.teach')
            //select schema_name, schema_owner from information_schema.schemata
            string Query = "select schema_name, schema_owner from information_schema.schemata";
            //append valid users
            string ConnectionString = GetConnectionString();
            string BlackList = System.Configuration.ConfigurationManager.AppSettings["PostgresSchemaBlackList"];
            string[] BlackListTokens = BlackList.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
            string WhereClause = string.Empty;
            foreach (string BL in BlackListTokens)
            {
                if (string.IsNullOrEmpty(WhereClause) == false)
                    WhereClause += " AND ";
                WhereClause += " LOWER(schema_name) != LOWER('" + BL + "') ";
            }
            if (string.IsNullOrEmpty(WhereClause) == false)
                Query += " WHERE " + WhereClause;

            System.Data.DataTable dt = new System.Data.DataTable();
            System.Data.DataSet ds = new System.Data.DataSet();

            Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(ConnectionString);
            conn.Open();

            Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter(Query, conn);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            conn.Close();

            return dt;
        }

        //delete schema and all objects
        static public bool DeleteSchema( string SchemaName )
        {
            string Query = "DROP SCHEMA If EXISTS " + SchemaName + " CASCADE";
            //append valid users
            string ConnectionString = GetConnectionString();
            Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(ConnectionString);
            Npgsql.NpgsqlCommand Command = new Npgsql.NpgsqlCommand(Query, conn);
            conn.Open();
            Command.ExecuteNonQuery();
            conn.Close();
            return true;
        }
        /// <summary>
        /// If connection string does not contain a password entry, assumes configured
        /// for SSPI,  otherwise MD5
        /// </summary>
        /// <returns></returns>
        static public bool ConnectionConfiguredForSSPI()
        {
            string Server = string.Empty;
            string PostgresPort = string.Empty;
            string UserName = string.Empty;
            string Password = string.Empty;
            string DatabaseName = string.Empty;
            string InitialSchemaName = string.Empty;
            bool IntegratedSecurity = false;
            if (ParametersFromConnectionString(out Server,
                                                 out PostgresPort,
                                                 out UserName,
                                                 out Password,
                                                 out DatabaseName,
                                                 out InitialSchemaName,
                                                 out IntegratedSecurity))
            {
                return IntegratedSecurity;
            }
            throw new Exception("Failed to retrieve and parse connection string.");
        }


        /// <summary>
        /// parse the connection string into parameters for other use
        /// </summary>
        /// <param name="PostgresServer"></param>
        /// <param name="PostresPort"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="DatabaseName"></param>
        /// <param name="InitialSchemaName"></param>
        /// <param name="IntegratedSecurity"></param>
        /// <returns></returns>
        static public bool ParametersFromConnectionString( out string PostgresServer,
                                                           out string PostresPort,
                                                           out string UserName,
                                                           out string Password,
                                                           out string DatabaseName,
                                                           out string InitialSchemaName,
                                                           out bool   IntegratedSecurity )
        {
            PostgresServer = string.Empty;
            PostresPort = "5432";  //default to the postgres port
            UserName = string.Empty;
            Password = string.Empty;
            DatabaseName = string.Empty;
            InitialSchemaName = string.Empty;
            IntegratedSecurity = true;

            string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            //User Id=postgres;Password:jellyfish;Host=localhost;Database=Roche_Medicare_Reimbursement_Develop;Integrated Security=True;Initial Schema=rmrrdb_20160304

            List<string> ConnectionTokens = ConnectionString.Split(new char[] { '=', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int Index = 0; Index < ConnectionTokens.Count; Index = Index + 2)
            {
                if (string.Compare(ConnectionTokens[Index], "user id", true) == 0)
                    UserName = ConnectionTokens[Index + 1];
                else if ( string.Compare(ConnectionTokens[Index], "password", true ) == 0)
                    Password = ConnectionTokens[Index + 1];
                else if (string.Compare(ConnectionTokens[Index], "host", true) == 0)
                    PostgresServer = ConnectionTokens[Index + 1];
                else if (string.Compare(ConnectionTokens[Index], "database", true) == 0)
                    DatabaseName = ConnectionTokens[Index + 1];
                else if (string.Compare(ConnectionTokens[Index], "port", true) == 0)
                    PostresPort = ConnectionTokens[Index + 1];
                else if (string.Compare(ConnectionTokens[Index], "initial schema", true) == 0)
                    InitialSchemaName = ConnectionTokens[Index + 1];
                else if (string.Compare(ConnectionTokens[Index], "integrated security", true) == 0)
                    IntegratedSecurity = System.Convert.ToBoolean(ConnectionTokens[Index + 1]);
            }


            return true;
        }

        //pgdump.exe -n _SCHEMANAME_ -O -x
        //pg_dump -Fp your_db | psql -h new_server -U postgres your_new_db
        //pg_dump -h old_server_ip -p 5432 -U username dbname | psql -h localhost -p 5432 -U username dbname

        //backup
        //try
        //    {
        //        if (textBox1.Text == "-------")
        //        {
        //            MessageBox.Show("Select the location to save");
        //            return;
        //        }
        //        StreamWriter sw = new StreamWriter("DBBackup.bat");
        //// Do not change lines / spaces b/w words.
        //StringBuilder strSB = new StringBuilder(strPG_dumpPath);

        //        if (strSB.Length != 0)
        //        {
        //            strSB.Append("pg_dump.exe --host " + strServer + " --port " + strPort + 
        //              " --username postgres --format custom --blobs --verbose --file ");
        //            strSB.Append("\"" + textBox1.Text + "\"");
        //            strSB.Append(" \"" + strDatabaseName + "\r\n\r\n");
        //            sw.WriteLine(strSB);
        //            sw.Dispose();
        //            sw.Close();
        //            Process processDB = Process.Start("DBBackup.bat");
        //            do
        //            {//dont perform anything
        //            }
        //            while (!processDB.HasExited);
        //            {
        //                MessageBox.Show(strDatabaseName + " Successfully Backed up at " + textBox1.Text);
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("Please Provide the Location to take Backup!");
        //        }
        //    }
        //    catch (Exception ex)
        //    { }

        //restore
        //    try
        //    {
        //        if (txtBackupFilePath.Text == string.Empty)
        //        {
        //            MessageBox.Show("Select backup file");
        //            return;
        //        }
        //        //check for the pre-requisites before restoring the database.*********
        //        if (strDatabaseName != "")
        //        {
        //            if (txtBackupFilePath.Text != "")
        //            {
        //                StreamWriter sw = new StreamWriter("DBRestore.bat");
        //// Do not change lines / spaces b/w words.
        //StringBuilder strSB = new StringBuilder(strPG_dumpPath);
        //                if (strSB.Length != 0)
        //                {
        //                    checkDBExists(strDatabaseName);
        //strSB.Append("pg_restore.exe --host " + strServer + 
        //                       " --port " + strPort + " --username postgres --dbname");
        //                    strSB.Append(" \"" + strDatabaseName + "\"");
        //                    strSB.Append(" --verbose ");
        //                    strSB.Append("\"" + txtBackupFilePath.Text + "\"");
        //                    sw.WriteLine(strSB);
        //                    sw.Dispose();
        //                    sw.Close();
        //                    Process processDB = Process.Start("DBRestore.bat");
        //                    do
        //                    {//dont perform anything
        //                    }
        //                    while (!processDB.HasExited);
        //                    {
        //                        MessageBox.Show("Successfully restored " + 
        //                           strDatabaseName + " Database from " + txtBackupFilePath.Text);
        //                    }
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Please enter the save path to get the backup!");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("Please enter the Database name to Restore!");
        //        }
        //    }
        //    catch (Exception ex)
        //    { }

    }
}
