using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using System.Xml;
using System;
using System.Text;
using ComponentPro.Saml2;
using System.Web;
using System.Web.Security;
using System.Collections;

namespace Milliman
{
    public partial class _Default : System.Web.UI.Page
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
    
            }
 
        }

        protected void Clear_Click(object sender, EventArgs e)
        {
            Global.ClearActions();
        }
       
    }
}