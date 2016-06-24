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
        public SQLiteDataReader Reader;

        public SQLiteDatabaseConnection(CustomerEnum  Customer, string Columns, string Table)
        {
            this._Customer = Customer;
            this.ConnectToDatabase(Columns, Table); // TODO Move the last 2 constructor args and this call to a Connect(...) or Initialize method
            // Probably including a GetMembership method with encapsulated column names is better
            // Each read from the reader returns a Dictionary<String,String> with column,value pairs
        }


        public SQLiteConnection GetConnection()
        {
            SQLiteConnection Conn = null;
            DirectoryInfo RootDirectoryInfo;
            string Source;

            switch (_Customer) {
                case CustomerEnum.WOAH:
                    RootDirectoryInfo = new DirectoryInfo(@"K:\PHI\0273WOH\3.005-0273WOH06\5-Support_files\");
                    var MostRecentDirectory = RootDirectoryInfo.GetDirectories().OrderByDescending(f => f.Name).First();
                    Source = RootDirectoryInfo + MostRecentDirectory.FullName + @"\035_Staging_Membership\Members_3.005-0273WOH06.sqlite";
                    break;

                default:
                    return null;
            }

            Conn = new SQLiteConnection("Data Source=" + Source);

            return Conn;
        }

        public SQLiteCommand GetCommand(SQLiteConnection Connection, string Columns, string Table, string WhereConditions=null)
        {
            SQLiteCommand Cmd = Connection.CreateCommand();

            Cmd.CommandText = "SELECT " + Columns + " FROM " + Table;
            if (!String.IsNullOrEmpty(WhereConditions))
            {
                Cmd.CommandText += " WHERE " + WhereConditions;
            }
            Cmd.CommandType = CommandType.Text;

            return Cmd;

        }
        public void ConnectToDatabase(string Columns, string Table) {

            SQLiteConnection MyConn = GetConnection();

            SQLiteCommand Command = GetCommand(MyConn, Columns, Table);

            this.Reader = Command.ExecuteReader();
            // This class should support multiple commands with dedicated methods that implement them.  Persist the reader in response to a command method being called.  

        }



    }
}

