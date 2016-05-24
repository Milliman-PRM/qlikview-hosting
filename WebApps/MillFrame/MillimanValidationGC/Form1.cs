using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRMValidationGC
{
    public partial class Form1 : Form
    {
        private List<QVDocumentCleaner> ProcessorList { get; set; }
        public Form1()
        {
            InitializeComponent();

            //string Root = @"C:\inetpub\wwwroot\InstalledApplications\MillimanSiteV2Live\QVDocuments";
            //string[] QVDirs = System.IO.Directory.GetDirectories(Root, "*", System.IO.SearchOption.AllDirectories);
            //foreach (string QVDir in QVDirs)
            //{
            //    QVDocumentCleaner QVDC = new QVDocumentCleaner(QVDir);
            //    QVDC.Process(true);
            //}

            DirectoryTextbox.Text = System.Configuration.ConfigurationManager.AppSettings["DefaultStartPath"];
            ProcessorList = new List<QVDocumentCleaner>();
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Browser = new FolderBrowserDialog();
            Browser.SelectedPath = System.Configuration.ConfigurationManager.AppSettings["DefaultStartPath"];
            DirectoryTextbox.Text = Browser.SelectedPath;
            DialogResult result = Browser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryTextbox.Text = Browser.SelectedPath;
            }

            //string T = @"D:\PRMSystem\QVDocuments\";
            //string[] Files = System.IO.Directory.GetFiles(Browser.SelectedPath, "*.hciprj", System.IO.SearchOption.AllDirectories);
            //foreach (string S in Files)
            //    System.Diagnostics.Debug.WriteLine(S.Substring(T.Length).Replace(".hciprj",".qvw"));
            //System.Diagnostics.Debug.WriteLine("done");
        }

        private void Process_Click(object sender, EventArgs e)
        {
            ProcessorList.Clear();
            Status.Nodes.Clear();

            System.IO.SearchOption Option = Subdirs.Checked ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly;
            List<String> QVDirs = System.IO.Directory.GetDirectories(DirectoryTextbox.Text, "*", Option).ToList();
            QVDirs.Add(DirectoryTextbox.Text);
            bool ProcessedData = false;
            foreach (string QVDir in QVDirs)
            {
                QVDocumentCleaner QVDC = new QVDocumentCleaner(QVDir);
                if (QVDC.IsProjectDirectory() == false)
                    continue;  //not a project dir, just skip over it

                ProcessedData = true;
                ProcessorList.Add(QVDC);
                QVDC.Process();

                TreeNode QVDirTN = new TreeNode(QVDir);
                TreeNode Required = new TreeNode("Required Files");
                foreach (string Req in QVDC.RequiredFiles)
                    Required.Nodes.Add(new TreeNode(Req));
                TreeNode NotRequired = new TreeNode("Garbage");
                foreach (string NotReq in QVDC.NonRequiredFiles)
                    NotRequired.Nodes.Add(new TreeNode(NotReq));

                QVDirTN.Nodes.Add(Required);
                QVDirTN.Nodes.Add(NotRequired);

                Status.Nodes.Add(QVDirTN);
                Status.ExpandAll();
            }

            if (ProcessedData == false)
                MessageBox.Show("Selctions did not contain and directories with projects to process.");
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            Status.Nodes.Clear();
            foreach (QVDocumentCleaner QVDC in ProcessorList)
            {
                TreeNode DirNode = new TreeNode(QVDC.DocumentFolder);
                if (QVDC.Cleanup())
                    DirNode.Nodes.Add(new TreeNode("Success - garbage archived"));
                else
                    DirNode.Nodes.Add(new TreeNode("Failed"));

                Status.Nodes.Add(DirNode);
            }
            Status.ExpandAll();
        }
    }
}
