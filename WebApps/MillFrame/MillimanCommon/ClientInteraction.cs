using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;

namespace MillimanCommon
{


    /// <summary>
    /// A JavaScript alert
    /// </summary>
    public static class Alert
    {
        /// <summary>
        /// Shows a client-side JavaScript alert in the browser.
        /// </summary>
        /// <param name="message">The message to appear in the alert.</param>
        public static void Show(string message)
        {
            // Cleans the message to allow single quotation marks
            string cleanMessage = message.Replace("'", "\\'");
            string script = "<script type=\"text/javascript\">alert('" + cleanMessage + "');</script>"; 
 

            // Gets the executing web page
            Page page = HttpContext.Current.CurrentHandler as Page;

            // Checks if the handler is a Page and that the script isn't allready on the Page
            if (page != null && !page.ClientScript.IsClientScriptBlockRegistered("alert"))
            {
                page.ClientScript.RegisterClientScriptBlock(typeof(Alert), "alert", script);
            }
        }

        public static void Refresh( int Seconds )
        {
            int Milliseconds = Seconds * 1000;
            string Refresher = "setTimeout('location.reload(true);', " + Milliseconds.ToString() + ");";
            string script = "<script type=\"text/javascript\">" + Refresher + "</script>";
            // Gets the executing web page
            Page page = HttpContext.Current.CurrentHandler as Page;
            // Checks if the handler is a Page and that the script isn't allready on the Page
            if (page != null && !page.ClientScript.IsClientScriptBlockRegistered("RefreshPage"))
            {
                page.ClientScript.RegisterClientScriptBlock(typeof(Alert), "RefreshPage", script);
            }
        }

        public static void DelayedNavigation(string URL, int Seconds)
        {
            int Milliseconds = Seconds * 1000;
            string Refresher = "setTimeout(\"window.location.href= '" + URL + "'\", " + Milliseconds.ToString() + ");";
            string script = "<script type=\"text/javascript\">" + Refresher + "</script>";
            // Gets the executing web page
            Page page = HttpContext.Current.CurrentHandler as Page;
            // Checks if the handler is a Page and that the script isn't allready on the Page
            if (page != null && !page.ClientScript.IsClientScriptBlockRegistered("DelayedNavigation"))
            {
                page.ClientScript.RegisterClientScriptBlock(typeof(Alert), "DelayedNavigation", script);
            }
        }

        public static void LaunchNewWindow(string URL)
        {
            String clientScriptName = "LaunchNewWindowScript";
            //Type clientScriptType = this.GetType();

            Page page = HttpContext.Current.CurrentHandler as Page;
            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager clientScript = page.ClientScript;

            // Checks if the handler is a Page and that the script isn't allready on the Page
            // Check to see if the client script is already registered.
            if (!clientScript.IsClientScriptBlockRegistered(clientScriptName))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<script type='text/javascript'>");
                sb.Append("window.open(' " + URL + "')"); //URL = where you want to redirect.
                sb.Append("</script>");
                clientScript.RegisterClientScriptBlock(typeof(Alert), clientScriptName, sb.ToString());
            }
        }
    }
}
