using Microsoft.VisualStudio.TestTools.UnitTesting;
using SystemReporting.Controller;
using SystemReporting.Controller.BusinessLogic.Controller;
using SystemReporting.Data.Repository;
using SystemReporting.Entities.Models;
using SystemReporting.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Web;

namespace TestFileProcessor
{

    [TestClass]
    public class RepositoryControllerTest: ControllerBase
    {
        [TestMethod, Owner("AK")]
        public void TestController()
        {
            //Service Call
            MillimanService ser = new MillimanService();
            User u = new User() { UserName = "Afsheen T3" };
            ser.Save(u);

            IisLog log = new IisLog()
            {
                QueryURI = "test",
                fk_user_id=2
            };
            ser.Save(log);

            ////Using the controller
            //var blogAa = ser.GetBlogs<Blog>();
            List<IisLog> blogs = new List<IisLog>();

            foreach (var item in blogs)
            {
                blogs.Add(item);
            }

            Assert.IsNotNull(blogs);

        }


        //[TestMethod]
        //public void TestMethod1()
        //{
        //    ////initiate my repository
        //    //Repository<Blog> repository = new Repository<Blog>();
        //    ////initiate controller - for complicated buisness logic
        //    //CommonController controller = new CommonController();

        //    ////Service Call
        //    //MillimanService ser = new MillimanService();
        //    //var newBlog = new Blog();
        //    //newBlog.Name = "TeAst001";
        //    //ser.Save(newBlog);

        //    ////Using the controller
        //    //var blogAa = ser.GetBlogs<Blog>();
        //    //List<Blog> blogs = new List<Blog>();
        //    //foreach (var item in blogAa)
        //    //{
        //    //    blogs.Add(item);
        //    //}

        //    // Assert.IsNotNull(blogs);

        //}
        [TestMethod]
        public void TestMethod_GetSessionLogListForReport()
        {
            string date = "04/01/2016";
            string reportName = "LIVE BPCI - PREMIER";
            
            var list = ControllerSessionLog.GetSessionLogListForGroup(date, date, reportName).ToList();
                        
            var path = @"C:\Users\afsheen.khan\Desktop\Test\";

            var resultsList = new List<string>();
           
            foreach (SessionLog curData in list)
            resultsList.Add(
                (curData.UserAccessDatetime.HasValue ? curData.UserAccessDatetime.Value.ToString() : "NULL").ToString() + "," +
                (!string.IsNullOrEmpty(curData.Document) ? curData.Document.ToString(): "NULL").ToString() + "," +
                (!string.IsNullOrEmpty(curData.SessionEndReason) ? curData.SessionEndReason.ToString() : "NULL").ToString() + "," +
                (!string.IsNullOrEmpty(curData.SessionDuration) ? curData.SessionDuration.ToString() : "NULL").ToString() + "," +
                (!string.IsNullOrEmpty(curData.User.UserName) ? curData.User.UserName.ToString() : "NULL").ToString() + "," +
                curData.Browser.ToString());

            resultsList.Insert(0,string.Format("{0},{1},{2},{3},{4},{5}",
                                      "Date/Time", "QVW", "QVW Close Reason", "User Session Length (HH:MM:SS)", "User", "Browser"));

            //Save Back To File
            System.IO.File.WriteAllLines(path + DateTime.Now.ToString("MMdd_hhmm") + ".csv"
                , resultsList.ToArray());
           
            Assert.IsNotNull(list);
            
        }

        [TestMethod]
        public void TestMethod_GetGetAuditLogListForReport()
        {
            //string date = "04/01/2016";
            //string reportName = "LIVE BPCI - PREMIER";
            //var list = AuditLogController.GetAuditLogListForReport(date,date,"").ToList();

            //var path = @"C:\Users\afsheen.khan\Desktop\Test\";

            //var resultsList = new List<string>();

            //foreach (AuditLog curData in list)
            //    resultsList.Add(
            //        (curData.UserAccessDatetime.HasValue ? curData.UserAccessDatetime.Value.ToString() : "NULL").ToString() + "," +
            //        (!string.IsNullOrEmpty(curData.Document) ? curData.Document.ToString() : "NULL").ToString() + "," +
            //        (!string.IsNullOrEmpty(curData.EventType) ? curData.EventType.ToString() : "NULL").ToString() + "," +
            //        (!string.IsNullOrEmpty(curData.User.UserName) ? curData.User.UserName.ToString() : "NULL").ToString() + "," +
            //        (!string.IsNullOrEmpty(curData.Message) ? curData.Message.ToString() : "NULL").ToString());

            //resultsList.Insert(0, string.Format("{0},{1},{2},{3},{4},{5}",
            //                          "Date/Time", "QVW", "QVW Close Reason", "User Session Length (HH:MM:SS)", "User", "Browser"));

            ////Save Back To File
            //System.IO.File.WriteAllLines(path + DateTime.Now.ToString("MMdd_hhmm") + ".csv"
            //    , resultsList.ToArray());

            //Assert.IsNotNull(list);

        }
    }
}
