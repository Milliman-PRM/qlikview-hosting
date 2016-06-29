using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteConnect;
using Devart.Data.SQLite;
using System.Data;
using System.IO;

namespace WOHSQLInterface
{
    public class WOHSQLiteInterface
    {
        private SQLiteDataReader Reader;
        private SQLiteCommand Command;
        private SQLiteConnection Connection;
        private SQLiteDbConnection WOHSQLConnection;

        
        public void Connect()
        {
            WOHSQLConnection = new SQLiteDbConnection(CustomerEnum.WOAH);
            Connection = WOHSQLConnection.ConnectSQLite();

            Command = Connection.CreateCommand();
            Command.CommandText = "SELECT member_id, mem_name, dob FROM member";
            Command.CommandType = CommandType.Text;
        }

        //Specific to WOH. Makes sure the information passed along matches what we have stored in our WOH database
        public bool CheckMembershipStatus(string MemberID, string FirstName, string LastName, string DOB)
        {
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

        public void Disconnect()
        {
            WOHSQLConnection.Disconnect();
        }
    }
}
