using FileProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportingTest
{
    [TestClass]
    public class FileProcessorTest
    {
        [TestMethod]
        public void TestIisLog()
        {
            //Make sure that there is file in folder \\Indy-ss01\ProductionLogsTest\IISLogs
            string fileType = "Iis";
            FileProcessorApplication.Program.Main(new string[] { fileType });
        }

        [TestMethod]
        public void TestAuditLog()
        {
            //Make sure that there is file in folder \\Indy-ss01\ProductionLogsTest\QVLogs
            string fileType = "Audit";
            FileProcessorApplication.Program.Main(new string[] { fileType });
        }

        [TestMethod]
        public void TestSessionLog()
        {
            //Make sure that there is file in folder \\Indy-ss01\ProductionLogsTest\QVLogs
            string fileType = "Session";
            FileProcessorApplication.Program.Main(new string[] { fileType });
        }

        [TestMethod]
        public void TestAll()
        {
            //Make sure that there is file in folder \\Indy-ss01\ProductionLogsTest\IISLogs
            FileProcessorApplication.Program.Main(new string[] { });
        }
        
        [TestMethod]
        public void TestHistoryIis()
        {
            var fileNameAndDir = @"C:\ProductionLogs\IISLogs\u_ex151002.log";
            //Make sure that there is file in folder \\Indy-ss01\ProductionLogsTest\IISLogs
            FileProcessorApplication.Program.Main(new string[] { fileNameAndDir });
        }

        [TestMethod]
        public void TestHistoryAudit()
        {
            var fileNameAndDir = @"C:\ProductionLogs\QVLogs\Audit_INDY-PRM-1_2014-10-10.log";
            //Make sure that there is file in folder \\Indy-ss01\ProductionLogsTest\IISLogs
            FileProcessorApplication.Program.Main(new string[] { fileNameAndDir });
        }

        [TestMethod]
        public void TestHistorySession()
        {
            var fileNameAndDir = @"C:\ProductionLogs\QVLogs\Sessions_INDY-PRM-1_2016-01-25.log";
            //Make sure that there is file in folder \\Indy-ss01\ProductionLogsTest\IISLogs
            FileProcessorApplication.Program.Main(new string[] { fileNameAndDir });
        }

    }
}
