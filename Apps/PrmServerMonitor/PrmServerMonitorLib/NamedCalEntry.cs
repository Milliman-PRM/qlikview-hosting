/*
 * CODE OWNERS: Tom Puckett, 
 * OBJECTIVE: A data type that can represent an instance of a Qlikview Named User CAL
 * DEVELOPER NOTES: 
 */


using System;

namespace PrmServerMonitorLib
{
    public struct NamedCalEntry
    {
        public string UserName;
        public DateTime LastUsedDateTime;
        public bool DeleteFlag;
    }
}
