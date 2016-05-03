using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanCommon
{
    public class QVWReportGenerator
    {
        private bool Exists(List<string> TreeItems, MillimanCommon.MillimanTreeNode MTree)
        {
            List<string> DataModelFields = MTree.FindDataModelFieldNames(TreeItems);
            if ((DataModelFields == null) || (DataModelFields.Count == 0))
                return false;
            return true;
        }

        /// <summary>
        /// loop over new tree and locate items not found in the old tree, and add to new item report - this version has be extracted from the project
        /// management console
        /// TODO: PMC should be refactored to use THESE methods, instead of version embeded in code file ComplexUpload/ReportGenerator.cs
        /// </summary>
        /// <param name="CurrentHierTree"></param>
        /// <param name="NewHierTree"></param>
        /// <param name="PathItems"></param>
        public void GenerateNewItemsReport(MillimanCommon.MillimanTreeNode CurrentHierTree, MillimanCommon.MillimanTreeNode NewHierTree, QVWReportBank Reports, List<string> PathItems = null)
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
                GenerateNewItemsReport(CurrentHierTree, KVP.Value, Reports, PathItems);
            }
            if (PathItems.Count > 0)
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
        
        /// <summary>
        /// - this version has be extracted from the project
        /// management console
        /// TODO: PMC should be refactored to use THESE methods, instead of version embeded in code file ComplexUpload/ReportGenerator.cs
        /// </summary>
        /// <param name="New"></param>
        /// <param name="QualifiedProjectFile"></param>
        /// <param name="Reports"></param>
        public void GenerateNotSelectable(MillimanCommon.MillimanTreeNode New, string QualifiedProjectFile, QVWReportBank Reports)
        {
            //load all selection files and find nodes in new tree
            string QVWPath = System.IO.Path.GetDirectoryName(QualifiedProjectFile);
            ProjectSettings PS = ProjectSettings.Load(QualifiedProjectFile);
            string ReducedQVWsPath = System.IO.Path.Combine(QVWPath, "ReducedUserQVWs");
            string[] Selections = System.IO.Directory.GetFiles(ReducedQVWsPath, PS.QVName + ".selections", System.IO.SearchOption.AllDirectories);

            List<string> MissingSelections = new List<string>();
            List<string> CurrentSelections = new List<string>();

            //get the concept file ready to load
            List<string> Concepts = new List<string>();
            string ConceptFile = System.IO.Path.Combine(QVWPath, System.IO.Path.GetFileNameWithoutExtension(QualifiedProjectFile) + ".concept_0");
            if (System.IO.File.Exists(ConceptFile))
            {
                string ConceptFileText = System.IO.File.ReadAllText(ConceptFile);
                Concepts = ConceptFileText.Split(new char[] { '|' }).ToList();
            }

            foreach (string SelectionFile in Selections)
            {
                string UserName = string.Empty;
                string[] Tokens = SelectionFile.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                UserName = MillimanCommon.Utilities.ConvertHexToString(Tokens[Tokens.Length - 2]);

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
    
        /// <summary>
        /// Find the master status log and add content to audit file
        /// </summary>
        /// <param name="WorkingDirectory"></param>
        /// <param name="Reports"></param>
        public void MasterLogToAudit(string WorkingDirectory, QVWReportBank Reports)
        {
            QVWReportBank.AuditClass AC = new QVWReportBank.AuditClass("Log4Net results here, but not piped");
            Reports.AuditList.Add(AC);

            //all configuration files should have the same master log file
            //string[] ConfigurationFiles = System.IO.Directory.GetFiles(WorkingDirectory, "*.config", System.IO.SearchOption.TopDirectoryOnly);
            //if ( (ConfigurationFiles == null) || (ConfigurationFiles.Length == 0) )
            //{
            //    QVWReportBank.AuditClass AC = new QVWReportBank.AuditClass("Master status log not available");
            //    Reports.AuditList.Add(AC);
            //}
            //else
            //{
            //    MillimanCommon.ReduceConfig RC = MillimanCommon.ReduceConfig.Deserialize(ConfigurationFiles[0]);
            //    string[] AllLines = System.IO.File.ReadAllLines(RC.MasterStatusLog);
            //    foreach (string Line in AllLines)
            //    {
            //        QVWReportBank.AuditClass AC = new QVWReportBank.AuditClass(Line);
            //        Reports.AuditList.Add(AC);
            //    }
            //}
        }
    
        public void AutoInclusionProcessor( string WorkingDirectory, MillimanCommon.ProjectSettings PS, QVWReportBank Reports)
        {
            //first look to see if auto inclusion is in effect by looking at autoinclude bool in project
            if ( PS.AutomaticInclusion == false )
            {
                return;  //nothing to report here
            }

            string OriginalSelectionsDirectory = System.IO.Path.Combine(PS.AbsoluteProjectPath, "ReducedUserQVWs");
            string WorkingSelectionsDirectory = System.IO.Path.Combine(WorkingDirectory, "ReducedUserQVWs");
            if ( System.IO.Directory.Exists(OriginalSelectionsDirectory) == false )
            {
                MillimanCommon.Report.Log(Report.ReportType.Error, "Missing selections directory - " + OriginalSelectionsDirectory);
                return;
            }
            if (System.IO.Directory.Exists(WorkingSelectionsDirectory) == false)
            {
                MillimanCommon.Report.Log(Report.ReportType.Error, "Missing selections directory - " + WorkingSelectionsDirectory);
                return;
            }
            string[] OriginalSelections = System.IO.Directory.GetFiles(OriginalSelectionsDirectory, "*.selections", System.IO.SearchOption.AllDirectories);
            string[] WorkingSelections = System.IO.Directory.GetFiles(WorkingSelectionsDirectory, "*.selections", System.IO.SearchOption.AllDirectories);

            if ( OriginalSelections.Length != WorkingSelections.Length )
            {
                MillimanCommon.Report.Log(Report.ReportType.Error, "Original and working projects contain a different number of selections files? - " + WorkingSelectionsDirectory);
                return;
            }
            string AccountName = string.Empty;
            foreach( string OriginalSelection in OriginalSelections )
            {
                string WorkingSelection = FindMatchingSelectionFile(OriginalSelection, WorkingSelections, out AccountName);
                if ( string.IsNullOrEmpty(WorkingSelection) == true )
                {
                    MillimanCommon.Report.Log(Report.ReportType.Error, "Could not find matching selection file in working set for original " + OriginalSelection);
                }
                else
                {
                    Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                    List<string> OriginalUserSelections = SS.Deserialize(OriginalSelection) as List<string>;
                    List<string> WorkingUserSelections = SS.Deserialize(WorkingSelection) as List<string>;

                    IEnumerable<string> Difference = WorkingUserSelections.Except(OriginalUserSelections);
                    if ((Difference == null) || (Difference.Count() == 0))
                    {
                        Reports.AddItemToList(new QVWReportBank.AutoInclusionClass(AccountName, "", "", PS.QVName, "[NODIFFERENCE]"));
                    }
                    else
                    {

                        string ConceptFile = System.IO.Path.Combine(PS.AbsoluteProjectPath, PS.ProjectName + ".concept_0");
                        string ConceptFileContents = System.IO.File.ReadAllText(ConceptFile);
                        string[] ConceptTokens = ConceptFileContents.Split(new char[] { '|' });
                        string Concept = ConceptTokens.Last();

                        //we don't look up the data model field, not sure it is really necessary plus is complicated, if needed
                        //parse the "*.datamodel_0" file in the directory, it contains the data model fields
                        string DataModelField = string.Empty;

                        string FirstItem = Difference.First();
                        string[] FirstItemTokens = FirstItem.Split( new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries );
                        string QVW = FirstItemTokens[0];
                        string Field = FirstItemTokens[FirstItemTokens.Length - 2]; //get next to last token for field name
                        string AllDifferences = string.Empty;
                        foreach (string Diff in Difference)
                        {
                            if (string.IsNullOrEmpty(AllDifferences) == false)
                                AllDifferences += "|";
                            AllDifferences += Diff;
                        }
                        //we are not lookup up the 
                        Reports.AddItemToList(new QVWReportBank.AutoInclusionClass(AccountName, DataModelField, Concept, QVW, AllDifferences));
                    }
                }

            }


        }

        /// <summary>
        /// Find the users selection item in the list, used to match the old selection to the new selections based on what
        /// the auto inclusion processor set
        /// </summary>
        /// <param name="SearchFile"></param>
        /// <param name="FileList"></param>
        /// <param name="AccountName"></param>
        /// <returns></returns>
        private string FindMatchingSelectionFile( string SearchFile, string[] FileList, out string AccountName)
        {
            AccountName = string.Empty;
            string SearchFileDirectory = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(SearchFile));
            string SearchListFile = string.Empty;
            foreach( string FileItem in FileList)
            {
                SearchListFile = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(FileItem));
                if ( string.Compare( SearchListFile, SearchFileDirectory, true) == 0 )
                {
                    AccountName = MillimanCommon.Utilities.ConvertHexToString(SearchListFile);
                    return SearchListFile;
                }
            }
            return "";
        }

        /// <summary>
        /// TODO: looks up each account and associated to report with success/fail status
        /// </summary>
        /// <param name="WorkingDirectory"></param>
        /// <param name="PS"></param>
        /// <param name="Reports"></param>
        public void ProcessingStatusProcessor( string WorkingDirectory, MillimanCommon.ProjectSettings PS, QVWReportBank Reports, Dictionary<string,List<string>> ReportToUserMap)
        {
            string ConfigurationDirectory = System.IO.Path.Combine(WorkingDirectory, "configurations");
            string[] MasterLogFile = System.IO.Directory.GetFiles(ConfigurationDirectory, "*_master.log", System.IO.SearchOption.TopDirectoryOnly);
            if (MasterLogFile.Count() == 0)
            {
                Reports.AddItemToList(new MillimanCommon.QVWReportBank.ProcessingStatusClass("[none]", "[none]", "No master log file returned from reduction server"));
                return;
            }
            else if (MasterLogFile.Count() > 1)
            {
                Reports.AddItemToList(new MillimanCommon.QVWReportBank.ProcessingStatusClass("[none]", "[none]", "Multiple master ambigious log files returned from reduction server"));
                return;
            }
            string[] MasterLogContents = System.IO.File.ReadAllLines(MasterLogFile[0]);
            foreach( string LogItem in MasterLogContents )
            {
                string[] Tokens = LogItem.Split(new char[] { '|' });
                string ConfigFile = System.IO.Path.Combine(WorkingDirectory, "configurations", Tokens[0]);
                if ( System.IO.File.Exists(ConfigFile))
                {
                    MillimanCommon.ReduceConfig RC = MillimanCommon.ReduceConfig.Deserialize(ConfigFile);
                    string ReducedName = RC.SelectionSets[0].ReducedQVWName;
                    if ( ReportToUserMap.ContainsKey(Tokens[0]))
                    {
                        foreach( string UserAccount in ReportToUserMap[Tokens[0]] )
                        {
                            Reports.AddItemToList(new MillimanCommon.QVWReportBank.ProcessingStatusClass(UserAccount, ReducedName, Tokens[1]));
                        }
                    }
                }
                else
                {
                    Reports.AddItemToList(new MillimanCommon.QVWReportBank.ProcessingStatusClass("[none]", "[none]", "Missing configuration file - " + Tokens[0]));
                }
            }


            ////OLD code
            // contain if a reduced report is reason == "Reduced - Available","Failed","Copied - Available","Starting reduction"
            //get all configuarion files and parse for status
            //string[] ConfigFiles = System.IO.Directory.GetFiles(WorkingDirectory, "*.config", System.IO.SearchOption.AllDirectories);
            //foreach( string ConfigFile in ConfigFiles )
            //{
            //    MillimanCommon.ReduceConfig RC = MillimanCommon.ReduceConfig.Deserialize(ConfigFile);
            //    foreach( MillimanCommon.ReductionSettings RS in RC.SelectionSets)
            //    {
            //        if ( System.IO.File.Exists( RS.QVWStatusLog ) == true )
            //        {
            //            string Contents = System.IO.File.ReadAllText(RS.QVWStatusLog);
            //            if ( Contents.ToLower().Contains("failed") == true)
            //            {
            //                Reports.AddItemToList(new MillimanCommon.QVWReportBank.ProcessingStatusClass("???", RC.MasterQVW, "failed"));
            //            }
            //            else
            //            {
            //                Reports.AddItemToList(new MillimanCommon.QVWReportBank.ProcessingStatusClass("???", RS.ReducedQVWName, "success"));
            //            }
            //        }
            //    }
            //}
        }
    }
}
