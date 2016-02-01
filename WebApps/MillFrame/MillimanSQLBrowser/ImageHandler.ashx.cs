using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace WebSqlUtility
{
    /// <summary>
    /// Summary description for ImageHandler
    /// </summary>
    public class ImageHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string _Path;
            string DBCacheDir = System.Configuration.ConfigurationSettings.AppSettings["HCIntelDBBrowserCache"];
            if ((DBCacheDir.EndsWith("/") == false) && (DBCacheDir.EndsWith("\\") == false))
                DBCacheDir += "\\";  //put a slash on it

            _Path = DBCacheDir + "DatabaseDiagrams\\" + MillimanCommon.Utilities.ConvertHexToString(context.Request.QueryString["image"]);

            context.Response.StatusCode = 200;
            context.Response.ContentType = "image/png";
            context.Response.WriteFile(_Path); 
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