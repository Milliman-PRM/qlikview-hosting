using System;
using System.Collections.Generic;
using System.Linq;
using SystemReporting.Entities.Models;
using SystemReporting.Entities.Proxy;
using SystemReporting.Service;
using SystemReporting.Utilities.ExceptionHandling;

namespace SystemReporting.Controller.BusinessLogic.Controller
{
    [Serializable]
    public class AuditLogController : ControllerBase
    {      
        private IMillimanService dbService { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AuditLogController() { }

        /// <summary>
        /// Method to process the logs list and enter data to db
        /// This will insert data into User, Group, Report and IIsLogs
        /// </summary>
        /// <param name="listProxyLogs"></param>
        /// <returns></returns>
        public bool ProcessLogs(List<ProxyAuditLog> listProxyLogs)
        {           
            var blnSucessful = false;
            try
            {
                var logEntity = new AuditLog();
                foreach (var entry in listProxyLogs)
                {
                    logEntity.UserAccessDatetime = string.IsNullOrEmpty(entry.UserAccessDatetime) ? (DateTime?)null : DateTime.Parse(entry.UserAccessDatetime);
                    logEntity.Document = (!string.IsNullOrEmpty(entry.Document)) ? entry.Document.Trim() : string.Empty;
                    logEntity.EventType = (!string.IsNullOrEmpty(entry.EventType)) ? entry.EventType.Trim() : string.Empty;
                    logEntity.Message = (!string.IsNullOrEmpty(entry.Message)) ? entry.Message.Trim() : string.Empty;
                    logEntity.IsReduced = (entry.IsReduced);
                    logEntity.AddDate = DateTime.Now;

                    #region User/ Report / Group

                    ///Insert User
                    if (!string.IsNullOrEmpty(entry.User))
                    {
                        var user = new User
                        {
                            UserName = entry.User.Trim()
                        };
                        var addOrGetUser = ControllerCommon.AddOrGetUser(user);
                        if (addOrGetUser != null)
                        {
                            //after insert set the id
                            logEntity.fk_user_id = addOrGetUser.Id;
                        }
                    }

                    //Insert Group
                    if (!string.IsNullOrEmpty(entry.Group))
                    {
                        var group = new Group
                        {
                            GroupName = entry.Group.Trim()
                        };
                        var addOrGetGroup = ControllerCommon.AddOrGetGroup(group);
                        if (addOrGetGroup != null)
                        {
                            //after insert set the id
                            logEntity.fk_group_id = addOrGetGroup.Id;
                        }
                    }

                    //Insert Report
                    if (!string.IsNullOrEmpty(entry.Report))
                    {
                        var report = new Report
                        {
                            ReportName = entry.Report.Trim()
                        };
                        var addOrGetReport = ControllerCommon.AddOrGetReport(report);
                        if (addOrGetReport != null)
                        {
                            //after insert set the id
                            logEntity.fk_report_id = addOrGetReport.Id;
                        }
                    }

                    #endregion

                    //initiate service
                    dbService = new MillimanService();
                    //5. Insert record in the table   
                    dbService.Save(logEntity); 
                    dbService.Dispose();
                    logEntity = new AuditLog();
                    blnSucessful = true;
                }
            }
            catch (Exception ex)
            {
                dbService.Dispose();
                ExceptionLogger.LogError(ex, "Exception Raised in Method ProcessLogs.", "AuditLog Controller Exception");
            }
            return blnSucessful;
        }

        /// <summary>
        /// get session log report by Group name
        /// </summary>
        /// <param name="startDate">todo: describe startDate parameter on GetSessionLogListForGroup</param>
        /// <param name="endDate">todo: describe endDate parameter on GetSessionLogListForGroup</param>
        /// <param name="paramId">todo: describe selectedItemId parameter on GetAuditLogListForGroup</param>
        /// <returns></returns>
        public List<AuditLog> GetAuditLogListForGroup(string startDate, string endDate, string paramId)
        {
            var dbService = new MillimanService();
            var listResult = new List<AuditLog>();

            Group obj = ControllerCommon.GetGroupById(paramId);

            DateTime? dtStartDate = DateTime.Parse(startDate);
            DateTime? dtEndDate = DateTime.Parse(endDate);

            if (obj != null)
            {
                listResult = dbService.GetAuditLogs<AuditLog>(s => s.fk_group_id == obj.Id
                                                                &&
                                                                s.UserAccessDatetime.Value > dtStartDate
                                                                &&
                                                                s.UserAccessDatetime.Value <= dtEndDate)
                                                                .OrderBy(a => a.UserAccessDatetime).ToList();
            }
            return listResult;
        }
        /// <summary>
        /// get session log report by ReportName
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="paramId">todo: describe selectedItemId parameter on GetAuditLogListForReport</param>
        /// <returns></returns>
        public List<AuditLog> GetAuditLogListForReport(string startDate, string endDate, string paramId)
        {
            var dbService = new MillimanService();
            var listResult = new List<AuditLog>();

            Report obj = ControllerCommon.GetReportById(paramId);

            DateTime? dtStartDate = DateTime.Parse(startDate);
            DateTime? dtEndDate = DateTime.Parse(endDate);

            if (obj != null)
            {
                listResult = dbService.GetAuditLogs<AuditLog>(s => s.fk_report_id == obj.Id
                                                &&
                                                s.UserAccessDatetime.Value > dtStartDate
                                                &&
                                                s.UserAccessDatetime.Value <= dtEndDate)
                                                .OrderBy(a => a.UserAccessDatetime).ToList();
            }
            return listResult;
        }

        /// <summary>
        /// get a session log report by user
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="paramId">todo: describe selectedItemId parameter on GetAuditLogListForUser</param>
        /// <returns></returns>
        public List<AuditLog> GetAuditLogListForUser(string startDate, string endDate, string paramId)
        {
            var dbService = new MillimanService();
            var listResult = new List<AuditLog>();

            User obj = ControllerCommon.GetUserById(paramId);

            DateTime? dtStartDate = DateTime.Parse(startDate);
            DateTime? dtEndDate = DateTime.Parse(endDate);

            if (obj != null)
            {
                listResult = dbService.GetAuditLogs<AuditLog>(s => s.fk_user_id == obj.Id
                                                &&
                                                s.UserAccessDatetime.Value > dtStartDate
                                                &&
                                                s.UserAccessDatetime.Value <= dtEndDate)
                                                .OrderBy(a => a.UserAccessDatetime).ToList();
            }
            return listResult;
        }
    }    
}
