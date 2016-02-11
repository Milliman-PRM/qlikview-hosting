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
            User user = new User();
            try
            {                
                var exists = dbService.GetUsers<User>(u => u.UserName == model.UserName).FirstOrDefault();
                if (exists == null)
                {
                    user.UserName = model.UserName.Trim();
                    dbService.Save(user);

                    user = new User();
                    //after insert set the id
                    user = dbService.GetUsers<User>(a => a.UserName == model.UserName).FirstOrDefault();
                }
                else
                {
                    user = exists;
                }

                dbService.Dispose();
            }
            catch (Exception ex)
            {
                dbService.Dispose();
                log.Fatal("Class SessionLogController. Method ProcessLogs.", ex);
            }

            return user;
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
            Report report = new Report();
            try
            {
                var exists = dbService.GetReports<Report>(u => u.ReportName == model.ReportName).FirstOrDefault();
                if (exists == null)
                {
                    report.ReportName = model.ReportName.Trim();
                    dbService.Save(report);

                    report = new Report();
                    //after insert set the id
                    report = dbService.GetReports<Report>(a => a.ReportName == model.ReportName).FirstOrDefault();
                }
                else
                {
                    report = exists;
                }

                dbService.Dispose();
            }
            catch (Exception ex)
            {
                dbService.Dispose();
                log.Fatal("Class SessionLogController. Method ProcessLogs.", ex);
            }

            return report;
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
            Group group = new Group();
            try
            {
                var exists = dbService.GetGroups<Group>(u => u.GroupName == model.GroupName).FirstOrDefault();
                if (exists == null)
                {
                    group.GroupName = model.GroupName.Trim();
                    dbService.Save(group);

                    group = new Group();
                    //after insert set the id
                    group = dbService.GetGroups<Group>(a => a.GroupName == model.GroupName).FirstOrDefault();
                }
                else
                {
                    group = exists;
                }

                dbService.Dispose();
            }
            catch (Exception ex)
            {
                dbService.Dispose();
                log.Fatal("Class SessionLogController. Method ProcessLogs.", ex);
            }

            return group;
        }
        #endregion
    }
}
