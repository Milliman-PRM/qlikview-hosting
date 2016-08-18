using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Reduction.SchedulerHost.Config {
    public class ProcessingElement: System.Configuration.ConfigurationElement {
        public ProcessingElement() { }
        public ProcessingElement(string name, string asm, string method, EnumMethodArgumentType arg) : this() {
            this.Name = name;
            this.Type = asm;
            this.Method = method;
            this.Arg = arg;
        }
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name {
            get { return this["Name"] as string; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("Type", IsRequired = true)]
        public string Type {
            get { return this["Type"] as string; }
            set { this["Type"] = value; }
        }

        [ConfigurationProperty("Method", IsRequired = true)]
        public string Method {
            get { return this["Method"] as string; }
            set { this["Method"] = value; }
        }

        [ConfigurationProperty("Arg", IsRequired = false, DefaultValue = EnumMethodArgumentType.FilePath)]
        public EnumMethodArgumentType Arg {
            get { return (EnumMethodArgumentType) this["Arg"]; }
            set { this["Arg"] = value; }
        }

        [ConfigurationProperty("OnSuccess", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ProcessingCollection), AddItemName ="Process")]
        public ProcessingCollection OnSuccess {
            get { return this["OnSuccess"] as ProcessingCollection; }
            set { this["OnSuccess"] = value; }
        }

        [ConfigurationProperty("OnFailure", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ProcessingCollection), AddItemName = "Process")]
        public ProcessingCollection OnFailure {
            get { return this["OnFailure"] as ProcessingCollection; }
            set { this["OnFailure"] = value; }
        }

    }
}
