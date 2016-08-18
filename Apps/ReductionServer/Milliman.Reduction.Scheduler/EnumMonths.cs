using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Reduction.SchedulerEngine {
    [Flags]
    public enum EnumMonths {
        Jan = 1,
        Feb = 2,
        Mar = 4,
        Apr = 8,
        May = 16,
        Jun = 32,
        Jul = 64,
        Ago = 128,
        Sep = 256,
        Oct = 512,
        Nov = 1024,
        Dec = 2048
    }
}
