using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.Admin
{
    public partial class QVWReloadError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string Msg = string.Empty;
                string Situation = MillimanCommon.Utilities.ConvertHexToString( Request["key"] );
                string[] Groups = Situation.Split(new char[] { '~' });
                if (Groups.Count() == 2)
                {
                    if (string.IsNullOrEmpty(Groups[1]) == true)
                    {
                        Msg = "<center>The QVW has not been signed.<br><br>The ability to update with unsigned QVWs has been disabled.</center>";
                    }
                    else
                    {
                        Msg = "The new QVW has been signed for association to the group<br><b> '" + Groups[1] + "'</b><br>However you are requesting it be placed in the group<br><b> '" + Groups[0] + "'</b><br><br>Moving QVWs across groups is not allowed.</b>";
                    }
                }
                else if (Groups.Count() == 1)
                {
                    Msg = "<center>The QVW has not been signed.<br><br>The ability to update with unsigned QVWs has been disabled.</center>";
                }

                errormsg.Text = Msg;
            }
        }
    }
}