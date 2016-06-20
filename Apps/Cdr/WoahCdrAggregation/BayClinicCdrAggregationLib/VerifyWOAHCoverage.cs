using BayClinicCernerAmbulatory;
using Devart.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CoverageVerification
{
    public class VerifyWOAHCoverage
    {

        public Boolean isCovered(MongodbPersonEntity person, SQLiteDataReader reader)
        {
            string PersonKey = person.LastName.ToLower() + ", " + person.FirstName.ToLower();
            string DOBKey = person.BirthDateTime.Split(' ')[0];

            while (reader.Read())
            {
                if (PersonKey == reader[0].ToString().ToLower())
                {
                    if(DOBKey == reader[1].ToString())        
                        return true;
                }
            }

            return false;

        }
    }
}
