using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLSPOCO
{
    public class AssayDescriptionSearch
    {
        private List<string> _AssayDescriptions;

        public List<string> AssayDescriptions
        {
            get
            {
                return _AssayDescriptions;
            }

            set
            {
                _AssayDescriptions = value;
            }
        }

        public AssayDescriptionSearch( bool Test)
        {
            _AssayDescriptions = new List<string>();
            if ( Test )
            {
                string TestData = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["TestDataPath"], "assaydescription.txt");

                if ( System.IO.File.Exists(TestData) == true )
                {
                    string[] Items = System.IO.File.ReadAllLines(TestData);
                    foreach (string Item in Items)
                        _AssayDescriptions.Add(Item);
                }
            }
        }
    }
}
