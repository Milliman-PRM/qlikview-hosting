using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milliman
{
    /// <summary>
    /// Summary description for Reflector
    /// </summary>
    public class Reflector : IHttpHandler
    {

        private string GetDocType(string ImagePathName)
        {
            string ext = System.IO.Path.GetExtension(ImagePathName).ToLower();
            if (ext.StartsWith("."))
                ext = ext.Substring(1);  //get rid of period if there
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
                case "png": return "image/png";
                case "gif": return "image/gif";
                case "jpg":
                case "jpeg": return "image/jpeg";
                case "mpg":
                case "mpeg": return "video/mpeg";
                case "mp4": return "video/mp4";
                default: return "application/unknown";
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            string HexString = context.Request["key"];
            string TextString = MillimanCommon.Utilities.ConvertHexToString(HexString);
            context.Response.StatusCode = 200;
            context.Response.ContentType = GetDocType(TextString);
            if (System.IO.File.Exists(TextString))
            {
                context.Response.ContentType = GetDocType(TextString);
                string physicalFileName = TextString;
                context.Response.WriteFile(physicalFileName);
            }
            else
            {
                context.Response.ContentType = "image/png";
                string physicalFileName = "images/missing.png";
                context.Response.WriteFile(physicalFileName);
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}