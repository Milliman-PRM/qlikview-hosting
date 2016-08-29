using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Milliman.Reduction.SchedulerHost {
    [RunInstaller(true)]
    public partial class SchedulerServiceInstaller: System.Configuration.Install.Installer {

        private ServiceProcessInstaller _process;
        private ServiceInstaller _installer;

        public SchedulerServiceInstaller() {
            _process = new ServiceProcessInstaller();
            _process.Account = ServiceAccount.NetworkService;
            _installer = new ServiceInstaller();
            _installer.ServiceName = "MillimanScheduler";
            _installer.DisplayName = "Milliman Reduction Scheduler";
            _installer.Description = "Manages schedules for Reduction and Monitoring of reduction files on folders...";
            _installer.StartType = ServiceStartMode.Automatic;
            Installers.Add(_process);
            Installers.Add(_installer);
        }
    }
}
