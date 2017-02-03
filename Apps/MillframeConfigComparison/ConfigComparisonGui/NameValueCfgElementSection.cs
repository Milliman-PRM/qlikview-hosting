using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigComparisonGui
{
    class NameValueCfgElementSection : ConfigurationSection
    {
        public NameValueCfgElementSection()
        {
            Product = "Default";
            Version = "Default";
        }

        public NameValueCfgElementSection(string VersionArg, string ProductArg)
        {
            Product = ProductArg;
            Version = VersionArg;
        }

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(NameValueCfgElementCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public NameValueCfgElementCollection AllNameValueElements
        {
            get { return (NameValueCfgElementCollection)base[""]; }
            set { this[""] = value; }
        }

        [ConfigurationProperty("Product", IsKey = true, IsRequired = true, IsDefaultCollection = true)]
        public string Product
        {
            get { return (string)base["Product"]; }
            set { this["Product"] = value; }
        }

        [ConfigurationProperty("Version", IsKey = true, IsRequired = true, IsDefaultCollection = true)]
        public string Version
        {
            get { return (string)base["Version"]; }
            set { this["Version"] = value; }
        }

        // not a cfg file property
        public string SectionNameInFile
        {
            get { return NameValueCfgElementSection.SectionName(Product, Version); }
        }

        // The go to place to convert product and version to a section name string
        public static string SectionName(string Product, string Version)
        {
            return (Product + Version).Replace(" ", "");
        }
    }
}
