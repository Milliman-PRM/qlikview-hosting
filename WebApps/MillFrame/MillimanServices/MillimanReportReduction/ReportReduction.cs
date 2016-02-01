using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanReportReduction
{
    public class ReportReduction
    {
        //factory singleton
        private static ReportReduction instance;
        private static object instance_lock = new object();
        public static ReportReduction GetInstance()
        {
            lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
            {
                if (instance == null)
                    instance = new ReportReduction();
                return instance;
            }
        }
        public static void KillInstance()
        {
            lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
            {
                if (instance != null)
                    instance = null;
            }
        }
        //end singleton

        private string QVDocumentRoot { get; set; }
        private string QVDocumentReducedRoot { get; set; }

        public ReportReduction()
        {
            QVDocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
            QVDocumentReducedRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentReducedRoot"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EnityID">NextGen/Milliman, etc...</param>
        /// <param name="AuditUserID">User logged in requesting the reduction</param>
        /// <param name="QualfiedMasterQVW">path to the QVw to reduce</param>
        /// <param name="UserID">User ID we are reducing for</param>
        /// <param name="ReductionKeys">Keys to reduce on</param>
        /// <param name="NoReduction">Don't reduce just copy</param>
        /// <returns></returns>
        public string Reduce(string EntityID, string AuditUserID, string QualfiedMasterQVW, string UserID, List<NVPairs> ReductionKeys, bool NoReduction = false)
        {
            string RetVal = string.Empty ;

            try
            {
                if (System.IO.File.Exists(QualfiedMasterQVW) == false)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Requested reduction on master qvw: " + QualfiedMasterQVW + " but file does not exist.");
                    return RetVal;
                }
                if (string.IsNullOrEmpty(AuditUserID) == true)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Requested reduction for user " + AuditUserID + ".  But user not found in system membership");
                    return RetVal;
                }
                if ((ReductionKeys == null) || (ReductionKeys.Count == 0))
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Requested to reduce QVW but not reduction critera was provided");
                    return RetVal;
                }

                string AuditMessage = "Reduction request by user: " + AuditUserID + "(" + EntityID + ") for user " + UserID + " against QVW " + QualfiedMasterQVW + " using keys ";
                foreach (NVPairs NVP in ReductionKeys)
                    AuditMessage += " " + NVP.Name + ":" + NVP.Value;
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Info, AuditMessage);

                ReducedReportMasterIndex RRMI = ReducedReportMasterIndex.Load(QualfiedMasterQVW);
                if (RRMI != null)
                {
                    string ReducedQVW = string.Empty;
                    if (NoReduction == false)
                    {
                        ReducedQVW = RRMI.IsReduced(ReductionKeys);
                        if (string.IsNullOrEmpty(ReducedQVW) == true)
                        {
                            //we don't have one so create it
                            QVWReducer Reduce = new QVWReducer();
                            Reduce.QualifiedQVWNameToReduce = QualfiedMasterQVW;
                            Reduce.QualifiedReducedQVWName = GetNameAndCreateDir(QualfiedMasterQVW, UserID);
                            Reduce.Variables = ReductionKeys;
                            Reduce.TaskID = Guid.NewGuid();
                            Reduce.DeleteTaskOnCompletion = true;
                            Reduce.TaskName = AuditUserID + ":" + UserID;
                            Reduce.TaskDescription = AuditUserID + " reducing for " + UserID + " on report " + System.IO.Path.GetFileName(QualfiedMasterQVW);
                            if (Reduce.ReduceBlocking() == true)
                            {
                                ReducedQVW = Reduce.QualifiedReducedQVWName;
                            }
                            else
                            {
                                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Reduction failed for user " + UserID + " because " + Reduce.TaskStatusMsg);
                            }
                        }
                    }
                    else  //just copy over the master, no reduction is required
                    {
                        string ReducedName = GetNameAndCreateDir(QualfiedMasterQVW, UserID);
                        System.IO.File.Copy(QualfiedMasterQVW, ReducedName, true);
                        if (System.IO.File.Exists(ReducedName))
                            ReducedQVW = ReducedName;
                    }

                    if (string.IsNullOrEmpty(ReducedQVW) == false)
                    {
                        if (UpdateReductionIndex(AuditUserID, QualfiedMasterQVW, UserID, ReductionKeys, ReducedQVW) == false)
                            RetVal = string.Empty;
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return RetVal ;
        }

        /// <summary>
        /// Creates the directory the reduced qvw will reside in, and returns its name and qualified path.
        /// </summary>
        /// <param name="MasterQVW"></param>
        /// <returns></returns>
        private string GetNameAndCreateDir(string MasterQVW, string UserID)
        {
            string RetVal = string.Empty;
            try
            {

                string ReducedPathAndName = MillimanCommon.ReducedQVWUtilities.GetUsersReducedQVWName(MasterQVW, UserID);
                
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ReducedPathAndName));

                return ReducedPathAndName;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "GetNameAndCreateDir", ex);
            }

            return RetVal;
        }

        private bool UpdateReductionIndex(string AuditUserID, string MasterQVW, string UserID, List<NVPairs> ReductionKeys, string ReducedQVW)
        {
            bool RetVal = false;

            ///create a selections file for the PMC
            string SelectionFile = ReducedQVW.ToLower().Replace(".qvw", ".selections");
            string QVWRootName = System.IO.Path.GetFileNameWithoutExtension(SelectionFile);
            List<string> SelectionData = new List<string>();
            foreach (NVPairs NVP in ReductionKeys)
            {
                SelectionData.Add(QVWRootName + "|" + NVP.Name + "|" + NVP.Value);
            }
            if (System.IO.File.Exists(SelectionFile))
                System.IO.File.Delete(SelectionFile);
            Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
            SS.Serialize(SelectionData, SelectionFile);
            SS = null;

            ///create or update the index for the report
            //ReducedReportIndex RRI = ReducedReportIndex.Load(ReducedQVW.ToLower().Replace(".qvw", ".xml"));
            //if (RRI == null)
            //    RRI = new ReducedReportIndex();
            ////"\0032cov01\medicaid\woah\ReducedUserQVWs\716265727440736f6d6577686572652e636f6d"
            //string GroupID = System.IO.Path.GetDirectoryName(ReducedQVW);
            //RRI.GroupID = GroupID.Substring(QVDocumentRoot.Length+1);  // group id is encoded in path
            //RRI.GroupID = RRI.GroupID.Substring(0, RRI.GroupID.IndexOf("\\ReducedUserQVWs"));
            //RRI.GroupID = RRI.GroupID.Replace('\\', '_');
            //RRI.QualifiedMasterQVW = MasterQVW;
            //RRI.ReducedByTime = DateTime.Now;
            //RRI.ReducedByUser = AuditUserID;
            //RRI.ReducedQVW = ReducedQVW;
            //RRI.ReductionKeys = ReductionKeys;
            //RRI.UserID = UserID;
            //RetVal = RRI.Save(ReducedQVW.ToLower().Replace(".qvw", ".xml"));

            //now update the master index for the report to be used with caching
            if (RetVal)
            {
                //Master QVW index is one level back from leaf
                string QVWName = System.IO.Path.GetFileName(ReducedQVW);
                string MasterPath = System.IO.Path.GetDirectoryName(ReducedQVW);
                MasterPath = System.IO.Path.GetDirectoryName(MasterPath); //back up on extra directory
                MasterPath = System.IO.Path.Combine(MasterPath, QVWName); //add the name back on
                ReducedReportMasterIndex RRMI = ReducedReportMasterIndex.Load(MasterQVW);
                if (RRMI == null)
                {
                    RRMI = new ReducedReportMasterIndex();
                    RRMI._LoadedFrom = MasterQVW.ToLower().Replace(".qvw", ".xml");
                }
                RetVal = RRMI.AddToReductionCache(ReducedQVW, ReductionKeys);
            }

            return RetVal;
        }

    }
}
