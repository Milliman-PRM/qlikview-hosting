using System;
using System.IO;
using System.Configuration;
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
        BayClinicCernerExtractLib.RawDataParser Parser = null;

        public Form1()
        {
            InitializeComponent();

            this.FormClosing += Form1_FormClosing;

            txtFolder.Focus();

            this.txtFolder.DoubleClick += new System.EventHandler(this.txtFolder_DoubleClick);

            var settings = ConfigurationManager.AppSettings;
            var x = settings["RedoxRawFilePath"];
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Parser != null)
            {
                Parser.EndProcessing = true;
            }
        }

        private void btnBayClinic_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Processing Bay Clinic data in folder: " + txtFolder.Text);
            Parser = new BayClinicCernerExtractLib.RawDataParser(@"H:\.prm_config\.mongodb", @"BayClinicTestMongoCredentials");
            if (!Directory.Exists(txtFolder.Text))
            {
                Console.Beep();
                MessageBox.Show("Input folder not found");
                return;
            }

            try
            {
                Parser.MigrateFolderToMongo(txtFolder.Text, chkMongoInsert.Checked, checkArchive.Checked ? Path.Combine(txtFolder.Text, "Archive") : null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnNorthBend_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Processing North Bend data in folder: " + txtFolder.Text);
            NorthBendUnityExtractLib.RawDataParser Parser = new NorthBendUnityExtractLib.RawDataParser(@"H:\.prm_config\.mongodb", @"NorthBendMongoCredentials");
            if (!Directory.Exists(txtFolder.Text))
            {
                Console.Beep();
                MessageBox.Show("Input folder not found");
                return;
            }

            try
            {
                Parser.MigrateRawToMongo(txtFolder.Text, chkMongoInsert.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private void btnRedox_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Processing Redox data in folder: " + txtFolder.Text);
            RedoxExtractLib.RawDataParser Parser = new RedoxExtractLib.RawDataParser(@"H:\.prm_config\.mongodb", @"RedoxMongoCredentials");
            if (!Directory.Exists(txtFolder.Text))
            {
                Console.Beep();
                MessageBox.Show("Input folder not found");
                return;
            }

            try
            {
                String RedoxArchiveFilePath = Path.Combine(txtFolder.Text, "Archive");

                Parser.MigrateRawToMongo(txtFolder.Text, RedoxArchiveFilePath, chkMongoInsert.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
