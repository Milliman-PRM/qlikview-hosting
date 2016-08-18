using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Milliman.Reduction.SchedulerEngine {

    public class SchedulerService: ISchedulerService {
        private Dictionary<string, Schedule> _watchers = new Dictionary<string, Schedule>();

        public void AddContinuous() {
            throw new NotImplementedException();
        }

        public void AddInterval(TimeSpan timespan) {
            throw new NotImplementedException();
        }

        public void AddMonthly(DateTime datetime, EnumMonths months, EnumMonthDays monthdays) {
            throw new NotImplementedException();
        }

        public void AddOneTime(DateTime datetime) {
            throw new NotImplementedException();
        }

        public void AddSchedule(Schedule schedule) {
            throw new NotImplementedException();
        }

        public void AddWeekly(DateTime datetime, EnumWeekDays weekdays) {
            throw new NotImplementedException();
        }

        private void AddScheduler(Milliman.Reduction.SchedulerEngine.Schedule schedule) {
            switch( schedule.ScheduleType ) {
                case EnumScheduleType.Continuous:
                case EnumScheduleType.Interval:
                case EnumScheduleType.OneTime:
                case EnumScheduleType.Weekly:
                case EnumScheduleType.Monthly:
                    break;
            }

        }
    }
}
