using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Reduction.SchedulerHost {
    public class ServiceListener {
        static ServiceListener _singletonInstance;
        static readonly object _instanceLock = new object();

        public static ServiceListener Instance {
            get {
                if(_singletonInstance == null ) {
                    lock(_instanceLock)
                        _singletonInstance = _singletonInstance ?? new ServiceListener();
                }
                return _singletonInstance;
            }
        }
    }
}
