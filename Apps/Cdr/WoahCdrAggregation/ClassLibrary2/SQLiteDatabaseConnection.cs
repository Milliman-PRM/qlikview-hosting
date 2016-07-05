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
    public class SQLiteDbConnection
    {
        internal SQLiteDataReader Reader = null;
        internal SQLiteConnection Connection = null;
        internal SQLiteCommand Command = null;

        public SQLiteDbConnection()
        {
        }

        public void Connect(string FileName)
        {
            String ConnectionStr = "DataSource=" + FileName + ";";
            Connection = new SQLiteConnection(ConnectionStr);
        }

        //Returns a list of all the column names in a specified table
        public List<String> GetColumnNames(string Table)
        {
            List<string> ColumnNames = new List<string>();

            Command = Connection.CreateCommand();
            Command.CommandText = "SELECT * FROM " + Table + " LIMIT 1";
            Command.CommandType = CommandType.Text;

            Connection.Open();
            Reader = Command.ExecuteReader();

            if (Reader.Read())
            {
                for (int i= 0 ; i < Reader.FieldCount; i++)
                {
                    ColumnNames.Add(Reader.GetName(i));
                }
            }

            Reader.Close();
            Command.Dispose();
            Connection.Close();

            return ColumnNames;
        }

        //Returns a List containing all of the information contained in specified columns
        // TODO This should return an iterable of some kind, rather than all the data in a memory resident object (there could be 90 million records)
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
            
            //Prevent leaks
            Reader.Close();
            Command.Dispose();

            return ComprehensiveColumnInformation;
        }

        

        //Checks to make sure that the connection state is not in the closed position.
        //It will return true if the connection state has another status besides open
        public bool CheckOpen()
        {
            // TODO needs some refinement (what about values: Broken, Connecting, ...)
            return Connection.State != ConnectionState.Closed;
        }

        //Disconnects and resets all of the SQLiteDbConnection objects 
        public void Disconnect()
        {
            Reader.Close();
            Command.Dispose();
            Connection.Close();
        }
    }
}

