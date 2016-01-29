using Microsoft.VisualStudio.TestTools.UnitTesting;
using Milliman.Controller;
using Milliman.Controller.BusinessLogic.Controller;
using Milliman.Data.Repository;
using Milliman.Entities.Models;
using Milliman.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFileProcessor
{
    [TestClass]
    public class RepositoryControllerTest
    {
        [TestMethod]
        public void TestController()
        {

            //Service Call
            MillimanService ser = new MillimanService();
            User u = new User() { UserName = "Afsheen T3" };
            ser.Save(u);

            IisLog log = new IisLog()
            {
                QueryURI = "test",
                UserName = u.UserName
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


        [TestMethod]
        public void TestMethod1()
        {
            ////initiate my repository
            //Repository<Blog> repository = new Repository<Blog>();
            ////initiate controller - for complicated buisness logic
            //CommonController controller = new CommonController();

            ////Service Call
            //MillimanService ser = new MillimanService();
            //var newBlog = new Blog();
            //newBlog.Name = "TeAst001";
            //ser.Save(newBlog);

            ////Using the controller
            //var blogAa = ser.GetBlogs<Blog>();
            //List<Blog> blogs = new List<Blog>();
            //foreach (var item in blogAa)
            //{
            //    blogs.Add(item);
            //}

            // Assert.IsNotNull(blogs);

        }
    }
}
