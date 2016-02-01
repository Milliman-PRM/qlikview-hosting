using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanSupport
{
    public partial class time : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ( !IsPostBack)
            {

                SystemTime.Text = DateTime.Now.ToString();

                try
                {
                    string StatusXML = string.Empty;
                    bool SystemIsInBadHealth = false;

                    bool CurrentStatus = true;
                    int CurrentStatusCode = 200;

                    //StatusXML += GetQVStatus(out CurrentStatus, out CurrentStatusCode);
                    //if (CurrentStatus == false)
                    //    SystemIsInBadHealth = true;

                    //StatusXML += GetDBStatus(out CurrentStatus, out CurrentStatusCode);
                    //if (CurrentStatus == false)
                    //    SystemIsInBadHealth = true;

                    StatusXML += GetMemoryStatus(out CurrentStatus, out CurrentStatusCode);
                    if (CurrentStatus == false)
                        SystemIsInBadHealth = true;

                    StatusXML += GetDiskStatus(out CurrentStatus, out CurrentStatusCode);
                    if (CurrentStatus == false)
                        SystemIsInBadHealth = true;

                    Status.Text = @"<status>" + StatusXML + @"</status>";
                    //if (SystemIsInBadHealth)
                    //{
                    //    Response.StatusDescription = Status.Text;
                    //    Response.StatusCode = 555;
                    //}
                }
                catch (Exception)
                {
                    //allow default settings of status code on failure
                }
             }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Status">True/False depending upon request health</param>
        /// <param name="StatusCode">200 or OK</param>
        /// <returns>Status message to be placed in outgoing response</returns>
        //private string GetQVStatus(out bool Status, out int StatusCode)
        //{
        //    Status = true;
        //    StatusCode = 200;
        //    string Health = string.Empty;
        //    MillimanDev.Dashboard HealthCheck = new MillimanDev.Dashboard();
        //    bool QVStatus = HealthCheck.QVServerHealthCheck(out Health);
        //    if (QVStatus)
        //    {
        //        Health = @"OK";
        //    }
        //    else
        //    {
        //        StatusCode = 555;
        //        Status = false;
        //    }
        //    return @"<QVServer>" + Health + @"</QVServer>";
        //}

        //private string GetDBStatus(out bool Status, out int StatusCode)
        //{
        //    Status = true;
        //    StatusCode = 200;
        //    string Health = @"OK";
         
        //    return @"<DBConnections>" + Health + @"</DBConnections>";
        //}

        private string GetMemoryStatus(out bool Status, out int StatusCode)
        {
            Status = true;
            StatusCode = 200;
            string Health = @"?";

            return @"<FreeMemoryPercentage>" + Health + @"</FreeMemoryPercentage>";
        }

        private string GetDiskStatus(out bool Status, out int StatusCode)
        {
            Status = true;
            StatusCode = 200;
            string Health = @"?";

            return @"<FreeDiskPercentage>" + Health + @"</FreeDiskPercentage>";
        }

    }
}