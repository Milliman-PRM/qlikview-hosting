using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.Admin
{
    public partial class History : System.Web.UI.Page
    {
        public class HistoryItem
        {
            public string Project_Name { get; set; }
            public string Published_On { get; set; }
            public string Published_By { get; set; }

            public HistoryItem(string _Project_Name, string _Published_On, string _Published_By)
            {
                Project_Name = _Project_Name;
                Published_On = _Published_On;
                Published_By = _Published_By;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));
                MillimanCommon.ProjectSettings LocalPS = MillimanCommon.ProjectSettings.Load(System.IO.Path.Combine(DocumentRoot, Request["ProjectPath"]));
                if (LocalPS != null)
                {
                    string VirtualDir = LocalPS.VirtualDirectory;
                    if (VirtualDir.ToLower().IndexOf(".hciprj") > 0)
                        VirtualDir = VirtualDir.Substring(0, VirtualDir.IndexOf('\\'));
                    //string Pusher = MillimanCommon.Utilities.ConvertStringToHex(System.Web.Security.Membership.GetUser().UserName);
                    string HistoryFolder = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["publishinghistory"], VirtualDir);
                    if (System.IO.Directory.Exists(HistoryFolder) == false)
                    {
                        //MillimanCommon.Alert.Show(LocalPS.ProjectName + " has not been published before.");
                    }
                    List<HistoryItem> Items = new List<HistoryItem>();

                    if (System.IO.Directory.Exists(HistoryFolder))
                    {
                        string FileID = MillimanCommon.Utilities.CalculateMD5Hash(LocalPS.ProjectName);
                        var Files = System.IO.Directory.GetFiles(HistoryFolder, "*.html").OrderByDescending(d => new System.IO.FileInfo(d).CreationTime);

                        foreach (string S in Files)
                        {
                            string[] FileItems = System.IO.Path.GetFileNameWithoutExtension(S).Split(new char[] { '_' });
                            if (string.Compare(FileID, FileItems[0], true) == 0)
                            {
                                //load into grid
                                string Pusher = MillimanCommon.Utilities.ConvertHexToString(FileItems[1]);
                                string LastSaved = System.IO.File.GetLastWriteTime(S).ToString();
                                string Project = LocalPS.ProjectName;

                                string ProjectLink = "<a href='_URL_' target='_blank'>_NAME_</a> ";
                                ProjectLink = ProjectLink.Replace("_NAME_", LocalPS.ProjectName);
                                string Reference = "DocumentReflector.aspx?key=" + MillimanCommon.Utilities.ConvertStringToHex(S);
                                ProjectLink = ProjectLink.Replace("_URL_", Reference);
                                Items.Add(new HistoryItem(ProjectLink, LastSaved, Pusher));
                            }
                        }
                    }
                    RadGrid1.DataSource = Items;
                    RadGrid1.DataBind();
                }
            }

        }

        protected void RadGrid1_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridHeaderItem)
            {
                Telerik.Web.UI.GridHeaderItem GHI = (Telerik.Web.UI.GridHeaderItem)e.Item;
                GHI.Cells[2].Text = "Project";
                GHI.Cells[3].Text = "Published to Server On";
                GHI.Cells[4].Text = "Published By";
            }

        }
    }
}