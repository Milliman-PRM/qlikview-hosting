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
            Response.Redirect("UserLogin.aspx");
        }
    }
}