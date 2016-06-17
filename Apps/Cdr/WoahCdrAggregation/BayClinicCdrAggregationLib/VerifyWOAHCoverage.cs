using BayClinicCernerAmbulatory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SasDataSetLib
{
    public class VerifyWOAHCoverage
    {

        public Boolean isCovered(MongodbPersonEntity person, OleDbDataReader reader)
        {
            //TODO add birthday as part of check
            string PersonKey = person.LastName.ToLower() + ", " + person.FirstName.ToLower();

            while (reader.Read())
            {
                if (PersonKey == reader.GetString(4).ToLower())
                {
                    return true;
                }
            }

            return false;

        }
    }
}
