using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLSPOCO
{
    public class MedicareRates
    {
        public List<MedicareRate> AllRates { get; set; }

        public MedicareRates(bool Test = false )
        {
            AllRates = new List<MedicareRate>();
            if ( Test )
            {
                string TestData = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["TestDataPath"], "rates.csv");

                string[] AllRateData = System.IO.File.ReadAllLines(TestData);
                foreach( string Rate in AllRateData)
                {
                    string[] Tokens = Rate.Split(new char[] { ',','"', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    string Notes = string.Empty;
                    for( int Index = 3; Index < Tokens.Length-2; Index++ )
                    {
                        if (string.IsNullOrEmpty(Notes) == false)
                            Notes += ",";
                        Notes += Tokens[Index];

                    }
                    AllRates.Add(new MedicareRate(Tokens[0], Tokens[1], Tokens[2], Notes, Tokens[Tokens.Length-2], Tokens[Tokens.Length-1]));
                }


            }
        }

    }
}
