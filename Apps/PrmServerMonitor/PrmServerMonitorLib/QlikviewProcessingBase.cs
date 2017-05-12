/*
 * CODE OWNERS: Tom Puckett, 
 * OBJECTIVE: An inheritable base class intended to be common to any class that acts as a Qlikview Management Service API client
 * DEVELOPER NOTES: All QMS client classes should inherit from this class
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

        /// <summary>
        /// Constructor that accepts arguments that guide mandatory behavior choices
        /// </summary>
        /// <param name="ServerNameArg"></param>
        /// <param name="LifetimeTraceArg"></param>
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
