using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using System;
using System.Web;
using System.Web.Security;
using ComponentPro.Saml;
using ComponentPro.Saml.Binding;
using ComponentPro.Saml2;
using ComponentPro.Saml2.Binding;

namespace Milliman
{
    public partial class UserLogin : System.Web.UI.Page
    {
        /// <summary>
        /// Builds an authentication request.
        /// </summary>
        /// <returns>The authentication request.</returns>

        protected override void OnLoad(System.EventArgs e)
        {           

            base.OnLoad(e);

            txtUserName.Focus();
         
            string error = Request.QueryString[Util.ErrorVarName];
            if (error == null)
                error = string.Empty;

            // Display any error message resulting from a failed login if any.
            lblErrorMessage.Text = error;
        }


        /// <summary>
        /// Handles the IdpLogin button to requests login at the Identify Provider site.
        /// </summary>
        /// <param name="sender">The button object.</param>
        /// <param name="e">The event arguments.</param>
 
        //local authentication
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (FormsAuthentication.Authenticate(txtUserName.Text, txtPassword.Text))
            {
                FormsAuthentication.RedirectFromLoginPage(txtUserName.Text, false);
                this.Session["milliman"] = "Yes, I am!";
                Session["patientid"] = "";
            }
            else if (Membership.ValidateUser(txtUserName.Text, txtPassword.Text))
            {
                this.Session["milliman"] = "Yes, I am!";
                Session["patientid"] = "";
                MembershipUserCollection MUC = Membership.FindUsersByName(txtUserName.Text);
                MembershipUser MU = MUC[txtUserName.Text];
                FormsAuthentication.SetAuthCookie(MU.UserName, false);

                Response.Redirect("default.aspx");
            }
            else
            {
                lblErrorMessage.Text = "A valid user name and password is required for access!";
            }

        }
    }
}