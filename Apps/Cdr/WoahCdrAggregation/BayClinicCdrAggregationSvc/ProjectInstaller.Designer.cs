namespace BayClinicCdrAggregationSvc
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.WoahBayClinicAggregationSvcProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.WoahBayClinicAggregationSvcInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // WoahBayClinicAggregationSvcProcessInstaller
            // 
            this.WoahBayClinicAggregationSvcProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.WoahBayClinicAggregationSvcProcessInstaller.Password = null;
            this.WoahBayClinicAggregationSvcProcessInstaller.Username = null;
            // 
            // WoahBayClinicAggregationSvcInstaller
            // 
            this.WoahBayClinicAggregationSvcInstaller.Description = "Aggregates raw data from Bay Clinic\'s daily Cerner ambulatory export";
            this.WoahBayClinicAggregationSvcInstaller.ServiceName = "WoahBayClinicAggregationSvc";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.WoahBayClinicAggregationSvcProcessInstaller,
            this.WoahBayClinicAggregationSvcInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller WoahBayClinicAggregationSvcProcessInstaller;
        private System.ServiceProcess.ServiceInstaller WoahBayClinicAggregationSvcInstaller;
    }
}