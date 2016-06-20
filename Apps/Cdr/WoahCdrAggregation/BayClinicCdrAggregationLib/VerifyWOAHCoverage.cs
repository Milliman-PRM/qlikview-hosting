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
        DateTime SasBase = new DateTime(1960, 1, 1);
        public Boolean isCovered(MongodbPersonEntity person, OleDbDataReader reader)
        {
            string PersonKey = person.LastName.ToLower() + ", " + person.FirstName.ToLower();

            DateTime ConvertedPersonDateTime = Convert.ToDateTime(person.BirthDateTime);
            int ConvertedDays = (int)(ConvertedPersonDateTime - SasBase).TotalDays;

            while (reader.Read())
            {
                if (PersonKey == reader.GetString(4).ToLower())
                {
                    if(ConvertedDays.ToString() == reader.GetValue(10).ToString())        
                        return true;
                }
            }

            return false;

        }
    }
}
