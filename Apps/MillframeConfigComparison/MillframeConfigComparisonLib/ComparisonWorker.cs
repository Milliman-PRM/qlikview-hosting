using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillframeConfigComparisonLib
{
    public class ComparisonWorker
    {
        public ComparisonResult Compare(string Path1, string Path2, List<string> Required1=null, List<string> Required2 = null)
        {
            ComparisonResult Result = new ComparisonResult(Path1, Path2, Required1, Required2);

            return Result;
        }
    }
}
