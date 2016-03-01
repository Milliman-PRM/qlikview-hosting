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

        [TestMethod]
        public void TestMethod_getSearchTermsForSpecificAnalyzerName()
        {
            var objList = new List<SearchTerm>();
            var aName = "COBAS 4000 A";
            objList = CLSController.getSearchTermsForSpecificAnalyzerName(aName);
            Assert.IsNotNull(objList.Count > 0);
        }

        /// <summary>
        /// Returns list of serach terms for analyzer
        /// </summary>
        [TestMethod]
        public void TestMethod_getSearchTermsForSpecificAnalyzerId()
        {
            var aId = 5;// "Roche_Hitachi Modular";
            var objList = CLSController.getSearchTermsForSpecificAnalyzerId(aId);
            Assert.IsNotNull(objList);
        }


        /// <summary>
        /// Returns list of serach terms for analyzer
        /// </summary>
        [TestMethod]
        public void TestMethod_getSearchTermsForSpecificAnalyzerIdArray()
        {
            //create array
            var array = new string[] { "807", "204", "59", "84", "345", "12", "379", "285", "54", "517" };
            //convert an array of integers to a comma-separated string
            var aray = string.Join(",", array);
            var objList = CLSController.getSearchTermsForSpecificAnalyzerIdListArray(aray);
            Assert.IsNotNull(objList);
        }

        /// <summary>
        /// Returns list of serach terms for analyzer
        /// </summary>
        [TestMethod]
        public void TestMethod_getCodeIdsForSpecificAnalyzerName()
        {
            var aName = "AVL 9180";
            var objList = CLSController.getAnalyzerIdsForSpecificAnalyzerName(aName);
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

        /// <summary>
        /// returns the distinct year list</summary>
        [TestMethod]
        public void TestMethod_getUniqueYear()
        {
            var objList = CLSController.getUniqueYear();
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
        public void TestMethod_getAssayDescriptionForSpecificCodeId()
        {
            var objList = new List<SearchTerm>();
            var codeId = 5;
            objList = CLSController.getAssayDescriptionForSpecificCodeId(codeId);
            Assert.IsNotNull(objList.Count > 0);
        }

        [TestMethod]
        public void TestMethod_getAnalyzerForSpecificSearchTermDesc()
        {
            var objList = new List<Analyzer>();
            var st = "Assay of procainamide";
            objList = CLSController.getAnalyzerForSpecificSearchTermDesc(st);
            Assert.IsNotNull(objList.Count == 10);
        }

        [TestMethod]
        public void TestMethod_getAnalyzerNamesListForSpecificSearchTermDesc()
        {
            var objList = new List<string>();
            var st = "Assay of procainamide";
            objList = CLSController.getAnalyzerNamesListForSpecificSearchTermDesc(st);
            Assert.IsNotNull(objList.Count == 10);
        }

        [TestMethod]
        public void TestMethod_getAnalyzerNamesForSpecificSearchTermId()
        {
            var sId = 5;
            var objList = CLSController.getAnalyzerNamesForSpecificSearchTermId(sId);
            Assert.IsNotNull(objList);
        }

        [TestMethod]
        public void TestMethod_getAssayDescriptionForSpecificAnalyzer()
        {
            var objList = new List<SearchTerm>();
            var codeId = 5;
            objList = CLSController.getAssayDescriptionForSpecificCodeId(codeId);
            Assert.IsNotNull(objList.Count == 2);
        }

        [TestMethod]
        public void TestMethod_getEverything()
        {
            var year = "2015";
            var objList = Convert.ToInt32(CLSController.getEverything(year));
            Assert.IsNotNull(objList);
        }
        

    }
}
