using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MillimanReportReduction.QMSAPIService;
using MillimanReportReduction.ServiceSupport;
using System.Threading;
using System.ComponentModel;

namespace MillimanReportReduction
{
    public class QVWHierarchy
    {
        //fully qualified QVW to index
        public string QualifiedQVWNameToIndex { get; set; }

        //file that sets boolean value so that partial reload can extract files
        public string QualifiedDebugFile { get; set; }

        //set task names and description if required
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }

        //should task be deleted upon completion
        public bool DeleteTaskOnCompletion { get; set; }
        public delegate void TaskCallback(QVWHierarchy Task);
        public TaskCallback Callback;

        //list of index files extracted from QVW
        public List<string> ExtractedHierarchyFiles { get; set; }

        //status return variables
        public DateTime TaskCompletedAt { get; set; }
        public string TaskStatusMsg { get; set; }
        public Guid TaskID { get; set; }

        private BackgroundWorker BGW = new BackgroundWorker();


        public QVWHierarchy()
        {
            Callback = null;
            DeleteTaskOnCompletion = true;
            TaskID = Guid.NewGuid();
            ExtractedHierarchyFiles = new List<string>();
            BGW.WorkerReportsProgress = false;
            BGW.WorkerSupportsCancellation = false;
        }

        public bool ExtractHierarchyNonBlocking()
        {
            BGW.DoWork += DoHierarchyWork;
            BGW.RunWorkerCompleted += DoHierarchyWorkCompleted;
            BGW.RunWorkerAsync(this);
            return true;
        }

        /// <summary>
        /// create a subdirectory and put the debug=true file in it
        /// </summary>
        /// <param name="SourceRoot"></param>
        /// <returns></returns>
        private string PrepSourceDirectory(string SourceRoot, out string SubDirectoryName, out string QualafiedQVWUniqueName, out string _QualafiedDebugFile )
        {
            SubDirectoryName = TaskID.ToString("N").ToUpper();
            string SubDir = System.IO.Path.Combine(SourceRoot, SubDirectoryName );
            System.IO.Directory.CreateDirectory(SubDir);

            QualafiedQVWUniqueName = System.IO.Path.Combine(SubDir, SubDirectoryName + ".qvw");
            _QualafiedDebugFile = string.Empty;
            string AbsoluteDebugFileValue = QualifiedDebugFile;
            string DebugFilename = System.IO.Path.GetFileName(AbsoluteDebugFileValue);

            System.IO.File.Copy(AbsoluteDebugFileValue, System.IO.Path.Combine(SubDir, DebugFilename), true);
            System.IO.File.Copy(AbsoluteDebugFileValue, System.IO.Path.Combine(SubDir, "ancillary_script.txt"), true);

            if ( System.IO.File.Exists(System.IO.Path.Combine(SubDir, DebugFilename)) == false )
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to copy debug file from '" + AbsoluteDebugFileValue + "' to '" + System.IO.Path.Combine(SubDir, DebugFilename) + "'");
                return "";

            }
            _QualafiedDebugFile = System.IO.Path.Combine(SubDir, DebugFilename);
            return SubDir;
        }

        public bool ExtractHierarchyBlocking()
        {
            try
            {
                QMSClient Client;
                string QMS = "http://localhost:4799/QMS/Service";
                Client = new QMSClient("BasicHttpBinding_IQMS", QMS);
                string key = Client.GetTimeLimitedServiceKey();
                ServiceKeyClientMessageInspector.ServiceKey = key;

                ServiceInfo[] serviceList = Client.GetServices(ServiceTypes.QlikViewDistributionService);
                if (serviceList.Count() != 1)
                {
                    TaskStatusMsg = @"Could not get distribution service";
                    return false;
                }
                Guid qdsGuid = serviceList[0].ID;

                //we need to copy the source over first
                DocumentFolder[] DFolders = Client.GetSourceDocumentFolders(qdsGuid, DocumentFolderScope.All);
                //there should only be 1
                if (DFolders.Count() != 1)
                {
                    TaskStatusMsg = @"QV source folder container contains multiple sources, it should only contain a single source. Please modify mounted source folders via QVManagement Console";
                    return false;
                }
                //Prep source folder will create a sub dir to work in for this request
                string SubDirName = string.Empty;
                string QualafiedUniqueQVWName = string.Empty;
                string QualafiedDebugFile = string.Empty;
                string SourceFolder = PrepSourceDirectory( DFolders[0].General.Path, out SubDirName, out QualafiedUniqueQVWName, out QualafiedDebugFile );
                if (string.IsNullOrEmpty(SourceFolder))
                {
                    TaskStatusMsg = @"Failed to prep directory for hierarchy extraction";
                    return false;
                }
                System.IO.File.Copy(QualifiedQVWNameToIndex, QualafiedUniqueQVWName);
                if (System.IO.File.Exists(QualafiedUniqueQVWName) == false)
                {
                    TaskStatusMsg = @"Failed to move QVW to source folder";
                    return false;
                }

                Client.ClearQVSCache(QVSCacheObjects.All);

                //start the reduction code
                DocumentTask documentTask = new DocumentTask();
                documentTask.ID = TaskID;


                DocumentNode[] sourceDocuments = Client.GetSourceDocuments(qdsGuid);
                documentTask.QDSID = qdsGuid;
                DocumentNode DocNode = null;
                foreach (DocumentNode DN in sourceDocuments)
                {
                    if (string.Compare(DN.Name, System.IO.Path.GetFileName(QualafiedUniqueQVWName), true) == 0)
                    {
                        DocNode = DN;
                        break;
                    }
                }
                if (DocNode == null)
                {
                    TaskStatusMsg = @"Failed to find source docuemnt, check to see that '" + System.IO.Path.GetFileName(QualafiedUniqueQVWName) + "' exists in the QV source folder";
                    return false;
                }
                documentTask.Document = DocNode;
                documentTask.General = new DocumentTask.TaskGeneral();
                documentTask.General.Enabled = true;
                documentTask.General.TaskName = TaskName;
                documentTask.General.TaskDescription = TaskDescription;
                documentTask.Scope = DocumentTaskScope.General | DocumentTaskScope.Reduce | DocumentTaskScope.Reload | DocumentTaskScope.Distribute | DocumentTaskScope.Triggering;

                documentTask.Reduce = new DocumentTask.TaskReduce();
                documentTask.Reduce.DocumentNameTemplate = "";

                documentTask.Reduce.Static = new DocumentTask.TaskReduce.TaskReduceStatic();

                documentTask.Reduce.Static.Reductions = new TaskReduction[0];

                documentTask.Reduce.Dynamic = new DocumentTask.TaskReduce.TaskReduceDynamic();

                documentTask.Distribute = new DocumentTask.TaskDistribute();
                documentTask.Distribute.Output = new DocumentTask.TaskDistribute.TaskDistributeOutput();
                documentTask.Distribute.Output.Type = TaskDistributionOutputType.QlikViewDocument;
                documentTask.Distribute.Static = new DocumentTask.TaskDistribute.TaskDistributeStatic();
                documentTask.Distribute.Static.DistributionEntries = new TaskDistributionEntry[1];
                documentTask.Distribute.Static.DistributionEntries[0] = new TaskDistributionEntry();

                documentTask.Distribute.Static.DistributionEntries[0].Recipients = new DirectoryServiceObject[1];
                documentTask.Distribute.Static.DistributionEntries[0].Recipients[0] = new DirectoryServiceObject();
                documentTask.Distribute.Static.DistributionEntries[0].Recipients[0].Type = DirectoryServiceObjectType.Authenticated;

                documentTask.Distribute.Static.DistributionEntries[0].Destination = new TaskDistributionDestination();
                documentTask.Distribute.Static.DistributionEntries[0].Destination.Folder = new TaskDistributionDestination.TaskDistributionDestinationFolder();

                documentTask.Distribute.Static.DistributionEntries[0].Destination.Type = TaskDistributionDestinationType.None;
                documentTask.Distribute.Static.DistributionEntries[0].Destination.Folder.Name = "";

                documentTask.Distribute.Notification = new DocumentTask.TaskDistribute.TaskDistributeNotification();
                documentTask.Distribute.Notification.SendNotificationEmail = false;
                documentTask.Distribute.Dynamic = new DocumentTask.TaskDistribute.TaskDistributeDynamic();
                documentTask.Distribute.Dynamic.IdentityType = UserIdentityValueType.DisplayName;

                documentTask.Distribute.Dynamic.Destinations = new TaskDistributionDestination[0];
                //documentTask.Distribute.Dynamic.Destinations[0] = new TaskDistributionDestination();
                //documentTask.Distribute.Dynamic.Destinations[0].Folder = new TaskDistributionDestination.TaskDistributionDestinationFolder();
                //documentTask.Distribute.Dynamic.Destinations[0].Folder.Name = @"c:\Data";
                //documentTask.Distribute.Dynamic.Destinations[0].Type = TaskDistributionDestinationType.Folder;
                //documentTask.Distribute.Dynamic.FieldName = "Decade";

                //documentTask.Distribute.Dynamic.ValidateEmails = false;

                documentTask.Reload = new DocumentTask.TaskReload();
                documentTask.Reload.Mode = TaskReloadMode.Partial;

                //documentTask.DocumentInfo = new DocumentTask.TaskDocumentInfo();
                documentTask.Triggering = new DocumentTask.TaskTriggering();
                documentTask.Triggering.ExecutionAttempts = 1;
                documentTask.Triggering.ExecutionTimeout = 1440;
                documentTask.Triggering.TaskDependencies = new TaskInfo[0];
                //documentTask.Triggering.TaskDependencies[0] = new TaskInfo();
                //documentTask.Triggering.TaskDependencies[0].ID = Guid.NewGuid();
                //documentTask.Triggering.TaskDependencies[0].Enabled = false;
                //documentTask.Triggering.TaskDependencies[0].Type = TaskType.DocumentTask;
           
                documentTask.Triggering.Triggers = new ScheduleTrigger[1];
                ScheduleTrigger ST = new ScheduleTrigger();
                ST.Enabled = true;
                ST.Type = TaskTriggerType.OnceTrigger;

                ST.StartAt = DateTime.Now; //don't schedule for now, it ignores if in past
                documentTask.Triggering.Triggers[0] = ST;
                //do something else
                
                Client.SaveDocumentTask(documentTask);

                //loop till task is ready
                TaskInfo TI = Client.FindTask(qdsGuid, TaskType.DocumentTask, documentTask.General.TaskName);
                while (TI == null)
                {
                    System.Threading.Thread.Sleep(100);
                    TI = Client.FindTask(qdsGuid, TaskType.DocumentTask, documentTask.General.TaskName);
                }
               
                System.Threading.Thread.Sleep(4000);//give it a second
                Client.RunTask(documentTask.ID);

                TaskStatus taskStatus = Client.GetTaskStatus(documentTask.ID, TaskStatusScope.All);
                bool Finished = false;
                //while (string.IsNullOrEmpty(taskStatus.Extended.LastLogMessages))
                DateTime TaskCompletedAt = DateTime.Now;
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                int MaxMilliseconds = 60 * 20 * 1000;  //only wait 20 mins for this
                bool FinishedWithError = false;
                while (Finished == false)
                {
                    System.Threading.Thread.Sleep(200);
                    taskStatus = Client.GetTaskStatus(documentTask.ID, TaskStatusScope.All);
                    Finished = DateTime.TryParse(taskStatus.Extended.FinishedTime, out TaskCompletedAt);

                    if (stopwatch.ElapsedMilliseconds > MaxMilliseconds)
                    {
                        FinishedWithError = true;
                        TaskStatusMsg = "Task timed out and did not execute";
                        TaskCompletedAt = DateTime.Now;
                    }
                }

                if (FinishedWithError == false)
                {
                    TaskCompletedAt = System.Convert.ToDateTime(taskStatus.Extended.FinishedTime);
                    TaskStatusMsg = taskStatus.Extended.LastLogMessages;
                }
                //delete all data other than the extracted data
                if (DeleteTaskOnCompletion == true)
                {
                    bool Status = Client.DeleteTask(documentTask.ID, TaskType.DocumentTask);
                    System.IO.File.Delete(QualafiedUniqueQVWName);  //go ahead and get rid of copied source file
                    System.IO.File.Delete(QualafiedDebugFile);
                }

                if (FinishedWithError == false)
                {
                    string[] ExtractedHierarchy = System.IO.Directory.GetFiles(SourceFolder);

                    if (ExtractedHierarchy.Count() < 2)
                    {
                        if (string.IsNullOrEmpty(TaskStatusMsg) == true)
                            TaskStatusMsg = @"Check selections, could not extract index information";
                        return false;
                    }
                    else
                    {
                        ExtractedHierarchyFiles = ExtractedHierarchy.ToList();
                        return true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                TaskStatusMsg = ex.ToString();
            }
            return false;
        }

        private void DoHierarchyWork(object sender, DoWorkEventArgs e)
        {
            QVWHierarchy Hierarchy = e.Argument as QVWHierarchy;
            if (Hierarchy != null)
            {
                try
                {
                    e.Result = Hierarchy.ExtractHierarchyBlocking();

                }
                catch (System.Exception ex)
                {
                    Hierarchy.TaskStatusMsg = ex.ToString();
                }
            }
            e.Result = false;
        }

        private void DoHierarchyWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Callback != null)
                Callback(this);
        }
    }
}
