using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CLSdbContext;

namespace CLSBusinessLogic
{
    //factory object used to hold instances of data that rarely change

    public class BusinessLogicManager
    {
        private static bool NEVER_CACHE = false;

        private static BusinessLogicManager instance;
        private static object instance_lock = new object();
        public static BusinessLogicManager GetInstance()
        {
            lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
            {
                //for dev version always load fresh, don't cache
                if (NEVER_CACHE)
                {
                instance = null;
                instance = Load();
                }
                else
                {
                    //production mode cache static result sets
                    if (instance == null)
                        instance = Load();
                }
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

        private System.Data.DataTable _AllData;
        public DataTable AllData
        {
            get
            {
                return _AllData;
            }

            set
            {
                _AllData = value;
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


        private bool PreloadStaticItems()
        {
            _UniqueAnalyzers = Controller.CLSController.getUniqueAnalyzers();
            _UniqueAssayDescriptions = Controller.CLSController.getUniqueSearchTerm();
            _UniqueLocalities = Controller.CLSController.getUniqueLocality();
            _FootNotes = Controller.CLSController.getUniqueFootnote();
            _WebURL = Controller.CLSController.getUniqueWeburl();
            _UniqueYears = Controller.CLSController.getUniqueYear();

            _AllData = FetchAllData();
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

        //by default we request all data and store it, such that all users will have the page populated the first time with all 90,000
        public System.Data.DataTable FetchAllData()
        {
            int ExecuteTimeSeconds = 0;
            int DataTableSizeMB = 0;
            return DynamicDataTable(GetConnectionString(), MainTableQueryBuilder(), out ExecuteTimeSeconds, out DataTableSizeMB);
        }

        #region DynamicQuery
        //dynamic datatable queries

        /// <summary>
        /// Construct the where clause section of the query
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="Values"></param>
        /// <param name="IsAdditionalWhereClause"></param>
        /// <returns></returns>
        private string WhereClauseBuilder(string FieldName, List<string> Values, bool IsAdditionalWhereClause)
        {
            if ((Values == null) || (Values.Count == 0))
                return "";
            string WhereTemplate = "_FIELD_ = '_VALUE_'";
            WhereTemplate = WhereTemplate.Replace("_FIELD_", FieldName);
            string AllWheres = string.Empty;
            foreach (string Value in Values)
            {
                if (string.IsNullOrEmpty(AllWheres) == false)
                    AllWheres += " OR ";
                AllWheres += WhereTemplate.Replace("_VALUE_", Value);
            }
            string PrefixedAnd = " AND ";
            if (IsAdditionalWhereClause == false)
                PrefixedAnd = string.Empty;
            return PrefixedAnd + AllWheres + " ";
        }

        /// <summary>
        /// Build query is set to always return rows as defined by primary window
        /// Analyzer    Assay Description              CPT Descriptor      Notes    Locality       Medicare Reimbursement Rate
        /// </summary>
        /// <param name="AnalyzerNames"></param>
        /// <param name="AnalyzerIDs"></param>
        /// <param name="AnalyzersByCodeIDs"></param>
        /// <param name="SearchTermDescs"></param>
        /// <param name="SearchTermIDs"></param>
        /// <param name="SearchTermByCodeIDs"></param>
        /// <param name="Localaties"></param>
        /// <param name="LocalatiesIDs"></param>
        /// <param name="LocalatiesByDescShrt"></param>
        /// <param name="Years"></param>
        /// <param name="CPTCodes"></param>
        /// <returns></returns>
        private string MainTableQueryBuilder(List<string> AnalyzerNames = null, List<string> AnalyzerIDs = null, List<string> AnalyzersByCodeIDs = null,
                                               List<string> SearchTermDescs = null, List<string> SearchTermIDs = null, List<string> SearchTermByCodeIDs = null,
                                               List<string> Localaties = null, List<string> LocalatiesIDs = null, List<string> LocalatiesByDescShrt = null,
                                               List<string> Years = null,
                                               List<string> CPTCodes = null)
        {
            //string QueryRoot = "select analyzers.analyzer_name, code.description, code.code, analyzers.notes, localities.locality_desc_shrt, reimbursement_rates.rate from rmrrdb_20160222.reimbursement_rates INNER JOIN rmrrdb_20160222.analyzers ON analyzers.fk_code_id = reimbursement_rates.fk_code_id INNER JOIN rmrrdb_20160222.code ON code.id = reimbursement_rates.fk_code_id INNER JOIN rmrrdb_20160222.localities ON localities.id = reimbursement_rates.fk_locality_id ";
            string QueryRoot = "SELECT analyzers.analyzer_name, code.description, code.code, analyzers.notes, localities.locality_desc_shrt, reimbursement_rates.rate FROM rmrrdb_20160222.analyzers, rmrrdb_20160222.code, rmrrdb_20160222.reimbursement_rates, rmrrdb_20160222.localities WHERE reimbursement_rates.fk_code_id = code.id AND reimbursement_rates.fk_code_id = analyzers.fk_code_id AND reimbursement_rates.fk_locality_id = localities.id ";
            string WhereClause = string.Empty;

            if (AnalyzerNames != null)
                WhereClause += WhereClauseBuilder("analyzers.analyzer_name", AnalyzerNames, !string.IsNullOrEmpty(WhereClause));
            if (AnalyzerIDs != null)
                WhereClause += WhereClauseBuilder("analyzers.id", AnalyzerIDs, !string.IsNullOrEmpty(WhereClause));
            if (AnalyzersByCodeIDs != null)
                WhereClause += WhereClauseBuilder("analyzers.fk_code_id", AnalyzersByCodeIDs, !string.IsNullOrEmpty(WhereClause));
            if (SearchTermDescs != null)
                WhereClause += WhereClauseBuilder("search_terms.search_desc", SearchTermDescs, !string.IsNullOrEmpty(WhereClause));
            if (SearchTermIDs != null)
                WhereClause += WhereClauseBuilder("search_terms.id", SearchTermIDs, !string.IsNullOrEmpty(WhereClause));
            if (SearchTermByCodeIDs != null)
                WhereClause += WhereClauseBuilder("search_terms.fk_code_id", SearchTermByCodeIDs, !string.IsNullOrEmpty(WhereClause));
            if (Localaties != null)
                WhereClause += WhereClauseBuilder("localities.locality", Localaties, !string.IsNullOrEmpty(WhereClause));
            if (LocalatiesIDs != null)
                WhereClause += WhereClauseBuilder("localities.id", LocalatiesIDs, !string.IsNullOrEmpty(WhereClause));
            if (LocalatiesByDescShrt != null)
                WhereClause += WhereClauseBuilder("localities.locality_desc_shrt", LocalatiesByDescShrt, !string.IsNullOrEmpty(WhereClause));
            if (Years != null)
                WhereClause += WhereClauseBuilder("reimbursement_rates.year", Years, !string.IsNullOrEmpty(WhereClause));
            if (CPTCodes != null)
                WhereClause += WhereClauseBuilder("code.code", CPTCodes, !string.IsNullOrEmpty(WhereClause));

            if (string.IsNullOrEmpty(WhereClause) == false)
                QueryRoot += WhereClause;

            QueryRoot += " ORDER BY analyzers.analyzer_name, code.description, localities.locality_desc_shrt;";
            return QueryRoot;
        }
        /// <summary>
        /// Get the data from the DB server in a dynamic data table
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="Query"></param>
        /// <returns></returns>
        private System.Data.DataTable DynamicDataTable(string ConnectionString, string Query, out int ExecuteInSeconds, out int DataTableSizeMB)
        {
            try
            {
                ExecuteInSeconds = 0;
                DataTableSizeMB = 0;

                DateTime Start = DateTime.Now;
                System.Data.DataTable dt = new System.Data.DataTable();
                System.Data.DataSet ds = new System.Data.DataSet();

                Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(ConnectionString);
                conn.Open();

                Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter(Query, conn);
                ds.Reset();
                da.Fill(ds);
                dt = ds.Tables[0];
                conn.Close();

                DateTime Stop = DateTime.Now;
                ExecuteInSeconds = ((Stop - Start).Minutes * 60) + (Stop - Start).Seconds;
                //check size, this can be removed in future will speed things a up a little
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, dt);
                stream.Seek(0, 0);
                byte[] result = stream.ToArray();
                stream.Close();
                DataTableSizeMB = (int)Math.Ceiling((double)result.Length / (double)1000000);
                result = null;  //get rid of it, no longer needed
                return dt;

            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                throw;
            }

        }

        private string GetConnectionString()
        {
            string ConnString = System.Configuration.ConfigurationManager.ConnectionStrings["CLSdbDataContextConnectionStringNOSCHEMA"].ConnectionString;
            return ConnString;
        } 
        #endregion
  

    }
}
