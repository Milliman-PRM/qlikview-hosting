using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Data;
using Devart.Data.SQLite;
using System.Data;
using System.IO;

// TODO Move the last 2 constructor args and this call to a Connect(...) or Initialize method
// Probably including a GetMembership method with encapsulated column names is better
// Each read from the reader returns a Dictionary<String,String> with column,value pairs
namespace SQLiteConnect
{

    public enum CustomerEnum { WOAH };


    public class SQLiteDbConnection
    {
        public CustomerEnum Customer;
        public SQLiteDataReader Reader = null;
        public SQLiteConnection Connection = null;
        public SQLiteCommand Command = null;
        private SQLiteConnectionStringBuilder ConnectionStr;

        //For use specific to a customer
        public SQLiteDbConnection(CustomerEnum Customer)
        {
            this.Customer = Customer;
            ConnectSQLite();
        }

        //For general purpose use
        public SQLiteDbConnection(string FileName)
        {
            ConnectionStr.ConnectionString = "Data Source=" + FileName;
            ConnectSQLite();
        }

        //If customer is specified then it will create a connection string according to the customer
        //Otherwise it will use the one provided in the constructor
        public void ConnectSQLite()
        {
            switch (Customer)
            {
                case CustomerEnum.WOAH:
                    DirectoryInfo RootDirectoryInfo = new DirectoryInfo(@"K:\PHI\0273WOH\3.005-0273WOH06\5-Support_files\");
                    var MostRecentDirectory = RootDirectoryInfo.GetDirectories().OrderByDescending(f => f.Name).First();
                    ConnectionStr.ConnectionString = RootDirectoryInfo + MostRecentDirectory.FullName + @"\035_Staging_Membership\Members_3.005-0273WOH06.sqlite";
                    break;

                default:                    //No customer was specified, connection string was already set
                    break;
            }

            Connection = new SQLiteConnection(ConnectionStr.ConnectionString);
        }

        public List<string> GetColumnNames(string Table)
        {
            List<string> ColumnNames = new List<string>();

            Command = Connection.CreateCommand();
            Command.CommandText = "SELECT * FROM " + Table + ".INFORMATION_SCHEMA.COLUMNS ";
            Command.CommandType = CommandType.Text;
            Reader = Command.ExecuteReader();

            while (Reader.Read())
            {
                ColumnNames.Add(Reader[0].ToString());
            }

            return ColumnNames;
        }
        public SQLiteDataReader GetSQLiteTable(string Columns, string Table, string WhereConditions = null)
        {
            Command = Connection.CreateCommand();

            Command.CommandText = "SELECT " + Columns + " FROM " + Table;
            if (!String.IsNullOrEmpty(WhereConditions))
            {
                Command.CommandText += " WHERE " + WhereConditions;
            }
            Command.CommandType = CommandType.Text;

            Reader = Command.ExecuteReader();

            return Reader;
        }

        public bool CheckMembership(string MemberID, string FirstName, string LastName, string DOB)
        {
            Command = Connection.CreateCommand();
            Command.CommandText = "SELECT member_id, mem_name, dob FROM member";
            Command.CommandType = CommandType.Text;
            Reader = Command.ExecuteReader();

            string PersonKey = LastName.ToLower() + ", " + FirstName.ToLower();
            string DOBKey = DOB.Split(' ')[0];

  
            while (Reader.Read())
            {
                if (MemberID == Reader[0].ToString())
                {
                    return true;
                }
                if (PersonKey == Reader[0].ToString().ToLower())
                {
                    if (DOBKey == Reader[1].ToString())
                        return true;
                }
            }

            return false;
        }
            
        

        public bool CheckOpen()
        {
            // TODO: This needs to be better, check on connection status
            return Connection != null;
        }

        public void Disconnect()
        {
            Connection = null;
            Reader = null;
            Command = null;
        }
    }
}

