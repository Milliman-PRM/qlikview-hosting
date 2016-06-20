using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Data;
using Devart.Data.SQLite;
using System.Data;

namespace SQLiteConnect
{
    public class SQLiteDatabaseConnection
    {
        string columns = "mem_name, dob";
        string table = "member";
        string source = @"K:\PHI\0273WOH\3.005-0273WOH06\5-Support_files\Data_Thru_201604_M5_QM\035_Staging_Membership\Members_0273WOH_3.005-0273WOH_v5.11.2_00_Data20160426_v20160524183418.sqlite";
        public SQLiteDataReader makeConnection() {

            SQLiteConnection myConn = new SQLiteConnection("Data Source=" + source);
            myConn.Open();

            SQLiteCommand cmd = myConn.CreateCommand();

            cmd.CommandText = "select " + columns + " from " + table;
            cmd.CommandType = CommandType.Text;

            SQLiteDataReader reader = cmd.ExecuteReader();

            return reader;
        }

    }
}
