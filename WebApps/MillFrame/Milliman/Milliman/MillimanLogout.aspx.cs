using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using System.Xml;
using System;
using ComponentPro.Saml2;

namespace MillimanDev
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

                if (this.Session["milliman"] == null)
                {
                    // Create a logout request.
                    LogoutRequest logoutRequest = new LogoutRequest();
                    logoutRequest.Issuer = new Issuer(Util.GetAbsoluteUrl(this, "~/"));
                    logoutRequest.NameId = new NameId(Context.User.Identity.Name);

                    // Send the logout request to the IdP over HTTP redirect.
                    string logoutUrl = SSOConfiguration.IdPLogoutIdProviderUrl;
                    X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.SPCertKey];
                    logoutRequest.Redirect(Response, logoutUrl, string.Empty, x509Certificate.PrivateKey, "Sha1");

                }
            

            }

            catch (Exception exception)
            {
                Trace.Write("ServiceProvider", "Error on logout page", exception);
            }

            Response.Redirect("UserLogin.aspx");
        }
    }
}