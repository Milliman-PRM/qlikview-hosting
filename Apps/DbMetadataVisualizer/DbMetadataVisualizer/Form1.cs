using System;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Principal;
using System.Web.Security;
using System.Configuration;
using MillimanCommon;

namespace DbMetadataVisualizer
{
    public partial class Form1 : Form
    {
        UserRepo Repo;
        string DocRoot = Path.GetFullPath(ConfigurationManager.AppSettings["QVDocumentRoot"]);

        public Form1()
        {
            InitializeComponent();

            try
            {
                groupBox1.Text = "Configuration   -   From file: " + AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

                Repo = UserRepo.GetInstance();

                LabelDocumentRoot.Text = ConfigurationManager.AppSettings["QVDocumentRoot"].ToString();
                LabelDbConnectionString.Text = ConfigurationManager.ConnectionStrings["PortalDB_ConnectionString"].ToString();
                LabelRulesFile.Text = Path.GetFullPath(ConfigurationManager.AppSettings["RepoFilePath"].ToString());

                ListBoxUserIds.Items.Clear();
                ListBoxAllGroupsWithUsers.Items.Clear();
                ListBoxAllGroupsWithNoUsers.Items.Clear();
                ListBoxDocumentsNotInAnyGroup.Items.Clear();

                foreach (MembershipUser x in Membership.GetAllUsers())
                {
                    ListBoxUserIds.Items.Add(x.UserName);
                }

                // Start with all docs in docroot and below, remove those that are believed OK
                List<string> QuestionableQvws = GetAllFilesWithExtension(DocRoot, "qvw", true);

                // Iterate over the roles defined in the Membership Database
                foreach (string Role in Roles.GetAllRoles())
                {
                    // Decide which ListBox the group should be displayed in
                    if (Roles.GetUsersInRole(Role).Count() > 0)
                    {
                        ListBoxAllGroupsWithUsers.Items.Add(Role);
                    }
                    else
                    {
                        ListBoxAllGroupsWithNoUsers.Items.Add(Role);
                    }

                    // Iterate over all groups in the UserRepo (rules.xml) associated with the current Membership role (usually 1)
                    foreach (KeyValuePair<string, List<UserRepo.QVEntity>> GroupDocs in Repo.RoleMap.Where(p => p.Key == Role))
                    {
                        // Iterate over each Qlikview entity associated with the current group (usually but not always 1)
                        foreach (UserRepo.QVEntity QvEntity in GroupDocs.Value.Where(e => !e.IsDirectory))
                        {
                            string GroupNameConvertedToPathFragment;

                            QuestionableQvws.Remove(Path.Combine(DocRoot, QvEntity.QualifiedPathName));

                            // Remove every QVW with directory path reflecting the group name. 
                            GroupNameConvertedToPathFragment = GroupDocs.Key.Replace("_", @"\") + @"\";
                            // FOR DEBUG var x = QuestionableQvws.Where(p => p.ToLower().Contains(GroupNameConvertedToPathFragment.ToLower()));
                            QuestionableQvws.RemoveAll(p => p.ToLower().Contains(GroupNameConvertedToPathFragment.ToLower()));
                        }
                    }
                }

                long TotalBytes = 0;
                foreach (string FileName in QuestionableQvws)
                {
                    ListBoxDocumentsNotInAnyGroup.Items.Add(FileName.Replace(DocRoot + @"\", ""));
                    TotalBytes += new FileInfo(FileName).Length;
                }

                LabelOrphanDocumentsReport.Text = "Ungrouped QVW count is " + QuestionableQvws.Count + " with combined file sizes: " + TotalBytes.ToString("N0");
            }
            // Any exception probably means configuration needs to be modified.  Display the empty form and let user edit the config. 
            catch
            {
                if (!Directory.Exists(ConfigurationManager.AppSettings["QVDocumentRoot"].ToString()))
                {
                    LabelDocumentRoot.Text += " *NOT FOUND*";
                }
                if (!File.Exists(ConfigurationManager.AppSettings["RepoFilePath"].ToString()))
                {
                    LabelRulesFile.Text += " *NOT FOUND*";
                }
                try
                {
                    using (SqlConnection Cxn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalDB_ConnectionString"].ToString() + @";Connection Timeout = 5"))
                    {
                        Cxn.Open();
                        Cxn.Close();
                    }
                }
                catch
                {
                    LabelDbConnectionString.Text += " *CONNECT FAILED*";
                }
            }
        }

        private List<string> GetAllFilesWithExtension(string RootPath, string Extension, bool Recurse = true)
        {
            List<String> AllDocs = new List<string>();
            AllDocs.AddRange(Directory.EnumerateFiles(RootPath, "*." + Extension));

            if (Recurse)
            {
                foreach (string Subfolder in Directory.GetDirectories(RootPath))
                {
                    AllDocs.AddRange(GetAllFilesWithExtension(Subfolder, Extension, Recurse));
                }
            }

            return AllDocs;
        }

        private void ListBoxUserIds_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBoxGroupsOfSelectedUser.Items.Clear();
            ListBoxUsersInSelectedGroup.Items.Clear();
            ListBoxDocuments.Items.Clear();
            foreach (string Role in Roles.GetRolesForUser(ListBoxUserIds.SelectedItem.ToString()))
            {
                ListBoxGroupsOfSelectedUser.Items.Add(Role);
            }
        }

        private void ListBoxGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ListBox)sender).SelectedItem == null) return;

            // manage other controls
            ListBoxDocuments.Items.Clear();
            ListBoxUsersInSelectedGroup.Items.Clear();
            // This handler handles selection for multiple ListBoxes.  Clear selections in the controls that did not send this event
            if (sender != ListBoxAllGroupsWithNoUsers) ListBoxAllGroupsWithNoUsers.ClearSelected();
            if (sender != ListBoxAllGroupsWithUsers) ListBoxAllGroupsWithUsers.ClearSelected();
            if (sender != ListBoxGroupsOfSelectedUser) ListBoxGroupsOfSelectedUser.ClearSelected();

            if (((ListBox)sender).SelectedItem.ToString() != "Administrator")
            {
                if (Repo.RoleMap.Keys.Contains(((ListBox)sender).SelectedItem.ToString()))
                {
                    foreach (var x in Repo.RoleMap[((ListBox)sender).SelectedItem.ToString()])
                    {
                        ListBoxDocuments.Items.Add(x.QualifiedPathName);
                    }
                }
            }

            string[] UsersInRole = Roles.GetUsersInRole(((ListBox)sender).SelectedItem.ToString());
            foreach (string User in UsersInRole)
            {
                ListBoxUsersInSelectedGroup.Items.Add(User);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string TextToCopy = string.Empty;
            foreach (string Item in ListBoxDocumentsNotInAnyGroup.Items)
            {
                TextToCopy += Item + "\r\n";
            }

            Clipboard.SetText(TextToCopy);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            string TextToCopy = string.Empty;
            foreach (string Item in ListBoxDocumentsNotInAnyGroup.Items)
            {
                TextToCopy += Path.Combine(DocRoot, Item) + "\r\n";
            }

            Clipboard.SetText(TextToCopy);
        }

        private void ButtonGotoConfig_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Notepad", AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            Application.Exit();
        }
    }
}
