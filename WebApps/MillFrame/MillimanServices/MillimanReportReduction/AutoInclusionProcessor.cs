using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanReportReduction
{
    public class AutoInclusionProcessor
    {
        /// <summary>
        /// Input old and new hierarchy files, will scan user directory and create new selections, plus a 
        /// "del" file that includes bad selections from days-gone-by that do not match new hierarchy.  If AutoIncludeValues
        /// is false,  will only scrub values
        /// </summary>
        /// <param name="OldHierarchyFile"></param>
        /// <param name="NewHierarchyFile"></param>
        /// <param name="UserDirectory">Directory where all ".selections" file exist</param>
        /// <param name="AutoIncludeValues">Set to false, will only remove bad values that no long exist</param>
        public void Process(string OldHierarchyFile, string NewHierarchyFile, string UserDirectory, bool AutoIncludeValues = false)
        {
            //don't run this if autoinclusion is not needed,  or if old hierarchy does not exist ( note: this is the first time its run, so no old hierarchy exists )
            if ((AutoIncludeValues == false) || (System.IO.File.Exists(OldHierarchyFile) == false))
                return;

            string ReportName = System.IO.Path.GetFileNameWithoutExtension(OldHierarchyFile);
            //read the old hierarchy file
            MillimanCommon.MillimanTreeNode OldHierarchyNodes = MillimanCommon.MillimanTreeNode.GetMemoryTree(OldHierarchyFile);
            OldHierarchyNodes.DisplayFieldName = System.IO.Path.GetFileNameWithoutExtension(OldHierarchyFile);  //fill out report name in root

            MillimanCommon.MillimanTreeNode NewHierarchyNodes = MillimanCommon.MillimanTreeNode.GetMemoryTree(NewHierarchyFile);
            NewHierarchyNodes.DisplayFieldName = System.IO.Path.GetFileNameWithoutExtension(NewHierarchyFile);

            //now get all the selections
            List<string> AllSelectionFiles = System.IO.Directory.GetFiles(UserDirectory, ReportName + ".selections", System.IO.SearchOption.AllDirectories).ToList();
            foreach (string SelectionsFile in AllSelectionFiles)
            {
                //read the current selections selections file for the user
                Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                List<string> TreeSelections = SS.Deserialize(SelectionsFile) as List<string>;
                //load fully selected nodes
                List<string> FullySelected = new List<string>();
                if (AutoIncludeValues)
                    FullySelected = FindAllFullySelectedNodes(OldHierarchyNodes, TreeSelections);

                List<string> NewSelections = GetNewNodeSelections(NewHierarchyNodes, TreeSelections, FullySelected, SelectionsFile + "_del", AutoIncludeValues);
                System.IO.File.Delete(SelectionsFile + "_old");
                System.IO.File.Move(SelectionsFile, SelectionsFile + "_old");
                SS.Serialize(NewSelections, SelectionsFile);

            }
        }

        /// <summary>
        /// Get the string node value qualfieid by all parent notes like:
        /// Care Coordinator Report|Toaster Strudel Health Center|Hosptal of Suffern|Dr Jab
        /// </summary>
        /// <param name="StartNode"></param>
        /// <returns></returns>
        private string QualifiedNodeString(string PrefixValue, MillimanCommon.MillimanTreeNode StartNode)
        {
            string NodeString = string.Empty;
            MillimanCommon.MillimanTreeNode Current = StartNode;
            while (Current != null)
            {
                if (string.IsNullOrEmpty(Current.DisplayFieldName) == false)
                {
                    if (string.IsNullOrEmpty(NodeString) == false)
                        NodeString = "|" + NodeString;
                    NodeString = Current.DisplayFieldName + NodeString;
                }
                Current = Current.Parent;
            }
            if ((string.IsNullOrEmpty(NodeString) == false) && (string.IsNullOrEmpty(PrefixValue) == false))
                NodeString = PrefixValue + "|" + NodeString;
            return NodeString;
        }

        /// <summary>
        /// blah, given an arbitrary tree with X levels, count the different levels that
        /// are selected based on the current selection file - done for each user
        /// </summary>
        /// <param name="CurrentSelections"></param>
        /// <returns></returns>
        private List<string> CreateSubNodeCountList(List<string> CurrentSelections)
        {
            List<string> SubNodeCounts = new List<string>();
            string Working = string.Empty;
            int Count = 0;
            if (CurrentSelections.Count() > 0)
            {
                int Levels = CurrentSelections[0].Count(x => x == '|');

                foreach (string Current in CurrentSelections)
                {
                    Working = Current;
                    for (int LevelCount = 0; LevelCount < Levels; LevelCount++)
                    {
                        Working = Current.Substring(0, Working.LastIndexOf('|'));
                        Count = 0;
                        foreach (string ScanCurrent in CurrentSelections)
                        {
                            if (ScanCurrent.Contains(Working))
                                Count++;
                        }
                        string CountEntry = Working + "|" + Count.ToString();
                        if (SubNodeCounts.Contains(CountEntry) == false)
                            SubNodeCounts.Add(CountEntry);
                    }
                }
            }
            return SubNodeCounts;
        }

        /// <summary>
        /// Count the number of leaf nodes in the tree
        /// </summary>
        /// <param name="Current"></param>
        /// <returns></returns>
        private int TotalLeafNodes(MillimanCommon.MillimanTreeNode Current)
        {
            int TotalCount = 0;
            foreach (KeyValuePair<string, MillimanCommon.MillimanTreeNode> Node in Current.SubNodes)
            {
                if ((Node.Value.SubNodes == null) || (Node.Value.SubNodes.Count() == 0))
                    TotalCount++;
                else
                    TotalCount += TotalLeafNodes(Node.Value);
            }
            return TotalCount;
        }
        /// <summary>
        /// Determines if the qualified node 'SelectedNodeInfo' has all nodes selected
        /// </summary>
        /// <param name="Root"></param>
        /// <param name="SelectedNodeInfo"></param>
        /// <returns></returns>
        private bool LevelIsFullySelected(MillimanCommon.MillimanTreeNode Root, string SelectedNodeInfo)
        {
            //D1
            List<string> Tokens = SelectedNodeInfo.Split(new char[] { '|' }).ToList();
            int SelectedNodeCount = System.Convert.ToInt32(Tokens.Last());
            Tokens.RemoveAt(Tokens.Count() - 1);  //remove last node has count selected in it
            MillimanCommon.MillimanTreeNode Current = Root;
            if (string.Compare(Current.DisplayFieldName, Tokens[0], true) != 0)
                return false;  //this is the wrong report, root name does not match
            Tokens.RemoveAt(0);  //remove the head to get started

            //special case
            if (Tokens.Count() == 0) //we removed head, so count should match total number of root subnodes
            {
                return SelectedNodeCount == TotalLeafNodes(Root);
            }

            foreach (string Token in Tokens)
            {
                foreach (KeyValuePair<string, MillimanCommon.MillimanTreeNode> SubNode in Current.SubNodes)
                {
                    if (string.Compare(SubNode.Key, Token, true) == 0)
                    {
                        Current = SubNode.Value;
                        break;
                    }
                }
            }
            //at this point we should be walked down the tree to the correct node, let's check
            if (string.Compare(Tokens.Last(), Current.DisplayFieldName, true) == 0)
            {  //we are at the correct place, if this node has the same number children as selections, it's fully selected
                if (Current.SubNodes == null)
                    return false;  //somehow we are on a leaf, something is wrong, but what?
                return SelectedNodeCount == Current.SubNodes.Count;
            }
            return false;
        }

        /// <summary>
        /// Generate a list of all the nodes that have thier subnodes selected, returns root node and leaves
        /// </summary>
        /// <param name="Root"></param>
        /// <param name="CurrentSelections"></param>
        /// <param name="Prefix"></param>
        /// <returns></returns>
        private List<string> FindAllFullySelectedNodes(MillimanCommon.MillimanTreeNode Root, List<string> CurrentSelections)
        {
            List<string> FullySelectedNodes = new List<string>();

            List<string> SelectedNodeCount = CreateSubNodeCountList(CurrentSelections);

            foreach (string Current in SelectedNodeCount)
            {
                if (LevelIsFullySelected(Root, Current))
                    FullySelectedNodes.Add(Current.Substring(0, Current.LastIndexOf('|'))); //remove last entry is the nodes selected value

            }

            if (CurrentSelections.Count() > 0)
            {
                //we are only interested in either the root selected, or leaves selected, don't care about any
                //intermediate nodes, they will display correctly if we get leaves and/or root
                int StandardTreeLevels = CurrentSelections[0].Count(x => x == '|') - 1; //sub 1, since we are getting nodes, with leaves
                string CurrentSel = string.Empty;
                for (int Index = FullySelectedNodes.Count() - 1; Index >= 0; Index--)
                {
                    CurrentSel = FullySelectedNodes[Index];
                    int LevelsCount = CurrentSel.Count(x => x == '|');
                    if ((LevelsCount != 0) && (LevelsCount != StandardTreeLevels))
                        FullySelectedNodes.RemoveAt(Index);  //intermediate node
                }
            }
            return FullySelectedNodes;
        }

        /// <summary>
        /// Variable used to turn the entire tree into selections
        /// </summary>
        private List<string> AllNodesAsSelections { get; set; }
        private void GetAllNodes(MillimanCommon.MillimanTreeNode Current)
        {
            foreach (KeyValuePair<string, MillimanCommon.MillimanTreeNode> Node in Current.SubNodes)
            {
                if ((Node.Value.SubNodes == null) || (Node.Value.SubNodes.Count() == 0))
                    AllNodesAsSelections.Add(QualifiedNodeString("", Node.Value));
                else
                    GetAllNodes(Node.Value);
            }
        }
        private List<string> GetFullTree(MillimanCommon.MillimanTreeNode NEWTREE)
        {
            if (AllNodesAsSelections != null)
                AllNodesAsSelections.Clear();
            else
                AllNodesAsSelections = new List<string>();
            GetAllNodes(NEWTREE);
            return AllNodesAsSelections;
        }
        /// <summary>
        /// based on a node, get all the sub nodes as qualfieid selections
        /// </summary>
        /// <param name="NEWTREE"></param>
        /// <param name="QualifiedNode"></param>
        /// <returns></returns>
        private List<string> GetFullSelectionsForNode(MillimanCommon.MillimanTreeNode NEWTREE, string QualifiedNode)
        {

            List<string> NodeTokens = QualifiedNode.Split(new char[] { '|' }).ToList();
            NodeTokens.RemoveAt(0);  //get rid of head
            if (NodeTokens.Count() == 0)
                return GetFullTree(NEWTREE);  //get everything

            MillimanCommon.MillimanTreeNode Current = NEWTREE;
            foreach (string Token in NodeTokens)
            {
                foreach (KeyValuePair<string, MillimanCommon.MillimanTreeNode> SubNode in Current.SubNodes)
                {
                    if (string.Compare(SubNode.Key, Token, true) == 0)
                    {
                        Current = SubNode.Value;
                        break;
                    }
                }
            }
            List<string> Selections = new List<string>();
            //at this point we should be walked down the tree to the correct node, let's check
            if (string.Compare(NodeTokens.Last(), Current.DisplayFieldName, true) == 0)
            {  //we are at the correct place, if this node has the same number children as selections, it's fully selected
                foreach (KeyValuePair<string, MillimanCommon.MillimanTreeNode> MTN in Current.SubNodes)
                {
                    string SelectionValue = QualifiedNodeString("", MTN.Value);
                    if (string.IsNullOrEmpty(SelectionValue) == false)
                        Selections.Add(SelectionValue);
                }
            }
            return Selections;
        }

        /// <summary>
        /// verify the node exists in new tree, otherwise delete it
        /// </summary>
        /// <param name="NewHierarchyTree"></param>
        /// <param name="Selection"></param>
        /// <returns></returns>
        private bool NodeExists(MillimanCommon.MillimanTreeNode NewHierarchyTree, string Selection)
        {
            List<string> NodeTokens = Selection.Split(new char[] { '|' }).ToList();
            if (string.Compare(NewHierarchyTree.DisplayFieldName, NodeTokens[0], true) != 0)
                return false; //some other tree, we are outa here
            NodeTokens.RemoveAt(0);  //get rid of head
            if (NodeTokens.Count() == 0)
                return true; //just the root

            MillimanCommon.MillimanTreeNode Current = NewHierarchyTree;
            foreach (string Token in NodeTokens)
            {
                foreach (KeyValuePair<string, MillimanCommon.MillimanTreeNode> SubNode in Current.SubNodes)
                {
                    if (string.Compare(SubNode.Key, Token, true) == 0)
                    {
                        Current = SubNode.Value;
                        break;
                    }
                }
            }
            return (string.Compare(NodeTokens.Last(), Current.DisplayFieldName, true) == 0);
        }

        /// <summary>
        /// Get a list of all the new nodes for the user considering all nodes were selected at this level in
        /// previous version
        /// </summary>
        /// <param name="NewHierarchyTree"></param>
        /// <param name="UsersCurrentTreeSelections"></param>
        /// <param name="FullySelectedNodes"></param>
        /// <returns></returns>
        private List<string> GetNewNodeSelections(MillimanCommon.MillimanTreeNode NewHierarchyTree, List<string> UsersCurrentTreeSelections, List<string> FullySelectedNodes, string RemovedItemsFile, bool AutoInclude)
        {
            List<string> NewSelections = new List<string>();
            NewSelections.AddRange(UsersCurrentTreeSelections);
            System.IO.File.Delete(RemovedItemsFile); //get rid of old ones
            ///verify all items in UsersCurrentTreeSelections are valid, remove and log bad values
            for (int Index = NewSelections.Count() - 1; Index >= 0; Index--)
            {
                string Current = NewSelections[Index];
                if (NodeExists(NewHierarchyTree, Current) == false)
                {
                    System.IO.File.AppendAllText(RemovedItemsFile, Current + System.Environment.NewLine);
                    NewSelections.RemoveAt(Index);
                }
            }

            //if autoinclude is false, we just scrub out bad values, but don't include any new ones
            if (AutoInclude == false)
                return NewSelections;

            List<string> FullNodeSelections = null;
            foreach (string Current in FullySelectedNodes)
            {
                FullNodeSelections = GetFullSelectionsForNode(NewHierarchyTree, Current);
                if (FullNodeSelections != null)
                {
                    foreach (string FNS in FullNodeSelections)
                    {
                        if (NewSelections.Contains(FNS) == false)
                            NewSelections.Add(FNS);
                    }
                }
            }
            return NewSelections;
        }
    }
}
