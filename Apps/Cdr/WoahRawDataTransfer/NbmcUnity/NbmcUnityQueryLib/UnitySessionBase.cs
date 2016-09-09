using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NbmcUnityQueryLib.AllscriptsUnity;

namespace NbmcUnityQueryLib
{
    public class UnitySessionBase
    {
        // Get Unity security token 
        protected UnityServiceClient UnityClient;
        private String _UnitySecurityToken;
        //private Timer SecurityTokenTimer;

        protected String UnitySecurityToken
        {
            private set { _UnitySecurityToken = value; }
            get { return _UnitySecurityToken; }
        }

        public UnitySessionBase()
        {
            //TimeSpan FifteenMinutes = new TimeSpan(0, 15, 0);
            //SecurityTokenTimer = new Timer(RefreshSecurityToken, null, FifteenMinutes, FifteenMinutes);
        }

        ~UnitySessionBase()
        {
            Disconnect();
        }

        public bool Connect(String UnityUserName, String UnityPassword, String UnityEndpoint)
        {
            // Unity client from service reference 
            if (UnityClient == null || UnityClient.State != System.ServiceModel.CommunicationState.Opened)
            {
                UnityClient = new UnityServiceClient("basichttp", UnityEndpoint);

                UnitySecurityToken = UnityClient.GetSecurityToken(UnityUserName, UnityPassword);
            }

            return true;
        }

        public void Disconnect()
        {
            if (UnityClient != null)
            {
                UnityClient.Close();
                UnityClient = null;
            }
        }

        public void RefreshSecurityToken(Object Arg)
        {
            // not sure how to refresh the token using the c# client component, or if it's needed at all
        }

    }
}
