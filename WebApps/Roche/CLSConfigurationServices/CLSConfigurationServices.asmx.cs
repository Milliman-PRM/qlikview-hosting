using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace CLSConfigurationServices
{
    /// <summary>
    /// Summary description for CLSConfigurationServices
    /// </summary>
    [WebService(Namespace = "http://labsystemshandbook.milliman.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CLSConfigurationServices : System.Web.Services.WebService
    {

        private string ActiveSchemaFilename = "ActiveSchema.txt";
        private string PendingActiveSchemaFilename = "PendingActiveSchema.txt";

        /// <summary>
        /// As minimum security check our IP whitelist, if not in list then no access allowed
        /// </summary>
        /// <returns></returns>
        private bool IsValidConnection()
        {
            bool IsValid = false;

            try
            {
                List<string> IPs = new List<string>();
                string ValidIPS = System.Configuration.ConfigurationManager.AppSettings["ValidIPs"];
                IPs.AddRange(ValidIPS.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                if (IPs.Contains("*"))
                    return true;  //we accept all

                IsValid = (IPs.Contains(HttpContext.Current.Request.UserHostAddress.ToLower()) || (IPs.Contains(HttpContext.Current.Request.Url.Host.ToLower())));
                if (IsValid == false)
                    CLSConfigurationCommon.Utilities.Log( CLSConfigurationCommon.Utilities.ReportType.Warning, "Invalid IP request denied access:" + HttpContext.Current.Request.UserHostAddress.ToLower());

            }
            catch (Exception ex)
            {
                CLSConfigurationCommon.Utilities.Log( CLSConfigurationCommon.Utilities.ReportType.Error,"", ex);
            }
            return IsValid;
        }

        [WebMethod]
        public bool DatabaseRestore( string SchemaName,
                                     byte[] DBBackup )
        {
            if (IsValidConnection() == false)
                return false;

            string PGRestore = System.Configuration.ConfigurationManager.AppSettings["PGRestore"];
          
            string DumpFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), SchemaName + ".backup");
            System.IO.File.WriteAllBytes(DumpFile, DBBackup);

            string ConStringSchemaName = string.Empty;
            string Server = string.Empty;
            string PostgresPort = string.Empty;
            string UserName = string.Empty;
            string Password = string.Empty;
            string DatabaseName = string.Empty;
            string InitialSchemaName = string.Empty;
            bool IntegratedSecurity = false;

            if ( CLSConfigurationCommon.PostgresqlUtilities.ParametersFromConnectionString(out Server,
                                                                                           out PostgresPort,
                                                                                           out UserName,
                                                                                           out Password,
                                                                                           out DatabaseName,
                                                                                           out InitialSchemaName,
                                                                                           out IntegratedSecurity))
            {
                if (CLSConfigurationCommon.PostgresqlUtilities.RestorePostreSQLFromBackup(Server, PostgresPort, DatabaseName, SchemaName, UserName, Password, PGRestore, DumpFile) == false )
                {
                    //error condition
                }
            }

            return true;
        }

        [WebMethod]
        public bool MakeLive( string SchemaName,
                              string OperatorEmail,
                              DateTime GoLiveDateTime )
        {
            if (IsValidConnection() == false)
                return false;

            string MakeLiveDir = System.Configuration.ConfigurationManager.AppSettings["MakeLiveDirectory"];
            string MakeLiveFile = System.IO.Path.Combine(MakeLiveDir, PendingActiveSchemaFilename);

            Schema CurrentSchema = new Schema(SchemaName, OperatorEmail, GoLiveDateTime);
            return CurrentSchema.Save(MakeLiveFile);
        }

        [WebMethod]
        public bool NewSchemaIsReady( out string SchemaName,
                                      out string OperatorEmail )
        {
            SchemaName = string.Empty;
            OperatorEmail = string.Empty;
            if (IsValidConnection() == false)
                return false;

            string MakeLiveDir = System.Configuration.ConfigurationManager.AppSettings["MakeLiveDirectory"];
            string MakeLiveFile = System.IO.Path.Combine(MakeLiveDir, PendingActiveSchemaFilename);
            if ( System.IO.File.Exists(MakeLiveFile))
            {
                Schema Current = Schema.Load(MakeLiveFile);
                SchemaName = Current.SchemaName;
                OperatorEmail = Current.OperatorEmail;
                return true;
            }
            return false;
        }

        [WebMethod]
        public void SetActiveSchemaName( string SchemaName )
        {
            string MakeLiveDir = System.Configuration.ConfigurationManager.AppSettings["MakeLiveDirectory"];
            string MakeLiveFile = System.IO.Path.Combine(MakeLiveDir, PendingActiveSchemaFilename);
            //production has made request active, so get rid of any pending requests
            System.IO.File.Delete(MakeLiveFile);
            string ActiveSchemaFile = System.IO.Path.Combine(MakeLiveDir, ActiveSchemaFilename);
            System.IO.File.Delete(ActiveSchemaFile);
            System.IO.File.WriteAllText(ActiveSchemaFile, SchemaName);

            //delete all schemas other than 'SchemaName' to prevent DB build up
            System.Data.DataTable DT = CLSConfigurationCommon.PostgresqlUtilities.GetSchemas();
            foreach( System.Data.DataRow DR in DT.Rows  )
            {
                string schema_name = DR["schema_name"].ToString();
                if ( string.Compare( schema_name, SchemaName, true) != 0) //never delete the active schema
                {
                    CLSConfigurationCommon.PostgresqlUtilities.DeleteSchema(schema_name);
                }
            }
        }

        [WebMethod]
        public string GetActiveSchemaName()
        {
            string MakeLiveDir = System.Configuration.ConfigurationManager.AppSettings["MakeLiveDirectory"];
            string ActiveSchemaFile = System.IO.Path.Combine(MakeLiveDir, ActiveSchemaFilename);
            if ( System.IO.File.Exists(ActiveSchemaFile))
            {
                return System.IO.File.ReadAllText(ActiveSchemaFile);
            }
            return "";
        }
    }
}
