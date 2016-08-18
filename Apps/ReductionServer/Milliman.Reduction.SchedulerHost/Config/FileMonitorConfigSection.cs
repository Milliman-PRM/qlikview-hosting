using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Reduction.SchedulerHost.Config {
    public class FileMonitorConfigSection : System.Configuration.ConfigurationSection {
        [ConfigurationProperty("Monitors", IsDefaultCollection =true)]
        [ConfigurationCollection(typeof(FileMonitorCollection), AddItemName ="FileMonitor")]
        public FileMonitorCollection Monitors {
            get { return this["Monitors"] as FileMonitorCollection; }
            set { this["Monitors"] = value; }
        }

    }
}
