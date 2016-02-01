using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManagementConsoleServices
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    Status.Text = DateTime.Now.ToString();
                    MillimanProjectManagementConsoleServices.MPMCServices MP = new MPMCServices();
                    Status.Text = MP.ValidUser("van.nanney@milliman.com", "vnanney@@1").ToString();
                    List<string> Gr = MP.GetGroups();
                }
                catch (Exception ex)
                {
                    Status.Text = ex.ToString();
                }

            }
        }
    }
}