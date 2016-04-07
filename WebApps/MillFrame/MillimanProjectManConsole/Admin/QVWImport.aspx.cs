using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.Admin
{
    public partial class QVWImport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (ImportQVW.HasFile)
                {
                    string SaveTo = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetTempFileName());
                    ImportQVW.SaveAs(SaveTo);

                    Response.Redirect("QVWImportVerify.aspx?key=" + MillimanCommon.Utilities.ConvertStringToHex(SaveTo));
                }
            }
            catch (Exception ex)
            {
                MillimanCommon.Alert.Show("Failed to upload file - " + ex.ToString());
            }
        }
    }
}