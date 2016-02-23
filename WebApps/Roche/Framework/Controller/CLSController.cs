using CLSdbContext;
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
        #endregion

        #region Localitites

        public static List<Locality> getUniqueLocality()
        {
            var context = new CLSdbDataContext();
            var resultList = context.Localities.Distinct().ToList();
            return resultList;
        }

        #endregion

        #region Footnote

        public static List<Footnote> getUniqueFootnote()
        {
            var context = new CLSdbDataContext();
            var resultList = context.Footnotes.Distinct().ToList();
            return resultList;
        }

        #endregion

        #region Weburl

        public static List<Weburl> getUniqueWeburl()
        {
            var context = new CLSdbDataContext();
            var resultList = context.Weburls.Distinct().ToList();
            return resultList;
        }

        #endregion

        #region Analyzer

        public static List<Analyzer> getUniqueAnalyzers()
        {
            var context = new CLSdbDataContext();
            var resultList = context.Analyzers.Distinct().ToList();
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
            var resultList = context.ReimbursementRates.Distinct().ToList();
            return resultList;
        }

        #endregion

        #region Search Terms

        public static List<SearchTerm> getUniqueSearchTerm()
        {
            var context = new CLSdbDataContext();
            var resultList = context.SearchTerms.Distinct().ToList();
            return resultList;
        }

        public static List<SearchTerm> getAssayDescriptionForSpecificAnalyzer(int param)
        {
            var context = new CLSdbDataContext();
            var resultList = context.SearchTerms.ToList();
            var result = resultList.Where(a => a.FkCodeId == param).ToList();
            return result;
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
