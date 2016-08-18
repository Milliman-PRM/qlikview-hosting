using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Reduction.SchedulerEngine {
    public class Engine {
        private System.Threading.Thread thd;

        public void Start() {
            if( thd != null && thd.ThreadState == System.Threading.ThreadState.Running ) return;
            thd = new System.Threading.Thread(() => this.MonitorFolder());
            thd.Start();
        }

        public void Stop() {

        }

        private void MonitorFolder() {

        }
    }
}
