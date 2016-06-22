using Devart.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteConnect
{
    public class VerifyWOAHCoverage
    {
        public Boolean IsCovered(SQLiteDatabaseConnection Connection, string memberid, string FirstName, string LastName, string DOB)
        {
            string PersonKey = LastName.ToLower() + ", " + FirstName.ToLower();
            string DOBKey = DOB.Split(' ')[0];

            SQLiteDataReader reader = Connection.reader;
            while (reader.Read())
            {
                if(memberid == reader[0].ToString())
                {
                    return true;
                }
                if (PersonKey == reader[0].ToString().ToLower())
                {
                    if (DOBKey == reader[1].ToString())
                        return true;
                }
            }

            return false;
        }
    }
}
