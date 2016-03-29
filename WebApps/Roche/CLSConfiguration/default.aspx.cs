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

            //CLSConfigurationCommon.PostgresqlUtilities.CreatePostgresqlBackup("indy-pgsql01.milliman.com", "5432", "Roche_Medicare_Reimbursement_Develop", "rmrrdb_20160311", "roche_admin", "MdDB4gHV", @"C:/Program Files/PostgreSQL/9.5/bin\pg_dump.exe", @"c:\tmp\schema.backup", true);
            //CLSConfigurationCommon.PostgresqlUtilities.RestorePostgresqlBackup("localhost", "5432", "Roche_Medicare_Reimbursement_Develop", "rmrrdb_20160311", "postgres", "jellyfish", @"C:/Program Files/PostgreSQL/9.5/bin\pg_restore.exe", @"c:\tmp\schema.backup", false);

            if ( !IsPostBack)
            {
                string UserID = HttpContext.Current.Request.ServerVariables["AUTH_USER"];
                if (string.IsNullOrEmpty(UserID) == false)
                {
                    if (UserID.ToLower().Contains("root_milliman"))
                        UserID = UserID.Substring(UserID.IndexOf('\\') + 1);
                    this.UserID.Text = "Welcome, " + UserID;
                }

                AllSchemas.DataSource = CLSConfigurationCommon.PostgresqlUtilities.GetSchemas();
                AllSchemas.DataTextField = "schema_name";
                AllSchemas.DataValueField = "schema_name";
                AllSchemas.DataBind();

                string TestStagingURL = System.Configuration.ConfigurationManager.AppSettings["StagingURL"];
                if (string.IsNullOrEmpty(TestStagingURL) == false)
                {
                    string textHTML = "window.open('" + TestStagingURL + "', '_blank'); ";
                    this.TestStaging.Attributes.Add("OnClick", textHTML);
                }
                string TestProductionURL = System.Configuration.ConfigurationManager.AppSettings["ProductionURL"];
                if (string.IsNullOrEmpty(TestProductionURL) == false)
                {
                    string textHTML = "window.open('" + TestProductionURL + "', '_blank'); ";
                    this.TestProduction.Attributes.Add("OnClick", textHTML);
                }

                ProductionSchema.Text = GetProductionSchema();
                StagingSchema.Text = GetStagingSchema();

                ListItem LI =  AllSchemas.Items.FindByText(StagingSchema.Text);
                if (LI != null)
                    LI.Selected = true;
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
                string CurrentSchema = FindSchemaNameInWebConfig();
                string WebConfigContents = System.IO.File.ReadAllText(WebConfig);

                string NewWebConfig = WebConfigContents.Replace(CurrentSchema, SelectedSchema);

                try
                {
                    System.IO.File.WriteAllText(WebConfig, NewWebConfig);
                    StagingSchema.Text = SelectedSchema;

                }
                catch (Exception ex)
                {
                    CLSConfigurationCommon.Utilities.Log(CLSConfigurationCommon.Utilities.ReportType.Error, "", ex);
                }
            }

        }


        protected void ActivateProduction_Click(object sender, EventArgs e)
        {
            //get production schema/data database machine
            //upload schema/data via web service
            string SchemaName = AllSchemas.Text;
            if (string.IsNullOrEmpty(SchemaName) == false)
            {
                Session["SCHEMANAME"] = SchemaName;
                Response.Redirect("UpdateActiveSchema.aspx");
            }
        }

        private string GetProductionSchema()
        {
            CLSConfigurationServices.CLSConfigurationServices CLSServices = new CLSConfigurationServices.CLSConfigurationServices();
            return CLSServices.GetActiveSchemaName();
        }

        private string GetStagingSchema()
        {
            return FindSchemaNameInWebConfig();
        }

        private string FindSchemaNameInWebConfig()
        {
            string SchemaName = string.Empty;
            string WebConfig = System.Configuration.ConfigurationManager.AppSettings["LabSystemsHandbookWebConfig"];
            if (System.IO.File.Exists(WebConfig) == true)
            {
                string WebConfigContents = System.IO.File.ReadAllText(WebConfig);

                string SearchToken = "initial schema";
                int StartIndex = WebConfigContents.ToLower().IndexOf(SearchToken) + SearchToken.Length;
                int QuotesEndIndex = WebConfigContents.IndexOf('"', StartIndex);
                int SemicolonEndIndex = WebConfigContents.IndexOf(';', StartIndex);
                int EndIndex = QuotesEndIndex;
                if (SemicolonEndIndex < QuotesEndIndex)  //could be ';' or '"' delimited
                    EndIndex = SemicolonEndIndex;  
                string CandidateName = WebConfigContents.Substring(StartIndex, EndIndex - StartIndex);
                //a bit brute force, but still a fast way to check
                string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJLKMNOPQRSTUVWXYZ-_0123456789";
                foreach (char C in CandidateName)
                {
                    if (validChars.Contains(C) == true)
                        SchemaName += C;
                }
            }
            return SchemaName;
        }
    }
}