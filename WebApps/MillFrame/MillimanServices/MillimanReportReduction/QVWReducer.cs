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
    public class QVWReducer
    {
        //input variables
        public string QualifiedQVWNameToReduce { get; set; }
        public List<NVPairs> Variables { get; set; }
        public string QualifiedReducedQVWName { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public bool DeleteTaskOnCompletion { get; set; }
        public delegate void TaskCallback(QVWReducer Task);
        public TaskCallback Callback;

        //return variables
        public DateTime TaskCompletedAt { get; set; }
        public string TaskStatusMsg { get; set; }
        public Guid TaskID { get; set; }

        private BackgroundWorker BGW = new BackgroundWorker();

        public QVWReducer()
        {
            Callback = null;
            Variables = new List<NVPairs>();
            DeleteTaskOnCompletion = true;
            TaskID = Guid.NewGuid();

            BGW.WorkerReportsProgress = false;
            BGW.WorkerSupportsCancellation = false;
            
        }

        public bool ReduceNonBlocking()
        {
            BGW.DoWork += DoReductionWork;
            BGW.RunWorkerCompleted += DoReductionWorkCompleted;
            BGW.RunWorkerAsync(this);
            return true;
        }

        public bool ReduceBlocking()
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
                    string SourceFolder = DFolders[0].General.Path;
                    //string SourceFolderFile = System.IO.Path.Combine(SourceFolder, System.IO.Path.GetFileName(QualifiedQVWNameToReduce));
                    string UID = System.Guid.NewGuid().ToString("N");
                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(SourceFolder, UID));
                    string SourceFolderFile = System.IO.Path.Combine(SourceFolder,UID, UID + ".qvw");
                    
                    System.IO.File.Copy(QualifiedQVWNameToReduce, SourceFolderFile);
                    if (System.IO.File.Exists(SourceFolderFile) == false)
                    {
                        TaskStatusMsg = @"Failed to move QVW to source folder";
                        return false;
                    }

                    //crap code to fix the QV publisher,  we have to check and see if tables were selected
                    //if a table was not selected we need to drop it
                    //bool DropRequired = DropTableProcessor(System.IO.Path.Combine(SourceFolder, UID));
                    bool DropRequired = DropTableProcessor(System.IO.Path.Combine(SourceFolder,UID));

                    System.Threading.Thread.Sleep(2000); //sleep to give QV server a chance to load up new source

                    //delete any previous reduced files
                    System.IO.File.Delete(QualifiedReducedQVWName);

                    //start the reduction code
                    DocumentTask documentTask = new DocumentTask();
                    documentTask.ID = TaskID;


                    DocumentNode[] sourceDocuments = Client.GetSourceDocuments(qdsGuid);
                    documentTask.QDSID = qdsGuid;
                    DocumentNode DocNode = null;
                    foreach (DocumentNode DN in sourceDocuments)
                    {
                        if (string.Compare(DN.Name, System.IO.Path.GetFileName(SourceFolderFile), true) == 0)
                        {
                            DocNode = DN;
                            break;
                        }
                    }
                    if (DocNode == null)
                    {
                        TaskStatusMsg = @"Failed to find source docuemnt, check to see that '" + System.IO.Path.GetFileName(QualifiedQVWNameToReduce) + "' exists in the QV source folder";
                        return false;
                    }
                    documentTask.Document = DocNode;
                    documentTask.General = new DocumentTask.TaskGeneral();
                    documentTask.General.Enabled = true;
                    documentTask.General.TaskName = TaskName;
                    documentTask.General.TaskDescription = TaskDescription;
                    documentTask.Scope = DocumentTaskScope.General | DocumentTaskScope.Reduce | DocumentTaskScope.Reload | DocumentTaskScope.Distribute | DocumentTaskScope.Triggering;

                    documentTask.Reduce = new DocumentTask.TaskReduce();
                    documentTask.Reduce.DocumentNameTemplate = System.IO.Path.GetFileNameWithoutExtension(QualifiedReducedQVWName);

                    documentTask.Reduce.Static = new DocumentTask.TaskReduce.TaskReduceStatic();

                    documentTask.Reduce.Static.Reductions = new TaskReduction[Variables.Count];
                    int Index = 0;
                    foreach (NVPairs NVP in Variables)
                    {
                        documentTask.Reduce.Static.Reductions[Index] = new TaskReduction();
                        documentTask.Reduce.Static.Reductions[Index].Type = TaskReductionType.ByField;
                        documentTask.Reduce.Static.Reductions[Index].Field = new TaskReduction.TaskReductionField();
                        documentTask.Reduce.Static.Reductions[Index].Field.Name = NVP.Name;
                        documentTask.Reduce.Static.Reductions[Index].Field.Value = NVP.Value;
                        documentTask.Reduce.Static.Reductions[Index].Field.IsNumeric = NVP.IsNumeric;
                        Index++;
                    }

                    //documentTask.Reduce.Static.Reductions = new TaskReduction[2];
                    //documentTask.Reduce.Static.Reductions[0] = new TaskReduction();
                    //documentTask.Reduce.Static.Reductions[0].Type = TaskReductionType.ByField;
                    //documentTask.Reduce.Static.Reductions[0].Field = new TaskReduction.TaskReductionField();
                    ////documentTask.Reduce.Static.Reductions[0].Field.Name = "Decade";
                    ////documentTask.Reduce.Static.Reductions[0].Field.Value = "1920's";
                    //documentTask.Reduce.Static.Reductions[0].Field.Name = "Title";
                    //documentTask.Reduce.Static.Reductions[0].Field.Value = "America";

                    //documentTask.Reduce.Static.Reductions[1] = new TaskReduction();
                    //documentTask.Reduce.Static.Reductions[1].Field = new TaskReduction.TaskReductionField();
                    //documentTask.Reduce.Static.Reductions[1].Type = TaskReductionType.ByField;
                    ////documentTask.Reduce.Static.Reductions[1].Field.Name = "Decade";
                    ////documentTask.Reduce.Static.Reductions[1].Field.Value = "1940's";
                    //documentTask.Reduce.Static.Reductions[1].Field.Name = "Decade";
                    //documentTask.Reduce.Static.Reductions[1].Field.Value = "1920's";


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

                    documentTask.Distribute.Static.DistributionEntries[0].Destination.Type = TaskDistributionDestinationType.Folder;
                    documentTask.Distribute.Static.DistributionEntries[0].Destination.Folder.Name = System.IO.Path.GetDirectoryName(QualifiedReducedQVWName);

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
                    if ( DropRequired )
                        documentTask.Reload.Mode = TaskReloadMode.Partial; 
                    else
                        documentTask.Reload.Mode = TaskReloadMode.None;
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
                    ST.StartAt = DateTime.Now.AddSeconds(5.0); //don't schedule for now, it ignores if in past
                    documentTask.Triggering.Triggers[0] = ST;

                    //do something else
                    Client.SaveDocumentTask(documentTask);

                TaskInfo TI = Client.FindTask(qdsGuid, TaskType.DocumentTask, documentTask.General.TaskName);
                while (TI == null)
                {
                    System.Threading.Thread.Sleep(100);
                    TI = Client.FindTask(qdsGuid, TaskType.DocumentTask, documentTask.General.TaskName);
                }

                //keep looping and telling to run, until it RUNS
                TaskStatus taskStatus = Client.GetTaskStatus(documentTask.ID, TaskStatusScope.All);
                while (string.Compare(taskStatus.General.Status.ToString(), "running", true) != 0)
                {
                    Client.RunTask(documentTask.ID);
                    System.Threading.Thread.Sleep(1000);
                    taskStatus = Client.GetTaskStatus(documentTask.ID, TaskStatusScope.All);
                }

                //loop till we get a log status message
                taskStatus = Client.GetTaskStatus(documentTask.ID, TaskStatusScope.All);
                while (string.IsNullOrEmpty(taskStatus.Extended.LastLogMessages))
                {
                    System.Threading.Thread.Sleep(1000);
                    taskStatus = Client.GetTaskStatus(documentTask.ID, TaskStatusScope.All);
                }

                TaskCompletedAt = System.Convert.ToDateTime(taskStatus.Extended.FinishedTime);
                TaskStatusMsg = taskStatus.Extended.LastLogMessages;

                if (DeleteTaskOnCompletion == true)
                    {
                        bool Status = Client.DeleteTask(documentTask.ID, TaskType.DocumentTask);
                        System.IO.Directory.Delete(System.IO.Path.GetDirectoryName(SourceFolderFile), true);
                    }

                    if (System.IO.File.Exists(QualifiedReducedQVWName) == false)
                    {
                        if (string.IsNullOrEmpty(TaskStatusMsg) == true)
                            TaskStatusMsg = @"Check selections, file could not reduced with requested parameters";
                        return false;
                    }
                    else
                        return true;
                }
                catch (System.Exception ex)
                {
                    TaskStatusMsg = ex.ToString();
                }
            return false;
        }

        private bool DropTableProcessor(string SourceDir)
        {
            try
            {
                int MapIndex = 0;
                string MapFile = QualifiedQVWNameToReduce.Replace(".qvw", ".map_" + MapIndex.ToString());
                while (System.IO.File.Exists(MapFile))
                {
                    MillimanCommon.TreeUtilities.DropTableProcessor DTP = MillimanCommon.TreeUtilities.DropTableProcessor.Load(MapFile);
                    if (DTP != null)
                    {
                        List<string> AllSelections = new List<string>();
                        foreach (NVPairs NVP in Variables)
                        {
                            DTP.SetFieldAccessed(NVP.Name);
                            AllSelections.Add(NVP.Name);
                        }
                        string DataModelFile = MapFile.ToLower().Replace(@".map_", @".datamodel_");
                        List<string> DroppedSelections = new List<string>();
                        List<string> TablesToDrop = DTP.TablesToDrop(DataModelFile, AllSelections, out DroppedSelections );
                        if ( (TablesToDrop != null) && (TablesToDrop.Count > 0) )
                        {
                            //remove the selected variables that should be dropped
                            foreach (string DroppedSelection in DroppedSelections)
                            {
                                //must remove all instances, thus the loop to remove all
                                while ( Variables.FindIndex(x => string.Compare(DroppedSelection, x.Name, true) == 0) >= 0 )
                                    Variables.RemoveAt(Variables.FindIndex(x => string.Compare(DroppedSelection, x.Name, true) == 0));
                            }
                            string ScriptFile = DropTableScript(TablesToDrop);
                            System.IO.File.Copy( ScriptFile, System.IO.Path.Combine( SourceDir, System.IO.Path.GetFileName( ScriptFile )));
                            string DebugFile = System.IO.Path.Combine(SourceDir, "Care_Coordinator_Report.txt");
                            System.IO.File.WriteAllText(DebugFile, "DataExtraction=true();");
                            //script name change PRM-1225
                            string NewDebugFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(DebugFile), "ancillary_script.txt");
                            System.IO.File.Copy(DebugFile, NewDebugFile);
                            return true;
                        }
                    }
                    MapIndex++;
                    MapFile = QualifiedQVWNameToReduce.Replace(".qvw", ".map_" + MapIndex.ToString());
                }
            }
            catch (System.Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Drop Table", ex);
            }
            return false;  //if no tables should be dropped return false - this is the default
        }

        /// <summary>
        /// Script helper functions,  writes the script into a temp file
        /// and returns fully qualfiied path - file must be named "SCRIPT.TXT"
        /// </summary>
        public string DropTableScript(List<string> Tables)
        {
            if ((Tables == null) || (Tables.Count == 0))
            {
                return string.Empty;
            }
            List<string> DropTables = new List<string>();
            foreach (string Table in Tables)
            {
                DropTables.Add("DROP TABLE " + Table + ";");
            }
            DropTables.Add("EXIT SCRIPT;");
            string TempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "script.txt");
            System.IO.File.WriteAllLines(TempFile, DropTables);
            return TempFile;
        }

        private void DoReductionWork(object sender, DoWorkEventArgs e)
        {
            QVWReducer Reducer = e.Argument as QVWReducer;
            if (Reducer != null)
            {
                try
                {
                    e.Result = Reducer.ReduceBlocking();
  
                }
                catch (System.Exception ex)
                {
                    Reducer.TaskStatusMsg = ex.ToString();
                }
            }
            e.Result = false;
        }

        private void DoReductionWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Callback != null)
                Callback(this);
        }


        //public bool ExtractQVWData(string QualifiedQVW, out string XMXLIndex, out List<string> DataFiles)
        //{
        //    //check to see if signed
        //    XMXLIndex = string.Empty;
        //    DataFiles = new List<string>();
        //    return true;
        //}
    }
}
