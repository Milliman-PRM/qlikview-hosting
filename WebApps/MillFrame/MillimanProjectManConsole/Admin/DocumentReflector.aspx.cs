using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanDev2
{
    public partial class DocumentReflector : System.Web.UI.Page
    {
        private string GetDocType(string ImagePathName)
        {
            string ext = System.IO.Path.GetExtension(ImagePathName).ToLower();
            switch (ext)
            {
                case "htm":
                case "html": return "text/html";
                case "doc": return "application/msword";
                case "docx": return "application/msword";
                case "pdf": return "application/pdf";
                case "txt":
                case "text": return "text/plain";
                case "rtf": return "application/rtf";
                case "zip": return "application/zip";
                default: return "application/unknown";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string HexString = Request["key"];
            string TextString = MillimanCommon.Utilities.ConvertHexToString(HexString);
            if (System.IO.File.Exists(TextString))
            {
                Response.ContentType = GetDocType(TextString);
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