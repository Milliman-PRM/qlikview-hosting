
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Milliman.Reduction.SchedulerHost {
    public partial class SchedulerServiceHost: ServiceBase {
        private ILog _L;
        private ServiceHost _serviceHost;
        private readonly Uri _serviceAddress = new Uri("http://localhost:22335/SchedulerService");
        private static System.Threading.AutoResetEvent _stopFlag = new System.Threading.AutoResetEvent(false);
        //private SchedulerEngine.ISchedulerService svc;

        public SchedulerServiceHost() {
            _L = Milliman.Reduction.SchedulerHost.Program.Logger;
            InitializeComponent();
        }
        public void Start(string[] args) {
            _L.Info("Initializing service...");
            if(args != null && args.Length > 0 && _L.IsDebugEnabled )
                _L.Debug("arg: " + string.Join("\r\narg: ", args));
            this.OnStart(args);
            _L.Info("SchedulerService successfully started...");
        }
        protected override void OnStart(string[] args) {
            if( _serviceHost != null ) _serviceHost.Close();
            _L.Debug("Service::OnStart()");
            try {
                //svc = new ();
                _serviceHost = new ServiceHost(typeof(SchedulerEngine.SchedulerService), _serviceAddress);
                _L.Debug("Service::OnStart()->_serviceHOst.Open()");
                _serviceHost.Open();

                _L.Debug("SerivceHost successfully opened and waiting for external connections...");
                //_stopFlag.WaitOne();
            } catch(Exception ex ) {
                _L.Error("Error on Service::OnStart()->_serviceHost.Open()", ex);
            }
        }

        protected override void OnStop() {
            _L.Info("Stopping service...");
            if(_serviceHost != null ) { _serviceHost.Close(); _serviceHost = null; }
            _L.Info("Service successfully stopped...");
        }
    }
}
