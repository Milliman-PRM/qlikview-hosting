using System;
using log4net;

namespace Milliman.Reduction.SchedulerSender {
    public class Sender {
        ILog _L = LogManager.GetLogger(typeof(Sender));

        public Sender() {
            // Establish communication with ReductionService
        }

        public void Send(string filePath) {
            using( var client = new Reduction.ReductionServiceClient() ) {
                _L.Debug("Initializing comm with ReductionService");
                for(int i = 1; i <= 2000; i++ ) {
                    try {
                        _L.Debug(string.Format("Try {0} of 2000: sending file '{1}' to be processed by ReductionSvc", i, filePath));
                        client.EnqueueReductionFolder(filePath);
                        _L.Debug(string.Format("File '{0}' successfully sent to ReductSvc",filePath));
                        return;
                    } catch( Exception ex ) {
                        if( i >= 20 ) /* If we've retried 20 times, it's about time to give up.*/
                            throw;
                        else {/* Wait two seconds before retrying*/
                            _L.Debug(string.Format("Error while trying to send file to ReductSvc: {0}\r\nWaiting 2 seconds and retrying...", ex.Message));
                            System.Threading.Thread.Sleep(2000);
                        }
                    }
                }
            }
        }

        public void SendAsync(string filePath) {

        }
    }
}
