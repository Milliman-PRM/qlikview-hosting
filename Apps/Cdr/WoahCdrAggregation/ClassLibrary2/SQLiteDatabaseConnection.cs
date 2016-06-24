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
        private SQLiteDataReader Reader = null;
        private SQLiteConnection Connection = null;
        private SQLiteCommand Command = null;
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
            ConnectSQLite(FileName);
        }

        //If customer is specified then it will create a connection string according to the customer
        //Otherwise it will use the one provided in the constructor
        public void ConnectSQLite(string FileName = null)
        {
            switch (Customer)
            {
                case CustomerEnum.WOAH:
                    DirectoryInfo RootDirectoryInfo = new DirectoryInfo(@"K:\PHI\0273WOH\3.005-0273WOH06\5-Support_files\");
                    var MostRecentDirectory = RootDirectoryInfo.GetDirectories().OrderByDescending(f => f.Name).First();
                    ConnectionStr.ConnectionString = RootDirectoryInfo + MostRecentDirectory.FullName + @"\035_Staging_Membership\Members_3.005-0273WOH06.sqlite";
                    break;

                default:                    //No customer was specified, this means the string was given in the constructor and passed to this method
                    ConnectionStr.ConnectionString = "Data Source=" + FileName;
                    break;
            }

            Connection = new SQLiteConnection(ConnectionStr.ConnectionString);
        }

        //Returns a list of all the column names in a specified table
        public List<String> GetColumnNames(string Table)
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

        //Returns a List containing all of the information contained in specified columns
        public Dictionary<String, List<String>> QuerySQLiteTable(string Columns, string Table, string WhereConditions = null)
        {
            Dictionary<String, List<String>> ComprehensiveColumnInformation = new Dictionary<string, List<string>>();
            List<String> ColumnContent = new List<String>();
            String[] ColumnNames = Columns.Split(',');
            int ColumnCount = ColumnNames.Length;

            Command = Connection.CreateCommand();
            Command.CommandText = "SELECT " + Columns + " FROM " + Table;
            if (!String.IsNullOrEmpty(WhereConditions))
            {
                Command.CommandText += " WHERE " + WhereConditions;
            }
            Command.CommandType = CommandType.Text;

            Reader = Command.ExecuteReader();

            for (int i = 0; i < ColumnCount; i++)
            {
                while (Reader.Read())
                {
                    ColumnContent.Add(Reader[i].ToString());
                }
                ComprehensiveColumnInformation.Add(ColumnNames[i], ColumnContent);
                ColumnContent.Clear();
            }

            return ComprehensiveColumnInformation;
        }

        //Specific to WOH. Makes sure the information passed along matches what we have stored in our WOH database
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
                if (PersonKey == Reader[1].ToString().ToLower())
                {
                    if (DOBKey == Reader[2].ToString())
                        return true;
                }
            }

            return false;
        }

        //Checks to make sure that the connection state is not in the closed position.
        //It will return true if the connection state has another status besides open
        public bool CheckOpen()
        {
            return Connection.State != ConnectionState.Closed;
        }

        //Disconnects and resets all of the SQLiteDbConnection objects 
        public void Disconnect()
        {
            Connection.State = ConnectionState.Closed;

            Reader = null;
            Command = null;
            ConnectionStr = null;
        }
    }
}

