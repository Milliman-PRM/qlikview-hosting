
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Milliman.Reduction.Service {
    static class Program {
        private static System.Threading.AutoResetEvent _stopFlag = new System.Threading.AutoResetEvent(false);
        static private ILog _L;

        public static ILog Logger { get { return _L; } }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main() {

            InitializeLogger();
            var svc = new ReductionService();
            svc.Start(null);
            _stopFlag.WaitOne();

            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new SchedulerServiceHost()
            //};
            //ServiceBase.Run(ServicesToRun);
        }

        private static void InitializeLogger() {
            _L = LogManager.GetLogger("MM.ReductionSvcHost");
            _L.Info("Logger successfully initialized...");

            Assembly entryAssembly = Assembly.GetEntryAssembly();
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            _L.Info("EntryAssembly=" + entryAssembly);
            _L.Info("TotalUsedMemory=" + GC.GetTotalMemory(true));
            _L.Info("WindowsIdentity=" + ((current != null) ? current.Name : string.Empty));
            bool flag = entryAssembly.GetCustomAttributes(typeof(DebuggableAttribute), false).Length > 0;
            _L.Info("IsJITTrackingEnabled=" + flag);

            // Ok, this is a nice trick to learn what architecture are we running against
            if( IntPtr.Size == 4 ) {
                _L.Info("Environment=32 bit");
            } else {
                _L.Info("Environment=64 bit");
            }
            Milliman.Common.SysInfo.MemoryStatus memoryStatus = new Milliman.Common.SysInfo.MemoryStatus();
            _L.Info(memoryStatus.ToString());
        }
    }
}