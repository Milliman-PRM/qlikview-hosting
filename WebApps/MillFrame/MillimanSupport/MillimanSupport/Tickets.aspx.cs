using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace MillimanSupport
{
    public partial class Tickets : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReBind();
                NotificationAccount.DataSource = UserList();
                NotificationAccount.DataBind();
            }
        }

        protected void Issues_SortCommand(object sender, GridSortCommandEventArgs e)
        {
            ReBind();
        }

        protected void Issues_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            ReBind();
        }

        protected void Issues_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
        {
            ReBind();
        }

        private void ReBind()
        {
            MillimanSupport.Issues.IssuesRepo IR = new Issues.IssuesRepo();
            IR.LoadIssueRepo();
            if (IR != null)
            {
                Issues.DataSource = IR.IssueList;
                Issues.DataBind();
            }
        }

        private List<string> UserList()
        {

           // return new List<string>() { "van.nanney@milliman.com", "john.bosko@milliman.com", "fred.somebody@somewhere.com" };

            System.Configuration.Configuration configuration =
               System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");

            // Get the authentication section.
            System.Web.Configuration.AuthenticationSection authenticationSection =
                (System.Web.Configuration.AuthenticationSection)configuration.GetSection(
                "system.web/authentication");


            System.Web.Configuration.FormsAuthenticationCredentials currentCredentials = authenticationSection.Forms.Credentials;// formsAuthentication.Credentials;
            List<string> Users = new List<string>();
            for (System.Int32 i = 0; i < currentCredentials.Users.Count; i++)
            {
                if (Users.Contains(currentCredentials.Users[i].Name) == false)
                    Users.Add(currentCredentials.Users[i].Name);
            }
            return Users;
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            MillimanSupport.Issues.IssuesRepo IR = new Issues.IssuesRepo();
            IR.LoadIssueRepo();

            string Me = Context.User.Identity.Name;
            MillimanSupport.Issues.Issue NewIssue = new Issues.Issue(IR.NextTicketID(), Me, NotificationAccount.Text, CovisintTicketID.Text, CUID.Text, Description.Text, "");
            NewIssue.SendEmail("hcintel.support@milliman.com", Global.FromType.TICKETING_SUPPORT);
            IR.IssueList.Insert(0, NewIssue);
            IR.SaveList();
            ReBind();
            NewIssuePanel.Expanded = false;
        }

        protected void Issues_PreRender(object sender, EventArgs e)
        {
            Issues.MasterTableView.GetColumn("AttachmentFile").Visible = false;
            Issues.MasterTableView.GetColumn("SendMailNotificationRequired").Visible = false;
        }
        
        
    }
}