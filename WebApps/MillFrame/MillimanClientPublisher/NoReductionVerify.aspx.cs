using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClientPublisher
{
    public partial class NoReductionVerify : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            int Index = 0;
            if (Request["Key"] != null)
                Index = System.Convert.ToInt32(Request["Key"]);

            ProjectSettingsExtension theProject = null;
            IList<ProjectSettingsExtension> Projects = Session["Projects"] as IList<ProjectSettingsExtension>;
            if ((Projects != null) && (Index < Projects.Count()))
            {
                theProject = Projects[Index];
            }
            if (theProject == null)
            {
                Response.Redirect("HTML/MissingProject.html");
                return;
            }
            if (System.Web.Security.Membership.GetUser() == null)
            {
                Response.Redirect("HTML/NotAuthorizedIssue.html");
                return;
            }

            ProjectName.Text = theProject.ProjectName;

            string WorkingDirectory = ClientPublisher.PublisherUtilities.GetWorkingDirectory(System.Web.Security.Membership.GetUser().UserName, theProject.ProjectName);

            List<string> IconExtensions = new List<string>() { ".jpg", ".jpeg", ".gif", ".png" };
            List<string> DocumentExtensions = new List<string>() { ".docx", ".xlsx", ".txt", ".pdf" };
            string[] Files = System.IO.Directory.GetFiles(WorkingDirectory, "*.*");
            if (Files != null)
            {
                foreach (string F in Files)
                {
                    //Updating QVW
                    if (string.Compare(System.IO.Path.GetExtension(F), ".qvw", true) == 0)
                    {
                        NewItems.Items.Add("New Qlikview QVW");
                    }

                    //Updating Icon
                    else if (IconExtensions.Contains(System.IO.Path.GetExtension(F).ToLower()))
                    {
                        NewItems.Items.Add("New Icon");
                    }
                    //Upating User Manual
                    else if (DocumentExtensions.Contains(System.IO.Path.GetExtension(F).ToLower()))
                    {
                        NewItems.Items.Add("New User Manual");
                    }
                    //Updating Tool Tip
                    else if (System.IO.Path.GetFileName(F).ToLower() == "tooltip.dat")
                    {
                        NewItems.Items.Add("Updated Tool Tip");
                    }
                    //Updating Description
                    else if (System.IO.Path.GetFileName(F).ToLower() == "description.dat")
                    {
                        NewItems.Items.Add("Updated Description");
                    }
                    //Updating Restricted Views
                    else if (System.IO.Path.GetFileName(F).ToLower() == "reductionrequired.dat")
                    {
                        bool Restricted = System.Convert.ToBoolean(System.IO.File.ReadAllText(F));
                        if (Restricted)
                            NewItems.Items.Add("Modified Viewing Restrictions(RESTRICTIONS ON)");
                        else
                            NewItems.Items.Add("Modified Viewing Restrictions(RESTRICTIONS OFF)");
                    }

                }
            }
        }


        protected void ApplyUpdatesNow_Click(object sender, EventArgs e)
        {
            Response.Redirect("NoReduction.aspx?key=" + Request["key"]);
        }
    }
}