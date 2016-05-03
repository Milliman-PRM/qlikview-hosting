using ConfigIt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLSBusinessLogic
{
    public partial class BusinessLogicManager
    {

        #region Loader
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


        private bool PreloadStaticItems()
        {
            _UniqueAnalyzers = Controller.CLSController.getUniqueAnalyzers();
            _UniqueAssayDescriptions = Controller.CLSController.getUniqueSearchTerm();
            _UniqueLocalities = Controller.CLSController.getUniqueLocality();
            _FootNotes = Controller.CLSController.getUniqueFootnote();
            _WebURL = Controller.CLSController.getUniqueWeburl();
            _UniqueYears = Controller.CLSController.getUniqueYear();
            _UniqueCPTCode = Controller.CLSController.getUniqueCode();
            _DataByYear = FetchDataByYear();

            return true;
        } 
        #endregion

        #region DynamicQuery
        //dynamic datatable queries

        /// <summary>
        /// Construct the where clause section of the query
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="Values"></param>
        /// <param name="IsAdditionalWhereClause"></param>
        /// <returns></returns>
        private string WhereClauseBuilder(string FieldName, List<string> Values)
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

            return " AND (" + AllWheres + ") ";
        }

        /// <summary>
        /// Build query is set to always return rows as defined by primary window
        /// Analyzer    Assay Description              CPT Descriptor      Notes    Locality       Medicare Reimbursement Rate
        /// </summary>
        /// <param name="SchemaName"> Required to access correct data</param>
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
        private string MainTableQueryBuilder(  string SchemaName,
                                               List<string> AnalyzerNames = null, List<string> AnalyzerIDs = null, List<string> AnalyzersByCodeIDs = null,
                                               List<string> SearchTermDescs = null, List<string> SearchTermIDs = null, List<string> SearchTermByCodeIDs = null,
                                               List<string> Localaties = null, List<string> LocalatiesIDs = null, List<string> LocalatiesByDescShrt = null,
                                               List<string> Years = null,
                                               List<string> CPTCodes = null)
        {
            //error condition
            if (string.IsNullOrEmpty(SchemaName))
            {
                throw new Exception("Required schema name was not provided for table reference.");
            }

            string QueryRoot = "SELECT DISTINCT analyzers.analyzer_name, code.description, code.code, analyzers.notes, localities.locality_description, reimbursement_rates.rate FROM _SCHEMA_.analyzers, _SCHEMA_.code, _SCHEMA_.reimbursement_rates, _SCHEMA_.localities _SEARCHTERMS_ WHERE reimbursement_rates.fk_code_id = code.id AND reimbursement_rates.fk_code_id = analyzers.fk_code_id AND reimbursement_rates.fk_locality_id = localities.id _SEARCHTERMCONDITIONS_ ";
            //if the query requires search terms we need to add it as a FROM clause
            if (((SearchTermDescs != null) && (SearchTermDescs.Count > 0)) || ((SearchTermIDs != null) && (SearchTermIDs.Count > 0)) || ((SearchTermByCodeIDs != null) && (SearchTermByCodeIDs.Count > 0)))
            {
                QueryRoot = QueryRoot.Replace("_SEARCHTERMS_", ", _SCHEMA_.search_terms"); //we need search terms
                QueryRoot = QueryRoot.Replace("_SEARCHTERMCONDITIONS_", "AND reimbursement_rates.fk_code_id = search_terms.fk_code_id");
            }
            else
            {
                QueryRoot = QueryRoot.Replace("_SEARCHTERMS_", "");  //not needed so remove
                QueryRoot = QueryRoot.Replace("_SEARCHTERMCONDITIONS_", "");
            }

            //we have to add a schema to get the right instance of data
            QueryRoot = QueryRoot.Replace("_SCHEMA_", SchemaName);

            string WhereClause = string.Empty;

            if (AnalyzerNames != null)
                WhereClause += WhereClauseBuilder("analyzers.analyzer_name", AnalyzerNames);
            if (AnalyzerIDs != null)
                WhereClause += WhereClauseBuilder("analyzers.id", AnalyzerIDs);
            if (AnalyzersByCodeIDs != null)
                WhereClause += WhereClauseBuilder("analyzers.fk_code_id", AnalyzersByCodeIDs);
            if (SearchTermDescs != null)
                WhereClause += WhereClauseBuilder("search_terms.search_desc", SearchTermDescs);
            if (SearchTermIDs != null)
                WhereClause += WhereClauseBuilder("search_terms.id", SearchTermIDs);
            if (SearchTermByCodeIDs != null)
                WhereClause += WhereClauseBuilder("search_terms.fk_code_id", SearchTermByCodeIDs);
            if (Localaties != null)
                WhereClause += WhereClauseBuilder("localities.locality", Localaties);
            if (LocalatiesIDs != null)
                WhereClause += WhereClauseBuilder("localities.id", LocalatiesIDs);
            if (LocalatiesByDescShrt != null)
                WhereClause += WhereClauseBuilder("localities.locality_description", LocalatiesByDescShrt);
            if (Years != null)
                WhereClause += WhereClauseBuilder("reimbursement_rates.year", Years);
            if (CPTCodes != null)
                WhereClause += WhereClauseBuilder("code.code", CPTCodes);

            if (string.IsNullOrEmpty(WhereClause) == false)
                QueryRoot += WhereClause;

            QueryRoot += " ORDER BY analyzers.analyzer_name, code.description, localities.locality_description;";
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

        /// <summary>
        /// process out the schema name and return for use on record names
        /// </summary>
        /// <param name="Schema"></param>
        /// <returns></returns>
        private string GetConnectionString(out string Schema)
        {
            //User Id=van.nanney;Host=indy-pgsql02;Database=Roche_Medicare_Reimbursement_Develop;Integrated Security=True;Initial Schema=rmrrdb_20160304
            Schema = string.Empty;
            //string ConnString = System.Configuration.ConfigurationManager.ConnectionStrings["CLSdbDataContextConnectionString"].ConnectionString;
            string ConnString = EnvironmentSettings.ConnectionStrings["CLSdbDataContextConnectionString"].ConnectionString;
            string SchemaKey = "initial schema";
            if ( ConnString.ToLower().Contains(SchemaKey))
            {
                List<string> ConnectionStringTokens = ConnString.Split(new char[] { ';', '=' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                ConnString = string.Empty;
                for( int Index = 0; Index < ConnectionStringTokens.Count(); Index = Index+2)
                {
                    if ( string.Compare(ConnectionStringTokens[Index], SchemaKey, true ) == 0 )
                    {
                        Schema = ConnectionStringTokens[Index + 1];
                    }
                    else
                    {
                        ConnString += ConnectionStringTokens[Index] + "=" + ConnectionStringTokens[Index + 1] + ";";
                    }
                }
            }
            return ConnString;
        }
        #endregion

    }
}
