using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_UserAdministrationLandingPage : System.Web.UI.Page
{
    // setting this as protected makes it available in markup
    protected string launchSystem
    {
        get
        {
            return (string)Request.QueryString["launchSystem"] ?? String.Empty;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(launchSystem))
        {
            switch (launchSystem)
            {
                case "usersAZ":
                    divusersAZ.Visible = true;
                    break;
                case "usersbyrole":
                    divusersbyrole.Visible = true;
                    break;
                case "emailBroadcast":
                    divemailBroadcast.Visible = true;
                    break;
                case "userslockedout":
                    divuserslockedout.Visible = true;
                    break;
                case "userOnline":
                    divuserOnline.Visible = true;
                    break;
                case "searchmembership":
                    divsearchmembership.Visible = true;
                    break;
                case "unapproved":
                    divunapproved.Visible = true;
                    break;
            }
        }
        else
        {
            Response.Redirect("default.aspx");
        }
        
    }
}