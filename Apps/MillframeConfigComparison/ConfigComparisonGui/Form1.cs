using System;
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

        public Form1()
        {
            InitializeComponent();

            this.TextBoxPath1.DoubleClick += new System.EventHandler(this.PromptForPath);
            this.TextBoxPath2.DoubleClick += new System.EventHandler(this.PromptForPath);
            this.Resize += new EventHandler(this.FormResize);
        }

        private void PromptForPath(object sender, EventArgs e)
        {
            DialogResult Res = FolderBrowserDialog1.ShowDialog(this);
            if (Directory.Exists(FolderBrowserDialog1.SelectedPath))
            {
                ((TextBox)sender).Text = FolderBrowserDialog1.SelectedPath;
            }
        }

        private void FormResize(object sender, EventArgs e)
        {
            Size NewFormSize = ((Control)sender).Size;
            tabControl1.Size = new Size(NewFormSize.Width - tabControl1.Left - 28, 
                                        NewFormSize.Height - tabControl1.Top - 50);
        }

        private void ButtonCompare_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(TextBoxPath1.Text) && Directory.Exists(TextBoxPath2.Text))
            {
                ComparisonResult Result = ComparisonLib.Compare(TextBoxPath1.Text, 
                                                                TextBoxPath2.Text, 
                                                                CheckBoxDoWebConfig.Checked, 
                                                                CheckBoxDoAppConfig.Checked);

                dataGridView1.DataSource = Result.ComparisonResults;
                dataGridView1.DataMember = "KeysInBothPaths";

                dataGridView2.DataSource = Result.ComparisonResults;
                dataGridView2.DataMember = "KeysInPath1Only";

                dataGridView3.DataSource = Result.ComparisonResults;
                dataGridView3.DataMember = "KeysInPath2Only";

                dataGridView4.DataSource = Result.ComparisonResults;
                dataGridView4.DataMember = "ConnectionStrings";

                dataGridView1.Columns[0].FillWeight = 20;
                dataGridView1.Columns[1].FillWeight = 50;
                dataGridView1.Columns[2].FillWeight = 50;

                dataGridView4.Columns[0].FillWeight = 20;
                dataGridView4.Columns[1].FillWeight = 50;
                dataGridView4.Columns[2].FillWeight = 50;
            }
            else
            {
                MessageBox.Show("Path not found, aborting");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ComparisonLib = new ComparisonWorker();

            dataGridView1.RowPostPaint += ComparisonGrid_RowPostPaint;
            dataGridView4.RowPostPaint += ComparisonGrid_RowPostPaint;
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
    }
}
