using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace MillimanProjectManConsole
{
    public class Global : System.Web.HttpApplication
    {
        //the communciations pipe to external server
        static private MPMCServices.MPMCServicesSoapClient RemoteServer = null;

        static public MPMCServices.MPMCServicesSoapClient GetInstance()
        {
            if (RemoteServer == null)
            {
                RemoteServer = new MPMCServices.MPMCServicesSoapClient();
            }
            return RemoteServer;
        }

        //task manager for publishing files
        static private MillimanProjectManConsole.ProductionPublisher _Publisher = null;

        public static MillimanProjectManConsole.ProductionPublisher Publisher
        {
            get {
                if (_Publisher == null)
                {
                    _Publisher = new ProductionPublisher( Global.GetInstance() );
                }
                  return Global._Publisher; }
            set { Global._Publisher = value; }
        }

        static private MillimanProjectManConsole.ComplexUpload.TaskManager _TaskManager = null;
        public static MillimanProjectManConsole.ComplexUpload.TaskManager TaskManager
        {
            get
            {
                if (_TaskManager == null)
                {
                    _TaskManager = new MillimanProjectManConsole.ComplexUpload.TaskManager();
                }
                return _TaskManager;
            }
            set { _TaskManager = value; }
        }

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        static public bool ServerLocalProjectsSame(MillimanCommon.ProjectSettings PS)
        {
            string ServerHash = GetInstance().GetHash(System.IO.Path.Combine( PS.VirtualDirectory, PS.ProjectName + ".hciprj"));
            string LocalHash = MillimanCommon.Utilities.CalculateMD5Hash(PS.LoadedFrom, true);
            return string.Compare(ServerHash, LocalHash, true ) == 0;
        }
    }
}