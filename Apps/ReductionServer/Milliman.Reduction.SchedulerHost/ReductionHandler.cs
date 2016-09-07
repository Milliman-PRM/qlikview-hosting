using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;   

namespace Milliman.Reduction.SchedulerHost {
    public class ReductionHandler {
        private ILog _L;

        public ReductionHandler() {
            _L = log4net.LogManager.GetLogger("Milliman Scheduler Service");
        }
        public void ProcessComplete(string filePath) {
            _L.Info(string.Format("Calling method ReductionHandler::ProcessComplete('{0}')", filePath));
        }

        public void BackupFile(string folderName) {
            _L.Info(string.Format("Calling method ReductionHandler::BackupFile('{0}')", folderName));
        }

        public void FailRecovery(string filePath) {
            _L.Info(string.Format("Calling method ReductionHandler::FailRecovery('{0}')", filePath));

        }

    }
}
