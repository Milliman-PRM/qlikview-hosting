
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Milliman.Reduction.ReductionEngine
{
    using Milliman.Common;
    using Milliman.Common.TreeUtilities;

    [Flags]
    public enum enumSummary
    {
        QMSConnection = 2,
        GetSourceDocument = 3,
        PrepareFolderReduction = 4,
        CreateAncillaryScript = 8,
        TaskCreateHierarchy = 16,
        TaskRunHierarchy = 32,
        CleanUp = 64,
        TreeInfo = 128,
        TaskCreateReduce = 256,
        TaskRunReduce = 512,
        PrepareFolderHierarchy = 1024
    }

    public class ReductionRunner
    {
        private static string C_LOG = "User Notification", C_CLEAN = "Clean Up", C_WRAP = "Wrap Up";
        private Guid _task_id_hierarchy, _task_id_reduction;
        private Guid _qds_guid;
        private Guid _qvs_guid;
        private QMSConnection _qms_conn;
        private QMSAPI.DocumentFolder _source_document;
        private QMSAPI.IQMS _qms_client;
        private QMSAPI.TaskInfo _task_info_hierarchy;
        StringBuilder _userMessage = new StringBuilder();

        private Dictionary<string, List<Action>> _late_execution = new Dictionary<string, List<Action>>();
        //private List<Action> _cleanUpActions = new List<Action>(), _wrapUpActions = new List<Action>();


        public Milliman.Common.ReduceConfig ConfigFile { get; set; }
        public string QVWOriginalFullFileName { get; set; }
        public string QVWHierarchyFQFileNameGUID { get; private set; }
        public string QVWHierarchyFQFolderNameGUID { get; set; }
        public string QVWReductionFQFileNameGUID { get; private set; }
        public string QVWReductionFQFolderNameGUID { get; set; }
        public string SourceDocumentWorkingPath { get; set; }
        public QMSSettings QVConnectionSettings { get; set; }
        public List<string>[] MasterCandidateFiles { get; set; }

        public ReductionRunner()
        {
            this.QVConnectionSettings = new QMSSettings();
        }
        public ReductionRunner(QMSSettings settings) : this()
        {
            this.QVConnectionSettings = settings;
        }

        public void Run()
        {
            try
            {
                enumSummary summary = 0;
                while (true)
                {
                    // Connects to Qlikview server
                    if (((summary |= connectToQMS(this.QVConnectionSettings)) & enumSummary.QMSConnection) == 0) break;
                    // Get the source document folder on the QVS. This will be used in all our future iterations
                    if (((summary |= getSourceDocumentFolder(out _source_document)) & enumSummary.GetSourceDocument) == 0) break;

                    // Now things start to get tricky. We know we'll need at least ONE task, so we start handling it here. 
                    _task_id_hierarchy = Guid.NewGuid();
                    string t1, t2;
                    if (((summary |= prepareFileStructureForHierarchy(this.QVWOriginalFullFileName, this.SourceDocumentWorkingPath, _task_id_hierarchy, out t1, out t2)) & enumSummary.PrepareFolderHierarchy) == 0) break;
                    QVWHierarchyFQFolderNameGUID = t1; QVWHierarchyFQFileNameGUID = t2;
                    if (((summary |= createAncillaryScript(this.QVWHierarchyFQFolderNameGUID)) & enumSummary.CreateAncillaryScript) == 0) break;
                    if (((summary |= createHierarchyTask(this.QVWHierarchyFQFileNameGUID, _source_document, out _task_info_hierarchy)) & enumSummary.TaskCreateHierarchy) == 0) break;
                    if (((summary |= runHierarchyTask(_task_info_hierarchy)) & enumSummary.TaskRunHierarchy) == 0) break;

                    string master_qvw_working_path = this.QVWHierarchyFQFolderNameGUID;

                    foreach (var set in this.ConfigFile.SelectionSets)
                    {
                        try
                        {
                            if (set == null) continue;
                            this.generateTreeInfo(set, this.QVWHierarchyFQFolderNameGUID);
                            if (string.IsNullOrEmpty(set.ReducedQVWName) || set.ReducedQVWName == "null") continue;
                            this.prepareFileStructureForReduction(this.QVWOriginalFullFileName, this.SourceDocumentWorkingPath);
                            QMSAPI.DocumentTask reduction_document_task = this.createReductionTask(this.SourceDocumentWorkingPath,
                                set.ReducedQVWName,
                                set.DropDisassoicatedDataModelTables,
                                set.SelectionCriteria, set.DropTables);
                            this.runReductionTask(reduction_document_task, true);

                            if ((set.UniqueValuesFromMasterQVWColumns != null) && (set.UniqueValuesFromMasterQVWColumns.Count > 0))
                                this.generateUniqueValuesForQVW(this.QVWHierarchyFQFolderNameGUID, set.UniqueValuesFromMasterQVWColumns, set, true);
                            if ((set.UniqueValuesFromReducedQVWColumns != null) && (set.UniqueValuesFromReducedQVWColumns.Count > 0))
                            {
                                _task_id_hierarchy = Guid.NewGuid();
                                string FQReducedQVWFileName = Path.Combine(this.QVWReductionFQFolderNameGUID, Path.GetFileName(set.ReducedQVWName) + ".qvw");
                                string FQNewFolderNameGUID, FQNewFileNameGUID;
                                QMSAPI.TaskInfo taskInfo = null;
                                this.prepareFileStructureForHierarchy(FQReducedQVWFileName, SourceDocumentWorkingPath, _task_id_hierarchy, out FQNewFolderNameGUID, out FQNewFileNameGUID);
                                createAncillaryScript(FQNewFolderNameGUID);
                                createHierarchyTask(FQNewFileNameGUID, _source_document, out taskInfo);
                                runHierarchyTask(taskInfo);

                                this.generateUniqueValuesForQVW(FQNewFolderNameGUID, set.UniqueValuesFromReducedQVWColumns, set);
                            }

                            flushLogMessages(set, true, null);
                        }
                        catch (Exception ex)
                        {
                            flushLogMessages(set, false, ex.Message);
                        }

                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);  //catch and log the exception
            }

            this.ExecuteActions(C_WRAP);
            this.ExecuteActions(C_CLEAN);
            this.ExecuteActions(C_LOG);

            createCompleteFlags(this.ConfigFile.ConfigFileNameWithoutExtension);
        }

        /// <summary>
        /// Create a "GUID_completed.txt" file for each completed task
        /// Check to see if I am the last task in this group to execute, if so write out a "processing_complete.txt" file
        /// </summary>
        /// <param name="ConfigurationName"></param>
        private void createCompleteFlags(string ConfigurationName)
        {
            string complete_process_stream = Path.Combine(ConfigFile.ConfigFQFolderName, ConfigurationName + "_completed.txt");
            using (StreamWriter complete_process_writer = new StreamWriter(complete_process_stream))
            {
                complete_process_writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"));
                complete_process_writer.Flush();
            }

            //Note: this code is experimental and does not always give the intended results.....
            //look to see if all the other tasks have completed, if so I am the last one, so write out the processing_complete.txt semaphore, all tasks are done
            string[] ConfigurationFiles = Directory.GetFiles(ConfigFile.ConfigFQFolderName, "*.config", SearchOption.TopDirectoryOnly);
            string[] CompletedFiles = Directory.GetFiles(ConfigFile.ConfigFQFolderName, "*_completed.txt", SearchOption.TopDirectoryOnly);
            if (ConfigurationFiles.Length == CompletedFiles.Length)
            {  //each config file has a completed file, thus we must be last task executing
                string all_done_stream = Path.Combine(ConfigFile.ConfigFQFolderName, "processing_complete.txt");
                using (StreamWriter all_done_writer = new StreamWriter(all_done_stream))
                {
                    all_done_writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"));
                    all_done_writer.Flush();
                }
            }
        }

        private void ExecuteActions(string category)
        {
            if (!_late_execution.ContainsKey(category)) return;
            foreach (var action in _late_execution[category])
            {
                try
                {
                    Trace.WriteLine(string.Format("Executing late action for '{0}':\r\n{1}", category, action.Method.ToString()));
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("Fail to run late action '{0}':\r\nERROR: {2}", category, ex.Message));
                }
            }
            _late_execution[category].Clear();
        }

        private void flushLogMessages(ReductionSettings set, bool success, string errorMessage)
        {
            string master_log = Path.Combine(ConfigFile.ConfigFQFolderName, ConfigFile.MasterStatusLog);
            System.Diagnostics.Debug.WriteLine("master->" + ConfigFile.MasterStatusLog);
            System.Diagnostics.Debug.WriteLine("task->" + set.QVWStatusLog);
            if (!File.Exists(master_log))
            {
                using (File.Create(master_log)) { }
            }

            using (StreamWriter master_log_writer = File.AppendText(master_log))
            {
                master_log_writer.WriteLine(string.Format("{0}|{1}{2}",
                    ConfigFile.ConfigFileName,
                    success ? "SUCCESS" : "FAIL",
                    success ? "" : "|" + errorMessage));
                master_log_writer.Flush();
            }

            //VWN:check to see if there is a QVWStatusLog, when processing the master QVW to generate just hierarchies there may
            //not be a QVWStausLog - not an error condition
            if (!string.IsNullOrEmpty(set.QVWStatusLog))
            {
                string status_log = Path.Combine(ConfigFile.ConfigFQFolderName, set.QVWStatusLog);
                using (StreamWriter status_log_writer = File.AppendText(status_log))
                {
                    status_log_writer.WriteLine(string.Format("{0}", success ? "SUCCESS" : "FAIL"));
                    status_log_writer.Flush();
                }
            }
        }

        private QMSAPI.DocumentNode GetNode(string qvwName)
        {
            QMSAPI.DocumentNode node = null;
            foreach (var doc_node in _qms_client.GetSourceDocuments(this._qds_guid))
            {
                if (doc_node.Name == qvwName)
                {
                    node = doc_node;
                    break;
                }
            }
            if (node == null)
            {
                Trace.WriteLine(string.Format("Could not find node '{0}' on Publisher's source documents. The process will be terminated ...", qvwName));
            }
            else
            {
                Trace.WriteLine(string.Format("Successfully fetched SourceDocument '{0}' from Qlikview Publisher", qvwName));
            }
            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        private void generateUniqueValuesForQVW(string sourceFolderNameGUID, List<NVPair> uniqueColumns, ReductionSettings set, bool isMaster = false)
        {
            TreeBuilderProcessor processor = new TreeBuilderProcessor();
            List<string> ColumnNames = new List<string>();
            foreach (NVPair Pair in uniqueColumns)
                ColumnNames.Add(Pair.FieldName);
            List<string> candidate_files = processor.FindCandidateFiles(sourceFolderNameGUID, ColumnNames);

            //VWN
            System.Diagnostics.Debug.WriteLine("Generating unique values - found " + candidate_files.Count.ToString() + " files to process");

            foreach (var candidate in candidate_files)
            {
                if (candidate_files == null) continue;
                // Figure out if the current file has any of the columns we want
                string[] columns = getColumnNamesFromFile(candidate);
                foreach (var pair in uniqueColumns)
                {
                    for (int i = 0; i < columns.Length; i++)
                        if (columns[i] == pair.FieldName)
                        {
                            generateUniqueValuesFile(candidate, i, Path.Combine(Path.GetDirectoryName(this.QVWOriginalFullFileName), Path.GetFileName(pair.Value)));
                            break;
                        }
                }
            }
        }

        private void generateUniqueValuesFile(string sourceFile, int index, string destinationFile)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(sourceFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] columns = line.Split('|');
                    if (columns.Length <= index) continue;
                    values[columns[index].ToLower()] = columns[index];
                }
            }
            using (StreamWriter writer = new StreamWriter(destinationFile))
            {
                foreach (var key in values.Keys)
                    writer.WriteLine(values[key].Trim(new char[] { '"' }));
            }
        }

        private string[] getColumnNamesFromFile(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                return ("" + reader.ReadLine()).Split('|');
            }
        }

        private enumSummary connectToQMS(QMSSettings qmsSettings)
        {
            if (qmsSettings == null)
            {
                Trace.WriteLine("Unable to establish connection to QMS: QMS Settings is null");
                return 0;
            }
            else if (string.IsNullOrEmpty((qmsSettings.QMSURL + "").Trim()))
            {
                Trace.WriteLine("Unable to establish connection to QMS: the QMS address is empty");
                return 0;
            }
            try
            {
                Trace.WriteLine(string.Format("Connecting to QMS on '{0}'", qmsSettings.QMSURL));
                _qms_conn = new QMSConnection();
                if (string.IsNullOrEmpty(qmsSettings.UserName))
                    _qms_conn.Connect(qmsSettings.QMSURL);
                else
                    _qms_conn.Connect(qmsSettings.QMSURL, qmsSettings.UserName, qmsSettings.GetPassword());
                _qms_client = _qms_conn.QlikClient;
                Trace.WriteLine("Connection successfully established...");
                return enumSummary.QMSConnection;
            }
            catch (Exception ex)
            {
                LogToUser("Error attempting to connect to Qlikview Server: {0}", ex.Message);
                Trace.WriteLine(string.Format("An error occurred when attempting to connect to QMS {0}", ex));
                return 0;
            }
        }

        internal enumSummary getSourceDocumentFolder(out QMSAPI.DocumentFolder qmsFolder)
        {
            qmsFolder = null;
            try
            {
                Trace.WriteLine("Attempting to fetch Publisher Service ID");
                var services = _qms_client.GetServices(QMSAPI.ServiceTypes.QlikViewDistributionService);
                if (services == null || services.Length == 0)
                {
                    Trace.WriteLine("QMS did not return any Publisher services. Current reduction will be terminated");
                    return 0;
                }
                else
                {
                    _qds_guid = services[0].ID;
                    Trace.WriteLine(string.Format("Found {0} Publisher. QDS ID used is '{1}'", services.Length, _qds_guid.ToString("N").ToUpper()));
                }

                services = _qms_client.GetServices(QMSAPI.ServiceTypes.QlikViewServer);
                if (services == null || services.Length == 0)
                {
                    Trace.WriteLine("QMS did not return any QVS services. Reduction will be continue to run but no statistics will be available for the server or cluster");
                }
                else
                {
                    _qvs_guid = services[0].ID;
                    Trace.WriteLine(string.Format("Found {0} QlikView Server. QMS ID used is '{1}'", services.Length, _qvs_guid.ToString("N").ToUpper()));
                }


                Trace.WriteLine("Publisher service successfully found.");
                Trace.WriteLine(string.Format("Working with publisher service '{0}', of id '{1}'", services[0].Name, services[0].ID));

                Trace.WriteLine("Attempting to fetch SourceDocuments path");
                QMSAPI.DocumentFolder[] paths = _qms_client.GetSourceDocumentFolders(_qds_guid, QMSAPI.DocumentFolderScope.All);
                if (paths == null || paths.Length == 0)
                {
                    Trace.WriteLine("Could not get any SourceDocument from the server. Current reduction will be terminated");
                    return 0;
                }
                else if (paths.Length > 1)
                {
                    Trace.WriteLine("Multiple paths were found on the Publisher. Searching for a Milliman path");
                    foreach (var p in paths)
                    {
                        Trace.WriteLine(string.Format("Searching for 'Milliman' string in '{0}' path...", p.General.Path));
                        if (p.General.Path.ToLower().IndexOf("milliman") > 0)
                        {
                            Trace.WriteLine(string.Format("Found a matching path for Milliman: {0}", p.General.Path));
                            qmsFolder = p;
                            break;
                        }
                    }
                    if (qmsFolder == null)
                    {
                        Trace.WriteLine(string.Format("No matching folders were found for Milliman. Falling back to first path found: '{0}'", paths[0].General.Path));
                        qmsFolder = paths[0];
                    }
                }
                else if (paths.Length == 1)
                {
                    Trace.WriteLine("Only one path found on the Publisher. Setting up working path...");
                    qmsFolder = paths[0];
                }
                this.SourceDocumentWorkingPath = qmsFolder.General.Path;
                Trace.WriteLine(string.Format("Working path set to '{0}'", qmsFolder.General.Path));
                return enumSummary.GetSourceDocument;
            }
            catch (Exception ex)
            {
                LogToUser("Error attempting to get SourceDocuments folder from Qlikview Server: {0}", ex.Message);
                Trace.WriteLine(string.Format("Unable to fetch DocumentFolder ID {0}", ex));
                return 0;
            }
        }

        /// <summary>
        /// Creates a folder for the reduction work and copies the original master qvw to that folder
        /// </summary>
        /// <param name="sourceFileName">The original qvw file to be reduced</param>
        /// <param name="destinationFolderName">The Qlikview server's self reported SourceDocuments folder.</param>
        /// <param name="taskId">A GUID that identifies the selectionset being processed</param>
        /// <param name="QVWFolderNameGUID"></param>
        /// <param name="QVWFileNameGUID"></param>
        /// <returns></returns>
        private enumSummary prepareFileStructureForHierarchy(string sourceFileName, string destinationFolderName, Guid taskId, out string QVWFolderNameGUID, out string QVWFileNameGUID)
        {
            QVWFolderNameGUID = string.Empty;
            QVWFileNameGUID = string.Empty;
            try
            {
                // Create the output folder as a subfolder of the QMS SourceDocuments folder
                QVWFolderNameGUID = Path.Combine(destinationFolderName, taskId.ToString("N"));
                Trace.WriteLine("Attempting to create destination folder");
                Directory.CreateDirectory(QVWFolderNameGUID);
                Trace.WriteLine(string.Format("Destination folder '{0}' created successfully...", QVWFolderNameGUID));

                QVWFileNameGUID = Path.Combine(QVWFolderNameGUID, taskId.ToString("N") + ".qvw");
                Trace.WriteLine(string.Format("Copying local QVW File {0} to Qlikview Server file {1}", sourceFileName, QVWFileNameGUID));
                File.Copy(sourceFileName, QVWFileNameGUID, true);
                return enumSummary.PrepareFolderHierarchy;
            }
            catch (Exception ex)
            {
                LogToUser("Unable to prepare Structure to Process Hierarchy files: {0}", ex.Message);
                Trace.WriteLine(string.Format("Unable copy local file to QlikView server {0}", ex));
                return 0;
            }

        }

        /// <summary>
        /// Create ancellary script and copy it to legacy script file
        /// </summary>
        /// <param name="destinationFolder"></param>
        /// <returns></returns>
        private enumSummary createAncillaryScript(string destinationFolder)
        {
            try
            {
                Trace.WriteLine("Creating 'ancillary script' on destination folder");
                string ancillary_full_file_path = Path.Combine(destinationFolder, "ancillary_script.txt");
                using (FileStream ancillary_stream = File.Create(ancillary_full_file_path))
                {
                    using (StreamWriter ancillary_writer = new StreamWriter(ancillary_stream))
                    {
                        ancillary_writer.WriteLine("LET DataExtraction=true();");
                        ancillary_writer.Flush();
                    }
                }
                Trace.WriteLine(string.Format("Ancillary script file created: '{0}'", ancillary_full_file_path));

                Trace.WriteLine("Creating legacy script file");
                string legacy_full_file_path = Path.Combine(destinationFolder, "care_coordinator_report.txt"); ;
                File.Copy(ancillary_full_file_path, legacy_full_file_path);
                Trace.WriteLine(string.Format("Ancillary script file copied to legacy script file: '{0}'", legacy_full_file_path));

                return enumSummary.CreateAncillaryScript;
            }
            catch (Exception ex)
            {
                LogToUser("Unable to create 'ancillary script' (or legacy script) for processing:\r\n{0}", ex.Message);
                Trace.WriteLine(string.Format("Unable to create 'ancillary script' (or legacy script)\r\n{0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qvwDocumentPath">The full path of the reduced qvw that this task is to create</param>
        /// <param name="qmsFolder"></param>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        private enumSummary createHierarchyTask(string qvwDocumentPath, QMSAPI.DocumentFolder qmsFolder, out QMSAPI.TaskInfo taskInfo)
        {
            taskInfo = null;
            try
            {
                string qvw_file_name = Path.GetFileName(qvwDocumentPath);
                Trace.WriteLine(string.Format("Creating task for Qlikview Document '{0}'", qvwDocumentPath));
                _qms_client.ClearQVSCache(QMSAPI.QVSCacheObjects.All);
                Trace.WriteLine("QVSCache successfully cleared");
                QMSAPI.DocumentNode node = null;
                Trace.WriteLine("Searching for correct document node on Publisher's Source Documents");
                foreach (var doc_node in _qms_client.GetSourceDocuments(this._qds_guid))
                {
                    if (doc_node.Name == qvw_file_name)
                    {
                        node = doc_node;
                        break;
                    }
                }
                if (node == null)
                {
                    Trace.WriteLine(string.Format("Could not find node '{0}' on Publisher's source documents. The process will be terminated ...", qvw_file_name));
                    return 0;
                }
                else
                {
                    Trace.WriteLine(string.Format("Successfully fetched SourceDocument '{0}' from Qlikview Publisher", qvw_file_name));
                }

                QMSAPI.DocumentTask temp_task = QMSWrapper.GetTask();
                Trace.WriteLine("Setting up task properties...");

                #region General Tab
                Trace.WriteLine("Configuring General Tab");
                temp_task.ID = this._task_id_hierarchy;
                temp_task.QDSID = this._qds_guid;
                temp_task.Document = node;
                temp_task.General = new QMSAPI.DocumentTask.TaskGeneral();
                temp_task.General.Enabled = true;
                temp_task.General.TaskName = "Reduction | Partial Reload | " + qvw_file_name.ToUpper();
                temp_task.General.TaskDescription = string.Format("Automatically generated by ReductionService on {0}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                #endregion

                #region Reload Tab
                Trace.WriteLine("Configuring Reload Tab");
                temp_task.Scope |= QMSAPI.DocumentTaskScope.Reload;
                temp_task.Reload = new QMSAPI.DocumentTask.TaskReload();
                temp_task.Reload.Mode = QMSAPI.TaskReloadMode.Partial;
                #endregion

                #region Reduce Tab
                Trace.WriteLine("Configuring Reduce Tab...");
                temp_task.Scope |= QMSAPI.DocumentTaskScope.Reduce;
                temp_task.Reduce = new QMSAPI.DocumentTask.TaskReduce();
                temp_task.Reduce.DocumentNameTemplate = string.Empty;
                temp_task.Reduce.Static = new QMSAPI.DocumentTask.TaskReduce.TaskReduceStatic();
                temp_task.Reduce.Static.Reductions = new QMSAPI.TaskReduction[] { new QMSAPI.TaskReduction() };
                temp_task.Reduce.Static.Reductions[0].Type = new QMSAPI.TaskReductionType();
                temp_task.Reduce.Dynamic = new QMSAPI.DocumentTask.TaskReduce.TaskReduceDynamic();
                temp_task.Reduce.Dynamic.Type = new QMSAPI.TaskReductionType();
                #endregion

                #region Distribution Tab // we don't really need it at the time.
                //Trace.WriteLine("Configuring Distribution Tab...");
                //temp_task.Scope |= QMSAPI.DocumentTaskScope.Distribute;
                //temp_task.Distribute = new QMSAPI.DocumentTask.TaskDistribute();
                //temp_task.Distribute.Output = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeOutput();
                //temp_task.Distribute.Output.Type = QMSAPI.TaskDistributionOutputType.QlikViewDocument;

                //temp_task.Distribute.Static = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeStatic();
                //temp_task.Distribute.Static.DistributionEntries = new QMSAPI.TaskDistributionEntry[] { new QMSAPI.TaskDistributionEntry() };
                //temp_task.Distribute.Static.DistributionEntries[0].Recipients = new QMSAPI.DirectoryServiceObject[] { new QMSAPI.DirectoryServiceObject() };
                //temp_task.Distribute.Static.DistributionEntries[0].Recipients[0].Type = QMSAPI.DirectoryServiceObjectType.Authenticated;
                //temp_task.Distribute.Static.DistributionEntries[0].Destination = new QMSAPI.TaskDistributionDestination();
                //temp_task.Distribute.Static.DistributionEntries[0].Destination.Folder = new QMSAPI.TaskDistributionDestination.TaskDistributionDestinationFolder();
                //temp_task.Distribute.Static.DistributionEntries[0].Destination.Type = QMSAPI.TaskDistributionDestinationType.None;
                //temp_task.Distribute.Static.DistributionEntries[0].Destination.Folder.Name = "";

                //temp_task.Distribute.Notification = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeNotification();
                //temp_task.Distribute.Notification.SendNotificationEmail = false;

                //temp_task.Distribute.Dynamic = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeDynamic();
                //temp_task.Distribute.Dynamic.IdentityType = QMSAPI.UserIdentityValueType.DisplayName;
                //temp_task.Distribute.Dynamic.Destinations = new QMSAPI.TaskDistributionDestination[] { new QMSAPI.TaskDistributionDestination() };
                #endregion

                #region Trigger Tab
                Trace.WriteLine("Configuring task trigger ...");
                temp_task.Scope |= QMSAPI.DocumentTaskScope.Triggering;
                temp_task.Triggering = new QMSAPI.DocumentTask.TaskTriggering();
                temp_task.Triggering.ExecutionAttempts = 1;
                temp_task.Triggering.ExecutionTimeout = 1440;
                temp_task.Triggering.TaskDependencies = new QMSAPI.TaskInfo[] { };
                temp_task.Triggering.Triggers = new QMSAPI.ScheduleTrigger[] {
                    new QMSAPI.ScheduleTrigger()
                };
                temp_task.Triggering.Triggers[0].Enabled = true;
                temp_task.Triggering.Triggers[0].Type = QMSAPI.TaskTriggerType.OnceTrigger;
                ((QMSAPI.ScheduleTrigger)temp_task.Triggering.Triggers[0]).StartAt = DateTime.Now;
                #endregion

                Trace.WriteLine("Assynchronously saving task...");
                _qms_client.SaveDocumentTask(temp_task);
                do
                {
                    System.Threading.Thread.Sleep(500);
                    taskInfo = _qms_client.FindTask(_qds_guid, QMSAPI.TaskType.DocumentTask, temp_task.General.TaskName);
                } while (taskInfo == null);

                Trace.WriteLine(string.Format("Task of ID '{0}' successfully saved", temp_task.ID.ToString("N")));
                return enumSummary.TaskCreateHierarchy;
            }
            catch (Exception ex)
            {
                LogToUser("Error when trying to setup task on Qlikview Server: {0}", ex.Message);
                Trace.WriteLine(string.Format("An error occurred while trying to setup and execute the Task on the Publisher server. The process will be terminated... {0}", ex));
                return 0;
            }
        }

        private enumSummary runHierarchyTask(QMSAPI.TaskInfo task)
        {
            try
            {
                DateTime start_time = DateTime.Now;
                this.RunTaskSynchronously(task.ID);
                this.AddCleanUpAction(new Action(() => {
                    Trace.WriteLine(string.Format("Deleting task '{0}'", task.ID.ToString("N")));
                    _qms_client.DeleteTask(task.ID, QMSAPI.TaskType.DocumentTask);
                }));
                DateTime end_time = DateTime.Now;
                LogToUser("Hierarchy Task successfully ran on Qlikview Server");
                return enumSummary.TaskRunHierarchy;
            }
            catch (Exception ex)
            {
                LogToUser("Error when trying to run Hierarchy task on Qlikview Server: {0}", ex.Message);
                Trace.WriteLine(string.Format("An error occurred when trying to run the Hierarchy task... {0}", ex));
                return 0;
            }
        }

        private enumSummary generateTreeInfo(ReductionSettings selectionSet, string workingFolder)
        {
            try
            {
                List<string> list_hierarchy, list_concept, list_drop, list_model;
                MasterCandidateFiles = new List<string>[4];
                TreeBuilderProcessor tree_processor = new TreeBuilderProcessor();
                Trace.WriteLine("Beginning TreeBuild processing...");
                if (!tree_processor.BuildTree(workingFolder, out list_hierarchy, out list_concept, out list_drop, out list_model))
                {
                    Trace.WriteLine("Unable to process ");
                    return 0;
                }
                MasterCandidateFiles[0] = list_hierarchy;
                MasterCandidateFiles[1] = list_concept;
                MasterCandidateFiles[2] = list_drop;
                MasterCandidateFiles[3] = list_model;

                string destination_folder = Path.GetDirectoryName(this.QVWOriginalFullFileName);
                string source_folder = this.QVWHierarchyFQFolderNameGUID, file_name = string.Empty;
                string tree_file_folder = Path.GetTempPath();

                if (!string.IsNullOrEmpty(selectionSet.HierarchyFile))
                {
                    Trace.WriteLine("Adding backlog file move for HierarchyFile");
                    this.AddWrapUpAction(Path.Combine(tree_file_folder, list_hierarchy[0]), Path.Combine(destination_folder, Path.GetFileName(selectionSet.HierarchyFile)), false);
                    this.AddCleanUpAction(Path.Combine(tree_file_folder, list_hierarchy[0]), "Hierarchy file for " + Path.GetDirectoryName(workingFolder));
                }
                if (!string.IsNullOrEmpty(selectionSet.ConceptFile))
                {
                    Trace.WriteLine("Adding backlog file move for ConceptFile");
                    this.AddWrapUpAction(Path.Combine(tree_file_folder, list_concept[0]), Path.Combine(destination_folder, Path.GetFileName(selectionSet.ConceptFile)), false);
                    this.AddCleanUpAction(Path.Combine(tree_file_folder, list_concept[0]), "Concept file for " + Path.GetDirectoryName(workingFolder));
                }

                if (!string.IsNullOrEmpty(selectionSet.MapFile))
                {
                    Trace.WriteLine("Adding backlog file move for MapFile");
                    this.AddWrapUpAction(Path.Combine(tree_file_folder, list_drop[0]), Path.Combine(destination_folder, Path.GetFileName(selectionSet.MapFile)), false);
                    this.AddCleanUpAction(Path.Combine(tree_file_folder, list_drop[0]), "Drop file for " + Path.GetDirectoryName(workingFolder));
                }

                if (!string.IsNullOrEmpty(selectionSet.DataModelFile))
                {
                    Trace.WriteLine("Adding backlog file move for DataModelFile");
                    this.AddWrapUpAction(Path.Combine(tree_file_folder, list_model[0]), Path.Combine(destination_folder, Path.GetFileName(selectionSet.DataModelFile)), false);
                    this.AddCleanUpAction(Path.Combine(tree_file_folder, list_model[0]), "Model file for " + Path.GetDirectoryName(workingFolder));
                }

                LogToUser("Generate TreeInfo successfully generated intermediate data files");
                return enumSummary.TreeInfo;
            }
            catch (Exception ex)
            {
                LogToUser("Unable to Generate Tree Info: {0}", ex.Message);
                Trace.WriteLine(string.Format("Unable to generate tree files {0}", ex));
                return 0;
            }
        }

        private enumSummary prepareFileStructureForReduction(string sourceFileName, string destinationFolderName)
        {
            try
            {
                _task_id_reduction = Guid.NewGuid();
                this.QVWReductionFQFolderNameGUID = Path.Combine(destinationFolderName, _task_id_reduction.ToString("N"));
                Trace.WriteLine(string.Format("Attempting to create destination folder '{0}'", this.QVWReductionFQFolderNameGUID));
                Directory.CreateDirectory(this.QVWReductionFQFolderNameGUID);
                Trace.WriteLine(string.Format("Destination folder '{0}' created successfully...", this.QVWReductionFQFolderNameGUID));

                this.QVWReductionFQFileNameGUID = Path.Combine(this.QVWReductionFQFolderNameGUID, _task_id_reduction.ToString("N") + ".qvw");
                Trace.WriteLine("Copying local QVW File into the Qlikview Server...");
                Trace.WriteLine(string.Format("Source path: '{0}'", sourceFileName));
                Trace.WriteLine(string.Format("Destination path: '{0}'", this.QVWReductionFQFileNameGUID));
                File.Copy(sourceFileName, this.QVWReductionFQFileNameGUID, true);
                Trace.WriteLine("File successfully copied");
                LogToUser("Successfully prepared folders and tasks to run the Reduction...");
                return enumSummary.PrepareFolderReduction;
            }
            catch (Exception ex)
            {
                LogToUser("Failed to prepare reduction folder and tasks for file '{0}': {1}", sourceFileName, ex.Message);
                Trace.WriteLine(string.Format("Unable copy local file to QlikView server {0}", ex));
                return 0;
            }
        }

        private enumSummary prepareFileStructureForReduction(string sourceFileName, string destinationFolderName, Guid taskId)
        {
            try
            {
                _task_id_reduction = Guid.NewGuid();
                this.QVWReductionFQFolderNameGUID = Path.Combine(destinationFolderName, _task_id_reduction.ToString("N"));
                Trace.WriteLine(string.Format("Attempting to create destination folder '{0}'", this.QVWReductionFQFolderNameGUID));
                Directory.CreateDirectory(this.QVWReductionFQFolderNameGUID);
                Trace.WriteLine(string.Format("Destination folder '{0}' created successfully...", this.QVWReductionFQFolderNameGUID));

                this.QVWReductionFQFileNameGUID = Path.Combine(this.QVWReductionFQFolderNameGUID, _task_id_reduction.ToString("N") + ".qvw");
                Trace.WriteLine("Copying local QVW File into the Qlikview Server...");
                Trace.WriteLine(string.Format("Source path: '{0}'", sourceFileName));
                Trace.WriteLine(string.Format("Destination path: '{0}'", this.QVWReductionFQFileNameGUID));
                File.Copy(sourceFileName, this.QVWReductionFQFileNameGUID, true);
                Trace.WriteLine("File successfully copied");
                LogToUser("Successfully prepared folders and tasks to run the Reduction...");
                return enumSummary.PrepareFolderReduction;
            }
            catch (Exception ex)
            {
                LogToUser("Failed to prepare reduction folder and tasks for file '{0}': {1}", sourceFileName, ex.Message);
                Trace.WriteLine(string.Format("Unable copy local file to QlikView server {0}", ex));
                return 0;
            }
        }

        private QMSAPI.DocumentTask createReductionTask(string destinationFolderName, string reducedQVWFileName, bool shouldDropDisassociatedTables, List<NVPair> selectionCriteria, List<string> tablesToDrop)
        {

            try
            {
                DateTime start;
                string task_id_string = _task_id_reduction.ToString("N");
                string task_folder = Path.Combine(destinationFolderName, task_id_string);
                string reduced_qvw_full_file_name = Path.Combine(task_folder, reducedQVWFileName);
                string task_full_file_name = Path.Combine(destinationFolderName, task_id_string, task_id_string + ".qvw");
                string task_file_name = task_id_string + ".qvw";
                Trace.WriteLine("Processing Drop Tables...");
                start = DateTime.Now;
                bool drop_required = ReductionHelper.dropTableProcessor(task_folder,
                    shouldDropDisassociatedTables,
                    this.ConfigFile.MasterQVW,
                    selectionCriteria,
                    tablesToDrop);
                System.Threading.Thread.Sleep(2000);

                //delete any previous reduced files
                //System.IO.File.Delete(ReducedQVWName);

                QMSAPI.DocumentNode node = null;
                Trace.WriteLine("Searching for correct document node on Publisher's Source Documents");
                foreach (var doc_node in _qms_client.GetSourceDocuments(this._qds_guid))
                {
                    if (doc_node.Name == task_file_name)
                    {
                        node = doc_node;
                        break;
                    }
                }
                if (node == null)
                {
                    Trace.WriteLine(string.Format("Could not find node '{0}' on Publisher's source documents. The process will be terminated ...", task_file_name));
                    return null;
                }
                else
                {
                    Trace.WriteLine(string.Format("Successfully fetched SourceDocument '{0}' from Qlikview Publisher", task_file_name));
                }
                Trace.WriteLine("Setting up task properties...");
                Trace.WriteLine("Configuring General Tab");
                QMSAPI.DocumentTask temp_task = QMSWrapper.GetTask();

                //Creates the Reduction Task
                QMSAPI.DocumentTask task_reduction = new QMSAPI.DocumentTask();
                task_reduction.ID = _task_id_reduction;
                task_reduction.QDSID = _qds_guid;
                task_reduction.Document = node;

                #region general
                task_reduction.Scope = QMSAPI.DocumentTaskScope.General;
                task_reduction.General = new QMSAPI.DocumentTask.TaskGeneral();
                task_reduction.General.Enabled = true;
                task_reduction.General.TaskName = "Reload & Reduce " + task_file_name;
                task_reduction.General.TaskDescription = "Performs reduction on " + task_file_name;
                #endregion

                #region reduce
                task_reduction.Scope |= QMSAPI.DocumentTaskScope.Reduce;
                task_reduction.Reduce = new QMSAPI.DocumentTask.TaskReduce();
                task_reduction.Reduce.DocumentNameTemplate = Path.GetFileNameWithoutExtension(reduced_qvw_full_file_name);
                task_reduction.Reduce.Static = new QMSAPI.DocumentTask.TaskReduce.TaskReduceStatic();
                task_reduction.Reduce.Static.Reductions = new QMSAPI.TaskReduction[selectionCriteria.Count];
                int Index = 0;
                foreach (var selection in selectionCriteria)
                {
                    Trace.WriteLine(string.Format("For task {0}, selection key: {1} with value {2}", task_id_string, selection.FieldName, selection.Value)); // TODO delete this
                    task_reduction.Reduce.Static.Reductions[Index] = new QMSAPI.TaskReduction();
                    task_reduction.Reduce.Static.Reductions[Index].Type = QMSAPI.TaskReductionType.ByField;
                    task_reduction.Reduce.Static.Reductions[Index].Field = new QMSAPI.TaskReduction.TaskReductionField();
                    task_reduction.Reduce.Static.Reductions[Index].Field.Name = selection.FieldName;
                    task_reduction.Reduce.Static.Reductions[Index].Field.Value = selection.Value;
                    task_reduction.Reduce.Static.Reductions[Index].Field.IsNumeric = selection.IsNumeric;
                    Index++;
                }

                task_reduction.Reduce.Dynamic = new QMSAPI.DocumentTask.TaskReduce.TaskReduceDynamic();
                #endregion

                #region distribute
                task_reduction.Scope |= QMSAPI.DocumentTaskScope.Distribute;
                task_reduction.Distribute = new QMSAPI.DocumentTask.TaskDistribute();
                task_reduction.Distribute.Output = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeOutput();
                task_reduction.Distribute.Output.Type = QMSAPI.TaskDistributionOutputType.QlikViewDocument;

                task_reduction.Distribute.Static = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeStatic();
                task_reduction.Distribute.Static.DistributionEntries = new QMSAPI.TaskDistributionEntry[1];
                task_reduction.Distribute.Static.DistributionEntries[0] = new QMSAPI.TaskDistributionEntry();
                task_reduction.Distribute.Static.DistributionEntries[0].Recipients = new QMSAPI.DirectoryServiceObject[1];
                task_reduction.Distribute.Static.DistributionEntries[0].Recipients[0] = new QMSAPI.DirectoryServiceObject();
                task_reduction.Distribute.Static.DistributionEntries[0].Recipients[0].Type = QMSAPI.DirectoryServiceObjectType.Authenticated;
                task_reduction.Distribute.Static.DistributionEntries[0].Destination = new QMSAPI.TaskDistributionDestination();
                task_reduction.Distribute.Static.DistributionEntries[0].Destination.Folder = new QMSAPI.TaskDistributionDestination.TaskDistributionDestinationFolder();
                task_reduction.Distribute.Static.DistributionEntries[0].Destination.Type = QMSAPI.TaskDistributionDestinationType.Folder;
                task_reduction.Distribute.Static.DistributionEntries[0].Destination.Folder.Name = System.IO.Path.GetDirectoryName(reduced_qvw_full_file_name);

                task_reduction.Distribute.Notification = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeNotification();
                task_reduction.Distribute.Notification.SendNotificationEmail = false;

                task_reduction.Distribute.Dynamic = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeDynamic();
                task_reduction.Distribute.Dynamic.IdentityType = QMSAPI.UserIdentityValueType.DisplayName;
                task_reduction.Distribute.Dynamic.Destinations = new QMSAPI.TaskDistributionDestination[0];
                #endregion

                #region reload
                task_reduction.Scope |= QMSAPI.DocumentTaskScope.Reload;
                task_reduction.Reload = new QMSAPI.DocumentTask.TaskReload();
                if (drop_required)
                    task_reduction.Reload.Mode = QMSAPI.TaskReloadMode.Partial; //this will result in tables being dropped as defined by the drop table processor
                else
                    task_reduction.Reload.Mode = QMSAPI.TaskReloadMode.None;
                #endregion

                #region trigger
                //documentTask.DocumentInfo = new DocumentTask.TaskDocumentInfo();
                task_reduction.Triggering = new QMSAPI.DocumentTask.TaskTriggering();
                task_reduction.Triggering.ExecutionAttempts = 1;
                task_reduction.Triggering.ExecutionTimeout = 1440;
                task_reduction.Triggering.TaskDependencies = new QMSAPI.TaskInfo[] { };

                task_reduction.Triggering.Triggers = new QMSAPI.ScheduleTrigger[1];
                var trigger_schedule = new QMSAPI.ScheduleTrigger();
                trigger_schedule.Enabled = true;
                trigger_schedule.Type = QMSAPI.TaskTriggerType.OnceTrigger;
                trigger_schedule.StartAt = DateTime.Now.AddSeconds(5.0); //don't schedule for now, it ignores if in past
                task_reduction.Triggering.Triggers[0] = trigger_schedule;
                #endregion

                _qms_client.SaveDocumentTask(task_reduction);

                System.Threading.Thread.Sleep(4000);//give it a second


                return task_reduction;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("An error occurred when trying to run the Reduction task... {0}", ex));
                return null;
            }
        }

        private enumSummary runReductionTask(QMSAPI.DocumentTask task, bool cleanUpWhenDone)
        {
            try
            {
                Trace.WriteLine(string.Format("Attempting to run newly created reduction task '{0}' ...", task.ID.ToString("N")));
                DateTime start_time = DateTime.Now;
                /*******************************************************************************************/
                this.RunTaskSynchronously(task.ID);
                /*******************************************************************************************/
                DateTime end_time = DateTime.Now;

                string remote_reduced_qvw_fq_file_name = Path.Combine(task.Distribute.Static.DistributionEntries[0].Destination.Folder.Name, task.Reduce.DocumentNameTemplate + ".qvw");
                string local_reduced_qvw_fq_file_name = Path.Combine(Path.GetDirectoryName(this.QVWOriginalFullFileName), Path.GetFileName(remote_reduced_qvw_fq_file_name));

                if (File.Exists(local_reduced_qvw_fq_file_name))
                {
                    Trace.WriteLine(string.Format("File '{0}' already exists so it'll be deleted before a copy is attempted...", local_reduced_qvw_fq_file_name));
                    File.Delete(local_reduced_qvw_fq_file_name);
                }
                Trace.WriteLine(string.Format("Copying reduced qvw from '{0}' back into the source folder '{1}'", remote_reduced_qvw_fq_file_name, local_reduced_qvw_fq_file_name));
                this.AddWrapUpAction(remote_reduced_qvw_fq_file_name, local_reduced_qvw_fq_file_name, false);

                //this.AddCleanUpAction(new Action(() => {
                //    Trace.WriteLine(string.Format("Deleting task '{0}'", task.ID.ToString("N")));
                //    _qms_client.DeleteTask(task.ID, QMSAPI.TaskType.DocumentTask);
                //    Trace.WriteLine(string.Format("Deleting original file '{0}'", task.General));
                //    Directory.Delete(Path.GetDirectoryName(remote_reduced_qvw_fq_file_name), true);
                //}));
                LogToUser("Successfully ran reduction task [id: {0}; name: \"{1}\"] for current selection set.", task.ID, task.General.TaskName);
                return enumSummary.TaskRunReduce;
            }
            catch (Exception ex)
            {
                LogToUser("Unable to run Reduction Task [id: {0}; name: {1}]: {2}", task.ID, task.General.TaskName, ex.Message);
                Trace.WriteLine(string.Format("An error occurred when trying to run the Reduction task... {0}", ex));
                return 0;
            }
        }

        /// <summary>
        /// Initiates and monitors the running task, and returns when it's over
        /// </summary>
        /// <param name="taskId"></param>
        private void RunTaskSynchronously(Guid taskId)
        {
            
            QMSAPI.TaskStatus status = null;
            int SecondsCounter = 0;
            string TaskIdStr = taskId.ToString("N");
            const int RunTaskIntervalSeconds = 5;
            const int CheckTaskIntervalSeconds = 2;
            DateTime ParsedTime;

            // Get the task running.  It generally takes more than 1 attempt.  
            while (status == null || status.Extended == null || !DateTime.TryParse(status.Extended.StartTime, out ParsedTime))
            {
                _qms_client.RunTask(taskId);

                System.Threading.Thread.Sleep(RunTaskIntervalSeconds * 1000);
                SecondsCounter += RunTaskIntervalSeconds;

                status = _qms_client.GetTaskStatus(taskId, QMSAPI.TaskStatusScope.All);
                Trace.WriteLine(string.Format("Task ID {0} has status {1} after _qms_client.RunTask(), for {2} seconds", TaskIdStr, status.General.Status.ToString(), SecondsCounter));
            }

            SecondsCounter = 0;

            // loop until task is completed.  Most reliable signal of completion is FinishedTime
            while (string.IsNullOrEmpty(status.Extended.FinishedTime) || !DateTime.TryParse(status.Extended.FinishedTime, out ParsedTime))
            {
                Trace.WriteLine(string.Format("Task ID {0} running {1} seconds, status {2}, FinishedTime {3}", TaskIdStr, SecondsCounter, status.General.Status.ToString(), ParsedTime == DateTime.MinValue ? "<none>" : ParsedTime.ToString()));

                System.Threading.Thread.Sleep(CheckTaskIntervalSeconds * 1000);
                SecondsCounter += CheckTaskIntervalSeconds;

                status = _qms_client.GetTaskStatus(taskId, QMSAPI.TaskStatusScope.All);
            }

            string Msg = string.IsNullOrEmpty(status.Extended.LastLogMessages) ? "" : status.Extended.LastLogMessages;
            Trace.WriteLine(string.Format("Task ID {0} finished at server time {1}. Current status is '{2}' with exit message '{3}'", TaskIdStr, status.Extended.FinishedTime, status.General.Status.ToString(), Msg));
        }

        #region Late Execution methods

        private void backLog(string category, Action action)
        {
            if (!_late_execution.ContainsKey(category))
                _late_execution.Add(category, new List<Action>());
            _late_execution[category].Add(action);
        }

        private void AddWrapUpAction(string source, string destination, bool deleteSource)
        {
            // VWN:when wrapped as an action the code never seems to execute
            // DPJ: Yeap, it's being called now, with the Wrap Up and User Log one
            this.AddWrapUpAction(() => {
                try
                {
                    File.Copy(source, destination, true);
                    Trace.WriteLine(string.Format("File '{0}' successfully copied", destination));
                    if (deleteSource)
                    {
                        Trace.WriteLine(string.Format("Deleting file '{0}'", source));
                        File.Delete(source);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("Failed to copy file '{0}',\r\n{1}", destination, ex.Message));
                }
            });
        }

        private void AddCleanUpAction(string file, string description = "")
        {
            this.AddCleanUpAction(() => {
                try
                {
                    Trace.WriteLine(string.Format("Removing file '{0}'", file));
                    if (File.Exists(file))
                        File.Delete(file);
                    else
                        Trace.WriteLine("File does not exist...");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("Unable to clean up file '{0}' - {1}\r\n{2}", file, description, ex.Message));
                }
            });
        }

        private void AddCleanUpAction(Action action)
        {
            this.backLog(C_CLEAN, action);
        }

        private void AddWrapUpAction(Action action)
        {
            this.backLog(C_WRAP, action);
        }
        private void LogToUser(string message, params object[] args)
        {
            LogToUser(string.Format(message, args));
        }
        private void LogToUser(string message)
        {
            _userMessage.Append(string.Format("{0} [USER] {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), message));
        }
        #endregion

        /// <summary>
        /// Clean up ALL orphan tasks for the Milliman Source Document folder
        /// </summary>
        public void CleanUpOrphans()
        {
            int error_count = 0;
            connectToQMS(this.QVConnectionSettings);
            getSourceDocumentFolder(out _source_document);

            var orphan_folders = _qms_client.GetSourceDocumentNodes(_qds_guid, _source_document.ID, "\\<Orphans>");
            if (orphan_folders.Length > 0)
                Trace.WriteLine(string.Format("Found {0} orphan folders", orphan_folders.Length));
            else
                Trace.WriteLine("No Orphan documents were found for clean up.");

            foreach (var node in orphan_folders)
            {
                var ophan_document_folders = _qms_client.GetSourceDocumentNodes(_qds_guid, _source_document.ID, "\\<Orphans>\\" + node.Name);
                foreach (var orphan_document_node in ophan_document_folders)
                {
                    try
                    {
                        Trace.WriteLine(string.Format("Checking tasks for document '{0}'", orphan_document_node.Name));
                        var orphan_document_tasks = _qms_client.GetTasksForDocument(orphan_document_node.ID);
                        foreach (var document_task in orphan_document_tasks)
                        {
                            try
                            {
                                Trace.WriteLine(string.Format("Deleting task '{0}' of type '{1}'", document_task.Name, document_task.Type.ToString()));
                                _qms_client.DeleteTask(document_task.ID, document_task.Type);
                            }
                            catch (Exception task_exception)
                            {
                                error_count++;
                                Trace.WriteLine(string.Format("Error while trying to delete task '{0}':\r\n\t\t{1}", document_task.Name, task_exception.Message));
                            }
                        }
                    }
                    catch (Exception orphan_document_exception)
                    {
                        error_count++;
                        Trace.WriteLine(string.Format("Error while trying to get documents for folder '{0}':\r\n\t\t{1}", orphan_document_node.Name, orphan_document_exception.Message));
                    }
                }
            }
            if (error_count > 0)
                Trace.WriteLine(string.Format("Sanitation complete with {0} error{1}.", error_count, error_count > 1 ? "s" : ""));
            else
                Trace.WriteLine("Sanitation was completed with no errors.");
        }

        public Dictionary<string, int> GetQVSDocumentsPerUser()
        {
            if (_qvs_guid != Guid.Empty)
            {
                return _qms_client.GetQVSDocumentsPerUser(_qvs_guid, QMSAPI.QueryTarget.Resource);
            }
            else
            {
                throw new InvalidDataException("Could not get QVS data because, during service initialization, we could not fetch a QVS GUID to work with. Please contact the administrator...");
            }
        }
    }
}
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using log4net;

//namespace Milliman.Reduction.ReductionEngine {
//    using Milliman.Common;
//    using Milliman.Common.TreeUtilities;

//    [Flags]
//    public enum enumSummary {
//        QMSConnection = 2,
//        GetSourceDocument = 3,
//        PrepareFolderReduction = 4,
//        CreateAncillaryScript = 8,
//        TaskCreateHierarchy = 16,
//        TaskRunHierarchy = 32,
//        CleanUp = 64,
//        TreeInfo = 128,
//        TaskCreateReduce = 256,
//        TaskRunReduce = 512,
//        PrepareFolderHierarchy = 1024
//    }

//    public class ReductionRunner {
//        private static string C_LOG = "User Notification", C_CLEAN = "Clean Up", C_WRAP = "Wrap Up";
//        private ILog _L = log4net.LogManager.GetLogger(typeof(ReductionRunner));
//        private Guid _task_id_hierarchy, _task_id_reduction;
//        private Guid _qds_guid;
//        private QMSConnection _qms_conn;
//        private QMSAPI.DocumentFolder _source_document;
//        private QMSAPI.IQMS _qms_client;
//        private QMSAPI.TaskInfo _task_info_hierarchy;
//        StringBuilder _userMessage = new StringBuilder();

//        private Dictionary<string, List<Action>> _late_execution = new Dictionary<string, List<Action>>();
//        //private List<Action> _cleanUpActions = new List<Action>(), _wrapUpActions = new List<Action>();


//        public Milliman.Common.ReduceConfig ConfigFile { get; set; }
//        public string QVWOriginalFullFileName { get; set; }
//        public string QVWHierarchyFQFileNameGUID { get; private set; }
//        public string QVWHierarchyFQFolderNameGUID { get; set; }
//        public string QVWReductionFQFileNameGUID { get; private set; }
//        public string QVWReductionFQFolderNameGUID { get; set; }
//        public string SourceDocumentWorkingPath { get; set; }
//        public QMSSettings QVConnectionSettings { get; set; }

//        public ReductionRunner() {
//            this.QVConnectionSettings = new QMSSettings();
//        }
//        public ReductionRunner(QMSSettings settings) : this() {
//            this.QVConnectionSettings = settings;
//        }

//        public void Run() {
//            try
//            {
//                enumSummary summary = 0;
//                while (true)
//                {
//                    if (((summary |= connectToQMS(this.QVConnectionSettings)) & enumSummary.QMSConnection) == 0) break;
//                    if (((summary |= getSourceDocumentFolder(out _source_document)) & enumSummary.GetSourceDocument) == 0) break;
//                    if (((summary |= prepareFileStructureForHierarchy(this.QVWOriginalFullFileName, this.SourceDocumentWorkingPath)) & enumSummary.PrepareFolderHierarchy) == 0) break;
//                    if (((summary |= createAncillaryScript(this.QVWHierarchyFQFolderNameGUID)) & enumSummary.CreateAncillaryScript) == 0) break;
//                    if (((summary |= createHierarchyTask(this.QVWHierarchyFQFileNameGUID, _source_document)) & enumSummary.TaskCreateHierarchy) == 0) break;
//                    if (((summary |= runHierarchyTask(_task_info_hierarchy)) & enumSummary.TaskRunHierarchy) == 0) break;

//                    foreach (var set in this.ConfigFile.SelectionSets)
//                    {
//                        try
//                        {
//                            if (set == null) continue;
//                            this.generateTreeInfo(set, this.QVWHierarchyFQFolderNameGUID);
//                            this.prepareFileStructureForReduction(this.QVWOriginalFullFileName, this.SourceDocumentWorkingPath);
//                            QMSAPI.DocumentTask reduction_document_task = this.createReductionTask(this.SourceDocumentWorkingPath,
//                                set.ReducedQVWName,
//                                set.DropDisassoicatedDataModelTables,
//                                set.SelectionCriteria, set.DropTables);
//                            this.runReductionTask(reduction_document_task, true);
//                            if ((set.UniqueValuesFromMasterQVWColumns != null ) && (set.UniqueValuesFromMasterQVWColumns.Count > 0))
//                                this.generateUniqueValuesForQVW(this.QVWHierarchyFQFileNameGUID, set.UniqueValuesFromMasterQVWColumns);
//                            if ((set.UniqueValuesFromReducedQVWColumns!=null) && (set.UniqueValuesFromReducedQVWColumns.Count > 0))
//                                this.generateUniqueValuesForQVW(set.ReducedQVWName, set.UniqueValuesFromReducedQVWColumns);
//                            flushLogMessages(set, true, null);
//                        }
//                        catch (Exception ex)
//                        {
//                            flushLogMessages(set, false, ex.Message);
//                        }

//                    }
//                    break;
//                }
//            }
//            catch (Exception ex)
//            {
//                Trace.WriteLine(ex);  //catch and log the exception
//            }

//            // Creates the Process_Complete flag file
//            createCompleteFlags(this.ConfigFile.ConfigFileNameWithoutExtension);
//        }

//        /// <summary>
//        /// Create a "GUID_completed.txt" file for each completed task
//        /// Check to see if I am the last task in this group to execute, if so write out a "processing_complete.txt" file
//        /// </summary>
//        /// <param name="ConfigurationName"></param>
//        private void createCompleteFlags( string ConfigurationName ) {
//            string complete_process_stream = Path.Combine(ConfigFile.ConfigFQFolderName , ConfigurationName +  "_completed.txt");
//            using(StreamWriter complete_process_writer = new StreamWriter(complete_process_stream) ) {
//                complete_process_writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"));
//                complete_process_writer.Flush();
//            }

//            //Note: this code is experimental and does not always give the intended results.....
//            //look to see if all the other tasks have completed, if so I am the last one, so write out the processing_complete.txt semaphore, all tasks are done
//            string[] ConfigurationFiles = Directory.GetFiles(ConfigFile.ConfigFQFolderName, "*.config", SearchOption.TopDirectoryOnly);
//            string[] CompletedFiles = Directory.GetFiles(ConfigFile.ConfigFQFolderName, "*_completed.txt", SearchOption.TopDirectoryOnly);
//            if (ConfigurationFiles.Length == CompletedFiles.Length)
//            {  //each config file has a completed file, thus we must be last task executing
//                string all_done_stream = Path.Combine(ConfigFile.ConfigFQFolderName, "processing_complete.txt");
//                using (StreamWriter all_done_writer = new StreamWriter(all_done_stream))
//                {
//                    all_done_writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"));
//                    all_done_writer.Flush();
//                }
//            }
//        }

//        private void flushLogMessages(ReductionSettings set, bool success, string errorMessage) {
//            string master_log = Path.Combine(ConfigFile.ConfigFQFolderName, ConfigFile.MasterStatusLog);
//            System.Diagnostics.Debug.WriteLine("master->" + ConfigFile.MasterStatusLog);
//            System.Diagnostics.Debug.WriteLine("task->" + set.QVWStatusLog);
//            if( !File.Exists(master_log) ) {
//                using( File.Create(master_log) ) { }
//            }

//            using( StreamWriter master_log_writer = File.AppendText(master_log) ) {
//                master_log_writer.WriteLine(string.Format("{0}|{1}{2}",
//                    ConfigFile.ConfigFileName,
//                    success ? "SUCCESS" : "FAIL",
//                    success ? "" : "|" + errorMessage));
//                master_log_writer.Flush();
//            }

//            //VWN:check to see if there is a QVWStatusLog, when processing the master QVW to generate just hierarchies there may
//            //not be a QVWStausLog - not an error condition
//            if (string.IsNullOrEmpty(set.QVWStatusLog) == false)
//            {
//                string status_log = Path.Combine(ConfigFile.ConfigFQFolderName, set.QVWStatusLog);
//                using (StreamWriter status_log_writer = File.AppendText(status_log))
//                {
//                    status_log_writer.WriteLine(string.Format("{0}", success ? "SUCCESS" : "FAIL"));
//                    status_log_writer.Flush();
//                }
//            }

//        }


//        private void generateUniqueValuesForQVW(string qvw, List<NVPair> uniqueColumns ) {

//        }

//        private enumSummary connectToQMS(QMSSettings qmsSettings) {
//            if( qmsSettings == null ) {
//                Trace.WriteLine("Unable to establish connection to QMS: QMS Settings is null");
//                return 0;
//            } else if( string.IsNullOrEmpty((qmsSettings.QMSURL + "").Trim()) ) {
//                Trace.WriteLine("Unable to establish connection to QMS: the QMS address is empty");
//                return 0;
//            }
//            try {
//                Trace.WriteLine(string.Format("Connecting to QMS on '{0}'", qmsSettings.QMSURL));
//                _qms_conn = new QMSConnection();
//                if( string.IsNullOrEmpty(qmsSettings.UserName) )
//                    _qms_conn.Connect(qmsSettings.QMSURL);
//                else
//                    _qms_conn.Connect(qmsSettings.QMSURL, qmsSettings.UserName, qmsSettings.GetPassword());
//                _qms_client = _qms_conn.QlikClient;
//                Trace.WriteLine("Connection successfully established...");
//                return enumSummary.QMSConnection;
//            } catch( Exception ex ) {
//                LogToUser("Error attempting to connect to Qlikview Server: {0}", ex.Message);
//                Trace.WriteLine("An error occurred when attempting to connect to QMS", ex);
//                return 0;
//            }
//        }

//        private enumSummary getSourceDocumentFolder(out QMSAPI.DocumentFolder qmsFolder) {
//            qmsFolder = null;
//            try {
//                Trace.WriteLine("Attempting to fetch Publisher Service ID");
//                var services = _qms_client.GetServices(QMSAPI.ServiceTypes.QlikViewDistributionService);
//                if( services == null || services.Length == 0 ) {
//                    Trace.WriteLine("QMS did not return any Publisher services. Current reduction will be terminated");
//                    return 0;
//                }
//                _qds_guid = services[0].ID;
//                Trace.WriteLine("Publisher service successfully found.");
//                Trace.WriteLine(string.Format("Working with publisher service '{0}', of id '{1}'", services[0].Name, services[0].ID));

//                Trace.WriteLine("Attempting to fetch SourceDocuments path");
//                QMSAPI.DocumentFolder[] paths = _qms_client.GetSourceDocumentFolders(_qds_guid, QMSAPI.DocumentFolderScope.All);
//                if( paths == null || paths.Length == 0 ) {
//                    Trace.WriteLine("Could not get any SourceDocument from the server. Current reduction will be terminated");
//                    return 0;
//                } else if( paths.Length > 1 ) {
//                    Trace.WriteLine("Multiple paths were found on the Publisher. Searching for a Milliman path");
//                    foreach( var p in paths ) {
//                        Trace.WriteLine(string.Format("Searching for 'Milliman' string in '{0}' path...", p.General.Path));
//                        if( p.General.Path.ToLower().IndexOf("milliman") > 0 ) {
//                            Trace.WriteLine(string.Format("Found a matching path for Milliman: {0}", p.General.Path));
//                            qmsFolder = p;
//                            break;
//                        }
//                    }
//                    if( qmsFolder == null ) {
//                        Trace.WriteLine(string.Format("No matching folders were found for Milliman. Falling back to first path found: '{0}'", paths[0].General.Path));
//                        qmsFolder = paths[0];
//                    }
//                } else if( paths.Length == 1 ) {
//                    Trace.WriteLine("Only one path found on the Publisher. Setting up working path...");
//                    qmsFolder = paths[0];
//                }
//                this.SourceDocumentWorkingPath = qmsFolder.General.Path;
//                Trace.WriteLine(string.Format("Working path set to '{0}'", qmsFolder.General.Path));
//                return enumSummary.GetSourceDocument;
//            } catch( Exception ex ) {
//                LogToUser("Error attempting to get SourceDocuments folder from Qlikview Server: {0}", ex.Message);
//                Trace.WriteLine("Unable to fetch DocumentFolder ID", ex);
//                return 0;
//            }
//        }

//        private enumSummary prepareFileStructureForHierarchy(string sourceFileName, string destinationFolderName) {
//            try {
//                _task_id_hierarchy = Guid.NewGuid();
//                this.QVWHierarchyFQFolderNameGUID = Path.Combine(destinationFolderName, _task_id_hierarchy.ToString("N"));
//                Trace.WriteLine("Attempting to create destination folder");
//                Directory.CreateDirectory(this.QVWHierarchyFQFolderNameGUID);
//                Trace.WriteLine(string.Format("Destination folder '{0}' created successfully...", this.QVWHierarchyFQFolderNameGUID));

//                this.QVWHierarchyFQFileNameGUID = Path.Combine(this.QVWHierarchyFQFolderNameGUID, _task_id_hierarchy.ToString("N") + ".qvw");
//                Trace.WriteLine("Copying local QVW File into the Qlikview Server...");
//                Trace.WriteLine(string.Format("Source path: '{0}'", sourceFileName));
//                Trace.WriteLine(string.Format("Destination path: '{0}'", this.QVWHierarchyFQFileNameGUID));
//                File.Copy(sourceFileName, this.QVWHierarchyFQFileNameGUID, true);
//                Trace.WriteLine("File successfully copied");
//                return enumSummary.PrepareFolderHierarchy;
//            } catch( Exception ex ) {
//                LogToUser("Unable to prepare Structure to Process Hierarchy files: {0}" , ex.Message);
//                Trace.WriteLine("Unable copy local file to QlikView server", ex);
//                return 0;
//            }

//        }

//        private enumSummary createAncillaryScript(string destinationFolder) {
//            try {
//                Trace.WriteLine("Attempting to create 'ancillary script' on destination folder");
//                string ancillary_full_file_path = Path.Combine(destinationFolder, "ancillary_script.txt");
//                using( FileStream ancillary_stream = File.Create(ancillary_full_file_path) ) {
//                    using( StreamWriter ancillary_writer = new StreamWriter(ancillary_stream) ) {
//                        ancillary_writer.WriteLine("LET DataExtraction=true();");
//                        ancillary_writer.Flush();
//                    }
//                }
//                string legacy_full_file_path = Path.Combine(destinationFolder, "care_coordinator_report.txt"); ;
//                Trace.WriteLine("Creating legacy file");
//                File.Copy(ancillary_full_file_path, legacy_full_file_path);
//                Trace.WriteLine(string.Format("Ancillary successfully created on '{0}'", ancillary_full_file_path));
//                return enumSummary.CreateAncillaryScript;
//            } catch( Exception ex ) {
//                LogToUser("Unable to create 'ancillary script' for processing: {0}", ex.Message);
//                Trace.WriteLine("Unable to create 'ancillary script'. The process will be terminated...", ex);
//                return 0;
//            }
//        }

//        private enumSummary createHierarchyTask(string qvwDocumentPath, QMSAPI.DocumentFolder qmsFolder) {
//            _task_info_hierarchy = null;
//            try {
//                string qvw_file_name = Path.GetFileName(qvwDocumentPath);
//                Trace.WriteLine(string.Format("Creating task for Qlikview Document '{0}'", qvwDocumentPath));
//                _qms_client.ClearQVSCache(QMSAPI.QVSCacheObjects.All);
//                Trace.WriteLine("QVSCache successfully cleared");
//                QMSAPI.DocumentNode node = null;
//                Trace.WriteLine("Searching for correct document node on Publisher's Source Documents");
//                foreach( var doc_node in _qms_client.GetSourceDocuments(this._qds_guid) ) {
//                    if( doc_node.Name == qvw_file_name ) {
//                        node = doc_node;
//                        break;
//                    }
//                }
//                if( node == null ) {
//                    Trace.WriteLine(string.Format("Could not find node '{0}' on Publisher's source documents. The process will be terminated ...", qvw_file_name));
//                    return 0;
//                } else {
//                    Trace.WriteLine(string.Format("Successfully fetched SourceDocument '{0}' from Qlikview Publisher", qvw_file_name));
//                }
//                Trace.WriteLine("Setting up task properties...");
//                Trace.WriteLine("Configuring General Tab");
//                QMSAPI.DocumentTask temp_task = QMSWrapper.GetTask();

//                temp_task.ID = this._task_id_hierarchy;
//                temp_task.QDSID = this._qds_guid;
//                temp_task.Document = node;
//                temp_task.General = new QMSAPI.DocumentTask.TaskGeneral();
//                temp_task.General.Enabled = true;
//                temp_task.General.TaskName = "Reduction | Partial Reload | " + qvw_file_name.ToUpper();
//                temp_task.General.TaskDescription = string.Format("Automatically generated by ReductionService on {0}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));

//                #region Reload Tab
//                Trace.WriteLine("Configuring Reload Tab");
//                temp_task.Scope |= QMSAPI.DocumentTaskScope.Reload;
//                temp_task.Reload = new QMSAPI.DocumentTask.TaskReload();
//                temp_task.Reload.Mode = QMSAPI.TaskReloadMode.Partial;
//                #endregion

//                #region Reduce Tab
//                Trace.WriteLine("Configuring Reduce Tab...");
//                temp_task.Scope |= QMSAPI.DocumentTaskScope.Reduce;
//                temp_task.Reduce = new QMSAPI.DocumentTask.TaskReduce();
//                temp_task.Reduce.DocumentNameTemplate = string.Empty;
//                temp_task.Reduce.Static = new QMSAPI.DocumentTask.TaskReduce.TaskReduceStatic();
//                temp_task.Reduce.Static.Reductions = new QMSAPI.TaskReduction[] { new QMSAPI.TaskReduction() };
//                temp_task.Reduce.Static.Reductions[0].Type = new QMSAPI.TaskReductionType();
//                temp_task.Reduce.Dynamic = new QMSAPI.DocumentTask.TaskReduce.TaskReduceDynamic();
//                temp_task.Reduce.Dynamic.Type = new QMSAPI.TaskReductionType();
//                #endregion

//                #region Distribution Tab // we don't really need it at the time.
//                //Trace.WriteLine("Configuring Distribution Tab...");
//                //temp_task.Scope |= QMSAPI.DocumentTaskScope.Distribute;
//                //temp_task.Distribute = new QMSAPI.DocumentTask.TaskDistribute();
//                //temp_task.Distribute.Output = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeOutput();
//                //temp_task.Distribute.Output.Type = QMSAPI.TaskDistributionOutputType.QlikViewDocument;

//                //temp_task.Distribute.Static = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeStatic();
//                //temp_task.Distribute.Static.DistributionEntries = new QMSAPI.TaskDistributionEntry[] { new QMSAPI.TaskDistributionEntry() };
//                //temp_task.Distribute.Static.DistributionEntries[0].Recipients = new QMSAPI.DirectoryServiceObject[] { new QMSAPI.DirectoryServiceObject() };
//                //temp_task.Distribute.Static.DistributionEntries[0].Recipients[0].Type = QMSAPI.DirectoryServiceObjectType.Authenticated;
//                //temp_task.Distribute.Static.DistributionEntries[0].Destination = new QMSAPI.TaskDistributionDestination();
//                //temp_task.Distribute.Static.DistributionEntries[0].Destination.Folder = new QMSAPI.TaskDistributionDestination.TaskDistributionDestinationFolder();
//                //temp_task.Distribute.Static.DistributionEntries[0].Destination.Type = QMSAPI.TaskDistributionDestinationType.None;
//                //temp_task.Distribute.Static.DistributionEntries[0].Destination.Folder.Name = "";

//                //temp_task.Distribute.Notification = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeNotification();
//                //temp_task.Distribute.Notification.SendNotificationEmail = false;

//                //temp_task.Distribute.Dynamic = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeDynamic();
//                //temp_task.Distribute.Dynamic.IdentityType = QMSAPI.UserIdentityValueType.DisplayName;
//                //temp_task.Distribute.Dynamic.Destinations = new QMSAPI.TaskDistributionDestination[] { new QMSAPI.TaskDistributionDestination() };
//                #endregion

//                #region Trigger Tab
//                Trace.WriteLine("Configuring task trigger ...");
//                temp_task.Scope |= QMSAPI.DocumentTaskScope.Triggering;
//                temp_task.Triggering = new QMSAPI.DocumentTask.TaskTriggering();
//                temp_task.Triggering.ExecutionAttempts = 1;
//                temp_task.Triggering.ExecutionTimeout = 1440;
//                temp_task.Triggering.TaskDependencies = new QMSAPI.TaskInfo[] { };
//                temp_task.Triggering.Triggers = new QMSAPI.ScheduleTrigger[] {
//                    new QMSAPI.ScheduleTrigger()
//                };
//                temp_task.Triggering.Triggers[0].Enabled = true;
//                temp_task.Triggering.Triggers[0].Type = QMSAPI.TaskTriggerType.OnceTrigger;
//                ((QMSAPI.ScheduleTrigger) temp_task.Triggering.Triggers[0]).StartAt = DateTime.Now;
//                #endregion

//                Trace.WriteLine("Assynchronously saving task...");
//                _qms_client.SaveDocumentTask(temp_task);
//                do {
//                    System.Threading.Thread.Sleep(500);
//                    _task_info_hierarchy = _qms_client.FindTask(_qds_guid, QMSAPI.TaskType.DocumentTask, temp_task.General.TaskName);
//                } while( _task_info_hierarchy == null );

//                Trace.WriteLine(string.Format("Task of ID '{0}' successfully saved", temp_task.ID.ToString("N")));
//                return enumSummary.TaskCreateHierarchy;
//            } catch( Exception ex ) {
//                LogToUser("Error when trying to setup task on Qlikview Server: {0}", ex.Message);
//                Trace.WriteLine("An error occurred while trying to setup and execute the Task on the Publisher server. The process will be terminated...", ex);
//                return 0;
//            }
//        }

//        private enumSummary runHierarchyTask(QMSAPI.TaskInfo task) {
//            try {
//                DateTime start_time = DateTime.Now;
//                this.RunTaskSynchronously(task.ID);
//                this.AddCleanUpAction(new Action(() => {
//                    Trace.WriteLine(string.Format("Deleting task '{0}'", task.ID.ToString("N")));
//                    _qms_client.DeleteTask(task.ID, QMSAPI.TaskType.DocumentTask);
//                }));
//                DateTime end_time = DateTime.Now;
//                LogToUser("Hierarchy Task successfully ran on Qlikview Server");
//                return enumSummary.TaskRunHierarchy;
//            } catch( Exception ex ) {
//                LogToUser("Error when trying to run Hierarchy task on Qlikview Server: {0}", ex.Message);
//                Trace.WriteLine("An error occurred when trying to run the Hierarchy task...", ex);
//                return 0;
//            }
//        }

//        private enumSummary generateTreeInfo(ReductionSettings selectionSet, string workingFolder) {
//            try {
//                List<string> list_hierarchy, list_concept, list_drop, list_model;
//                TreeBuilderProcessor tree_processor = new TreeBuilderProcessor();
//                Trace.WriteLine("Beginning TreeBuild processing...");
//                if( !tree_processor.BuildTree(workingFolder, out list_hierarchy, out list_concept, out list_drop, out list_model) ) {
//                    Trace.WriteLine("Unable to process ");
//                    return 0;
//                }

//                string destination_folder = Path.GetDirectoryName(this.QVWOriginalFullFileName);
//                string source_folder = this.QVWHierarchyFQFolderNameGUID, file_name = string.Empty;

//                if( !string.IsNullOrEmpty(selectionSet.HierarchyFile) ) {
//                    Trace.WriteLine("Adding backlog file move for HierarchyFile");
//                    this.AddWrapUpAction(list_hierarchy[0], Path.Combine(destination_folder, Path.GetFileName(selectionSet.HierarchyFile)));
//                }
//                if( !string.IsNullOrEmpty(selectionSet.ConceptFile) ) {
//                    Trace.WriteLine("Adding backlog file move for ConceptFile");
//                    this.AddWrapUpAction(list_concept[0], Path.Combine(destination_folder, Path.GetFileName(selectionSet.ConceptFile)));
//                }

//                if( !string.IsNullOrEmpty(selectionSet.MapFile) ) {
//                    Trace.WriteLine("Adding backlog file move for MapFile");
//                    this.AddWrapUpAction(list_drop[0], Path.Combine(destination_folder, Path.GetFileName(selectionSet.MapFile)));
//                }

//                if( !string.IsNullOrEmpty(selectionSet.DataModelFile) ) {
//                    Trace.WriteLine("Adding backlog file move for DataModelFile");
//                    this.AddWrapUpAction(list_model[0], Path.Combine(destination_folder, Path.GetFileName(selectionSet.DataModelFile)));
//                }

//                LogToUser("Generate TreeInfo successfully generated intermediate data files");
//                return enumSummary.TreeInfo;
//            } catch( Exception ex ) {
//                LogToUser("Unable to Generate Tree Info: {0}", ex.Message);
//                Trace.WriteLine("Unable to generate tree files", ex);
//                return 0;
//            }
//        }

//        private enumSummary prepareFileStructureForReduction(string sourceFileName, string destinationFolderName) {
//            try {
//                _task_id_reduction = Guid.NewGuid();
//                this.QVWReductionFQFolderNameGUID = Path.Combine(destinationFolderName, _task_id_reduction.ToString("N"));
//                Trace.WriteLine(string.Format("Attempting to create destination folder '{0}'", this.QVWReductionFQFolderNameGUID));
//                Directory.CreateDirectory(this.QVWReductionFQFolderNameGUID);
//                Trace.WriteLine(string.Format("Destination folder '{0}' created successfully...", this.QVWReductionFQFolderNameGUID));

//                this.QVWReductionFQFileNameGUID = Path.Combine(this.QVWReductionFQFolderNameGUID, _task_id_reduction.ToString("N") + ".qvw");
//                Trace.WriteLine("Copying local QVW File into the Qlikview Server...");
//                Trace.WriteLine(string.Format("Source path: '{0}'", sourceFileName));
//                Trace.WriteLine(string.Format("Destination path: '{0}'", this.QVWReductionFQFileNameGUID));
//                File.Copy(sourceFileName, this.QVWReductionFQFileNameGUID, true);
//                Trace.WriteLine("File successfully copied");
//                LogToUser("Successfully prepared folders and tasks to run the Reduction...");
//                return enumSummary.PrepareFolderReduction;
//            } catch( Exception ex ) {
//                LogToUser("Failed to prepare reduction folder and tasks for file '{0}': {1}", sourceFileName, ex.Message);
//                Trace.WriteLine("Unable copy local file to QlikView server", ex);
//                return 0;
//            }
//        }

//        private QMSAPI.DocumentTask createReductionTask(string destinationFolderName, string reducedQVWFileName, bool shouldDropDisassociatedTables, List<NVPair> selectionCriteria, List<string> tablesToDrop) {

//            try {
//                DateTime start;
//                string task_id_string = _task_id_reduction.ToString("N");
//                string task_folder = Path.Combine(destinationFolderName, task_id_string);
//                string reduced_qvw_full_file_name = Path.Combine(task_folder, reducedQVWFileName);
//                string task_full_file_name = Path.Combine(destinationFolderName, task_id_string, task_id_string + ".qvw");
//                string task_file_name = task_id_string + ".qvw";
//                Trace.WriteLine("Processing Drop Tables...");
//                start = DateTime.Now;
//                bool drop_required = ReductionHelper.dropTableProcessor(task_folder,
//                    shouldDropDisassociatedTables,
//                    this.ConfigFile.MasterQVW,
//                    selectionCriteria,
//                    tablesToDrop);
//                System.Threading.Thread.Sleep(2000);

//                //delete any previous reduced files
//                //System.IO.File.Delete(ReducedQVWName);

//                QMSAPI.DocumentNode node = null;
//                Trace.WriteLine("Searching for correct document node on Publisher's Source Documents");
//                foreach( var doc_node in _qms_client.GetSourceDocuments(this._qds_guid) ) {
//                    if( doc_node.Name == task_file_name ) {
//                        node = doc_node;
//                        break;
//                    }
//                }
//                if( node == null ) {
//                    Trace.WriteLine(string.Format("Could not find node '{0}' on Publisher's source documents. The process will be terminated ...", task_file_name));
//                    return null;
//                } else {
//                    Trace.WriteLine(string.Format("Successfully fetched SourceDocument '{0}' from Qlikview Publisher", task_file_name));
//                }
//                Trace.WriteLine("Setting up task properties...");
//                Trace.WriteLine("Configuring General Tab");
//                QMSAPI.DocumentTask temp_task = QMSWrapper.GetTask();

//                //Creates the Reduction Task
//                QMSAPI.DocumentTask task_reduction = new QMSAPI.DocumentTask();
//                task_reduction.ID = _task_id_reduction;
//                task_reduction.QDSID = _qds_guid;
//                task_reduction.Document = node;

//                #region general
//                task_reduction.Scope = QMSAPI.DocumentTaskScope.General;
//                task_reduction.General = new QMSAPI.DocumentTask.TaskGeneral();
//                task_reduction.General.Enabled = true;
//                task_reduction.General.TaskName = "Reload & Reduce " + task_file_name;
//                task_reduction.General.TaskDescription = "Performs reduction on " + task_file_name;
//                #endregion

//                #region reduce
//                task_reduction.Scope |= QMSAPI.DocumentTaskScope.Reduce;
//                task_reduction.Reduce = new QMSAPI.DocumentTask.TaskReduce();
//                task_reduction.Reduce.DocumentNameTemplate = Path.GetFileNameWithoutExtension(reduced_qvw_full_file_name);
//                task_reduction.Reduce.Static = new QMSAPI.DocumentTask.TaskReduce.TaskReduceStatic();
//                task_reduction.Reduce.Static.Reductions = new QMSAPI.TaskReduction[selectionCriteria.Count];
//                int Index = 0;
//                foreach( var selection in selectionCriteria ) {
//                    task_reduction.Reduce.Static.Reductions[Index] = new QMSAPI.TaskReduction();
//                    task_reduction.Reduce.Static.Reductions[Index].Type = QMSAPI.TaskReductionType.ByField;
//                    task_reduction.Reduce.Static.Reductions[Index].Field = new QMSAPI.TaskReduction.TaskReductionField();
//                    task_reduction.Reduce.Static.Reductions[Index].Field.Name = selection.FieldName;
//                    task_reduction.Reduce.Static.Reductions[Index].Field.Value = selection.Value;
//                    task_reduction.Reduce.Static.Reductions[Index].Field.IsNumeric = selection.IsNumeric;
//                    Index++;
//                }

//                task_reduction.Reduce.Dynamic = new QMSAPI.DocumentTask.TaskReduce.TaskReduceDynamic();
//                #endregion

//                #region distribute
//                task_reduction.Scope |= QMSAPI.DocumentTaskScope.Distribute;
//                task_reduction.Distribute = new QMSAPI.DocumentTask.TaskDistribute();
//                task_reduction.Distribute.Output = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeOutput();
//                task_reduction.Distribute.Output.Type = QMSAPI.TaskDistributionOutputType.QlikViewDocument;

//                task_reduction.Distribute.Static = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeStatic();
//                task_reduction.Distribute.Static.DistributionEntries = new QMSAPI.TaskDistributionEntry[1];
//                task_reduction.Distribute.Static.DistributionEntries[0] = new QMSAPI.TaskDistributionEntry();
//                task_reduction.Distribute.Static.DistributionEntries[0].Recipients = new QMSAPI.DirectoryServiceObject[1];
//                task_reduction.Distribute.Static.DistributionEntries[0].Recipients[0] = new QMSAPI.DirectoryServiceObject();
//                task_reduction.Distribute.Static.DistributionEntries[0].Recipients[0].Type = QMSAPI.DirectoryServiceObjectType.Authenticated;
//                task_reduction.Distribute.Static.DistributionEntries[0].Destination = new QMSAPI.TaskDistributionDestination();
//                task_reduction.Distribute.Static.DistributionEntries[0].Destination.Folder = new QMSAPI.TaskDistributionDestination.TaskDistributionDestinationFolder();
//                task_reduction.Distribute.Static.DistributionEntries[0].Destination.Type = QMSAPI.TaskDistributionDestinationType.Folder;
//                task_reduction.Distribute.Static.DistributionEntries[0].Destination.Folder.Name = System.IO.Path.GetDirectoryName(reduced_qvw_full_file_name);

//                task_reduction.Distribute.Notification = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeNotification();
//                task_reduction.Distribute.Notification.SendNotificationEmail = false;

//                task_reduction.Distribute.Dynamic = new QMSAPI.DocumentTask.TaskDistribute.TaskDistributeDynamic();
//                task_reduction.Distribute.Dynamic.IdentityType = QMSAPI.UserIdentityValueType.DisplayName;
//                task_reduction.Distribute.Dynamic.Destinations = new QMSAPI.TaskDistributionDestination[0];
//                #endregion

//                #region reload
//                task_reduction.Scope |= QMSAPI.DocumentTaskScope.Reload;
//                task_reduction.Reload = new QMSAPI.DocumentTask.TaskReload();
//                if( drop_required )
//                    task_reduction.Reload.Mode = QMSAPI.TaskReloadMode.Partial; //this will result in tables being dropped as defined by the drop table processor
//                else
//                    task_reduction.Reload.Mode = QMSAPI.TaskReloadMode.None;
//                #endregion

//                #region trigger
//                //documentTask.DocumentInfo = new DocumentTask.TaskDocumentInfo();
//                task_reduction.Triggering = new QMSAPI.DocumentTask.TaskTriggering();
//                task_reduction.Triggering.ExecutionAttempts = 1;
//                task_reduction.Triggering.ExecutionTimeout = 1440;
//                task_reduction.Triggering.TaskDependencies = new QMSAPI.TaskInfo[] { };

//                task_reduction.Triggering.Triggers = new QMSAPI.ScheduleTrigger[1];
//                var trigger_schedule = new QMSAPI.ScheduleTrigger();
//                trigger_schedule.Enabled = true;
//                trigger_schedule.Type = QMSAPI.TaskTriggerType.OnceTrigger;
//                trigger_schedule.StartAt = DateTime.Now.AddSeconds(5.0); //don't schedule for now, it ignores if in past
//                task_reduction.Triggering.Triggers[0] = trigger_schedule;
//                #endregion

//                _qms_client.SaveDocumentTask(task_reduction);

//                System.Threading.Thread.Sleep(4000);//give it a second


//                return task_reduction;
//            } catch( Exception ex ) {
//                Trace.WriteLine("An error occurred when trying to run the Reduction task...", ex);
//                return null;
//            }
//        }

//        private enumSummary runReductionTask(QMSAPI.DocumentTask task, bool cleanUpWhenDone) {
//            try {
//                Trace.WriteLine(string.Format("Attempting to run newly created task '{0}' ...", task.ID));
//                DateTime start_time = DateTime.Now;
//                /*******************************************************************************************/
//                this.RunTaskSynchronously(task.ID);
//                /*******************************************************************************************/
//                DateTime end_time = DateTime.Now;

//                string remote_reduced_qvw_fq_file_name = Path.Combine(task.Distribute.Static.DistributionEntries[0].Destination.Folder.Name, task.Reduce.DocumentNameTemplate + ".qvw");
//                string local_reduced_qvw_fq_file_name = Path.Combine(Path.GetDirectoryName(this.QVWOriginalFullFileName), Path.GetFileName(remote_reduced_qvw_fq_file_name));

//                if( File.Exists(local_reduced_qvw_fq_file_name) ) {
//                    Trace.WriteLine(string.Format("File '{0}' already exists so it'll be deleted before a copy is attempted...", local_reduced_qvw_fq_file_name));
//                    File.Delete(local_reduced_qvw_fq_file_name);
//                }
//                Trace.WriteLine(string.Format("Copying reduced qvw from '{0}' back into the source folder '{1}'", remote_reduced_qvw_fq_file_name, local_reduced_qvw_fq_file_name));
//                this.AddWrapUpAction(remote_reduced_qvw_fq_file_name, local_reduced_qvw_fq_file_name);

//                this.AddCleanUpAction(new Action(() => {
//                    Trace.WriteLine(string.Format("Deleting task '{0}'", task.ID.ToString("N")));
//                    _qms_client.DeleteTask(task.ID, QMSAPI.TaskType.DocumentTask);
//                    Trace.WriteLine(string.Format("Deleting original file '{0}'", task.General));
//                    Directory.Delete(Path.GetDirectoryName(remote_reduced_qvw_fq_file_name), true);
//                }));
//                LogToUser("Successfully ran reduction task [id: {0}; name: \"{1}\"] for current selection set.", task.ID, task.General.TaskName);
//                return enumSummary.TaskRunReduce;
//            } catch( Exception ex ) {
//                LogToUser("Unable to run Reduction Task [id: {0}; name: {1}]: {2}", task.ID, task.General.TaskName, ex.Message);
//                Trace.WriteLine("An error occurred when trying to run the Reduction task...", ex);
//                return 0;
//            }
//        }

//        private void RunTaskSynchronously(Guid taskId) {
//            // Monitors the running task and returns when it's over
//            QMSAPI.TaskStatus status = null;
//            bool is_running = true;
//            int check_if_running = 1;
//            while( is_running ) {
//                Trace.WriteLine(string.Format("Attempting to run task of id '{0}'", taskId.ToString("N")));
//                _qms_client.RunTask(taskId);

//                while( true ) {
//                    if( check_if_running > 0 ) check_if_running++;
//                    status = _qms_client.GetTaskStatus(taskId, QMSAPI.TaskStatusScope.All);
//                    //if( status == null || status.General.Status == QMSAPI.TaskStatusValue.Running )
//                    if( status.General.Status == QMSAPI.TaskStatusValue.Running ) {
//                        if( check_if_running != -1 ) Trace.WriteLine(string.Format("Task of ID '{0}' is currently running", taskId.ToString("N")));
//                        check_if_running = -1;
//                    }

//                    if( string.IsNullOrEmpty(status.Extended.FinishedTime) || status.Extended.FinishedTime == "Never" ) {
//                        Trace.WriteLine(string.Format("Task has not finished running yet: {0} seconds", check_if_running));
//                        System.Threading.Thread.Sleep(1000); /* Waiting 1 second to check on the task execution again */
//                    } else {
//                        Trace.WriteLine(string.Format("Task has finished running at {0} (server time). Current status is '{1}'", status.Extended.FinishedTime, status.General.Status.ToString()));
//                        is_running = false;
//                        break;
//                    }
//                    if( check_if_running % 10 == 0 ) break;
//                }
//            }
//        }

//        #region Late Execution methods

//        private void backLog(string category, Action action) {
//            if( !_late_execution.ContainsKey(category) )
//                _late_execution.Add(category, new List<Action>());
//            _late_execution[category].Add(action);
//        }

//        private void AddWrapUpAction(string source, string destination) {
//         //   VWN:when wrapped as an action the code never seems to execute
//         //   this.AddWrapUpAction(() => {
//                try {
//                    File.Copy(source, destination, true);
//                    Trace.WriteLine(string.Format("File '{0}' successfully copied", destination));
//                } catch( Exception ex ) {
//                    Trace.WriteLine(string.Format("Failed to copy file '{0}',\r\n{1}", destination, ex.Message));
//                }
//         //   });
//        }

//        private void AddCleanUpAction(Action action) {
//            this.backLog(C_CLEAN, action);
//        }

//        private void AddWrapUpAction(Action action) {
//            this.backLog(C_WRAP, action);
//        }
//        private void LogToUser(string message, params object[] args) {
//            LogToUser(string.Format(message, args));
//        }
//        private void LogToUser(string message) {
//            _userMessage.Append(string.Format("{0} [USER] {1}",DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), message));
//        }
//        #endregion



//    }
//}
