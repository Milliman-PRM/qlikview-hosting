using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.ComplexUpload
{
    public partial class GenerateReducedQVWs : System.Web.UI.Page
    {
        public class GenerateReducedQVWsClass : TaskBase
        {
            public string DisplayMessage { get; set; }
            public string NavigateTo { get; set; }

            /// <summary>
            /// Remove all the old cached versions
            /// </summary>
            /// <returns></returns>
            private bool Clean()
            {
                try
                {
                    Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("Removeing invalid QVWs from reduction path"));
                    MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(QualafiedProjectFile);
                    string QVWPath = System.IO.Path.GetDirectoryName(QualafiedProjectFile);
                    //need to make sure cache is empty, so delete it and re-create it
                    string ReducedQVWsPath = System.IO.Path.Combine(QVWPath, "ReducedCachedQVWs");
                    if (System.IO.Directory.Exists(ReducedQVWsPath) == true)
                        System.IO.Directory.Delete(ReducedQVWsPath, true);

                    System.IO.Directory.CreateDirectory(ReducedQVWsPath);
                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified", ex);
                    return false;
                }
                return true;
            }


            public override void TaskProcessor(object parms)
            {
                Clean();  //git rid of any previous reductions that were not protmoted

                MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(QualafiedProjectFile);

                //VWN we are only supporting 1 hierarchy for reduction
                //load the new hierarchy file, field names could have changed
                List<string> Hierarchies = MillimanCommon.ReducedQVWUtilities.GetHierarchyFilenames(QualafiedProjectFile, true);
                if ((Hierarchies == null) || (Hierarchies.Count == 0))
                {
                    DisplayMessage = "Could not load new hierarchy information extracted from QVW";
                    Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("No hierarchy files found for reduction - exiting"));
                    base.TaskProcessor(parms);
                    NavigateTo = "";
                    return;
                }
                string NewHierTreeFile = Hierarchies[0];
                MillimanCommon.MillimanTreeNode NewHierTree = MillimanCommon.MillimanTreeNode.GetMemoryTree(NewHierTreeFile);

                DisplayMessage = "Creating reduced view QVWs for users of project '" + PS.ProjectName + "'";
                Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass(DisplayMessage));

                string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                string QVWPathName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), PS.QVName + ".qvw_new");
                string QVWPath = System.IO.Path.GetDirectoryName(QualafiedProjectFile);
                string ReducedQVWsPath = System.IO.Path.Combine(QVWPath, "ReducedUserQVWs");
                string[] Selections = System.IO.Directory.GetFiles(ReducedQVWsPath, PS.QVName + ".selections", System.IO.SearchOption.AllDirectories);

                if ((Selections == null) || (Selections.Count() == 0))
                {
                    //if this is empty,  then no selections have been made yet
                    DisplayMessage = "No user selections have been previously made";
                    Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass(DisplayMessage));
                    base.TaskProcessor(parms);
                    NavigateTo = "../ComplexReporting/ReportingShell.aspx";
                    return;
                }
                //used to hold status of reducing master qvw
                List<string> SuccessfulReductions = new List<string>();
                List<string> FailedReductions = new List<string>();

                int Index = 1;
                foreach (string SelectionFile in Selections)
                {
                    string UserName = string.Empty;
                    string[] Tokens = SelectionFile.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    UserName = MillimanCommon.Utilities.ConvertHexToString(Tokens[Tokens.Length - 2]);  //user name is hex string encoded to create a dir, so unencode it for display
                    DisplayMessage = "Reducing QVW for user " + UserName + " (" + Index.ToString() + " of " + Selections.Count().ToString() + " users)";
                    Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass(DisplayMessage));
                    Reports.AddItemToList(new MillimanCommon.QVWReportBank.ProcessingStatusClass(UserName, QualafiedProjectFile, "Starting reduction"));
                    
                    //create the cache processor
                    MillimanReportReduction.QVWCaching QVWC = new MillimanReportReduction.QVWCaching(QVWPathName);

                    //string CachedQVW = GetCachedQVW(SelectionFile, PS.QVName);
                    string CachedQVW = QVWC.FindReducedQVW(SelectionFile);

                    if (string.IsNullOrEmpty(CachedQVW))
                    {
                        try
                        {
                            Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass(UserName + "'s " + PS.QVName + " requires reduction - no cached version available"));

                            string ReduceToQVW = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SelectionFile), PS.QVName + ".qvw_new");
                            Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                            List<string> TreeSelections = SS.Deserialize(SelectionFile) as List<string>;

                            MillimanReportReduction.QVWReducer QVWR = new MillimanReportReduction.QVWReducer();
                            QVWR.QualifiedQVWNameToReduce = QVWPathName;
                            QVWR.QualifiedReducedQVWName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), QVWNameFromSelection(SelectionFile));

                            foreach (string TS in TreeSelections)
                            {
                                string[] TSTokens = TS.Split(new char[] { '|' });
                                List<string> DataModelFieldNames = null;
                                List<string> DisplayValues = new List<string>();
                                for (int TokenIndex = 1; TokenIndex < TSTokens.Count(); TokenIndex++)
                                {
                                    string DisplayToken = TSTokens[TokenIndex];
                                    DisplayValues.Add(DisplayToken);
                                    DataModelFieldNames = NewHierTree.FindDataModelFieldNames(DisplayValues);
                                    if (DataModelFieldNames != null)
                                    {
                                        foreach (string DMFN in DataModelFieldNames)
                                        {
                                            QVWR.Variables.Add(new MillimanReportReduction.NVPairs(DMFN, DisplayToken));
                                        }
                                    }
                                    else
                                    {
                                        //VWN - failed selection
                                        //add to report - failed to find selected value
                                    }
                                }
                            }
                            //QVWR.Variables.Add(new MillimanReportReduction.NVPairs("Physician Practice", "Muncie"));
                            //QVWR.Variables.Add(new MillimanReportReduction.NVPairs("Physician Practice", "Avon West"));
                            ////QVWR.Variables.Add(new QVWReducer.NVPairs("Assigned Physician", "Van MD, Robert"));
                            ////QVWR.Variables.Add(new QVWReducer.NVPairs("Assigned Physician", "May MD, Larry"));

                            QVWR.TaskName = UserName + " Limiting View";
                            QVWR.TaskDescription = "Limiting data access for " + UserName;
                            QVWR.DeleteTaskOnCompletion = true;
            

                            if (QVWR.ReduceBlocking() == false)
                            {
                                FailedReductions.Add(UserName);
                            }
                            else
                            {
                                //must do this nonsense, since publisher will not allow creating of file with a ext
                                //other than qvw
                                if (System.IO.File.Exists(ReduceToQVW) == true)
                                    System.IO.File.Delete(ReduceToQVW);
                                System.IO.File.Copy(QVWR.QualifiedReducedQVWName, ReduceToQVW);

                                string QVWInCache = QVWC.MoveNewQVWToCache(SelectionFile, ReduceToQVW, System.IO.Path.GetFileNameWithoutExtension( QVWR.QualifiedReducedQVWName ));
                                MillimanReportReduction.QVWCaching.WriteQVWRedirector(QVWR.QualifiedQVWNameToReduce, UserName, QVWInCache, true);
                                SuccessfulReductions.Add(UserName);
                                Reports.AddItemToList(new MillimanCommon.QVWReportBank.ProcessingStatusClass(UserName, PS.QVName, "Reduced - Available"));
                                Reports.AddItemToList(new MillimanCommon.QVWReportBank.ProcessingStatusClass(UserName, PS.QVName, "success"));
                            }
                        }
                        catch (Exception ex)
                        {
                            Reports.AddItemToList(new MillimanCommon.QVWReportBank.ProcessingStatusClass(UserName, PS.QVName, "Failed"));
                            string ErrorMsg = "Failed to reduce QVW '" + QVWPathName + "' for user '" + UserName + "'";
                            Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass(ErrorMsg));
                            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, ErrorMsg, ex);
                            FailedReductions.Add(UserName);
                        }
                    }
                    else
                    {
                        DisplayMessage += " *";
                        //we found a cached version so move it over
                        string ReduceToQVW = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SelectionFile), PS.QVName + ".qvw_new");
                        Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass(UserName + "'s " + PS.QVName + " copied from cached version - '" + CachedQVW));

                        //write the redirector
                        MillimanReportReduction.QVWCaching.WriteQVWRedirector(QVWPathName, UserName, CachedQVW, true);

                        //don't copy
                        //System.IO.File.Delete(ReduceToQVW);
                        //System.IO.File.Copy(CachedQVW, ReduceToQVW);
                        SuccessfulReductions.Add(UserName);
                        Reports.AddItemToList(new MillimanCommon.QVWReportBank.ProcessingStatusClass(UserName, PS.QVName, "Copied - Available"));
                        Reports.AddItemToList(new MillimanCommon.QVWReportBank.ProcessingStatusClass(UserName, PS.QVName, "success"));
                    }
                    Index++;
                }

                TaskCompletionMessage = DisplayMessage;
                //VWN - at end we should show another report on the success failure rate
                //dump success, failed lists to file for Reduction Report
                NavigateTo = "../ComplexReporting/ReportingShell.aspx";
                base.TaskProcessor(parms);

            }

            /// <summary>
            /// if we brought down an old QVW name, get is name and use it, otherwise create a new one
            /// </summary>
            /// <param name="SelectionFile"></param>
            /// <returns></returns>
            private string QVWNameFromSelection(string SelectionFile)
            {
                string QVWName = string.Empty;
                string RedirectFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SelectionFile), "[REFERENCE].redirect");
                if (System.IO.File.Exists(RedirectFile))
                {  //read old file and get old GUID
                    string RedirectTxt = System.IO.File.ReadAllText(RedirectFile);
                    List<string> Tokens = RedirectTxt.Split(new char[] { '\\' }).ToList();
                    QVWName = Tokens.Last();
                }
                else
                { //create a new guid and send back
                    QVWName = System.IO.Path.GetTempFileName().ToLower().Replace(".tmp", ".qvw");
                }
                return QVWName;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["Processing"] == null)
                {
                    string ProjectPath = Session["ProjectPath"].ToString();
                    GenerateReducedQVWsClass GRQVWs = new GenerateReducedQVWsClass();
                    GRQVWs.QualafiedProjectFile = ProjectPath;

                    Global.TaskManager.ScheduleTask(GRQVWs);
                    GRQVWs.StartTask();
                    IndexDownload.Text = "Creating difference report";
                    Response.Redirect("GenerateReducedQVWs.aspx?Processing=" + GRQVWs.TaskID);
                }
                else
                {
                    string ProcessID = Request["Processing"].ToString();
                    GenerateReducedQVWsClass GRQVWs = Global.TaskManager.FindTask(ProcessID) as GenerateReducedQVWsClass;
                    if (GRQVWs != null)
                    {
                        IndexDownload.Text = GRQVWs.DisplayMessage;
                        if (GRQVWs.TaskCompletionWithError)
                        {
                            IndexDownload.Text = GRQVWs.TaskCompletionMessage;
                            GRQVWs.AbortTask = true;
                            Global.TaskManager.DeleteTask(GRQVWs.TaskID);
                        }
                        else if (GRQVWs.TaskCompleted)
                        {
                            IndexDownload.Text = GRQVWs.TaskCompletionMessage;
                            Global.TaskManager.DeleteTask(GRQVWs.TaskID);
                            MillimanCommon.Alert.DelayedNavigation(GRQVWs.NavigateTo, 3);
                        }
                        else  //task is running
                        {
                            MillimanCommon.Alert.Refresh(3);
                            IndexDownload.Text = GRQVWs.DisplayMessage;
                        }
                    }
                    else
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "System error - missing task");
                        Response.Redirect("errors/missingtask.aspx");

                    }
                }
            }
        }
    }
}