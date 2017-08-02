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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.LabelHierarchy = new System.Windows.Forms.Label();
            this.ButtonToggleExpand = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ListViewUsers = new System.Windows.Forms.ListView();
            this.ColumnHeaderUserName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LabelUserDetail = new System.Windows.Forms.Label();
            this.ListBoxUserDetail = new System.Windows.Forms.ListBox();
            this.LabelSelectionsNotFound = new System.Windows.Forms.Label();
            this.ListBoxErrors = new System.Windows.Forms.ListBox();
            this.TreeViewHierarchy = new System.Windows.Forms.TreeView();
            this.ContextMenuStripUserList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemEditSelections = new System.Windows.Forms.ToolStripMenuItem();
            this.ButtonReadProject = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.ContextMenuStripUserList.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(13, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1067, 512);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ButtonToggleExpand);
            this.tabPage2.Controls.Add(this.splitContainer1);
            this.tabPage2.Controls.Add(this.ButtonReadProject);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1059, 486);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "User Selections";
            this.tabPage2.UseVisualStyleBackColor = true;
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
            this.ToolStripMenuItemEditSelections});
            this.ContextMenuStripUserList.Name = "ContextMenuStripUserList";
            this.ContextMenuStripUserList.Size = new System.Drawing.Size(210, 26);
            // 
            // ToolStripMenuItemEditSelections
            // 
            this.ToolStripMenuItemEditSelections.Name = "ToolStripMenuItemEditSelections";
            this.ToolStripMenuItemEditSelections.Size = new System.Drawing.Size(209, 22);
            this.ToolStripMenuItemEditSelections.Text = "Edit This User\'s Selections";
            this.ToolStripMenuItemEditSelections.Click += new System.EventHandler(this.ToolStripMenuItemEditSelections_Click);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1092, 537);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Project Metadata Visualizer";
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ContextMenuStripUserList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog OpenFileDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
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
    }
}

