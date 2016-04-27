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
    }    
}
