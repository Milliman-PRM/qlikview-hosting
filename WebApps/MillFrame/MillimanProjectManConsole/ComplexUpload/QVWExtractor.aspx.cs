using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.ComplexUpload
{
    public partial class QVWExtractor : System.Web.UI.Page
    {
        public class QVWExtractorClass : TaskBase
        {
            public string DisplayMessage { get; set; }
            public string NavigateTo { get; set; }
            public string QualifiedDebugFile { get; set; }

            public override void TaskProcessor(object parms)
            {
                MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(QualafiedProjectFile);

                DisplayMessage = "Extracting hierarchy information from QVW associated with project '" + PS.ProjectName + "'";
                Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass(DisplayMessage));

                string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                string QVWPathName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), PS.QVName + ".qvw_new");

                MillimanReportReduction.QVWHierarchy Hier = new MillimanReportReduction.QVWHierarchy();
                Hier.DeleteTaskOnCompletion = true;
                Hier.QualifiedQVWNameToIndex = QVWPathName;
                Hier.TaskDescription = "Extracting hierarchy info via partial reload - " + QVWPathName;
                Hier.TaskName = "Extracting Heirarchy";
                Hier.QualifiedDebugFile = QualifiedDebugFile;
                DateTime Start = DateTime.Now;
                Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("Extraction started via partial reload"));
                if (Hier.ExtractHierarchyBlocking() == false)
                {
                    TaskCompletionWithError = true;
                    TaskCompletionMessage = Hier.TaskStatusMsg;
                    base.TaskProcessor(parms);
                    return;
                }
                DateTime Finished = DateTime.Now;
                var DiffSeconds = (Finished - Start).TotalSeconds;
                DisplayMessage = "Hierarchy extraction completed.";
                TaskCompletionMessage = DisplayMessage;
                Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("Extraction completed in" + DiffSeconds.ToString() + " seconds"));

                //get the directory they files exist in and pass along in the chain
                if ((Hier.ExtractedHierarchyFiles != null) && (Hier.ExtractedHierarchyFiles.Count > 0))
                {
                    string Directory = System.IO.Path.GetDirectoryName(Hier.ExtractedHierarchyFiles[0]);
                    string URLDir = Uri.EscapeDataString(Directory);
                    Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("Starting index builder for directory '" + URLDir + "'"));
                    NavigateTo = "IndexBuilder.aspx?dir=" + URLDir;
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
                    QVWExtractorClass QEC = new QVWExtractorClass();
                    QEC.QualafiedProjectFile = ProjectPath;
                    ///get the debug file and copy it over into new directory, including using the name of file verbatim
                    string DebugFileTAG = "HierarchyDebugFileTemplate";
                    string DebugFileValue = System.Configuration.ConfigurationManager.AppSettings[DebugFileTAG];
                    if (string.IsNullOrEmpty(DebugFileValue))
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Web config for application is missing tag : '" + DebugFileTAG + "'");
                        MillimanCommon.Alert.Show("Web config is missing a required value.  Please contact the system administrator for missing tag : '" + DebugFileTAG + "'");
                        return;
                    }

                    string appPath = HttpContext.Current.Request.ApplicationPath;
                    string physicalPath = HttpContext.Current.Request.MapPath(appPath);
                    QEC.QualifiedDebugFile = System.IO.Path.Combine(physicalPath, DebugFileValue);

                    Global.TaskManager.ScheduleTask(QEC);
                    QEC.StartTask();
                    IndexDownload.Text = "Starting extraction of hierarchy information from QVW.";
                    Response.Redirect("QVWExtractor.aspx?Processing=" + QEC.TaskID);
                }
                else
                {
                    string ProcessID = Request["Processing"].ToString();
                    QVWExtractorClass QEC = Global.TaskManager.FindTask(ProcessID) as QVWExtractorClass;
                    if (QEC != null)
                    {
                        IndexDownload.Text = QEC.DisplayMessage;
                        if (QEC.TaskCompletionWithError)
                        {
                            IndexDownload.Text = QEC.TaskCompletionMessage;
                            QEC.AbortTask = true;
                            Global.TaskManager.DeleteTask(QEC.TaskID);
                        }
                        else if (QEC.TaskCompleted)
                        {
                            IndexDownload.Text = QEC.TaskCompletionMessage;
                            Global.TaskManager.DeleteTask(QEC.TaskID);
                            MillimanCommon.Alert.DelayedNavigation(QEC.NavigateTo, 3);
                        }
                        else  //task is running
                        {
                            MillimanCommon.Alert.Refresh(3);
                            IndexDownload.Text = QEC.DisplayMessage;
                        }
                    }
                    else
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "System error - missing task");
                        Response.Redirect("errors/missingtask.html");
                        IndexDownload.Text = "Could not retrieve task status";
                    }
                }
            }
        }
    }
}