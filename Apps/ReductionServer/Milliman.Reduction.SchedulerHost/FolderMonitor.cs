using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
namespace Milliman.Reduction.SchedulerHost {

    public delegate void FolderHandler(string path);
    public class FolderMonitor {
        ILog _L;
        Thread _processThread;
        FolderHandler _onNewFileFoundHandler;
        FolderHandler _onNewFileProcessedHandler;
        private string _folder;
        private bool _shouldStop;
        private DateTime _lastChecked;
        private HashSet<string> _cacheRunningFiles = new HashSet<string>();
        private Milliman.Common.EnumThreadStatus _status = Common.EnumThreadStatus.Waiting;
        List<DynamicProcessor> _processors = new List<DynamicProcessor>();
        public string Name { get; set; }
        public string FilePattern { get; set; }
        public bool ProcessSubFolders { get; set; }

        public event FolderHandler OnNewFileFound {
            add { this._onNewFileFoundHandler += value; }
            remove { this._onNewFileFoundHandler -= value; }
        }

        public event FolderHandler OnNewFileProcessed {
            add { this._onNewFileProcessedHandler += value; }
            remove { this._onNewFileProcessedHandler -= value; }
        }
        public Milliman.Common.EnumThreadStatus State { get { return this._status; } }
        public int WatcherInterval { get; set; }

        public FolderMonitor() {
            this._status = Common.EnumThreadStatus.Waiting;
            this.WatcherInterval = 5;
            _L = Milliman.Reduction.SchedulerHost.Program.Logger;
        }
        public FolderMonitor(string folder) : this() {
            this._folder = folder;
        }

        public FolderMonitor(string folder, string pattern) : this(folder) {
            this._folder = folder;
            this.FilePattern = pattern;
        }

        public bool Start() {
            this._status = Common.EnumThreadStatus.Starting;
            bool result = false;
            try {
                if( this._processThread != null && this._processThread.IsAlive )
                    this._processThread.Abort();
                this._processThread = new Thread(new ParameterizedThreadStart(Watch));
                this._processThread.IsBackground = true;
                _L.Debug("Starting Watcher Thread ...");
                this._processThread.Start();
                _L.Info(string.Format("Watcher for folder '{0}' successfully started...", this._folder));
                this._status = Common.EnumThreadStatus.Running;
            } catch( Exception ex ) {
                _L.Error(string.Format("Fail on starting up of folder monitor for folder '{0}'", this._folder), ex);
                this._status = Common.EnumThreadStatus.Failed;
            }
            return result;
        }
        public bool Start(string folder) {
            this._folder = folder;
            return this.Start();
        }

        private void Watch(object data) {
            Thread.Sleep(1000);
            string folder = this._folder as string;
            if( string.IsNullOrEmpty(folder) ) {
                _L.Error(string.Format("Cannot monitor folder '{0}'. Reason: Folder's missing", folder));
                this._status = Common.EnumThreadStatus.Failed;
                return;
            }

            while( true ) {
                try {
                    if( !System.IO.Directory.Exists(folder) ) {
                        _L.Error(string.Format("Cannot monitor folder '{0}'. Reason: folder does not exist ", folder));
                        this._status = Common.EnumThreadStatus.Failed;
                        return; ;
                    }
                } catch( Exception ex ) {
                    _L.Error(string.Format("Cannot monitor folder '{0}'.Reason: Unknown ", folder), ex);
                    this._status = Common.EnumThreadStatus.Failed;
                    return;
                }

                // All right, folder's not empty, folder exists, and we can actually get to it. 
                try {
                    _L.Debug(string.Format("Searching folder '{0}', with pattern '{1}', {2}considering subfolders",
                        folder, this.FilePattern, this.ProcessSubFolders ? "": "NOT "));
                    string[] files = System.IO.Directory.GetFiles(folder, this.FilePattern, this.ProcessSubFolders ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly);
                    _L.Debug(string.Format("Found {0} files...", files.Length));

                    foreach(var found_file in files ) {
                        if( _cacheRunningFiles.Contains(found_file) ) {
                            _L.Info(string.Format("Found file '{0}' for processing, but it seems like it's already being processed by a previous folder sweep. The file will be skipped", found_file));
                            continue;
                        } else {
                            _cacheRunningFiles.Add(found_file);
                        }

                        if(this._processors.Count > 0 ) { 
                            foreach(var p in this._processors ) {
                                try {
                                    p.Execute(p.Arg == Config.EnumMethodArgumentType.FilePath ? found_file : Path.GetDirectoryName(found_file));
                                    // File's removed from the cached running file list once the process' over
                                    _cacheRunningFiles.Remove(found_file);
                                }catch(Exception ex ) {
                                    // TODO: Log the exception
                                    _L.Error("An error occurred while trying to call out for external program", ex);
                                }
                            }
                        } else {
                            _L.Info("A new file was found, but no action was configured to be performed once it's found... sorry");
                        }
                    }
                } catch(Exception ex) {
                    _L.Error("An error occurred while trying to get and process files from folder...", ex);
                    this._status = Common.EnumThreadStatus.Failed;
                    return;
                }

                this._lastChecked = DateTime.Now;
                

                if( _shouldStop ) {
                    this._status = Common.EnumThreadStatus.Stopping;
                    _L.Info("Break order received. Exiting file watcher service...");
                    break;
                }
                _L.Info(string.Format("Waiting for {0} seconds before trying to read folder again ...", this.WatcherInterval/1000));
                // wait for X seconds (according to settings)
                Thread.Sleep(this.WatcherInterval);
                if( _shouldStop ) {
                    this._status = Common.EnumThreadStatus.Stopping;
                    _L.Info("Break order received. Exiting file watcher service...");
                    break;
                }
            }
            this._status = Common.EnumThreadStatus.Waiting;
        }

        public void Stop() {
            // Clean out resources

            // Flags out the stopping
            this._shouldStop = true;
        }
        public void AddProcess(Config.ProcessingElement element) {
            var p = GetHandler(element);
            if(p != null)
                this._processors.Add(p);
        }

        private DynamicProcessor GetHandler(Config.ProcessingElement element) {
            DynamicProcessor handler = new DynamicProcessor();
            handler._L = this._L;
            // First we load the type of the processor
            try {
                Type t = Type.GetType(element.Type, true);
                var o = Activator.CreateInstance(t);
                MethodInfo m = t.GetMethod(element.Method);

                handler.Arg = element.Arg;
                handler.Method = m;
                handler.Instance = o;
                handler.Name = element.Name;

                foreach(Config.ProcessingElement p1 in element.OnSuccess ) {
                    var p2 = GetHandler(p1);
                    if( p2 != null )
                        handler.OnSuccess.Add(p2);
                }
                foreach( Config.ProcessingElement p1 in element.OnFailure) {
                    var p2 = GetHandler(p1);
                    if( p2 != null )
                        handler.OnFailure.Add(p2);
                }

            } catch( Exception ) {
                handler = null;
            }

            return handler;
        }
    }
}