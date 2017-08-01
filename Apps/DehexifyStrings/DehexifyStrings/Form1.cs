using System;
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
        public Form1()
        {
            InitializeComponent();
        }

        private void ButtonFromFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FileName = openFileDialog1.FileName;

                using (StreamReader Reader = new StreamReader(FileName))
                {
                    string Line;
                    while (Reader.Peek() > 0)
                    {
                        Line = Reader.ReadLine();
                        TextBoxResult.Text += HexToString(Line) + "\r\n";
                    }
                }
            }
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
            if (!Regex.IsMatch(Input, @"^[a-fA-F0-9]+$"))
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

        private void ButtonReadSelections_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open Selections file";
            openFileDialog1.Filter = "Selection files (*.selections)|*.selections|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK && File.Exists(openFileDialog1.FileName))
            {
                string HierarchyFolder = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(openFileDialog1.FileName), "..\\.."));
                string HierarchyFile = Directory.GetFiles(HierarchyFolder, "*.hierarchy_*").First();

                XmlDocument HierarchyDocument = DecodeHierarchyFile(HierarchyFile);
                XmlToTree(HierarchyDocument);

                using (StreamReader Reader = new StreamReader(openFileDialog1.FileName))
                {
                    while (Reader.Peek() > 0)
                    {
                        string Line = Reader.ReadToEnd();

                        XmlDocument XMLD = new XmlDocument();
                        string XML = System.IO.File.ReadAllText(openFileDialog1.FileName);
                        XMLD.LoadXml(XML);

                        foreach (XmlNode SelectionNode in XMLD.SelectSingleNode("Collection").SelectSingleNode("Items").SelectNodes("Simple"))
                        {
                            string[] SelectionStrings = SelectionNode.Attributes["value"].Value.Split('|');
                            CheckSelectedNode(SelectionStrings.Skip(1), TreeViewHierarchy.Nodes[0]);
                        }

                        //var x = TreeViewHierarchy.Nodes.Find(SelectedLeaf[1], false);
                    }
                }
            }
        }

        private bool CheckSelectedNode(IEnumerable<string> SelectionStrings, TreeNode ParentTreeNode)
        {
            int ChildChecks = 0;

            foreach (TreeNode ChildNode in ParentTreeNode.Nodes)
            {
                if (SelectionStrings.First() == ChildNode.Text)
                {
                    if (SelectionStrings.Count() == 1)  // meaning if this is the leaf node for this hierarchy
                    {
                        ChildNode.Checked = true;
                        ChildNode.BackColor = Color.LightGreen;
                        ChildChecks++;
                    }
                    else
                    {
                        if (CheckSelectedNode(SelectionStrings.Skip(1), ChildNode))
                        {
                            ChildChecks++;
                        }
                    }
                }
            }

            if (ChildChecks == ParentTreeNode.Nodes.Count)
            {
                ParentTreeNode.Checked = true;
                ParentTreeNode.BackColor = Color.LightGreen;
                return true;
            }
            else if (ChildChecks > 0)
            {
                ParentTreeNode.Checked = true;
                ParentTreeNode.BackColor = Color.Yellow;
                return true;
            }

            return false;
        }
    }
}
