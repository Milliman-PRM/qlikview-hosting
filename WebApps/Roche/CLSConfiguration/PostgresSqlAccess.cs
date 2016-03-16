using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLSConfiguration
{
    public class PostgresSqlAccess
    {
        
        public System.Data.DataTable GetSchemas()
        {
            List<string> SchemaList = new List<string>();
            //select schema_name, schema_owner from information_schema.schemata where LOWER(schema_owner)=LOWER('brad.teach')
            //select schema_name, schema_owner from information_schema.schemata
            string Query = "select schema_name, schema_owner from information_schema.schemata";
            //append valid users
            string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            string BlackList = System.Configuration.ConfigurationManager.AppSettings["PostgresSchemaBlackList"];
            string[] BlackListTokens = BlackList.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
            string WhereClause = string.Empty;
            foreach( string BL in BlackListTokens )
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

    }
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

