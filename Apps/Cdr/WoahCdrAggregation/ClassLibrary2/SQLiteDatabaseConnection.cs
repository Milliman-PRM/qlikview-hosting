using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Data;
using Devart.Data.SQLite;
using System.Data;
using System.IO;

namespace SQLiteConnect
{
    public class SQLiteDatabaseConnection
    {
        string Columns = "mem_name, dob";
        string Table = "member";
        string Directory = @"K:\PHI\0273WOH\3.005-0273WOH06\5-Support_files\";
        string source = "";
        public SQLiteDataReader makeConnection() {

            var getDirectory = new DirectoryInfo(Directory);
            var mostRecentDirectory = getDirectory.GetDirectories().OrderByDescending(f => f.Name).First();

            source = Directory + mostRecentDirectory + @"\035_Staging_Membership\Members_3.005-0273WOH06.sqlite";

            SQLiteConnection myConn = new SQLiteConnection("Data Source=" + source);
            myConn.Open();

            SQLiteCommand cmd = myConn.CreateCommand();

            cmd.CommandText = "select " + Columns + " from " + Table;
            cmd.CommandType = CommandType.Text;

            SQLiteDataReader reader = cmd.ExecuteReader();

            return reader;
        }

    }
}

