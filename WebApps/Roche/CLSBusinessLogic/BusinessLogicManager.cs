using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLSdbContext;

namespace CLSBusinessLogic
{
    //factory object used to hold instances of data that rarely change

    public class BusinessLogicManager
    {
        private static BusinessLogicManager instance;
        private static object instance_lock = new object();
        public static BusinessLogicManager GetInstance()
        {
            lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
            {
                //for dev version always load fresh, don't cache
                instance = null;
                instance = Load();

                //if (instance == null)
                //    instance = Load();
                return instance;
            }
        }
        public static void KillInstance()
        {
            lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
            {
                if (instance != null)
                    instance = null;
            }
        }
        //end singleton

        public BusinessLogicManager()
        { }

        public static BusinessLogicManager Load()
        {
            BusinessLogicManager BLM = new BusinessLogicManager();
            BLM.PreloadStaticItems();

            return BLM;
        }

        private List<CLSdbContext.Analyzer> _UniqueAnalyzers;
        public List<CLSdbContext.Analyzer> UniqueAnalyzers
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

        private List<CLSdbContext.SearchTerm> _UniqueAssayDescriptions;
        public List<SearchTerm> UniqueAssayDescriptions
        {
            get
            {
                return _UniqueAssayDescriptions;
            }

            set
            {
                _UniqueAssayDescriptions = value;
            }
        }

        private List<CLSdbContext.Locality> _UniqueLocalities;
        public List<Locality> UniqueLocalities
        {
            get
            {
                return _UniqueLocalities;
            }

            set
            {
                _UniqueLocalities = value;
            }
        }

        private List<CLSdbContext.Footnote> _FootNotes;
        public List<Footnote> FootNotes
        {
            get
            {
                return _FootNotes;
            }

            set
            {
                _FootNotes = value;
            }
        }

        private List<CLSdbContext.Weburl> _WebURL;
        public List<CLSdbContext.Weburl> WebURL
        {
            get
            {
                return _WebURL;
            }

            set
            {
                _WebURL = value;
            }
        }

        private List<CLSdbContext.ReimbursementRate> _ReimbursementRate;
        public List<CLSdbContext.ReimbursementRate> ReimbursementRate
        {
            get
            {
                return _ReimbursementRate;
            }

            set
            {
                _ReimbursementRate = value;
            }
        }

        private List<CLSdbContext.ReimbursementRate> _ReimbursementRatesAllData;
        public List<ReimbursementRate> ReimbursementRatesAllData
        {
            get
            {
                return _ReimbursementRatesAllData;
            }

            set
            {
                _ReimbursementRatesAllData = value;
            }
        }

        private List<string> _Year;
        public List<string> Year
        {
            get
            {
                return _Year;
            }

            set
            {
                _Year = value;
            }
        }
                
        private bool PreloadStaticItems()
        {
            _UniqueAnalyzers = Controller.CLSController.getUniqueAnalyzers();
            _UniqueAssayDescriptions = Controller.CLSController.getUniqueSearchTerm();
            _UniqueLocalities = Controller.CLSController.getUniqueLocality();
            _FootNotes = Controller.CLSController.getUniqueFootnote();
            _WebURL = Controller.CLSController.getUniqueWeburl();
            _ReimbursementRate = Controller.CLSController.getUniqueReimbursementRate();
            _ReimbursementRatesAllData = Controller.CLSController.getAllReimbursementRates();
            _Year = Controller.CLSController.getUniqueYear();
            return true;
        }

        public List<SearchTerm> FindAssayDescriptionForAnalyzer(List<string> AnalyzerIDs)
        {
            List<SearchTerm> Terms = new List<SearchTerm>();
            foreach (string AnalyzerID in AnalyzerIDs)
            {
                var codeID = System.Convert.ToInt32(AnalyzerID);
                List<SearchTerm> Results = Controller.CLSController.getSearchTermsForAnalyzerName(codeID);
                if (Results != null)
                    Terms.AddRange(Results);
            }
            return Terms;
        }
    }
}
