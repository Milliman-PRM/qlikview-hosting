namespace PRMServerMonitorGUI
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ButtonRemoveOrphanTasks = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabPageTasks = new System.Windows.Forms.TabPage();
            this.TabPageDocCals = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.DataGridViewDocCals = new System.Windows.Forms.DataGridView();
            this.ColumnDocument = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnUserId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLastAccessDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDelete = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ButtonDeleteSelectedCals = new System.Windows.Forms.Button();
            this.ButtonClearAllDeleteChecks = new System.Windows.Forms.Button();
            this.CheckBoxAllowUndatedCalSelection = new System.Windows.Forms.CheckBox();
            this.ButtonEnumerateDocCALs = new System.Windows.Forms.Button();
            this.TabPageUserCals = new System.Windows.Forms.TabPage();
            this.TextBoxUserName = new System.Windows.Forms.TextBox();
            this.ButtonDeleteTest = new System.Windows.Forms.Button();
            this.ButtonCalReport = new System.Windows.Forms.Button();
            this.TabPageDebug = new System.Windows.Forms.TabPage();
            this.TextBoxPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TextBoxDocName = new System.Windows.Forms.TextBox();
            this.ButtonDeleteDocCalTest = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.TextBoxDocUserName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ComboBoxServer = new System.Windows.Forms.ComboBox();
            this.LabelCheckedCount = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.NumericUpDownMinAge = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.TabPageTasks.SuspendLayout();
            this.TabPageDocCals.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewDocCals)).BeginInit();
            this.TabPageUserCals.SuspendLayout();
            this.TabPageDebug.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownMinAge)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonRemoveOrphanTasks
            // 
            this.ButtonRemoveOrphanTasks.Location = new System.Drawing.Point(6, 6);
            this.ButtonRemoveOrphanTasks.Name = "ButtonRemoveOrphanTasks";
            this.ButtonRemoveOrphanTasks.Size = new System.Drawing.Size(210, 23);
            this.ButtonRemoveOrphanTasks.TabIndex = 2;
            this.ButtonRemoveOrphanTasks.Text = "Remove Orphan Tasks";
            this.ButtonRemoveOrphanTasks.UseVisualStyleBackColor = true;
            this.ButtonRemoveOrphanTasks.Click += new System.EventHandler(this.ButtonRemoveOrphanTasks_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.TabPageTasks);
            this.tabControl1.Controls.Add(this.TabPageDocCals);
            this.tabControl1.Controls.Add(this.TabPageUserCals);
            this.tabControl1.Controls.Add(this.TabPageDebug);
            this.tabControl1.Location = new System.Drawing.Point(12, 37);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1160, 562);
            this.tabControl1.TabIndex = 4;
            // 
            // TabPageTasks
            // 
            this.TabPageTasks.Controls.Add(this.ButtonRemoveOrphanTasks);
            this.TabPageTasks.Location = new System.Drawing.Point(4, 22);
            this.TabPageTasks.Name = "TabPageTasks";
            this.TabPageTasks.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageTasks.Size = new System.Drawing.Size(1047, 536);
            this.TabPageTasks.TabIndex = 0;
            this.TabPageTasks.Text = "Qlikview Tasks";
            this.TabPageTasks.UseVisualStyleBackColor = true;
            // 
            // TabPageDocCals
            // 
            this.TabPageDocCals.Controls.Add(this.splitContainer1);
            this.TabPageDocCals.Location = new System.Drawing.Point(4, 22);
            this.TabPageDocCals.Name = "TabPageDocCals";
            this.TabPageDocCals.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageDocCals.Size = new System.Drawing.Size(1152, 536);
            this.TabPageDocCals.TabIndex = 1;
            this.TabPageDocCals.Text = "Qlikview Document CALs";
            this.TabPageDocCals.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(7, 7);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.DataGridViewDocCals);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label7);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.NumericUpDownMinAge);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.LabelCheckedCount);
            this.splitContainer1.Panel2.Controls.Add(this.ButtonDeleteSelectedCals);
            this.splitContainer1.Panel2.Controls.Add(this.ButtonClearAllDeleteChecks);
            this.splitContainer1.Panel2.Controls.Add(this.CheckBoxAllowUndatedCalSelection);
            this.splitContainer1.Panel2.Controls.Add(this.ButtonEnumerateDocCALs);
            this.splitContainer1.Panel2MinSize = 54;
            this.splitContainer1.Size = new System.Drawing.Size(1139, 523);
            this.splitContainer1.SplitterDistance = 465;
            this.splitContainer1.TabIndex = 6;
            // 
            // DataGridViewDocCals
            // 
            this.DataGridViewDocCals.AllowUserToAddRows = false;
            this.DataGridViewDocCals.AllowUserToDeleteRows = false;
            this.DataGridViewDocCals.AllowUserToResizeRows = false;
            this.DataGridViewDocCals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridViewDocCals.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.DataGridViewDocCals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewDocCals.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnDocument,
            this.ColumnUserId,
            this.ColumnLastAccessDateTime,
            this.ColumnDelete});
            this.DataGridViewDocCals.Location = new System.Drawing.Point(3, 3);
            this.DataGridViewDocCals.Name = "DataGridViewDocCals";
            this.DataGridViewDocCals.Size = new System.Drawing.Size(1131, 457);
            this.DataGridViewDocCals.TabIndex = 0;
            this.DataGridViewDocCals.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewDocCals_CellMouseUp);
            this.DataGridViewDocCals.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewDocCals_CellValueChanged);
            this.DataGridViewDocCals.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.DataGridViewDocCals_RowPostPaint);
            // 
            // ColumnDocument
            // 
            this.ColumnDocument.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnDocument.FillWeight = 65F;
            this.ColumnDocument.HeaderText = "Document";
            this.ColumnDocument.Name = "ColumnDocument";
            this.ColumnDocument.ReadOnly = true;
            // 
            // ColumnUserId
            // 
            this.ColumnUserId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnUserId.FillWeight = 35F;
            this.ColumnUserId.HeaderText = "User";
            this.ColumnUserId.Name = "ColumnUserId";
            this.ColumnUserId.ReadOnly = true;
            // 
            // ColumnLastAccessDateTime
            // 
            this.ColumnLastAccessDateTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnLastAccessDateTime.HeaderText = "Last Access";
            this.ColumnLastAccessDateTime.Name = "ColumnLastAccessDateTime";
            this.ColumnLastAccessDateTime.ReadOnly = true;
            this.ColumnLastAccessDateTime.Width = 90;
            // 
            // ColumnDelete
            // 
            this.ColumnDelete.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ColumnDelete.HeaderText = "Delete";
            this.ColumnDelete.Name = "ColumnDelete";
            this.ColumnDelete.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnDelete.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ColumnDelete.Width = 63;
            // 
            // ButtonDeleteSelectedCals
            // 
            this.ButtonDeleteSelectedCals.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ButtonDeleteSelectedCals.Location = new System.Drawing.Point(456, 3);
            this.ButtonDeleteSelectedCals.Name = "ButtonDeleteSelectedCals";
            this.ButtonDeleteSelectedCals.Size = new System.Drawing.Size(165, 46);
            this.ButtonDeleteSelectedCals.TabIndex = 8;
            this.ButtonDeleteSelectedCals.Text = "Delete Selected CALs";
            this.ButtonDeleteSelectedCals.UseVisualStyleBackColor = true;
            this.ButtonDeleteSelectedCals.Click += new System.EventHandler(this.ButtonDeleteSelectedCals_Click);
            // 
            // ButtonClearAllDeleteChecks
            // 
            this.ButtonClearAllDeleteChecks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonClearAllDeleteChecks.Location = new System.Drawing.Point(1034, 3);
            this.ButtonClearAllDeleteChecks.Name = "ButtonClearAllDeleteChecks";
            this.ButtonClearAllDeleteChecks.Size = new System.Drawing.Size(100, 23);
            this.ButtonClearAllDeleteChecks.TabIndex = 7;
            this.ButtonClearAllDeleteChecks.Text = "&Uncheck All";
            this.ButtonClearAllDeleteChecks.UseVisualStyleBackColor = true;
            this.ButtonClearAllDeleteChecks.Click += new System.EventHandler(this.ButtonClearAllDeleteChecks_Click);
            // 
            // CheckBoxAllowUndatedCalSelection
            // 
            this.CheckBoxAllowUndatedCalSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckBoxAllowUndatedCalSelection.AutoSize = true;
            this.CheckBoxAllowUndatedCalSelection.Location = new System.Drawing.Point(909, 34);
            this.CheckBoxAllowUndatedCalSelection.Name = "CheckBoxAllowUndatedCalSelection";
            this.CheckBoxAllowUndatedCalSelection.Size = new System.Drawing.Size(229, 17);
            this.CheckBoxAllowUndatedCalSelection.TabIndex = 6;
            this.CheckBoxAllowUndatedCalSelection.Text = "Allow Selection Of 0001-01-01 Dated CALs";
            this.CheckBoxAllowUndatedCalSelection.UseVisualStyleBackColor = true;
            this.CheckBoxAllowUndatedCalSelection.CheckedChanged += new System.EventHandler(this.CheckBoxAllowUndatedCalSelection_CheckedChanged);
            // 
            // ButtonEnumerateDocCALs
            // 
            this.ButtonEnumerateDocCALs.Location = new System.Drawing.Point(3, 3);
            this.ButtonEnumerateDocCALs.Name = "ButtonEnumerateDocCALs";
            this.ButtonEnumerateDocCALs.Size = new System.Drawing.Size(75, 46);
            this.ButtonEnumerateDocCALs.TabIndex = 5;
            this.ButtonEnumerateDocCALs.Text = "&Refresh Table";
            this.ButtonEnumerateDocCALs.UseVisualStyleBackColor = true;
            this.ButtonEnumerateDocCALs.Click += new System.EventHandler(this.ButtonEnumerateDocCALs_Click);
            // 
            // TabPageUserCals
            // 
            this.TabPageUserCals.Controls.Add(this.TextBoxUserName);
            this.TabPageUserCals.Controls.Add(this.ButtonDeleteTest);
            this.TabPageUserCals.Controls.Add(this.ButtonCalReport);
            this.TabPageUserCals.Location = new System.Drawing.Point(4, 22);
            this.TabPageUserCals.Name = "TabPageUserCals";
            this.TabPageUserCals.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageUserCals.Size = new System.Drawing.Size(1047, 536);
            this.TabPageUserCals.TabIndex = 2;
            this.TabPageUserCals.Text = "Qlikview User CALs";
            this.TabPageUserCals.UseVisualStyleBackColor = true;
            // 
            // TextBoxUserName
            // 
            this.TextBoxUserName.Location = new System.Drawing.Point(88, 38);
            this.TextBoxUserName.Name = "TextBoxUserName";
            this.TextBoxUserName.Size = new System.Drawing.Size(177, 20);
            this.TextBoxUserName.TabIndex = 6;
            // 
            // ButtonDeleteTest
            // 
            this.ButtonDeleteTest.Location = new System.Drawing.Point(7, 36);
            this.ButtonDeleteTest.Name = "ButtonDeleteTest";
            this.ButtonDeleteTest.Size = new System.Drawing.Size(75, 23);
            this.ButtonDeleteTest.TabIndex = 5;
            this.ButtonDeleteTest.Text = "Delete Test";
            this.ButtonDeleteTest.UseVisualStyleBackColor = true;
            this.ButtonDeleteTest.Click += new System.EventHandler(this.ButtonDeleteTest_Click);
            // 
            // ButtonCalReport
            // 
            this.ButtonCalReport.Location = new System.Drawing.Point(6, 6);
            this.ButtonCalReport.Name = "ButtonCalReport";
            this.ButtonCalReport.Size = new System.Drawing.Size(259, 23);
            this.ButtonCalReport.TabIndex = 4;
            this.ButtonCalReport.Text = "Report CALs";
            this.ButtonCalReport.UseVisualStyleBackColor = true;
            this.ButtonCalReport.Click += new System.EventHandler(this.ButtonCalReport_Click);
            // 
            // TabPageDebug
            // 
            this.TabPageDebug.Controls.Add(this.TextBoxPath);
            this.TabPageDebug.Controls.Add(this.label4);
            this.TabPageDebug.Controls.Add(this.TextBoxDocName);
            this.TabPageDebug.Controls.Add(this.ButtonDeleteDocCalTest);
            this.TabPageDebug.Controls.Add(this.label3);
            this.TabPageDebug.Controls.Add(this.TextBoxDocUserName);
            this.TabPageDebug.Controls.Add(this.label2);
            this.TabPageDebug.Location = new System.Drawing.Point(4, 22);
            this.TabPageDebug.Name = "TabPageDebug";
            this.TabPageDebug.Size = new System.Drawing.Size(1047, 536);
            this.TabPageDebug.TabIndex = 3;
            this.TabPageDebug.Text = "TabPageDebug";
            this.TabPageDebug.UseVisualStyleBackColor = true;
            // 
            // TextBoxPath
            // 
            this.TextBoxPath.Location = new System.Drawing.Point(172, 29);
            this.TextBoxPath.Name = "TextBoxPath";
            this.TextBoxPath.Size = new System.Drawing.Size(416, 20);
            this.TextBoxPath.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(134, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Path:";
            // 
            // TextBoxDocName
            // 
            this.TextBoxDocName.Location = new System.Drawing.Point(172, 55);
            this.TextBoxDocName.Name = "TextBoxDocName";
            this.TextBoxDocName.Size = new System.Drawing.Size(416, 20);
            this.TextBoxDocName.TabIndex = 13;
            // 
            // ButtonDeleteDocCalTest
            // 
            this.ButtonDeleteDocCalTest.Location = new System.Drawing.Point(3, 36);
            this.ButtonDeleteDocCalTest.Name = "ButtonDeleteDocCalTest";
            this.ButtonDeleteDocCalTest.Size = new System.Drawing.Size(75, 39);
            this.ButtonDeleteDocCalTest.TabIndex = 11;
            this.ButtonDeleteDocCalTest.Text = "Delete Doc CAL Test";
            this.ButtonDeleteDocCalTest.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(107, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Document:";
            // 
            // TextBoxDocUserName
            // 
            this.TextBoxDocUserName.Location = new System.Drawing.Point(172, 3);
            this.TextBoxDocUserName.Name = "TextBoxDocUserName";
            this.TextBoxDocUserName.Size = new System.Drawing.Size(416, 20);
            this.TextBoxDocUserName.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(120, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "User ID:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Server:";
            // 
            // ComboBoxServer
            // 
            this.ComboBoxServer.FormattingEnabled = true;
            this.ComboBoxServer.Items.AddRange(new object[] {
            "localhost"});
            this.ComboBoxServer.Location = new System.Drawing.Point(60, 10);
            this.ComboBoxServer.Name = "ComboBoxServer";
            this.ComboBoxServer.Size = new System.Drawing.Size(150, 21);
            this.ComboBoxServer.TabIndex = 6;
            // 
            // LabelCheckedCount
            // 
            this.LabelCheckedCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelCheckedCount.Location = new System.Drawing.Point(909, 8);
            this.LabelCheckedCount.Name = "LabelCheckedCount";
            this.LabelCheckedCount.Size = new System.Drawing.Size(119, 23);
            this.LabelCheckedCount.TabIndex = 9;
            this.LabelCheckedCount.Text = "0 Rows Checked";
            this.LabelCheckedCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(84, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Don\'t Select More Recent";
            // 
            // NumericUpDownMinAge
            // 
            this.NumericUpDownMinAge.Increment = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.NumericUpDownMinAge.Location = new System.Drawing.Point(122, 18);
            this.NumericUpDownMinAge.Maximum = new decimal(new int[] {
            168,
            0,
            0,
            0});
            this.NumericUpDownMinAge.Minimum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.NumericUpDownMinAge.Name = "NumericUpDownMinAge";
            this.NumericUpDownMinAge.ReadOnly = true;
            this.NumericUpDownMinAge.Size = new System.Drawing.Size(41, 20);
            this.NumericUpDownMinAge.TabIndex = 11;
            this.NumericUpDownMinAge.Value = new decimal(new int[] {
            48,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(169, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Hours";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(84, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Than";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 611);
            this.Controls.Add(this.ComboBoxServer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(650, 280);
            this.Name = "Form1";
            this.Text = "PRM Server Monitor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.TabPageTasks.ResumeLayout(false);
            this.TabPageDocCals.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewDocCals)).EndInit();
            this.TabPageUserCals.ResumeLayout(false);
            this.TabPageUserCals.PerformLayout();
            this.TabPageDebug.ResumeLayout(false);
            this.TabPageDebug.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownMinAge)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ButtonRemoveOrphanTasks;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabPageTasks;
        private System.Windows.Forms.TabPage TabPageDocCals;
        private System.Windows.Forms.TabPage TabPageUserCals;
        private System.Windows.Forms.Button ButtonCalReport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ComboBoxServer;
        private System.Windows.Forms.Button ButtonDeleteTest;
        private System.Windows.Forms.TextBox TextBoxUserName;
        private System.Windows.Forms.Button ButtonEnumerateDocCALs;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView DataGridViewDocCals;
        private System.Windows.Forms.CheckBox CheckBoxAllowUndatedCalSelection;
        private System.Windows.Forms.Button ButtonClearAllDeleteChecks;
        private System.Windows.Forms.Button ButtonDeleteSelectedCals;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDocument;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnUserId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLastAccessDateTime;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnDelete;
        private System.Windows.Forms.TabPage TabPageDebug;
        private System.Windows.Forms.TextBox TextBoxPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TextBoxDocName;
        private System.Windows.Forms.Button ButtonDeleteDocCalTest;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBoxDocUserName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LabelCheckedCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown NumericUpDownMinAge;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
    }
}

