using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanSupport
{
    public partial class UserStatus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if ( ( Session["CUIDS"] != null ) && (Session["CUIDS"].ToString() != string.Empty) )
                {
                    string[] IDs = Session["CUIDS"].ToString().Split(new char[] { ',' });
                    foreach (string ID in IDs)
                        CUIDS.Items.Add(ID);
                }

               
            }
        }
    }
}