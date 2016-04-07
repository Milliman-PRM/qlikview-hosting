using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportingCommon
{
    /// <summary>
    /// An abstract base class for logged events from any source.  
    /// Log events derived from this class are distinct from and inferred from log file entries.  
    /// </summary>
    public abstract class UserEventBase
    {
        public string User { get; set; }
        public string Group { get; set; }
        public string Browser { get; set; }
        public DateTime TimeStamp = DateTime.MinValue;
    }

    /// <summary>
    /// Events are sorted primarily by timestamp.  If multiple events occur with the same timestamp,
    /// the event types are the secondary sorting criterion.  
    /// </summary>
    public class UserEventComparer : Comparer<UserEventBase>
    {
        List<Type> AllQlikEventTypes = new List<Type> { typeof(QlikviewSessionEvent), typeof(QlikviewAuditEvent) };

        public override int Compare(UserEventBase x, UserEventBase y)
        {
            if (x.TimeStamp.CompareTo(y.TimeStamp) != 0)  // different timestamp
            {
                return x.TimeStamp.CompareTo(y.TimeStamp);
            }
            else // same timestamp
            {
                Type XType = x.GetType();
                Type YType = y.GetType();

                // equal types
                if (XType == YType) 
                    return 0;
                // both Qlikview types
                if (AllQlikEventTypes.Contains(XType) && AllQlikEventTypes.Contains(YType))
                {
                    // Qlik session event should precede other Qlik event types
                    if (XType == typeof(QlikviewSessionEvent) && YType != typeof(QlikviewSessionEvent))
                        return -1;
                    else if (XType != typeof(QlikviewSessionEvent) && YType == typeof(QlikviewSessionEvent))
                        return 1;
                    else
                        // neither is session event, preserve order
                        return 0;
                }
                // x=IIS and y=any Qlikview
                if (XType == typeof(IisAccessEvent) && AllQlikEventTypes.Contains(YType)) 
                    return -1;
                // x=any Qlikview and y=IIS
                if (AllQlikEventTypes.Contains(XType) && YType == typeof(IisAccessEvent)) 
                    return 1;

                // Should never get here.  If so, time to throw up:
                throw new ArgumentException("Unexpected arguments in UserEventComparer::Compare()");
            }
        }
    }
}
