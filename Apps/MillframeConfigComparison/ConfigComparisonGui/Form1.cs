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

                foreach (DataGridViewRow Row in dataGridView4.Rows)
                {
                    Row.DefaultCellStyle.BackColor = Color.LightPink;//to color the row
                    if (Row.Cells["Path 1 Value"].Value.ToString() != Row.Cells["Path 2 Value"].Value.ToString())
                    {
                        Row.DefaultCellStyle.BackColor = Color.LightPink;//to color the row
                    }
                    else
                    {
                        Row.DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }

                foreach (DataGridViewRow Row in dataGridView1.Rows)
                {
                    if (Row.Cells["Path 1 Value"].Value.ToString() != Row.Cells["Path 2 Value"].Value.ToString())
                    {
                        Row.DefaultCellStyle.BackColor = Color.LightPink;//to color the row
                    }
                    else
                    {
                        Row.DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }

                dataGridView1.Columns[0].Width = 60;
                dataGridView1.Columns[1].FillWeight = 40;
                dataGridView1.Columns[2].FillWeight = 40;
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
