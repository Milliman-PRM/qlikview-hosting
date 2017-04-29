/*
 * CODE OWNERS: Tom Puckett, 
 * OBJECTIVE: A data type that can be used to represent an instance of a Qlikview Document CAL
 * DEVELOPER NOTES: 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrmServerMonitorLib
{
    public struct DocCalEntry
    {
        public string DocumentName;
        public string RelativePath;
        public string UserName;
        public DateTime LastUsedDateTime;
        public bool DeleteFlag;
    }

}
