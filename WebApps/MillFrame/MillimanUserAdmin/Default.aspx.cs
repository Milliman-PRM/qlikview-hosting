using System;
using System.Web.Security;
public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //this is needed to make IIS work when doing a redirect

        if (Membership.GetUser() != null)
            Response.Redirect("admin/default.aspx");
        else
            Response.Redirect("login.aspx");
    }
}
