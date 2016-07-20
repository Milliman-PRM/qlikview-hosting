using System;
using System.Collections.Generic;
using System.Linq;
using SystemReporting.Entities.Models;
using SystemReporting.Service;
using SystemReporting.Utilities.ExceptionHandling;

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
                ExceptionLogger.LogError(ex, "Exception Raised in Method AddOrGetUser.", "Common Controller Exception");
            }

            return obj;
        }

        /// <summary>
        /// Method to check if model exist. If it does then return and if it does not then add and return
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public User GetUserById(int? id)
        {
            //initiate service
            dbService = new MillimanService();
            var obj = new User();
            try
            {
                var exists = dbService.GetUsers<User>(u => u.Id == id.Value).FirstOrDefault();
                if (exists != null)
                {
                    obj = exists;
                }
                dbService.Dispose();
            }
            catch (Exception ex)
            {
                dbService.Dispose();
                ExceptionLogger.LogError(ex, "Exception Raised in Method GetUserById.", "Common Controller Exception");
            }

            return obj;
        }

        public List<User> GetUserList()
        {
            //initiate service
            var dbService = new MillimanService();
            var obj = new List<User>();
            try
            {
                var exists = dbService.GetUsers<User>().OrderBy(o => o.UserName).ToList(); 
                if (exists.Count > 0)
                {
                    obj = exists;
                }
                dbService.Dispose();
            }
            catch (Exception ex)
            {
                dbService.Dispose();
                ExceptionLogger.LogError(ex, "Exception Raised in Method GetUserList.", "Common Controller Exception");
            }
            return obj;
        }
        #endregion

        #region Report
        /// <summary>
        /// Method checks if object exist then add to db - No Update
        /// </summary>
        /// <param name="model"></param>
        public Report AddOrGetReport(Report Report, String Document)
        {
            //initiate service
            dbService = new MillimanService();
            var obj = new Report();
            try
            {
                var exists = dbService.GetReports<Report>(u => u.ReportName == Report.ReportName).FirstOrDefault();
                if (exists == null)
                {
                    obj.ReportName = Report.ReportName.Trim();
                    obj.ReportType = GetReportType(Report, Document);
                    obj.AddDate = DateTime.Now;
                    dbService.Save(obj);

                    obj = new Report();
                    //after insert set the id
                    obj = dbService.GetReports<Report>(a => a.ReportName == Report.ReportName).FirstOrDefault();
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
                ExceptionLogger.LogError(ex, "Exception Raised in Method AddOrGetReport.", "Common Controller Exception");
            }

            return obj;
        }

        public static Report ReportGet(string reportName)
        {
            //initiate service
            var dbService = new MillimanService();
            var obj = new Report();
            try
            {
                var exists = dbService.GetReports<Report>(r=>r.ReportName == reportName).FirstOrDefault();
                if (exists != null)
                {
                    obj = exists;
                }
                dbService.Dispose();
            }
            catch (Exception ex)
            {
                dbService.Dispose();
                ExceptionLogger.LogError(ex, "Exception Raised in Method ReportGet.", "Common Controller Exception");
            }

            return obj;
        }

        public static List<Report> ReportGetListBySearch(string searchText)
        {
            //initiate service
            var dbService = new MillimanService();
            var obj = new List<Report>();
            try
            {
                var list = dbService.GetReports<Report>(r => r.ReportName.Contains(searchText));
                if (list != null)
                {
                    obj = list.ToList();
                }
                dbService.Dispose();
            }
            catch (Exception ex)
            {
                dbService.Dispose();
                ExceptionLogger.LogError(ex, "Exception Raised in Method ReportGetListBySearch.", "Common Controller Exception");
            }

            return obj;
        }

        public List<Report> GetReportList()
        {
            //initiate service
            var dbService = new MillimanService();
            var obj = new List<Report>();
            try
            {
                var exists = dbService.GetReports<Report>().OrderBy(o => o.ReportName).ToList(); 
                if (exists.Count>0)
                {
                    obj = exists;
                }
                dbService.Dispose();
            }
            catch (Exception ex)
            {
                dbService.Dispose();
                ExceptionLogger.LogError(ex, "Exception Raised in Method GetReportList.", "Common Controller Exception");
            }

            return obj;
        }
        #endregion

        #region"Report Type"

        public string GetReportType(Report Report, String Document)
        {
            string ReportType = "";
            List<string> ReportNameTokens;
            if (Report.ReportName.Any(c => char.IsDigit(c)) && !Report.ReportName.Contains(' '))
            {
                ReportNameTokens = GetParentReportType(Report, Document).Split(' ').ToList();
            }
            else
            {
                ReportNameTokens = Report.ReportName.Split(' ').ToList();
            }
            
            var dbService = new MillimanService();

            var ReportTypeList = dbService.GetReportTypes<ReportType>().OrderBy(o => o.Type).ToList();

            foreach(ReportType type in ReportTypeList)
            {
                List<String> TypeTokens = type.Keywords.Split(';').ToList();

                List<String> CommonTokens = TypeTokens.Intersect(ReportNameTokens).ToList();
                
                if (CommonTokens.SequenceEqual(TypeTokens))                 //TODO Will only return true if they are in the same order. They should usually be but should probably fix this
                {
                    return type.Type;
                }
                
            }

            return ReportType;
        }

        public string GetParentReportType(Report Report, String Document)
        {
            var dbService = new MillimanService();

            string RootParentDirectory = Document.Substring(0, Document.IndexOf("REDUCEDCACHEDQVWS"));

            var ParentRootQuery = dbService.GetAuditLogs<AuditLog>(a => a.Document.Contains(RootParentDirectory)).Distinct();

            foreach(AuditLog Log in ParentRootQuery)
            {
                if(RootParentDirectory.Count(x => x == '\\') == Log.Document.Count(x => x == '\\'))
                {
                    string ParentReportName = Log.Document.Substring(Log.Document.LastIndexOf('\\')+1, Log.Document.Length - Log.Document.LastIndexOf('\\') - 5);
                    return ParentReportName;
                }
            }

            return "";
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
                ExceptionLogger.LogError(ex, "Exception Raised Method AddOrGetGroup.", "Common Controller Exception");
            }

            return obj;
        }
        public List<Group> GetGroupList()
        {
            //initiate service
            var dbService = new MillimanService();
            var objList = new List<Group>();
            try
            {
                var list = dbService.GetGroups<Group>().OrderBy(o=>o.GroupName).ToList();
                if (list.Count > 0)
                {
                    objList = list;
                }
                dbService.Dispose();
            }
            catch (Exception ex)
            {
                if(dbService != null)
                {
                    dbService.Dispose();
                }                
                ExceptionLogger.LogError(ex, "Exception Raised in Method GroupListGet.", "Common Controller Exception");
            }

            return objList;
        }
        #endregion
    }
}
