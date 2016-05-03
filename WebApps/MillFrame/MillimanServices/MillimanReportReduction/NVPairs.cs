using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanReportReduction
{
    public class NVPairs
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsNumeric { get; set; }

        public NVPairs()
        {
            IsNumeric = false;
        }

        public NVPairs(string _Name, string _Value, bool _IsNumeric = false)
        {
            Name = _Name;
            Value = _Value;
            IsNumeric = _IsNumeric;
        }
    }
}
