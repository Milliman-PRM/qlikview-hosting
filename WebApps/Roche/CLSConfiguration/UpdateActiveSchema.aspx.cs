using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CLSConfiguration
{
    public partial class UpdateActiveSchema : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ( !IsPostBack)
            {
                Status.Text = "Retrieving database information for schema '" + Session["SCHEMANAME"].ToString() + "'";
                MakeLive();
            }
        }

        private void MakeLive()
        {
            string SchemaName = Session["SCHEMANAME"].ToString();
            string Server = string.Empty;
            string PostgresPort = string.Empty;
            string UserName = string.Empty;
            string Password = string.Empty;
            string DatabaseName = string.Empty;
            string InitialSchemaName = string.Empty;
            bool IntegratedSecurity = false;

            if ( CLSConfigurationCommon.PostgresqlUtilities.ParametersFromConnectionString( out Server,
                                                                                            out PostgresPort,
                                                                                            out UserName,
                                                                                            out Password,
                                                                                            out DatabaseName, 
                                                                                            out InitialSchemaName,
                                                                                            out IntegratedSecurity) )
            {
                string PGDump = System.Configuration.ConfigurationManager.AppSettings["PGDump"];
                string DumpFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), SchemaName + ".backup");
                System.IO.File.Delete(DumpFile);  //delete if already exists
                bool Results = false;
                if ( IntegratedSecurity == true )
                {
                    Results = CLSConfigurationCommon.PostgresqlUtilities.CreatePostreSQLBackup(Server, PostgresPort, DatabaseName, SchemaName, PGDump, DumpFile);
                }
                else
                {
                    Results = CLSConfigurationCommon.PostgresqlUtilities.CreatePostreSQLBackup(Server, PostgresPort, DatabaseName, SchemaName, UserName, Password, PGDump, DumpFile);
                }

                if ( Results )
                {
                    Status.Text = "Pushing database/schema to production server.";
                    CLSConfigurationServices.CLSConfigurationServices Production = new CLSConfigurationServices.CLSConfigurationServices();
                    byte[] DumpFileContents = System.IO.File.ReadAllBytes(DumpFile);
                    if ( Production.DatabaseRestore(SchemaName, DumpFileContents) == true )
                    {
                        string OperatorEmail = HttpContext.Current.Request.ServerVariables["AUTH_USER"] + "@milliman.com";  //must be a milliman person
                        if (OperatorEmail.Contains('\\'))
                            OperatorEmail = OperatorEmail.Substring(OperatorEmail.IndexOf('\\') + 1);  //remove any domain that might be prefixed

                        if ( Production.MakeLive( SchemaName, OperatorEmail, DateTime.Now) )
                        {
                            Status.Text = "Production system database has been updated successfully.  On the next web site refresh cyle, the update will be made live to users.  You will recieve an email when the refresh cycle completes.";
                        }
                    }
                }
                Refresh.Visible = true;
            }
            
        }

        protected void Refresh_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx");
        }
    }
}