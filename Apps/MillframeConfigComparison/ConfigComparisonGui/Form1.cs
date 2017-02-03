using System;
using System.Configuration;
using System.Diagnostics;
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
        Dictionary<string, List<string>> ProductsAndVersions = new Dictionary<string, List<string>>();

        public Form1()
        {
            InitializeComponent();

            this.TextBoxPath1.DoubleClick += new System.EventHandler(this.PromptForPath);
            this.TextBoxPath2.DoubleClick += new System.EventHandler(this.PromptForPath);
            this.Resize += new EventHandler(this.FormResize);

            OpenFileDialog1.Filter = "Configuration Files (*.config)|*.config|All Files (*.*)|*.*";
            OpenFileDialog1.FilterIndex = 1;
            OpenFileDialog1.RestoreDirectory = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ComparisonLib = new ComparisonWorker();

            dataGridView1.RowPostPaint += ComparisonGrid_RowPostPaint;
            dataGridView4.RowPostPaint += ComparisonGrid_RowPostPaint;
            dataGridView5.RowPostPaint += RequiredKeyGrid_RowPostPaint;
            dataGridView6.RowPostPaint += RequiredKeyGrid_RowPostPaint;

            SetVersionControlsVisible(false);
            LoadRequiredKeys();
        }

        private void PromptForPath(object sender, EventArgs e)
        {
            DialogResult Res = OpenFileDialog1.ShowDialog(this);
            if (File.Exists(OpenFileDialog1.FileName))
            {
                ((TextBox)sender).Text = OpenFileDialog1.FileName;
            }
        }

        private void FormResize(object sender, EventArgs e)
        {
            Size NewFormSize = ((Control)sender).Size;
            tabControl1.Size = new Size(NewFormSize.Width - tabControl1.Left - 28, 
                                        NewFormSize.Height - tabControl1.Top - 50);

            Size NewRequiredKeyGridSize = new Size(tabControl1.TabPages["TabPageRequiredKeys"].ClientSize.Width / 2 - 6, tabControl1.TabPages["TabPageRequiredKeys"].ClientSize.Height - 6);
            // dataGridView5 stays in the same upper left location
            dataGridView5.Size = NewRequiredKeyGridSize;
            dataGridView6.Location = new Point(dataGridView6.Parent.ClientSize.Width / 2 + 3, dataGridView6.Top);
            dataGridView6.Size = NewRequiredKeyGridSize;
        }

        private void ButtonCompare_Click(object sender, EventArgs e)
        {
            if (File.Exists(TextBoxPath1.Text) && File.Exists(TextBoxPath2.Text))
            {
                List<string> RequiredList1 = null, RequiredList2 = null;

                #region Find the appropriate lists of required config keys
                Configuration ThisAppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConfigurationSectionGroup RequiredConfigurationKeysGroup = ThisAppConfig.GetSectionGroup("RequiredConfigurationKeys");

                if (ComboBoxCfg1Version.Text != "" && ComboBoxCfg1Version.Visible)
                {
                    RequiredList1 = new List<string>();
                    NameValueCfgElementSection ConfigSection = (NameValueCfgElementSection)RequiredConfigurationKeysGroup.Sections[NameValueCfgElementSection.SectionName(ComboBoxProducts.Text, ComboBoxCfg1Version.Text)];
                    foreach (NameValueCfgElement NameValueElement in ConfigSection.AllNameValueElements)
                    {
                        RequiredList1.Add(NameValueElement.ElementInformation.Properties["KeyName"].Value.ToString());
                    }
                }
                if (ComboBoxCfg2Version.Text != "" && ComboBoxCfg2Version.Visible)
                {
                    RequiredList2 = new List<string>();
                    NameValueCfgElementSection ConfigSection = (NameValueCfgElementSection)RequiredConfigurationKeysGroup.Sections[NameValueCfgElementSection.SectionName(ComboBoxProducts.Text, ComboBoxCfg2Version.Text)];
                    foreach (NameValueCfgElement NameValueElement in ConfigSection.AllNameValueElements)
                    {
                        RequiredList2.Add(NameValueElement.ElementInformation.Properties["KeyName"].Value.ToString());
                    }
                }
                #endregion

                // Populate the comparison results
                ComparisonResult Result = ComparisonLib.Compare(TextBoxPath1.Text, TextBoxPath2.Text, RequiredList1, RequiredList2);

                dataGridView1.DataSource = Result.ComparisonResults;
                dataGridView1.DataMember = "KeysInBothPaths";

                dataGridView2.DataSource = Result.ComparisonResults;
                dataGridView2.DataMember = "KeysInPath1Only";

                dataGridView3.DataSource = Result.ComparisonResults;
                dataGridView3.DataMember = "KeysInPath2Only";

                dataGridView4.DataSource = Result.ComparisonResults;
                dataGridView4.DataMember = "ConnectionStrings";

                if (Result.ComparisonResults.Tables.Contains("TableOfPath1RequiredKeys"))
                {
                    dataGridView5.DataSource = Result.ComparisonResults;
                    dataGridView5.DataMember = "TableOfPath1RequiredKeys";
                }

                if (Result.ComparisonResults.Tables.Contains("TableOfPath2RequiredKeys"))
                {
                    dataGridView6.DataSource = Result.ComparisonResults;
                    dataGridView6.DataMember = "TableOfPath2RequiredKeys";
                }

                dataGridView1.Columns[0].FillWeight = 20;
                dataGridView1.Columns[1].FillWeight = 50;
                dataGridView1.Columns[2].FillWeight = 50;

                dataGridView2.Columns[0].FillWeight = 20;
                dataGridView2.Columns[1].FillWeight = 50;

                dataGridView3.Columns[0].FillWeight = 20;
                dataGridView3.Columns[1].FillWeight = 50;

                dataGridView4.Columns[0].FillWeight = 20;
                dataGridView4.Columns[1].FillWeight = 50;
                dataGridView4.Columns[2].FillWeight = 50;

                if (Result.ComparisonResults.Tables.Contains("TableOfPath1RequiredKeys"))
                {
                    dataGridView5.Columns[0].FillWeight = 20;
                    dataGridView5.Columns[1].FillWeight = 50;
                }

                if (Result.ComparisonResults.Tables.Contains("TableOfPath2RequiredKeys"))
                {
                    dataGridView6.Columns[0].FillWeight = 20;
                    dataGridView6.Columns[1].FillWeight = 50;
                }
            }
            else
            {
                MessageBox.Show("Path not found, aborting");
            }
        }

        private void ComparisonGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridViewRow Row = ((DataGridView)sender).Rows[e.RowIndex];

            // Any row with non matching values in Path1 and Path 2 should be colored pink
            if (Row.Cells["Path 1 Value"].Value.ToString() != Row.Cells["Path 2 Value"].Value.ToString())
            {
                Row.DefaultCellStyle.BackColor = Color.LightPink;//to color the row

                // Any cell with an empty value from Path 1 or Path 2 should be colored red
                foreach (string CellName in new string[]{ "Path 1 Value", "Path 2 Value" })
                {
                    if (Row.Cells[CellName].Value.ToString() == "")
                    {
                        Row.Cells[CellName].Value = "<null>";
                        Row.Cells[CellName].Style.BackColor = Color.Red;
                    }
                }
            }
            else
            {
                Row.DefaultCellStyle.BackColor = Color.LightGreen;
            }
        }

        private void RequiredKeyGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridViewRow Row = ((DataGridView)sender).Rows[e.RowIndex];

            // Any row with non matching values in Path1 and Path 2 should be colored pink
            if (Row.Cells["Value"].Value.ToString() == ComparisonResult.KEY_NOT_FOUND)
            {
                Row.DefaultCellStyle.BackColor = Color.Red;
            }
            else
            {
                Row.DefaultCellStyle.BackColor = Color.LightGreen;
            }
        }

        private bool LoadRequiredKeys()
        {
            // Required keys are configured in this application's app.config
            Configuration ThisAppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationSectionGroup RequiredConfigurationKeysGroup = ThisAppConfig.GetSectionGroup("RequiredConfigurationKeys");
            if (RequiredConfigurationKeysGroup == null)
            {
                MessageBox.Show("Configuration section group \"RequiredConfigurationKeys\" not found.  Not loading products and versions.");
                return false;
            }

            foreach (ConfigurationSection Section in RequiredConfigurationKeysGroup.Sections)
            {
                var Product = Section.ElementInformation.Properties["Product"].Value.ToString();
                var Version = Section.ElementInformation.Properties["Version"].Value.ToString();

                if (ProductsAndVersions.ContainsKey(Product))
                {
                    ProductsAndVersions[Product].Add(Version);
                }
                else
                {
                    ProductsAndVersions.Add(Product, new List<string> { Version });
                    ComboBoxProducts.Items.Add(Product);
                }
            }

            return true;
        }

        private void SeedConfigContent(object sender, EventArgs e)
        {
            Configuration ThisAppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ThisAppConfig.SectionGroups.Add("RequiredConfigurationKeys", new ConfigurationSectionGroup());
            ConfigurationSectionGroup RequiredConfigurationKeysGroup = ThisAppConfig.GetSectionGroup("RequiredConfigurationKeys");

            NameValueCfgElementSection Version40Section = new NameValueCfgElementSection("4.0", "Client Publisher");
            RequiredConfigurationKeysGroup.Sections.Add(Version40Section.SectionNameInFile, Version40Section);
            Version40Section.AllNameValueElements.Add(new NameValueCfgElement("ServerName40"));
            Version40Section.AllNameValueElements.Add(new NameValueCfgElement("NamedCALCost"));
            ThisAppConfig.Save(ConfigurationSaveMode.Modified);

            NameValueCfgElementSection Version41Section = new NameValueCfgElementSection("4.1", "Client Publisher");
            RequiredConfigurationKeysGroup.Sections.Add(Version41Section.SectionNameInFile, Version41Section);
            Version41Section.AllNameValueElements.Add(new NameValueCfgElement("ServerName41"));
            Version41Section.AllNameValueElements.Add(new NameValueCfgElement("NamedCALCost"));
            ThisAppConfig.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("RequiredConfigurationKeys");

            LoadRequiredKeys();
        }

        private void ComboBoxProducts_TextChanged(object sender, EventArgs e)
        {
            string SelectedProduct = ((ComboBox)sender).Text;

            if (ProductsAndVersions.Keys.Contains(SelectedProduct))
            {
                ComboBoxCfg1Version.Items.Clear();
                ComboBoxCfg2Version.Items.Clear();

                foreach (string Version in ProductsAndVersions[SelectedProduct])
                {
                    ComboBoxCfg1Version.Items.Add(Version);
                    ComboBoxCfg2Version.Items.Add(Version);
                }

                SetVersionControlsVisible(true);
            }
            else
            {
                SetVersionControlsVisible(false);
            }

        }

        private List<string> GetRequiredKeys(string Product, string Version)
        {
            List<string> Result = new List<string>();

            return Result;
        }

        private void SetVersionControlsVisible(bool Visible=true)
        {
            label3.Visible = Visible;
            ComboBoxCfg1Version.Visible = Visible;
            label4.Visible = Visible;
            ComboBoxCfg2Version.Visible = Visible;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SeedConfigContent(sender, e);
        }
    }
}
