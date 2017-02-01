using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillframeConfigComparisonLib
{
    public class ComparisonWorker
    {
        public ComparisonResult Compare(string Path1, string Path2)
        {
            ComparisonResult Result = new ComparisonResult(Path1, Path2);

            return Result;
        }
    }
}
