using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.ComplexUpload
{
    public partial class ReportGenerator : System.Web.UI.Page
    {
        public class ReportGeneratorClass : TaskBase
        {
            public string DisplayMessage { get; set; }
            public string NavigateTo { get; set; }
            public MillimanCommon.ProjectSettings PS { get; set; }

            public override void TaskProcessor(object parms)
            {
                PS = MillimanCommon.ProjectSettings.Load(QualafiedProjectFile);

                DisplayMessage = "Creating difference report of changes made to project data hierarchy '" + PS.ProjectName + "'";
                Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass(DisplayMessage));

                string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                string QVWPathName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), PS.QVName + ".qvw_new");

                //VWN should support multiple hierarchies, but not for now just support 1
                bool IsNewProject = false;
                string[] HierachyFiles = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName( QVWPathName), "*.hierarchy*");
                foreach (string Hier in HierachyFiles)
                {
                    //look at the extension, to see if new, if it is simple diff
                    if (System.IO.Path.GetExtension(Hier).IndexOf("_new") != -1)
                    {
                        string PreviousHier = Hier.Replace("_new", "");
                        //if no prvious version exists we are all new
                        if (System.IO.File.Exists(PreviousHier) == false)
                            IsNewProject = true;
                    }
                }
                //VWN this is completely bogus, we need to check for
                //diff new hier with old to generate data on "NEW" items added
                //check selections of users against new to generate a "CANNOT" reduce on
                if (IsNewProject)
                {
                    Reports.AddItemToList(new MillimanCommon.QVWReportBank.NewItemClass("All Items", "Initial report"));
                    DisplayMessage = "This project has not been indexed before - hierarchy difference does not apply";
                }
                else
                {
                    DisplayMessage = "This project has been indexed before - generating hierarchy difference reports";
                    //we only support 1 hier file for now but put in loop anyway
                    foreach (string Hier in HierachyFiles)
                    {
                        //HierarchyFiles contains both old and new items, just look at new items
                        if (Hier.ToLower().IndexOf("_new") != -1)
                        {
                            string OldHier = Hier.Replace("_new", "");
                            MillimanCommon.MillimanTreeNode OldHierarchy = MillimanCommon.MillimanTreeNode.GetMemoryTree(OldHier);
                            MillimanCommon.MillimanTreeNode NewHierarchy = MillimanCommon.MillimanTreeNode.GetMemoryTree(Hier);
                            //generate reports
                            //new items report
                            Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("Generating report to identify new items in hiearchy '" + Hier + "'"));
                            try
                            {
                                GenerateNewItemsReport(OldHierarchy, NewHierarchy);
                                Reports.AddItemToList(new MillimanCommon.QVWReportBank.NewItemClass("completed", "success"));
                            }
                            catch (Exception ex)
                            {
                                Reports.AddItemToList(new MillimanCommon.QVWReportBank.NewItemClass("completed", "failed - " + ex.ToString()));
                            }
                            //not selectable report
                            Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("Generating report to identify selections that are not available in new report '" + Hier + "'"));
                            try
                            {
                                GenerateNotSelectable(NewHierarchy);
                                Reports.AddItemToList(new MillimanCommon.QVWReportBank.NotSelectableClass("completed", "completed", "completed", "completed", "success"));
                            }
                            catch (Exception ex)
                            {
                                Reports.AddItemToList(new MillimanCommon.QVWReportBank.NotSelectableClass("completed", "completed", "completed", "completed", "failed - " + ex.ToString()));
                            }
                        }
                    }
                }
   
                //pre-process the selections files based on auto-inclusion, if auto-inclusion is ON, we need
                //to figure out which nodes in the original had ALL subnodes included, and then figure out if
                //there are new nodes in the eqivilant new tree node and include them.
                //first: get the auto-inclusion info
                string InclusionFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), "AutoInclude");
                bool AutoInclude = false;
                if (System.IO.File.Exists(InclusionFile))
                    AutoInclude = System.Convert.ToBoolean(System.IO.File.ReadAllText(InclusionFile, System.Text.Encoding.UTF8));
                //update project
                PS.AutomaticInclusion = AutoInclude;
                PS.Save();
                string PreviousHierarchy = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), System.IO.Path.GetFileNameWithoutExtension(QualafiedProjectFile) + @".hierarchy_0");
                string CurrentHierarchy = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), System.IO.Path.GetFileNameWithoutExtension(QualafiedProjectFile) + @".hierarchy_0_new");
                string UserDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), @"ReducedUserQVWs");
                MillimanReportReduction.AutoInclusionProcessor AIP = new MillimanReportReduction.AutoInclusionProcessor();
                AIP.Process(PreviousHierarchy, CurrentHierarchy, UserDirectory, AutoInclude);
                
                NavigateTo = "GenerateReducedQVWs.aspx";

                base.TaskProcessor(parms);
            }

            private bool Exists( List<string> TreeItems, MillimanCommon.MillimanTreeNode MTree )
            {
                List<string> DataModelFields = MTree.FindDataModelFieldNames(TreeItems);
                if ( (DataModelFields == null) || (DataModelFields.Count == 0) )
                    return false;
                return true;
            }
      
            /// <summary>
            /// loop over new tree and locate items not found in the old tree, and add to new item report
            /// </summary>
            /// <param name="CurrentHierTree"></param>
            /// <param name="NewHierTree"></param>
            /// <param name="PathItems"></param>
            private void GenerateNewItemsReport( MillimanCommon.MillimanTreeNode CurrentHierTree,  MillimanCommon.MillimanTreeNode NewHierTree, List<string> PathItems = null )
            {
                if (PathItems == null)
                    PathItems = new List<string>();

                //don't do this on root, in which display field name is always empty
                if (string.IsNullOrEmpty(NewHierTree.DisplayFieldName) == false)
                {
                    PathItems.Add(NewHierTree.DisplayFieldName);

                    if (Exists(PathItems, CurrentHierTree) == false)
                    {
                        //VWN where does concept name come from
                        string ConceptName = "?";
                        Reports.AddItemToList(new MillimanCommon.QVWReportBank.NewItemClass(ConceptName, NewHierTree.DisplayFieldName));
                    }
                }
                foreach (KeyValuePair<string, MillimanCommon.MillimanTreeNode> KVP in NewHierTree.SubNodes)
                {
                    GenerateNewItemsReport(CurrentHierTree, KVP.Value, PathItems);
                }
                if ( PathItems.Count > 0 )
                    PathItems.RemoveAt(PathItems.Count - 1);
            }

            private string ListToDisplayString(List<string> Selections)
            {
                string CatString = string.Empty;
                foreach (string S in Selections)
                {
                    if (string.IsNullOrEmpty(CatString) == false)
                        CatString += "->";
                    CatString += S;
                }
                return CatString;
            }
            private void GenerateNotSelectable( MillimanCommon.MillimanTreeNode New )
            {
                //load all selection files and find nodes in new tree
                string QVWPath = System.IO.Path.GetDirectoryName(QualafiedProjectFile);
                string ReducedQVWsPath = System.IO.Path.Combine(QVWPath, "ReducedUserQVWs");
                string[] Selections = System.IO.Directory.GetFiles(ReducedQVWsPath, PS.QVName + ".selections", System.IO.SearchOption.AllDirectories);
                
                List<string> MissingSelections = new List<string>();
                List<string> CurrentSelections = new List<string>();
                
                //get the concept file ready to load
                List<string> Concepts = new List<string>();
                string ConceptFile = System.IO.Path.Combine(QVWPath, System.IO.Path.GetFileNameWithoutExtension(QualafiedProjectFile) + ".concept_0");
                if (System.IO.File.Exists(ConceptFile))
                {
                    string ConceptFileText = System.IO.File.ReadAllText(ConceptFile);
                    Concepts = ConceptFileText.Split(new char[] { '|' }).ToList();
                }

                foreach (string SelectionFile in Selections)
                {
                    string UserName = string.Empty;
                    string[] Tokens = SelectionFile.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    UserName = MillimanCommon.Utilities.ConvertHexToString( Tokens[Tokens.Length - 2] );

                    Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                    List<string> TreeSelections = SS.Deserialize(SelectionFile) as List<string>;
                    foreach (string TS in TreeSelections)
                    {
                        CurrentSelections.Clear();
                        //VWN concept name missing
                        string ConceptName = Concepts.Last();
                        string[] TSTokens = TS.Split(new char[] { '|' });
                        //start with 1,  first token is the qvw name, not a search field
                        for (int Index = 1; Index < TSTokens.Length; Index++)
                        {
                            CurrentSelections.Add(TSTokens[Index]);
                            if (Exists(CurrentSelections, New) == false)
                            {
                                //change concept name based on level
                                ConceptName = Concepts[CurrentSelections.Count() - 1];
                                string CatString = ListToDisplayString(CurrentSelections);
                                if (MissingSelections.Contains(CatString) == false)
                                {
                                    MissingSelections.Add(CatString);
                                    Reports.AddItemToList(new MillimanCommon.QVWReportBank.NotSelectableClass(UserName, CatString, ConceptName, PS.QVName, ""));
                                }
                            }
                        }
                    }
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["Processing"] == null)
                {
                    string ProjectPath = Session["ProjectPath"].ToString();
                    ReportGeneratorClass RGC = new ReportGeneratorClass();
                    RGC.QualafiedProjectFile = ProjectPath;

                    Global.TaskManager.ScheduleTask(RGC);
                    RGC.StartTask();
                    IndexDownload.Text = "Creating difference report";
                    Response.Redirect("ReportGenerator.aspx?Processing=" + RGC.TaskID);
                }
                else
                {
                    string ProcessID = Request["Processing"].ToString();
                    ReportGeneratorClass IBC = Global.TaskManager.FindTask(ProcessID) as ReportGeneratorClass;
                    if (IBC != null)
                    {
                        IndexDownload.Text = IBC.DisplayMessage;
                        if (IBC.TaskCompletionWithError)
                        {
                            IndexDownload.Text = IBC.TaskCompletionMessage;
                            IBC.AbortTask = true;
                            Global.TaskManager.DeleteTask(IBC.TaskID);
                        }
                        else if (IBC.TaskCompleted)
                        {
                            IndexDownload.Text = IBC.TaskCompletionMessage;
                            Global.TaskManager.DeleteTask(IBC.TaskID);
                            MillimanCommon.Alert.DelayedNavigation(IBC.NavigateTo, 3);
                        }
                        else  //task is running
                        {
                            MillimanCommon.Alert.Refresh(3);
                            IndexDownload.Text = IBC.DisplayMessage;
                        }
                    }
                    else
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "System error - missing task");
                        Response.Redirect("errors/missingtask.html");
                    }
                }
            }
        }
    }
}