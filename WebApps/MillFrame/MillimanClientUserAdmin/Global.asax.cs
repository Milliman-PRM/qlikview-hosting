using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace MillimanClientUserAdmin
{
    public class Global : System.Web.HttpApplication
    {

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
            Response.Redirect("UnspecifiedError.html");
        }

        protected void Session_End(object sender, EventArgs e)
        {
            //when the session expires, delete the cached SSO token
            if (Session["SSOToken"] != null)
            {
                string CachedSSOToken = Session["SSOToken"].ToString();
                if (System.IO.File.Exists(CachedSSOToken))
                    System.IO.File.Delete(CachedSSOToken);
            }
            Session.Clear();
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}