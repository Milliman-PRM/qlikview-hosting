using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigComparisonGui
{
    class NameValueCfgElement : ConfigurationElement
    {
        public NameValueCfgElement() { }

        public NameValueCfgElement(string NameArg)
        {
            KeyName = NameArg;
        }

        [ConfigurationProperty("KeyName", IsRequired = true, IsKey = true)]
        public string KeyName
        {
            get { return (string) this["KeyName"]; }
            set { this["KeyName"] = value; }
        }

    }
}
