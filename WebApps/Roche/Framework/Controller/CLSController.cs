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
            var resultList = dboList.GroupBy(x => x.Code1).Select(y => y.First()).OrderBy(a=>a.Code1).ToList();
            return resultList;
        }
        #endregion

        #region Localitites

        public static List<Locality> getUniqueLocality()
        {
            var context = new CLSdbDataContext();
            var dboList = context.Localities.Distinct().ToList();
            var resultList = dboList.GroupBy(x => x.Locality1).Select(y => y.First()).OrderBy(a=>a.LocalityDescription).ToList();
            var finallist = resultList.Where(l => !string.IsNullOrEmpty(l.LocalityDescription))
                                    .OrderByDescending(x => x.LocalityDescription == "National Limit").ToList();
            return finallist;
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
            var resultList = dboList.GroupBy(x => x.AnalyzerName).Select(y => y.First()).OrderBy(a=>a.AnalyzerName).ToList();
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
            var resultList = dboList.GroupBy(x => x.AnalyzerName).Select(y => y.First()).OrderBy(a=>a.AnalyzerName).ToList();
            return resultList;
        }

        /// <summary>
        /// Returns the list of search term for a specific analyzer Name
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<SearchTerm> getSearchTermsForSpecificAnalyzerName(string param)
        {
            var context = new CLSdbDataContext();
            IQueryable<Analyzer> analyzer = context.Analyzers;
            IQueryable<SearchTerm> searchTerms = context.SearchTerms;
            var query = from s in searchTerms
                        join a in analyzer on s.FkCodeId equals a.FkCodeId
                        where a.AnalyzerName == param
                        select s;

            var resultList = new List<SearchTerm>();
            resultList = query.ToList();
            //sort list by alphabatic order then by number
            var orderedListByAlphabatsThenNumbers = resultList.OrderByDescending(x => x.SearchDesc.All(char.IsLetter))
                                                    .ThenByDescending(x => x.SearchDesc.Any(char.IsDigit))
                                                    .ToList();
            return orderedListByAlphabatsThenNumbers;
        }

        /// <summary>
        /// Returns the list of search term for a specific analyzer id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<SearchTerm> getSearchTermsForSpecificAnalyzerId(int param)
        {
            var context = new CLSdbDataContext();
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

        /// <summary>
        /// Returns the list of search term for a specific analyzer id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<SearchTerm> getSearchTermsForSpecificAnalyzerIdListArray(string param)
        {
            var context = new CLSdbDataContext();
            IQueryable<Analyzer> analyzer = context.Analyzers;
            IQueryable<SearchTerm> searchTerms = context.SearchTerms;

            //split ids
            var splittedIds = param.Split(',').Select(str => int.Parse(str));
            #region
            //var query = (from s in searchTerms
            //             join a in analyzer on s.FkCodeId equals a.FkCodeId
            //             where splittedIds.Contains(a.Id)
            //             select s);
            #endregion
            var query = analyzer                                // starting point - table in the "from" statement
                       .Join(searchTerms,                       // the source table of the inner join
                                  azTble => azTble.FkCodeId,     // Select the primary key (the first part of the "on" clause in an sql "join" statement)
                                  stTble => stTble.FkCodeId,     // Select the foreign key (the second part of the "on" clause)
                                  (azTble, stTble) =>
                                      new {   Az = azTble,  St = stTble }
                                   )    // selection
                       .Where(sa => splittedIds.Contains(sa.Az.Id)) // where statement
                       .OrderBy(a=>a.St.SearchDesc)
                       .Select(a=>a.St);    // seelct record set

            var resultList = new List<SearchTerm>();
            resultList = query.ToList();
            //sort list by alphabatic order then by number
            var orderedListByAlphabatsThenNumbers = resultList.OrderByDescending(x => x.SearchDesc.All(char.IsLetter))
                                                    .ThenByDescending(x => x.SearchDesc.Any(char.IsDigit))
                                                    .ToList();
            return orderedListByAlphabatsThenNumbers;
        }

        public static List<string> getAnalyzerIdsForSpecificAnalyzerName(string param)
        {
            //Query the reimbursmenet_rate table and return a list of unquie year values
            var context = new CLSdbDataContext();
            var dboList = context.Analyzers.Where(a=>a.AnalyzerName == param)
                                  .Select(x => new
                                  {
                                      AnalyzerIds = x.Id.ToString()
                                  }).ToList();

            var resultList = new List<string>(dboList.Select(s => s.AnalyzerIds.ToString()));
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
            var resultList = dboList.GroupBy(x => x.SearchDesc).Select(y => y.First()).OrderBy(a => a.SearchDesc).ToList();
            var finallist = resultList.Where(l => !string.IsNullOrEmpty(l.SearchDesc)).ToList();
            //sort list by alphabatic order then by number
            var orderedListByAlphabatsThenNumbers = finallist.OrderByDescending(x => x.SearchDesc.All(char.IsLetter))
                                                .ThenByDescending(x => x.SearchDesc.Any(char.IsDigit))
                                                .ToList();
            return orderedListByAlphabatsThenNumbers;
        }
        public static List<SearchTerm> getAssayDescriptionForSpecificCodeId(int param)
        {
            var context = new CLSdbDataContext();
            var dboList = context.SearchTerms.Distinct().ToList();
            var resultList = dboList.Where(a => a.FkCodeId == param).OrderBy(a=>a.SearchDesc).ToList();
            return resultList;
        }

        /// <summary>
        /// Returns list of Analyzers as an object for specific search term name
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<Analyzer> getAnalyzerForSpecificSearchTermDesc(string param)
        {
            var context = new CLSdbDataContext();
            IQueryable<Analyzer> analyzer = context.Analyzers;
            IQueryable<SearchTerm> searchTerms = context.SearchTerms;
            var query = from a in analyzer
                        join s in searchTerms on a.FkCodeId equals s.FkCodeId
                        where s.SearchDesc == param
                        select a;

            var resultList = new List<Analyzer>();
            resultList = query.ToList();
            return resultList;
        }

        /// <summary>
        /// Returns list of Analyzer for specific search term desc. The list is retured as string not an object
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<string> getAnalyzerNamesListForSpecificSearchTermDesc(string param)
        {
            var context = new CLSdbDataContext();
            IQueryable<Analyzer> analyzer = context.Analyzers;
            IQueryable<SearchTerm> searchTerms = context.SearchTerms;
            var query = from a in analyzer
                        join s in searchTerms on a.FkCodeId equals s.FkCodeId
                        where s.SearchDesc == param
                        select a;
                        
            var resultList = new List<Analyzer>();
            resultList = query.OrderBy(a=>a.AnalyzerName).ToList();

            var analyzerNamesList = resultList.Select(l => l.AnalyzerName).ToList();

            return analyzerNamesList;
        }

        /// <summary>
        /// Returns the list of Analyzer for specifc serch term id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<Analyzer> getAnalyzerNamesForSpecificSearchTermId(int param)
        {
            var context = new CLSdbDataContext();
            IQueryable<Analyzer> analyzer = context.Analyzers;
            IQueryable<SearchTerm> searchTerms = context.SearchTerms;
            var query = from a in analyzer
                        join s in searchTerms on a.FkCodeId equals s.FkCodeId
                        where s.Id == param
                        select a;

            var resultList = new List<Analyzer>();
            resultList = query.ToList();
            return resultList;
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
