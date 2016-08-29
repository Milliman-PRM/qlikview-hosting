using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Reduction.SchedulerHost.Config {

    public class FileMonitorElement: ConfigurationElement {
        [ConfigurationProperty("PollingTime", IsRequired = false, DefaultValue = 5)]
        public int SleepTime{
            get { return (int) this["PollingTime"] ; }
            set { this["PollingTime"] = value; }
        }

        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name {
            get { return this["Name"] as string; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("FolderName", IsRequired = true)]
        public string FolderName {
            get { return this["FolderName"] as string; }
            set { this["FolderName"] = value; }
        }

        [ConfigurationProperty("Pattern", IsRequired = false, DefaultValue ="*.*")]
        public string Pattern {
            get { return this["Pattern"] as string; }
            set { this["Pattern"] = value; }
        }

        [ConfigurationProperty("SubFolders", IsRequired = false, DefaultValue =false)]
        public bool SubFolders {
            get { return (bool) this["SubFolders"]; }
            set { this["SubFolders"] = value; }
        }

        [ConfigurationProperty("Processing", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ProcessingCollection), AddItemName ="Process")]
        public ProcessingCollection Processing {
            get { return (ProcessingCollection) base["Processing"]; }
        }

    }
}