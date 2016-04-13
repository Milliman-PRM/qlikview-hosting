using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.ComplexReporting
{
    public partial class ReportingShell : System.Web.UI.Page
    {
        private string GetVirtualPath()
        {
            string VirtualPath = string.Empty;
 
            if (Request["ProjectPath"] != null)
            {
                //path coming from the explorer interface always has an extra root dir on the front get rid of it
                VirtualPath = Request["ProjectPath"];
                VirtualPath = VirtualPath.Replace('\\', '/');
                if ( VirtualPath.Contains('/') )
                    VirtualPath = VirtualPath.Substring(VirtualPath.IndexOf('/')+1);

                //put on session to make report work, must be Absolute path
                string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                Session["ProjectPath"] = System.IO.Path.Combine(DocumentRoot, VirtualPath );
            }
            else if (Session["ProjectPath"] != null)
            {
                string AbsPath = Session["ProjectPath"].ToString();
                string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                string VPath = AbsPath.Substring(DocumentRoot.Length);
                VirtualPath = VPath.TrimStart(new char[] { '\\', '/' });
            }
            return VirtualPath;
        }

        private bool ContainsFilesToPublish()
        {
            string ProjectPath = Session["ProjectPath"].ToString();
            if (ProjectPath.ToLower().EndsWith(".hciprj"))
                ProjectPath = System.IO.Path.GetDirectoryName(ProjectPath);

            string[] AllFiles = System.IO.Directory.GetFiles(ProjectPath, "*", System.IO.SearchOption.AllDirectories);
            foreach (string File in AllFiles)
            {
                if (File.ToLower().EndsWith("_new"))
                    return true;
            }
            return false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            EnableViewState = false;

            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(GetVirtualPath()) == true)
                    Publish.Enabled = false;

                if (ContainsFilesToPublish() == false)
                {
                    Response.Redirect("NoFilesToPublish.html");
                }
                //look to see if there is a new QVW to publish, if not we need to hide some tabs
                string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));
                MillimanCommon.ProjectSettings LocalPS = MillimanCommon.ProjectSettings.Load(Session["ProjectPath"].ToString());
                string QVWQualfiedPath = System.IO.Path.Combine(LocalPS.AbsoluteProjectPath, LocalPS.QVName + ".qvw_new");
                bool UploadNewQVW = System.IO.File.Exists(QVWQualfiedPath);
                Menu.Tabs[1].Visible = UploadNewQVW;
                Menu.Tabs[2].Visible = UploadNewQVW;
                Menu.Tabs[3].Visible = UploadNewQVW;
                Menu.Tabs[4].Visible = UploadNewQVW;
            }
        }
        //this shell is the cross roads between a session based system and a url based session.
        //thus we check both to locate the directory, since it could be invoked from either
        protected void Publish_Click(object sender, EventArgs e)
        {
            Response.Redirect("..\\admin\\PushToProduction.aspx?ProjectPath=" + GetVirtualPath());
        }
    }
}