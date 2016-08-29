using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MillimanDirectoryCleaner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void LoadData()
        {
            DirectoryGrid.Rows.Clear();
            List<string> AllDirs = System.IO.Directory.GetDirectories(SourceDirectory.Text, "*ReducedUserQVWs*", System.IO.SearchOption.AllDirectories).ToList();
            foreach( string _Dir in AllDirs )
            {
                List<string> SubDirs = System.IO.Directory.GetDirectories(_Dir, "*", System.IO.SearchOption.TopDirectoryOnly).ToList();
                foreach (string _SubDir in SubDirs)
                {
                    string DirName = System.IO.Path.GetFileName(_SubDir);
                    if (string.Compare(DirName, "reductionreports", true) != 0)
                    {
                        string User = MillimanCommon.Utilities.ConvertHexToString(DirName);
                        bool ShouldDelete = false;
                        if (System.Web.Security.Membership.GetUser(User) == null)
                            ShouldDelete = true;

                        object[] RowEntry = new object[] { ShouldDelete, User, _SubDir };
                        DirectoryGrid.Rows.Add(RowEntry);
                    }
                }
            }
        }

        private void Process_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow DGVR in DirectoryGrid.Rows)
            {
                if (System.Convert.ToBoolean(DGVR.Cells["Delete"].Value))
                {
                    string Directory = DGVR.Cells["Directory"].Value.ToString();
                    System.IO.Directory.Delete(Directory, true);
                }
            }
            LoadData();
        }
        private void Browse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Browser = new FolderBrowserDialog();
            Browser.SelectedPath = System.Configuration.ConfigurationManager.AppSettings["DefaultStartPath"];
            SourceDirectory.Text = Browser.SelectedPath;
            DialogResult result = Browser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SourceDirectory.Text = Browser.SelectedPath;
                LoadData();
            }

        }
    }
}
