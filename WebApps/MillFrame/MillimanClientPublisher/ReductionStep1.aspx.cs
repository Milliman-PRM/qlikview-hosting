using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClientPublisher
{
    public partial class ReductionStep1 : System.Web.UI.Page
    {
        public class SimpleUpdateClass : TaskBase
        {
            //used to display progress bars
            public bool ShowAllProgressBars { get; set; }
            public bool ShowOnlyMoveProgressBar { get; set;  }
            public int TotalUser { get; set; }
            public int Errored { get; set;}
            public int Completed { get; set; }
            public int Executing { get; set; }
           //end progress bars

            public string ReductionIndex { get; set;  }
            public string DisplayMessage { get; set; }
            public string NavigateTo { get; set; }
            public ClientPublisher.ProjectSettingsExtension ProjectSettings { get; set; }

            public string WorkingDirectory { get; set; }

            public string Account { get; set; }


            public override void TaskProcessor(object parms)
            {

                Process(parms);
            }

            //this code is split off from taskprocessor so we can re-use it to check the QVW signature, tis re-used by
            //reduction report.aspx since it's the first screen of publishing
            public void Process(object parms)
            {
                //we would have created a tooltip.dat, description.dat and reductionrequired.dat file if the values changed
                ClientPublisher.ProcessingCode.Processor Process = new ClientPublisher.ProcessingCode.Processor();

                DisplayMessage = "Setting up local working directories....";

                string ReducedCacheDir = System.IO.Path.Combine(ProjectSettings.AbsoluteProjectPath, "ReducedCachedQVWs");
                string ReducedUserDir = System.IO.Path.Combine(ProjectSettings.AbsoluteProjectPath, "ReducedUserQVWs");
                string WorkingCacheDir = System.IO.Path.Combine(WorkingDirectory, "ReducedCachedQVWs");
                string WorkingUserDir = System.IO.Path.Combine(WorkingDirectory, "ReducedUserQVWs");

                //first clear the working directories
               if ( Process.SetupWorkingDirectory(WorkingDirectory) == false )
               {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to clear working directory'" + WorkingDirectory + "'");
                    NavigateTo = "HTML/GeneralIssue.aspx?msg=" + MillimanCommon.Utilities.ConvertStringToHex("Failed to clear working directory'" + WorkingDirectory + "'");  
                    TaskCompletionWithError = true;
                    TaskCompleted = true;
                    base.TaskProcessor(parms);
                    return;
               }
               DisplayMessage = "Copying files to local working directories....";
                //Copy selections files & user dirs
                if ( Process.CopyLegacyProjectFiles( WorkingDirectory, ProjectSettings) == false )
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to copy needed files from published project '" + ProjectSettings.ProjectName + "'");
                    NavigateTo = "HTML/GeneralIssue.aspx?msg=" + MillimanCommon.Utilities.ConvertStringToHex("Failed to copy needed files from published project '" + ProjectSettings.ProjectName + "'");
                    TaskCompletionWithError = true;
                    TaskCompleted = true;
                    base.TaskProcessor(parms);
                    return;
                }

                DisplayMessage = "Generating hierarchy information setup for new QVW....";
                //issue request for hierarchy from new master QVW ( needed for autoinclusion processor )
                MillimanCommon.ReduceConfig ReductionConfiguration = Process.NewMasterQVWParameters(WorkingDirectory, ProjectSettings);
                if ( ReductionConfiguration == null )
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to create configuration file to extract data from '" + ProjectSettings.ProjectName + "'");
                    NavigateTo = "HTML/GeneralIssue.aspx?msg=" + MillimanCommon.Utilities.ConvertStringToHex("Failed to create configuration file to extract data from '" + ProjectSettings.ProjectName + "'");
                    TaskCompletionWithError = true;
                    TaskCompleted = true;
                    base.TaskProcessor(parms);
                    return;
                }

                //always do this since we need the hiearchy info anyway
                DisplayMessage = "Request processing to generate new QVW hierarchy....";
                string RootReductionFolder = System.Configuration.ConfigurationManager.AppSettings["ReductionServerRoot"];
                //issue request for hierarchy from new master QVW ( needed for autoinclusion processor )
                bool NewMasterQVWProcessing = Process.ProcessMasterQVW(WorkingDirectory, ProjectSettings, ReductionConfiguration, RootReductionFolder);
                
                if (NewMasterQVWProcessing == false)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to process new master QVW '" + ProjectSettings.ProjectName + "'");
                    NavigateTo = "HTML/GeneralIssue.aspx?msg=" + MillimanCommon.Utilities.ConvertStringToHex("Failed to process new master QVW '" + ProjectSettings.ProjectName + "'");
                    TaskCompletionWithError = true;
                    TaskCompleted = true;
                    base.TaskProcessor(parms);
                    return;
                }

                DisplayMessage = "Hierarchy generated, moving files for processing.....";
                //copy processed files to working directory
                if ( Process.CopyFilesToWorkingDir( WorkingDirectory, ReductionConfiguration) == false )
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to move hierarchy files to local working area '" + ProjectSettings.ProjectName + "'");
                    NavigateTo = "HTML/GeneralIssue.aspx?msg=" + MillimanCommon.Utilities.ConvertStringToHex("Failed to move hierarchy files to local working area '" + ProjectSettings.ProjectName + "'");
                    TaskCompletionWithError = true;
                    TaskCompleted = true;
                    base.TaskProcessor(parms);
                    return;
                }

                if (ProjectSettings.AutomaticInclusion)
                {
                    DisplayMessage = "Checking selections for auto-inclusion.....";
                    //HAVE TO CHECK AUTO INCLUSION Measures!!! can be check via selections vs current hiearchy tree ( adjust selectsions when true by tunking last level of sel )
                    if (Process.AutoInclusion(WorkingDirectory, ProjectSettings) == false)
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Auto inclusion processing failed for '" + ProjectSettings.ProjectName + "'");
                        NavigateTo = "HTML/GeneralIssue.aspx?msg=" + MillimanCommon.Utilities.ConvertStringToHex("Auto inclusion processing failed for '" + ProjectSettings.ProjectName + "'");
                        TaskCompletionWithError = true;
                        TaskCompleted = true;
                        base.TaskProcessor(parms);
                        return;
                    }
                }
                //Create reduction list to implement caching so we do minimal reductions and moving, this will generate a dictionary will
                //duplicate selections files grouped  ie                 UniqueSels.UniqueSelectionDictionary

                DisplayMessage = "Generating reduction configurations.....";
                ProcessingCode.UniqueSelections UniqueSels = new ProcessingCode.UniqueSelections(WorkingDirectory, ProjectSettings, ReductionConfiguration.SelectionSets[0].DataModelFile );

                //coherace each selection file into criteria file( only unique selections ) - this is done on instantation of UniqueSelections
                DisplayMessage = "Moving reduction configurations for processing.....";
                //create working dir on reduction server
                string RemoteReductionDir = System.IO.Path.Combine(RootReductionFolder, Guid.NewGuid().ToString("N"));
                System.IO.Directory.CreateDirectory(RemoteReductionDir);
                if ( System.IO.Directory.Exists(RemoteReductionDir) == false )
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to create remote reduction directory -" + RemoteReductionDir);
                    NavigateTo = "HTML/GeneralIssue.aspx?msg=" + MillimanCommon.Utilities.ConvertStringToHex("Remote reduction server did not allow directory to be created. Please contact PRM support to correct this issue");
                    TaskCompletionWithError = true;
                    TaskCompleted = true;
                    base.TaskProcessor(parms);
                    return;
                }
                //copy master QVW to reduction server working dir
                string QVWRemoteVersion = System.IO.Path.Combine(RemoteReductionDir, ProjectSettings.QVName + ".qvw");
                string QVWLocalVersion = System.IO.Path.Combine(ProjectSettings.AbsoluteProjectPath, ProjectSettings.QVName + ".qvw");
                System.IO.File.Copy( QVWLocalVersion, QVWRemoteVersion);
                if ( System.IO.File.Exists(QVWRemoteVersion) == false )
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to master QVW to remote reduction directory -" + RemoteReductionDir);
                    NavigateTo = "HTML/GeneralIssue.aspx?msg=" + MillimanCommon.Utilities.ConvertStringToHex("Failed to copy master QVW to reduction server. Please contact PRM support to correct this issue");
                    TaskCompletionWithError = true;
                    TaskCompleted = true;
                    base.TaskProcessor(parms);
                    return;
                }

                int TotalAccounts = 0;
                List<string> MissingRemoteFiles = new List<string>();
                //copy all criterai files to reduction server working dir
                int Index = System.Convert.ToInt32( System.Configuration.ConfigurationManager.AppSettings["DebugConfigurationLimit"] );  
                foreach( KeyValuePair<string, ProcessingCode.UniqueSelections.UniqueSelection> KVP in UniqueSels.UniqueSelectionDictionary )
                {
                    string RemoteConfigurationFile = System.IO.Path.Combine(RemoteReductionDir, KVP.Key + ".config");
                    KVP.Value.ReductionConfiguration.Serialize(RemoteConfigurationFile); //write config file to reduction server
                    //check to see if really there
                    if (System.IO.File.Exists( RemoteConfigurationFile) == false)
                    {
                        MissingRemoteFiles.Add(RemoteConfigurationFile);
                    }
                    else
                    {
                        TotalAccounts += KVP.Value.Accounts.Count;
                    }

                    //restrict the number config files tos ave time in testing
                    Index--;
                    if (Index < 0)
                        break;
                }
                //create reduction server semaphore file
                string RemoteSemaphore = System.IO.Path.Combine(RemoteReductionDir, "request_complete.txt");
                System.IO.File.WriteAllText(RemoteSemaphore, " ");

                //loop watching reductions and updating display
                bool ReductionStatus =  WaitOnReductionProcessing(WorkingDirectory, ProjectSettings, RemoteReductionDir, TotalAccounts, UniqueSels);
                
                bool ReductionMovingStatus = WaitOnMovingReductionProcessing(WorkingDirectory, ProjectSettings, RemoteReductionDir, TotalAccounts, UniqueSels);
                
                if ( ReductionStatus == false )
                {
                    NavigateTo = "HTML/ReductionIssue.html";
                    TaskCompletionWithError = true;
                    TaskCompleted = true;
                    base.TaskProcessor(parms);
                    return;
                }
                else if ( ReductionMovingStatus == false )
                {
                    NavigateTo = "HTML/ReductionMoveIssue.html";
                    TaskCompletionWithError = true;
                    TaskCompleted = true;
                    base.TaskProcessor(parms);
                    return;
                }
                else
                {
                    if ( WaitOnSystemReportProcessing(WorkingDirectory, ProjectSettings, RemoteReductionDir, TotalAccounts, UniqueSels) )
                    {
                        //write this file we are ready, don't use session since we might need to trigger manually
                        string ReadyFile = System.IO.Path.Combine(WorkingDirectory, "ready_to_publish.txt");
                        System.IO.File.WriteAllText(ReadyFile, DateTime.Now.ToString());
                        NavigateTo = "ReductionStep2.aspx?key=" + ReductionIndex;  //report generation
                        TaskCompletionWithError = true;
                        TaskCompleted = true;
                        base.TaskProcessor(parms);
                    }
                    else
                    {
                        NavigateTo = "HTML/SystemReportingError.html";
                        TaskCompletionWithError = true;
                        TaskCompleted = true;
                        base.TaskProcessor(parms);
                        return;
                    }
                }
            }

            private bool WaitOnReductionProcessing(string WorkingDirectory, MillimanCommon.ProjectSettings Settings, string RemoteReductionDir, int TotalAccounts, ProcessingCode.UniqueSelections UniqueSels )
            {
                try
                {
                    string CompletedSemaphore = System.IO.Path.Combine(RemoteReductionDir, @"processing_complete.txt");

                    while (System.IO.File.Exists(CompletedSemaphore) == false)
                    {
                        System.Threading.Thread.Sleep(1000);  //sleep 1 second
                        string[] RunningFiles = System.IO.Directory.GetFiles(RemoteReductionDir, "*_running.txt");
                        string[] CompletedFiles = System.IO.Directory.GetFiles(RemoteReductionDir, "*_completed.txt");
                        int RunningAccounts = 0;
                        int CompletedAccounts = 0;
                        foreach (string RunFile in RunningFiles)
                        {
                            string Key = System.IO.Path.GetFileName(RunFile);
                            Key = Key.Substring(0, Key.IndexOf("_running.txt"));
                            RunningAccounts += UniqueSels.UniqueSelectionDictionary[Key].Accounts.Count;
                        }
                        foreach (string CompletedFile in CompletedFiles)
                        {
                            string Key = System.IO.Path.GetFileName(CompletedFile);
                            Key = Key.Substring(0, Key.IndexOf("_completed.txt"));
                            CompletedAccounts += UniqueSels.UniqueSelectionDictionary[Key].Accounts.Count;
                        }
                        int QueuedAccounts = TotalAccounts - (RunningAccounts + CompletedAccounts);
                        string Progress = "PROGRESS:TOTAL:" + TotalAccounts.ToString() + ":QUEUED:" + QueuedAccounts.ToString() + ":RUNNING:" + RunningAccounts.ToString() + ":COMPLETED:" + CompletedAccounts.ToString();

                        TotalUser = TotalAccounts;
                        Executing = RunningAccounts;
                        Completed = CompletedAccounts;
                        Errored = TotalAccounts - (RunningAccounts + CompletedAccounts); //is this correct??????
                        ShowAllProgressBars = true;
                        DisplayMessage = "Creating Reduced Reports";
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed reduction", ex);
                }
                return false;

            }

            /// <summary>
            /// Move all the files generated on remote server to local working directory
            /// </summary>
            /// <param name="WorkingDirectory"></param>
            /// <param name="Settings"></param>
            /// <param name="RemoteReductionDir"></param>
            /// <param name="TotalAccounts"></param>
            /// <param name="UniqueSels"></param>
            /// <returns></returns>
            private bool WaitOnMovingReductionProcessing(string WorkingDirectory, MillimanCommon.ProjectSettings Settings, string RemoteReductionDir, int TotalAccounts, ProcessingCode.UniqueSelections UniqueSels)
            {
                try
                {
                    string MasterQVW = Settings.QVName + ".qvw";

                    ShowAllProgressBars = false;
                    ShowOnlyMoveProgressBar = true;
                    DisplayMessage = "Retrieving Processed Reports";
                    Errored = 0;
                    Completed = 0;
                    Executing = 0;
                    string[] AllFiles = System.IO.Directory.GetFiles(RemoteReductionDir, "*.*", System.IO.SearchOption.AllDirectories);
                    TotalUser = AllFiles.Count();

                    string Extension = string.Empty;
                    foreach( string File in AllFiles )
                    {
                        Extension = System.IO.Path.GetExtension(File).ToLower();
                        if ( Extension != ".txt" )
                        {
                            //move me, am a candidate,  put config files in the confi dir and QVWs in ReducedCachedQVW, others in workingdirectory
                            string DestinationFile = System.IO.Path.Combine(WorkingDirectory, System.IO.Path.GetFileName(File));
                            if ((Extension == ".config") || (Extension == ".log"))
                            {
                                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(WorkingDirectory, "configurations"));
                                DestinationFile = System.IO.Path.Combine(WorkingDirectory, "configurations", System.IO.Path.GetFileName(File));
                            }
                            else if ( Extension == ".qvw")
                            {
                                DestinationFile = System.IO.Path.Combine(WorkingDirectory, "ReducedCachedQVWs", System.IO.Path.GetFileName(File));
                            }
                            System.IO.File.Delete(DestinationFile);
                            System.IO.File.Copy(File, DestinationFile);
                        }
                        Completed++;  //update this counter, outside of loop since the skipped files are already present locally
                    }

                    //at this point we also need to move the selections and bookmark files up from legacy directory
                    string SelDestination = System.IO.Path.Combine(WorkingDirectory, "ReducedUserQVWs");
                    string SelSource = System.IO.Path.Combine(WorkingDirectory, @"legacy\ReducedUserQVWs");
                    MillimanCommon.Utilities.DirectoryCopy(SelSource, SelDestination, true);  //copy all files

                    //Do not copy project file from 'legacy' should already exist correctly in new parent directory
                    //string ProjectSource = System.IO.Path.Combine(WorkingDirectory, "Legacy", ProjectSettings.QVName + ".hciprj");
                    //string ProjectDestination = System.IO.Path.Combine(WorkingDirectory, ProjectSettings.QVName + ".hciprj");
                    //System.IO.File.Copy(ProjectSource, ProjectDestination);

                    //if no icon in new directory, copy up the legacy version
                    if (string.IsNullOrEmpty(Settings.QVThumbnail) == false)
                    {
                        string IconDestination = System.IO.Path.Combine(WorkingDirectory, Settings.QVThumbnail);
                        if (System.IO.File.Exists(IconDestination) == false)
                        {  //no new one available, so copy up the old one
                            string IconSource = System.IO.Path.Combine(WorkingDirectory, "legacy", Settings.QVThumbnail);
                            if (System.IO.File.Exists(IconSource))
                                System.IO.File.Copy(IconSource, IconDestination);
                        }
                    }

                    //if no user manual in new directory copy up legacy version
                    if (string.IsNullOrEmpty(Settings.UserManual) == false)
                    {
                        string ManualDestination = System.IO.Path.Combine(WorkingDirectory, Settings.UserManual);
                        if (System.IO.File.Exists(ManualDestination) == false)
                        {  //no new one available, so copy up the old one
                            string ManualSource = System.IO.Path.Combine(WorkingDirectory, "legacy", Settings.UserManual);
                            if (System.IO.File.Exists(ManualSource))
                                System.IO.File.Copy(ManualSource, ManualDestination);
                        }
                    }

                    List<string> BookmarkExtenstions = new List<string>() { "*.shared", "*.meta", "*.cacheIndex" };
                    string BookmarkDestination = System.IO.Path.Combine(WorkingDirectory, "ReducedCachedQVWs");
                    string BookmarkSource = System.IO.Path.Combine(WorkingDirectory, @"legacy\ReducedCachedQVWs");
                    MillimanCommon.Utilities.DirectoryCopy(BookmarkSource, BookmarkDestination, true, BookmarkExtenstions); 
                    return true;

                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed system report generation from reduction processing", ex);
                }
                return false;
            }
            /// <summary>
            /// build all the reports here
            /// </summary>
            /// <param name="WorkingDirectory"></param>
            /// <param name="Settings"></param>
            /// <param name="RemoteReductionDir"></param>
            /// <param name="TotalAccounts"></param>
            /// <param name="UniqueSels"></param>
            /// <returns></returns>
            private bool WaitOnSystemReportProcessing(string WorkingDirectory, MillimanCommon.ProjectSettings Settings, string RemoteReductionDir, int TotalAccounts, ProcessingCode.UniqueSelections UniqueSels)
            {
                try
                {
                    ShowAllProgressBars = false;
                    DisplayMessage = "Creating Processing Summary Reports";

                    MillimanCommon.QVWReportGenerator RepGen = new MillimanCommon.QVWReportGenerator();
                    Reports = new MillimanCommon.QVWReportBank(System.IO.Path.Combine(WorkingDirectory, Settings.QVName + @".hciprj"));

                    //Reports.AddItemToList(new MillimanCommon.QVWReportBank.NewItemClass("conceptname", "value"));
                    string OldHierarchyFile = System.IO.Path.Combine(Settings.AbsoluteProjectPath, Settings.QVName + ".hierarchy_0");
                    string NewHierarchyFile = System.IO.Path.Combine(WorkingDirectory, Settings.QVName + ".hierarchy_0");
                    MillimanCommon.MillimanTreeNode OldHierarchy = MillimanCommon.MillimanTreeNode.GetMemoryTree(OldHierarchyFile);
                    MillimanCommon.MillimanTreeNode NewHierarchy = MillimanCommon.MillimanTreeNode.GetMemoryTree(NewHierarchyFile);
                    RepGen.GenerateNewItemsReport(OldHierarchy, NewHierarchy, Reports);  //pass in reports instance to update
                    
                    //Reports.AddItemToList(new MillimanCommon.QVWReportBank.NotSelectableClass("Username", "FieldName", "ConceptFieldName", "QVWName", "reasons"));
                    RepGen.GenerateNotSelectable(NewHierarchy, System.IO.Path.Combine(WorkingDirectory, Settings.QVName + @".hciprj"), Reports);

                    //Reports.AddItemToList(new MillimanCommon.QVWReportBank.AutoInclusionClass("Account", "FieldName", "ConceptFielname","QVWName", "value"));
                    RepGen.AutoInclusionProcessor(WorkingDirectory, Settings, Reports);

                    //this class will contain if a reduced report is reason == "Reduced - Available","Failed","Copied - Available","Starting reduction"
                    //Reports.AddItemToList(new MillimanCommon.QVWReportBank.ProcessingStatusClass("Account", "QVWN", "reason"));
                    RepGen.ProcessingStatusProcessor(WorkingDirectory, Settings, Reports, UniqueSels.ReducedReportToAccountMapping());


                    //general logs on issues
                    //Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("message"));
                    RepGen.MasterLogToAudit(WorkingDirectory, Reports);

                    Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("Yo Van"));
                    
                    return true;

                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed system report generation from reduction processing", ex);
                }
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["Processing"] == null)
                {
                    int Index = 0;
                    if (Request["Key"] != null)
                        Index = System.Convert.ToInt32(Request["Key"]);
                    else
                    {
                        string NavigateTo = "HTML/GeneralIssue.aspx?msg=" + MillimanCommon.Utilities.ConvertStringToHex("Missing project index - cannot publish project.'");
                        Response.Redirect(NavigateTo);
                        return;
                    }

                    ProjectSettingsExtension theProject = null;
                    IList<ProjectSettingsExtension> Projects = Session["Projects"] as IList<ProjectSettingsExtension>;
                    if ((Projects != null) && (Index < Projects.Count()))
                    {
                        theProject = Projects[Index];
                    }
                    if (theProject == null)
                    {
                        Response.Redirect("HTML/MissingProject.html");
                        return;
                    }
                    if (System.Web.Security.Membership.GetUser() == null)
                    {
                        Response.Redirect("HTML/NotAuthorizedIssue.html");
                        return;
                    }
                    string WorkingDirectory = PublisherUtilities.GetWorkingDirectory(System.Web.Security.Membership.GetUser().UserName, theProject.ProjectName);
                    SimpleUpdateClass SU = new SimpleUpdateClass();
                    SU.ReductionIndex = Index.ToString();
                    SU.ProjectSettings = theProject;
                    SU.WorkingDirectory = WorkingDirectory;
                    SU.Account = System.Web.Security.Membership.GetUser().UserName;
                    SU.ShowAllProgressBars = false;
                    Global.TaskManager.ScheduleTask(SU);
                    SU.StartTask();
                    Response.Redirect("ReductionStep1.aspx?Processing=" + SU.TaskID);
                }
                else
                {
                    string ProcessID = Request["Processing"].ToString();
                    SimpleUpdateClass SU = Global.TaskManager.FindTask(ProcessID) as SimpleUpdateClass;
                    if (SU != null)
                    {
                        Status.Text = SU.DisplayMessage;
                        if (SU.TaskCompletionWithError)
                        {
                            Status.Text = SU.TaskCompletionMessage;
                            SU.AbortTask = true;
                            Global.TaskManager.DeleteTask(SU.TaskID);
                            Response.Redirect(SU.NavigateTo);
                        }
                        else if (SU.TaskCompleted) 
                        {
                            Status.Text = SU.TaskCompletionMessage;
                            Status.Visible = true;
                            Global.TaskManager.DeleteTask(SU.TaskID);
                            Response.Redirect(SU.NavigateTo);
                        }
                        else
                        {
                            Status.Text = SU.DisplayMessage;
                            if ( SU.ShowAllProgressBars)
                            { 
                               //"PROGRESS:TOTAL:" + TotalAccounts.ToString() + ":QUEUED:" + QueuedAccounts.ToString() + ":RUNNING:" + RunningAccounts.ToString() + ":COMPLETED:" + CompletedAccounts.ToString();

                                NumberCompleted.Text = SU.Completed.ToString();
                                NumberPending.Text = (SU.TotalUser - (SU.Executing + SU.Completed)).ToString();
                                NumberProcessing.Text = SU.Executing.ToString();
                                TotalUsers.Text = SU.TotalUser.ToString();
                                Errored.Width = new Unit(0.0, UnitType.Percentage);
                                Executing.Width = new Unit((((double)SU.Executing + (double)SU.Completed) / (double)SU.TotalUser) * 100.0, UnitType.Percentage);  //yes executing + completed is correct, we want exe bar to stick out from behind completed
                                Finished.Width = new Unit(((double)SU.Completed / (double)SU.TotalUser) * 100.0, UnitType.Percentage);
                                ProgressBars.Visible = true;
                                ProgressBarLegends.Visible = true;
                            }
                            MillimanCommon.Alert.Refresh(3);
                        }
                    }
                    else
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "System error - missing task");
                        Response.Redirect("errors/missingtask.aspx");
                        Status.Text = "Could not retrieve processing status.";
                    }
                }
            }
        }
    }
}