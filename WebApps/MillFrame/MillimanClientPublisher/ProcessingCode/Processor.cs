using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace ClientPublisher.ProcessingCode
{
    public class Processor
    {
        const string ReadySemaphore = @"request_complete.txt";
        const string ProcessCompleteSemaphore = @"processing_complete.txt";
        /// <summary>
        /// Create the temporary working directory, all new files will be on root of working directory
        /// and in reducedcachedqvw and reduceduserqvws,  while all the old files are copied from current
        /// published directory to a "legacy" directory below working directory.  Legacy maintains the
        /// working structure with reducedd directories
        /// </summary>
        /// <param name="WorkingDirectory"></param>
        /// <returns></returns>
        public bool SetupWorkingDirectory( string WorkingDirectory )
        {
            if ( Directory.Exists(WorkingDirectory) == false  )
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Requested to setup working dir, but it does not exist '" + WorkingDirectory + ";");
                return false;
            }
            //clear out the reduced dirs below working directory
            if (System.IO.Directory.Exists(System.IO.Path.Combine(WorkingDirectory, "ReducedCachedQVWs")))
                System.IO.Directory.Delete(System.IO.Path.Combine(WorkingDirectory, "ReducedCachedQVWs"),true);
            if (System.IO.Directory.Exists(System.IO.Path.Combine(WorkingDirectory, "ReducedUserQVWs")))
                System.IO.Directory.Delete(System.IO.Path.Combine(WorkingDirectory, "ReducedUserQVWs"),true);
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(WorkingDirectory, "ReducedCachedQVWs"));
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(WorkingDirectory, "ReducedUserQVWs"));

            //get "legacy" directory ready
            if (System.IO.Directory.Exists(System.IO.Path.Combine(WorkingDirectory, "Legacy")))
                System.IO.Directory.Delete(System.IO.Path.Combine(WorkingDirectory, "Legacy"),true);
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(WorkingDirectory, @"Legacy\ReducedCachedQVWs"));
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(WorkingDirectory, @"Legacy\ReducedUserQVWs"));

            return System.IO.Directory.Exists(WorkingDirectory);
        }

        /// <summary>
        /// Copy over all the relevant files from published site to the "legacy" directory
        /// </summary>
        /// <param name="WorkingDirectory"></param>
        /// <param name="CurrentProject"></param>
        /// <returns></returns>
        public bool CopyLegacyProjectFiles( string WorkingDirectory, ProjectSettingsExtension CurrentProject )
        {
            try
            {
                WorkingDirectory = Path.Combine(WorkingDirectory, "Legacy");
                string[] Extensions = new string[] { "*.hierarchy*", "*.concept*", "*.map*", "*.selections","*.redirect","*.png","*.gif","*.jpg","*.jpeg" };
                foreach (string Ext in Extensions)
                {
                    string[] AllFiles = System.IO.Directory.GetFiles(CurrentProject.AbsoluteProjectPath, Ext, System.IO.SearchOption.AllDirectories);
                    if (AllFiles != null)
                    {
                        foreach (string theFile in AllFiles)
                        {
                            string VirtualLocation = theFile.Substring(CurrentProject.AbsoluteProjectPath.Count()+1);
                            string Destination = System.IO.Path.Combine(WorkingDirectory, VirtualLocation);
                            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Destination));
                            System.IO.File.Copy(theFile, Destination);
                        }
                    }
                }
                //always copy over project and legacy qvw
                string LegacyQVW = Path.Combine(CurrentProject.AbsoluteProjectPath, CurrentProject.QVName + ".qvw");
                string LegacyProject = Path.Combine(CurrentProject.AbsoluteProjectPath, CurrentProject.ProjectName + ".hciprj");
                File.Copy(LegacyQVW, Path.Combine(WorkingDirectory, Path.GetFileName(LegacyQVW)));
                File.Copy(LegacyProject, Path.Combine(WorkingDirectory, Path.GetFileName(LegacyProject)));

                //go ahead and copy the bookmarks over with metas and cache indexs
                string SourceReducedCachedQVWsDir = Path.Combine(CurrentProject.AbsoluteProjectPath, "ReducedCachedQVWs");
                string DestReducedCachedQVWsDir = Path.Combine(WorkingDirectory, "ReducedCachedQVWs");
                List<string> CachedExtensions = new List<string> { "*.shared", "*.meta", "*.cacheindex" };
                MillimanCommon.Utilities.DirectoryCopy(SourceReducedCachedQVWsDir, DestReducedCachedQVWsDir, true, CachedExtensions);

                //special case - we don't want to copy over all the legacy QVWs - however if there is a master qvw that has been added to the cache
                //using the project name we do want to move it
                string LegacyMasterCachedQVW = Path.Combine(CurrentProject.AbsoluteProjectPath,"ReducedCachedQVWs", CurrentProject.QVName + ".qvw");
                if ( System.IO.File.Exists(LegacyMasterCachedQVW))
                {
                    string CopyTo = Path.Combine(WorkingDirectory , "ReducedCachedQVWs", CurrentProject.QVName + ".qvw");
                    System.IO.File.Copy(LegacyMasterCachedQVW, CopyTo);
                }
                //end special case
                return true;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified", ex);
            }
            return false;
        }

        //from the data model file contents,  return the items at a specific level(column), first level is zero
        private List<string> DataModelFieldsByLevel( int Level, List<string> DataModel )
        {
            List<string> ItemsAtLevel = new List<string>();
            foreach (string DataModelItem in DataModel)
            {
                string[]  DMTokens = DataModelItem.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if ((DMTokens != null) && (Level < DMTokens.Count())) //mem_report_hier_2|mem_report_hier_3
                    ItemsAtLevel.Add(DMTokens[Level]);
            }
            return ItemsAtLevel;
        }
        //get all the items from selection file by level(column)
        private List<string> SelectionFieldsByLevel(int Level, List<string> Selections)
        {
            List<string> ItemsAtLevel = new List<string>();
            foreach (string Selection in Selections )
            {
                List<string> SelTokens = Selection.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                SelTokens.RemoveAt(0); //remove first item, is always the report name
                if ((SelTokens != null) && (Level < SelTokens.Count() )) //Selection == Care Coordinator Report|Woffie Center|Dr Bob
                    ItemsAtLevel.Add(SelTokens[Level]);
            }
            return ItemsAtLevel;
        }


        public bool AutoInclusion(string WorkingDirectory, ProjectSettingsExtension ProjectSettings )
        {
            try
            {
                MillimanCommon.NewAutoInclusionProcessor NAIP = new MillimanCommon.NewAutoInclusionProcessor();
                string PreviousHierarchyFile = System.IO.Path.Combine(WorkingDirectory, "legacy", ProjectSettings.ProjectName + ".hierarchy_0");
                string CurrentHierarchyFile = System.IO.Path.Combine(WorkingDirectory, ProjectSettings.ProjectName + ".hierarchy_0");
                string UserDirectory = System.IO.Path.Combine(WorkingDirectory, "ReducedUserQVWs");
                NAIP.Process(PreviousHierarchyFile, CurrentHierarchyFile, UserDirectory, true);

                return true;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified error", ex);
            }
            return false;
        }

        /// <summary>
        /// Move all the items to the Reduction server and wait on the results
        /// </summary>
        /// <param name="WorkingDirectory"></param>
        /// <param name="ProjectSettings"></param>
        /// <param name="ReductionConfiguration"></param>
        /// <returns></returns>
        public bool ProcessMasterQVW( string WorkingDirectory, ProjectSettingsExtension ProjectSettings, MillimanCommon.ReduceConfig ReductionConfiguration, string RootReductionFolder )
        {
            //craete a directory on reduction server using GUID
            string NewReductionFolder = System.IO.Path.Combine(RootReductionFolder, ReductionConfiguration.UniqueID);
            System.IO.Directory.CreateDirectory(NewReductionFolder);

            //should only be 1
            List<string> NewMasterQVWList = System.IO.Directory.GetFiles(WorkingDirectory, "*.qvw", SearchOption.TopDirectoryOnly).ToList();
            if (NewMasterQVWList.Count() != 1)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Only 1 QVW should exist in the working directory, but " + NewMasterQVWList.Count().ToString() + " exit.");
                return false;
            }

            string NewMasterQVW = System.IO.Path.Combine(NewReductionFolder, System.IO.Path.GetFileName(NewMasterQVWList[0]));
            System.IO.File.Copy(NewMasterQVWList[0], NewMasterQVW);
            string MasterParmsFile = System.IO.Path.Combine(NewReductionFolder, "master_paramaters.config");
            ReductionConfiguration.Serialize(MasterParmsFile);

            if (System.IO.File.Exists(NewMasterQVW) == false)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to write remote file '" + NewMasterQVW + "' for reduction - remote dir = '" + NewReductionFolder + "'");
                return false;
            }
            if ( System.IO.File.Exists(MasterParmsFile) == false )
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to write remote file '" + MasterParmsFile + "' for reduction - remote dir = '" + NewReductionFolder + "'");
                return false;
            }

            //writing this empty file will start processing
            System.IO.File.WriteAllText(System.IO.Path.Combine(NewReductionFolder, ReadySemaphore),"");

            //this is the completion semaphore file
            string ProcessingComplete = System.IO.Path.Combine(NewReductionFolder, ProcessCompleteSemaphore);

            //wait till it is created
            int TimeoutSeconds = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["PrimaryReductionTimeoutSeconds"]);
            DateTime StartProcessing = DateTime.Now;
            bool ProcessedCompleted = false;
            while (DateTime.Now < StartProcessing.AddSeconds(TimeoutSeconds))
            {
                System.Threading.Thread.Sleep(100);
                //here we are waiting on the remote reduction server to process the job
                if (System.IO.File.Exists(ProcessingComplete))
                {
                    ProcessedCompleted = true;
                    break;
                }
            }

            if ( ProcessedCompleted )
            {  //copy over files - hierarchy, datamodel, map and concept
                string HierarchyLocal = System.IO.Path.Combine(WorkingDirectory, ReductionConfiguration.SelectionSets[0].HierarchyFile);
                string HierarchyRemote = System.IO.Path.Combine(NewReductionFolder, ReductionConfiguration.SelectionSets[0].HierarchyFile);
                System.IO.File.Delete(HierarchyLocal);
                if (System.IO.File.Exists(HierarchyRemote))
                    System.IO.File.Copy(HierarchyRemote, HierarchyLocal);
                else
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Reduction server did not produce a hierarchy file - required for processing");
                    return false;
                }
                //string DataModelLocal = System.IO.Path.Combine(WorkingDirectory, ReductionConfiguration.SelectionSets[0].DataModel);
                //string DataModelRemote = System.IO.Path.Combine(NewReductionFolder, ReductionConfiguration.SelectionSets[0].DataModel);

                string ConceptLocal = System.IO.Path.Combine(WorkingDirectory, ReductionConfiguration.SelectionSets[0].ConceptFile);
                string ConceptRemote = System.IO.Path.Combine(NewReductionFolder, ReductionConfiguration.SelectionSets[0].ConceptFile);
                System.IO.File.Delete(ConceptLocal);
                if (System.IO.File.Exists(ConceptRemote))
                    System.IO.File.Copy(ConceptRemote, ConceptLocal);
                else
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Reduction server did not produce a concept file - required for processing");
                    return false;
                }

                string MapLocal = System.IO.Path.Combine(WorkingDirectory, ReductionConfiguration.SelectionSets[0].MapFile);
                string MapRemote = System.IO.Path.Combine(NewReductionFolder, ReductionConfiguration.SelectionSets[0].MapFile);
                System.IO.File.Delete(MapLocal);
                if (System.IO.File.Exists(MapRemote))
                    System.IO.File.Copy(MapRemote, MapLocal);
                else
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Reduction server did not produce a map file - required for processing");
                    return false;  
                }
                return true;

            }
            else
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Waiting on remote reduction server timed out");
                return false;
            }
           

        }

        /// <summary>
        /// Move the hierarchy, concept, datamodel and map file back to working area
        /// </summary>
        /// <param name="WorkingDirectory"></param>
        /// <param name="ReductionConfiguration"></param>
        /// <returns></returns>
        public bool CopyFilesToWorkingDir( string WorkingDirectory, MillimanCommon.ReduceConfig ReductionConfiguration)
        {
            try
            {
                string RootReductionFolder = System.Configuration.ConfigurationManager.AppSettings["ReductionServerRoot"];
                string ReductionMasterQVW = System.IO.Path.Combine(RootReductionFolder, ReductionConfiguration.UniqueID, ReductionConfiguration.MasterQVW);

                string SourceHierarchy = ReductionMasterQVW.ToLower().Replace(".qvw", ".hierarchy_0");
                string SourceConcept = ReductionMasterQVW.ToLower().Replace(".qvw", ".concept_0");
                string SourceDataModel = ReductionMasterQVW.ToLower().Replace(".qvw", ".datamodel_0");
                string SourceMap = ReductionMasterQVW.ToLower().Replace(".qvw", ".map_0");

                string DestHierarchy = System.IO.Path.Combine(WorkingDirectory, System.IO.Path.GetFileName(SourceHierarchy));
                string DestConcept = System.IO.Path.Combine(WorkingDirectory, System.IO.Path.GetFileName(SourceConcept));
                string DestDataModel = System.IO.Path.Combine(WorkingDirectory, System.IO.Path.GetFileName(SourceDataModel));
                string DestMap = System.IO.Path.Combine(WorkingDirectory, System.IO.Path.GetFileName(SourceMap));

                if (System.IO.File.Exists(SourceHierarchy) == false)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Missing generated hierarchy file from - '" + SourceHierarchy + "'");
                    return false;
                }
                if (System.IO.File.Exists(SourceConcept) == false)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Missing generated concept file from - '" + SourceConcept + "'");
                    return false;
                }
                if (System.IO.File.Exists(SourceDataModel) == false)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Missing generated data model file from - '" + SourceDataModel + "'");
                    return false;
                }
                if (System.IO.File.Exists(SourceMap) == false)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Missing generated map file from - '" + SourceHierarchy + "'");
                    return false;
                }

                System.IO.File.Copy(SourceHierarchy,DestHierarchy, true);
                System.IO.File.Copy(SourceConcept, DestConcept, true);
                System.IO.File.Copy(SourceDataModel, DestDataModel, true);
                System.IO.File.Copy(SourceMap, DestMap, true);

                return true;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified", ex);
            }
            return false;
        }

        /// <summary>
        /// Convience method used to retrieve the basic QVW files from the NEW master QVW such that automatic inclusion can be processed
        /// </summary>
        /// <param name="WorkingDirectory"></param>
        /// <param name="ProjectSettings"></param>
        /// <returns></returns>
        public MillimanCommon.ReduceConfig NewMasterQVWParameters(string WorkingDirectory, ProjectSettingsExtension ProjectSettings )
        {
            //should only be 1 item in list of QVWS, if more, then an error
            List<string> NewMasterQVWList = System.IO.Directory.GetFiles(WorkingDirectory, "*.qvw", SearchOption.TopDirectoryOnly).ToList();
            if (NewMasterQVWList.Count() != 1)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Only 1 QVW should exist in the working directory, but " + NewMasterQVWList.Count().ToString() + " exit.");
                return null;
            }
            //do not include paths, they should all be local on the reduction server

            //string NewMasterQVW = System.IO.Path.Combine(WorkingDirectory, ProjectSettings.QVName + ".qvw");
            //string NewHierarchyFile = System.IO.Path.Combine(WorkingDirectory, ProjectSettings.QVName + ".hierarchy_0");
            //string NewMapFile = System.IO.Path.Combine(WorkingDirectory, ProjectSettings.QVName + ".map_0");
            //string NewConceptFile = System.IO.Path.Combine(WorkingDirectory, ProjectSettings.QVName + ".concept_0");
            //string NewDataModelFile = System.IO.Path.Combine(WorkingDirectory, ProjectSettings.QVName + ".datamodel_0");

            //only get the file, since the configuration file when writtent to disk should be all relative to configuration file location
            string NewMasterQVW = System.IO.Path.GetFileName(NewMasterQVWList[0]);
            string NewHierarchyFile = NewMasterQVW.Replace(".qvw",".hierarchy_0");
            string NewMapFile =  NewMasterQVW.Replace(".qvw", ".map_0");
            string NewConceptFile = NewMasterQVW.Replace(".qvw", ".concept_0");
            string NewDataModelFile = NewMasterQVW.Replace(".qvw", ".datamodel_0");


            MillimanCommon.ReduceConfig ReductionConfig = ReductionConfigurationFromSelectionFile(NewMasterQVW,
                                                                                                NewMasterQVW.Replace(".qvw", ".log"),
                                                                                                string.Empty,
                                                                                                string.Empty,
                                                                                                NewHierarchyFile,
                                                                                                NewMapFile,
                                                                                                NewDataModelFile,
                                                                                                NewConceptFile,
                                                                                                null,
                                                                                                null,
                                                                                                false,
                                                                                                null,
                                                                                                null,
                                                                                                string.Empty,
                                                                                                string.Empty,
                                                                                                false);
            return ReductionConfig;
        }

        /// <summary>
        /// Create a reduction configuration file for use with the reduction server, if used to reduce, this can
        /// process a selections file and automatically add to data
        /// </summary>
        /// <param name="MasterQVWName"></param>
        /// <param name="RequestedMasterStatusLog"></param>
        /// <param name="RequestedReducedQVWName"></param>
        /// <param name="RequestedReducedQVWStatusLog"></param>
        /// <param name="RequestedHierarchyFilename"></param>
        /// <param name="RequestedMapFilename"></param>
        /// <param name="RequestedDataModelFilename"></param>
        /// <param name="RequestedConceptFilename"></param>
        /// <param name="RequestedIgnoreTables"></param>
        /// <param name="RequestedDropTables"></param>
        /// <param name="RequestedDropDissociatedTables"></param>
        /// <param name="RequestedUniqueValuesFromMasterColumns"></param>
        /// <param name="RequestedUniqueValuesFromReducedColumns"></param>
        /// <param name="SelectionsFile"></param>
        /// <param name="DataModelFile"></param>
        /// <param name="SelectionCriteriaProvided"></param>
        /// <returns></returns>
        public MillimanCommon.ReduceConfig ReductionConfigurationFromSelectionFile(  string MasterQVWName,
                                                                                     string RequestedMasterStatusLog,
                                                                                     string RequestedReducedQVWName,
                                                                                     string RequestedReducedQVWStatusLog,
                                                                                     string RequestedHierarchyFilename,
                                                                                     string RequestedMapFilename,
                                                                                     string RequestedDataModelFilename,
                                                                                     string RequestedConceptFilename,
                                                                                     List<string> RequestedIgnoreTables,
                                                                                     List<string> RequestedDropTables,
                                                                                     bool   RequestedDropDissociatedTables,
                                                                                     List<MillimanCommon.NVPair> RequestedUniqueValuesFromMasterColumns,
                                                                                     List<MillimanCommon.NVPair> RequestedUniqueValuesFromReducedColumns,
                                                                                     string SelectionsFile, 
                                                                                     string DataModelFile,
                                                                                     bool   SelectionCriteriaProvided = true )
        {
            if (SelectionCriteriaProvided)  //dont check, we don't care about selections for this processing
            {
                if (File.Exists(SelectionsFile) == false)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Missing selection file '" + SelectionsFile + "'");
                    return null;
                }
                if (File.Exists(DataModelFile) == false)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Missing data model file '" + DataModelFile + "'");
                    return null;
                }
            }

            MillimanCommon.ReduceConfig RC = new MillimanCommon.ReduceConfig();
            RC.MasterQVW = MasterQVWName;
            RC.MasterStatusLog = RequestedMasterStatusLog;
            RC.SelectionSets = new List<MillimanCommon.ReductionSettings>();

            Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
            List<string> Selections = new List<string>();
            List<string> DataModel = new List<string>();
            if ( System.IO.File.Exists(SelectionsFile))
                Selections = SS.Deserialize(SelectionsFile) as List<string>;
            if ( System.IO.File.Exists(DataModelFile))
                DataModel = SS.Deserialize(DataModelFile) as List<string>;

            MillimanCommon.ReductionSettings RS = new MillimanCommon.ReductionSettings();
            RS.ReducedQVWName = RequestedReducedQVWName;
            RS.QVWStatusLog = RequestedReducedQVWStatusLog;
            RS.HierarchyFile = RequestedHierarchyFilename;
            RS.MapFile = RequestedMapFilename;
            RS.DataModelFile = RequestedDataModelFilename;
            RS.ConceptFile = RequestedConceptFilename;
            RS.IgnoreTables = RequestedIgnoreTables;
            RS.DropTables = RequestedDropTables;
            RS.DropDisassoicatedDataModelTables = RequestedDropDissociatedTables;
            RS.UniqueValuesFromMasterQVWColumns = RequestedUniqueValuesFromMasterColumns;
            RS.UniqueValuesFromReducedQVWColumns = RequestedUniqueValuesFromReducedColumns;
            RS.SelectionCriteria = new List<MillimanCommon.NVPair>();

            //the loop below will index and create the selection criteria in the correct order, all selections must be made
            //from top level(column) to lower level(column) or the QV reduction engine will fail when reducing
            if (SelectionCriteriaProvided)
            {
                int LevelCounter = 0;
                List<string> LevelItem = DataModelFieldsByLevel(LevelCounter, DataModel);
                while (LevelItem.Count() != 0)  //no items to process
                {
                    List<string> SelectionItem = SelectionFieldsByLevel(LevelCounter, Selections);
                    foreach (string LevelItemToken in LevelItem)
                    {
                        foreach (string SelectionItemToken in SelectionItem)
                        {
                            RS.SelectionCriteria.Add(new MillimanCommon.NVPair(LevelItemToken, SelectionItemToken));
                        }
                    }
                    LevelCounter++;
                    LevelItem = DataModelFieldsByLevel(LevelCounter, DataModel);
                }
            }
            RC.SelectionSets.Add(RS);
            return RC;
        }
    }
}