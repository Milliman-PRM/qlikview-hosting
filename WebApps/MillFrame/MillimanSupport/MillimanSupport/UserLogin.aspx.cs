using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using System;
using System.Web;
using System.Web.Security;


namespace MillimanSupport
{
    public partial class UserLogin : System.Web.UI.Page
    {
       

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


        //local authentication
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (FormsAuthentication.Authenticate(txtUserName.Text, txtPassword.Text))
            {
                FormsAuthentication.RedirectFromLoginPage(txtUserName.Text, false);
                this.Session["milliman"] = "Yes, I am!";
                Session["patientid"] = "";
                //Response.Redirect("default.aspx");
            }
            //else if (Membership.ValidateUser(txtUserName.Text, txtPassword.Text))
            //{
            //    this.Session["milliman"] = "Yes, I am!";
            //    Session["patientid"] = "";
            //    MembershipUserCollection MUC = Membership.FindUsersByName(txtUserName.Text);
            //    MembershipUser MU = MUC[txtUserName.Text];
            //    FormsAuthentication.SetAuthCookie(MU.UserName, false);
            //    Response.Redirect("default.aspx");
            //}
            else
            {
                lblErrorMessage.Text = "A valid user name and password is required for access!";
            }
        }
    }
}