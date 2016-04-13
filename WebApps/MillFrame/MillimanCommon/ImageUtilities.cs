using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanCommon
{
    public class ImageUtilities
    {
 

        static public string GetIcon(string QualifiedItem, out string MimeType)
        {
            MimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(QualifiedItem).ToLower();
            if (ext.StartsWith("."))
                ext = ext.Substring(1);  //get rid of period if there
            switch (ext)
            {
                case "htm":
                case "html": MimeType = "text/html";
                            return "images/placeholder.gif";
                case "doc": MimeType = "application/msword";
                            return "images/word-icon.png";
                case "docx": MimeType = "application/msword";
                            return "images/word-icon.png";
                case "pdf": MimeType = "application/pdf";
                            return "images/placeholder.gif";
                case "txt":
                case "text": MimeType = "text/plain";
                            return "images/placeholder.gif";
                case "rtf": MimeType = "application/rtf";
                            return "images/placeholder.gif";
                case "zip": MimeType = "application/zip";
                            return "images/zip-icon.png";
                case "png": MimeType = "image/png";
                            return "images/image-icon.png";
                case "gif": MimeType = "image/gif";
                            return "images/image-icon.png";
                case "jpg":
                case "jpeg": MimeType = "image/jpeg";
                            return "images/image-icon.png";
                case "mpg":
                case "mpeg": MimeType = "video/mpeg";
                            return "images/placeholder.gif";
                case "mp4": MimeType = "video/mp4";
                            return "images/placeholder.gif";
                case "xls": MimeType = "application/vnd.ms-excel";
                            return "images/excel-icon.png";
                case "xlsm": MimeType = "application/vnd.ms-excel.sheet.macroenabled.12";
                            return "images/excel-icon.png";
                case "xlsx": MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            return "images/excel-icon.png";
                case "pptx": MimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                            return "images/powerpoint-icon.png";
                case "ppt": MimeType = "application/vnd.ms-powerpoint";
                            return "images/powerpoint-icon.png";
                case "xml": MimeType = "application/vnd.ms-powerpoint";
                            return "images/xml-icon.png";

                default: return "images/placeholder.gif";
            }
        }
    }
}
