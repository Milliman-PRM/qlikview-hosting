using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.ComplexUpload
{
    public partial class IndexDownloader : System.Web.UI.Page
    {
        public class IndexDownloaderClass : TaskBase
        {
            public string DisplayMessage { get; set; }
            public string NavigateTo { get; set; }

            public override void TaskProcessor(object parms)
            {
                try
                {
                    MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(QualafiedProjectFile);

                    DisplayMessage = "Retrieving data from server.....";
                    string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                    string VirtualPathProject = QualafiedProjectFile.Substring(DocumentRoot.Length);
                    if ((VirtualPathProject.StartsWith("\\")) || (VirtualPathProject.StartsWith("/")))
                        VirtualPathProject = VirtualPathProject.Substring(1);
                    bool IsErrorState = false;
                    bool ProjectMissing = false;
                    string XML = MillimanProjectManConsole.Global.GetInstance().GetIndexsAndSelections(VirtualPathProject, out ProjectMissing, out IsErrorState);
                    //make sure we really got something from the service
                    if (ProjectMissing)
                    {
                        // if either of these is true, it was uploaded before, and is now missing from production
                        if ( (string.IsNullOrEmpty(PS.UploadedToProduction) == false ) || (PS.UploadedToProductionDate != null) )
                        {
                            DisplayMessage = XML; //not really xml and error message
                            TaskCompletionWithError = true;
                            TaskCompletionMessage = XML;
                            base.TaskProcessor(parms);
                            return;
                        }
                    }
                    else if (IsErrorState)
                    {
                        DisplayMessage = XML; //not really xml and error message
                        TaskCompletionWithError = true;
                        TaskCompletionMessage = XML;
                        base.TaskProcessor(parms);
                        return;
                    }

                    if (ProjectMissing) //project has not been upload yet, so yes it's missing, remove error message from XML
                    {
                        DisplayMessage = "Completed index/selections retrieval";
                        NavigateTo = "QVWExtractor.aspx";
                        TaskCompletionWithError = false;
                        TaskCompletionMessage = DisplayMessage;
                        base.TaskProcessor(parms);
                        return;
                    }

                    DisplayMessage = "Data retrieved,  parsing into index/selection files";

                    Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                    System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                    var stringBytes = System.Text.Encoding.ASCII.GetBytes(XML);
                    memStream.Write(stringBytes, 0, stringBytes.Length);
                    memStream.Seek(0, System.IO.SeekOrigin.Begin);
                    Dictionary<string, string> IndexsSelections = SS.Deserialize(memStream) as Dictionary<string, string>;

                    DisplayMessage = "Data parsed,  saving for comparison processing...";
                   // string DocumentRoot = System.IO.Path.GetDirectoryName(QualafiedProjectFile);
                    //string ProjectReducedData = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), "ReducedUserQVWs");
                    int Index = 0;
                    foreach (KeyValuePair<string, string> KVP in IndexsSelections)
                    {
                        DisplayMessage = "Updating " + Index.ToString() + " of " + IndexsSelections.Count.ToString() + " files";
                        if (KVP.Key.ToLower().IndexOf(".hierarchy_") != -1)
                        {
                            string HierarchyPathFilename = System.IO.Path.Combine(DocumentRoot, KVP.Key);
                            System.IO.File.Delete(HierarchyPathFilename);
                            System.IO.File.WriteAllText(HierarchyPathFilename, KVP.Value);
                        }
                        else if (string.Compare(KVP.Key, "AutoInclude", true) == 0)
                        {
                            string AutoIncludeFilename = System.IO.Path.Combine(PS.LoadedFromPath, KVP.Key);
                            System.IO.File.Delete(AutoIncludeFilename);
                            System.IO.File.WriteAllText(AutoIncludeFilename, KVP.Value.ToString());  //true/false
                        }
                        else
                        {
                            string UserSelection = System.IO.Path.Combine(DocumentRoot, KVP.Key);
                            if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(UserSelection)) == false)
                                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(UserSelection));
                            if (System.IO.File.Exists(UserSelection))
                                System.IO.File.Delete(UserSelection);
                            System.IO.File.WriteAllText(UserSelection, KVP.Value);
                        }
                        Index++;
                    }
                    DisplayMessage = "Completed index/selections retrieval";
                    NavigateTo = "QVWExtractor.aspx";

                    TaskCompletionWithError = false;
                    TaskCompletionMessage = DisplayMessage;
                    base.TaskProcessor(parms);
                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to download indexs", ex);
                    TaskCompletionWithError = true;
                    TaskCompletionMessage = ex.ToString();
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
                    IndexDownloaderClass IDC = new IndexDownloaderClass();
                    IDC.QualafiedProjectFile = ProjectPath;
                    Global.TaskManager.ScheduleTask(IDC);
                    IDC.StartTask();
                    IndexDownload.Text = "Starting index/user selections retrieval.";
                    Response.Redirect("IndexDownloader.aspx?Processing=" + IDC.TaskID);
                }
                else
                {
                    string ProcessID = Request["Processing"].ToString();
                    IndexDownloaderClass IDC = Global.TaskManager.FindTask(ProcessID) as IndexDownloaderClass;
                    if (IDC != null)
                    {
                        IndexDownload.Text = IDC.DisplayMessage;
                        if (IDC.TaskCompletionWithError)
                        {
                            IndexDownload.Text = IDC.TaskCompletionMessage;
                            IDC.AbortTask = true;
                            Global.TaskManager.DeleteTask(IDC.TaskID);
                        }
                        else if (IDC.TaskCompleted)
                        {
                            IndexDownload.Text = IDC.TaskCompletionMessage;
                            Global.TaskManager.DeleteTask(IDC.TaskID);
                            MillimanCommon.Alert.DelayedNavigation(IDC.NavigateTo, 3);
                        }
                        else  //task is running
                        {
                            MillimanCommon.Alert.Refresh(3);
                            IndexDownload.Text = IDC.DisplayMessage;
                        } 
                    }
                    else
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "System error - missing task");
                        Response.Redirect("errors/missingtask.aspx");
                        IndexDownload.Text = "Could not retrieve task status";
                    }
                }
            }
        }
    }
}