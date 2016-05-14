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
                if (string.Compare(Arg, "AutoRun", true) == 0)
                    AutoRun = true;
                
            }
            MaxAgeInDays = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxAgeInDays"]);

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
            LicenseGrid.Rows.Clear();
            MillimanReportReduction.QVLicensing QVL = new MillimanReportReduction.QVLicensing();
            List<MillimanReportReduction.QVLicensing.LicenseInfo> AllLicense = QVL.GetAllCALSOfTypeInUse(MillimanReportReduction.QVLicensing.LicenseTypeEnum.ANY);
            foreach( MillimanReportReduction.QVLicensing.LicenseInfo LI in AllLicense )
            {
                TimeSpan TS = DateTime.Now - LI.LastAccessed;
                bool IsDelateCandidate = TS.Days > MaxAgeInDays ? true : false;
                AddRow(IsDelateCandidate, LI.LastAccessed.ToShortDateString(), LI.License.ToString(), LI.User, LI.DocumentName);
            }
        }

        private void CleanLicense(int MaxAge )
        {
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
                        MessageBox.Show("Request to delete " + LicenseType + "  CAL with no given account value");
                        continue;
                    }

                    if ( string.Compare(LicenseType, "NAMED", 0) == 0)
                    {
                        if (QVL.RemoveNamedUserCAL(User) == false)
                            MessageBox.Show("Failed to delete named CAL for user:" + User);
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
                    }
                }
            }
            LoadDisplay();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            CleanLicense(MaxAgeInDays);
        }
    }
}
