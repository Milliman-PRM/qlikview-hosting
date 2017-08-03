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

namespace DehexifyStrings
{
    public partial class Form1 : Form
    {
        string _ProjectFolder = string.Empty;
        string _ProjectName = string.Empty;
        int _HierarchyLevelsIncludingProjectName = 0;

        public Form1()
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
                    string HierarchyFile = Directory.GetFiles(_ProjectFolder, "*.hierarchy_*").FirstOrDefault();

                    if (!File.Exists(HierarchyFile))
                    {
                        MessageBox.Show("No hierarchy file found in folder " + _ProjectFolder);
                        return;
                    }

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
                        string SelectionFile = Path.Combine(Folder, _ProjectName + ".selections");
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

            //Xml = string.Format("<xml Text=\"{0}\" Value=\"Project Name\">", _ProjectName) + System.IO.File.ReadAllText(TextBoxLeftFile.Text) + "</xml>";
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
            TextBoxLeftFile.Text = (ListViewUsers.FocusedItem.Tag as UserItemDetail).SelectionFile;
            TextBoxRightFile.Text = TextBoxLeftFile.Text + "_new";
            tabControl1.SelectedTab = TabPageCompareSelections;
        }

        private void ButtonCompareSelectionFiles_Click(object sender, EventArgs e)
        {
            DataGridViewSelectionComparison.AutoGenerateColumns = true;
            //DataGridViewSelectionComparison.Rows.Clear();
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
                        C.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        C.Width = 50;
                        //MessageBox.Show(C.Name + " is bool");
                        break;
                    default:
                        break;
                }
            }

            foreach (DataGridViewRow R in DataGridViewSelectionComparison.Rows)
            {
            }

        }

        private void TextBoxLeftFile_TextChanged(object sender, EventArgs e)
        {
            DataGridViewSelectionComparison.Enabled = false;
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
    }
}
