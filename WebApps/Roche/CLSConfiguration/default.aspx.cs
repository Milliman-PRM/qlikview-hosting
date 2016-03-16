using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CLSConfiguration
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ( !IsPostBack)
            {
                string UserText = HttpContext.Current.Request.ServerVariables["AUTH_USER"];
                if (UserText.ToLower().Contains("root_milliman"))
                    UserText = "Welcome, " + UserText.Substring(UserText.IndexOf('\\')+1);
                UserID.Text = UserText;

                PostgresSqlAccess Postresql = new PostgresSqlAccess();
                AllSchemas.DataSource = Postresql.GetSchemas();
                AllSchemas.DataTextField = "schema_name";
                AllSchemas.DataValueField = "schema_name";
                AllSchemas.DataBind();
            }
        }

        protected void ActivateStaging_Click(object sender, EventArgs e)
        {
            UpdateStagingWebConfig();
        }

        private void UpdateStagingWebConfig()
        {
            string WebConfig = System.Configuration.ConfigurationManager.AppSettings["LabSystemsHandbookWebConfig"];
            if ( (System.IO.File.Exists(WebConfig)== true) && (AllSchemas.SelectedIndex != -1) )
            {
                string SelectedSchema = AllSchemas.SelectedItem.Text;
                string WebConfigContents = System.IO.File.ReadAllText(WebConfig);

                string SearchToken = "initial schema";
                int StartIndex = WebConfigContents.ToLower().IndexOf(SearchToken);
                int EndIndex = WebConfigContents.IndexOf('"', StartIndex);

                string NewWebConfig = WebConfigContents.Substring(0, StartIndex) + SearchToken + "=" + SelectedSchema + ";" + WebConfigContents.Substring(EndIndex);

                try
                {
                    System.IO.File.WriteAllText(WebConfig, NewWebConfig);

                }
                catch (Exception ex)
                {
                }
            }

        }


        protected void TestStaging_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void ActivateProduction_Click(object sender, EventArgs e)
        {

        }

        protected void TestProduction_Click(object sender, ImageClickEventArgs e)
        {

        }
    }
}