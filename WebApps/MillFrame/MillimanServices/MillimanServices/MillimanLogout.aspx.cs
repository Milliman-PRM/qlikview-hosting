using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using System.Xml;
using System;
using ComponentPro.Saml2;

namespace Milliman
{
    public partial class MillimanLogout : System.Web.UI.Page
    {

        //just calling this page will log the user out
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Logout locally.
                System.Web.Security.FormsAuthentication.SignOut();
                Session.Abandon();
            }

            catch (Exception exception)
            {
                Trace.Write("ServiceProvider", "Error on logout page", exception);
            }

            Response.Redirect("UserLogin.aspx");
        }
    }
}