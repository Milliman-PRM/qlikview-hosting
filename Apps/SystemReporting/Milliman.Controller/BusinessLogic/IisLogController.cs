using System;
using System.Collections.Generic;
using System.Linq;
using SystemReporting.Entities.Models;
using SystemReporting.Entities.Proxy;
using SystemReporting.Service;
using SystemReporting.Utilities.ExceptionHandling;

namespace SystemReporting.Controller.BusinessLogic.Controller
{
    public class IisLogController : ControllerBase
    {
         private IMillimanService dbService { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public IisLogController()  { }

        /// <summary>
        /// Method to process the logs list and enter data to db
        /// This will insert data into User, Group, Report and Session log
        /// </summary>
        /// <param name="listProxyLogs"></param>
        /// <returns></returns>
        public bool ProcessLogs(List<ProxyIisLog> listProxyLogs)
        {
            var blnSucessful = false;            
            try
            {
                var logEntity = new IisLog();
                foreach (var entry in listProxyLogs)
                {
                    logEntity.UserAccessDatetime = string.IsNullOrEmpty(entry.UserAccessDatetime) ? (DateTime?)null : DateTime.Parse(entry.UserAccessDatetime);
                    logEntity.ClientIpAddress = (!string.IsNullOrEmpty(entry.ClientIpAddress)) ? entry.ClientIpAddress.Trim() : string.Empty; 
                    logEntity.ServerIPAddress = (!string.IsNullOrEmpty(entry.ServerIPAddress)) ? entry.ServerIPAddress.Trim() : string.Empty;
                    logEntity.PortNumber = entry.PortNumber != null ? entry.PortNumber : null;
                    logEntity.CommandSentMethod = (!string.IsNullOrEmpty(entry.CommandSentMethod)) ? entry.CommandSentMethod.Trim() : string.Empty;
                    logEntity.StepURI = (!string.IsNullOrEmpty(entry.StepURI)) ? entry.StepURI.Trim() : string.Empty;
                    logEntity.QueryURI = (!string.IsNullOrEmpty(entry.QueryURI)) ? entry.QueryURI.Trim() : string.Empty;
                    logEntity.StatusCode = entry.StatusCode != null ? entry.StatusCode : null;
                    logEntity.SubStatusCode = entry.SubStatusCode != null ? entry.SubStatusCode : null;
                    logEntity.Win32StatusCode = entry.Win32StatusCode != null ? entry.Win32StatusCode : null;
                    logEntity.ResponseTime = entry.ResponseTime != null ? entry.ResponseTime : null;
                    logEntity.UserAgent = (!string.IsNullOrEmpty(entry.UserAgent)) ? entry.UserAgent.Trim() : string.Empty;
                    //Need some rule around this
                    logEntity.ClientReferer = (!string.IsNullOrEmpty(entry.ClientReferer)) ? entry.ClientReferer.Trim() : string.Empty;
                    logEntity.Browser = (!string.IsNullOrEmpty(entry.Browser)) ? entry.Browser.Trim() : string.Empty;
                    logEntity.EventType = (!string.IsNullOrEmpty(entry.EventType)) ? entry.EventType.Trim() : string.Empty;
                    logEntity.AddDate = DateTime.Now;
                    
                    #region User / Report / Group

                    //Insert User
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
                    #endregion

                    //initiate service
                    dbService = new MillimanService();
                    //5. Insert record in the table   
                    dbService.Save(logEntity); 
                    dbService.Dispose();
                    logEntity = new IisLog();
                    blnSucessful = true;
                }
            }
            catch (Exception ex)
            {
                dbService.Dispose();
                ExceptionLogger.LogError(ex, "Exception Raised in Method ProcessLogs.", "IisLog Controller Exception");
            }
            return blnSucessful;
        }

        /// <summary>
        /// get session log report by Group name
        /// </summary>
        /// <param name="date"></param>
        /// <param name="reportName"></param>
        /// <param name="startDate">todo: describe startDate parameter on GetSessionLogListForGroup</param>
        /// <param name="endDate">todo: describe endDate parameter on GetSessionLogListForGroup</param>
        /// <returns></returns>
        public List<IisLog> GetIisLogListForGroup(string startDate, string endDate, string paramId)
        {
            var dbService = new MillimanService();
            DateTime? dtStartDate = DateTime.Parse(startDate);
            DateTime? dtEndDate = DateTime.Parse(endDate);

            var obj = new Group();
            obj = ControllerCommon.GetGroupById(paramId);

            var listResult = new List<IisLog>();
            var listFinalResult = new List<IisLog>();
            if (obj != null)
            {
                listResult = dbService.GetIisLogs<IisLog>(s => s.fk_group_id == obj.Id
                                                &&
                                                s.UserAccessDatetime.Value > dtStartDate
                                                &&
                                                s.UserAccessDatetime.Value <= dtEndDate)
                                                .OrderBy(a => a.UserAccessDatetime).ToList();

                var logEntity = new IisLog();
                foreach (var entry in listResult)
                {
                    logEntity.UserAccessDatetime = entry.UserAccessDatetime.HasValue ? entry.UserAccessDatetime.Value : (DateTime?)null;
                    logEntity.ClientIpAddress = (!string.IsNullOrEmpty(entry.ClientIpAddress)) ? entry.ClientIpAddress.Trim() : string.Empty;
                    logEntity.ServerIPAddress = (!string.IsNullOrEmpty(entry.ServerIPAddress)) ? entry.ServerIPAddress.Trim() : string.Empty;
                    logEntity.PortNumber = entry.PortNumber != null ? entry.PortNumber : null;
                    logEntity.CommandSentMethod = (!string.IsNullOrEmpty(entry.CommandSentMethod)) ? entry.CommandSentMethod.Trim() : string.Empty;
                    logEntity.StepURI = (!string.IsNullOrEmpty(entry.StepURI)) ? entry.StepURI.Trim() : string.Empty;
                    logEntity.QueryURI = (!string.IsNullOrEmpty(entry.QueryURI)) ? entry.QueryURI.Trim() : string.Empty;
                    logEntity.StatusCode = entry.StatusCode != null ? entry.StatusCode : null;
                    logEntity.SubStatusCode = entry.SubStatusCode != null ? entry.SubStatusCode : null;
                    logEntity.Win32StatusCode = entry.Win32StatusCode != null ? entry.Win32StatusCode : null;
                    logEntity.ResponseTime = entry.ResponseTime != null ? entry.ResponseTime : null;
                    logEntity.UserAgent = (!string.IsNullOrEmpty(entry.UserAgent)) ? entry.UserAgent.Trim() : string.Empty;
                    //Need some rule around this
                    logEntity.ClientReferer = (!string.IsNullOrEmpty(entry.ClientReferer)) ? entry.ClientReferer.Trim() : string.Empty;
                    logEntity.Browser = (!string.IsNullOrEmpty(entry.Browser)) ? entry.Browser.Trim() : string.Empty;
                    logEntity.EventType = (!string.IsNullOrEmpty(entry.EventType)) ? entry.EventType.Trim() : string.Empty;

                    logEntity.fk_user_id = entry.fk_user_id.HasValue ? entry.fk_user_id.Value : (int?)null;
                    logEntity.User = entry.User != null ? entry.User : null;

                    logEntity.fk_group_id = entry.fk_group_id.HasValue ? entry.fk_group_id.Value : (int?)null;
                    logEntity.Group = entry.Group != null ? entry.Group : null;

                    listFinalResult.Add(logEntity);
                    logEntity = new IisLog();
                }
            }
            return listFinalResult;
        }
       
        /// <summary>
        /// get a session log report by user
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="reportName"></param>
        /// <returns></returns>
        public  List<IisLog> GetIisLogListForUser(string startDate, string endDate, string paramId)
        {
            var dbService = new MillimanService();
            var listResult = new List<IisLog>();

            var obj = new User();
            int id = int.Parse(paramId);
            obj = dbService.GetUsers<User>(a => a.Id == id).FirstOrDefault();

            DateTime? dtStartDate = DateTime.Parse(startDate);
            DateTime? dtEndDate = DateTime.Parse(endDate);

            if (obj != null)
            {
                listResult = dbService.GetIisLogs<IisLog>(s => s.fk_user_id == obj.Id
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
