using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigIt
{
    /// <summary>
    /// This class allows us to work with our custom section
    /// </summary>
    public class KeyValueConfigurationSection : ConfigurationSection
    {
        public KeyValueConfigurationSection()
        {
            this["connectionStrings"] = new ConnectionStringSettingsCollection();
            this["elements"] = new KeyValueConfigurationCollection();
            this["applicationSettings"] = new ApplicationSettingsSectionGroup();
        }

        [ConfigurationProperty("connectionStrings")]
        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return (ConnectionStringSettingsCollection)this["connectionStrings"]; }
        }

        [ConfigurationProperty("elements")]
        public KeyValueConfigurationCollection Elements
        {
            get { return (KeyValueConfigurationCollection)this["elements"]; }
        }

        [ConfigurationCollection(typeof(JobElement))]
        public ApplicationSettingsSectionGroup AppSettingsGroup
        {
            get { return (ApplicationSettingsSectionGroup)base["applicationSettings"]; }
        }
    }

    public sealed class ApplicationSettingsSectionGroup : ConfigurationSectionGroup
    {
        #region Constructors
        public ApplicationSettingsSectionGroup() {}
        #endregion

        #region Properties
        [ConfigurationProperty("CLSConfiguration.Properties.Settings")]
        public ClientSettingsSection CLSConfigPropertiesSettings
        {
            get { return (ClientSettingsSection)base.Sections["CLSConfiguration.Properties.Settings"]; }
        }
        #endregion
    }

    /// <summary>
    /// An example configuration section class.
    /// </summary>
    public class ClientSettingsSection : ConfigurationSection
    {
        #region Constructors
        static ClientSettingsSection()
        {
            // Predefine properties here
            _type = new ConfigurationProperty("type", typeof(string), null, ConfigurationPropertyOptions.None);
            _requirePermission = new ConfigurationProperty("requirePermission", typeof(bool), false, ConfigurationPropertyOptions.None);
        }
        #endregion

        private static ConfigurationProperty _type;
        /// <summary>
        /// Gets the StringValue setting.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)base[_type]; }
        }

        private static ConfigurationProperty _requirePermission;
        /// <summary>
        /// Gets the StringValue setting.
        /// </summary>
        [ConfigurationProperty("requirePermission", IsRequired = true)]
        public string RequirePermission
        {
            get { return (string)base[_requirePermission]; }
        }              
        
    }
}
