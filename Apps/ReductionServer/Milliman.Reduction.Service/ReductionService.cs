using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceProcess;
using log4net;

namespace Milliman.Reduction.Service {
    public class ReductionService: ServiceBase {
        private ILog _L;
        private ServiceHost _serviceHost;
        private ReductionEngine.IReductionService svc;
        private System.ComponentModel.IContainer components = null;

        public ReductionService() {
            this._L = Milliman.Reduction.Service.Program.Logger;
            this.InitializeComponent();
        }
        public void Start(string[] args) {
            _L.Info("Initializing service...");
            if( args != null && args.Length > 0 && _L.IsDebugEnabled )
                _L.Debug("arg: " + string.Join("\r\narg: ", args));
            this.OnStart(args);
            _L.Info("ReductionService successfully started...");
        }
        protected override void OnStart(string[] args) {
            if( _serviceHost != null ) _serviceHost.Close();
            _L.Debug("Service::OnStart()");
            try {
                svc = new ReductionEngine.ReductionService();
                _serviceHost = new ServiceHost(svc);
                svc.Init();
                _L.Debug("Service::OnStart()->_serviceHOst.Open()");
                _serviceHost.Open();
                
                _L.Debug("SerivceHost successfully opened and waiting for external connections...");
            } catch(Exception ex ) {
                _L.Error("Error on Service::OnStart()->_serviceHost.Open()", ex);
            }
        }

        protected override void OnStop() {
            base.OnStop();  
        }
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            this.ServiceName = "MillimanReductionSvc";
        }

        protected override void Dispose(bool disposing) {
            if( disposing && (components != null) ) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
