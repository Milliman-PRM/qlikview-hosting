using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLSPOCO
{
    public class MedicareRate
    {
        public string Analyzer { get; set; }
        public string AssayDescription { get; set; }
        public string CPTDescriptor { get; set; }
        public string Notes { get; set; }
        public string Locality { get; set; }
        public string Rate { get; set;  }

        public MedicareRate()
        {

        }

        public MedicareRate( string _Analyzer, string _AssayDescription, string _CPTDescriptor, string _Notes, string _Locality, string _Rate )
        {
            Analyzer = _Analyzer;
            AssayDescription = _AssayDescription;
            CPTDescriptor = _CPTDescriptor;
            Notes = _Notes;
            Locality = _Locality;
            Rate = _Rate;
        }
    }
}
