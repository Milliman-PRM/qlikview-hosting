using SystemReporting.Data.Database;
using SystemReporting.Data.Repository;
using SystemReporting.Entities.Models;
using SystemReporting.Entities.Proxy;
using SystemReporting.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SystemReporting.Utilities;

namespace SystemReporting.Controller.BusinessLogic.Controller
{
    public class IisLogController : ControllerBase
    {
        private IRepository<IisLog> LogRepository = null;

        public IisLogController()
        {
            LogRepository = new Repository<IisLog>();
        }

        public IisLogController(IRepository<IisLog> repository)
        {
            LogRepository = repository;
        }

        public bool ProcessLogs(List<ProxyIisLog> listProxyLogs)
        {
            bool blnSucessful = false;
            MillimanService serviceMilliman = new MillimanService();
            try
            {
                IisLog logEntity = new IisLog();
                foreach (var entry in listProxyLogs)
                {
                    //DateTime userAccessDatetime = Convert.ToDateTime(entry.UserAccessDatetime);
                    //string userAccessDatetimeStr = userAccessDatetime.ToString("MM-dd-yyyy hh:mm:ss tt");
                    logEntity.UserAccessDatetime = string.IsNullOrEmpty(entry.UserAccessDatetime) ? (DateTime?)null : DateTime.Parse(entry.UserAccessDatetime);
                    logEntity.ClientIpAddress = (!string.IsNullOrEmpty(entry.ClientIpAddress)) ? entry.ClientIpAddress.Trim() : string.Empty;

                    #region User
                    //Insert User
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

                    logEntity.ServerIPAddress = (!string.IsNullOrEmpty(entry.ServerIPAddress)) ? entry.ServerIPAddress.Trim() : string.Empty;
                    logEntity.PortNumber = entry.PortNumber != null ? entry.PortNumber : null;
                    logEntity.CommandSentMethod = (!string.IsNullOrEmpty(entry.CommandSentMethod)) ? entry.CommandSentMethod.Trim() : string.Empty;
                    logEntity.StepURI = (!string.IsNullOrEmpty(entry.StepURI)) ? entry.StepURI.Trim() : string.Empty;
                    logEntity.QueryURI = (!string.IsNullOrEmpty(entry.QueryURI)) ? entry.QueryURI.Trim() : string.Empty;

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

                    #endregion

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
                  
                    //5. Insert record in the table     
                    serviceMilliman.Save(logEntity);
                    logEntity = new IisLog();
                    blnSucessful = true;
                }
            }
            catch (Exception ex)
            {
                Logger(ex, "Class IisLogController. Method ProcessLogs.");
            }
            return blnSucessful;
        }
        private void Logger(Exception ex, string message)
        {
            if (ex == null)
                ex = null;
            if (string.IsNullOrEmpty(message))
                message = "Error";

            LogError(DateTime.Now + " ||-|| " + message + Environment.NewLine 
                                  + ex.Message + " ||-|| " + ex.InnerException + Environment.NewLine);
        }
    }
}
