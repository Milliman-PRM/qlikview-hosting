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
            var resultList = new List<Code>();
            try
            {
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
                                .Distinct()
                                .OrderBy(c => c.Code1)
                                .ToList();
                }

                //distinct objects
                resultList = (from obj in objList
                              select obj).Distinct().ToList();

                resultList.Sort((x, y) => string.Compare(x.Code1, y.Code1, StringComparison.Ordinal));
            }
            catch (Exception ex)
            {
                throw new Exception("getUniqueCode", ex);
            }

            return resultList;
        }


        /// <summary>
        /// Returns the list of search term for a specific analyzer id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<Code> getCodesForSpecificAnalyzerIdListArray(string param)
        {
            var context = new CLSdbDataContext();
            var orderedListByAlphabatsThenNumbers = new List<Code>();
            try
            {
                IQueryable<Analyzer> analyzer = context.Analyzers;
                IQueryable<Code> codes = context.Codes;

                //split ids
                var splittedIds = param.Split(',').Select(str => int.Parse(str));
                var query = analyzer                                    // starting point - table in the "from" statement
                           .Join(codes,                                 // the source table of the inner join
                                      azTble => azTble.FkCodeId,        // Select the primary key (the first part of the "on" clause in an sql "join" statement)
                                      cTble => cTble.Id,                // Select the foreign key (the second part of the "on" clause)
                                      (azTble, cTble) =>
                                          new { Az = azTble, Cc = cTble }
                                       )    // selection
                           .Where(sa => splittedIds.Contains(sa.Az.Id)) // where statement
                           .OrderBy(a => a.Cc.Code1)
                           .Select(a => a.Cc);    // seelct record set

                var resultList = new List<Code>();
                //distinct objects
                resultList = (from obj in query
                              select obj).Distinct().ToList();
                resultList.Sort((x, y) => string.Compare(x.Code1, y.Code1, StringComparison.Ordinal));

                //sort list by alphabatic order then by number
                orderedListByAlphabatsThenNumbers = resultList.OrderByDescending(x => x.Code1.All(char.IsLetter))
                                                        .ThenByDescending(x => x.Code1.Any(char.IsDigit))
                                                        .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("getCodesForSpecificAnalyzerIdListArray", ex);
            }

            return orderedListByAlphabatsThenNumbers;
        }

        /// <summary>
        /// Returns list of Analyzer for specific search term desc. The list is retured as string not an object
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<Code> getCptCodeListForAssayDescription(string param)
        {
            var context = new CLSdbDataContext();
            var resultList = new List<Code>();
            try
            {
                IQueryable<Code> code = context.Codes;
                IQueryable<SearchTerm> searchTerms = context.SearchTerms;
                var query = from c in code
                            join s in searchTerms on c.Id equals s.FkCodeId
                            where c.Code1 == param
                            select c;

                //distinct objects
                resultList = (from obj in query
                              select obj).Distinct().ToList();

                resultList.Sort((x, y) => string.Compare(x.Code1, y.Code1, StringComparison.Ordinal));
            }
            catch (Exception ex)
            {
                throw new Exception("getCptCodeListForAssayDescription", ex);
            }

            return resultList;
        }

        /// <summary>
        /// Returns the list of search term for a specific analyzer id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<Code> getCptCodesListForAnalyzersIdListArray(string param)
        {
            var context = new CLSdbDataContext();
            var orderedListByAlphabatsThenNumbers = new List<Code>();
            try
            {
                IQueryable<Analyzer> analyzer = context.Analyzers;
                IQueryable<Code> codes = context.Codes;

                //split ids
                var splittedIds = param.Split(',').Select(str => int.Parse(str));
                var query = analyzer                                // starting point - table in the "from" statement
                           .Join(codes,                       // the source table of the inner join
                                      azTble => azTble.FkCodeId,     // Select the primary key (the first part of the "on" clause in an sql "join" statement)
                                      ccTble => ccTble.Id,     // Select the foreign key (the second part of the "on" clause)
                                      (azTble, ccTble) =>
                                          new { Az = azTble, Ct = ccTble }
                                       )    // selection
                           .Where(a => splittedIds.Contains(a.Az.Id)) // where statement
                           .OrderBy(a => a.Ct.Code1)
                           .Select(a => a.Ct);    // seelct record set

                var resultList = new List<Code>();
                //distinct objects
                resultList = (from obj in query
                              select obj).Distinct().ToList();

                resultList.Sort((x, y) => string.Compare(x.Code1, y.Code1, StringComparison.Ordinal));

                //sort list by alphabatic order then by number
                orderedListByAlphabatsThenNumbers = resultList.OrderByDescending(x => x.Code1.All(char.IsLetter))
                                                        .ThenByDescending(x => x.Code1.Any(char.IsDigit))
                                                        .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("getCptCodesListForAnalyzersIdListArray", ex);
            }
            return orderedListByAlphabatsThenNumbers;
        }

        #endregion

        #region Localitites

        public static List<Locality> getUniqueLocality()
        {
            var context = new CLSdbDataContext();
            var finallist = new List<Locality>();
            try
            {
                var dboList = context.Localities.Distinct().ToList();
                var resultList = dboList.GroupBy(x => x.Locality1).Select(y => y.First()).OrderBy(a => a.LocalityDescription).ToList();
                finallist = resultList.Where(l => !string.IsNullOrEmpty(l.LocalityDescription))
                                        .OrderByDescending(x => x.LocalityDescription == "National Limit").ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("getUniqueLocality", ex);
            }
            return finallist;
        }

        #endregion

        #region Footnote

        public static List<Footnote> getUniqueFootnote()
        {
            var context = new CLSdbDataContext();
            var resultList = new List<Footnote>();
            try
            {
                var dboList = context.Footnotes.Distinct().ToList();
                resultList = dboList.GroupBy(x => x.Footnote1).Select(y => y.First()).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("getUniqueFootnote", ex);
            }
            return resultList;
        }

        #endregion

        #region Weburl

        public static List<Weburl> getUniqueWeburl()
        {
            var context = new CLSdbDataContext();
            var resultList = new List<Weburl>();
            try
            {
                var dboList = context.Weburls.Distinct().ToList();
                resultList = dboList.GroupBy(x => x.Webaddressurl).Select(y => y.First()).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("getUniqueWeburl", ex);
            }
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
            var resultList = new List<Analyzer>();
            try
            {
                var context = new CLSdbDataContext();
                var dboList = context.Analyzers.Distinct().ToList();
                resultList = dboList.GroupBy(x => x.AnalyzerName).Select(y => y.First()).OrderBy(a => a.AnalyzerName).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception("getUniqueAnalyzers", ex);
            }
            return resultList;
        }

        public static List<string> getAnalyzerIdsForSpecificAnalyzerName(string param)
        {
            var resultList = new List<string>();
            try
            {
                //Query the reimbursmenet_rate table and return a list of unquie year values
                var context = new CLSdbDataContext();
                var dboList = context.Analyzers.Where(a => a.AnalyzerName == param)
                                      .Select(x => new
                                      {
                                          AnalyzerIds = x.Id.ToString()
                                      }).ToList();

                //distinct objects
                resultList = new List<string>(dboList.Select(s => s.AnalyzerIds.ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception("getAnalyzerIdsForSpecificAnalyzerName", ex);
            }
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
            var resultList = new List<Analyzer>();
            var resultListFinal = new List<string>();
            try
            {
                IQueryable<Analyzer> analyzer = context.Analyzers;
                IQueryable<SearchTerm> searchTerms = context.SearchTerms;
                var query = from a in analyzer
                            join s in searchTerms on a.FkCodeId equals s.FkCodeId
                            where s.SearchDesc == param
                            select a;

                //distinct objects
                resultList = (from obj in query
                              select obj).Distinct().OrderBy(x => x.AnalyzerName).ToList();

                resultList.Sort((x, y) => string.Compare(x.AnalyzerName, y.AnalyzerName, StringComparison.Ordinal));
                resultListFinal = resultList.Select(l => l.AnalyzerName).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("getAnalyzerNamesListForSpecificSearchTermDesc", ex);
            }
            return resultListFinal;
        }

        /// <summary>
        /// Returns list of Analyzer for specific search term desc. The list is retured as string not an object
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<Analyzer> getAnalyzerNamesListForSpecificCptCode(string param)
        {
            var context = new CLSdbDataContext();
            var resultList = new List<Analyzer>();
            try
            {
                IQueryable<Analyzer> analyzer = context.Analyzers;
                IQueryable<Code> code = context.Codes;
                var query = from a in analyzer
                            join c in code on a.FkCodeId equals c.Id
                            where c.Code1 == param
                            select a;

                //distinct objects
                resultList = (from obj in query
                              select obj).Distinct().ToList();

                resultList.Sort((x, y) => string.Compare(x.AnalyzerName, y.AnalyzerName, StringComparison.Ordinal));
            }
            catch (Exception ex)
            {
                throw new Exception("getAnalyzerNamesListForSpecificCptCode", ex);
            }
            return resultList;
        }

        #endregion

        #region ReimbursementRate
        /// <summary>
        /// Returns list of Year
        /// </summary>
        /// <returns></returns>
        public static List<string> getUniqueYear()
        {
            var resultList = new List<string>();
            try
            {
                //Query the reimbursmenet_rate table and return a list of unquie year values
                var context = new CLSdbDataContext();
                var dboList = context.ReimbursementRates.GroupBy(a => a.Year)
                                      .Select(x => new
                                      {
                                          Year = x.Key.Value.ToString()
                                      }).ToList();

                resultList = new List<string>(dboList.Select(s => s.Year.ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception("getUniqueYear", ex);
            }
            return resultList;

        }
        #endregion

        #region Search Terms

        public static List<SearchTerm> getUniqueSearchTerm()
        {
            var context = new CLSdbDataContext();
            var orderedListByAlphabatsThenNumbers = new List<SearchTerm>();
            try
            {
                var dboList = context.SearchTerms.Distinct().ToList();
                var resultList = dboList.GroupBy(x => x.SearchDesc).Select(y => y.First()).OrderBy(a => a.SearchDesc).ToList();
                var finallist = resultList.Where(l => !string.IsNullOrEmpty(l.SearchDesc)).ToList();

                finallist.Sort((x, y) => string.Compare(x.SearchDesc, y.SearchDesc, StringComparison.Ordinal));

                //sort list by alphabatic order then by number
                orderedListByAlphabatsThenNumbers = finallist.OrderByDescending(x => x.SearchDesc.All(char.IsLetter))
                                                    .ThenByDescending(x => x.SearchDesc.Any(char.IsDigit))
                                                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("getUniqueSearchTerm", ex);
            }

            return orderedListByAlphabatsThenNumbers;
        }
        /// <summary>
        /// Returns the list of search term for a specific analyzer id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<SearchTerm> getSearchTermsForSpecificAnalyzerIdListArray(string param)
        {
            var context = new CLSdbDataContext();
            var orderedListByAlphabatsThenNumbers = new List<SearchTerm>();
            try
            {
                IQueryable<Analyzer> analyzer = context.Analyzers;
                IQueryable<SearchTerm> searchTerms = context.SearchTerms;
                //split ids
                var splittedIds = param.Split(',').Select(str => int.Parse(str));

                var query = analyzer                                // starting point - table in the "from" statement
                           .Join(searchTerms,                       // the source table of the inner join
                                      azTble => azTble.FkCodeId,     // Select the primary key (the first part of the "on" clause in an sql "join" statement)
                                      stTble => stTble.FkCodeId,     // Select the foreign key (the second part of the "on" clause)
                                      (azTble, stTble) =>
                                          new { Az = azTble, St = stTble }
                                       )    // selection
                           .Where(sa => splittedIds.Contains(sa.Az.Id)) // where statement
                           .OrderBy(a => a.St.SearchDesc)
                           .Distinct()
                           .Select(a => a.St);    // seelct record set


                var resultList = new List<SearchTerm>();
                //distinct objects
                resultList = (from obj in query
                              select obj).Distinct().ToList();

                resultList.Sort((x, y) => string.Compare(x.SearchDesc, y.SearchDesc, StringComparison.Ordinal));

                //sort list by alphabatic order then by number
                orderedListByAlphabatsThenNumbers = resultList.OrderByDescending(x => x.SearchDesc.All(char.IsLetter))
                                                        .ThenByDescending(x => x.SearchDesc.Any(char.IsDigit))
                                                        .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("getSearchTermsForSpecificAnalyzerIdListArray", ex);
            }
            return orderedListByAlphabatsThenNumbers;
        }

        public static List<SearchTerm> getAssayDescriptionListForSpecificCptCode(string param)
        {
            var context = new CLSdbDataContext();
            var resultList = new List<SearchTerm>();
            try
            {
                IQueryable<SearchTerm> searchTerms = context.SearchTerms;
                IQueryable<Code> code = context.Codes;
                var query = from s in searchTerms
                            join c in code on s.FkCodeId equals c.Id
                            where s.FkCodeId == int.Parse(param.ToString())
                            select s;

                //distinct objects
                resultList = (from obj in query
                              select obj).Distinct().ToList();
                resultList.Sort((x, y) => string.Compare(x.SearchDesc, y.SearchDesc, StringComparison.Ordinal));
            }
            catch (Exception ex)
            {
                throw new Exception("getAssayDescriptionListForSpecificCptCode", ex);
            }
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

    }
}
