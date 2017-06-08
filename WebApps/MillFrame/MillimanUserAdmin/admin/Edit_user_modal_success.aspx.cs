using System;

public partial class admin_Edit_user_modal_success : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Ensure user is currently authenticated to the application. If not, redirect back to login page.
        if (!Page.Request.IsAuthenticated)
        {
            Response.Redirect(MapPath("~/login.aspx"));
        }
    }
}