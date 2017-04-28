/*
 * CODE OWNERS: Tom Puckett, 
 * OBJECTIVE: <What and WHY.>
 * DEVELOPER NOTES: <What future developers need to know.>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrmServerMonitorLib.Qms;
using PrmServerMonitorLib.ServiceSupport;

namespace PrmServerMonitorLib
{
    public class QlikviewProcessingBase : MonitorProcessingBase
    {
        protected string ServerName = string.Empty;
        protected QMSClient Client = null;

        protected QlikviewProcessingBase(string ServerNameArg, bool LifetimeTraceArg) : base(LifetimeTraceArg)
        {
            ServerName = @"http://" + ServerNameArg + @":4799/QMS/Service";
        }

        /// <summary>
        /// Connects the web service client instance to the service
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        /// <returns></returns>
        protected bool ConnectClient(string endpointConfigurationName, string remoteAddress)
        {
            try
            {
                if (Client == null)
                {
                    Client = new QMSClient(endpointConfigurationName, remoteAddress);
                    string key = Client.GetTimeLimitedServiceKey();
                    ServiceKeyClientMessageInspector.ServiceKey = key;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Disconnects the client from the service and destroys the instance if successful
        /// </summary>
        /// <returns></returns>
        protected bool DisconnectClient()
        {
            try
            {
                if (Client != null && 
                    new System.ServiceModel.CommunicationState[] 
                    {
                        System.ServiceModel.CommunicationState.Opened,
                          System.ServiceModel.CommunicationState.Opening
                    }.Contains(Client.State))
                {
                    Client.Close();
                    Client = null;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

    }
}
