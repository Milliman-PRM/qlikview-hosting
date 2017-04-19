using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrmServerMonitor.Qms;
using PrmServerMonitor.ServiceSupport;

namespace PrmServerMonitor
{
    public class QlikviewCalManager
    {
        TextWriterTraceListener TraceFile;

        public void EstablishTraceLog()
        {
            TraceFile = new TextWriterTraceListener("Trace_QlikviewCalManager_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".log");
            Trace.AutoFlush = true;
            Trace.Listeners.Add(TraceFile);
        }

        public void CloseTraceLog()
        {
            TraceFile.Flush();
            Trace.Listeners.Remove(TraceFile);
            TraceFile.Close();
            TraceFile = null;
        }

        public bool EnumerateAllCals()
        {
            QMSClient Client;
            bool ReturnValue;

            EstablishTraceLog();

            try
            {
                string QMS = "http://localhost:4799/QMS/Service";
                Client = new QMSClient("BasicHttpBinding_IQMS", QMS);
                string key = Client.GetTimeLimitedServiceKey();
                ServiceKeyClientMessageInspector.ServiceKey = key;
                ServiceInfo QvsService = Client.GetServices(ServiceTypes.QlikViewServer).First();

                CALConfiguration config = Client.GetCALConfiguration(QvsService.ID, CALConfigurationScope.All);

                if (config != null)
                {
                    //Document CALs
                    Trace.WriteLine("\r\nDocument CALs");
                    Trace.WriteLine("# assigned document CALs:" + config.DocumentCALs.Assigned);
                    Trace.WriteLine("# in license document CALs:" + config.DocumentCALs.InLicense);
                    Trace.WriteLine("# limit document CALs:" + config.DocumentCALs.Limit);

                    //Named CALs
                    Trace.WriteLine("\r\nNamed CALs");
                    Trace.WriteLine("Identification mode:" + config.NamedCALs.IdentificationMode);
                    Trace.WriteLine("# assigned named CALs:" + config.NamedCALs.Assigned);
                    Trace.WriteLine("# in license named CALs:" + config.NamedCALs.InLicense);
                    Trace.WriteLine("# limit document CALs:" + config.NamedCALs.Limit);
                    foreach (var assignedCal in config.NamedCALs.AssignedCALs)
                    {
                        Trace.WriteLine(String.Format("User:{0}\r\nNamed CAL last used:{1}", assignedCal.UserName, assignedCal.LastUsed));
                    }

                    foreach (var leasedCal in config.NamedCALs.LeasedCALs)
                    {
                        Trace.WriteLine(String.Format("User:{0}\r\nLeased CAL last used:{1}", leasedCal.UserName, leasedCal.LastUsed));
                    }

                    //Session CALs
                    Trace.WriteLine("\r\nSession CALs");
                    Trace.WriteLine("# available session CALs:" + config.SessionCALs.Available);
                    Trace.WriteLine("# in license session CALs:" + config.SessionCALs.InLicense);
                    Trace.WriteLine("# limit session CALs:" + config.SessionCALs.Limit);

                    ReturnValue = true;
                }
                else
                {
                    ReturnValue = false;
                }
            }
            catch
            {
                ReturnValue = false;
            }
            finally
            {
                CloseTraceLog();
            }

            return ReturnValue;
        }
    }
}
