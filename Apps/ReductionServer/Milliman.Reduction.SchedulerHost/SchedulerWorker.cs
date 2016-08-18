using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using System.Security.Principal;

namespace Milliman.Reduction.SchedulerHost {
    public delegate void FileProcessHandler(string file);
    public class SchedulerWorker {
        private ILog _L;
        private Dictionary<string, FolderMonitor> _monitorDictionary = new Dictionary<string, FolderMonitor>();

        private bool _isShuttingDown;

        public bool IsStandAloneEXE { get; set; }

        public bool IsShuttingDown { get { return this._isShuttingDown; } set { this._isShuttingDown = value ? value : this._isShuttingDown; } }

        public bool ShutDownComplete {
            get;
            private set;
        }
        public SchedulerWorker(Config.FileMonitorConfigSection section) : this() {
            foreach(Config.FileMonitorElement element in section.Monitors ) {
                this.AddMonitor(element);
            }

        }
        public SchedulerWorker() {
            _L = Milliman.Reduction.SchedulerHost.Program.Logger;
            this._isShuttingDown = false;
            this.IsStandAloneEXE = false;
            this.ShutDownComplete = false;
        }

        public void Run(object data) {
            Action<string> action = data as Action<string>;
            //SnmpService snmpService = null;
            try {
                foreach( var key in this._monitorDictionary.Keys ) 
                    this._monitorDictionary[key].Start();
            } catch( Exception ex ) {
                _L.Error("Unable to start up FrontEndObjects. Exiting Run Function...", ex);
            }

            try {
                while( !this.IsShuttingDown ) {
                    // ********* License Checker, in case this is needed in the future
                    // if(!LicenseChecker.IsServerLicensed() ) {
                    //    this.IsShuttingDown = true;
                    //}
                    // *********************************
                    // Yes, I know, this means we'll be polling for the changes. 
                    // Why do we do it, despite the fact that I'm anti-polling advocate? 
                    // Well, we cannot trust the internal failed objects managed to clean up nicely,
                    // and that we know something wrong happened. All we want is a flag, and the 
                    // service we'll be brought down... 
                    Thread.Sleep(1000);
                }
            } catch( Exception ex ) {
                _L.Error(null, ex);
            }
            this.StopMonitors();
            this.ShutDownComplete = true;
        }

        private void SchedulerWorker_OnNewFileFound(string path) {
            string folder = System.IO.Path.GetDirectoryName(path);
            string processedFoldername = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), "processed");
            if( !System.IO.Directory.Exists(processedFoldername) ) {
                _L.Info(string.Format( "Creating folder '{0}'", processedFoldername));
                System.IO.Directory.CreateDirectory(processedFoldername);
            }
            _L.Info(string.Format("Moving file '{0}' to '{1}'", path, System.IO.Path.Combine(processedFoldername, System.IO.Path.GetFileName(path))));
            System.IO.File.Move(path, System.IO.Path.Combine(processedFoldername, System.IO.Path.GetFileName(path)));
        }

        public void AddFolder(string folder, string pattern) {
            if( this._monitorDictionary.Keys.Contains(folder) ) return;
            this._monitorDictionary.Add(folder, new FolderMonitor(folder, pattern));
        }

        public void AddMonitor(Config.FileMonitorElement element) {
            FolderMonitor monitor = new FolderMonitor(element.FolderName);
            monitor.Name = element.Name;
            monitor.FilePattern = element.Pattern;
            monitor.ProcessSubFolders = element.SubFolders;
            monitor.WatcherInterval = element.SleepTime;
            // LazyLoad processing collection and assign as events to the FolderMonitor class
            foreach(Config.ProcessingElement process in element.Processing ) {
                if(!string.IsNullOrEmpty(process.Type))
                    monitor.AddProcess(process);
            }
            this._monitorDictionary.Add(monitor.Name, monitor);
        }

        private void StopMonitors() {
            foreach(var key in this._monitorDictionary.Keys ) {
                var monitor = this._monitorDictionary[key];
                if( monitor.State == Common.EnumThreadStatus.Running ) monitor.Stop();
                while( monitor.State != Common.EnumThreadStatus.Failed || monitor.State != Common.EnumThreadStatus.Waiting )
                    Thread.Sleep(100);
            }
        }

        
    }
}
