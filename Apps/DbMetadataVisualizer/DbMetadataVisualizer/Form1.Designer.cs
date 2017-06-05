namespace DbMetadataVisualizer
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
            this.ListBoxUserIds = new System.Windows.Forms.ListBox();
            this.ListBoxGroupsOfSelectedUser = new System.Windows.Forms.ListBox();
            this.ListBoxDocuments = new System.Windows.Forms.ListBox();
            this.ListBoxUsersInSelectedGroup = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ListBoxAllGroupsWithNoUsers = new System.Windows.Forms.ListBox();
            this.ListBoxAllGroupsWithUsers = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ListBoxDocumentsNotInAnyGroup = new System.Windows.Forms.ListBox();
            this.contextMenuStripUngroupedDocuments = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemCopyRelative = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopyAbsolute = new System.Windows.Forms.ToolStripMenuItem();
            this.LabelDocumentRoot = new System.Windows.Forms.Label();
            this.LabelDbConnectionString = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ButtonGotoConfig = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.LabelRulesFile = new System.Windows.Forms.Label();
            this.LabelOrphanDocumentsReport = new System.Windows.Forms.Label();
            this.toolStripMenuItemCopyDistinctFolders = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripUngroupedDocuments.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ListBoxUserIds
            // 
            this.ListBoxUserIds.FormattingEnabled = true;
            this.ListBoxUserIds.Location = new System.Drawing.Point(12, 117);
            this.ListBoxUserIds.Name = "ListBoxUserIds";
            this.ListBoxUserIds.Size = new System.Drawing.Size(256, 251);
            this.ListBoxUserIds.TabIndex = 1;
            this.ListBoxUserIds.SelectedIndexChanged += new System.EventHandler(this.ListBoxUserIds_SelectedIndexChanged);
            this.ListBoxUserIds.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.AnyListBox_PreviewKeyDown);
            // 
            // ListBoxGroupsOfSelectedUser
            // 
            this.ListBoxGroupsOfSelectedUser.FormattingEnabled = true;
            this.ListBoxGroupsOfSelectedUser.Location = new System.Drawing.Point(274, 117);
            this.ListBoxGroupsOfSelectedUser.Name = "ListBoxGroupsOfSelectedUser";
            this.ListBoxGroupsOfSelectedUser.Size = new System.Drawing.Size(256, 251);
            this.ListBoxGroupsOfSelectedUser.TabIndex = 2;
            this.ListBoxGroupsOfSelectedUser.SelectedIndexChanged += new System.EventHandler(this.ListBoxGroups_SelectedIndexChanged);
            this.ListBoxGroupsOfSelectedUser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.AnyListBox_PreviewKeyDown);
            // 
            // ListBoxDocuments
            // 
            this.ListBoxDocuments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListBoxDocuments.FormattingEnabled = true;
            this.ListBoxDocuments.Location = new System.Drawing.Point(536, 117);
            this.ListBoxDocuments.Name = "ListBoxDocuments";
            this.ListBoxDocuments.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.ListBoxDocuments.Size = new System.Drawing.Size(471, 108);
            this.ListBoxDocuments.TabIndex = 3;
            this.ListBoxDocuments.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.AnyListBox_PreviewKeyDown);
            // 
            // ListBoxUsersInSelectedGroup
            // 
            this.ListBoxUsersInSelectedGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListBoxUsersInSelectedGroup.FormattingEnabled = true;
            this.ListBoxUsersInSelectedGroup.Location = new System.Drawing.Point(536, 247);
            this.ListBoxUsersInSelectedGroup.Name = "ListBoxUsersInSelectedGroup";
            this.ListBoxUsersInSelectedGroup.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.ListBoxUsersInSelectedGroup.Size = new System.Drawing.Size(469, 121);
            this.ListBoxUsersInSelectedGroup.TabIndex = 4;
            this.ListBoxUsersInSelectedGroup.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.AnyListBox_PreviewKeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "All Users";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(271, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(147, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Groups Of The Selected User";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(533, 231);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(158, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "All Users in The Selected Group";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(533, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(172, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Documents In The Selected Group";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(271, 371);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Groups With No Users";
            // 
            // ListBoxAllGroupsWithNoUsers
            // 
            this.ListBoxAllGroupsWithNoUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ListBoxAllGroupsWithNoUsers.FormattingEnabled = true;
            this.ListBoxAllGroupsWithNoUsers.Location = new System.Drawing.Point(274, 387);
            this.ListBoxAllGroupsWithNoUsers.Name = "ListBoxAllGroupsWithNoUsers";
            this.ListBoxAllGroupsWithNoUsers.Size = new System.Drawing.Size(256, 251);
            this.ListBoxAllGroupsWithNoUsers.TabIndex = 10;
            this.ListBoxAllGroupsWithNoUsers.SelectedIndexChanged += new System.EventHandler(this.ListBoxGroups_SelectedIndexChanged);
            this.ListBoxAllGroupsWithNoUsers.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.AnyListBox_PreviewKeyDown);
            // 
            // ListBoxAllGroupsWithUsers
            // 
            this.ListBoxAllGroupsWithUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ListBoxAllGroupsWithUsers.FormattingEnabled = true;
            this.ListBoxAllGroupsWithUsers.Location = new System.Drawing.Point(12, 387);
            this.ListBoxAllGroupsWithUsers.Name = "ListBoxAllGroupsWithUsers";
            this.ListBoxAllGroupsWithUsers.Size = new System.Drawing.Size(256, 251);
            this.ListBoxAllGroupsWithUsers.TabIndex = 11;
            this.ListBoxAllGroupsWithUsers.SelectedIndexChanged += new System.EventHandler(this.ListBoxGroups_SelectedIndexChanged);
            this.ListBoxAllGroupsWithUsers.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.AnyListBox_PreviewKeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 371);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Groups With At Least 1 User";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(533, 371);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(146, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Documents Not In Any Group";
            // 
            // ListBoxDocumentsNotInAnyGroup
            // 
            this.ListBoxDocumentsNotInAnyGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListBoxDocumentsNotInAnyGroup.ContextMenuStrip = this.contextMenuStripUngroupedDocuments;
            this.ListBoxDocumentsNotInAnyGroup.FormattingEnabled = true;
            this.ListBoxDocumentsNotInAnyGroup.Location = new System.Drawing.Point(536, 387);
            this.ListBoxDocumentsNotInAnyGroup.Name = "ListBoxDocumentsNotInAnyGroup";
            this.ListBoxDocumentsNotInAnyGroup.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.ListBoxDocumentsNotInAnyGroup.Size = new System.Drawing.Size(471, 225);
            this.ListBoxDocumentsNotInAnyGroup.TabIndex = 13;
            this.ListBoxDocumentsNotInAnyGroup.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.AnyListBox_PreviewKeyDown);
            // 
            // contextMenuStripUngroupedDocuments
            // 
            this.contextMenuStripUngroupedDocuments.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCopyRelative,
            this.toolStripMenuItemCopyAbsolute,
            this.toolStripMenuItemCopyDistinctFolders});
            this.contextMenuStripUngroupedDocuments.Name = "contextMenuStrip1";
            this.contextMenuStripUngroupedDocuments.Size = new System.Drawing.Size(254, 70);
            this.contextMenuStripUngroupedDocuments.Text = "Copy To Clipboard";
            // 
            // toolStripMenuItemCopyRelative
            // 
            this.toolStripMenuItemCopyRelative.Name = "toolStripMenuItemCopyRelative";
            this.toolStripMenuItemCopyRelative.Size = new System.Drawing.Size(253, 22);
            this.toolStripMenuItemCopyRelative.Text = "Copy All With Relative Paths";
            this.toolStripMenuItemCopyRelative.ToolTipText = "Copy All Contents with Relative Paths To Clipboard";
            this.toolStripMenuItemCopyRelative.Click += new System.EventHandler(this.toolStripMenuItemCopyRelative_Click);
            // 
            // toolStripMenuItemCopyAbsolute
            // 
            this.toolStripMenuItemCopyAbsolute.Name = "toolStripMenuItemCopyAbsolute";
            this.toolStripMenuItemCopyAbsolute.Size = new System.Drawing.Size(253, 22);
            this.toolStripMenuItemCopyAbsolute.Text = "Copy All With Absolute Paths";
            this.toolStripMenuItemCopyAbsolute.ToolTipText = "Copy All Contents with Absolute Paths To Clipboard";
            this.toolStripMenuItemCopyAbsolute.Click += new System.EventHandler(this.toolStripMenuItemCopyAbsolute_Click);
            // 
            // LabelDocumentRoot
            // 
            this.LabelDocumentRoot.AutoSize = true;
            this.LabelDocumentRoot.Location = new System.Drawing.Point(160, 16);
            this.LabelDocumentRoot.Name = "LabelDocumentRoot";
            this.LabelDocumentRoot.Size = new System.Drawing.Size(109, 13);
            this.LabelDocumentRoot.TabIndex = 15;
            this.LabelDocumentRoot.Text = "Completed at run time";
            this.LabelDocumentRoot.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // LabelDbConnectionString
            // 
            this.LabelDbConnectionString.AutoSize = true;
            this.LabelDbConnectionString.Location = new System.Drawing.Point(160, 52);
            this.LabelDbConnectionString.Name = "LabelDbConnectionString";
            this.LabelDbConnectionString.Size = new System.Drawing.Size(109, 13);
            this.LabelDbConnectionString.TabIndex = 16;
            this.LabelDbConnectionString.Text = "Completed at run time";
            this.LabelDbConnectionString.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.ButtonGotoConfig);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.LabelRulesFile);
            this.groupBox1.Controls.Add(this.LabelDocumentRoot);
            this.groupBox1.Controls.Add(this.LabelDbConnectionString);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(994, 73);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Configuration";
            // 
            // ButtonGotoConfig
            // 
            this.ButtonGotoConfig.Location = new System.Drawing.Point(908, 16);
            this.ButtonGotoConfig.Name = "ButtonGotoConfig";
            this.ButtonGotoConfig.Size = new System.Drawing.Size(80, 51);
            this.ButtonGotoConfig.TabIndex = 21;
            this.ButtonGotoConfig.Text = "Edit Configuration And Close";
            this.ButtonGotoConfig.UseVisualStyleBackColor = true;
            this.ButtonGotoConfig.Click += new System.EventHandler(this.ButtonGotoConfig_Click);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(14, 34);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(140, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "RepoFilePath:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(14, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "QVDocumentRoot:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(14, 52);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(140, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "PortalDB_ConnectionString:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LabelRulesFile
            // 
            this.LabelRulesFile.AutoSize = true;
            this.LabelRulesFile.Location = new System.Drawing.Point(160, 34);
            this.LabelRulesFile.Name = "LabelRulesFile";
            this.LabelRulesFile.Size = new System.Drawing.Size(109, 13);
            this.LabelRulesFile.TabIndex = 17;
            this.LabelRulesFile.Text = "Completed at run time";
            this.LabelRulesFile.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // LabelOrphanDocumentsReport
            // 
            this.LabelOrphanDocumentsReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelOrphanDocumentsReport.Location = new System.Drawing.Point(536, 615);
            this.LabelOrphanDocumentsReport.Name = "LabelOrphanDocumentsReport";
            this.LabelOrphanDocumentsReport.Size = new System.Drawing.Size(471, 23);
            this.LabelOrphanDocumentsReport.TabIndex = 18;
            this.LabelOrphanDocumentsReport.Text = "LabelOrphanDocumentsReport";
            this.LabelOrphanDocumentsReport.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // toolStripMenuItemCopyDistinctFolders
            // 
            this.toolStripMenuItemCopyDistinctFolders.Name = "toolStripMenuItemCopyDistinctFolders";
            this.toolStripMenuItemCopyDistinctFolders.Size = new System.Drawing.Size(253, 22);
            this.toolStripMenuItemCopyDistinctFolders.Text = "Copy All Distinct Absolute Folders";
            this.toolStripMenuItemCopyDistinctFolders.Click += new System.EventHandler(this.toolStripMenuItemCopyDistinctFolders_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1019, 651);
            this.Controls.Add(this.LabelOrphanDocumentsReport);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ListBoxDocumentsNotInAnyGroup);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ListBoxAllGroupsWithUsers);
            this.Controls.Add(this.ListBoxAllGroupsWithNoUsers);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ListBoxUsersInSelectedGroup);
            this.Controls.Add(this.ListBoxDocuments);
            this.Controls.Add(this.ListBoxGroupsOfSelectedUser);
            this.Controls.Add(this.ListBoxUserIds);
            this.Name = "Form1";
            this.Text = "Millframe Database / Metadata Visualizer";
            this.contextMenuStripUngroupedDocuments.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox ListBoxUserIds;
        private System.Windows.Forms.ListBox ListBoxGroupsOfSelectedUser;
        private System.Windows.Forms.ListBox ListBoxDocuments;
        private System.Windows.Forms.ListBox ListBoxUsersInSelectedGroup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox ListBoxAllGroupsWithNoUsers;
        private System.Windows.Forms.ListBox ListBoxAllGroupsWithUsers;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListBox ListBoxDocumentsNotInAnyGroup;
        private System.Windows.Forms.Label LabelDocumentRoot;
        private System.Windows.Forms.Label LabelDbConnectionString;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label LabelRulesFile;
        private System.Windows.Forms.Label LabelOrphanDocumentsReport;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripUngroupedDocuments;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopyRelative;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopyAbsolute;
        private System.Windows.Forms.Button ButtonGotoConfig;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopyDistinctFolders;
    }
}

