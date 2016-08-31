using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using System;
using System.Web;
using System.Web.Security;
using ComponentPro.Saml;
using ComponentPro.Saml.Binding;
using ComponentPro.Saml2;
using ComponentPro.Saml2.Binding;

namespace MillimanDev
{
    public partial class UserLogin : System.Web.UI.Page
    {
        /// <summary>
        /// Builds an authentication request.
        /// </summary>
        /// <returns>The authentication request.</returns>
        private AuthnRequest BuildAuthenticationRequest()
        {
            // Create some URLs to identify the service provider to the identity provider.
            // As we're using the same endpoint for the different bindings, add a query string parameter
            // to identify the binding.
            string issuerUrl = Util.GetAbsoluteUrl(this, "~/");
            string assertionConsumerServiceUrl = string.Format("{0}?{1}={2}", Util.GetAbsoluteUrl(this, "~/AssertionService.aspx"), Util.BindingVarName, HttpUtility.UrlEncode(idpToSPBindingList.SelectedValue));

            // Create the authentication request.
            AuthnRequest authnRequest = new AuthnRequest();
            authnRequest.Destination = SSOConfiguration.IdPSingleSignonIdProviderUrl;
            authnRequest.Issuer = new Issuer(issuerUrl);
            authnRequest.ForceAuthn = false;
            authnRequest.NameIdPolicy = new NameIdPolicy(null, null, true);
            authnRequest.ProtocolBinding = idpToSPBindingList.SelectedValue;
            authnRequest.AssertionConsumerServiceUrl = assertionConsumerServiceUrl;

            // Don't sign if using HTTP redirect as the generated query string is too long for most browsers.
            if (spToIdPBindingList.SelectedValue != SamlBindingUri.HttpRedirect)
            {
                // Sign the authentication request.
                X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.SPCertKey];

               // authnRequest.Sign(x509Certificate);
            }
            return authnRequest;

        }

        protected override void OnLoad(System.EventArgs e)
        {           

            base.OnLoad(e);

            txtUserName.Focus();
         
            string error = Request.QueryString[Util.ErrorVarName];
            if (error == null)
                error = string.Empty;

            // Display any error message resulting from a failed login if any.
            lblErrorMessage.Text = error;

            //inject the script we are redirecto to identity provider
            if (Request.RawUrl.ToLower().IndexOf("sso_redirect") != -1)
            {
                string Script = "\nvar SelectedItem = document.getElementById('btnIdPLogin');";
                Script += "\nvar FormContents = document.getElementById('myform');";
                Script += "\nif (SelectedItem) {";
                Script += "\nFormContents.style.display = 'none';";
                Script += "\nSelectedItem.click();";
                //Script += "\nsetTimeout(function(){alert('Redirect to IDP'); SelectedItem.click(); },1000)";
                Script += "\n}";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RedirectScript", Script, true);
            }
            else if (Request.RawUrl.ToLower().IndexOf("secureid=") != -1)
            {
                string AdminName = string.Empty;
                string UserName = string.Empty;
                string UserID = string.Empty;
                DateTime CertCreated = DateTime.MinValue;
                if (MillimanCommon.SecureLink.IsSecureLinkValid(Request["SecureID"], out AdminName, out UserName, out UserID, out CertCreated) == true)
                {
                    MembershipUserCollection Users = Membership.FindUsersByName(UserName);
                    if ((Users == null) || (Users.Count == 0) || (Users.Count > 1))
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Secure link cert recieved but no users of that name appear in membership - " + UserName);
                        Response.Redirect("SecureLinkIssue.html");
                        return;
                    }
                    MembershipUser User = Users[UserName];
                    //User should never be null if we got this far
                    if (string.Compare(User.ProviderUserKey.ToString(), UserID, true) != 0)
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Secure link cert recieved but user id did not match with the name that appears in membership - " + UserName + ":" + UserID);
                        Response.Redirect("SecureLinkIssue.html");
                        return;

                    }
                    if (Membership.ValidateUser(UserName, User.GetPassword()))
                    {
                        this.Session["milliman"] = "Yes, I am!";
                        Session["patientid"] = "";
                        MembershipUserCollection MUC = Membership.FindUsersByName(UserName);
                        MembershipUser MU = MUC[UserName];

                        //there should be a reset file too, if not they are trying to use smart link more than once
                        string ResetFile = System.IO.Path.Combine(WebConfigurationManager.AppSettings["ResetUserInfoRoot"], UserID + ".rst");
                        if (System.IO.File.Exists(ResetFile))
                        {
                            FormsAuthentication.SetAuthCookie(MU.UserName, false);
                            Response.Redirect("default.aspx");
                        }
                        else
                            Response.Redirect("SecureLinkUsed.html");
                    }
                }
                else
                {  //secure link is not valid or has expired
                    Response.Redirect("SecureLinkIssue.html");
                }
            }
            else if (Membership.GetUser() != null)
            {
                Response.Redirect("default.aspx");  //already logged in
            }
        }


        /// <summary>
        /// Handles the IdpLogin button to requests login at the Identify Provider site.
        /// </summary>
        /// <param name="sender">The button object.</param>
        /// <param name="e">The event arguments.</param>
        protected void btnIdPLogin_Click(object sender, EventArgs e)
        {
            // Create the authentication request.
            AuthnRequest authnRequest = BuildAuthenticationRequest();

            // Create and cache the relay state so we remember which SP resource the user wishes 
            // to access after SSO.
            //string spResourceUrl = Util.GetAbsoluteUrl(this, FormsAuthentication.GetRedirectUrl("", false));
            string PatientID = Global.GetParameter(Request.RawUrl, @"patientid");
            string PatientQuery = string.IsNullOrEmpty(PatientID) ? string.Empty : @"&patientid=" + PatientID;
            string spResourceUrl = Util.GetAbsoluteUrl(this, "dashboard.aspx?dashboardid=" + Global.GetParameter(Request.RawUrl, @"dashboardid") + PatientQuery);
            string relayState = Guid.NewGuid().ToString();
            SamlSettings.CacheProvider.Insert(relayState, spResourceUrl, new TimeSpan(1, 0, 0));

            // Send the authentication request to the identity provider over the selected binding.
            string idpUrl = string.Format("{0}?{1}={2}", SSOConfiguration.IdPSingleSignonIdProviderUrl, Util.BindingVarName, HttpUtility.UrlEncode(spToIdPBindingList.SelectedValue));
            //authnRequest.AssertionConsumerServiceUrl = "http://hcintel.cloudapp.net/milliman/assertionservice.aspx";
            //authnRequest.Id = "MillimanHCIntel_Version1";
            //authnRequest.ProviderName = "MillimanHCIntel_Version1";
            switch (spToIdPBindingList.SelectedValue)
            {
                case SamlBindingUri.HttpRedirect:
                    X509Certificate2 x509Certificate = (X509Certificate2)Application[Global.SPCertKey];


                    authnRequest.Redirect(Response, idpUrl, relayState, x509Certificate.PrivateKey);
   
                    break;

                case SamlBindingUri.HttpPost:
                   
                    authnRequest.SendHttpPost(Response, idpUrl, relayState);
                  
                    // Don't send this form.
                    Response.End();
                    break;

                case SamlBindingUri.HttpArtifact:
                    // Create the artifact.
                    string identificationUrl = Util.GetAbsoluteUrl(this, "~/");
                    Saml2ArtifactType0004 httpArtifact = new Saml2ArtifactType0004(SamlArtifact.GetSourceId(identificationUrl), SamlArtifact.GetHandle());

                    // Cache the authentication request for subsequent sending using the artifact resolution protocol.
                    SamlSettings.CacheProvider.Insert(httpArtifact.ToString(), authnRequest.GetXml(), new TimeSpan(1, 0, 0));

                    // Send the artifact.
                    httpArtifact.Redirect(Response, idpUrl, relayState);
                    break;
            }
        }

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