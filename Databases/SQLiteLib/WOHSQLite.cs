using System;
using System.Diagnostics;
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
    public class WOHSQLiteInterface : CustomerSpecificBase
    {
        private SQLiteDbConnection WOHSQLConnection;
        private String Volume = @"\\indy-netapp\prm_phi";

        // Devart types
        private SQLiteCommand Command;
        private SQLiteDataReader Reader;

        public WOHSQLiteInterface()
        {
            WOHSQLConnection = new SQLiteDbConnection();
        }

        ~WOHSQLiteInterface()
        {
            Disconnect();
        }

        public void ConnectToMembershipData(String SqlLiteFileOverride = null)
        {
            String SQLiteFile;

            if (String.IsNullOrEmpty(SqlLiteFileOverride))
            {
                SupportFilesRoot = new DirectoryInfo(Path.Combine(Volume, @"PHI\0273WOH\3.005-0273WOH06\5-Support_files\"));
                DirectoryInfo MostRecentSupportFilesDirectory = SupportFilesRoot.GetDirectories().OrderByDescending(f => f.Name).First();
                SQLiteFile = Path.Combine(MostRecentSupportFilesDirectory.FullName, @"035_Staging_Membership\Members_3.005-0273WOH06.sqlite");
            }
            else
            {
                SQLiteFile = SqlLiteFileOverride;
            }

            WOHSQLConnection.Connect(SQLiteFile);
            List<String> x = WOHSQLConnection.GetColumnNames("member");
        }

        //Specific to WOH. Makes sure the information passed along matches what we have stored in our WOH database
        public bool CheckMembershipStatus(string MemberID, string FirstName, string LastName, string DOB)
        {
            string PersonKey = LastName.ToLower() + ", " + FirstName.ToLower();
            string DOBKey = DOB.Split(' ')[0];

            Command = WOHSQLConnection.Connection.CreateCommand();

            Command.CommandText = "SELECT member_id, mem_name, dob FROM member WHERE member_id = :id OR (mem_name = :key AND dob = :dob";
            Command.Parameters.Add("id", MemberID);
            Command.Parameters.Add("key", PersonKey);
            Command.Parameters.Add("dob", DOB);
            Command.CommandType = CommandType.Text;

            WOHSQLConnection.Connection.Open();
            Reader = Command.ExecuteReader();


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

        public IEnumerable<String> GetWoahIds()
        {
            Command = WOHSQLConnection.Connection.CreateCommand();

            Command.CommandText = "SELECT member_id, mem_name, dob FROM member";
            Command.CommandType = CommandType.Text;

            WOHSQLConnection.Connection.Open();
            Reader = Command.ExecuteReader();

            while (Reader.Read())
            {
                yield return Reader[0].ToString();
            }
        }

        public void Disconnect()
        {
            WOHSQLConnection.Disconnect();
        }
    }
}
