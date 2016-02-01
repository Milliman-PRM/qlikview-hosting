using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanCommon
{
    public class QVWReportBankProcessor
    {
        
        public QVWReportBank ReportBank { get; set; }

        public QVWReportBankProcessor(string QualafiedProjectPath)
        {
            ReportBank = new QVWReportBank(QualafiedProjectPath);
        }

        public bool SummaryReductionStats(out int TotalUsers, out int ReducedUsers)
        {
            TotalUsers = 0;
            ReducedUsers = 0;
            if (ReportBank.IsInError == false)
            {
                List<string> SuccessfulReductionUsers = new List<string>();
                List<string> TotalReductionUsers = new List<string>();
                foreach (QVWReportBank.ProcessingStatusClass PSC in ReportBank.ProcessingStatusList)
                {
                    if (PSC.Reason.ToLower().IndexOf("available") != -1)
                    {
                        if (SuccessfulReductionUsers.Contains(PSC.UserName) == false)
                        {
                            ReducedUsers++;
                            SuccessfulReductionUsers.Add(PSC.UserName);
                        }
                    }

                    if (TotalReductionUsers.Contains(PSC.UserName) == false)
                    {
                        TotalReductionUsers.Add(PSC.UserName);
                    }
                }

                TotalUsers = TotalReductionUsers.Count;
                return true;
            }

            return false;
        }

        public Dictionary<string, List<QVWReportBank.NewItemClass>> NewItemsGroupByConcept()
        {
            Dictionary<string, List<QVWReportBank.NewItemClass>> NewItemsByConcept = new Dictionary<string, List<QVWReportBank.NewItemClass>>();
            foreach (QVWReportBank.NewItemClass IC in ReportBank.NewItemList)
            {
                //VWN get rid of completed items
                //ignore any items with concept completed, just a sofware artifact marker
                if (string.Compare(IC.ItemConceptName, "completed", 0) != 0)
                {
                    if (NewItemsByConcept.ContainsKey(IC.ItemConceptName) == true)
                    {
                        NewItemsByConcept[IC.ItemConceptName].Add(IC);
                    }
                    else
                    {
                        NewItemsByConcept.Add(IC.ItemConceptName, new List<QVWReportBank.NewItemClass>());
                        NewItemsByConcept[IC.ItemConceptName].Add(IC);
                    }
                }
            }
            return NewItemsByConcept;
        }

        public Dictionary<string, List<QVWReportBank.NotSelectableClass>> NotSelectableByUser()
        {
            Dictionary<string, List<QVWReportBank.NotSelectableClass>> NotSelectableByUser = new Dictionary<string, List<QVWReportBank.NotSelectableClass>>();
            foreach (QVWReportBank.NotSelectableClass NS in ReportBank.NotSelectableList)
            {
                //ignore any items with concept completed, just a sofware artifact marker
                if (string.Compare(NS.ConceptFieldName, "completed", 0) != 0)
                {
                    if (NotSelectableByUser.ContainsKey(NS.UserName) == true)
                    {
                        NotSelectableByUser[NS.UserName].Add(NS);
                    }
                    else
                    {
                        NotSelectableByUser.Add(NS.UserName, new List<QVWReportBank.NotSelectableClass>());
                        NotSelectableByUser[NS.UserName].Add(NS);
                    }
                }
            }
            return NotSelectableByUser;
        }

        public List<QVWReportBank.AuditClass> AuditData()
        {
            return ReportBank.AuditList;
        }

        public bool ReductionStatus(out List<string> SuccessfulReductionUsers, out List<string> FailedReductionUsers)
        {
            SuccessfulReductionUsers = new List<string>();
            FailedReductionUsers = new List<string>();
            //find everyone who succeded
            foreach (QVWReportBank.ProcessingStatusClass PS in ReportBank.ProcessingStatusList)
            {
                if (PS.Reason.ToLower().IndexOf("available") != -1)
                {
                    if (SuccessfulReductionUsers.Contains(PS.UserName) == false)
                        SuccessfulReductionUsers.Add(PS.UserName);
                }
            }
            //if you are not successful, then you failed
            foreach (QVWReportBank.ProcessingStatusClass PS in ReportBank.ProcessingStatusList)
            {
                if (SuccessfulReductionUsers.Contains(PS.UserName) == false)
                    FailedReductionUsers.Add(PS.UserName);
            }

            return true;
            
        }

        public class UserDownloadItems
        {
            public string ItemName { get; set; }
            public string Status { get; set; }  //Unchanged, Modified, New, Deleted
            public string Timestamp { get; set; }
            public UserDownloadItems(string _ItemName, string _Status, string _Timestamp)
            {
                ItemName = _ItemName;
                Status = _Status;
                Timestamp = _Timestamp;
            }
        }

        public List<UserDownloadItems> UserDownloadStatus(ProjectSettings PS)
        {
            List<string> DistinctFileList = new List<string>();
            List<UserDownloadItems> UserDownloads = new List<UserDownloadItems>();
            string QualafiedProjectPath = System.IO.Path.Combine(PS.LoadedFromPath, PS.ProjectName + "_Data");
            if (System.IO.Directory.Exists(QualafiedProjectPath))
            {
                string[] AllFiles = System.IO.Directory.GetFiles(QualafiedProjectPath);
                foreach (string file in AllFiles)
                {
                    if ((file.ToLower().EndsWith(".description") == false) && (file.ToLower().EndsWith(".description_new") == false))
                    {
                        string Timestamp = new System.IO.FileInfo(file).CreationTime.ToString();
                        string FN = System.IO.Path.GetFileName(file);
                        string DistinctFN = FN;
                        if (DistinctFN.EndsWith("_new"))
                            DistinctFN = DistinctFN.Replace("_new", "");
                        if (DistinctFileList.Contains(DistinctFN) == false)
                        {
                            DistinctFileList.Add(DistinctFN);
                            string Status = "Unchanged";
                            if (FN != DistinctFN)
                            {
                                //if original file exists it's a mod, otherwise it's new
                                string OrgFile = file.Replace("_new", "");
                                if (System.IO.File.Exists(OrgFile) == true)
                                    Status = "Modified";
                                else
                                    Status = "New";
                            }
                            UserDownloads.Add(new UserDownloadItems(DistinctFN, Status, Timestamp));
                        }
                        else if (FN != DistinctFN)  //must be a modified item
                        {
                            foreach (UserDownloadItems UDI in UserDownloads)
                            {
                                if (string.Compare(UDI.ItemName, DistinctFN, true) == 0)
                                {
                                    UDI.Status = "Modified";
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return UserDownloads;
        }

        public List<List<string>> SelectedItems(MillimanCommon.ProjectSettings PS, out List<string> ColumnNames)
        {
            //we need to read the concept names for headers on the report
            //projectname.concept_0  a file for each hierarchy
            //should have the same number of levels as the depth of each selection file
            //VWN we only support 1 concept file for now

            ColumnNames = new List<string>();
            string ConceptFile = System.IO.Path.Combine(PS.LoadedFromPath, PS.ProjectName + ".concept_0");
            if (System.IO.File.Exists(ConceptFile))
            {
                ColumnNames.Add("Report Name");
                ColumnNames.Add("Account Name");
                string ConceptFileContenets = System.IO.File.ReadAllText(ConceptFile);
                string[] Concepts = ConceptFileContenets.Split( new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string Concept in Concepts)
                    ColumnNames.Add(Concept);

            }
            //ColumnNames.Add("User");
            //ColumnNames.Add("Group");
            //ColumnNames.Add("Practice");
            //ColumnNames.Add("Physician");
            List<List<string>> ContainerList = new List<List<string>>();

            string ReducedQVWConstant = "reduceduserqvws";
            string ReducedQVWFilePath = System.IO.Path.Combine(PS.LoadedFromPath, ReducedQVWConstant);
            string[] Selections = System.IO.Directory.GetFiles(ReducedQVWFilePath,  PS.QVName + ".selections", System.IO.SearchOption.AllDirectories);
            foreach (string Selection in Selections)
            {
                //username is encoded in directory name
                string UserName = System.IO.Path.GetDirectoryName(Selection);
                UserName = UserName.Substring(UserName.IndexOf(ReducedQVWConstant) + ReducedQVWConstant.Length + 1);
                UserName = MillimanCommon.Utilities.ConvertHexToString(UserName);
                Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                List<string> Sels = SS.Deserialize(Selection) as List<string>;
                for( int Index = 0; Index < Sels.Count; Index++ )
                {
                    string[] SelectionTokens = Sels[Index].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    ContainerList.Add(new List<string>());
                    ContainerList[ContainerList.Count-1].Add(PS.ProjectName);
                    ContainerList[ContainerList.Count - 1].Add(UserName);

                    for (int TokenIndex = 1; TokenIndex < SelectionTokens.Count(); TokenIndex++)
                    {
                        ContainerList[ContainerList.Count - 1].Add(SelectionTokens[TokenIndex]);
                    }
                }
            }

            //    ContainerList.Add(new List<string>());
            //    ContainerList[0].Add("DisneyProject");
            //    ContainerList[0].Add("Cruella.Deville@dissney.com");
            //    ContainerList[0].Add("XPatho Org");
            //    ContainerList[0].Add("Cleveland Division");
            //    ContainerList[0].Add("Dr Jones");

            //    ContainerList.Add(new List<string>());
            //    ContainerList[1].Add("");
            //    ContainerList[1].Add("");
            //    ContainerList[1].Add("");
            //    ContainerList[1].Add("");
            //    ContainerList[1].Add("Dr Phillips");

            //    ContainerList.Add(new List<string>());
            //    ContainerList[2].Add("");
            //    ContainerList[2].Add("");
            //    ContainerList[2].Add("");
            //    ContainerList[2].Add("");
            //    ContainerList[2].Add("Dr Mo");

            //    ContainerList.Add(new List<string>());
            //    ContainerList[3].Add("");
            //    ContainerList[3].Add("");
            //    ContainerList[3].Add("CardioX");
            //    ContainerList[3].Add("Cincinati");
            //    ContainerList[3].Add("Dr Rowland");

            return ContainerList;
        }

        public List<List<string>> UserDownloads(MillimanCommon.ProjectSettings PS, out List<string> ColumnNames)
        {
            List<List<string>> DownloadItems = new List<List<string>>();
            ColumnNames = new List<string>();
            ColumnNames.Add("Report Name");
            ColumnNames.Add("Account Name");
            ColumnNames.Add("Downloadable Item");
            string QualifiedQVW = System.IO.Path.Combine(PS.VirtualDirectory, PS.QVName + ".qvw");
            List<CustomUserDownloads.CustomDownloads> Downloads = MillimanCommon.CustomUserDownloads.GetInstance().FindItemsForQVW(QualifiedQVW);
            foreach (CustomUserDownloads.CustomDownloads CD in Downloads)
            {
                DownloadItems.Add(new List<string>());
                DownloadItems[DownloadItems.Count - 1].Add(System.IO.Path.GetFileNameWithoutExtension( CD.VirtualReport ));
                DownloadItems[DownloadItems.Count - 1].Add(CD.AccountName);
                DownloadItems[DownloadItems.Count - 1].Add(System.IO.Path.GetFileNameWithoutExtension(CD.VirtualItemPath));
            }
            return DownloadItems;

        }

   
    }
}
