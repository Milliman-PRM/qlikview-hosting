using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milliman
{
    public class DebugHelper
    {
        static public string MethodHelper( string MethodName, params string[] Arguments )
        {
            string Output = MethodName + "(";
            string Args = string.Empty;
            foreach (string S in Arguments)
            {
                if (Args != "")
                    Args += ",";
                Args += S;
            }

            return Output + Args + ")";
        }

        static public string ParamterListHelper(List<string> MyList)
        {
            string Output = string.Empty;
            foreach (string S in MyList)
            {
                if (Output != "")
                    Output += ",";
                Output += S;
            }
            return "[" + Output + "]";
        }
        static public string ParamterListHelper(List<Guid> MyList)
        {
            string Output = string.Empty;
            foreach (Guid S in MyList)
            {
                if (Output != "")
                    Output += ",";
                Output += S.ToString();
            }
            return "[" + Output + "]";
        }
        /// <summary>
        /// Make sure the group is marked as external( found in ExternalGroup file )
        /// and has correct owner ( determined by system account context )
        /// and exits in membership system for server
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="Owner"></param>
        /// <returns></returns>
        static public bool IsValidExternallyOwnedGroup(string GroupName, string Owner)
        {
            Milliman.Data.ExternalSystemGroups ESD = Milliman.Data.ExternalSystemGroups.GetInstance();
            if (ESD != null)
            {
                if (ESD.NextGenAssociatedGroups.ContainsKey(GroupName) == true)
                {
                    if (string.Compare(ESD.NextGenAssociatedGroups[GroupName].Owner, Owner, true) == 0)
                    {
                        if (System.Web.Security.Roles.RoleExists(ESD.NextGenAssociatedGroups[GroupName].InternalGroupID) == false)
                        {
                            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Info, "Requested group: " + GroupName + " is tagged as external an has appropriate owner, but group was not found in system membership system ");
                            Milliman.Global.AddSystemMsg(Global.MsgType.ERROR, "Requested group: " + GroupName + " is tagged as external an has appropriate owner, but group was not found in system membership system ");
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Info, "Requested group: " + GroupName + " is not tagged as owned by " + Owner);
                        Milliman.Global.AddSystemMsg(Global.MsgType.ERROR, "Requested group: " + GroupName + " is not tagged as owned by " + Owner);
                    }
                }
                else
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Info, "Requested group: " + GroupName + " is not tagged as external group.");
                    Milliman.Global.AddSystemMsg(Global.MsgType.ERROR, "Requested group: " + GroupName + " is not tagged as external group.");
                }
            }
            return false;
        }
    }
}