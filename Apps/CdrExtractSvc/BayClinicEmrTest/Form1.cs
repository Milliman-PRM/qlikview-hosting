using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CdrExtractTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.txtFolder.DoubleClick += new System.EventHandler(this.txtFolder_DoubleClick);
        }

        private void btnBayClinic_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Processing Bay Clinic data in folder: " + txtFolder.Text);
            BayClinicEmrLib.RawDataParser Parser = new BayClinicEmrLib.RawDataParser(@"H:\.prm_config\.mongodb", @"BayClinicMongoCredentials");
            if (! Directory.Exists(txtFolder.Text))
            {
                Console.Beep();
                return;
            }
            Parser.MigrateRawToMongo(txtFolder.Text, chkMongoInsert.Checked);
        }

        private void btnNorthBend_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Processing North Bend data in folder: " + txtFolder.Text);
            NorthBendUnityLib.RawDataParser Parser = new NorthBendUnityLib.RawDataParser(@"H:\.prm_config\.mongodb", @"NorthBendMongoCredentials");
            if (!Directory.Exists(txtFolder.Text))
            {
                Console.Beep();
                return;
            }
            Parser.MigrateRawToMongo(txtFolder.Text, chkMongoInsert.Checked);
        }

        private void txtFolder_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtFolder_DoubleClick(object sender, System.EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = Directory.Exists(folderBrowserDialog1.SelectedPath) ?
                folderBrowserDialog1.SelectedPath :
                folderBrowserDialog1.RootFolder.ToString();

            DialogResult R = folderBrowserDialog1.ShowDialog();
            if (R == DialogResult.OK)
            {
                txtFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

    }
}
