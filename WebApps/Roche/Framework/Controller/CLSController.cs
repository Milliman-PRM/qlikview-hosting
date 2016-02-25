using CLSdbContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controller
{
    public class CLSController
    {
        public CLSController()
        {

        }

        #region Code

        public static List<Code> getUniqueCode()
        {
            var objList = new List<Code>();
            using (CLSdbDataContext context = new CLSdbDataContext())
            {
                objList = (from aEntity in context.Codes
                           select new
                           { // select only columns you need
                               aEntity.Id,
                               aEntity.Code1
                           })
                            .AsEnumerable() // execute query
                            .Select(x => new Code
                            { // create instance of class
                                Id = x.Id,
                                Code1 = x.Code1
                            })
                            .Distinct().ToList();

            }

            return objList;
        }
        public static List<Code> getUniqueCodeByCodeName()
        {
            var context = new CLSdbDataContext();
            var dboList = context.Codes.Distinct().ToList();
            var resultList = dboList.GroupBy(x => x.Code1).Select(y => y.First()).ToList();
            return resultList;
        }
        #endregion

        #region Localitites

        public static List<Locality> getUniqueLocality()
        {
            var context = new CLSdbDataContext();
            var dboList = context.Localities.Distinct().ToList();
            var resultList = dboList.GroupBy(x => x.Locality1).Select(y => y.First()).ToList();
            return resultList;
        }

        #endregion

        #region Footnote

        public static List<Footnote> getUniqueFootnote()
        {
            var context = new CLSdbDataContext();
            var dboList = context.Footnotes.Distinct().ToList();
            var resultList = dboList.GroupBy(x => x.Footnote1).Select(y => y.First()).ToList();
            return resultList;
        }

        #endregion

        #region Weburl

        public static List<Weburl> getUniqueWeburl()
        {
            var context = new CLSdbDataContext();
            var dboList = context.Weburls.Distinct().ToList();
            var resultList = dboList.GroupBy(x => x.Webaddressurl).Select(y => y.First()).ToList();
            return resultList;
        }

        #endregion

        #region Analyzer
        /// <summary>
        /// Returns list of unique analizers name from analyzer table
        /// </summary>
        /// <returns></returns>
        public static List<Analyzer> getUniqueAnalyzers()
        {
            var context = new CLSdbDataContext();
            var dboList = context.Analyzers.Distinct().ToList();
            var resultList = dboList.GroupBy(x => x.AnalyzerName).Select(y => y.First()).ToList();
            return resultList;
        }

        /// <summary>
        /// Returns list of unique analyzerNames from analyzer table
        /// </summary>
        /// <returns></returns>
        public static List<Analyzer> getUniqueAnalyzerNames()
        {
            var context = new CLSdbDataContext();
            var dboList = context.Analyzers.Distinct().ToList();
            var resultList = dboList.GroupBy(x => x.AnalyzerName).Select(y => y.First()).ToList();
            return resultList;
        }

        /// <summary>
        /// Returns the list of search term for a specific analyzer id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<SearchTerm> getSearchTermsForAnalyzerName(int param)
        {
            var context = new CLSdbDataContext();

            ////Query the analyzer table and get all rows where "analyzer_name" == AnalyzerName
            //var analyzer = context.Analyzers.Where(a => a.AnalyzerName == param);

            //var resultList = new List<SearchTerm>();
            ////for each row item returned in query the searchterms table using the "fk_code_id"
            ////for each row item returned from searchterm add to return list of search terms
            //foreach (var a in analyzer)
            //{
            //    var searchTermsforAnalyzer = context.SearchTerms.Where<SearchTerm>(s => s.FkCodeId == a.FkCodeId).ToList();
            //    foreach (var s in searchTermsforAnalyzer)
            //    {
            //        var searhTerm = context.SearchTerms.Select(sa => sa.FkCodeId == s.FkCodeId);
            //        var searchTermEntity = new SearchTerm
            //        {
            //            Id = s.Id,
            //            Code = s.Code,
            //            FkCodeId = s.FkCodeId,
            //            SearchDesc = s.SearchDesc
            //        };
            //        resultList.Add(searchTermEntity);
            //    }
            //}
            
            IQueryable<Analyzer> analyzer = context.Analyzers;
            IQueryable<SearchTerm> searchTerms = context.SearchTerms;
            var query = from s in searchTerms
                        join a in analyzer on s.FkCodeId equals a.FkCodeId
                        where a.Id == param
                        select s;

            var resultList = new List<SearchTerm>();
            resultList = query.ToList();
            return resultList;
        }

        public static void SaveAnalyzers(Analyzer obj)
        {
            var context = new CLSdbDataContext();
            if (obj != null)
            {
                var codeObj = new Code();
                codeObj.Code1 = "Test1";

                context.Codes.InsertOnSubmit(codeObj);
                context.SubmitChanges();

                context = new CLSdbDataContext();
                context.Analyzers.InsertOnSubmit(obj);
                context.SubmitChanges();
            }

        }
        #endregion

        #region ReimbursementRate

        public static List<ReimbursementRate> getUniqueReimbursementRate()
        {
            var context = new CLSdbDataContext();
            var dboList = context.ReimbursementRates.Distinct().ToList();
            var resultList = dboList.GroupBy(g => g.Rate).Select(y => y.First()).ToList();
            return resultList;
        }
        public static List<ReimbursementRate> getAllReimbursementRates()
        {
            var context = new CLSdbDataContext();
            var dboList = context.ReimbursementRates.Distinct().ToList();
            var resultList = dboList.OrderBy(o => o.Year).ToList();
            return resultList;
        }

        /// <summary>
        /// Returns list of Year
        /// </summary>
        /// <returns></returns>
        public static List<string> getUniqueYear()
        {
            //Query the reimbursmenet_rate table and return a list of unquie year values
            var context = new CLSdbDataContext();
            var dboList = context.ReimbursementRates.GroupBy(a => a.Year)
                                  .Select(x => new
                                  {
                                      Year = x.Key.Value.ToString()
                                  }).ToList();

            var resultList = new List<string>(dboList.Select(s => s.Year.ToString()));
            return resultList;
        }
        #endregion

        #region Search Terms

        public static List<SearchTerm> getUniqueSearchTerm()
        {
            var context = new CLSdbDataContext();
            var dboList = context.SearchTerms.Distinct().ToList();
            var resultList = dboList.GroupBy(x => x.SearchDesc).Select(y => y.First()).ToList();
            return resultList;
        }
        public static List<SearchTerm> getAssayDescriptionForSpecificAnalyzer(int param)
        {
            var context = new CLSdbDataContext();
            var dboList = context.SearchTerms.Distinct().ToList();
            var resultList = dboList.Where(a => a.FkCodeId == param).ToList();
            return resultList;
        }
        public static List<Analyzer> getAnalyzerForSpecificAssayDescription(string param)
        {
            var context = new CLSdbDataContext();

            IQueryable<Analyzer> analyzer = context.Analyzers;
            IQueryable<SearchTerm> searchTerms = context.SearchTerms;
            var query = from a in analyzer
                        join s in searchTerms on a.FkCodeId equals s.FkCodeId
                        where s.SearchDesc == param.ToString()
                        select a;

            var result = new List<Analyzer>();
            result = query.ToList();
            return result;
        }

        #endregion

        public static int? getEverything(string param)
        {
            var context = new CLSdbDataContext();
            var everyThing =
                                    from r in context.ReimbursementRates
                                    join c in context.Codes on r.FkCodeId equals c.Id
                                    join s in context.SearchTerms on c.Id equals s.FkCodeId
                                    join a in context.Analyzers on s.FkCodeId equals a.FkCodeId
                                    join l in context.Localities on r.FkLocalityId equals l.Id
                                    where r.Year.Value == int.Parse(param.ToString())
                                    select new
                                    {
                                        r.Rate,
                                        r.Year,
                                        r.Code.Code1,
                                        s.SearchDesc,
                                        a.AnalyzerName,
                                        a.Code,
                                        l.Locality1
                                    };


            var analyzers = from a in everyThing
                            select a.AnalyzerName;

            var locality = from l in everyThing
                           select l.Locality1;

            var codes = from c in everyThing
                        select c.Code.Code1;

            var searchTerms = from s in everyThing
                              select s.SearchDesc;

            var reimRates = from r in everyThing
                            select r.Rate;

            return everyThing.Count();
        }
        
    }
}
