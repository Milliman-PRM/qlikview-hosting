using Milliman.Data.Repository;
using Milliman.Entities.Models;
using Milliman.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Milliman.Controller.BusinessLogic.Controller
{
    [Serializable]
    public class SessionLogController : ControllerBase
    {
        private IRepository<SessionLog> LogRepository = null;

        public SessionLogController()
        {
            LogRepository = new Repository<SessionLog>();
        }

        public SessionLogController(IRepository<SessionLog> repository)
        {
            LogRepository = repository;
        }

        public bool ProcessLogs(List<ProxySessionLog> listProxyLogs)
        {
            bool blnSucessful = false;
            MillimanService serviceMilliman = new MillimanService();
            try
            {
                SessionLog logEntity = new SessionLog();
                foreach (var entry in listProxyLogs)
                {                   
                    logEntity.Document = (!string.IsNullOrEmpty(entry.Document)) ? entry.Document.Trim() : string.Empty;
                    logEntity.ExitReason = (!string.IsNullOrEmpty(entry.ExitReason)) ? entry.ExitReason.Trim() : string.Empty;
                    logEntity.SessionStartTime = string.IsNullOrEmpty(entry.SessionStartTime) ? (DateTime?)null : DateTime.Parse(entry.SessionStartTime);
                    logEntity.SessionDuration = string.IsNullOrEmpty(entry.SessionLength) ? (int?)null : int.Parse(entry.SessionLength);
                    logEntity.SessionEndReason = (!string.IsNullOrEmpty(entry.SessionEndReason)) ? entry.SessionEndReason.Trim() : string.Empty;
                    logEntity.CpuSpentS = entry.CpuSpentS != null ? entry.CpuSpentS : 0.0;
                   
                    //special 
                    logEntity.IdentifyingUser = (!string.IsNullOrEmpty(entry.IdentifyingUser)) ? entry.IdentifyingUser : string.Empty;
                    #region User
                    //Insert User
                    if (!string.IsNullOrEmpty(entry.IdentifyingUser))
                    {
                        User user = new User();
                        var userExist = serviceMilliman.GetUsers<User>(u => u.UserName == entry.IdentifyingUser).FirstOrDefault();
                        if (userExist == null)
                        {
                            user.UserName = entry.IdentifyingUser.Trim();
                            ControllerCommon.SaveUser(user);
                            user = new User();

                            logEntity.IdentifyingUser = entry.IdentifyingUser.Trim();
                            //after insert set the id
                            User userFound = serviceMilliman.GetUsers<User>(a => a.UserName == entry.IdentifyingUser).FirstOrDefault();
                            logEntity.fk_user_id = userFound.Id;
                        }
                        else
                        {
                            logEntity.IdentifyingUser = userExist.UserName.Trim();
                            logEntity.fk_user_id = userExist.Id;
                        }
                    }

                    #endregion

                    logEntity.ClientType = (!string.IsNullOrEmpty(entry.ClientType)) ? entry.ClientType.Trim() : string.Empty;
                    logEntity.ClientAddress = (!string.IsNullOrEmpty(entry.ClientAddress)) ? entry.ClientAddress.Trim() : string.Empty;
                    logEntity.CalType = (!string.IsNullOrEmpty(entry.CalType)) ? entry.CalType.Trim() : string.Empty;
                    logEntity.CalUsageCount = entry.CalUsageCount.HasValue ? entry.CalUsageCount.Value : 0;

                    logEntity.Browser = (!string.IsNullOrEmpty(entry.Browser)) ? entry.Browser.Trim() : string.Empty;
                    logEntity.IsReduced = entry.IsReduced;
                    
                    #region Report / Group

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
                    #endregion
                                    
                    logEntity.AddDate = DateTime.Now;
                    //5. Insert record in the table     
                    serviceMilliman.Save(logEntity);
                    logEntity = new SessionLog();
                    blnSucessful = true;
                }
            }
            catch (Exception ex)
            {
                LogError(DateTime.Now + " ||-|| "
                        + "Class SessionLogController. Method ProcessLogs." + Environment.NewLine
                        + ex.Message + " ||-|| " + ex.InnerException + Environment.NewLine);
            }
            return blnSucessful;
        }



    }
}
