using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Web.UI;

namespace MillimanCommon
{
    public class TreeToFileUtilities
    {
        public class KeyValuePair
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public int Level { get; set; }

            public KeyValuePair(string _Key, string _Value, int _Level)
            {
                Key = _Key;
                Value = _Value;
                Level = _Level;
            }
        }
        public class ReductionSelections
        {
            public string QualafiedQVW { get; set; }
            public List<KeyValuePair> KeyValuePairs { get; set; }

            public ReductionSelections()
            {
                KeyValuePairs = new List<KeyValuePair>();
            }

            public string WriteSelectionsToTemp()
            {
                string TempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.Guid.NewGuid().ToString("N") + ".selections");
                Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                SS.Serialize(KeyValuePairs, TempFile);
                return TempFile;
            }
        }

        /// <summary>
        /// We need to sort by level, so when the QV publisher runs it gets items in the correct
        /// order to to a reduction across multiple nodes
        /// </summary>
        /// <param name="TreeNodes"></param>
        /// <returns></returns>
        private List<RadTreeNode> SortByLevel(List<RadTreeNode> TreeNodes)
        {
            List<RadTreeNode> RTNList = new List<RadTreeNode>();
            int Level = 0;
            while (RTNList.Count != TreeNodes.Count)
            {
                foreach (RadTreeNode RTN in TreeNodes)
                {
                    if (RTN.Level == Level)
                        RTNList.Add(RTN);
                }
                Level++;
            }
            return RTNList;
        }
        public List<ReductionSelections> GetAccessSelectionsForReduction(RadTreeView RTV, out bool AllNodesSelected)
        {
            List<ReductionSelections> ItemsSelected = new List<ReductionSelections>();
            AllNodesSelected = RTV.GetAllNodes().Count == RTV.CheckedNodes.Count;
            List<RadTreeNode> SortedList = SortByLevel(RTV.CheckedNodes as List<RadTreeNode>);
            foreach (RadTreeNode RTN in SortedList)
            {
                if (RTN.Level == 0)
                {
                    ItemsSelected.Add(new ReductionSelections());
                    ItemsSelected[ItemsSelected.Count - 1].QualafiedQVW = RTN.Value;
                }
                else
                {
                    string[] ValueTokens = RTN.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string VToken in ValueTokens)
                    {
                        ItemsSelected[ItemsSelected.Count - 1].KeyValuePairs.Add(new KeyValuePair(VToken, RTN.Text, RTN.Level));
                    }
                }
            }

            return ItemsSelected;
        }


        //public List<string> GetAccessSelectionsForReduction(RadTreeView RTV)
        //{
        //    List<string> ItemsSelected = new List<string>();
        //    int Level = 0;

        //    while (ItemsSelected.Count != RTV.CheckedNodes.Count)
        //    {
        //        foreach (RadTreeNode RTN in RTV.CheckedNodes)
        //        {
        //            if (RTN.Level == Level)
        //                ItemsSelected.Add(RTN.Level.ToString() + "|" + RTN.Value + "|" + RTN.Text);
        //        }
        //        Level++;
        //    }

        //    return ItemsSelected;
        //}

        private List<string> GetAccessSelectionsForFileSave( RadTreeView RTV, out Dictionary<string, List<string>> SaveSelections)
        {

            SaveSelections = new Dictionary<string, List<string>>();

            List<string> ItemsSelected = new List<string>();
            int Level = 0;

            while (ItemsSelected.Count != RTV.CheckedNodes.Count)
            {
                foreach (RadTreeNode RTN in RTV.CheckedNodes)
                {
                    if (RTN.Level == Level)
                        ItemsSelected.Add(RTN.Level.ToString() + "|" + RTN.Value + "|" + RTN.Text);
                }
                Level++;
            }

            //update the save selection list
            string LastQVWRoot = string.Empty;
            foreach (RadTreeNode RTN in RTV.CheckedNodes)
            {
                if (RTN.Level == 0)
                {
                    LastQVWRoot = RTN.Value;
                    SaveSelections.Add(RTN.Value, new List<string>());
                }
                else if ((string.IsNullOrEmpty(LastQVWRoot) == false) && (RTN.Nodes.Count == 0))
                {
                    //SaveSelections[LastQVWRoot].Add(RTN.Level.ToString() + "|" + RTN.Value + "|" + RTN.Text);
                    SaveSelections[LastQVWRoot].Add(RTN.GetFullPath("|"));
                }
            }

            return ItemsSelected;
        }
        private List<string> GetDownloadSelections(RadTreeView RTV, out Dictionary<string, List<string>> SaveSelections)
        {
            SaveSelections = new Dictionary<string, List<string>>();

            List<string> ItemsSelected = new List<string>();
            int Level = 0;
            while (ItemsSelected.Count != RTV.CheckedNodes.Count)
            {
                foreach (RadTreeNode RTN in RTV.CheckedNodes)
                {
                    if (RTN.Level == Level)
                        ItemsSelected.Add(RTN.Level.ToString() + "|" + RTN.Value + "|" + RTN.Text);
                }
                Level++;
            }

            //update the save selection list
            string LastQVWRoot = string.Empty;
            foreach (RadTreeNode RTN in RTV.CheckedNodes)
            {
                if (RTN.Level == 0)
                {
                    LastQVWRoot = RTN.Value;
                    SaveSelections.Add(RTN.Value, new List<string>());
                }
                else if (string.IsNullOrEmpty(LastQVWRoot) == false)
                {
                    SaveSelections[LastQVWRoot].Add(RTN.Level.ToString() + "|" + RTN.Value + "|" + RTN.Text + "|" + RTN.ToolTip);
                    //SaveSelections[LastQVWRoot].Add(RTN.GetFullPath("|") + "|" + RTN.ToolTip);
                }
            }
            return ItemsSelected;
        }

        private bool RequiresDataReduction(string OldSelectionFile, List<string> Selections)
        {
            if (System.IO.File.Exists(OldSelectionFile) == false)
                return true;  //file does not exist, so first time to try selections so must reduce
            Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
            List<string> OldSelections = SS.Deserialize(OldSelectionFile) as List<string>;

            if (OldSelections.Count() != Selections.Count())
                return true;  //list not same length, must reduce

            foreach (string Selection in OldSelections)
            {
                if (Selections.Contains(Selection) == false)
                    return true;  //diferent selection, must reduce
            }
            return false;
        }

        /// <summary>
        /// Added for Issue 1569 - remove the selections file and redirector for user that have no selections made
        /// </summary>
        /// <param name="CurrentProjectSettings">Valid project settings file</param>
        /// <param name="Accounts">A list of all user accounts to remove slections</param>
        private void RemoveSelectionsForUsers(MillimanCommon.ProjectSettings CurrentProjectSettings, List<string> Accounts)
        {
            //strings used to log nice error messages on failure
            string ErrorMsgRoot = "Failed to delete user selections file for project '" + CurrentProjectSettings.OriginalProjectName + "' ";
            string CurrentErrorMsg = ErrorMsgRoot;
            try
            {
                if (Accounts != null)
                {
                    foreach (string AccountName in Accounts)
                    {
                        CurrentErrorMsg = ErrorMsgRoot + " account '" + AccountName + "'";
                        string UserDir = MillimanCommon.ReducedQVWUtilities.GetUserDir(CurrentProjectSettings.AbsoluteProjectPath, AccountName);
                        //clear all files from dir - quick way is delete directory and re-create it
                        if (System.IO.Directory.Exists(UserDir))
                        {
                            System.IO.Directory.Delete(UserDir, true);
                            System.IO.Directory.CreateDirectory(UserDir);
                        }
                    }
                }
            }
            catch (Exception)
            {
                MillimanCommon.Report.Log(Report.ReportType.Error, CurrentErrorMsg);
            }
        }

        public bool SaveSettings( MillimanCommon.ProjectSettings PS,  RadTreeView AccessTree, RadTreeView DownloadsTree, List<string> Accounts, out bool RequiresReduction)
        {
            RequiresReduction = true;

            try
            {

                string MasterQVW = System.IO.Path.Combine(PS.LoadedFromPath, PS.QVName + ".qvw");

                Dictionary<string, List<string>> AccessSaveSelections = null;
                List<string> SelectedItems = GetAccessSelectionsForFileSave(AccessTree, out AccessSaveSelections);

                if ((SelectedItems == null) || (SelectedItems.Count() == 0))
                {  //Per Issue 1569 we no longer require selections, but want to delete any existing selections file

                    RemoveSelectionsForUsers(PS, Accounts);
                    return true;
                }
                    

                //issue reduction on selectedItems - HERE 
                
                foreach (string Account in Accounts)
                {
                    //store access save selections
                    foreach (KeyValuePair<string, List<string>> Selection in AccessSaveSelections)
                    {
                        string SaveTo = MillimanCommon.ReducedQVWUtilities.GetUsersReducedQVWName(Selection.Key, Account);
                        SaveTo = SaveTo.ToLower().Replace(".qvw", ".selections");
                        //call to see if the selections changed on restriction tree, if no - then don't reduce 
                        RequiresReduction = RequiresDataReduction(SaveTo, Selection.Value);

                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(SaveTo));
                        System.IO.File.Delete(SaveTo);
                        Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                        SS.Serialize(Selection.Value, SaveTo);
                    }

                    Dictionary<string, List<string>> DownloadSaveSelections = null;
                    List<string> DownloadItems = GetDownloadSelections(DownloadsTree, out DownloadSaveSelections);
                    //store access save selections
                    foreach (KeyValuePair<string, List<string>> Selection in DownloadSaveSelections)
                    {
                        string SaveTo = MillimanCommon.ReducedQVWUtilities.GetUsersReducedQVWName(Selection.Key, Account);
                        string SaveToDownloads = SaveTo.ToLower().Replace(".qvw", ".downloads");
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(SaveToDownloads));
                        System.IO.File.Delete(SaveToDownloads);
                        Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                        SS.Serialize(Selection.Value, SaveToDownloads);

                        //update custom user downloads
                        MillimanCommon.CustomUserDownloads CUD = MillimanCommon.CustomUserDownloads.GetInstance();
                        CUD.ClearItems(Account, MillimanCommon.QVWUtilities.AbsoluteToVirtualFromQVDocRoot(SaveTo));
                        string Icon;
                        string Mime;
                        string Tooltip;
                        foreach (string SelectedDownloads in Selection.Value)
                        {
                            string[] SDTokens = SelectedDownloads.Split(new char[] { '|' });
                            string DownloadItem = SDTokens[1];
                            string RelativeDownloadItem = MillimanCommon.QVWUtilities.AbsoluteToVirtualFromQVDocRoot(DownloadItem);
                            Icon = MillimanCommon.ImageUtilities.GetIcon(DownloadItem, out Mime);
                            Tooltip = SDTokens[SDTokens.Length - 1];

                            CUD.AddorUpdateItem(Account, MillimanCommon.QVWUtilities.AbsoluteToVirtualFromQVDocRoot(MasterQVW), RelativeDownloadItem, Icon, Mime, Tooltip);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed saving settings", ex);
            }
            return false;
        }
    
    
    
    }
}