namespace SystemReportingWinUI
{
    partial class SystemReporting
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
            this.ddlGroupName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdoText = new System.Windows.Forms.RadioButton();
            this.rdoCsv = new System.Windows.Forms.RadioButton();
            this.rdoExcel = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoBtnUser = new System.Windows.Forms.RadioButton();
            this.rdoBtnGroup = new System.Windows.Forms.RadioButton();
            this.rdoBtnReport = new System.Windows.Forms.RadioButton();
            this.btnGenerateReport = new System.Windows.Forms.Button();
            this.txtFolderBrowser = new System.Windows.Forms.TextBox();
            this.lblOutputPath = new System.Windows.Forms.Label();
            this.groReportTypes = new System.Windows.Forms.GroupBox();
            this.lblLotTypeCat = new System.Windows.Forms.Label();
            this.chkListLogType = new System.Windows.Forms.CheckedListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dtBeginDate = new System.Windows.Forms.DateTimePicker();
            this.lblBeginDate = new System.Windows.Forms.Label();
            this.dtEndDate = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.ddlUserName = new System.Windows.Forms.ComboBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.ddlReportName = new System.Windows.Forms.ComboBox();
            this.lblReportName = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.grpLog.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groReportTypes.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ddlGroupName
            // 
            this.ddlGroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlGroupName.FormattingEnabled = true;
            this.ddlGroupName.Location = new System.Drawing.Point(105, 21);
            this.ddlGroupName.Name = "ddlGroupName";
            this.ddlGroupName.Size = new System.Drawing.Size(589, 33);
            this.ddlGroupName.TabIndex = 0;
            this.ddlGroupName.SelectedIndexChanged += new System.EventHandler(this.ddlGroupName_SelectedIndexChanged);
            this.ddlGroupName.TextChanged += new System.EventHandler(this.ddlGroupName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Group Name:";
            // 
            // grpLog
            // 
            this.grpLog.BackColor = System.Drawing.Color.Bisque;
            this.grpLog.Controls.Add(this.groupBox2);
            this.grpLog.Controls.Add(this.groupBox1);
            this.grpLog.Controls.Add(this.btnGenerateReport);
            this.grpLog.Controls.Add(this.txtFolderBrowser);
            this.grpLog.Controls.Add(this.lblOutputPath);
            this.grpLog.Controls.Add(this.groReportTypes);
            this.grpLog.Controls.Add(this.panel1);
            this.grpLog.Controls.Add(this.ddlUserName);
            this.grpLog.Controls.Add(this.lblUserName);
            this.grpLog.Controls.Add(this.ddlReportName);
            this.grpLog.Controls.Add(this.lblReportName);
            this.grpLog.Controls.Add(this.ddlGroupName);
            this.grpLog.Controls.Add(this.label1);
            this.grpLog.Location = new System.Drawing.Point(12, 12);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(709, 628);
            this.grpLog.TabIndex = 2;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log Report Generator";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdoText);
            this.groupBox2.Controls.Add(this.rdoCsv);
            this.groupBox2.Controls.Add(this.rdoExcel);
            this.groupBox2.Location = new System.Drawing.Point(105, 425);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(312, 95);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Report Output Type";
            // 
            // rdoText
            // 
            this.rdoText.AutoSize = true;
            this.rdoText.Location = new System.Drawing.Point(180, 40);
            this.rdoText.Name = "rdoText";
            this.rdoText.Size = new System.Drawing.Size(65, 21);
            this.rdoText.TabIndex = 17;
            this.rdoText.TabStop = true;
            this.rdoText.Text = "TEXT";
            this.rdoText.UseVisualStyleBackColor = true;
            // 
            // rdoCsv
            // 
            this.rdoCsv.AutoSize = true;
            this.rdoCsv.Location = new System.Drawing.Point(8, 40);
            this.rdoCsv.Name = "rdoCsv";
            this.rdoCsv.Size = new System.Drawing.Size(56, 21);
            this.rdoCsv.TabIndex = 14;
            this.rdoCsv.TabStop = true;
            this.rdoCsv.Text = "CSV";
            this.rdoCsv.UseVisualStyleBackColor = true;
            // 
            // rdoExcel
            // 
            this.rdoExcel.AutoSize = true;
            this.rdoExcel.Location = new System.Drawing.Point(88, 40);
            this.rdoExcel.Name = "rdoExcel";
            this.rdoExcel.Size = new System.Drawing.Size(73, 21);
            this.rdoExcel.TabIndex = 16;
            this.rdoExcel.TabStop = true;
            this.rdoExcel.Text = "EXCEL";
            this.rdoExcel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoBtnUser);
            this.groupBox1.Controls.Add(this.rdoBtnGroup);
            this.groupBox1.Controls.Add(this.rdoBtnReport);
            this.groupBox1.Location = new System.Drawing.Point(105, 324);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312, 95);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Report Category Type";
            // 
            // rdoBtnUser
            // 
            this.rdoBtnUser.AutoSize = true;
            this.rdoBtnUser.Location = new System.Drawing.Point(180, 37);
            this.rdoBtnUser.Name = "rdoBtnUser";
            this.rdoBtnUser.Size = new System.Drawing.Size(59, 21);
            this.rdoBtnUser.TabIndex = 20;
            this.rdoBtnUser.TabStop = true;
            this.rdoBtnUser.Text = "User";
            this.rdoBtnUser.UseVisualStyleBackColor = true;
            // 
            // rdoBtnGroup
            // 
            this.rdoBtnGroup.AutoSize = true;
            this.rdoBtnGroup.Location = new System.Drawing.Point(8, 37);
            this.rdoBtnGroup.Name = "rdoBtnGroup";
            this.rdoBtnGroup.Size = new System.Drawing.Size(69, 21);
            this.rdoBtnGroup.TabIndex = 18;
            this.rdoBtnGroup.TabStop = true;
            this.rdoBtnGroup.Text = "Group";
            this.rdoBtnGroup.UseVisualStyleBackColor = true;
            // 
            // rdoBtnReport
            // 
            this.rdoBtnReport.AutoSize = true;
            this.rdoBtnReport.Location = new System.Drawing.Point(89, 37);
            this.rdoBtnReport.Name = "rdoBtnReport";
            this.rdoBtnReport.Size = new System.Drawing.Size(72, 21);
            this.rdoBtnReport.TabIndex = 19;
            this.rdoBtnReport.TabStop = true;
            this.rdoBtnReport.Text = "Report";
            this.rdoBtnReport.UseVisualStyleBackColor = true;
            // 
            // btnGenerateReport
            // 
            this.btnGenerateReport.Location = new System.Drawing.Point(130, 564);
            this.btnGenerateReport.Name = "btnGenerateReport";
            this.btnGenerateReport.Size = new System.Drawing.Size(156, 51);
            this.btnGenerateReport.TabIndex = 23;
            this.btnGenerateReport.Text = "Generate Report";
            this.btnGenerateReport.UseVisualStyleBackColor = true;
            this.btnGenerateReport.Click += new System.EventHandler(this.btnGenerateReport_Click);
            // 
            // txtFolderBrowser
            // 
            this.txtFolderBrowser.Location = new System.Drawing.Point(130, 536);
            this.txtFolderBrowser.Name = "txtFolderBrowser";
            this.txtFolderBrowser.Size = new System.Drawing.Size(398, 22);
            this.txtFolderBrowser.TabIndex = 22;
            this.txtFolderBrowser.DoubleClick += new System.EventHandler(this.txtFolderBrowser_DoubleClick);
            // 
            // lblOutputPath
            // 
            this.lblOutputPath.AutoSize = true;
            this.lblOutputPath.Location = new System.Drawing.Point(9, 536);
            this.lblOutputPath.Name = "lblOutputPath";
            this.lblOutputPath.Size = new System.Drawing.Size(114, 17);
            this.lblOutputPath.TabIndex = 21;
            this.lblOutputPath.Text = "Output File Path:";
            // 
            // groReportTypes
            // 
            this.groReportTypes.Controls.Add(this.lblLotTypeCat);
            this.groReportTypes.Controls.Add(this.chkListLogType);
            this.groReportTypes.Location = new System.Drawing.Point(105, 218);
            this.groReportTypes.Name = "groReportTypes";
            this.groReportTypes.Size = new System.Drawing.Size(312, 95);
            this.groReportTypes.TabIndex = 19;
            this.groReportTypes.TabStop = false;
            this.groReportTypes.Text = "Log Category Types";
            // 
            // lblLotTypeCat
            // 
            this.lblLotTypeCat.AutoSize = true;
            this.lblLotTypeCat.Location = new System.Drawing.Point(5, 32);
            this.lblLotTypeCat.Name = "lblLotTypeCat";
            this.lblLotTypeCat.Size = new System.Drawing.Size(72, 17);
            this.lblLotTypeCat.TabIndex = 15;
            this.lblLotTypeCat.Text = "Log Type:";
            // 
            // chkListLogType
            // 
            this.chkListLogType.FormattingEnabled = true;
            this.chkListLogType.Items.AddRange(new object[] {
            "Iis",
            "Session",
            "Audit"});
            this.chkListLogType.Location = new System.Drawing.Point(104, 32);
            this.chkListLogType.Name = "chkListLogType";
            this.chkListLogType.Size = new System.Drawing.Size(182, 55);
            this.chkListLogType.TabIndex = 14;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dtBeginDate);
            this.panel1.Controls.Add(this.lblBeginDate);
            this.panel1.Controls.Add(this.dtEndDate);
            this.panel1.Controls.Add(this.lblEndDate);
            this.panel1.Location = new System.Drawing.Point(105, 147);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(312, 65);
            this.panel1.TabIndex = 18;
            // 
            // dtBeginDate
            // 
            this.dtBeginDate.Location = new System.Drawing.Point(104, 3);
            this.dtBeginDate.Name = "dtBeginDate";
            this.dtBeginDate.Size = new System.Drawing.Size(200, 22);
            this.dtBeginDate.TabIndex = 6;
            // 
            // lblBeginDate
            // 
            this.lblBeginDate.AutoSize = true;
            this.lblBeginDate.Location = new System.Drawing.Point(5, 8);
            this.lblBeginDate.Name = "lblBeginDate";
            this.lblBeginDate.Size = new System.Drawing.Size(83, 17);
            this.lblBeginDate.TabIndex = 7;
            this.lblBeginDate.Text = "Degin Date:";
            // 
            // dtEndDate
            // 
            this.dtEndDate.Location = new System.Drawing.Point(104, 37);
            this.dtEndDate.Name = "dtEndDate";
            this.dtEndDate.Size = new System.Drawing.Size(200, 22);
            this.dtEndDate.TabIndex = 8;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(5, 37);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(71, 17);
            this.lblEndDate.TabIndex = 9;
            this.lblEndDate.Text = "End Date:";
            // 
            // ddlUserName
            // 
            this.ddlUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlUserName.FormattingEnabled = true;
            this.ddlUserName.Location = new System.Drawing.Point(105, 103);
            this.ddlUserName.Name = "ddlUserName";
            this.ddlUserName.Size = new System.Drawing.Size(589, 33);
            this.ddlUserName.TabIndex = 4;
            this.ddlUserName.SelectedIndexChanged += new System.EventHandler(this.ddlUserName_SelectedIndexChanged);
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(6, 88);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(83, 17);
            this.lblUserName.TabIndex = 5;
            this.lblUserName.Text = "User Name:";
            // 
            // ddlReportName
            // 
            this.ddlReportName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlReportName.FormattingEnabled = true;
            this.ddlReportName.Location = new System.Drawing.Point(105, 62);
            this.ddlReportName.Name = "ddlReportName";
            this.ddlReportName.Size = new System.Drawing.Size(589, 33);
            this.ddlReportName.TabIndex = 2;
            this.ddlReportName.SelectedIndexChanged += new System.EventHandler(this.ddlReportName_SelectedIndexChanged);
            // 
            // lblReportName
            // 
            this.lblReportName.AutoSize = true;
            this.lblReportName.Location = new System.Drawing.Point(6, 58);
            this.lblReportName.Name = "lblReportName";
            this.lblReportName.Size = new System.Drawing.Size(96, 17);
            this.lblReportName.TabIndex = 3;
            this.lblReportName.Text = "Report Name:";
            // 
            // SystemReporting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 645);
            this.Controls.Add(this.grpLog);
            this.Name = "SystemReporting";
            this.Text = "System Reporting";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groReportTypes.ResumeLayout(false);
            this.groReportTypes.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox ddlGroupName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.ComboBox ddlUserName;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.ComboBox ddlReportName;
        private System.Windows.Forms.Label lblReportName;
        private System.Windows.Forms.Label lblBeginDate;
        private System.Windows.Forms.DateTimePicker dtBeginDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.DateTimePicker dtEndDate;
        private System.Windows.Forms.RadioButton rdoText;
        private System.Windows.Forms.RadioButton rdoExcel;
        private System.Windows.Forms.RadioButton rdoCsv;
        private System.Windows.Forms.GroupBox groReportTypes;
        private System.Windows.Forms.Label lblLotTypeCat;
        private System.Windows.Forms.CheckedListBox chkListLogType;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblOutputPath;
        private System.Windows.Forms.TextBox txtFolderBrowser;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button btnGenerateReport;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoBtnUser;
        private System.Windows.Forms.RadioButton rdoBtnGroup;
        private System.Windows.Forms.RadioButton rdoBtnReport;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

