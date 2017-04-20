/*
 * CODE OWNERS: Tom Puckett, 
 * OBJECTIVE: Dedicated class to process tracking of Qlikview CALs.
 * DEVELOPER NOTES: Note that this is derived from ServerMonitorProcessingBase, as all future processing classes should be
 */

using System;
using System.Diagnostics;
using System.Linq;
using PrmServerMonitor.Qms;
using PrmServerMonitor.ServiceSupport;

namespace PrmServerMonitor
{
    public class QlikviewCalManager : ServerMonitorProcessingBase
    {
        /// <summary>
        /// Used by a caller to identify a specific supported type of CAL statistic
        /// </summary>
        public enum CalStatisticField
        {
            NamedCalAssigned,
            NamedCalLimit,
            DocumentCalAssigned,
            DocumentCalLimit
        }

        /// <summary>
        /// Dumps a single caller selectable CAL related statistic value to trace log.  Intended mainly for use in non-interactive mode.  
        /// </summary>
        /// <param name="FieldToReport"></param>
        public void ReportCalStatistic(CalStatisticField FieldToReport)
        {
            EstablishTraceLog();

            CALConfiguration CalConfig = EnumerateAllCals(false);
            if (CalConfig == null)
            {
                Trace.WriteLine("QlikviewCalManager.EnumerateAllCals() returned null");
                return;
            }

            switch (FieldToReport)
            {
                case CalStatisticField.DocumentCalAssigned:
                    Trace.WriteLine(CalConfig.DocumentCALs.Assigned);
                    break;
                case CalStatisticField.DocumentCalLimit:
                    Trace.WriteLine(CalConfig.DocumentCALs.Limit);
                    break;
                case CalStatisticField.NamedCalAssigned:
                    Trace.WriteLine(CalConfig.NamedCALs.Assigned);
                    break;
                case CalStatisticField.NamedCalLimit:
                    Trace.WriteLine(CalConfig.NamedCALs.Limit);
                    break;
            }

            CloseTraceLog();
        }

        /// <summary>
        /// Obtains the CALConfiguration object representing the server configuration and optionally reports certain statistics to the trace log.  
        /// </summary>
        /// <param name="TraceOutput">true if trace log is to be written with CAL statistics</param>
        /// <returns>null if operation failed</returns>
        public CALConfiguration EnumerateAllCals(bool TraceOutput = false)
        {
            CALConfiguration CalConfig = null;

            try
            {
                string QMS = "http://localhost:4799/QMS/Service";

                QMSClient Client = new QMSClient("BasicHttpBinding_IQMS", QMS);
                string key = Client.GetTimeLimitedServiceKey();
                ServiceKeyClientMessageInspector.ServiceKey = key;
                ServiceInfo QvsService = Client.GetServices(ServiceTypes.QlikViewServer).First();

                CalConfig = Client.GetCALConfiguration(QvsService.ID, CALConfigurationScope.All);

                if (CalConfig != null && TraceOutput)
                {
                    EstablishTraceLog();

                    //Document CALs
                    Trace.WriteLine("\r\nDocument CALs");
                    Trace.WriteLine("# assigned document CALs:" + CalConfig.DocumentCALs.Assigned);
                    Trace.WriteLine("# in license document CALs:" + CalConfig.DocumentCALs.InLicense);
                    Trace.WriteLine("# limit document CALs:" + CalConfig.DocumentCALs.Limit);

                    //Named CALs
                    Trace.WriteLine("\r\nNamed CALs");
                    Trace.WriteLine("Identification mode:" + CalConfig.NamedCALs.IdentificationMode);
                    Trace.WriteLine("# assigned named CALs:" + CalConfig.NamedCALs.Assigned);
                    Trace.WriteLine("# in license named CALs:" + CalConfig.NamedCALs.InLicense);
                    Trace.WriteLine("# limit document CALs:" + CalConfig.NamedCALs.Limit);
                    foreach (var assignedCal in CalConfig.NamedCALs.AssignedCALs)
                    {
                        Trace.WriteLine(String.Format("User:{0}\r\nNamed CAL last used:{1}", assignedCal.UserName, assignedCal.LastUsed));
                    }

                    foreach (var leasedCal in CalConfig.NamedCALs.LeasedCALs)
                    {
                        Trace.WriteLine(String.Format("User:{0}\r\nLeased CAL last used:{1}", leasedCal.UserName, leasedCal.LastUsed));
                    }

                    //Session CALs
                    Trace.WriteLine("\r\nSession CALs");
                    Trace.WriteLine("# available session CALs:" + CalConfig.SessionCALs.Available);
                    Trace.WriteLine("# in license session CALs:" + CalConfig.SessionCALs.InLicense);
                    Trace.WriteLine("# limit session CALs:" + CalConfig.SessionCALs.Limit);

                    CloseTraceLog();
                }
            }
            catch  /*(anything)*/
            { /*Do nothing*/}

            return CalConfig;
        }
    }
}
