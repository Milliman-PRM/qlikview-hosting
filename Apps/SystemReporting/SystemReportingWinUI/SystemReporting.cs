using ReportFileGenerator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SystemReporting.Controller.BusinessLogic.Controller;
using SystemReporting.Entities.Models;

namespace SystemReportingWinUI
{
    public partial class SystemReporting : Form
    {
        int _selectedIndex;
        string _text;
        public SystemReporting()
        {
            InitializeComponent();
            PopulateDDLGroupName();
            PopulateDDLReportName();
            PopulateDDLUserName();
        }

        private void ddlGroupName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Called when a new index is selected.
            _selectedIndex = ddlGroupName.SelectedIndex;
            Display();
        }

        private void ddlReportName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Called when a new index is selected.
            _selectedIndex = ddlReportName.SelectedIndex;
            Display();

       }

        private void ddlUserName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Called when a new index is selected.
            _selectedIndex = ddlUserName.SelectedIndex;
            Display();
        }
        private void ddlGroupName_TextChanged(object sender, EventArgs e)
        {
            // Called whenever text changes.
            _text = ddlGroupName.Text;
            Display();
        }

        void Display()
        {
            this.Text = string.Format("Text: {0}; SelectedIndex: {1}",
                                        _text,
                                        _selectedIndex);
        }

        public void PopulateDDLGroupName()
        {
            var listResults = new List<Group>();
            listResults = BusinessLogicController.GetGroupList();
            ddlGroupName.DataSource = listResults;
            ddlGroupName.DisplayMember = "GroupName";
            ddlGroupName.ValueMember = "Id";
            ddlGroupName.SelectedIndex = -1;
        }
        public void PopulateDDLReportName()
        {
            var listResults = new List<Report>();
            listResults = BusinessLogicController.GetReportList();
            ddlReportName.DataSource = listResults;
            ddlReportName.DisplayMember = "ReportName";
            ddlReportName.ValueMember = "Id";
            ddlReportName.SelectedIndex = -1;            
        }
        public void PopulateDDLUserName()
        {
            var listResults = new List<User>();
            listResults = BusinessLogicController.GetUserList();
            ddlUserName.DataSource = listResults;
            ddlUserName.DisplayMember = "UserName";
            ddlUserName.ValueMember = "Id";
            ddlUserName.SelectedIndex = -1;            

        }

        private void txtFolderBrowser_DoubleClick(object sender, EventArgs e)
        {
            var R = folderBrowserDialog.ShowDialog();
            if (R == DialogResult.OK)
            {
                txtFolderBrowser.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            if (ddlGroupName.SelectedIndex > -1 && ddlReportName.SelectedIndex > -1 && ddlUserName.SelectedIndex > -1)
            {
                if (MessageBox.Show("You can only select one Category for report", "Close Application", MessageBoxButtons.OK) != DialogResult.OK)
                {

                }
            }
            else
            {
                var startDate = dtBeginDate.Value.Date;
                var endDate = dtEndDate.Value.Date;

                //set output file type //CSV, Excel or Txt
                var logCategoryTypeList = new List<string>();
                if (chkListLogType.Items.Count > 0)
                {
                    foreach (object itemChecked in chkListLogType.CheckedItems)
                    {
                        logCategoryTypeList.Add(itemChecked.ToString());
                    }
                }

                //select report category
                var reportCategoryType = string.Empty;
                if (rdoBtnGroup.Checked)
                {
                    reportCategoryType = rdoBtnGroup.Text;
                }
                else if (rdoBtnReport.Checked)
                {
                    reportCategoryType = rdoBtnReport.Text;
                }
                else if (rdoBtnUser.Checked)
                {
                    reportCategoryType = rdoBtnUser.Text;
                }

                //Get Report output type
                var reportOutPutType = string.Empty;
                var fileExtension = string.Empty;
                if (rdoCsv.Checked)
                {
                    reportOutPutType = rdoCsv.Text;
                    fileExtension = ".csv";
                }
                else if (rdoExcel.Checked)
                {
                    reportOutPutType = rdoExcel.Text;
                    fileExtension = ".xls";
                }
                else if (rdoText.Checked)
                {
                    reportOutPutType = rdoText.Text;
                    fileExtension = ".txt";
                }

                var fileNameWithFolderPath = string.Empty;
                var reportName = string.Empty;
                //generate
                foreach (var log in logCategoryTypeList)
                {
                    if (!string.IsNullOrEmpty(txtFolderBrowser.Text))
                        fileNameWithFolderPath = txtFolderBrowser.Text + "\\"+  log +"_"+ DateTime.Now.ToString("MMdd_hhmm") + fileExtension;

                    if (log == "Iis")
                    {
                        if (ddlGroupName.SelectedIndex > -1)
                        {
                            reportName = ddlGroupName.Text;
                            GenerateIisLogsReport.ProcessReportGenerateForGroupName(startDate, endDate, reportName, reportOutPutType, fileNameWithFolderPath);
                        }
                        if (ddlReportName.SelectedIndex > -1)
                        {
                            reportName = ddlReportName.Text;
                            GenerateIisLogsReport.ProcessReportGenerateForReportName(startDate, endDate, reportName, reportOutPutType, fileNameWithFolderPath);
                        }
                        if (ddlUserName.SelectedIndex > -1)
                        {
                            reportName = ddlUserName.Text;
                            GenerateIisLogsReport.ProcessReportGenerateForUserName(startDate, endDate, reportName, reportOutPutType, fileNameWithFolderPath);
                        }
                    }
                    if (log == "Audit")
                    {
                        if (ddlGroupName.SelectedIndex > -1)
                        {
                            reportName = ddlGroupName.Text;
                            GenerateQVAuditLogsReport.ProcessReportGenerateForGroupName(startDate, endDate, reportName, reportOutPutType, fileNameWithFolderPath);
                        }
                        if (ddlReportName.SelectedIndex > -1)
                        {
                            reportName = ddlReportName.Text;
                            GenerateQVAuditLogsReport.ProcessReportGenerateForReportName(startDate, endDate, reportName, reportOutPutType, fileNameWithFolderPath);
                        }
                        if (ddlUserName.SelectedIndex > -1)
                        {
                            reportName = ddlUserName.Text;
                            GenerateQVAuditLogsReport.ProcessReportGenerateForUserName(startDate, endDate, reportName, reportOutPutType, fileNameWithFolderPath);
                        }

                    }
                    if (log == "Session")
                    {
                        if (ddlGroupName.SelectedIndex > -1)
                        {
                            reportName = ddlGroupName.Text;
                            GenerateQVSessionLogsReport.ProcessReportGenerateForGroupName(startDate, endDate, reportName, reportOutPutType, fileNameWithFolderPath);
                        }
                        if (ddlReportName.SelectedIndex > -1)
                        {
                            reportName = ddlReportName.Text;
                            GenerateQVSessionLogsReport.ProcessReportGenerateForReportName(startDate, endDate, reportName, reportOutPutType, fileNameWithFolderPath);
                        }
                        if (ddlUserName.SelectedIndex > -1)
                        {
                            reportName = ddlUserName.Text;
                            GenerateQVSessionLogsReport.ProcessReportGenerateForUserName(startDate, endDate, reportName, reportOutPutType, fileNameWithFolderPath);
                        }

                    }
                }
            }
        }
    }
}
