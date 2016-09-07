using System;
using System.Drawing;
using System.Windows.Forms;

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

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to close me?",
                    "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SystemReporting));
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.txtFolderBrowser = new System.Windows.Forms.RichTextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dtBeginDate = new System.Windows.Forms.DateTimePicker();
            this.lblBeginDate = new System.Windows.Forms.Label();
            this.dtEndDate = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnAbout = new System.Windows.Forms.Button();
            this.lblReportType = new System.Windows.Forms.Label();
            this.ddlUserName = new System.Windows.Forms.ComboBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.ddlReportName = new System.Windows.Forms.ComboBox();
            this.lblReportName = new System.Windows.Forms.Label();
            this.ddlGroupName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdoText = new System.Windows.Forms.RadioButton();
            this.rdoCsv = new System.Windows.Forms.RadioButton();
            this.rdoExcel = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoBtnUser = new System.Windows.Forms.RadioButton();
            this.rdoBtnGroup = new System.Windows.Forms.RadioButton();
            this.rdoBtnReport = new System.Windows.Forms.RadioButton();
            this.btnGenerateReport = new System.Windows.Forms.Button();
            this.lblOutputPath = new System.Windows.Forms.Label();
            this.groReportTypes = new System.Windows.Forms.GroupBox();
            this.lblLotTypeCat = new System.Windows.Forms.Label();
            this.chkListLogType = new System.Windows.Forms.CheckedListBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.grpLog.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groReportTypes.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpLog
            // 
            this.grpLog.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.grpLog.Controls.Add(this.txtFolderBrowser);
            this.grpLog.Controls.Add(this.groupBox4);
            this.grpLog.Controls.Add(this.groupBox3);
            this.grpLog.Controls.Add(this.groupBox2);
            this.grpLog.Controls.Add(this.groupBox1);
            this.grpLog.Controls.Add(this.btnGenerateReport);
            this.grpLog.Controls.Add(this.lblOutputPath);
            this.grpLog.Controls.Add(this.groReportTypes);
            this.grpLog.Location = new System.Drawing.Point(9, 10);
            this.grpLog.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpLog.Name = "grpLog";
            this.grpLog.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grpLog.Size = new System.Drawing.Size(584, 416);
            this.grpLog.TabIndex = 2;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log Report Generator";
            // 
            // txtFolderBrowser
            // 
            this.txtFolderBrowser.Location = new System.Drawing.Point(79, 372);
            this.txtFolderBrowser.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtFolderBrowser.Name = "txtFolderBrowser";
            this.txtFolderBrowser.Size = new System.Drawing.Size(234, 32);
            this.txtFolderBrowser.TabIndex = 29;
            this.txtFolderBrowser.Text = "";
            this.toolTip.SetToolTip(this.txtFolderBrowser, "Double click the text box");
            this.txtFolderBrowser.DoubleClick += new System.EventHandler(this.txtFolderBrowser_DoubleClick_1);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dtBeginDate);
            this.groupBox4.Controls.Add(this.lblBeginDate);
            this.groupBox4.Controls.Add(this.dtEndDate);
            this.groupBox4.Controls.Add(this.lblEndDate);
            this.groupBox4.Location = new System.Drawing.Point(9, 173);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Size = new System.Drawing.Size(486, 55);
            this.groupBox4.TabIndex = 28;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Report Date Selection";
            // 
            // dtBeginDate
            // 
            this.dtBeginDate.Location = new System.Drawing.Point(72, 26);
            this.dtBeginDate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dtBeginDate.Name = "dtBeginDate";
            this.dtBeginDate.Size = new System.Drawing.Size(151, 20);
            this.dtBeginDate.TabIndex = 10;
            // 
            // lblBeginDate
            // 
            this.lblBeginDate.AutoSize = true;
            this.lblBeginDate.Location = new System.Drawing.Point(5, 26);
            this.lblBeginDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBeginDate.Name = "lblBeginDate";
            this.lblBeginDate.Size = new System.Drawing.Size(64, 13);
            this.lblBeginDate.TabIndex = 11;
            this.lblBeginDate.Text = "Degin Date:";
            // 
            // dtEndDate
            // 
            this.dtEndDate.Location = new System.Drawing.Point(310, 26);
            this.dtEndDate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dtEndDate.Name = "dtEndDate";
            this.dtEndDate.Size = new System.Drawing.Size(151, 20);
            this.dtEndDate.TabIndex = 12;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(243, 26);
            this.lblEndDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(55, 13);
            this.lblEndDate.TabIndex = 13;
            this.lblEndDate.Text = "End Date:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnAbout);
            this.groupBox3.Controls.Add(this.lblReportType);
            this.groupBox3.Controls.Add(this.ddlUserName);
            this.groupBox3.Controls.Add(this.lblUserName);
            this.groupBox3.Controls.Add(this.ddlReportName);
            this.groupBox3.Controls.Add(this.lblReportName);
            this.groupBox3.Controls.Add(this.ddlGroupName);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(9, 20);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Size = new System.Drawing.Size(560, 150);
            this.groupBox3.TabIndex = 26;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Report Type:";
            // 
            // btnAbout
            // 
            this.btnAbout.Image = ((System.Drawing.Image)(resources.GetObject("btnAbout.Image")));
            this.btnAbout.Location = new System.Drawing.Point(534, 15);
            this.btnAbout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(22, 23);
            this.btnAbout.TabIndex = 30;
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // lblReportType
            // 
            this.lblReportType.AutoSize = true;
            this.lblReportType.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReportType.Location = new System.Drawing.Point(86, 24);
            this.lblReportType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblReportType.Name = "lblReportType";
            this.lblReportType.Size = new System.Drawing.Size(213, 13);
            this.lblReportType.TabIndex = 12;
            this.lblReportType.Text = "You can select only one report type:";
            // 
            // ddlUserName
            // 
            this.ddlUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlUserName.FormattingEnabled = true;
            this.ddlUserName.Location = new System.Drawing.Point(88, 110);
            this.ddlUserName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ddlUserName.Name = "ddlUserName";
            this.ddlUserName.Size = new System.Drawing.Size(468, 24);
            this.ddlUserName.TabIndex = 10;
            this.toolTip.SetToolTip(this.ddlUserName, "Select a User Name");
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(14, 113);
            this.lblUserName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(63, 13);
            this.lblUserName.TabIndex = 11;
            this.lblUserName.Text = "User Name:";
            // 
            // ddlReportName
            // 
            this.ddlReportName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlReportName.FormattingEnabled = true;
            this.ddlReportName.Location = new System.Drawing.Point(88, 76);
            this.ddlReportName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ddlReportName.Name = "ddlReportName";
            this.ddlReportName.Size = new System.Drawing.Size(468, 24);
            this.ddlReportName.TabIndex = 8;
            this.toolTip.SetToolTip(this.ddlReportName, "Select a Report Name");
            // 
            // lblReportName
            // 
            this.lblReportName.AutoSize = true;
            this.lblReportName.Location = new System.Drawing.Point(14, 80);
            this.lblReportName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblReportName.Name = "lblReportName";
            this.lblReportName.Size = new System.Drawing.Size(73, 13);
            this.lblReportName.TabIndex = 9;
            this.lblReportName.Text = "Report Name:";
            // 
            // ddlGroupName
            // 
            this.ddlGroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlGroupName.FormattingEnabled = true;
            this.ddlGroupName.Location = new System.Drawing.Point(88, 43);
            this.ddlGroupName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ddlGroupName.Name = "ddlGroupName";
            this.ddlGroupName.Size = new System.Drawing.Size(468, 24);
            this.ddlGroupName.TabIndex = 6;
            this.toolTip.SetToolTip(this.ddlGroupName, "Select a group Name");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Group Name:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdoText);
            this.groupBox2.Controls.Add(this.rdoCsv);
            this.groupBox2.Controls.Add(this.rdoExcel);
            this.groupBox2.Location = new System.Drawing.Point(79, 315);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Size = new System.Drawing.Size(234, 52);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Report Output Type";
            this.toolTip.SetToolTip(this.groupBox2, "Select an Output file type");
            // 
            // rdoText
            // 
            this.rdoText.AutoSize = true;
            this.rdoText.Enabled = false;
            this.rdoText.Location = new System.Drawing.Point(159, 23);
            this.rdoText.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rdoText.Name = "rdoText";
            this.rdoText.Size = new System.Drawing.Size(53, 17);
            this.rdoText.TabIndex = 17;
            this.rdoText.TabStop = true;
            this.rdoText.Text = "TEXT";
            this.rdoText.UseVisualStyleBackColor = true;
            // 
            // rdoCsv
            // 
            this.rdoCsv.AutoSize = true;
            this.rdoCsv.Location = new System.Drawing.Point(30, 23);
            this.rdoCsv.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rdoCsv.Name = "rdoCsv";
            this.rdoCsv.Size = new System.Drawing.Size(46, 17);
            this.rdoCsv.TabIndex = 14;
            this.rdoCsv.TabStop = true;
            this.rdoCsv.Text = "CSV";
            this.rdoCsv.UseVisualStyleBackColor = true;
            // 
            // rdoExcel
            // 
            this.rdoExcel.AutoSize = true;
            this.rdoExcel.Location = new System.Drawing.Point(91, 23);
            this.rdoExcel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rdoExcel.Name = "rdoExcel";
            this.rdoExcel.Size = new System.Drawing.Size(59, 17);
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
            this.groupBox1.Location = new System.Drawing.Point(319, 233);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(123, 77);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Report Category Type";
            this.toolTip.SetToolTip(this.groupBox1, "Select a report category type");
            // 
            // rdoBtnUser
            // 
            this.rdoBtnUser.AutoSize = true;
            this.rdoBtnUser.Enabled = false;
            this.rdoBtnUser.Location = new System.Drawing.Point(30, 57);
            this.rdoBtnUser.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rdoBtnUser.Name = "rdoBtnUser";
            this.rdoBtnUser.Size = new System.Drawing.Size(47, 17);
            this.rdoBtnUser.TabIndex = 20;
            this.rdoBtnUser.TabStop = true;
            this.rdoBtnUser.Text = "User";
            this.rdoBtnUser.UseVisualStyleBackColor = true;
            // 
            // rdoBtnGroup
            // 
            this.rdoBtnGroup.AutoSize = true;
            this.rdoBtnGroup.Enabled = false;
            this.rdoBtnGroup.Location = new System.Drawing.Point(30, 21);
            this.rdoBtnGroup.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rdoBtnGroup.Name = "rdoBtnGroup";
            this.rdoBtnGroup.Size = new System.Drawing.Size(54, 17);
            this.rdoBtnGroup.TabIndex = 18;
            this.rdoBtnGroup.TabStop = true;
            this.rdoBtnGroup.Text = "Group";
            this.rdoBtnGroup.UseVisualStyleBackColor = true;
            // 
            // rdoBtnReport
            // 
            this.rdoBtnReport.AutoSize = true;
            this.rdoBtnReport.Enabled = false;
            this.rdoBtnReport.Location = new System.Drawing.Point(30, 40);
            this.rdoBtnReport.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rdoBtnReport.Name = "rdoBtnReport";
            this.rdoBtnReport.Size = new System.Drawing.Size(57, 17);
            this.rdoBtnReport.TabIndex = 19;
            this.rdoBtnReport.TabStop = true;
            this.rdoBtnReport.Text = "Report";
            this.rdoBtnReport.UseVisualStyleBackColor = true;
            // 
            // btnGenerateReport
            // 
            this.btnGenerateReport.Location = new System.Drawing.Point(325, 362);
            this.btnGenerateReport.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnGenerateReport.Name = "btnGenerateReport";
            this.btnGenerateReport.Size = new System.Drawing.Size(117, 41);
            this.btnGenerateReport.TabIndex = 23;
            this.btnGenerateReport.Text = "Generate Report";
            this.btnGenerateReport.UseVisualStyleBackColor = true;
            this.btnGenerateReport.Click += new System.EventHandler(this.btnGenerateReport_Click);
            // 
            // lblOutputPath
            // 
            this.lblOutputPath.AutoSize = true;
            this.lblOutputPath.Location = new System.Drawing.Point(7, 375);
            this.lblOutputPath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOutputPath.Name = "lblOutputPath";
            this.lblOutputPath.Size = new System.Drawing.Size(51, 13);
            this.lblOutputPath.TabIndex = 21;
            this.lblOutputPath.Text = "File Path:";
            // 
            // groReportTypes
            // 
            this.groReportTypes.Controls.Add(this.lblLotTypeCat);
            this.groReportTypes.Controls.Add(this.chkListLogType);
            this.groReportTypes.Location = new System.Drawing.Point(79, 233);
            this.groReportTypes.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groReportTypes.Name = "groReportTypes";
            this.groReportTypes.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groReportTypes.Size = new System.Drawing.Size(234, 77);
            this.groReportTypes.TabIndex = 19;
            this.groReportTypes.TabStop = false;
            this.groReportTypes.Text = "Log Category Types";
            // 
            // lblLotTypeCat
            // 
            this.lblLotTypeCat.AutoSize = true;
            this.lblLotTypeCat.Location = new System.Drawing.Point(14, 36);
            this.lblLotTypeCat.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLotTypeCat.Name = "lblLotTypeCat";
            this.lblLotTypeCat.Size = new System.Drawing.Size(55, 13);
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
            this.chkListLogType.Location = new System.Drawing.Point(78, 22);
            this.chkListLogType.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkListLogType.Name = "chkListLogType";
            this.chkListLogType.Size = new System.Drawing.Size(138, 34);
            this.chkListLogType.TabIndex = 14;
            this.toolTip.SetToolTip(this.chkListLogType, "Select one or more log types");
            // 
            // SystemReporting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(601, 435);
            this.Controls.Add(this.grpLog);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "SystemReporting";
            this.Text = "System Reporting";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groReportTypes.ResumeLayout(false);
            this.groReportTypes.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.RadioButton rdoText;
        private System.Windows.Forms.RadioButton rdoExcel;
        private System.Windows.Forms.RadioButton rdoCsv;
        private System.Windows.Forms.GroupBox groReportTypes;
        private System.Windows.Forms.Label lblLotTypeCat;
        private System.Windows.Forms.CheckedListBox chkListLogType;
        private System.Windows.Forms.Label lblOutputPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button btnGenerateReport;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoBtnUser;
        private System.Windows.Forms.RadioButton rdoBtnGroup;
        private System.Windows.Forms.RadioButton rdoBtnReport;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox ddlUserName;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.ComboBox ddlReportName;
        private System.Windows.Forms.Label lblReportName;
        private System.Windows.Forms.ComboBox ddlGroupName;
        private System.Windows.Forms.Label label1;
        private ToolTip toolTip;
        private RichTextBox txtFolderBrowser;
        private GroupBox groupBox4;
        private DateTimePicker dtBeginDate;
        private Label lblBeginDate;
        private DateTimePicker dtEndDate;
        private Label lblEndDate;
        private Label lblReportType;
        private Button btnAbout;
    }
}

