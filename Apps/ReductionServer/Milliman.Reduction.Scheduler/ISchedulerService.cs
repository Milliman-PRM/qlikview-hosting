using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Milliman.Reduction.SchedulerEngine {

    [ServiceContract]
    public interface ISchedulerService {

        [OperationContract]
        void AddContinuous();
        [OperationContract]
        void AddInterval(TimeSpan timespan);
        [OperationContract]
        void AddOneTime(DateTime datetime);
        [OperationContract]
        void AddWeekly(DateTime datetime, EnumWeekDays weekdays);
        [OperationContract]
        void AddMonthly(DateTime datetime, EnumMonths months, EnumMonthDays monthdays);
        [OperationContract]
        void AddSchedule(Schedule schedule);
    }
}
