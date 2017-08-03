namespace DehexifyStrings
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabPageHierarchySelections = new System.Windows.Forms.TabPage();
            this.CheckBoxUseNewSelections = new System.Windows.Forms.CheckBox();
            this.ButtonToggleExpand = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ListViewUsers = new System.Windows.Forms.ListView();
            this.ColumnHeaderUserName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LabelHierarchy = new System.Windows.Forms.Label();
            this.LabelUserDetail = new System.Windows.Forms.Label();
            this.ListBoxUserDetail = new System.Windows.Forms.ListBox();
            this.LabelSelectionsNotFound = new System.Windows.Forms.Label();
            this.ListBoxErrors = new System.Windows.Forms.ListBox();
            this.TreeViewHierarchy = new System.Windows.Forms.TreeView();
            this.ContextMenuStripUserList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemEditSelections = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemCompareSelectionsFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.ButtonReadProject = new System.Windows.Forms.Button();
            this.TabPageCompareSelections = new System.Windows.Forms.TabPage();
            this.DataGridViewSelectionComparison = new System.Windows.Forms.DataGridView();
            this.ButtonCompareSelectionFiles = new System.Windows.Forms.Button();
            this.LabelComparisonFile = new System.Windows.Forms.Label();
            this.LabelBaseFile = new System.Windows.Forms.Label();
            this.TextBoxRightFile = new System.Windows.Forms.TextBox();
            this.TextBoxLeftFile = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.TabPageHierarchySelections.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.ContextMenuStripUserList.SuspendLayout();
            this.TabPageCompareSelections.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewSelectionComparison)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.TabPageHierarchySelections);
            this.tabControl1.Controls.Add(this.TabPageCompareSelections);
            this.tabControl1.Location = new System.Drawing.Point(13, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1067, 512);
            this.tabControl1.TabIndex = 2;
            // 
            // TabPageHierarchySelections
            // 
            this.TabPageHierarchySelections.Controls.Add(this.CheckBoxUseNewSelections);
            this.TabPageHierarchySelections.Controls.Add(this.ButtonToggleExpand);
            this.TabPageHierarchySelections.Controls.Add(this.splitContainer1);
            this.TabPageHierarchySelections.Controls.Add(this.ButtonReadProject);
            this.TabPageHierarchySelections.Location = new System.Drawing.Point(4, 22);
            this.TabPageHierarchySelections.Name = "TabPageHierarchySelections";
            this.TabPageHierarchySelections.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageHierarchySelections.Size = new System.Drawing.Size(1059, 486);
            this.TabPageHierarchySelections.TabIndex = 1;
            this.TabPageHierarchySelections.Text = "User Selections";
            this.TabPageHierarchySelections.UseVisualStyleBackColor = true;
            // 
            // CheckBoxUseNewSelections
            // 
            this.CheckBoxUseNewSelections.AutoSize = true;
            this.CheckBoxUseNewSelections.Location = new System.Drawing.Point(89, 9);
            this.CheckBoxUseNewSelections.Name = "CheckBoxUseNewSelections";
            this.CheckBoxUseNewSelections.Size = new System.Drawing.Size(149, 17);
            this.CheckBoxUseNewSelections.TabIndex = 4;
            this.CheckBoxUseNewSelections.Text = "Use \'_new\' Selection Files";
            this.CheckBoxUseNewSelections.UseVisualStyleBackColor = true;
            // 
            // ButtonToggleExpand
            // 
            this.ButtonToggleExpand.Location = new System.Drawing.Point(425, 6);
            this.ButtonToggleExpand.Name = "ButtonToggleExpand";
            this.ButtonToggleExpand.Size = new System.Drawing.Size(75, 23);
            this.ButtonToggleExpand.TabIndex = 1;
            this.ButtonToggleExpand.Text = "Expand All";
            this.ButtonToggleExpand.UseVisualStyleBackColor = true;
            this.ButtonToggleExpand.Click += new System.EventHandler(this.ButtonToggleExpand_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(7, 37);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ListViewUsers);
            this.splitContainer1.Panel1.Resize += new System.EventHandler(this.splitContainer1_Panel1_Resize);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.LabelHierarchy);
            this.splitContainer1.Panel2.Controls.Add(this.LabelUserDetail);
            this.splitContainer1.Panel2.Controls.Add(this.ListBoxUserDetail);
            this.splitContainer1.Panel2.Controls.Add(this.LabelSelectionsNotFound);
            this.splitContainer1.Panel2.Controls.Add(this.ListBoxErrors);
            this.splitContainer1.Panel2.Controls.Add(this.TreeViewHierarchy);
            this.splitContainer1.Size = new System.Drawing.Size(1046, 446);
            this.splitContainer1.SplitterDistance = 204;
            this.splitContainer1.TabIndex = 3;
            // 
            // ListViewUsers
            // 
            this.ListViewUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListViewUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeaderUserName});
            this.ListViewUsers.FullRowSelect = true;
            this.ListViewUsers.HideSelection = false;
            this.ListViewUsers.LabelWrap = false;
            this.ListViewUsers.Location = new System.Drawing.Point(0, 0);
            this.ListViewUsers.Margin = new System.Windows.Forms.Padding(0);
            this.ListViewUsers.MultiSelect = false;
            this.ListViewUsers.Name = "ListViewUsers";
            this.ListViewUsers.Size = new System.Drawing.Size(204, 446);
            this.ListViewUsers.TabIndex = 0;
            this.ListViewUsers.UseCompatibleStateImageBehavior = false;
            this.ListViewUsers.View = System.Windows.Forms.View.Details;
            this.ListViewUsers.SelectedIndexChanged += new System.EventHandler(this.ListViewUsers_SelectedIndexChanged);
            this.ListViewUsers.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListViewUsers_MouseClick);
            // 
            // ColumnHeaderUserName
            // 
            this.ColumnHeaderUserName.Text = "User Name";
            this.ColumnHeaderUserName.Width = 200;
            // 
            // LabelHierarchy
            // 
            this.LabelHierarchy.AutoSize = true;
            this.LabelHierarchy.Location = new System.Drawing.Point(3, 9);
            this.LabelHierarchy.Name = "LabelHierarchy";
            this.LabelHierarchy.Size = new System.Drawing.Size(52, 13);
            this.LabelHierarchy.TabIndex = 5;
            this.LabelHierarchy.Text = "Hierarchy";
            // 
            // LabelUserDetail
            // 
            this.LabelUserDetail.AutoSize = true;
            this.LabelUserDetail.Location = new System.Drawing.Point(304, 9);
            this.LabelUserDetail.Name = "LabelUserDetail";
            this.LabelUserDetail.Size = new System.Drawing.Size(109, 13);
            this.LabelUserDetail.TabIndex = 6;
            this.LabelUserDetail.Text = "Selected User Details";
            // 
            // ListBoxUserDetail
            // 
            this.ListBoxUserDetail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListBoxUserDetail.FormattingEnabled = true;
            this.ListBoxUserDetail.Location = new System.Drawing.Point(307, 22);
            this.ListBoxUserDetail.Margin = new System.Windows.Forms.Padding(0);
            this.ListBoxUserDetail.Name = "ListBoxUserDetail";
            this.ListBoxUserDetail.Size = new System.Drawing.Size(531, 108);
            this.ListBoxUserDetail.TabIndex = 5;
            // 
            // LabelSelectionsNotFound
            // 
            this.LabelSelectionsNotFound.AutoSize = true;
            this.LabelSelectionsNotFound.Location = new System.Drawing.Point(304, 156);
            this.LabelSelectionsNotFound.Name = "LabelSelectionsNotFound";
            this.LabelSelectionsNotFound.Size = new System.Drawing.Size(165, 13);
            this.LabelSelectionsNotFound.TabIndex = 4;
            this.LabelSelectionsNotFound.Text = "Selection Errors / Inconsistencies";
            // 
            // ListBoxErrors
            // 
            this.ListBoxErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListBoxErrors.FormattingEnabled = true;
            this.ListBoxErrors.Location = new System.Drawing.Point(307, 169);
            this.ListBoxErrors.Margin = new System.Windows.Forms.Padding(0);
            this.ListBoxErrors.Name = "ListBoxErrors";
            this.ListBoxErrors.Size = new System.Drawing.Size(531, 277);
            this.ListBoxErrors.TabIndex = 4;
            // 
            // TreeViewHierarchy
            // 
            this.TreeViewHierarchy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.TreeViewHierarchy.CheckBoxes = true;
            this.TreeViewHierarchy.ContextMenuStrip = this.ContextMenuStripUserList;
            this.TreeViewHierarchy.Location = new System.Drawing.Point(0, 22);
            this.TreeViewHierarchy.Margin = new System.Windows.Forms.Padding(0);
            this.TreeViewHierarchy.Name = "TreeViewHierarchy";
            this.TreeViewHierarchy.ShowNodeToolTips = true;
            this.TreeViewHierarchy.Size = new System.Drawing.Size(300, 424);
            this.TreeViewHierarchy.TabIndex = 2;
            // 
            // ContextMenuStripUserList
            // 
            this.ContextMenuStripUserList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemEditSelections,
            this.ToolStripMenuItemCompareSelectionsFiles});
            this.ContextMenuStripUserList.Name = "ContextMenuStripUserList";
            this.ContextMenuStripUserList.Size = new System.Drawing.Size(215, 48);
            // 
            // ToolStripMenuItemEditSelections
            // 
            this.ToolStripMenuItemEditSelections.Name = "ToolStripMenuItemEditSelections";
            this.ToolStripMenuItemEditSelections.Size = new System.Drawing.Size(214, 22);
            this.ToolStripMenuItemEditSelections.Text = "Edit This User\'s Selections";
            this.ToolStripMenuItemEditSelections.Click += new System.EventHandler(this.ToolStripMenuItemEditSelections_Click);
            // 
            // ToolStripMenuItemCompareSelectionsFiles
            // 
            this.ToolStripMenuItemCompareSelectionsFiles.Name = "ToolStripMenuItemCompareSelectionsFiles";
            this.ToolStripMenuItemCompareSelectionsFiles.Size = new System.Drawing.Size(214, 22);
            this.ToolStripMenuItemCompareSelectionsFiles.Text = "Compare 2 Selections Files";
            this.ToolStripMenuItemCompareSelectionsFiles.Click += new System.EventHandler(this.ToolStripMenuItemCompareSelectionsFiles_Click);
            // 
            // ButtonReadProject
            // 
            this.ButtonReadProject.Location = new System.Drawing.Point(7, 6);
            this.ButtonReadProject.Name = "ButtonReadProject";
            this.ButtonReadProject.Size = new System.Drawing.Size(75, 23);
            this.ButtonReadProject.TabIndex = 0;
            this.ButtonReadProject.Text = "Browse...";
            this.ButtonReadProject.UseVisualStyleBackColor = true;
            this.ButtonReadProject.Click += new System.EventHandler(this.ButtonReadProject_Click);
            // 
            // TabPageCompareSelections
            // 
            this.TabPageCompareSelections.Controls.Add(this.DataGridViewSelectionComparison);
            this.TabPageCompareSelections.Controls.Add(this.ButtonCompareSelectionFiles);
            this.TabPageCompareSelections.Controls.Add(this.LabelComparisonFile);
            this.TabPageCompareSelections.Controls.Add(this.LabelBaseFile);
            this.TabPageCompareSelections.Controls.Add(this.TextBoxRightFile);
            this.TabPageCompareSelections.Controls.Add(this.TextBoxLeftFile);
            this.TabPageCompareSelections.Location = new System.Drawing.Point(4, 22);
            this.TabPageCompareSelections.Name = "TabPageCompareSelections";
            this.TabPageCompareSelections.Size = new System.Drawing.Size(1059, 486);
            this.TabPageCompareSelections.TabIndex = 2;
            this.TabPageCompareSelections.Text = "Compare Selections";
            this.TabPageCompareSelections.UseVisualStyleBackColor = true;
            this.TabPageCompareSelections.Resize += new System.EventHandler(this.TabPageCompareSelections_Resize);
            // 
            // DataGridViewSelectionComparison
            // 
            this.DataGridViewSelectionComparison.AllowUserToAddRows = false;
            this.DataGridViewSelectionComparison.AllowUserToDeleteRows = false;
            this.DataGridViewSelectionComparison.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataGridViewSelectionComparison.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewSelectionComparison.Location = new System.Drawing.Point(6, 106);
            this.DataGridViewSelectionComparison.Name = "DataGridViewSelectionComparison";
            this.DataGridViewSelectionComparison.ReadOnly = true;
            this.DataGridViewSelectionComparison.Size = new System.Drawing.Size(1047, 377);
            this.DataGridViewSelectionComparison.TabIndex = 6;
            this.DataGridViewSelectionComparison.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.DataGridViewSelectionComparison_RowPostPaint);
            // 
            // ButtonCompareSelectionFiles
            // 
            this.ButtonCompareSelectionFiles.Location = new System.Drawing.Point(4, 77);
            this.ButtonCompareSelectionFiles.Name = "ButtonCompareSelectionFiles";
            this.ButtonCompareSelectionFiles.Size = new System.Drawing.Size(102, 23);
            this.ButtonCompareSelectionFiles.TabIndex = 5;
            this.ButtonCompareSelectionFiles.Text = "Compare Files";
            this.ButtonCompareSelectionFiles.UseVisualStyleBackColor = true;
            this.ButtonCompareSelectionFiles.Click += new System.EventHandler(this.ButtonCompareSelectionFiles_Click);
            // 
            // LabelComparisonFile
            // 
            this.LabelComparisonFile.AutoSize = true;
            this.LabelComparisonFile.Location = new System.Drawing.Point(3, 47);
            this.LabelComparisonFile.Name = "LabelComparisonFile";
            this.LabelComparisonFile.Size = new System.Drawing.Size(84, 13);
            this.LabelComparisonFile.TabIndex = 4;
            this.LabelComparisonFile.Text = "Comparison File:";
            // 
            // LabelBaseFile
            // 
            this.LabelBaseFile.AutoSize = true;
            this.LabelBaseFile.Location = new System.Drawing.Point(3, 19);
            this.LabelBaseFile.Name = "LabelBaseFile";
            this.LabelBaseFile.Size = new System.Drawing.Size(53, 13);
            this.LabelBaseFile.TabIndex = 3;
            this.LabelBaseFile.Text = "Base File:";
            // 
            // TextBoxRightFile
            // 
            this.TextBoxRightFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxRightFile.Location = new System.Drawing.Point(90, 44);
            this.TextBoxRightFile.Name = "TextBoxRightFile";
            this.TextBoxRightFile.Size = new System.Drawing.Size(963, 20);
            this.TextBoxRightFile.TabIndex = 1;
            this.TextBoxRightFile.Text = "Double-Click to Browse";
            this.TextBoxRightFile.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxAnyFileChosen_MouseDoubleClick);
            // 
            // TextBoxLeftFile
            // 
            this.TextBoxLeftFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxLeftFile.Location = new System.Drawing.Point(90, 16);
            this.TextBoxLeftFile.Name = "TextBoxLeftFile";
            this.TextBoxLeftFile.Size = new System.Drawing.Size(966, 20);
            this.TextBoxLeftFile.TabIndex = 0;
            this.TextBoxLeftFile.Text = "Double-Click to Browse";
            this.TextBoxLeftFile.TextChanged += new System.EventHandler(this.TextBoxLeftFile_TextChanged);
            this.TextBoxLeftFile.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxAnyFileChosen_MouseDoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1092, 537);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Project Metadata Visualizer";
            this.tabControl1.ResumeLayout(false);
            this.TabPageHierarchySelections.ResumeLayout(false);
            this.TabPageHierarchySelections.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ContextMenuStripUserList.ResumeLayout(false);
            this.TabPageCompareSelections.ResumeLayout(false);
            this.TabPageCompareSelections.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewSelectionComparison)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog OpenFileDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabPageHierarchySelections;
        private System.Windows.Forms.Button ButtonReadProject;
        private System.Windows.Forms.TreeView TreeViewHierarchy;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView ListViewUsers;
        private System.Windows.Forms.ColumnHeader ColumnHeaderUserName;
        private System.Windows.Forms.Button ButtonToggleExpand;
        private System.Windows.Forms.Label LabelHierarchy;
        private System.Windows.Forms.Label LabelSelectionsNotFound;
        private System.Windows.Forms.ListBox ListBoxErrors;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStripUserList;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemEditSelections;
        private System.Windows.Forms.Label LabelUserDetail;
        private System.Windows.Forms.ListBox ListBoxUserDetail;
        private System.Windows.Forms.TabPage TabPageCompareSelections;
        private System.Windows.Forms.TextBox TextBoxRightFile;
        private System.Windows.Forms.TextBox TextBoxLeftFile;
        private System.Windows.Forms.Label LabelComparisonFile;
        private System.Windows.Forms.Label LabelBaseFile;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemCompareSelectionsFiles;
        private System.Windows.Forms.Button ButtonCompareSelectionFiles;
        private System.Windows.Forms.DataGridView DataGridViewSelectionComparison;
        private System.Windows.Forms.CheckBox CheckBoxUseNewSelections;
    }
}

