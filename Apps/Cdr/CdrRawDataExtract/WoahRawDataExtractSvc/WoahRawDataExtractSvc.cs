using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CredentialManagement;

namespace WoahRawDataExtractSvc
{
    public partial class WoahRawDataExtractSvc : ServiceBase
    {
        RedoxRawDataExtractionManager RedoxExtractManager;

        public WoahRawDataExtractSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Pause enough time to attach debugger to this process
            int PauseTime = 10;
            for (int i=0; i<PauseTime ; i++)
            {
                Trace.WriteLine(String.Format("Paused {0} seconds of {1}", i, PauseTime));
                Thread.Sleep(1000);
            }

            CredentialManagement.Credential c;

            c = new Credential("MyUserName1", "MyPassword", "uvw1", CredentialType.Generic);
            c.PersistanceType = PersistanceType.LocalComputer;
            c.Description = "Description 1";
            c.Save();

            c = new Credential("MyUserName2", "MyPassword", "uvw2", CredentialType.Generic);
            c.PersistanceType = PersistanceType.LocalComputer;
            c.Description = "Description 2";
            c.Save();

            //c = new Credential("MyUserName3", "MyPassword", "uvw3", CredentialType.Generic);
            //c.PersistanceType = PersistanceType.LocalComputer;
            //c.Description = "Description 3";
            //c.Save();

            for (;;)
            {
                CredentialManagement.CredentialSet x = new CredentialSet("uvw");
                x.Load();

                CredentialManagement.Credential c2 = new Credential
                {
                    Target = "uvw2",
                    Type = CredentialType.Generic,
                    PersistanceType = PersistanceType.Enterprise
                };
                c2.Load();

                CredentialManagement.Credential c3 = new Credential
                {
                    Target = "uvw3",
                    Type = CredentialType.Generic,
                    PersistanceType = PersistanceType.LocalComputer
                };
                c3.Load();

                c2.Delete();
                c3.Delete();

            }

            RedoxExtractManager = new RedoxRawDataExtractionManager();
            RedoxExtractManager.StartThread();  // can override default settings, see signature
        }

        protected override void OnStop()
        {
            RedoxExtractManager.EndThread(5000);
            RedoxExtractManager = null;
        }
    }
}
