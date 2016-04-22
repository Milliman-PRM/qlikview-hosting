using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;

namespace MillimanDev2
{
    public partial class ImageReflector : System.Web.UI.Page
    {
        private string GetImageType(string ImagePathName)
        {
            string ext = System.IO.Path.GetExtension(ImagePathName).ToLower();
            switch (ext)
            {
                case "jpg":
                case "jpeg": return "image/jpeg";
                case "gif": return "image/gif";
                case "png": return "image/png";
                default: return "application/unknown";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string HexString = Request["key"];
            string TextString = MillimanCommon.Utilities.ConvertHexToString(HexString);
            if (TextString.IndexOf(":") == -1)
            {
                string QVDocumentRoot = WebConfigurationManager.AppSettings["QVDocumentRoot"];
                TextString = System.IO.Path.Combine(QVDocumentRoot, TextString);
            } 
            if (System.IO.File.Exists(TextString))
            {
                Response.ContentType = GetImageType(TextString);
                string physicalFileName = TextString;
                Response.WriteFile(physicalFileName);
            }
            else
            {
                Response.ContentType = "image/png";
                string physicalFileName = "images/missing.png";
                Response.WriteFile(physicalFileName);
            }
        }
    }
}