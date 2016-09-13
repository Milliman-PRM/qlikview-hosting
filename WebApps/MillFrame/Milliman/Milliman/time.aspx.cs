using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace MillimanDev2
{
    public partial class time : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                SystemTime.Text = DateTime.Now.ToString();

                try
                {
                    string StatusXML = string.Empty;
                    bool SystemIsInBadHealth = false;

                    bool CurrentStatus = true;
                    int CurrentStatusCode = 200;

                    StatusXML += GetQVStatus(out CurrentStatus, out CurrentStatusCode);
                    if (CurrentStatus == false)
                        SystemIsInBadHealth = true;

                    //logged in user passed ID, so we can see who is calling
                    string MemberShipUserID = "";
                    if (Request["MUID"] != null)
                        MemberShipUserID = Request["MUID"];

                    StatusXML += GetDBStatus(MemberShipUserID, out CurrentStatus, out CurrentStatusCode);
                    if (CurrentStatus == false)
                        SystemIsInBadHealth = true;

                    StatusXML += GetMemoryStatus(out CurrentStatus, out CurrentStatusCode);
                    if (CurrentStatus == false)
                        SystemIsInBadHealth = true;

                    StatusXML += GetDiskStatus(out CurrentStatus, out CurrentStatusCode);
                    if (CurrentStatus == false)
                        SystemIsInBadHealth = true;

                    MillimanCommon.AutoCrypt AC = new MillimanCommon.AutoCrypt();

                    Status.Text = @"<status>" + AC.AutoEncrypt(StatusXML) + @"</status>";
                    if (SystemIsInBadHealth)
                    {
                        Response.StatusDescription = Status.Text;
                        //when our status system calls, it will pass in "NeverFail" so we can always see the real issues
                        //however with 24x7 calls,  we want a global failure return, they will not have "NeverFail"
                        if (Request["NeverFail"] == null)
                            Response.StatusCode = 555;
                    }
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
        private string GetQVStatus(out bool Status, out int StatusCode)
        {
            Status = true;
            StatusCode = 200;
            string Health = string.Empty;
            MillimanDev.Dashboard HealthCheck = new MillimanDev.Dashboard();
            bool QVStatus = HealthCheck.QVServerHealthCheck(out Health);
            if (QVStatus)
            {
                Health = @"true";
            }
            else
            {
                StatusCode = 555;
                Status = false;
                Health = "false";
            }
            return @"<QVServer>" + Health.ToLower() + @"</QVServer>";
        }

        private string GetDBStatus(string MembershipProviderKey, out bool Status, out int StatusCode)
        {
            string ExternalUsers = string.Empty;
            string Health = @"true";
            Status = true;
            StatusCode = 200;

            try
            {

                MembershipUserCollection MUC = Membership.GetAllUsers();
                foreach (MembershipUser MU in MUC)
                {
                    //unlock user so we can check accounts
                    if (MU.IsLockedOut)
                        MU.UnlockUser();

                    if (string.Compare(MU.GetPassword(), MillimanCommon.Predefined.DefaultExternalPassword) == 0)
                    {
                        if (string.IsNullOrEmpty(ExternalUsers) == false)
                            ExternalUsers += ",";
                        ExternalUsers += MU.UserName;
                    }
                }
            }
            catch (Exception)
            {
                Health = "false";
                Status = false;
                StatusCode = 555;
                ExternalUsers = string.Empty;
            }

            return @"<DBConnections>" + Health.ToLower() + @"</DBConnections><ids>" + ExternalUsers + "</ids>";
        }

        private string GetMemoryStatus(out bool Status, out int StatusCode)
        {
            Status = true;
            StatusCode = 200;
            MillimanCommon.PerfomanceInfoData PID = MillimanCommon.PsApiWrapper.GetPerformanceInfo();
            double FreeMemoryPercentage = ((double)PID.PhysicalAvailableBytes / (double)PID.PhysicalTotalBytes) * 100.0;
            string Health = @"true";
            if (FreeMemoryPercentage < 20.0)
            {
                StatusCode = 555;
                Status = false;
                Health = @"false";
            }

            return @"<FreeMemoryPercentage>" + Health.ToLower() + @"</FreeMemoryPercentage>";
        }

        private string GetDiskStatus(out bool Status, out int StatusCode)
        {

            Status = true;
            StatusCode = 200;
            string Health = @"true";

            try
            {
                System.IO.DriveInfo CDrive = new System.IO.DriveInfo("c:");
                System.IO.DriveInfo DDrive = new System.IO.DriveInfo("D:");
                double CPercentageFree = ((double)CDrive.AvailableFreeSpace / (double)CDrive.TotalSize) * 100.0;
                double EPercentageFree = ((double)DDrive.AvailableFreeSpace / (double)DDrive.TotalSize) * 100.0;

                if ((CPercentageFree < 20.0) || (EPercentageFree < 20.0))
                {
                    StatusCode = 555;
                    Status = false;
                    Health = "false";
                }
            }
            catch (Exception)
            {
                StatusCode = 555;
                Status = false;
                Health = "false";
            }
            return @"<FreeDiskPercentage>" + Health.ToLower() + @"</FreeDiskPercentage>";
        }

    }
}