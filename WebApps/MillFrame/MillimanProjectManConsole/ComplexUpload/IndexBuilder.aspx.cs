using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.ComplexUpload
{
    public partial class IndexBuilder : System.Web.UI.Page
    {
        public class IndexBuilderClass : TaskBase
        {
            public string DisplayMessage { get; set; }
            public string NavigateTo { get; set; }
            public string ExtractedFileDirectory { get; set; }

            public override void TaskProcessor(object parms)
            {
                MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(QualafiedProjectFile);

                DisplayMessage = "Generating hierarchy index information from QVW associated with project '" + PS.ProjectName + "'";
                Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass(DisplayMessage));

                string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                string QVWPathName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), PS.QVName + ".qvw_new");

                Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("Constructing tree builder"));
                MillimanCommon.TreeBuilder TB = new MillimanCommon.TreeBuilder();
                List<string> TreeFiles = null;
                List<string> ConceptLabelFiles = null;
                List<string> DropTableFiles = null;
                string DataModelFile = string.Empty;
                if (TB.BuildTree(ExtractedFileDirectory, out TreeFiles, out ConceptLabelFiles, out DropTableFiles, out DataModelFile) == true)
                {
                    if ((TreeFiles == null) || (TreeFiles.Count == 0))
                    {
                        TaskCompletionWithError = true;
                        TaskCompletionMessage = "No index files were generated.";
                        base.TaskProcessor(parms);
                        return;
                        //VWN may not be an error
                    }
                    DisplayMessage = "Index files successfully generated.";
                    Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass(DisplayMessage));

                    string QVWDir = System.IO.Path.GetDirectoryName(QualafiedProjectFile);
                    int Index = 0;
                    foreach (string IndexFile in TreeFiles)
                    {
                        string NewHier = System.IO.Path.Combine(QVWDir, PS.QVName + ".hierarchy_" + Index.ToString() + "_new");
                        System.IO.File.Delete(NewHier);
                        System.IO.File.Copy(IndexFile, NewHier);
                        if (System.IO.File.Exists(NewHier) == false)
                        {
                            Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("Could not copy file '" + IndexFile + "' to '" + NewHier + "'"));
                            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Could not copy file '" + IndexFile + "' to '" + NewHier + "'");
                        }
                    }
                    Index = 0;
                    foreach (string ConceptFile in ConceptLabelFiles)
                    {
                        string NewConcept = System.IO.Path.Combine(QVWDir, PS.QVName + ".concept_" + Index.ToString() + "_new");
                        System.IO.File.Delete(NewConcept);
                        System.IO.File.Copy(ConceptFile, NewConcept);
                        if (System.IO.File.Exists(NewConcept) == false)
                        {
                            Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("Could not copy file '" + ConceptFile + "' to '" + NewConcept + "'"));
                            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Could not copy file '" + ConceptFile + "' to '" + NewConcept + "'");
                        }
                    }
                    Index = 0;
                    foreach (string DropTableFile in DropTableFiles)
                    {
                        string DropTable = System.IO.Path.Combine(QVWDir, PS.QVName + ".map_" + Index.ToString() + "_new");
                        System.IO.File.Delete(DropTable);
                        System.IO.File.Copy(DropTableFile, DropTable);
                        if (System.IO.File.Exists(DropTable) == false)
                        {
                            Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("Could not copy file '" + DropTableFile + "' to '" + DropTable + "'"));
                            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Could not copy file '" + DropTableFile + "' to '" + DropTable + "'");
                        }
                    }

                    if (string.IsNullOrEmpty(DataModelFile) == false)
                    {
                        string DM = System.IO.Path.Combine(QVWDir, PS.QVName + ".datamodel_0_new");
                        System.IO.File.Delete(DM);
                        System.IO.File.Copy(DataModelFile, DM);
                        if (System.IO.File.Exists(DM) == false)
                        {
                            Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("Could not copy file '" + DataModelFile + "' to '" + DM + "'"));
                            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Could not copy file '" + DataModelFile + "' to '" + DM + "'");
                        }

                    }
                    DisplayMessage = "Cleanup of temporary files in progress.";
                    //VWN cannot delete directory says candidate files are in use by IIS?????
                    try
                    {
                        System.IO.Directory.Delete(ExtractedFileDirectory, true);
                    }
                    catch (Exception)
                    {
                        //don't report error, needs to be resolved 
                    } 
                    DisplayMessage = "Cleanup of temporary files completed.";
                    Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass(DisplayMessage));

                    NavigateTo = "ReportGenerator.aspx";

                    TaskCompletionMessage = DisplayMessage;
                }
                else
                {
                    TaskCompletionWithError = true;
                    TaskCompletionMessage = TB.LastErrorMessage;
                }

                base.TaskProcessor(parms);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["Processing"] == null)
                {
                    string ProjectPath = Session["ProjectPath"].ToString();
                    IndexBuilderClass IBC = new IndexBuilderClass();
                    IBC.QualafiedProjectFile = ProjectPath;
                    IBC.ExtractedFileDirectory = Uri.UnescapeDataString( Request["dir"] );

                    Global.TaskManager.ScheduleTask(IBC);
                    IBC.StartTask();
                    IndexDownload.Text = "Generating new indexes from updated QVW";
                    Response.Redirect("IndexBuilder.aspx?Processing=" + IBC.TaskID);
                }
                else
                {
                    string ProcessID = Request["Processing"].ToString();
                    IndexBuilderClass IBC = Global.TaskManager.FindTask(ProcessID) as IndexBuilderClass;
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