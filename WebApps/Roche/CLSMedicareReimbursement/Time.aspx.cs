using CLSConfigurationCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CLSMedicareReimbursement
{
    public partial class Time : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //----------------------------System Info-------------------------------------------
            var bSystemHealth = false;

            lblSystemDate.Text = DateTime.Now.ToString();

            var spaceError = "";
            var memoryError = "";
            var dbError = "";
            var webError = "";
            //----------------------------bSystemHealth-------------------------------------------
            bool bQuerStringExist = Request.Url.AbsoluteUri.IndexOf("NeverFail") >= 0 ? true : false;
            if (bQuerStringExist)
            {
                lblMemory.Text = string.Format("<em>Un-Avalible</em>");
                ////Existing code exists to retrieve memory use
                //MillimanCommon.PerfomanceInfoData PID = MillimanCommon.PsApiWrapper.GetPerformanceInfo();
                //double FreeMemoryPercentage = ((double)PID.PhysicalAvailableBytes / (double)PID.PhysicalTotalBytes) * 100.0;
                double FreeMemoryPercentage = 100.0;
                
                var confMemory = (Convert.ToDouble(ConfigurationManager.AppSettings["Memory"].ToString()));
                if (FreeMemoryPercentage >= confMemory)
                {
                    lblMemory.Text = string.Format("<em>Avalible</em>");
                }
                else
                {
                    memoryError = "No disk space (free memory < 80 % (web.config)  )";
                    bSystemHealth = false;
                }

                //Existing code exists for disk status:
                lblDiskSpace.Text = string.Format("<em>Un-Avalible</em>");
                var CDrive = new DriveInfo("c:");
                var DDrive = new DriveInfo("D:");
                var CPercentageFree = ((double)CDrive.AvailableFreeSpace / (double)CDrive.TotalSize) * 100.0;
                var DPercentageFree = ((double)DDrive.AvailableFreeSpace / (double)DDrive.TotalSize) * 100.0;

                //from config
                var confDiskSpace = (Convert.ToDouble(ConfigurationManager.AppSettings["DiskSpace"].ToString()));
                              
                if (CPercentageFree >= confDiskSpace)
                {
                    lblDiskSpace.Text = string.Format("<em>Avalible</em>");
                }
                else
                {
                    spaceError = "No disk space (free space < 80 % (web.config) )";
                    bSystemHealth = false;
                }
                if (DPercentageFree >= confDiskSpace)
                {
                    lblDiskSpace.Text = string.Format("<em>Avalible</em>");
                }
                else
                {
                    spaceError = "No disk space (free space < 80 % (web.config) )";
                    bSystemHealth = false;
                }

                //----------------------------Database-------------------------------------------
                var stringConn = ConfigurationManager.ConnectionStrings["CLSdbDataContextConnectionString"].ToString();
                var database = DatabaseConnectionStringParser.GetDatabaseName(stringConn.ToString());
                var schema = DatabaseConnectionStringParser.GetSchemaName(stringConn.ToString());

                lblDbInfo.Text = string.Format("<em>Un-Avalible</em>");
                lblSchemaInfo.Text = string.Format("<em>Un-Avalible</em>");
                lblWebServiceInfo.Text = string.Format("<em>Un-Avalible</em>");
                
                if (!string.IsNullOrEmpty(database))
                {
                    lblDbInfo.Text = string.Format("<em>Avalible</em>");
                }
                else
                {
                    dbError = ("Database not accessible").ToString();
                    bSystemHealth = false;
                }
                if (!string.IsNullOrEmpty(schema))
                {
                    lblSchemaInfo.Text = string.Format("\"{0}\"", schema);
                }
                else
                {
                    bSystemHealth = false;
                }

                //----------------------------Web Service-------------------------------------------
                var url = "http://labsystemshandbook.milliman.com/";               
                try
                {
                    var myRequest = (HttpWebRequest)WebRequest.Create(url);
                    var response = (HttpWebResponse)myRequest.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //  it's at least in some way responsive but may be internally broken as you could find out if you called one of the methods for real
                        //lblWebServiceInfo.Text = string.Format("{0} | <em>Avalible</em>", url);
                        lblWebServiceInfo.Text = string.Format("<em>Avalible</em>");
                    }
                    else
                    {
                        webError = ("Web server not accessible").ToString();
                        bSystemHealth = false;
                    }
                }
                catch (Exception ex)
                {
                    //  not available at all, for some reason
                    lblMessage.Text = lblMessage.Text + (string.Format("{0} unavailable: {1}", url, ex.Message));
                }
            }
            else
            {
                if (!bSystemHealth)
                    Response.StatusCode = 555;
                if (bSystemHealth)
                    Response.StatusCode = 200;
            }
            lblMessage.Text = "StatusCode: " + Response.StatusCode.ToString();
            
            if (!bSystemHealth)
            {
                var item1 = ("General processing error").ToString();
                var item2 = spaceError;
                var item3 = memoryError;
                var item4 = dbError;
                var item5 = webError;

                lblSystemHealth.Text = "\u25A1   '" + item1 +
                                        "<br>" + "\u25A1   '" + item2 +
                                        "<br>" + "\u25A1   '" + item3 +
                                        "<br>" + "\u25A1  '" + item4 +
                                        "<br>" + "\u25A1  '" + item5;
            }
            else
            {
                lblSystemHealth.Text = "-------------------------------";
            }        
            //----------------------------browser-------------------------------------------
            lblIsMobile.Text = this.Request.Browser.IsMobileDevice.ToString() +
                                        " - " + this.Request.Browser.MobileDeviceModel +
                                        " - " + this.Request.Browser.MobileDeviceManufacturer;

            lblPlatform.Text = this.Request.Browser.Platform;
            if (this.Request.Browser.RequiresSpecialViewStateEncoding == false)
            {
                lblNeedsSpecialVS.Text = "No Encoding is required";
            }
            else
            {
                lblNeedsSpecialVS.Text = "Encoding is required";
            }

            lblScreen.Text = this.Request.Browser.ScreenPixelsHeight + " X " + this.Request.Browser.ScreenPixelsWidth;

            lblSupports.Text = (this.Request.Browser.SupportsCss ? "Css is good" : "No CSS = No Good?!")
                                            + " | " + (this.Request.Browser.SupportsXmlHttp ? "AJAX is Good" : "No AJAX = No Worky");

            lblBrowserStuff.Text = string.Format(
                                                "<em>Browser</em>: {0} | <em>Type</em>: {1} | <em>Version</em>: {2}"
                                                , this.Request.Browser.Browser
                                                , this.Request.Browser.Type
                                                , this.Request.Browser.Version);

        }
    }
}