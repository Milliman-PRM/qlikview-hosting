using System;

public partial class verification : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //add meta tag for refresh, make it refresh 1 minute beyond the timeout of the session per webconfig session timeout key
        Response.AppendHeader("Refresh", Convert.ToString((Session.Timeout * 60) + 60));
    }
}
