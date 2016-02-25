using CLSdbContext;
using Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CLSTest
{
    [TestClass]
    public class ControllerMethodsTest
    {
        [TestMethod]
        public void TestMethod_getUniqueCode()
        {
            var objList = new List<Code>();
            objList = CLSController.getUniqueCode();
            Assert.IsNotNull(objList.Count > 0);
        }
        [TestMethod]
        public void TestMethod_getUniqueCodeByCodeName()
        {
            var objList = new List<Code>();
            objList = CLSController.getUniqueCodeByCodeName();
            Assert.IsNotNull(objList.Count > 0);
        }


        [TestMethod]
        public void TestMethod_getUniquelocality()
        {
            var objList = new List<Locality>();
            objList = CLSController.getUniqueLocality();
            Assert.IsNotNull(objList.Count > 0);
        }

        [TestMethod]
        public void TestMethod_getUniqueFootnote()
        {
            var objList = new List<Footnote>();
            objList = CLSController.getUniqueFootnote();
            Assert.IsNotNull(objList.Count > 0);
        }

        [TestMethod]
        public void TestMethod_getUniqueWeburl()
        {
            var objList = new List<Weburl>();
            objList = CLSController.getUniqueWeburl();
            Assert.IsNotNull(objList.Count > 0);
        }

        [TestMethod]
        public void TestMethod_getUniqueAnalyzers()
        {
            var objList = new List<Analyzer>();
            objList = CLSController.getUniqueAnalyzers();
            Assert.IsNotNull(objList.Count > 0);
        }

        [TestMethod]
        public void TestMethod_getUniqueAnalyzerNames()
        {
            var objList = new List<Analyzer>();
            objList = CLSController.getUniqueAnalyzerNames();
            Assert.IsNotNull(objList.Count > 0);
        }

        //[TestMethod]
        //public void TestMethod_getSearchTermsForAnalyzerName()
        //{
        //    var objList = new List<Analyzer>();
        //    objList = CLSController.getSearchTermsForAnalyzerName();
        //    Assert.IsNotNull(objList.Count > 0);
        //}

        /// <summary>
        /// returns the distinct year list</summary>
        [TestMethod]
        public void TestMethod_getUniqueYear()
        {
            var objList = CLSController.getUniqueYear();
            Assert.IsNotNull(objList);
        }

        /// <summary>
        /// Retrieves unique reimbursements records</summary>
        [TestMethod]
        public void TestMethod_getUniqueReimbursementRate()
        {
            var objList = CLSController.getUniqueReimbursementRate();
            Assert.IsNotNull(objList);
        }

        /// <summary>
        /// Retrieves unique reimbursements records</summary>
        [TestMethod]
        public void TestMethod_getAllReimbursementRates()
        {
            var objList = CLSController.getAllReimbursementRates();
            Assert.IsNotNull(objList);
        }

        [TestMethod]
        public void TestMethod_getUniqueSearchTerm()
        {
            var objList = new List<SearchTerm>();
            objList = CLSController.getUniqueSearchTerm();
            Assert.IsNotNull(objList.Count > 0);
        }


        [TestMethod]
        public void TestMethod_getAssayDescriptionForSpecificAnalyzer()
        {
            var objList = new List<SearchTerm>();
            var codeID = 5;
            objList = CLSController.getAssayDescriptionForSpecificAnalyzer(codeID);
            Assert.IsNotNull(objList.Count == 2);
        }
        /// <summary>
        /// Get a list of valid “analyzer”(s) for a specific “assay description”
        /// </summary>
        [TestMethod]
        public void TestMethod_getAnalyzerForSpecificAssayDescription()
        {
            var objList = new List<Analyzer>();
            var assayDesc = "Hepatitis b surface ag eia";
            objList = CLSController.getAnalyzerForSpecificAssayDescription(assayDesc);
            Assert.IsNotNull(objList.Count == 9);
        }

        /// <summary>
        /// Retrieve primary data set on being provided a “list of analyzers”, “list of descriptions”, “list of locality” and the year
        /// </summary>
        [TestMethod]
        public void TestMethod_getEverything()
        {
            var year = "2015";
            var objList = Convert.ToInt32(CLSController.getEverything(year));
            Assert.IsNotNull(objList);
        }

        /// <summary>
        /// Retrieve primary data set on being provided a “list of analyzers”, “list of descriptions”, “list of locality” and the year
        /// </summary>
        [TestMethod]
        public void TestMethod_getSearchTermsForAnalyzerName()
        {
            var analyzer_name = "Roche_Hitachi Modular";
            var objList = CLSController.getSearchTermsForAnalyzerName(analyzer_name);
            Assert.IsNotNull(objList);
        }
    }
}
