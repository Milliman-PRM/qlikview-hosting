using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MillimanQVLicenseCleaner
{
    public partial class Main : Form
    {
        private bool AutoRun { get; set; }
        private int  MaxAgeInDays { get; set;  }
        public Main()
        {
            InitializeComponent();
            string[] Args = Environment.GetCommandLineArgs();
            foreach (string Arg in Args)
            {
                if (Arg.ToLower().IndexOf("runsilent") != -1 )
                    AutoRun = true;
                
            }
            MaxAgeInDays = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxAgeInDays"]);
            string MaxToRemove = System.Configuration.ConfigurationManager.AppSettings["MaxToRemove"];

            Settings.Items.Add("MaxAgeInDays=" + MaxAgeInDays.ToString());
            Settings.Items.Add("MaxToRemove=" + MaxToRemove);
            this.Text = "QV License Cleaner";
            Help.Text = "MaxToRemove key is used to limit the number of users that automatically qualify for license removal. By manually selecting users,  more license can be cleared than specified by the limit.  Any user who's license is less that 48 hours old will have thier license quarantined and will not be able to access the document associated with the license until the quaratine period has expired.";
            if (AutoRun == false)
            {
                LoadDisplay();
            }
            else
            {
                CleanLicense(MaxAgeInDays);
                System.Environment.Exit(0);
            }
        }

        private void AddRow( bool IsQuarantined, string LastAccess, string CALType, string Account, string Document)
        {
            object[] NewRow = new object[] { IsQuarantined, LastAccess, CALType, Account, Document };
            LicenseGrid.Rows.Add(NewRow);
        }

        private void LoadDisplay()
        {
            int MaxToRemove = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxToRemove"]);
            int RemovalCount = 0;
            LicenseGrid.Rows.Clear();
            MillimanReportReduction.QVLicensing QVL = new MillimanReportReduction.QVLicensing();
            List<MillimanReportReduction.QVLicensing.LicenseInfo> AllLicense = QVL.GetAllCALSOfTypeInUse(MillimanReportReduction.QVLicensing.LicenseTypeEnum.ANY);
            foreach( MillimanReportReduction.QVLicensing.LicenseInfo LI in AllLicense )
            {
                TimeSpan TS = DateTime.Now - LI.LastAccessed;
                bool IsDelateCandidate = false;
                if ( ( TS.Days > MaxAgeInDays ) && (RemovalCount < MaxToRemove) )
                {
                    RemovalCount++;
                    IsDelateCandidate = true;
                }
                AddRow(IsDelateCandidate, LI.LastAccessed.ToShortDateString(), LI.License.ToString(), LI.User, LI.DocumentName);
            }
        }

        private void CleanLicense(int MaxAge )
        {
            List<string> RemovalList = new List<string>();
            int DocCalCount = 0;
            int NamedCalCount = 0;

            MillimanReportReduction.QVLicensing QVL = new MillimanReportReduction.QVLicensing();

            foreach ( DataGridViewRow DGVR in LicenseGrid.Rows)
            {
                if (System.Convert.ToBoolean(DGVR.Cells["Delete"].Value))
                {
                    string LicenseType = DGVR.Cells["LicenseType"].Value.ToString();
                    string User = DGVR.Cells["UserName"].Value.ToString();
                    string Document = DGVR.Cells["Document"].Value.ToString();
                    if (string.IsNullOrEmpty(User))
                    {
                        if (AutoRun == false)
                        {
                            MessageBox.Show("Request to delete " + LicenseType + "  CAL with no given account value");
                        }
                        continue;
                    }

                    if ( string.Compare(LicenseType, "NAMED", 0) == 0)
                    {
                        if (QVL.RemoveNamedUserCAL(User) == false)
                        {
                            if ( AutoRun == false)
                            MessageBox.Show("Failed to delete named CAL for user:" + User);
                        }
                        else
                        {
                            RemovalList.Add(User + " NAMED LICENSE removed" + Environment.NewLine);
                            NamedCalCount++;
                        }
                    }
                    else
                    {
                        if ( string.IsNullOrEmpty(Document))
                        {
                            MessageBox.Show("Requested to delete document cal for user:" + User + " but no document name was provided.");
                            continue;
                        }
                        if (QVL.RemoveUserDocCAL(User, Document) == false)
                            MessageBox.Show("Failed to delete doc cal for user:" + User + "  document:" + Document);
                        else
                        {
                            RemovalList.Add(User + " DOC LICENSE removed" + Environment.NewLine);
                            DocCalCount++;
                        }
                    }
                }
            }

            if ( AutoRun == false )
                LoadDisplay();

            string RemovalMsg = "Removed " + NamedCalCount.ToString() + " NAMED CALs" + Environment.NewLine + "Removed " + DocCalCount.ToString() + " DOC CALs" + Environment.NewLine + Environment.NewLine;
            foreach (string Msg in RemovalList)
                RemovalMsg += Msg;

            if (AutoRun == false)
                MessageBox.Show(RemovalMsg, "Status");
        }

        private void RemoveLicenseNow(object sender, EventArgs e)
        {
            CleanLicense(MaxAgeInDays);
        }
    }
}
