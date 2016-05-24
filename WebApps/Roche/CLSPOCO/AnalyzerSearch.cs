using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLSPOCO
{
    public class AnalyzerSearch
    {
        private List<string> _UniqueAnalyzers;

        public List<string> UniqueAnalyzers
        {
            get
            {
                return _UniqueAnalyzers;
            }

            set
            {
                _UniqueAnalyzers = value;
            }
        }

        public AnalyzerSearch(bool Test = false)
        {
            Test = false;
            _UniqueAnalyzers = new List<string>();
            if ( Test == false  )
            {
                //_UniqueAnalyzers = CLSBusinessLogic.BusinessLogicManager.GetInstance().UniqueAnalyzers;
            }
            else
            {
                _UniqueAnalyzers.Add("AVL 9181");
                _UniqueAnalyzers.Add("COBAS 4000 A");
                _UniqueAnalyzers.Add("COBAS 6000 A");
                _UniqueAnalyzers.Add("cobas b 123");
                _UniqueAnalyzers.Add("cobas b 221 blood gas system");
                _UniqueAnalyzers.Add("cobas c 502");
                _UniqueAnalyzers.Add("cobas c 701-702");
                _UniqueAnalyzers.Add("INTEGRA 400 A");
                _UniqueAnalyzers.Add("Roche_Hitachi Modular");
                _UniqueAnalyzers.Add("COBAS 4000 B");
                _UniqueAnalyzers.Add("COBAS 4800");
                _UniqueAnalyzers.Add("COBAS 6000 B");
                _UniqueAnalyzers.Add("cobas e 602");
                _UniqueAnalyzers.Add("COBAS Liat System");
                _UniqueAnalyzers.Add("Elecsys 2010");
                _UniqueAnalyzers.Add("Elecsys E170 Modular");
                _UniqueAnalyzers.Add("INTEGRA 400 b");
                _UniqueAnalyzers.Add("Multichannel");
                _UniqueAnalyzers.Add("Organ-Disease Panels");
                _UniqueAnalyzers.Add("PCR Based MDx Inf Disease");
                _UniqueAnalyzers.Add("PCR Based MDx Microbiology");
                _UniqueAnalyzers.Add("T1 Molecular Pathology Assays");
            }
        }
    }
}
