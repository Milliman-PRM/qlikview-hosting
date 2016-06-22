using System;
using System.Collections.Generic;
using SystemReporting.Entities.Models;

namespace ReportFileGenerator
{
    public class BusinessLogicController : ControllerAccess
    {
        public static List<Group> GetGroupList()
        {
            var c = new ControllerAccess();
            var objlist = new List<Group>();
            objlist = c.ControllerCommon.GetGroupList();
            return objlist;
        }

        public static List<Report> GetReportList()
        {
            var c = new ControllerAccess();
            var objlist = new List<Report>();
            objlist = c.ControllerCommon.GetReportList();
            return objlist;
        }

        public static List<User> GetUserList()
        {
            var c = new ControllerAccess();
            var objlist = new List<User>();
            objlist = c.ControllerCommon.GetUserList();
            return objlist;
        }


        public static List<AuditLog> GetAuditLogListForGroup(DateTime? startDate, DateTime? endDate,
                                                                string reportName)
        {
            var c = new ControllerAccess();
            var objlist = new List<AuditLog>();
            objlist = c.ControllerAuditLog.GetAuditLogListForGroup(startDate.Value.ToString()
                                                                    , endDate.Value.ToString(), reportName);
            return objlist;
        }

        public static List<AuditLog> GetAuditLogListForReport(DateTime? startDate, DateTime? endDate,
                                                                string reportName)
        {
            var c = new ControllerAccess();
            var objlist = new List<AuditLog>();
            objlist = c.ControllerAuditLog.GetAuditLogListForReport(startDate.Value.ToString()
                                                                    , endDate.Value.ToString(), reportName);
            return objlist;
        }

        public static List<AuditLog> GetAuditLogListForUser(DateTime? startDate, DateTime? endDate,
                                                        string reportName)
        {
            var c = new ControllerAccess();
            var objlist = new List<AuditLog>();
            objlist = c.ControllerAuditLog.GetAuditLogListForUser(startDate.Value.ToString()
                                                                    , endDate.Value.ToString(), reportName);
            return objlist;
        }

    }
}
