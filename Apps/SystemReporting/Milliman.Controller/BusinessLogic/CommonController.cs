using SystemReporting.Data.Repository;
using SystemReporting.Entities.Models;
using SystemReporting.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SystemReporting.Controller.BusinessLogic.Controller
{
    public class CommonController : ControllerBase
    {
        private MillimanService serviceMilliman = null;

        public CommonController()
        {
            serviceMilliman = new MillimanService();
        }

        public CommonController(MillimanService mserv)
        {
            serviceMilliman = mserv;
        }

        #region User

        public void SaveUser(User model)
        {
            try
            {
                var userExist = serviceMilliman.GetUsers<User>(a => a.UserName == model.UserName).FirstOrDefault();
                User user = new User();
                if (userExist == null)
                {
                    user = new User()
                    {
                        UserName = model.UserName.Trim()
                    };
                }
                serviceMilliman.Save(user);
            }
            catch (Exception ex)
            {
                LogError(DateTime.Now + " ||-|| "
                                        + "Class CommonController. Method SaveUser." + Environment.NewLine
                                        + ex.Message + " ||-|| " + ex.InnerException + Environment.NewLine);
            }
        }
        #endregion

        #region Report
        public void SaveReport(Report model)
        {
            try
            {
                var reportExist = serviceMilliman.GetReports<Report>(a => a.ReportName == model.ReportName).FirstOrDefault();
                Report report = new Report();
                if (reportExist == null)
                {
                    report = new Report()
                    {
                        ReportName = model.ReportName.Trim()
                    };
                }
                serviceMilliman.Save(report);

            }
            catch (Exception ex)
            {
                LogError(DateTime.Now + " ||-|| "
                                        + "Class CommonController. Method SaveReport." + Environment.NewLine
                                        + ex.Message + " ||-|| " + ex.InnerException + Environment.NewLine);
            }

        }

        #endregion

        #region Group

        public void SaveGroup(Group model)
        {
            try
            {
                var groupExist = serviceMilliman.GetGroups<Group>(a => a.GroupName == model.GroupName).FirstOrDefault(); 
                Group group = new Group();
                if (groupExist == null)
                {
                    group = new Group()
                    {
                        GroupName = model.GroupName.Trim()
                    };
                }
                serviceMilliman.Save(group);

            }
            catch (Exception ex)
            {
                LogError(DateTime.Now + " ||-|| "
                        + "Class CommonController. Method SaveGroup." + Environment.NewLine
                        + ex.Message + " ||-|| " + ex.InnerException + Environment.NewLine);
            }

        }

        #endregion
    }
}
