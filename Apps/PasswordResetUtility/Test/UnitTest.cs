using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Configuration;

namespace Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod_DbConnection()
        {
            var connString = ConfigurationManager.ConnectionStrings["PWUdbContextConnectionString"].ConnectionString;
            var con = new SqlConnection(connString);
            con.Open();

            Assert.IsTrue(con.State == System.Data.ConnectionState.Open);
        }

        [TestMethod]
        public void TestMethod_Main()
        {
            var users = new string[] { "aadie@sjhnh.org" };            
            PasswordResetUtilityApplication.Program.Main(users);
        }
        [TestMethod]
        public void TestMethod_MainAll()
        {
            var users = new string[] { "All" };
            PasswordResetUtilityApplication.Program.Main(users);
        }
    }
}
