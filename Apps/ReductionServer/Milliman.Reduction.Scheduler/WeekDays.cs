using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Reduction.SchedulerEngine {
    [Flags]
    public enum EnumWeekDays {
        Sun = 1,
        Mon = 2,
        Tue = 4,
        Wed = 8,
        Thu = 16,
        Fri = 32,
        Sat = 64
    }
}
