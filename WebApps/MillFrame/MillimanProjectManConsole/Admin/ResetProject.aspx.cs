using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.Admin
{
    public partial class ResetProject : System.Web.UI.Page
    {

        public class RestProjectClass : MillimanProjectManConsole.ComplexUpload.TaskBase
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

                    DisplayMessage = "Completed file set retrieval";
                    NavigateTo = "";
                    List<string> AllFiles = MillimanProjectManConsole.Global.GetInstance().GetAllProjectFiles(PS.VirtualDirectory);
                    //create a temp dir to retrieve server files and store to
                    //string RootTemp = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.Guid.NewGuid().ToString("N"));
                    string RootTemp = System.IO.Path.Combine(@"D:\TestDocuments\test", System.Guid.NewGuid().ToString("N"));

                    DisplayMessage = "Creating temp directories for server data retrieval";
                    string DataFolderName = PS.ProjectName.ToLower() + "_data";
                    string ReducedCacheName = "ReducedCachedQVWs";
                    string ReducedUserQVWs = "ReducedUserQVWs";

                    //create the dirs we need
                    System.IO.Directory.CreateDirectory(RootTemp);
                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(RootTemp, "backup"));

                    DisplayMessage = "Creating backup of current data set";
                    string BackupZip = System.IO.Path.Combine(RootTemp, "backup", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".zip");
                    System.IO.Compression.ZipFile.CreateFromDirectory(System.IO.Path.GetDirectoryName(QualafiedProjectFile), BackupZip);
                    if ( System.IO.File.Exists(BackupZip) == false )
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to create backup set for project - " + PS.ProjectName);
                        DisplayMessage = "Failed to create backup set for project - " + PS.ProjectName;
                        TaskCompletionWithError = true;
                        TaskCompletionMessage = DisplayMessage;
                        base.TaskProcessor(parms);
                        return;
                    }
                    DisplayMessage = "Backup created";
                    
                    byte[] FileContents = null;
                    string SaveTo = string.Empty;
                    int Index = 1;
                    List<string> SavedItems = new List<string>();

 

                    foreach (string File in AllFiles)
                    {
                        DisplayMessage = "Downloading and processing item " + Index.ToString() + " of " + AllFiles.Count.ToString();
                        Index++;
                        //all files are virtualized,  thus any path with only one \ is at root and we want to keep
                        if (File.Substring(PS.Groups.Length + 1).Count(f => f == '\\') == 0)
                        {
                            FileContents = MillimanProjectManConsole.Global.GetInstance().Get(File);
                            if (FileContents != null)
                            {
                                SaveTo = System.IO.Path.Combine(RootTemp, File);
                                if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(SaveTo)) == false)
                                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(SaveTo));
                                System.IO.File.WriteAllBytes(SaveTo, FileContents);
                                SavedItems.Add(SaveTo);
                            }
                            else
                            {
                                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to retrieve file '" + File + "' from server for project reset");
                                DisplayMessage = "Failed to retrieve file from server for project reset";
                                TaskCompletionWithError = true;
                                TaskCompletionMessage = DisplayMessage;
                                base.TaskProcessor(parms);
                                return;
                            }
                        }
                        else if ( (File.ToLower().Contains(DataFolderName.ToLower()))   ||
                                  (File.ToLower().Contains(ReducedCacheName.ToLower())) ||
                                  (File.ToLower().Contains(ReducedUserQVWs.ToLower())) )
                        {   //this is an item that needs syching
                            string LocalDataFolderName = System.IO.Path.Combine(RootTemp, System.IO.Path.GetDirectoryName(File));
                            if (System.IO.Directory.Exists(LocalDataFolderName) == false)
                                System.IO.Directory.CreateDirectory(LocalDataFolderName);

                            FileContents = MillimanProjectManConsole.Global.GetInstance().Get(File);
                            if (FileContents != null)
                            {
                                SaveTo = System.IO.Path.Combine(RootTemp, File);
                                if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(SaveTo)) == false)
                                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(SaveTo));
                                System.IO.File.WriteAllBytes(SaveTo, FileContents);
                                SavedItems.Add(SaveTo);
                            }
                            else
                            {
                                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to retrieve file '" + File + "' from server for project reset");
                                DisplayMessage = "Failed to retrieve file from server for project reset";
                                TaskCompletionWithError = true;
                                TaskCompletionMessage = DisplayMessage;
                                base.TaskProcessor(parms);
                                return;
                            }
                        }
                    }

                    //we have downloaded everything time to sync
                    //get rid of _data, reducedcachedqvws and reduceduserqvws
                    if (System.IO.Directory.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), PS.ProjectName + "_data")))
                        System.IO.Directory.Delete(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), PS.ProjectName + "_data"), true);
                    if (System.IO.Directory.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), ReducedCacheName)))
                        System.IO.Directory.Delete(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), ReducedCacheName), true);
                    if (System.IO.Directory.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), ReducedUserQVWs)))
                        System.IO.Directory.Delete(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualafiedProjectFile), ReducedUserQVWs), true);

                    foreach (string FileItem in SavedItems)
                    {
                        DisplayMessage = "Updating local file - " + System.IO.Path.GetFileName(FileItem);
                        RebaseAndCopy(RootTemp, System.IO.Path.GetDirectoryName(QualafiedProjectFile), FileItem);
                    }

                    TaskCompletionWithError = false;
                    TaskCompletionMessage = DisplayMessage;
                    NavigateTo = "ResetSuccessful.html";
                    base.TaskProcessor(parms);
                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to download indexs", ex);
                    TaskCompletionWithError = true;
                    TaskCompletionMessage = ex.ToString();
                    NavigateTo = "ResetFailed.html";
                }
                base.TaskProcessor(parms);
            }

            private void RebaseAndCopy(string SourceDir, string ToDir, string FileItem)
            {
                string Virtualized = FileItem.Substring(SourceDir.Length);
                if ( Virtualized.StartsWith(@"\") || Virtualized.StartsWith(@"/") )
                    Virtualized = Virtualized.Substring(1);
                string QVRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                string Rebased = System.IO.Path.Combine(QVRoot, Virtualized);

                if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Rebased)) == false)
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Rebased));
                System.IO.File.Delete(Rebased);
                System.IO.File.Copy(FileItem, Rebased);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
                if (Request["Processing"] == null)
                {  
                    //I get this horid format, blah
                        //QVDocuments/0032COV01/Medicaid/WOAH/Care Coordinator Report.hciprj
                    string ProjectPath = System.IO.Path.Combine( System.IO.Path.GetDirectoryName( System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"]), Request["ProjectPath"].ToString() );
                    RestProjectClass IDC = new RestProjectClass();
                    IDC.QualafiedProjectFile = ProjectPath;
                    Global.TaskManager.ScheduleTask(IDC);
                    IDC.StartTask();
                    IndexDownload.Text = "Starting index/user selections retrieval.";
                    Response.Redirect("ResetProject.aspx?Processing=" + IDC.TaskID);
                }
                else
                {
                    string ProcessID = Request["Processing"].ToString();
                    RestProjectClass IDC = Global.TaskManager.FindTask(ProcessID) as RestProjectClass;
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
                        Response.Redirect("errors/missingtask.html");
                        IndexDownload.Text = "Could not retrieve task status";
                    }
                }
            }
        }
}