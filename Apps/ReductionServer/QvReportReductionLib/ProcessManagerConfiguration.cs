using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QvReportReductionLib
{
    public class ProcessManagerConfiguration
    {
        public string RootPath { get; set; }
        public int MaxConcurrentTasks { get; set; }
    }
}
