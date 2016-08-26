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

                    int? reportTypeId;
                    //If the report does not have a report type then it is set to null
                    reportTypeId = getReportTypeID(Report, Document);
                    obj.fk_report_type_id = reportTypeId.HasValue ? reportTypeId.Value : (int?)null;
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

        #region Report Type
        /// <summary>
        /// Takes the report name and matches it up with keywords associated with types
        /// in the reporttype table in the database.
        /// </summary>
        /// <param name="Report"></param>
        /// <param name="Document"></param>
        /// <returns></returns>
        public int? getReportTypeID(Report Report, string Document)
        {
            int? reportTypeId = null; 
            try
            {
                var dbService = new MillimanService();

                List<string> reportNameTokens;
                if (Report.ReportName.Any(c => char.IsDigit(c)) && !Report.ReportName.Contains(' '))
                {
                    reportNameTokens = getParentReportType(Document).Split(' ').ToList();
                }
                else
                {
                    reportNameTokens = Report.ReportName.Split(' ').ToList();
                }

                var reportTypeList = dbService.GetReportTypes<ReportType>().OrderBy(o => o.Type).ToList();

                if (reportTypeList.Count > 0)
                {
                    foreach (ReportType reportType in reportTypeList)
                    {
                        List<String> KeywordTokens = reportType.Keywords.Split(',').ToList();
                        List<String> CommonTokens = KeywordTokens.Intersect(reportNameTokens).ToList();

                        if (CommonTokens.SequenceEqual(KeywordTokens))
                        {
                            dbService.Dispose();
                            reportTypeId = reportType.Id;
                        }
                    }
                }

                dbService.Dispose();
            }
            catch(Exception ex)
            {
                ExceptionLogger.LogError(ex, "Exception Raised in Common Controller.", "ReportType Exceptions");
            }

            //If report does not match up to any report type
            return reportTypeId;
        }
        /// <summary>
        /// Matches reports with GUID's to their
        /// parent report
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public string getParentReportType(string document)
        {
            var dbService = new MillimanService();
            string rootParentDirectory;
            string parentReportName=string.Empty;

            if (document.IndexOf("REDUCEDCACHEDQVWS") < 0) { 
                return "";
            }

            rootParentDirectory = document.Substring(0, document.IndexOf("REDUCEDCACHEDQVWS"));

            var parentAuditLogRootQuery = dbService.GetAuditLogs<AuditLog>(a => a.Document.Contains(rootParentDirectory)).OrderBy(a => a.Id);
            if (parentAuditLogRootQuery != null)
            {
                foreach (AuditLog aulog in parentAuditLogRootQuery)
                {
                    if (aulog.Document.IndexOf("REDUEDCACHEDQVWS") < 0)
                    {
                        parentReportName = aulog.Document.Substring(aulog.Document.LastIndexOf('\\') + 1, aulog.Document.Length - aulog.Document.LastIndexOf('\\') - 5);
                    }
                }
            }

            var parentSessionLogRootQuery = dbService.GetSessionLogs<SessionLog>(a => a.Document.Contains(rootParentDirectory)).OrderBy(a => a.Id);
            if (parentSessionLogRootQuery != null)
            {
                foreach (SessionLog slog in parentSessionLogRootQuery)
                {
                    if (slog.Document.IndexOf("REDUEDCACHEDQVWS") < 0)
                    {
                        parentReportName = slog.Document.Substring(slog.Document.LastIndexOf('\\') + 1, slog.Document.Length - slog.Document.LastIndexOf('\\') - 5);
                    }
                }
            }

            dbService.Dispose();
            return parentReportName;
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

        public Group GetGroupById(string id)
        {
            var dbService = new MillimanService();
            var obj = new Group();
            try
            {                
                int objId = int.Parse(id);
                obj = dbService.GetGroups<Group>(a => a.Id == objId).FirstOrDefault();
                dbService.Dispose();
            }
            catch (Exception ex)
            {
                if (dbService != null)
                {
                    dbService.Dispose();
                }
                ExceptionLogger.LogError(ex, "Exception Raised in Method GetGroupById.", "Common Controller Exception");
            }

            return obj;
        }
        public Report GetReportById(string id)
        {
            var dbService = new MillimanService();
            var obj = new Report();
            try
            {
                int objId = int.Parse(id);
                obj = dbService.GetReports<Report>(a => a.Id == objId).FirstOrDefault();
                dbService.Dispose();
            }
            catch (Exception ex)
            {
                if (dbService != null)
                {
                    dbService.Dispose();
                }
                ExceptionLogger.LogError(ex, "Exception Raised in Method GetGroupById.", "Common Controller Exception");
            }
            return obj;
        }
        public User GetUserById(string id)
        {
            var dbService = new MillimanService();
            var obj = new User();
            try
            {
                int objId = int.Parse(id);
                obj = dbService.GetUsers<User>(a => a.Id == objId).FirstOrDefault();
                dbService.Dispose();
            }
            catch (Exception ex)
            {
                if (dbService != null)
                {
                    dbService.Dispose();
                }
                ExceptionLogger.LogError(ex, "Exception Raised in Method GetGroupById.", "Common Controller Exception");
            }
            return obj;
        }
    }
}
