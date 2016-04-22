using CLSdbContext;
using ConfigIt;
using Controller;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CLSMedicareReimbursement
{
    public partial class ConfigIt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Getting config items from the config file directly
            var config = WebConfigurationManager.OpenWebConfiguration("~");
            lblConfigFile.Text = config.FilePath;

            // var ConfigurationUserLevelconfigPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            //lblConfigurationUserLevelconfigPath.Text = ConfigurationUserLevelconfigPath;

            // var ConfigurationGetExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            // lblConfigurationGetExecutingAssembly.Text = ConfigurationGetExecutingAssembly;
            var databaseName = "CLSdbDataContextConnectionString";
            
            //Getting config items from the EnvironmentSettings static var...
            lbEnvior.Text = "You are in || " + EnvironmentSettings.Environment.ToUpper() + " || Environment";
            lbEnvirSQL.Text = EnvironmentSettings.ConnectionStrings[databaseName].ConnectionString;
            lbEnvirSMTP.Text = EnvironmentSettings.SystemNet.MailSettings.Smtp.Network.Host;

            //Getting config items from WebConfigurationManager
            lbWebConfigManSQL.Text = WebConfigurationManager.ConnectionStrings[databaseName].ConnectionString;
            
            lbWebConfigDirectSQL.Text = config.ConnectionStrings.ConnectionStrings[databaseName].ConnectionString;
            var _systemNet = (System.Net.Configuration.NetSectionGroup)config.GetSectionGroup("system.net");
            lbWebConfigDirectSMTP.Text = _systemNet.MailSettings.Smtp.Network.Host;

            //Getting Config items from ConfigurationManager NOTE this is not suggested in a Web site.
            lbConfigManSQL.Text = ConfigurationManager.ConnectionStrings[databaseName].ConnectionString;
            
            //read values form the configs 
            var ProductionSiteWebConfig = EnvironmentSettings.Elements["ProductionSiteWebConfig"].Value;
            var FromEmail = EnvironmentSettings.Elements["FromEmail"].Value;
            var Subject = EnvironmentSettings.Elements["Subject"].Value;
            var SMTPServer = EnvironmentSettings.Elements["SMTPServer"].Value;
            var SMTPPort = EnvironmentSettings.Elements["SMTPPort"].Value;
            var LabSystemsHandbookWebConfig = EnvironmentSettings.Elements["LabSystemsHandbookWebConfig"].Value;
            var PostgresSchemaBlackList = EnvironmentSettings.Elements["PostgresSchemaBlackList"].Value;
            var ProductionURL = EnvironmentSettings.Elements["ProductionURL"].Value;
            var StagingURL = EnvironmentSettings.Elements["StagingURL"].Value;
            var ValidIPs = EnvironmentSettings.Elements["ValidIPs"].Value;
            var MakeLiveDirectory = EnvironmentSettings.Elements["MakeLiveDirectory"].Value;
            var PGDump = EnvironmentSettings.Elements["PGDump"].Value;
            var PGRestore = EnvironmentSettings.Elements["PGRestore"].Value;
            var Memory = EnvironmentSettings.Elements["Memory"].Value;
            var DiskSpace = EnvironmentSettings.Elements["DiskSpace"].Value;
            var DiskDrives = EnvironmentSettings.Elements["DiskDrives"].Value;

            lblInfo.Text = ProductionSiteWebConfig + " || " +
                            FromEmail + " || " +
                            Subject + " || " +
                            SMTPServer + " || " +
                            SMTPPort + " || " +
                            LabSystemsHandbookWebConfig + " || " +
                            PostgresSchemaBlackList + " || " +
                            ProductionURL + " || " +
                            StagingURL + " || " +
                            ValidIPs + " || " +
                            MakeLiveDirectory + " || " +
                            PGDump + " || " +
                            PGRestore + " || " +
                            Memory + " || " +
                            DiskSpace + " || " +
                            DiskDrives;

            var objList = new List<Locality>();
            objList = CLSController.getUniqueLocality();

            lblInfo2.Text = lblInfo2.Text + " || Locatlity Count = " +  objList.Count;
        }
    }
}