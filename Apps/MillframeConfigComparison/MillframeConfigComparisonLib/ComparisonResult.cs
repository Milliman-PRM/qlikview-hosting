using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillframeConfigComparisonLib
{
    public class ComparisonResult
    {
        OneConfiguration Config1;
        OneConfiguration Config2;

        public ComparisonResult(string Path1, string Path2, bool DoWebConfig, bool DoAppConfig)
        {
            Config1 = new OneConfiguration(Path1, DoWebConfig, DoAppConfig);
            Config2 = new OneConfiguration(Path2, DoWebConfig, DoAppConfig);
        }
    }
}
