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
        public Boolean IsCovered(SQLiteDatabaseConnection Connection, string MemberID, string FirstName, string LastName, string DOB)
        {
            string PersonKey = LastName.ToLower() + ", " + FirstName.ToLower();
            string DOBKey = DOB.Split(' ')[0];

            SQLiteDataReader Reader = Connection.Reader;
            while (Reader.Read())
            {
                if(MemberID == Reader[0].ToString())
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
    }
}
