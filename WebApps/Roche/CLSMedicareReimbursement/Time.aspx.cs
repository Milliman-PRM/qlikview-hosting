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
using Controller;
using log4net;
using log4net.Config;

namespace CLSMedicareReimbursement
{
    public partial class Time : System.Web.UI.Page
    {
        public static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            log.Info(DateTime.Now + "||" + "Health Check - Application Started.");
            //----------------------------System Info-------------------------------------------
            var bSystemHealth = true;
            lblSystemDate.Text = DateTime.Now.ToString();

            var spaceError = "";
            var memoryError = "";
            var dbError = "";
            //----------------------------bSystemHealth-------------------------------------------
            var bQuerStringExist = Request.Url.AbsoluteUri.IndexOf("NeverFail") >= 0 ? true : false;
            //defaults
            lblMemory.Text = string.Format("<em>Un-Avalible</em>");
            lblDiskSpace.Text = string.Format("<em>Un-Avalible</em>");
            lblDbInfo.Text = string.Format("<em>Un-Avalible</em>");
            lblSchemaInfo.Text = string.Format("<em>Un-Avalible</em>");

            try
            {
                ////Existing code exists to retrieve memory use
                var PID = PsApiWrapper.GetPerformanceInfo();
                var FreeMemoryPercentage = (((double)PID.PhysicalAvailableBytes / (double)PID.PhysicalTotalBytes) * 100.0);
                var configMemory = (Convert.ToDouble(ConfigurationManager.AppSettings["Memory"].ToString()));
                if (FreeMemoryPercentage >= (100 - configMemory))
                {
                    lblMemory.Text = string.Format("<em>Avalible</em>");
                }
                else
                {
                    memoryError = "No memory (free memory < " + configMemory + " % (web.config)).";
                    bSystemHealth = false;                   
                }
            }
            catch (Exception ex)
            {
                Logger(ex, "memoryError");
                memoryError = "An exception occured, check excelption logging.";
                bSystemHealth = false;
            }

            try
            {
                //get the drives info
                var diskDriveArray = ConfigurationManager.AppSettings["DiskDrives"].Split(',').ToArray();
                //from config
                var configDiskSpace = (Convert.ToDouble(ConfigurationManager.AppSettings["DiskSpace"].ToString()));

                //loop through and find space
                foreach (var item in diskDriveArray)
                {
                    var diskDive = item + ":";
                    if (isDriveExists(diskDive.ToString()))
                    {
                        var driveInfo = new DriveInfo(diskDive);
                        var driveSpacePercentFree = (((double)driveInfo.AvailableFreeSpace / (double)driveInfo.TotalSize) * 100.0);//gives space in %
                        if (driveSpacePercentFree >= (100 - configDiskSpace))
                        {
                            lblDiskSpace.Text = string.Format("<em>Avalible</em>");
                        }
                        else
                        {
                            spaceError = "No disk space (free space < " + configDiskSpace + " % (web.config)).";
                            bSystemHealth = false;
                        }
                    }
                    else
                    {
                        spaceError = "Drive " + item.ToString().ToUpper() + " does not exist.";
                        bSystemHealth = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger(ex, "spaceError");
                spaceError = "An exception occured, check excelption logging.";
                bSystemHealth = false;
            }
            //----------------------------Database-------------------------------------------
            try
            {
                var stringConn = ConfigurationManager.ConnectionStrings["CLSdbDataContextConnectionString"].ToString();
                var database = DatabaseConnectionStringParser.GetDatabaseName(stringConn.ToString());
                var schema = DatabaseConnectionStringParser.GetSchemaName(stringConn.ToString());

                if (!string.IsNullOrEmpty(database))
                {
                    var objList = CLSController.getUniqueYear();
                    if (objList.Count > 0)
                    {
                        //test db query to see the results are coming back - Test db access
                        lblDbInfo.Text = string.Format("<em>Avalible</em>");
                    }
                    else
                    {
                        dbError = ("Database not accessible.").ToString();
                        bSystemHealth = false;
                    }
                }
                else
                {
                    dbError = ("Database not accessible.").ToString();
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
            }
            catch (Exception ex)
            {
                Logger(ex, "dbError");
                dbError = "An exception occured, check excelption logging.";
                bSystemHealth = false;
            }

            var status = "";
            if (bQuerStringExist)
            {
                status="200";
            }
            else
            {
                if (!bSystemHealth)
                    status = "555";
                if (bSystemHealth)
                    status = "200";
            }

            lblStatusCode.Text = status.ToString();
            Response.StatusCode = int.Parse(status.ToString());

            if (!bSystemHealth)
            {
                var item1 = ("General processing error.").ToString();
                var item2 = spaceError;
                var item3 = memoryError;
                var item4 = dbError;

                lblSystemHealth.Text = item1 + " " + item2 + " " + item3 + " " + item4;
            }
            else
            {
                lblSystemHealth.Text = "-------------------------------";
            }

        }
        /// <summary>
        /// Method to check if certian drive exist
        /// </summary>
        /// <param name="driveLetterWithColonAndSlash"></param>
        /// <returns></returns>
        bool isDriveExists(string driveLetterWithColonAndSlash)
        {
            var compareToDrive = driveLetterWithColonAndSlash + @"\";
            return DriveInfo.GetDrives().Any(x => x.Name == compareToDrive.ToUpper());
        }

        public static void Logger(Exception ex, string message)
        {
            log.Fatal(DateTime.Now + "||" + message, ex);
        }

   }
}