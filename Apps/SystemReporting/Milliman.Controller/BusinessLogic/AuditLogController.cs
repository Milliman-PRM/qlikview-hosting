using SystemReporting.Data.Repository;
using SystemReporting.Entities.Models;
using SystemReporting.Entities.Proxy;
using SystemReporting.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SystemReporting.Controller.BusinessLogic.Controller
{
    [Serializable]
    public class AuditLogController : ControllerBase
    {
        private IRepository<AuditLog> LogRepository = null;

        public AuditLogController()
        {
            LogRepository = new Repository<AuditLog>();
        }

        public AuditLogController(IRepository<AuditLog> repository)
        {
            LogRepository = repository;
        }

        public bool ProcessLogs(List<ProxyAuditLog> listProxyLogs)
        {
            bool blnSucessful = false;
            MillimanService serviceMilliman = new MillimanService();
            try
            {
                AuditLog logEntity = new AuditLog();
                foreach (var entry in listProxyLogs)
                {
                    logEntity.UserAccessDatetime = string.IsNullOrEmpty(entry.UserAccessDatetime) ? (DateTime?)null : DateTime.Parse(entry.UserAccessDatetime);
                    logEntity.Document = (!string.IsNullOrEmpty(entry.Document)) ? entry.Document.Trim() : string.Empty;
                    logEntity.EventType = (!string.IsNullOrEmpty(entry.EventType)) ? entry.EventType.Trim() : string.Empty;

                    #region User

                    ///Insert User
                    if (!string.IsNullOrEmpty(entry.User))
                    {
                        User user = new User();
                        var userExist = serviceMilliman.GetUsers<User>(u => u.UserName == entry.User).FirstOrDefault();
                        if (userExist == null)
                        {
                            user.UserName = entry.User.Trim();
                            ControllerCommon.SaveUser(user);
                            user = new User();
                            
                            //after insert set the id
                            User userFound = serviceMilliman.GetUsers<User>(a => a.UserName == entry.User).FirstOrDefault();
                            logEntity.fk_user_id = userFound.Id;
                        }
                        else
                        {
                            logEntity.fk_user_id = userExist.Id;
                        }
                    }

                    #endregion
                    logEntity.Message = (!string.IsNullOrEmpty(entry.Message)) ? entry.Message.Trim() : string.Empty;

                    #region Report / Group

                    //Insert Group
                    if (!string.IsNullOrEmpty(entry.Group))
                    {
                        Group group = new Group();
                        var groupExist = serviceMilliman.GetGroups<Group>(a => a.GroupName == entry.Group).FirstOrDefault();
                        if (groupExist == null)
                        {
                            group.GroupName = entry.Group;
                            ControllerCommon.SaveGroup(group);
                            group = new Group();

                            //after insert set the id
                            Group groupFound = serviceMilliman.GetGroups<Group>(a => a.GroupName == entry.Group).FirstOrDefault();
                            logEntity.fk_group_id = groupFound.Id;
                        }
                        else
                        {
                            logEntity.fk_group_id = groupExist.Id;
                        }
                    }

                    //Insert Report
                    if (!string.IsNullOrEmpty(entry.Report))
                    {
                        Report report = new Report();
                        var reportExist = serviceMilliman.GetReports<Report>(a => a.ReportName == entry.Report).FirstOrDefault();
                        if (reportExist == null)
                        {
                            report.ReportName = entry.Report.Trim();
                            ControllerCommon.SaveReport(report);
                            report = new Report();

                            //after insert set the id
                            Report reportFound = serviceMilliman.GetReports<Report>(a => a.ReportName == entry.Report).FirstOrDefault();
                            logEntity.fk_report_id = reportFound.Id;
                        }
                        else
                        {
                            logEntity.fk_report_id = reportExist.Id;
                        }
                    }                   

                    #endregion

                    logEntity.AddDate = DateTime.Now;

                    //5. Insert record in the table     
                    serviceMilliman.Save(logEntity);
                    logEntity = new AuditLog();
                    blnSucessful = true;
                }
            }
            catch (Exception ex)
            {
                LogError(DateTime.Now + " ||-|| "
                                        + "Class AuditLogController. Method ProcessLogs." + Environment.NewLine
                                        + ex.Message + " ||-|| " + ex.InnerException + Environment.NewLine);
            }
            return blnSucessful;
        }



    }
}
