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
        public SQLiteConnection ConnectSQLite(string FileName = null)
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
            return Connection;
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

            Reader.Close();
            Command.Dispose();

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

            for(int i = 0; i < ColumnCount; i++)
            {
                ComprehensiveColumnInformation.Add(ColumnNames[i], ColumnContent);
            }

            while (Reader.Read())
            {
                for(int i = 0; i < ColumnCount; i++)
                {
                    ComprehensiveColumnInformation[ColumnNames[i]].Add(Reader[i].ToString());
                }
            }

            Command.Dispose();
            Reader.Close();

            return ComprehensiveColumnInformation;
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
            Command.Dispose();
            Connection.Close();
            Reader.Close();
            ConnectionStr.Clear();
        }
    }
}

