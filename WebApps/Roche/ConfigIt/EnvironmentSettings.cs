using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace ConfigIt
{
    public static class EnvironmentSettings
    {
        //public static readonly Configuration instance = GetConfiguration("Web.config");

        #region Public ReadOnly Properties
        public static string GetSafeElementValue(string p_configKey)
        {
            return (Elements[p_configKey] != null) ? Elements[p_configKey].Value : string.Empty;
        }

        private static KeyValueConfigurationCollection _elements;
        public static KeyValueConfigurationCollection Elements
        {
            get { return _elements; }
        }

        private static string _environment;
        public static string Environment
        {
            get { return _environment; }
        }

        private static ConnectionStringSettingsCollection _connectionStrings;
        public static ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return _connectionStrings; }
        }

        private static System.Net.Configuration.NetSectionGroup _systemNet;
        public static System.Net.Configuration.NetSectionGroup SystemNet
        {
            get { return _systemNet; }
        }

        private static ApplicationSettingsSectionGroup _applicationSettings;
        public static ApplicationSettingsSectionGroup ApplicationSettings
        {
            get { return _applicationSettings; }
        }

        #endregion
        //Constructor
        static EnvironmentSettings()
        {
            try
            {
                KeyValueConfigurationSection section = null;
                //ConfigurationSectionGroup appGroup = null;

                var config = WebConfigurationManager.OpenWebConfiguration("~");
                //Next Get which environment we are in based off what the machine name this code is running on
                section = config.GetSection("environmentConfiguration") as KeyValueConfigurationSection;

                //appGroup = config.GetSectionGroup("applicationSettings") as ApplicationSettingsSectionGroup;

                //based on machine name it sets environment
                //var machineName = System.Environment.MachineName.ToLower();
                /************************************************************************************/
                _environment = section.Elements["EnvironmentSetUp"].Value.ToString();
                /************************************************************************************/
                //var applicationSettingSectionGroup = config.SectionGroups["applicationSettings"];
                //var executableSection = applicationSettingSectionGroup.Sections["CLSConfigurationProductionDaemon.Properties.Settings"];

                //_applicationSettings = (ApplicationSettingsSectionGroup)config.GetSectionGroup("applicationSettings"); 
                _elements = ((KeyValueConfigurationSection)config.GetSection(_environment)).Elements;
                _connectionStrings = ((KeyValueConfigurationSection)config.GetSection(_environment)).ConnectionStrings;

                //SetConfigurationElementsFromProperties();
                SetConfigurationElementsFromConfigsElements();

                //var CLSConfigurationProductionDaemon = ConfigurationManager.GetSection("CLSConfigurationProductionDaemon") as NameValueCollection;
                //var CLSConfigurationProductionDaemon2 = ConfigurationManager.GetSection("CLSConfigurationProductionDaemon") as NameValueCollection;
                //var set = EnvironmentSectionGroup.GetCustomSetting("applicationSettings/MyApp.Properties.Settings",
                //                        "CLSConfigurationProductionDaemon_CLSConfigurationServices_CLSConfigurationServices");

                //NOTE we only want to save the config file if something changed...
                var somethingChanged = false;

                
                //ConfigurationSectionGroup appSettingsGroup = config.GetSectionGroup("applicationSettings");
                //ClientSettingsSection clientSettings = (ClientSettingsSection)appSettingsGroup.Sections["CLSConfiguration.Properties.Settings"];
                //ConfigurationElement element = clientSettings.Settings.Get("CLSConfiguration_CLSConfigurationServices_CLSConfigurationServices");
                //var xml = ((SettingElement)element).Value.ValueXml.InnerXml;
                //var xs = new XmlSerializer(typeof(string[]));
                //string[] strings = (string[])xs.Deserialize(new XmlTextReader(xml, XmlNodeType.Element, null));
                //foreach (string s in strings)
                //{
                //    //
                //}
                //_applicationSettings = (ConfigurationSectionGroup)config.GetSectionGroup("applicationSettings");

                //Update the email stuff if environment specific value is different
                _systemNet = (System.Net.Configuration.NetSectionGroup)config.GetSectionGroup("system.net");
                if (_systemNet.MailSettings.Smtp.Network.Host != null
                                        && _elements["SMTPServer"] != null
                                        && _systemNet.MailSettings.Smtp.Network.Host != _elements["SMTPServer"].Value)
                {
                    _systemNet.MailSettings.Smtp.Network.Host = _elements["SMTPServer"].Value;
                    somethingChanged = true;
                }

                //Update the connection strings if the environment specific value is different
                foreach (ConnectionStringSettings cons in _connectionStrings)
                {
                    var foundIt = false;
                    foreach (ConnectionStringSettings css in config.ConnectionStrings.ConnectionStrings)
                    {
                        if (cons.Name.Trim() == css.Name.Trim())
                        {
                            foundIt = true;
                            if (css.ConnectionString != cons.ConnectionString)
                            {
                                css.ConnectionString = cons.ConnectionString;
                                config.ConnectionStrings.SectionInformation.RestartOnExternalChanges = true;

                                somethingChanged = true;
                            }
                        }
                    }
                    if (!foundIt)
                    {
                        config.ConnectionStrings.ConnectionStrings.Add(cons);
                        config.ConnectionStrings.SectionInformation.RestartOnExternalChanges = true;

                        somethingChanged = true;
                    }
                }

                if (somethingChanged)
                {
                    config.Save(ConfigurationSaveMode.Modified);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void SetConfigurationElementsFromConfigsElements()
        {
            var ProductionSiteWebConfig = _elements["ProductionSiteWebConfig"].Value;
            var FromEmail = _elements["FromEmail"].Value;
            var Subject = _elements["Subject"].Value;
            var SMTPServer = _elements["SMTPServer"].Value;
            var SMTPPort = _elements["SMTPPort"].Value;
            var LabSystemsHandbookWebConfig = _elements["LabSystemsHandbookWebConfig"].Value;
            var PostgresSchemaBlackList = _elements["PostgresSchemaBlackList"].Value;
            var ProductionURL = _elements["ProductionURL"].Value;
            var StagingURL = _elements["StagingURL"].Value;
            var ValidIPs = _elements["ValidIPs"].Value;
            var MakeLiveDirectory = _elements["MakeLiveDirectory"].Value;
            var PGDump = _elements["PGDump"].Value;
            var PGRestore = _elements["PGRestore"].Value;
        }

        public class ConfigHelper
        {
            //public string GetUrl()
            //{
            //    var section = (ClientSettingsSection)ConfigurationManager.
            //                        GetSection("applicationSettings/CLSConfiguration.Properties.Settings");
            //    var url = section.Settings.Get("CLSConfiguration_CLSConfigurationServices_CLSConfigurationServices").Value.ValueXml.InnerText;
            //    return url;
            //}

        }
    }
}
