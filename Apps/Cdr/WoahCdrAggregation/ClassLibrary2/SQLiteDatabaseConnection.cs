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

    public enum CustomerEnum { WOAH };


    public class SQLiteDatabaseConnection
    {
        public CustomerEnum _Customer;
        public SQLiteDataReader reader;

        public SQLiteDatabaseConnection(CustomerEnum  Customer, string Columns, string Table)
        {
            this._Customer = Customer;
            this.ConnectToDatabase(Columns, Table);
        }


        public SQLiteConnection GetConnection()
        {
            SQLiteConnection Conn = null;

            if (_Customer == CustomerEnum.WOAH) { 
                var getDirectory = new DirectoryInfo(@"K:\PHI\0273WOH\3.005-0273WOH06\5-Support_files\");
                var mostRecentDirectory = getDirectory.GetDirectories().OrderByDescending(f => f.Name).First();

                string source = @"K:\PHI\0273WOH\3.005-0273WOH06\5-Support_files\" + mostRecentDirectory + @"\035_Staging_Membership\Members_3.005-0273WOH06.sqlite";

                Conn = new SQLiteConnection("Data Source=" + source);
            }


            return Conn;
        }

        public SQLiteCommand GetCommand(SQLiteConnection connection, string Columns, string Table)
        {
            SQLiteCommand cmd = connection.CreateCommand();

            cmd.CommandText = "select " + Columns + " from " + Table;
            cmd.CommandType = CommandType.Text;

            return cmd;

        }
        public void ConnectToDatabase(string Columns, string Table) {

            SQLiteConnection myConn = GetConnection();

            SQLiteCommand command = GetCommand(myConn, Columns, Table);

            this.reader = command.ExecuteReader();

        }



    }
}

