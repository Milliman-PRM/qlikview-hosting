using System;
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

        public Form1()
        {
            InitializeComponent();

            ButtonToggleExpand.Text = "Expand All";
        }

        private XmlDocument DecodeHierarchyFile(string FileName)
        {
            if (!File.Exists(FileName))
            {
                return null;
            }

            XmlDocument XMLD = new XmlDocument();
            string XML = "<xml>" + System.IO.File.ReadAllText(FileName) + "</xml>";
            XMLD.LoadXml(XML);

            DecodeXmlNode(XMLD.SelectSingleNode("xml"));

            return XMLD;
        }

        private XmlNode DecodeXmlNode(XmlNode EncodedNode)
        {
            if (EncodedNode.Attributes.Count == 2)
            {
                string EncodedText = EncodedNode.Attributes["Text"].Value;
                string EncodedValue = EncodedNode.Attributes["Value"].Value;

                EncodedNode.Attributes["Value"].Value = DecodeDelimitedString(EncodedValue, '|');
                EncodedNode.Attributes["Text"].Value = DecodeDelimitedString(EncodedText, '|');
            }

            foreach (XmlNode Child in EncodedNode.ChildNodes.OfType<XmlElement>())
            {
                DecodeXmlNode(Child);
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

            TreeViewHierarchy.Nodes.Add(new TreeNode(Dom.DocumentElement.Name));
            TreeNode TNode = TreeViewHierarchy.Nodes[0];

            AddTreeNode(Dom.DocumentElement, TNode);
        }

        private void AddTreeNode(XmlNode NodeToAdd, TreeNode TargetTreeNode)
        {
            foreach (XmlNode NextChildXmlNode in NodeToAdd.ChildNodes)
            {
                TreeNode NewChildTreeNode = new TreeNode(NextChildXmlNode.Name);
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
            OpenFileDialog1.Title = "Open Selections file";
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

                    // Find all subfolders that are named only with hexidecimal digits
                    string[] SelectionsFolders = Directory.GetDirectories(Path.Combine(_ProjectFolder, "ReducedUserQVWs")).Where(d => !Regex.IsMatch(Path.GetFileName(d), "[^a-fA-F0-9]")).ToArray();

                    ListViewUsers.Items.Clear();
                    foreach (string Folder in SelectionsFolders)
                    {
                        string UserName = HexToString(Path.GetFileName(Folder));
                        ListViewItem NewItem = new ListViewItem(UserName);
                        ListViewUsers.Items.Add(NewItem);
                        ListViewUsers.Columns[0].Text = string.Format("User Name ({0})", ListViewUsers.Items.Count);
                    }

                    XmlDocument HierarchyDocument = DecodeHierarchyFile(HierarchyFile);
                    XmlToTree(HierarchyDocument);
                }
                finally
                {
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

        private bool CheckSelectedTreeNode(IEnumerable<string> SelectionStrings, TreeNode ParentTreeNode, bool Checked=true)
        {
            int ChildChecks = 0;

            foreach (TreeNode ChildNode in ParentTreeNode.Nodes)
            {
                if (SelectionStrings.First() == ChildNode.Text)
                {
                    if (SelectionStrings.Count() == 1)  // meaning if this is the leaf node for this hierarchy
                    {
                        ChildNode.Checked = Checked;
                        ChildNode.BackColor = Color.LightGreen;
                        ChildChecks++;
                        break;
                    }
                    else
                    {
                        if (CheckSelectedTreeNode(SelectionStrings.Skip(1), ChildNode, Checked))
                        {
                            ChildChecks++;
                        }
                    }
                }
            }

            if (ChildChecks == ParentTreeNode.Nodes.Count)
            {
                ParentTreeNode.Checked = Checked;
                ParentTreeNode.BackColor = Color.LightGreen;
                return true;
            }
            else if (ChildChecks > 0)
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
            SetTreeNodeCheck(TreeViewHierarchy.Nodes[0], false, true);

            // Undo any previous BackColor values
            foreach (ListViewItem Item in ListViewUsers.Items)
            {
                Item.BackColor = Color.White;
            }
            TextBoxErrorList.Text = string.Empty;

            ListView.SelectedListViewItemCollection SelectedListItems = ListViewUsers.SelectedItems;
            // If the ListView is set for single selection then this foreach will always find at most one selected item
            foreach (ListViewItem SelectedUserItem in SelectedListItems)
            {
                SelectedUserItem.BackColor = SystemColors.Highlight;

                string HexedUserName = StringToHex(SelectedUserItem.Text);
                string SelectionFolder = Path.Combine(_ProjectFolder, "ReducedUserQVWs", HexedUserName);

                string[] SelectionFiles = Directory.GetFiles(SelectionFolder, "*.selections");

                foreach (string SelectionFile in SelectionFiles)
                {
                    using (StreamReader Reader = new StreamReader(SelectionFile))
                    {
                        XmlDocument XMLD = new XmlDocument();
                        string XML = Reader.ReadToEnd();
                        XMLD.LoadXml(XML);

                        foreach (XmlNode SelectionNode in XMLD.SelectSingleNode("Collection").SelectSingleNode("Items").SelectNodes("Simple"))
                        {
                            bool Found = false;
                            string[] SelectionStrings = SelectionNode.Attributes["value"].Value.Split('|');
                            this.Invoke((MethodInvoker)delegate {
                                Found = CheckSelectedTreeNode(SelectionStrings.Skip(1), TreeViewHierarchy.Nodes[0]);
                            });
                            if (!Found)
                            {
                                TextBoxErrorList.Text += SelectionNode.Attributes["value"].Value + "\r\n";
                                // TODO Indicate this problem in the UI somewhere. 
                            }
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
    }
}
