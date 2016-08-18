using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Reduction.SchedulerEngine {
    [DataContract]
    public class Schedule {
        [DataMember]
        public EnumScheduleType ScheduleType { get; set; }
        [DataMember]
        public DateTime ScheduleTime { get; set; }
        [DataMember]
        public TimeSpan Interval { get; set; }
        [DataMember]
        public EnumWeekDays WeekDays { get; set; }
        [DataMember]
        public EnumMonths Months { get; set; }
        [DataMember]
        public bool WatchSubfolders { get; set; }
        [DataMember]
        public string Folder { get; set; }

        private Milliman.Reduction.SchedulerEngine.Engine Engine { get; set; }

    }
}
