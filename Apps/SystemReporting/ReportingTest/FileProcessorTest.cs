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

    }
}
