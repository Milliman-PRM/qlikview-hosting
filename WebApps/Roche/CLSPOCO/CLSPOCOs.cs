using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLSPOCO
{
    public class CLSPOCO
    {
        private static CLSPOCO _CLSDomainData = null;

        public static CLSPOCO GetCLSDomainData()
        {
            if (_CLSDomainData == null)
                _CLSDomainData = new CLSPOCO(true);

            return _CLSDomainData;
        }

        public MedicareRates RatesData { get; set; }
        public Footer        FooterData { get; set; }
        public AnalyzerSearch AnalyzerData { get; set; }
        public AssayDescriptionSearch AssayDescriptionData { get; set;  }
        public LocalitySearch LocalityData { get; set; }

        public CLSPOCO(bool Test = false )
        {
            RatesData = new MedicareRates(Test);
            FooterData = new Footer(Test);
            AnalyzerData = new AnalyzerSearch(Test);
            AssayDescriptionData = new AssayDescriptionSearch(Test);
            LocalityData = new LocalitySearch(Test);
        }

    }
}
