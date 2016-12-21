using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanReportReduction
{
    public class QVWReductionProcessor
    {
        //SelectionsPerQVW ==
        //[0]: "0|C:\\inetpub\\wwwroot\\InstalledApplications\\MillimanSite\\QVDocuments\\Demo\\DemoProject.qvw|DemoProject"
        //[1]: "1|mem_report_hier_2|exc_mem_report_hier_2|Provider Location City 0001"
        //[2]: "1|mem_report_hier_2|exc_mem_report_hier_2|Provider Location City 0002"
        //[3]: "2|mem_report_hier_3|exc_mem_report_hier_3|Provider Location Name 0002"
        //[4]: "2|mem_report_hier_3|exc_mem_report_hier_3|Provider Location Name 0007"

        public bool ProcessUsers(string GroupID, List<string> CreatedUsers, List<MillimanCommon.TreeToFileUtilities.ReductionSelections> SelectionsPerQVW, bool AllNodesSelected)
        {
            //per Issue1569 we may be called with no selections to be made,  thus we do not reduce any items
            if ((SelectionsPerQVW == null) || (SelectionsPerQVW.Count() == 0))
                return true;

            bool ReturnValue = false;
            bool MustAuthorize = false;
            //VWN if no users created or no selections is this an error
            if ((CreatedUsers.Count > 0) && (SelectionsPerQVW.Count > 0))
            {
                string StatusMessage = string.Empty;
                foreach (MillimanCommon.TreeToFileUtilities.ReductionSelections RS in SelectionsPerQVW)
                {
                    if (AllNodesSelected == false)
                    {
                        //VWN if redirector exists in cache, write it for user
                        string TempFile = RS.WriteSelectionsToTemp();
                        QVWCaching QVWC = new QVWCaching(RS.QualafiedQVW);
                        string CachedVersion = QVWC.FindReducedQVW(TempFile);
                        if (string.IsNullOrEmpty(CachedVersion) == false)
                        {
                            //for each user write the redirector file
                            foreach (string User in CreatedUsers)
                                QVWCaching.WriteQVWRedirector(RS.QualafiedQVW, User, CachedVersion);

                            ReturnValue = true;
                        }
                        else
                        {  //must reduce and write redirectory
                            string ReducedQVW = ReduceQVW(RS.QualafiedQVW, RS.KeyValuePairs, CreatedUsers[0], out StatusMessage);
                            if (string.IsNullOrEmpty(ReducedQVW) == false)
                            {
                                MustAuthorize = true;
                                ReturnValue = true;
                                string CacheVersion = QVWC.MoveNewQVWToCache(TempFile, ReducedQVW);
                                //VWN NOTE: copy bookmark over here????
                                foreach (string User in CreatedUsers)
                                    QVWCaching.WriteQVWRedirector(RS.QualafiedQVW, User, CacheVersion);
                            }
                        }
                    }
                    else
                    {
                        //for each user write the redirector file back to master QVW
                        foreach (string User in CreatedUsers)
                            QVWCaching.WriteQVWRedirector(RS.QualafiedQVW, User, RS.QualafiedQVW);
                        ReturnValue = true;
                    }

                }
            }
            if (MustAuthorize)
            {
                return AuthorizeAllQVWs();
            }
            return ReturnValue;
        }

        static public bool AuthorizeAllQVWs()
        {
            try
            {
                QVSAPI QVS = new QVSAPI();
                bool Verified;
                string ReturnValue = QVS.AuthorizeAllDocuments("", out Verified);
                if (string.IsNullOrEmpty(ReturnValue))
                    return true;
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to authorize QVWs");
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "AuthorizeAllQVWs failed", ex);
            }
            return false;
        }

        public string ReduceQVW(string MasterQVWFilePath, List<MillimanCommon.TreeToFileUtilities.KeyValuePair> Selections, string UserName, out string StatusMessage)
        {
            StatusMessage = string.Empty;
            //VWN call to check for cached version of QVW instead of re-createing each time

            MillimanReportReduction.QVWReducer QVWR = new MillimanReportReduction.QVWReducer();
            QVWR.QualifiedQVWNameToReduce = MasterQVWFilePath;
            //we have to create the reduced version in temp dir and then copy back since publisher
            //will not allow us to create a QVW that does not have the ".QVW" extension, duh!
            QVWR.QualifiedReducedQVWName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetTempFileName().ToLower().Replace(".tmp", ".qvw"));

            foreach (MillimanCommon.TreeToFileUtilities.KeyValuePair KVP in Selections)
            {
                //do an httpdecode on values,  they went though XML and HTML to arrive here and may be escaped
                QVWR.Variables.Add(new MillimanReportReduction.NVPairs( System.Web.HttpUtility.HtmlDecode( KVP.Key ), System.Web.HttpUtility.HtmlDecode( KVP.Value)));
            }
            QVWR.TaskName = UserName + " Limiting View";
            QVWR.TaskDescription = "Limiting data access for " + UserName;
            QVWR.DeleteTaskOnCompletion = true;
            if (QVWR.ReduceBlocking() == false)
            {
                StatusMessage = QVWR.TaskStatusMsg;
                return ""; //reduction failed
            }
            //it was a success, so copy the temp file over
            string RequestedQualifiedReducedQVWName = MillimanCommon.ReducedQVWUtilities.GetUserDir(System.IO.Path.GetDirectoryName(MasterQVWFilePath), UserName);
            RequestedQualifiedReducedQVWName = System.IO.Path.Combine(RequestedQualifiedReducedQVWName, System.IO.Path.GetFileName(MasterQVWFilePath) + "_tmp");
            System.IO.File.Delete(RequestedQualifiedReducedQVWName);
            System.IO.File.Copy(QVWR.QualifiedReducedQVWName, RequestedQualifiedReducedQVWName);
            if (System.IO.File.Exists(RequestedQualifiedReducedQVWName) == false)
                return "";
            return RequestedQualifiedReducedQVWName;
        }
    }
}
