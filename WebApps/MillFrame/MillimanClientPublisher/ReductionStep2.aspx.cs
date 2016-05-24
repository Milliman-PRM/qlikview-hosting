using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClientPublisher
{
    public partial class ReductionStep2 : System.Web.UI.Page
    {
        //private string GetVirtualPath()
        //{
        //    string VirtualPath = string.Empty;
 
        //    if (Request["ProjectPath"] != null)
        //    {
        //        //path coming from the explorer interface always has an extra root dir on the front get rid of it
        //        VirtualPath = Request["ProjectPath"];
        //        VirtualPath = VirtualPath.Replace('\\', '/');
        //        if ( VirtualPath.Contains('/') )
        //            VirtualPath = VirtualPath.Substring(VirtualPath.IndexOf('/')+1);

        //        //put on session to make report work, must be Absolute path
        //        string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
        //        Session["ProjectPath"] = System.IO.Path.Combine(DocumentRoot, VirtualPath );
        //    }
        //    else if (Session["ProjectPath"] != null)
        //    {
        //        string AbsPath = Session["ProjectPath"].ToString();
        //        string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
        //        string VPath = AbsPath.Substring(DocumentRoot.Length);
        //        VirtualPath = VPath.TrimStart(new char[] { '\\', '/' });
        //    }
        //    return VirtualPath;
        //}

        private bool ContainsFilesToPublish(out string WorkingDirectory, out string WorkingProject)
        {
            WorkingDirectory = string.Empty;
            WorkingProject = string.Empty;

            int Index = 0;
            if (Request["Key"] != null)
                Index = System.Convert.ToInt32(Request["Key"]);
            else
            {
                string NavigateTo = "HTML/GeneralIssue.aspx?msg=" + MillimanCommon.Utilities.ConvertStringToHex("Missing project index - cannot publish project.'");
                Response.Redirect(NavigateTo);
                return false;
            }

            ProjectSettingsExtension theProject = null;
            IList<ProjectSettingsExtension> Projects = Session["Projects"] as IList<ProjectSettingsExtension>;
            if ((Projects != null) && (Index < Projects.Count()))
            {
                theProject = Projects[Index];
            }
            if (theProject == null)
            {
                Response.Redirect("HTML/MissingProject.html");
                return false;
            }
            if (System.Web.Security.Membership.GetUser() == null)
            {
                Response.Redirect("HTML/NotAuthorizedIssue.html");
                return false;
            }

            

            WorkingDirectory =  PublisherUtilities.GetWorkingDirectory(System.Web.Security.Membership.GetUser().UserName, theProject.ProjectName);
            WorkingProject = System.IO.Path.Combine(WorkingDirectory, theProject.QVName + ".hciprj");
            string ReadyFile = System.IO.Path.Combine(WorkingDirectory, "ready_to_publish.txt");
            return System.IO.File.Exists(ReadyFile);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            EnableViewState = false;

            if (!IsPostBack)
            {

                string WorkingDirectory = string.Empty;
                string WorkingProject = string.Empty;
                if (ContainsFilesToPublish(out WorkingDirectory, out WorkingProject) == false)
                {
                    Response.Redirect("NoFilesToPublish.html");
                    return;
                }
                //look to see if there is a new QVW to publish, if not we need to hide some tabs
  
                MillimanCommon.ProjectSettings PSE = MillimanCommon.ProjectSettings.Load(WorkingProject);

                Session["ProjectPath"] = WorkingProject;

                string QVWQualfiedPath = System.IO.Path.Combine(WorkingDirectory, PSE.ProjectName + ".qvw");
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
            Response.Redirect("ReductionStep3.aspx?Key=" + Request["Key"]);
        }
    }
}