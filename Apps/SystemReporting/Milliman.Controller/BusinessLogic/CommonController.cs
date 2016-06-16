using SystemReporting.Data.Repository;
using SystemReporting.Entities.Models;
using SystemReporting.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemReporting.Utilities;

namespace SystemReporting.Controller.BusinessLogic.Controller
{
    public class CommonController : ControllerBase
    {
         private IMillimanService dbService { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CommonController(){  }

        #region User

        /// <summary>
        /// Method to check if model exist. If it does then return and if it does not then add and return
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public User AddOrGetUser(User model)
        {
            //initiate service
            dbService = new MillimanService();
            var obj = new User();
            try
            {                
                var exists = dbService.GetUsers<User>(u => u.UserName == model.UserName).FirstOrDefault();
                if (exists == null)
                {
                    obj.UserName = model.UserName.Trim();
                    obj.AddDate = DateTime.Now;
                    dbService.Save(obj);

                    obj = new User();
                    //after insert set the id
                    obj = dbService.GetUsers<User>(a => a.UserName == model.UserName).FirstOrDefault();
                }
                else
                {
                    obj = exists;
                }

                dbService.Dispose();
            }
            catch (Exception ex)
            {
                dbService.Dispose();
                log.Fatal("Class SessionLogController. Method ProcessLogs.", ex);
                SendEmail("Exception Raised", "Common Controller Exception");
            }

            return obj;
        }
        #endregion

        #region Report
        /// <summary>
        /// Method checks if object exist then add to db - No Update
        /// </summary>
        /// <param name="model"></param>
        public Report AddOrGetReport(Report model)
        {
            //initiate service
            dbService = new MillimanService();
            var obj = new Report();
            try
            {
                var exists = dbService.GetReports<Report>(u => u.ReportName == model.ReportName).FirstOrDefault();
                if (exists == null)
                {
                    obj.ReportName = model.ReportName.Trim();
                    obj.AddDate = DateTime.Now;
                    dbService.Save(obj);

                    obj = new Report();
                    //after insert set the id
                    obj = dbService.GetReports<Report>(a => a.ReportName == model.ReportName).FirstOrDefault();
                }
                else
                {
                    obj = exists;
                }

                dbService.Dispose();
            }
            catch (Exception ex)
            {
                dbService.Dispose();
                log.Fatal("Class SessionLogController. Method ProcessLogs.", ex);
                SendEmail("Exception Raised", "Common Controller Exception");
            }

            return obj;
        }
        #endregion

        #region Group
        /// <summary>
        /// Method checks if object exist then add to db - No Update
        /// </summary>
        /// <param name="model"></param>
        public Group AddOrGetGroup(Group model)
        {
            //initiate service
            dbService = new MillimanService();
            var obj = new Group();
            try
            {
                var exists = dbService.GetGroups<Group>(u => u.GroupName == model.GroupName).FirstOrDefault();
                if (exists == null)
                {
                    obj.GroupName = model.GroupName.Trim();
                    obj.AddDate = DateTime.Now;
                    dbService.Save(obj);

                    obj = new Group();
                    //after insert set the id
                    obj = dbService.GetGroups<Group>(a => a.GroupName == model.GroupName).FirstOrDefault();
                }
                else
                {
                    obj = exists;
                }

                dbService.Dispose();
            }
            catch (Exception ex)
            {
                dbService.Dispose();
                log.Fatal("Class SessionLogController. Method ProcessLogs.", ex);
                SendEmail("Exception Raised", "Common Controller Exception");
            }

            return obj;
        }
        #endregion
    }
}
