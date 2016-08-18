using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Milliman.Reduction.SchedulerHost {


    public class DynamicProcessor {
        internal ILog _L;
        public Milliman.Reduction.SchedulerHost.Config.EnumMethodArgumentType Arg;
        public string Name { get; set; }
        public string AssemblyName { get; set; }
        public string MethodName { get; set; }
        public object Instance { get; set; }
        public MethodInfo Method { get; set; }

        public List<DynamicProcessor> OnSuccess;
        public List<DynamicProcessor> OnFailure;

        public DynamicProcessor() {
            OnSuccess = new List<DynamicProcessor>();
            OnFailure = new List<DynamicProcessor>();
        }

        public void Execute(string argument) {
            try {
                _L.Debug(string.Format("Preparing to execute process '{0}'", this.Name));
                if(this.Method == null ) {
                    _L.Info("Cannot execute inexistent method. Exiting Execution thread...");
                    return;
                }
                this.Method.Invoke(this.Instance, new object[] { argument });
                _L.Info(string.Format("FileMonitor '{0}' successfully processed source '{1}'", this.Name, argument));
                foreach(var p in this.OnSuccess ) {
                    _L.Info(string.Format("Found chained event '{0}' for monitor '{1}'", p.Name, this.Name));
                    p.Execute(argument);
                }
            } catch( Exception e1 ) {
                if( OnFailure.Count > 0) {
                    foreach( var f in OnFailure )
                        try {
                            _L.Info(string.Format("Found chained error event '{0}' for monitor '{1}'",f.Name, this.Name));
                            f.Execute(argument);
                        }catch(Exception e2 ) {
                            _L.Error(string.Format("Error process '{0}' for '{1}' failed to execute. ", f.Name, this.Name), e2);
                        }
                } else {
                    _L.Error(string.Format("No error handler found for '{0}'. Throwing the error up to the caller", this.Name), e1);
                    throw;
                }
            }
        }

    }
}
