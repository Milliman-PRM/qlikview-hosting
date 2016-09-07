using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientPublisher
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
                case "xls": return "application/vnd.ms-excel";
                case "xlsm": return "application/vnd.ms-excel.sheet.macroenabled.12";
                case "xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case "ppt": return "application/vnd.ms-powerpoint";

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
                if (context.Request["nofile"] == null) // if we get a nofile, it's the users manual so dont add th download file
                    context.Response.AddHeader("Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(physicalFileName));
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