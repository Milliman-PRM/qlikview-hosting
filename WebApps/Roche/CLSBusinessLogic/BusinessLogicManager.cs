using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CLSdbContext;

namespace CLSBusinessLogic
{
    //factory object used to hold instances of data that rarely change

    public partial class BusinessLogicManager
    {

        //this class is maintained via reference in the session an contains all the user selections to date
        public class CurrentSelections
        {
            public enum QueryFieldNames { ALL, ANALYZERNAMES, ANALYZERIDS, ANALYZERSBYCODEID, SEARCHTERMDESCS, SEARCHTERMIDS, SEARCHTERMBYCODEIDS, LOCALATIES, LOCALATIESIDS, LOCALATIESBYDESCSHRT, YEARS, CPTCODES }
            public List<string> AnalyzerNames { get; set; }
            public List<string> AnalyzerIDs { get; set; }
            public List<string> AnalyzersByCodeIDs { get; set; }
            public List<string> SearchTermDescs { get; set; }
            public List<string> SearchTermIDs { get; set; }
            public List<string> SearchTermByCodeIDs { get; set; }
            public List<string> Localaties { get; set; }
            public List<string> LocalatiesIDs { get; set; }
            public List<string> LocalatiesByDescShrt { get; set; }
            public List<string> Years { get; set; }
            public List<string> CPTCodes { get; set; }

            public CurrentSelections()
            {
                AnalyzerNames = new List<string>();
                AnalyzerIDs = new List<string>();
                AnalyzersByCodeIDs = new List<string>();
                SearchTermDescs = new List<string>();
                SearchTermIDs = new List<string>();
                SearchTermByCodeIDs = new List<string>();
                Localaties = new List<string>();
                LocalatiesIDs = new List<string>();
                LocalatiesByDescShrt = new List<string>();
                Years = new List<string>();
                CPTCodes = new List<string>();
            }
            private List<string> FindListByFieldName(QueryFieldNames FieldName)
            {
                switch (FieldName)
                {
                    case QueryFieldNames.ALL: return null;
                    case QueryFieldNames.ANALYZERIDS: return AnalyzerIDs;
                    case QueryFieldNames.ANALYZERNAMES: return AnalyzerNames;
                    case QueryFieldNames.ANALYZERSBYCODEID: return AnalyzersByCodeIDs;
                    case QueryFieldNames.CPTCODES: return CPTCodes;
                    case QueryFieldNames.LOCALATIES: return Localaties;
                    case QueryFieldNames.LOCALATIESBYDESCSHRT: return LocalatiesByDescShrt;
                    case QueryFieldNames.LOCALATIESIDS: return LocalatiesIDs;
                    case QueryFieldNames.SEARCHTERMBYCODEIDS: return SearchTermByCodeIDs;
                    case QueryFieldNames.SEARCHTERMDESCS: return SearchTermDescs;
                    case QueryFieldNames.SEARCHTERMIDS: return SearchTermIDs;
                    case QueryFieldNames.YEARS: return Years;
                }
                return null;
            }
            public void Clear(QueryFieldNames FieldName)
            {
                if (FieldName == QueryFieldNames.ALL)
                {
                    AnalyzerIDs.Clear();
                    AnalyzerNames.Clear();
                    AnalyzersByCodeIDs.Clear();
                    CPTCodes.Clear();
                    Localaties.Clear();
                    LocalatiesByDescShrt.Clear();
                    LocalatiesIDs.Clear();
                    SearchTermByCodeIDs.Clear();
                    SearchTermDescs.Clear();
                    SearchTermIDs.Clear();
                    // Years.Clear();  never clear years, there is always one set
                }
                else
                {
                    FindListByFieldName(FieldName).Clear();
                }
            }

            public void AddToList(QueryFieldNames FieldName, string Value)
            {
                if (FieldName != QueryFieldNames.ALL)
                {
                    List<string> QueryList = FindListByFieldName(FieldName);
                    if (QueryList.Contains(Value) == false)
                        QueryList.Add(Value);
                }
            }

            /// <summary>
            /// return true if no selections have been made,  this routine
            /// does not take into account YEAR, it is always selected
            /// </summary>
            /// <returns></returns>
            public bool NoSelectionsMade()
            {
                return ((AnalyzerIDs.Count() == 0) &&
                (AnalyzerNames.Count() == 0) &&
                (AnalyzersByCodeIDs.Count() == 0) &&
                (CPTCodes.Count() == 0) &&
                (Localaties.Count() == 0) &&
                (LocalatiesByDescShrt.Count() == 0) &&
                (LocalatiesIDs.Count() == 0) &&
                (SearchTermByCodeIDs.Count() == 0) &&
                (SearchTermDescs.Count() == 0) &&
                (SearchTermIDs.Count() == 0));
            }
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

        private List<Code> _UniqueCPTCode;
        public List<Code> UniqueCPTCode
        {
            get
            {
                return _UniqueCPTCode;
            }

            set
            {
                _UniqueCPTCode = value;
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

        private Dictionary<string, System.Data.DataTable> _DataByYear;
        public Dictionary<string, DataTable> DataByYear
        {
            get
            {
                return _DataByYear;
            }

            set
            {
                _DataByYear = value;
            }
        }

        private List<string> _UniqueYears;
        public List<string> UniqueYears
        {
            get
            {
                return _UniqueYears;
            }

            set
            {
                _UniqueYears = value;
            }
        }

        public List<SearchTerm> FindAssayDescriptionForAnalyzer(List<string> AnalyzerIDs)
        {
            string AnalyzerIDList = "";
            foreach (string AnalyzerID in AnalyzerIDs)
            {
                if (AnalyzerIDList.Length > 0)
                    AnalyzerIDList += ",";
                AnalyzerIDList += AnalyzerID;
            }

            return Controller.CLSController.getSearchTermsForSpecificAnalyzerIdListArray(AnalyzerIDList);
        }

        public List<Code> FindCodesForAnalyzer(List<string> analyzerIds)
        {
            string idList = "";
            foreach (var item in analyzerIds)
            {
                if (idList.Length > 0)
                    idList += ",";
                idList += analyzerIds;
            }

            return Controller.CLSController.getCodesForSpecificAnalyzerIdListArray(idList);
        }

        //we are going to cache each years's full data set once, that way all users will have fast access to all data per year
        public Dictionary<string, System.Data.DataTable> FetchDataByYear()
        {
            Dictionary<string, System.Data.DataTable> DataByYear = new Dictionary<string, DataTable>();
            int ExecuteTimeSeconds = 0;
            int DataTableSizeMB = 0;
            string Schema = string.Empty;
            string ConnectionString = GetConnectionString(out Schema);

            List<string> UniqueYearList = Controller.CLSController.getUniqueYear();
            foreach (string Year in UniqueYearList)
            {
                List<string> YearAsList = new List<string>() { Year };
                System.Data.DataTable ThisYearsData = DynamicDataTable(ConnectionString, MainTableQueryBuilder(Schema, null, null, null, null, null, null, null, null, null, YearAsList), out ExecuteTimeSeconds, out DataTableSizeMB);
                DataByYear.Add(Year, ThisYearsData);
            }
            return DataByYear;

        }

        /// <summary>
        /// call to get a new data set that matches the user selections
        /// </summary>
        /// <param name="Selections"></param>
        /// <returns></returns>
        public System.Data.DataTable FetchDataForSelections(CurrentSelections Selections)
        {
            int ExecuteTimeSeconds = 0;
            int DataTableSizeMB = 0;
            string Schema = string.Empty;
            string ConnectionString = GetConnectionString(out Schema);
            return DynamicDataTable(ConnectionString, MainTableQueryBuilder(Schema, Selections.AnalyzerNames, Selections.AnalyzerIDs, Selections.AnalyzersByCodeIDs,
                                                                                    Selections.SearchTermDescs, Selections.SearchTermIDs, Selections.SearchTermByCodeIDs,
                                                                                    Selections.Localaties, Selections.LocalatiesIDs, Selections.LocalatiesByDescShrt,
                                                                                    Selections.Years, Selections.CPTCodes), out ExecuteTimeSeconds, out DataTableSizeMB);
        }

        public List<string> FindAnalyzersForAssayDescription(string AssayDescription)
        {
            return Controller.CLSController.getAnalyzerNamesListForSpecificSearchTermDesc(AssayDescription);
        }

        public string FindAnalyzerIDFromName(string AnalyzerName)
        {
            foreach (CLSdbContext.Analyzer Analyze in _UniqueAnalyzers)
            {
                if (string.Compare(Analyze.AnalyzerName, AnalyzerName, true) == 0)
                    return Analyze.FkCodeId.ToString();
            }
            return "";
        }

        public List<string> FindAnalyzerIDsFromName(string AnalyzerName)
        {
            return Controller.CLSController.getAnalyzerIdsForSpecificAnalyzerName(AnalyzerName);
        }

        public string FindLocalityByID(string LocalityID)
        {
            foreach (Locality L in _UniqueLocalities)
            {
                if (L.Id == System.Convert.ToInt32(LocalityID))
                    return L.LocalityDescription;
            }
            return "";
        }

        public List<Code> FindCptCodesForAnalyzer(List<string> analyzerIds)
        {
            string newIdList = "";
            foreach (var item in analyzerIds)
            {
                if (newIdList.Length > 0)
                    newIdList += ",";
                newIdList += item;
            }

            return Controller.CLSController.getCptCodesListForAnalyzersIdListArray(newIdList);
        }

        public List<Code> GetCptCodeListForAssayDescription(string assayDesc)
        {
            return Controller.CLSController.getCptCodeListForAssayDescription(assayDesc);
        }
        public List<SearchTerm> GetAssayDescriptionListForCptCode(string cptCode)
        {
            return Controller.CLSController.getAssayDescriptionListForSpecificCptCode(cptCode);
        }
        public List<Analyzer> GetAnalyzerListForCptCode(string cptCode)
        {
            return Controller.CLSController.getAnalyzerNamesListForSpecificCptCode(cptCode);
        }
    }
}
