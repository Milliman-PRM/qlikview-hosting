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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ButtonRemoveOrphanTasks = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabPageTasks = new System.Windows.Forms.TabPage();
            this.TabPageDocCals = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.DataGridViewDocCals = new System.Windows.Forms.DataGridView();
            this.ColumnDocument = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnUserId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLastAccessDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDeleteDocCal = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.NumericUpDownDocCalMinAge = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.LabelDocCalCheckedCount = new System.Windows.Forms.Label();
            this.ButtonDeleteSelectedDocCals = new System.Windows.Forms.Button();
            this.ButtonClearAllDocCalDeleteChecks = new System.Windows.Forms.Button();
            this.CheckBoxAllowUndatedDocCalSelection = new System.Windows.Forms.CheckBox();
            this.ButtonRefreshDocCALs = new System.Windows.Forms.Button();
            this.TabPageNamedUserCals = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.DataGridViewNamedCals = new System.Windows.Forms.DataGridView();
            this.ButtonDeleteSelectedNamedCals = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.NumericUpDownNamedCalMinAge = new System.Windows.Forms.NumericUpDown();
            this.CheckBoxAllowUndatedNamedCalSelection = new System.Windows.Forms.CheckBox();
            this.ButtonClearAllNamedCalDeleteChecks = new System.Windows.Forms.Button();
            this.LabelNamedCalCheckedCount = new System.Windows.Forms.Label();
            this.ButtonRefreshNamedCALs = new System.Windows.Forms.Button();
            this.TabPageDebug = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TextBoxDocUserName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBoxPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ButtonDeleteDocCalTest = new System.Windows.Forms.Button();
            this.TextBoxDocName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ButtonCalReport = new System.Windows.Forms.Button();
            this.TextBoxUserName = new System.Windows.Forms.TextBox();
            this.ButtonDeleteTest = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ComboBoxServer = new System.Windows.Forms.ComboBox();
            this.CheckBoxLogToFile = new System.Windows.Forms.CheckBox();
            this.ColumnUserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLastNamedCalAccess = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDeleteNamedCal = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tabControl1.SuspendLayout();
            this.TabPageTasks.SuspendLayout();
            this.TabPageDocCals.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewDocCals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownDocCalMinAge)).BeginInit();
            this.TabPageNamedUserCals.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewNamedCals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownNamedCalMinAge)).BeginInit();
            this.TabPageDebug.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.tabControl1.Controls.Add(this.TabPageNamedUserCals);
            this.tabControl1.Controls.Add(this.TabPageDebug);
            this.tabControl1.Location = new System.Drawing.Point(12, 37);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(960, 562);
            this.tabControl1.TabIndex = 4;
            // 
            // TabPageTasks
            // 
            this.TabPageTasks.Controls.Add(this.ButtonRemoveOrphanTasks);
            this.TabPageTasks.Location = new System.Drawing.Point(4, 22);
            this.TabPageTasks.Name = "TabPageTasks";
            this.TabPageTasks.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageTasks.Size = new System.Drawing.Size(698, 536);
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
            this.TabPageDocCals.Size = new System.Drawing.Size(952, 536);
            this.TabPageDocCals.TabIndex = 1;
            this.TabPageDocCals.Text = "Qlikview Document CALs";
            this.TabPageDocCals.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
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
            this.splitContainer1.Panel2.Controls.Add(this.NumericUpDownDocCalMinAge);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.LabelDocCalCheckedCount);
            this.splitContainer1.Panel2.Controls.Add(this.ButtonDeleteSelectedDocCals);
            this.splitContainer1.Panel2.Controls.Add(this.ButtonClearAllDocCalDeleteChecks);
            this.splitContainer1.Panel2.Controls.Add(this.CheckBoxAllowUndatedDocCalSelection);
            this.splitContainer1.Panel2.Controls.Add(this.ButtonRefreshDocCALs);
            this.splitContainer1.Size = new System.Drawing.Size(939, 523);
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridViewDocCals.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridViewDocCals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewDocCals.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnDocument,
            this.ColumnUserId,
            this.ColumnLastAccessDateTime,
            this.ColumnDeleteDocCal});
            this.DataGridViewDocCals.Location = new System.Drawing.Point(3, 3);
            this.DataGridViewDocCals.Name = "DataGridViewDocCals";
            this.DataGridViewDocCals.Size = new System.Drawing.Size(933, 459);
            this.DataGridViewDocCals.TabIndex = 0;
            this.DataGridViewDocCals.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewDocCals_CellMouseUp);
            this.DataGridViewDocCals.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewDocCals_CellValueChanged);
            this.DataGridViewDocCals.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.DataGridView_RowPostPaint);
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
            // ColumnDeleteDocCal
            // 
            this.ColumnDeleteDocCal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ColumnDeleteDocCal.HeaderText = "Delete";
            this.ColumnDeleteDocCal.Name = "ColumnDeleteDocCal";
            this.ColumnDeleteDocCal.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnDeleteDocCal.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ColumnDeleteDocCal.Width = 63;
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(169, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Hours";
            // 
            // NumericUpDownDocCalMinAge
            // 
            this.NumericUpDownDocCalMinAge.Increment = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.NumericUpDownDocCalMinAge.Location = new System.Drawing.Point(122, 18);
            this.NumericUpDownDocCalMinAge.Maximum = new decimal(new int[] {
            168,
            0,
            0,
            0});
            this.NumericUpDownDocCalMinAge.Minimum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.NumericUpDownDocCalMinAge.Name = "NumericUpDownDocCalMinAge";
            this.NumericUpDownDocCalMinAge.ReadOnly = true;
            this.NumericUpDownDocCalMinAge.Size = new System.Drawing.Size(41, 20);
            this.NumericUpDownDocCalMinAge.TabIndex = 11;
            this.NumericUpDownDocCalMinAge.Value = new decimal(new int[] {
            72,
            0,
            0,
            0});
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
            // LabelDocCalCheckedCount
            // 
            this.LabelDocCalCheckedCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelDocCalCheckedCount.Location = new System.Drawing.Point(713, 8);
            this.LabelDocCalCheckedCount.Name = "LabelDocCalCheckedCount";
            this.LabelDocCalCheckedCount.Size = new System.Drawing.Size(119, 13);
            this.LabelDocCalCheckedCount.TabIndex = 9;
            this.LabelDocCalCheckedCount.Text = "0 Rows Checked";
            this.LabelDocCalCheckedCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ButtonDeleteSelectedDocCals
            // 
            this.ButtonDeleteSelectedDocCals.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ButtonDeleteSelectedDocCals.Location = new System.Drawing.Point(357, 3);
            this.ButtonDeleteSelectedDocCals.Name = "ButtonDeleteSelectedDocCals";
            this.ButtonDeleteSelectedDocCals.Size = new System.Drawing.Size(165, 46);
            this.ButtonDeleteSelectedDocCals.TabIndex = 8;
            this.ButtonDeleteSelectedDocCals.Text = "Delete Selected Doc CALs";
            this.ButtonDeleteSelectedDocCals.UseVisualStyleBackColor = true;
            this.ButtonDeleteSelectedDocCals.Click += new System.EventHandler(this.ButtonDeleteSelectedDocCals_Click);
            // 
            // ButtonClearAllDocCalDeleteChecks
            // 
            this.ButtonClearAllDocCalDeleteChecks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonClearAllDocCalDeleteChecks.Location = new System.Drawing.Point(861, 3);
            this.ButtonClearAllDocCalDeleteChecks.Name = "ButtonClearAllDocCalDeleteChecks";
            this.ButtonClearAllDocCalDeleteChecks.Size = new System.Drawing.Size(75, 23);
            this.ButtonClearAllDocCalDeleteChecks.TabIndex = 7;
            this.ButtonClearAllDocCalDeleteChecks.Text = "&Uncheck All";
            this.ButtonClearAllDocCalDeleteChecks.UseVisualStyleBackColor = true;
            this.ButtonClearAllDocCalDeleteChecks.Click += new System.EventHandler(this.ButtonClearAllDocCalDeleteChecks_Click);
            // 
            // CheckBoxAllowUndatedDocCalSelection
            // 
            this.CheckBoxAllowUndatedDocCalSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckBoxAllowUndatedDocCalSelection.AutoSize = true;
            this.CheckBoxAllowUndatedDocCalSelection.Location = new System.Drawing.Point(707, 34);
            this.CheckBoxAllowUndatedDocCalSelection.Name = "CheckBoxAllowUndatedDocCalSelection";
            this.CheckBoxAllowUndatedDocCalSelection.Size = new System.Drawing.Size(229, 17);
            this.CheckBoxAllowUndatedDocCalSelection.TabIndex = 6;
            this.CheckBoxAllowUndatedDocCalSelection.Text = "Allow Selection Of 0001-01-01 Dated CALs";
            this.CheckBoxAllowUndatedDocCalSelection.UseVisualStyleBackColor = true;
            this.CheckBoxAllowUndatedDocCalSelection.CheckedChanged += new System.EventHandler(this.CheckBoxAllowUndatedDocCalSelection_CheckedChanged);
            // 
            // ButtonRefreshDocCALs
            // 
            this.ButtonRefreshDocCALs.Location = new System.Drawing.Point(3, 3);
            this.ButtonRefreshDocCALs.Name = "ButtonRefreshDocCALs";
            this.ButtonRefreshDocCALs.Size = new System.Drawing.Size(75, 46);
            this.ButtonRefreshDocCALs.TabIndex = 5;
            this.ButtonRefreshDocCALs.Text = "&Refresh Table";
            this.ButtonRefreshDocCALs.UseVisualStyleBackColor = true;
            this.ButtonRefreshDocCALs.Click += new System.EventHandler(this.ButtonRefreshDocCALs_Click);
            // 
            // TabPageNamedUserCals
            // 
            this.TabPageNamedUserCals.Controls.Add(this.splitContainer2);
            this.TabPageNamedUserCals.Location = new System.Drawing.Point(4, 22);
            this.TabPageNamedUserCals.Name = "TabPageNamedUserCals";
            this.TabPageNamedUserCals.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageNamedUserCals.Size = new System.Drawing.Size(721, 536);
            this.TabPageNamedUserCals.TabIndex = 2;
            this.TabPageNamedUserCals.Text = "Qlikview Named User CALs";
            this.TabPageNamedUserCals.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(7, 7);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.DataGridViewNamedCals);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.ButtonDeleteSelectedNamedCals);
            this.splitContainer2.Panel2.Controls.Add(this.label8);
            this.splitContainer2.Panel2.Controls.Add(this.label9);
            this.splitContainer2.Panel2.Controls.Add(this.label10);
            this.splitContainer2.Panel2.Controls.Add(this.NumericUpDownNamedCalMinAge);
            this.splitContainer2.Panel2.Controls.Add(this.CheckBoxAllowUndatedNamedCalSelection);
            this.splitContainer2.Panel2.Controls.Add(this.ButtonClearAllNamedCalDeleteChecks);
            this.splitContainer2.Panel2.Controls.Add(this.LabelNamedCalCheckedCount);
            this.splitContainer2.Panel2.Controls.Add(this.ButtonRefreshNamedCALs);
            this.splitContainer2.Size = new System.Drawing.Size(709, 524);
            this.splitContainer2.SplitterDistance = 465;
            this.splitContainer2.TabIndex = 0;
            // 
            // DataGridViewNamedCals
            // 
            this.DataGridViewNamedCals.AllowUserToAddRows = false;
            this.DataGridViewNamedCals.AllowUserToDeleteRows = false;
            this.DataGridViewNamedCals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridViewNamedCals.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.DataGridViewNamedCals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewNamedCals.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnUserName,
            this.ColumnLastNamedCalAccess,
            this.ColumnDeleteNamedCal});
            this.DataGridViewNamedCals.Location = new System.Drawing.Point(3, 3);
            this.DataGridViewNamedCals.Name = "DataGridViewNamedCals";
            this.DataGridViewNamedCals.Size = new System.Drawing.Size(702, 459);
            this.DataGridViewNamedCals.TabIndex = 0;
            this.DataGridViewNamedCals.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewNamedCals_CellMouseUp);
            this.DataGridViewNamedCals.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewNamedCals_CellValueChanged);
            this.DataGridViewNamedCals.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.DataGridView_RowPostPaint);
            // 
            // ButtonDeleteSelectedNamedCals
            // 
            this.ButtonDeleteSelectedNamedCals.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ButtonDeleteSelectedNamedCals.Location = new System.Drawing.Point(241, 3);
            this.ButtonDeleteSelectedNamedCals.Name = "ButtonDeleteSelectedNamedCals";
            this.ButtonDeleteSelectedNamedCals.Size = new System.Drawing.Size(165, 46);
            this.ButtonDeleteSelectedNamedCals.TabIndex = 17;
            this.ButtonDeleteSelectedNamedCals.Text = "Delete Selected Named CALs";
            this.ButtonDeleteSelectedNamedCals.UseVisualStyleBackColor = true;
            this.ButtonDeleteSelectedNamedCals.Click += new System.EventHandler(this.ButtonDeleteSelectedNamedCals_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(84, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Than";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(169, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Hours";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(84, 3);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(130, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "Don\'t Select More Recent";
            // 
            // NumericUpDownNamedCalMinAge
            // 
            this.NumericUpDownNamedCalMinAge.Increment = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.NumericUpDownNamedCalMinAge.Location = new System.Drawing.Point(122, 18);
            this.NumericUpDownNamedCalMinAge.Maximum = new decimal(new int[] {
            168,
            0,
            0,
            0});
            this.NumericUpDownNamedCalMinAge.Minimum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.NumericUpDownNamedCalMinAge.Name = "NumericUpDownNamedCalMinAge";
            this.NumericUpDownNamedCalMinAge.ReadOnly = true;
            this.NumericUpDownNamedCalMinAge.Size = new System.Drawing.Size(41, 20);
            this.NumericUpDownNamedCalMinAge.TabIndex = 12;
            this.NumericUpDownNamedCalMinAge.Value = new decimal(new int[] {
            72,
            0,
            0,
            0});
            // 
            // CheckBoxAllowUndatedNamedCalSelection
            // 
            this.CheckBoxAllowUndatedNamedCalSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckBoxAllowUndatedNamedCalSelection.AutoSize = true;
            this.CheckBoxAllowUndatedNamedCalSelection.Location = new System.Drawing.Point(476, 34);
            this.CheckBoxAllowUndatedNamedCalSelection.Name = "CheckBoxAllowUndatedNamedCalSelection";
            this.CheckBoxAllowUndatedNamedCalSelection.Size = new System.Drawing.Size(229, 17);
            this.CheckBoxAllowUndatedNamedCalSelection.TabIndex = 7;
            this.CheckBoxAllowUndatedNamedCalSelection.Text = "Allow Selection Of 0001-01-01 Dated CALs";
            this.CheckBoxAllowUndatedNamedCalSelection.UseVisualStyleBackColor = true;
            this.CheckBoxAllowUndatedNamedCalSelection.CheckedChanged += new System.EventHandler(this.CheckBoxAllowUndatedNamedCalSelection_CheckedChanged);
            // 
            // ButtonClearAllNamedCalDeleteChecks
            // 
            this.ButtonClearAllNamedCalDeleteChecks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonClearAllNamedCalDeleteChecks.Location = new System.Drawing.Point(630, 3);
            this.ButtonClearAllNamedCalDeleteChecks.Name = "ButtonClearAllNamedCalDeleteChecks";
            this.ButtonClearAllNamedCalDeleteChecks.Size = new System.Drawing.Size(75, 23);
            this.ButtonClearAllNamedCalDeleteChecks.TabIndex = 2;
            this.ButtonClearAllNamedCalDeleteChecks.Text = "Uncheck All";
            this.ButtonClearAllNamedCalDeleteChecks.UseVisualStyleBackColor = true;
            this.ButtonClearAllNamedCalDeleteChecks.Click += new System.EventHandler(this.ButtonClearAllNamedCalDeleteChecks_Click);
            // 
            // LabelNamedCalCheckedCount
            // 
            this.LabelNamedCalCheckedCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelNamedCalCheckedCount.Location = new System.Drawing.Point(482, 8);
            this.LabelNamedCalCheckedCount.Name = "LabelNamedCalCheckedCount";
            this.LabelNamedCalCheckedCount.Size = new System.Drawing.Size(119, 13);
            this.LabelNamedCalCheckedCount.TabIndex = 1;
            this.LabelNamedCalCheckedCount.Text = "0";
            this.LabelNamedCalCheckedCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ButtonRefreshNamedCALs
            // 
            this.ButtonRefreshNamedCALs.Location = new System.Drawing.Point(3, 3);
            this.ButtonRefreshNamedCALs.Name = "ButtonRefreshNamedCALs";
            this.ButtonRefreshNamedCALs.Size = new System.Drawing.Size(75, 46);
            this.ButtonRefreshNamedCALs.TabIndex = 0;
            this.ButtonRefreshNamedCALs.Text = "Refresh Table";
            this.ButtonRefreshNamedCALs.UseVisualStyleBackColor = true;
            this.ButtonRefreshNamedCALs.Click += new System.EventHandler(this.ButtonRefreshNamedCALs_Click);
            // 
            // TabPageDebug
            // 
            this.TabPageDebug.Controls.Add(this.groupBox2);
            this.TabPageDebug.Controls.Add(this.groupBox1);
            this.TabPageDebug.Location = new System.Drawing.Point(4, 22);
            this.TabPageDebug.Name = "TabPageDebug";
            this.TabPageDebug.Size = new System.Drawing.Size(1152, 536);
            this.TabPageDebug.TabIndex = 3;
            this.TabPageDebug.Text = "TabPageDebug";
            this.TabPageDebug.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.TextBoxDocUserName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.TextBoxPath);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.ButtonDeleteDocCalTest);
            this.groupBox2.Controls.Add(this.TextBoxDocName);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(603, 107);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Document CALs";
            // 
            // TextBoxDocUserName
            // 
            this.TextBoxDocUserName.Location = new System.Drawing.Point(176, 19);
            this.TextBoxDocUserName.Name = "TextBoxDocUserName";
            this.TextBoxDocUserName.Size = new System.Drawing.Size(416, 20);
            this.TextBoxDocUserName.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(124, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "User ID:";
            // 
            // TextBoxPath
            // 
            this.TextBoxPath.Location = new System.Drawing.Point(176, 45);
            this.TextBoxPath.Name = "TextBoxPath";
            this.TextBoxPath.Size = new System.Drawing.Size(416, 20);
            this.TextBoxPath.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(111, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Document:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(138, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Path:";
            // 
            // ButtonDeleteDocCalTest
            // 
            this.ButtonDeleteDocCalTest.Location = new System.Drawing.Point(7, 52);
            this.ButtonDeleteDocCalTest.Name = "ButtonDeleteDocCalTest";
            this.ButtonDeleteDocCalTest.Size = new System.Drawing.Size(75, 39);
            this.ButtonDeleteDocCalTest.TabIndex = 11;
            this.ButtonDeleteDocCalTest.Text = "Delete Doc CAL Test";
            this.ButtonDeleteDocCalTest.UseVisualStyleBackColor = true;
            // 
            // TextBoxDocName
            // 
            this.TextBoxDocName.Location = new System.Drawing.Point(176, 71);
            this.TextBoxDocName.Name = "TextBoxDocName";
            this.TextBoxDocName.Size = new System.Drawing.Size(416, 20);
            this.TextBoxDocName.TabIndex = 13;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ButtonCalReport);
            this.groupBox1.Controls.Add(this.TextBoxUserName);
            this.groupBox1.Controls.Add(this.ButtonDeleteTest);
            this.groupBox1.Location = new System.Drawing.Point(3, 116);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(272, 93);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Named CALs";
            // 
            // ButtonCalReport
            // 
            this.ButtonCalReport.Location = new System.Drawing.Point(6, 19);
            this.ButtonCalReport.Name = "ButtonCalReport";
            this.ButtonCalReport.Size = new System.Drawing.Size(259, 23);
            this.ButtonCalReport.TabIndex = 18;
            this.ButtonCalReport.Text = "Report CALs";
            this.ButtonCalReport.UseVisualStyleBackColor = true;
            this.ButtonCalReport.Click += new System.EventHandler(this.ButtonCalReport_Click);
            // 
            // TextBoxUserName
            // 
            this.TextBoxUserName.Location = new System.Drawing.Point(88, 51);
            this.TextBoxUserName.Name = "TextBoxUserName";
            this.TextBoxUserName.Size = new System.Drawing.Size(177, 20);
            this.TextBoxUserName.TabIndex = 20;
            // 
            // ButtonDeleteTest
            // 
            this.ButtonDeleteTest.Location = new System.Drawing.Point(7, 49);
            this.ButtonDeleteTest.Name = "ButtonDeleteTest";
            this.ButtonDeleteTest.Size = new System.Drawing.Size(75, 23);
            this.ButtonDeleteTest.TabIndex = 19;
            this.ButtonDeleteTest.Text = "Delete Test";
            this.ButtonDeleteTest.UseVisualStyleBackColor = true;
            this.ButtonDeleteTest.Click += new System.EventHandler(this.ButtonDeleteNamedCalTest_Click);
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
            // CheckBoxLogToFile
            // 
            this.CheckBoxLogToFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckBoxLogToFile.AutoSize = true;
            this.CheckBoxLogToFile.Location = new System.Drawing.Point(893, 12);
            this.CheckBoxLogToFile.Name = "CheckBoxLogToFile";
            this.CheckBoxLogToFile.Size = new System.Drawing.Size(79, 17);
            this.CheckBoxLogToFile.TabIndex = 7;
            this.CheckBoxLogToFile.Text = "Log To File";
            this.CheckBoxLogToFile.UseVisualStyleBackColor = true;
            // 
            // ColumnUserName
            // 
            this.ColumnUserName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnUserName.HeaderText = "User ID";
            this.ColumnUserName.Name = "ColumnUserName";
            this.ColumnUserName.ReadOnly = true;
            // 
            // ColumnLastNamedCalAccess
            // 
            this.ColumnLastNamedCalAccess.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnLastNamedCalAccess.HeaderText = "Last Accessed";
            this.ColumnLastNamedCalAccess.Name = "ColumnLastNamedCalAccess";
            this.ColumnLastNamedCalAccess.ReadOnly = true;
            this.ColumnLastNamedCalAccess.Width = 102;
            // 
            // ColumnDeleteNamedCal
            // 
            this.ColumnDeleteNamedCal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnDeleteNamedCal.HeaderText = "Delete";
            this.ColumnDeleteNamedCal.Name = "ColumnDeleteNamedCal";
            this.ColumnDeleteNamedCal.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ColumnDeleteNamedCal.Width = 63;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 611);
            this.Controls.Add(this.CheckBoxLogToFile);
            this.Controls.Add(this.ComboBoxServer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(750, 280);
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
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownDocCalMinAge)).EndInit();
            this.TabPageNamedUserCals.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewNamedCals)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDownNamedCalMinAge)).EndInit();
            this.TabPageDebug.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ButtonRemoveOrphanTasks;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabPageTasks;
        private System.Windows.Forms.TabPage TabPageDocCals;
        private System.Windows.Forms.TabPage TabPageNamedUserCals;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ComboBoxServer;
        private System.Windows.Forms.Button ButtonRefreshDocCALs;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView DataGridViewDocCals;
        private System.Windows.Forms.CheckBox CheckBoxAllowUndatedDocCalSelection;
        private System.Windows.Forms.Button ButtonClearAllDocCalDeleteChecks;
        private System.Windows.Forms.Button ButtonDeleteSelectedDocCals;
        private System.Windows.Forms.TabPage TabPageDebug;
        private System.Windows.Forms.TextBox TextBoxPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TextBoxDocName;
        private System.Windows.Forms.Button ButtonDeleteDocCalTest;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBoxDocUserName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LabelDocCalCheckedCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown NumericUpDownDocCalMinAge;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ButtonCalReport;
        private System.Windows.Forms.TextBox TextBoxUserName;
        private System.Windows.Forms.Button ButtonDeleteTest;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView DataGridViewNamedCals;
        private System.Windows.Forms.Button ButtonRefreshNamedCALs;
        private System.Windows.Forms.Label LabelNamedCalCheckedCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDocument;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnUserId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLastAccessDateTime;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnDeleteDocCal;
        private System.Windows.Forms.Button ButtonClearAllNamedCalDeleteChecks;
        private System.Windows.Forms.CheckBox CheckBoxAllowUndatedNamedCalSelection;
        private System.Windows.Forms.NumericUpDown NumericUpDownNamedCalMinAge;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button ButtonDeleteSelectedNamedCals;
        private System.Windows.Forms.CheckBox CheckBoxLogToFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnUserName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLastNamedCalAccess;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnDeleteNamedCal;
    }
}

