using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClientPublisher.HTML
{
    public partial class GeneralIssue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ( Request["msg"] != null )
            {
                ErrorMsg.Text = MillimanCommon.Utilities.ConvertHexToString(Request["msg"]);
            }
        }
    }
}