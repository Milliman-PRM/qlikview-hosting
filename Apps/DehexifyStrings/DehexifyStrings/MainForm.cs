/*
 * CODE OWNERS: Tom Puckett,
 * OBJECTIVE: Visualize hierarchies and associated selections, and see insights to the result of preparations for report publication
 * DEVELOPER NOTES: <What future developers need to know.>
 */

using System;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MillimanCommon;

namespace DehexifyStrings
{
    public partial class MainForm : Form
    {
        string _ProjectFolder = string.Empty;
        string _ProjectName = string.Empty;
        int _HierarchyLevelsIncludingProjectName = 0;

        public MainForm()
        {
            InitializeComponent();

            ButtonToggleExpand.Text = "Expand All";
            TabPageCompareSelections_Resize(null, null);
        }

        private XmlDocument DecodeHierarchyFile(string FileName)
        {
            if (!File.Exists(FileName))
            {
                return null;
            }

            XmlDocument XMLD = new XmlDocument();
            string XML = string.Format("<xml Text=\"{0}\" Value=\"Project Name\">", _ProjectName) + System.IO.File.ReadAllText(FileName) + "</xml>";
            XMLD.LoadXml(XML);

            // Record the depth of the hierarchy
            _HierarchyLevelsIncludingProjectName = 0;
            XmlNode NodeWalk = XMLD;
            while (NodeWalk.HasChildNodes)
            {
                NodeWalk = NodeWalk.FirstChild;
                _HierarchyLevelsIncludingProjectName++;
            }

            DecodeXmlNode(XMLD.SelectSingleNode("xml"), true, true);

            return XMLD;
        }

        private XmlNode DecodeXmlNode(XmlNode EncodedNode, bool ReturnInputOnFail, bool RecurseDown = true)
        {
            if (EncodedNode.Attributes.GetNamedItem("Text") != null && EncodedNode.Attributes.GetNamedItem("Value") != null)
            {
                string EncodedText = EncodedNode.Attributes["Text"].Value;
                EncodedNode.Attributes["Text"].Value = DecodeDelimitedString(EncodedText, '|');
                if (ReturnInputOnFail && string.IsNullOrEmpty(EncodedNode.Attributes["Text"].Value) && EncodedText.Length > 1)
                {
                    EncodedNode.Attributes["Text"].Value = EncodedText;
                }

                string EncodedValue = EncodedNode.Attributes["Value"].Value;
                EncodedNode.Attributes["Value"].Value = DecodeDelimitedString(EncodedValue, '|');
                if (ReturnInputOnFail && string.IsNullOrEmpty(EncodedNode.Attributes["Value"].Value) && EncodedValue.Length > 1)
                {
                    EncodedNode.Attributes["Value"].Value = EncodedValue;
                }
            }

            if (RecurseDown)
            {
                foreach (XmlNode Child in EncodedNode.ChildNodes.OfType<XmlElement>())
                {
                    DecodeXmlNode(Child, ReturnInputOnFail, RecurseDown);
                }
            }

            return EncodedNode;
        }

        string DecodeDelimitedString(string EncodedString, char Delimiter)
        {
            string ReturnVal = string.Empty;
            string[] InputArray = EncodedString.Split(Delimiter);
            List<string> OutputList = new List<string>();
            foreach (string EncodedElement in InputArray)
            {
                OutputList.Add(HexToString(EncodedElement));
            }
            ReturnVal = string.Join(Delimiter.ToString(), OutputList);
            return ReturnVal;
        }

        private string HexToString(string Input)
        {
            if (Regex.IsMatch(Input, @"[^a-fA-F0-9]"))
            {
                return null;
            }

            string ReturnVal = string.Empty;

            while (Input.Length > 1)
            {
                string NextCharHex = Input.Substring(0, 2);
                Input = Input.Substring(2);
                ReturnVal += System.Convert.ToChar(Convert.ToUInt16(NextCharHex, 16));
            }

            return ReturnVal;
        }

        private string StringToHex(string Input)
        {
            if (!Regex.IsMatch(Input, @"[^a-fA-F0-9]"))
            {
                return null;
            }

            string ReturnVal = string.Empty;

            while (Input.Length > 0)
            {
                ReturnVal += Convert.ToByte(Input[0]).ToString("x2");
                Input = Input.Substring(1);
            }

            return ReturnVal;
        }

        private void XmlToTree(XmlDocument Dom)
        {
            TreeViewHierarchy.Nodes.Clear();

            TreeNode TNode = new TreeNode(Dom.DocumentElement.Name);
            TreeViewHierarchy.Nodes.Add(TNode);

            AddTreeNode(Dom.DocumentElement, TNode);

            TreeViewHierarchy.Sort();
        }

        private void AddTreeNode(XmlNode NodeToAdd, TreeNode TargetTreeNode)
        {
            foreach (XmlNode NextChildXmlNode in NodeToAdd.ChildNodes)
            {
                TreeNode NewChildTreeNode = new TreeNode(NextChildXmlNode.Name);  // Tree node text is modified below
                TargetTreeNode.Nodes.Add(NewChildTreeNode);
                AddTreeNode(NextChildXmlNode, NewChildTreeNode);
            }

            if (NodeToAdd.Attributes.GetNamedItem("Value") != null && NodeToAdd.Attributes.GetNamedItem("Text") != null)
            {
                TargetTreeNode.ToolTipText = NodeToAdd.Attributes["Value"].Value;
                TargetTreeNode.Text = NodeToAdd.Attributes["Text"].Value;
            }
        }

        private void ButtonReadProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog1.Title = "Open Project file";
            OpenFileDialog1.Filter = "Project files (*.hciprj)|*.hciprj|All files (*.*)|*.*";

            if (OpenFileDialog1.ShowDialog() == DialogResult.OK && File.Exists(OpenFileDialog1.FileName))
            {
                Cursor OriginalCursor = this.Cursor;
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string ProjectFile = OpenFileDialog1.FileName;
                    _ProjectFolder = Path.GetFullPath(Path.GetDirectoryName(ProjectFile));

                    string HierarchySuffix = CheckBoxUseNewSelections.Checked ? "*.hierarchy_*_old" : "*.hierarchy_*_new";
                    string HierarchyFile = Directory.GetFiles(_ProjectFolder, HierarchySuffix).FirstOrDefault();

                    if (!File.Exists(HierarchyFile))
                    {
                        MessageBox.Show("No hierarchy file found in folder " + _ProjectFolder);
                        return;
                    }

                    LabelHierarchyFile.Text = HierarchyFile;

                    XmlDocument HciprojXml = new XmlDocument();
                    using (StreamReader R = new StreamReader(ProjectFile))
                    {
                        HciprojXml.LoadXml(R.ReadToEnd());
                    }
                    _ProjectName = HciprojXml.SelectSingleNode("Complex").SelectSingleNode("Properties").SelectSingleNode("Simple[@name=\"ProjectName\"]").Attributes["value"].Value;

                    XmlDocument HierarchyDocument = DecodeHierarchyFile(HierarchyFile);
                    XmlToTree(HierarchyDocument);

                    // Find all subfolders that are named only with hexidecimal digits
                    string[] SelectionsFolders = Directory.GetDirectories(Path.Combine(_ProjectFolder, "ReducedUserQVWs")).Where(d => !Regex.IsMatch(Path.GetFileName(d), "[^a-fA-F0-9]")).ToArray();

                    ListViewUsers.Items.Clear();
                    foreach (string Folder in SelectionsFolders)
                    {
                        string Extension = CheckBoxUseNewSelections.Checked ? ".selections_old" : ".selections";
                        string SelectionFile = Path.Combine(Folder, _ProjectName + Extension);
                        UserItemDetail ThisUserDetail = new UserItemDetail
                        {
                            SelectionFile = File.Exists(SelectionFile) ? SelectionFile : "Not Found",
                            HexedUserName = Path.GetFileName(Folder),
                        };

                        string UserName = HexToString(Path.GetFileName(Folder));
                        ListViewItem NewItem = new ListViewItem(UserName);
                        NewItem.Tag = ThisUserDetail;
                        ListViewUsers.Items.Add(NewItem);
                        ListViewUsers.Columns[0].Text = string.Format("User Name ({0} total)", ListViewUsers.Items.Count);
                        ListViewUsers.Items[ListViewUsers.Items.Count - 1].Selected = true;
                        ListViewUsers.Items[ListViewUsers.Items.Count - 1].Selected = false;
                    }
                    ListViewUsers.Sorting = SortOrder.Ascending;
                    ListViewUsers.Sort();
                }
                finally
                {
                    ListBoxUserDetail.Items.Clear();
                    this.Cursor = OriginalCursor;
                }

            }
        }

        private void SetTreeNodeCheck(TreeNode RootNode, bool Checked, bool RecurseDown = false)
        {
            RootNode.Checked = Checked;
            if (!Checked)
            {
                RootNode.BackColor = Color.White;
            }
            else
            {
                // TODO What about this?  Need to evaluate parent nodes for color?
            }

            if (RecurseDown)
            {
                foreach (TreeNode Child in RootNode.Nodes)
                {
                    SetTreeNodeCheck(Child, Checked, RecurseDown);
                }
            }
        }

        private bool CheckSelectedTreeNode(string[] SelectionStrings, TreeNode ParentTreeNode, bool Checked=true)
        {
            foreach (TreeNode ChildNode in ParentTreeNode.Nodes)
            {
                string[] ChildSelectionStrings = SelectionStrings.Skip(1).ToArray();
                if (ChildSelectionStrings.First() == ChildNode.Text)
                {
                    if (ChildSelectionStrings.Count() == 1)  // meaning if this is the leaf node for this hierarchy
                    {
                        ChildNode.Checked = Checked;
                        ChildNode.BackColor = Color.LightGreen;
                        break;
                    }
                    else
                    {
                        CheckSelectedTreeNode(ChildSelectionStrings, ChildNode, Checked);
                    }
                }
            }

            int ChildredChecked = 0;
            foreach (TreeNode N in ParentTreeNode.Nodes)
            {
                if (N.Checked) ChildredChecked++;
            }

            if (ChildredChecked == ParentTreeNode.Nodes.Count)
            {
                ParentTreeNode.Checked = Checked;
                ParentTreeNode.BackColor = Color.LightGreen;
                return true;
            }
            else if (ChildredChecked > 0)
            {
                ParentTreeNode.Checked = Checked;
                ParentTreeNode.BackColor = Color.Yellow;
                return true;
            }

            return false;
        }

        private void ListViewUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Uncheck everything in the hierarchy tree
            if (TreeViewHierarchy.Nodes.Count > 0)
            {
                SetTreeNodeCheck(TreeViewHierarchy.Nodes[0], false, true);
            }

            ListBoxErrors.Items.Clear();
            ListBoxUserDetail.Items.Clear();
            int SelectionCount = 0;

            ListView.SelectedListViewItemCollection SelectedListItems = ListViewUsers.SelectedItems;
            // If the ListView is set for single selection then this foreach will always find at most one selected item
            foreach (ListViewItem SelectedUserItem in SelectedListItems)
            {
                SelectedUserItem.BackColor = Color.LightGreen;
                UserItemDetail UserDetail = SelectedUserItem.Tag as UserItemDetail;

                string SelectionFolder = Path.Combine(_ProjectFolder, "ReducedUserQVWs", UserDetail.HexedUserName);

                ListBoxUserDetail.Items.Add("Selection file: " + UserDetail.SelectionFile);
                ListBoxUserDetail.Items.Add("Encoded User Name: " + UserDetail.HexedUserName);

                string SelectionFile = UserDetail.SelectionFile;
                if (File.Exists(SelectionFile))
                using (StreamReader Reader = new StreamReader(SelectionFile))
                {
                    XmlDocument XMLD = new XmlDocument();
                    string XML = Reader.ReadToEnd();
                    XMLD.LoadXml(XML);

                    foreach (XmlNode SelectionNode in XMLD.SelectSingleNode("Collection").SelectSingleNode("Items").SelectNodes("Simple"))
                    {
                        SelectionCount++;
                        bool Found = false;
                        string[] SelectionStrings = SelectionNode.Attributes["value"].Value.Split('|');
                        int Levels = SelectionStrings.Length;
                        if (Levels != _HierarchyLevelsIncludingProjectName)
                        {
                            SelectedUserItem.BackColor = Color.OrangeRed;
                            ListBoxErrors.Items.Add("Wrong selection depth: " + SelectionNode.Attributes["value"].Value);
                        }

                        this.Invoke((MethodInvoker)delegate {
                            Found = CheckSelectedTreeNode(SelectionStrings, TreeViewHierarchy.Nodes[0]);
                        });

                        if (!Found)
                        {
                            SelectedUserItem.BackColor = Color.OrangeRed;
                            ListBoxErrors.Items.Add(SelectionNode.Attributes["value"].Value);
                            // TODO Indicate this problem in the UI somewhere. 
                        }
                    }
                }
            }

            ListBoxUserDetail.Items.Add("Leaf Nodes Selected: " + SelectionCount);
        }

        private void ButtonToggleExpand_Click(object sender, EventArgs e)
        {
            switch (ButtonToggleExpand.Text)
            {
                case "Collapse All":
                    TreeViewHierarchy.CollapseAll();
                    ButtonToggleExpand.Text = "Expand All";
                    break;
                case "Expand All":
                    TreeViewHierarchy.ExpandAll();
                    ButtonToggleExpand.Text = "Collapse All";
                    break;
                default:
                    break;
            }
        }

        private void splitContainer1_Panel1_Resize(object sender, EventArgs e)
        {
            if (ListViewUsers.Columns.Count > 0)
            ListViewUsers.Columns[0].Width = splitContainer1.Panel1.Width - 4;
        }

        private void ListViewUsers_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ListViewUsers.GetItemAt(e.X, e.Y) != null)
                {
                    ContextMenuStripUserList.Show(Cursor.Position);
                }
            }
        }

        private void ToolStripMenuItemEditSelections_Click(object sender, EventArgs e)
        {
            string SelectionsFile = (ListViewUsers.FocusedItem.Tag as UserItemDetail).SelectionFile;

            if (File.Exists(SelectionsFile))
            {
                Process.Start("notepad.exe", SelectionsFile);
            }
        }

        private void TextBoxAnyFileChosen_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog1.Title = "Open Selections File";
            OpenFileDialog1.Filter = "Selection files (*.selections*)|*.selections*|All files (*.*)|*.*";

            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                (sender as TextBox).Text = OpenFileDialog1.FileName;
            }
        }
        private DataTable CompareSelectionFiles()
        {
            if (!File.Exists(TextBoxLeftFile.Text) || !File.Exists(TextBoxRightFile.Text))
            {
                return null;
            }

            XmlDocument BaseXml = new XmlDocument();
            XmlDocument ComparisonXml = new XmlDocument();
            string Xml = string.Empty;
            HashSet<string> LeftFileSelections = new HashSet<string>();
            HashSet<string> RightFileSelections = new HashSet<string>();
            HashSet<string> UnionOfAllSelections = new HashSet<string>();
            int MaxDepth = 0;

            Xml = System.IO.File.ReadAllText(TextBoxLeftFile.Text);
            BaseXml.LoadXml(Xml);
            foreach (XmlNode SelectionNode in BaseXml.SelectSingleNode("Collection").SelectSingleNode("Items").SelectNodes("Simple"))
            {
                LeftFileSelections.Add(SelectionNode.Attributes["value"].Value);
                UnionOfAllSelections.Add(SelectionNode.Attributes["value"].Value);
                MaxDepth = Math.Max(MaxDepth, SelectionNode.Attributes["value"].Value.Split('|').Length - 1);
            }

            Xml = System.IO.File.ReadAllText(TextBoxRightFile.Text);
            ComparisonXml.LoadXml(Xml);
            foreach (XmlNode SelectionNode in ComparisonXml.SelectSingleNode("Collection").SelectSingleNode("Items").SelectNodes("Simple"))
            {
                RightFileSelections.Add(SelectionNode.Attributes["value"].Value);
                UnionOfAllSelections.Add(SelectionNode.Attributes["value"].Value);
                MaxDepth = Math.Max(MaxDepth, SelectionNode.Attributes["value"].Value.Split(new char[] {'|'}, StringSplitOptions.None).Length - 1);
            }

            // Define the columns
            DataTable ReturnValue = new DataTable("SelectionComparisonResultTable");
            for (int Level=1; Level <= MaxDepth; Level++)
            {
                ReturnValue.Columns.Add("Hier" + Level, typeof(string));
            }
            ReturnValue.Columns.Add("In Base", typeof(bool));
            ReturnValue.Columns.Add("In Comparison", typeof(bool));
            ReturnValue.Columns.Add("Status", typeof(string));

            // Add the rows
            foreach (string Selection in UnionOfAllSelections.OrderBy(s => s))
            {
                string[] SelectionArray = Selection.Split('|');
                DataRow NewRow = ReturnValue.NewRow();
                for (int Level = 1 ; Level <= MaxDepth ; Level++)
                {
                    NewRow["Hier" + Level] = SelectionArray[Level];
                }
                NewRow["In Base"] = LeftFileSelections.Contains(Selection);
                NewRow["In Comparison"] = RightFileSelections.Contains(Selection);
                var x = (bool)NewRow["In Base"];
                if (LeftFileSelections.Contains(Selection) && RightFileSelections.Contains(Selection))
                {
                    NewRow["Status"] = "No Change";
                }
                else if (LeftFileSelections.Contains(Selection) && !RightFileSelections.Contains(Selection))
                {
                    NewRow["Status"] = "Removed";
                }
                else if (!LeftFileSelections.Contains(Selection) && RightFileSelections.Contains(Selection))
                {
                    NewRow["Status"] = "Added";
                }
                ReturnValue.Rows.Add(NewRow);
            }

            return ReturnValue;
        }

        private void TabPageCompareSelections_Resize(object sender, EventArgs e)
        {
            TextBoxLeftFile.Width = TabPageCompareSelections.Width - TextBoxLeftFile.Left - TextBoxLeftFile.Margin.Right;
            TextBoxRightFile.Width = TabPageCompareSelections.Width - TextBoxRightFile.Left - TextBoxRightFile.Margin.Right;
        }

        private void ToolStripMenuItemCompareSelectionsFiles_Click(object sender, EventArgs e)
        {
            string DepictedSelections = (ListViewUsers.FocusedItem.Tag as UserItemDetail).SelectionFile;

            if (DepictedSelections.EndsWith("_old"))
            {
                TextBoxLeftFile.Text = DepictedSelections;
                TextBoxRightFile.Text = DepictedSelections.Substring(0, DepictedSelections.Length - "_old".Length);
            }
            else if(DepictedSelections.EndsWith(".selections"))
            {
                TextBoxLeftFile.Text = DepictedSelections + "_old";
                TextBoxRightFile.Text = DepictedSelections;
            }
            else
            {
                TextBoxLeftFile.Text = DepictedSelections;
                TextBoxRightFile.Text = string.Empty;
            }

            tabControl1.SelectedTab = TabPageCompareSelections;  // Switch to the comparison tab
            DataGridViewSelectionComparison.DataSource = null;  // Clear the results grid in case it has a previous comparison
        }

        private void ButtonCompareSelectionFiles_Click(object sender, EventArgs e)
        {
            foreach (TextBox B in new TextBox[] { TextBoxLeftFile, TextBoxRightFile })
            {
                if (!File.Exists(B.Text))
                {
                    MessageBox.Show(string.Format("File {0} not found", B.Text));
                    return;
                }
            }

            DataGridViewSelectionComparison.AutoGenerateColumns = true;
            DataGridViewSelectionComparison.DataSource = null;
            DataGridViewSelectionComparison.Enabled = true;

            DataSet SelectionComparisonResult = new DataSet();
            DataTable SelectionComparisonResultTable = CompareSelectionFiles();
            SelectionComparisonResult.Tables.Add(SelectionComparisonResultTable);

            DataGridViewSelectionComparison.DataSource = SelectionComparisonResult;
            DataGridViewSelectionComparison.DataMember = "SelectionComparisonResultTable";

            foreach (DataGridViewColumn C in DataGridViewSelectionComparison.Columns)
            {
                switch (C.ValueType.ToString())
                {
                    case "System.String":
                        //MessageBox.Show(C.Name + " is string");
                        C.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        break;
                    case "System.Boolean":
                        C.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        //MessageBox.Show(C.Name + " is bool");
                        break;
                    default:
                        break;
                }
            }
        }

        private void TextBoxAnyFile_TextChanged(object sender, EventArgs e)
        {
            DataGridViewSelectionComparison.DataSource = null;
        }

        private void DataGridViewSelectionComparison_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridViewRow R = ((DataGridView)sender).Rows[e.RowIndex];

            if (R.Cells["Status"].Value == null)
            {
                return;
            }
            switch (R.Cells["Status"].Value.ToString())
            {
                case "Added":
                    R.DefaultCellStyle.BackColor = Color.IndianRed;
                    break;
                case "Removed":
                    R.DefaultCellStyle.BackColor = Color.LightGreen;
                    break;
                case "No Change":
                default:
                    break;
            }

        }

        private void TextBoxTimeAspxStatusEncoded_TextChanged(object sender, EventArgs e)
        {
            MillimanCommon.AutoCrypt AC = new MillimanCommon.AutoCrypt();

            TextBoxTimeAspxStatusDecoded.Text = AC.AutoDecrypt(TextBoxTimeAspxStatusEncoded.Text);
        }

        private void ButtonSelectHierarchyUserAccessFiles_Click(object sender, EventArgs e)
        {
            ListViewSelections.Items.Clear();
            ListViewSelectionUsers.Items.Clear();

            OpenFileDialog1.Title = "Open Hierarchy File";
            OpenFileDialog1.Filter = "Hierarchy1 files (*.hierarchy1.txt)|*.hierarchy1.txt|UserAccess1 files (*.hierarchy1.txt)|*.UserAccess1.txt|All files (*.*)|*.*";

            if (OpenFileDialog1.ShowDialog() != DialogResult.OK || 
                !File.Exists(OpenFileDialog1.FileName) ||
                ( !OpenFileDialog1.FileName.ToLower().Contains(".hierarchy1.txt") && !OpenFileDialog1.FileName.ToLower().Contains(".useraccess1.txt")))
            {
                return;
            }

            string FileBaseName = OpenFileDialog1.FileName.Substring(0, OpenFileDialog1.FileName.LastIndexOf('.', OpenFileDialog1.FileName.Length-5));
            string Hierarchy1FileName = FileBaseName + ".hierarchy1.txt";
            string UserAccess1FileName = FileBaseName + ".useraccess1.txt";

            // Read contents of the Hierarchy file
            string[] AllHierarchyLines = File.ReadAllLines(Hierarchy1FileName);
            List<string> HierarchyValues = new List<string>();
            for (int i=1; i< AllHierarchyLines.Length; i++)
            {
                HierarchyValues.Add(AllHierarchyLines[i].Split('|')[0]);
            }
            foreach (var SelectionValue in HierarchyValues.OrderBy(v => v))
            {
                ListViewSelections.Items.Add(new ListViewItem(SelectionValue));
            }

            // Read contents of the UserAccess file
            string[] AllUserAccessLines = File.ReadAllLines(UserAccess1FileName);
            List<string> AccessFileHierarchyValues = AllUserAccessLines[0].Split('|').Skip(1).ToList();
            // Validate
            if (AccessFileHierarchyValues.Count != HierarchyValues.Count ||
                AccessFileHierarchyValues.OrderBy(v=>v).Except(HierarchyValues.OrderBy(v => v)).Count() > 0 ||
                HierarchyValues.OrderBy(v => v).Except(AccessFileHierarchyValues.OrderBy(v => v)).Count() > 0)
            {
                MessageBox.Show(string.Format("Mismatch of hierarchy values between UserAccess file and Hierarchy file!"));
                return;
            }

            Dictionary<string, Dictionary<string, bool>> ExpectedUserSelections = new Dictionary<string, Dictionary<string, bool>>();
            for (int i = 1; i < AllUserAccessLines.Length; i++)
            {
                string UserName = AllUserAccessLines[i].Split('|').First();
                List<string> Selections = AllUserAccessLines[i].Split('|').Skip(1).ToList();
                Dictionary<string, bool> ThisUserSelections = new Dictionary<string, bool>();
                for (int j = 0; j < Selections.Count; j++)
                {
                    ThisUserSelections[AccessFileHierarchyValues[j]] = Selections[j].ToLower() == "x" ? true : false;
                }
                ExpectedUserSelections[UserName] = ThisUserSelections;
            }

            foreach (var User in ExpectedUserSelections.OrderBy(u=>u.Key))
            {
                ListViewItem NewItem = new ListViewItem(User.Key);
                NewItem.Tag = User.Value;
                ListViewSelectionUsers.Items.Add(NewItem);
            }
        }

        private void ListViewSelectionUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBoxSelectionDetails.Items.Clear();
            foreach (ListViewItem SelectionItem in ListViewSelections.Items)
            {
                SelectionItem.Checked = false;
                SelectionItem.BackColor = Color.OrangeRed;
            }

            foreach (ListViewItem SelectedUser in ListViewSelectionUsers.SelectedItems)
            {
                ListBoxSelectionDetails.Items.Add("Selected user " + SelectedUser.Text);

                Dictionary<string,bool> SelectionDict = SelectedUser.Tag as Dictionary<string, bool>;
                foreach (ListViewItem SelectionValueItem in ListViewSelections.Items)
                {
                    if (SelectionDict[SelectionValueItem.Text])
                    {
                        SelectionValueItem.Checked = true;
                        SelectionValueItem.BackColor = Color.LightGreen;
                    }
                }
            }

            ListBoxSelectionDetails.Items.Add(string.Format("{0} items ARE selected", ListViewSelections.CheckedItems.Count));
            ListBoxSelectionDetails.Items.Add(string.Format("{0} items NOT selected", ListViewSelections.Items.Count - ListViewSelections.CheckedItems.Count));
        }
    }
}
