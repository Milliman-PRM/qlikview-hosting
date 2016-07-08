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
            this.grpLog.Location = new System.Drawing.Point(12, 12);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(778, 512);
            this.grpLog.TabIndex = 2;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log Report Generator";
            // 
            // txtFolderBrowser
            // 
            this.txtFolderBrowser.Location = new System.Drawing.Point(105, 458);
            this.txtFolderBrowser.Name = "txtFolderBrowser";
            this.txtFolderBrowser.Size = new System.Drawing.Size(311, 39);
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
            this.groupBox4.Location = new System.Drawing.Point(12, 213);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(648, 68);
            this.groupBox4.TabIndex = 28;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Report Date Selection";
            // 
            // dtBeginDate
            // 
            this.dtBeginDate.Location = new System.Drawing.Point(96, 32);
            this.dtBeginDate.Name = "dtBeginDate";
            this.dtBeginDate.Size = new System.Drawing.Size(200, 22);
            this.dtBeginDate.TabIndex = 10;
            // 
            // lblBeginDate
            // 
            this.lblBeginDate.AutoSize = true;
            this.lblBeginDate.Location = new System.Drawing.Point(7, 32);
            this.lblBeginDate.Name = "lblBeginDate";
            this.lblBeginDate.Size = new System.Drawing.Size(83, 17);
            this.lblBeginDate.TabIndex = 11;
            this.lblBeginDate.Text = "Degin Date:";
            // 
            // dtEndDate
            // 
            this.dtEndDate.Location = new System.Drawing.Point(413, 32);
            this.dtEndDate.Name = "dtEndDate";
            this.dtEndDate.Size = new System.Drawing.Size(200, 22);
            this.dtEndDate.TabIndex = 12;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(324, 32);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(71, 17);
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
            this.groupBox3.Location = new System.Drawing.Point(12, 25);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(747, 184);
            this.groupBox3.TabIndex = 26;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Report Type:";
            // 
            // btnAbout
            // 
            this.btnAbout.Image = ((System.Drawing.Image)(resources.GetObject("btnAbout.Image")));
            this.btnAbout.Location = new System.Drawing.Point(712, 18);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(29, 28);
            this.btnAbout.TabIndex = 30;
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // lblReportType
            // 
            this.lblReportType.AutoSize = true;
            this.lblReportType.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReportType.Location = new System.Drawing.Point(115, 29);
            this.lblReportType.Name = "lblReportType";
            this.lblReportType.Size = new System.Drawing.Size(272, 17);
            this.lblReportType.TabIndex = 12;
            this.lblReportType.Text = "You can select only one report type:";
            // 
            // ddlUserName
            // 
            this.ddlUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlUserName.FormattingEnabled = true;
            this.ddlUserName.Location = new System.Drawing.Point(118, 135);
            this.ddlUserName.Name = "ddlUserName";
            this.ddlUserName.Size = new System.Drawing.Size(623, 28);
            this.ddlUserName.TabIndex = 10;
            this.toolTip.SetToolTip(this.ddlUserName, "Select a User Name");
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(19, 139);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(83, 17);
            this.lblUserName.TabIndex = 11;
            this.lblUserName.Text = "User Name:";
            // 
            // ddlReportName
            // 
            this.ddlReportName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlReportName.FormattingEnabled = true;
            this.ddlReportName.Location = new System.Drawing.Point(118, 94);
            this.ddlReportName.Name = "ddlReportName";
            this.ddlReportName.Size = new System.Drawing.Size(623, 28);
            this.ddlReportName.TabIndex = 8;
            this.toolTip.SetToolTip(this.ddlReportName, "Select a Report Name");
            // 
            // lblReportName
            // 
            this.lblReportName.AutoSize = true;
            this.lblReportName.Location = new System.Drawing.Point(18, 99);
            this.lblReportName.Name = "lblReportName";
            this.lblReportName.Size = new System.Drawing.Size(96, 17);
            this.lblReportName.TabIndex = 9;
            this.lblReportName.Text = "Report Name:";
            // 
            // ddlGroupName
            // 
            this.ddlGroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlGroupName.FormattingEnabled = true;
            this.ddlGroupName.Location = new System.Drawing.Point(118, 53);
            this.ddlGroupName.Name = "ddlGroupName";
            this.ddlGroupName.Size = new System.Drawing.Size(623, 28);
            this.ddlGroupName.TabIndex = 6;
            this.toolTip.SetToolTip(this.ddlGroupName, "Select a group Name");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "Group Name:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdoText);
            this.groupBox2.Controls.Add(this.rdoCsv);
            this.groupBox2.Controls.Add(this.rdoExcel);
            this.groupBox2.Location = new System.Drawing.Point(105, 388);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(312, 64);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Report Output Type";
            this.toolTip.SetToolTip(this.groupBox2, "Select an Output file type");
            // 
            // rdoText
            // 
            this.rdoText.AutoSize = true;
            this.rdoText.Enabled = false;
            this.rdoText.Location = new System.Drawing.Point(212, 28);
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
            this.rdoCsv.Location = new System.Drawing.Point(40, 28);
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
            this.rdoExcel.Location = new System.Drawing.Point(121, 28);
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
            this.groupBox1.Location = new System.Drawing.Point(425, 287);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(164, 95);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Report Category Type";
            this.toolTip.SetToolTip(this.groupBox1, "Select a report category type");
            // 
            // rdoBtnUser
            // 
            this.rdoBtnUser.AutoSize = true;
            this.rdoBtnUser.Enabled = false;
            this.rdoBtnUser.Location = new System.Drawing.Point(40, 70);
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
            this.rdoBtnGroup.Enabled = false;
            this.rdoBtnGroup.Location = new System.Drawing.Point(40, 26);
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
            this.rdoBtnReport.Enabled = false;
            this.rdoBtnReport.Location = new System.Drawing.Point(40, 49);
            this.rdoBtnReport.Name = "rdoBtnReport";
            this.rdoBtnReport.Size = new System.Drawing.Size(72, 21);
            this.rdoBtnReport.TabIndex = 19;
            this.rdoBtnReport.TabStop = true;
            this.rdoBtnReport.Text = "Report";
            this.rdoBtnReport.UseVisualStyleBackColor = true;
            // 
            // btnGenerateReport
            // 
            this.btnGenerateReport.Location = new System.Drawing.Point(433, 446);
            this.btnGenerateReport.Name = "btnGenerateReport";
            this.btnGenerateReport.Size = new System.Drawing.Size(156, 51);
            this.btnGenerateReport.TabIndex = 23;
            this.btnGenerateReport.Text = "Generate Report";
            this.btnGenerateReport.UseVisualStyleBackColor = true;
            this.btnGenerateReport.Click += new System.EventHandler(this.btnGenerateReport_Click);
            // 
            // lblOutputPath
            // 
            this.lblOutputPath.AutoSize = true;
            this.lblOutputPath.Location = new System.Drawing.Point(9, 462);
            this.lblOutputPath.Name = "lblOutputPath";
            this.lblOutputPath.Size = new System.Drawing.Size(67, 17);
            this.lblOutputPath.TabIndex = 21;
            this.lblOutputPath.Text = "File Path:";
            // 
            // groReportTypes
            // 
            this.groReportTypes.Controls.Add(this.lblLotTypeCat);
            this.groReportTypes.Controls.Add(this.chkListLogType);
            this.groReportTypes.Location = new System.Drawing.Point(105, 287);
            this.groReportTypes.Name = "groReportTypes";
            this.groReportTypes.Size = new System.Drawing.Size(312, 95);
            this.groReportTypes.TabIndex = 19;
            this.groReportTypes.TabStop = false;
            this.groReportTypes.Text = "Log Category Types";
            // 
            // lblLotTypeCat
            // 
            this.lblLotTypeCat.AutoSize = true;
            this.lblLotTypeCat.Location = new System.Drawing.Point(18, 44);
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
            this.chkListLogType.Location = new System.Drawing.Point(104, 27);
            this.chkListLogType.Name = "chkListLogType";
            this.chkListLogType.Size = new System.Drawing.Size(182, 55);
            this.chkListLogType.TabIndex = 14;
            this.toolTip.SetToolTip(this.chkListLogType, "Select one or more log types");
            // 
            // SystemReporting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(801, 535);
            this.Controls.Add(this.grpLog);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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

