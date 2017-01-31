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
using MillframeConfigComparisonLib;

namespace ConfigComparisonGui
{
    public partial class Form1 : Form
    {
        ComparisonWorker ComparisonLib = null;

        public Form1()
        {
            InitializeComponent();

            this.TextBoxPath1.DoubleClick += new System.EventHandler(this.PromptForPath);
            this.TextBoxPath2.DoubleClick += new System.EventHandler(this.PromptForPath);
        }

        private void PromptForPath(object sender, EventArgs e)
        {
            DialogResult Res = FolderBrowserDialog1.ShowDialog(this);
            if (Directory.Exists(FolderBrowserDialog1.SelectedPath))
            {
                ((TextBox)sender).Text = FolderBrowserDialog1.SelectedPath;
            }
        }

        private void ButtonCompare_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(TextBoxPath1.Text)  && Directory.Exists(TextBoxPath2.Text))
            {
                ComparisonLib.Compare(TextBoxPath1.Text, TextBoxPath2.Text, CheckBoxDoWebConfig.Checked, CheckBoxDoAppConfig.Checked);
            }
            else
            {
                MessageBox.Show("Path not found, aborting");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ComparisonLib = new ComparisonWorker();
        }
    }
}
