using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Security;
using MillimanCommon;

namespace ClientPublisher
{
    public partial class Dashboard : System.Web.UI.Page
    {
        //dashboard.aspx?dashboardid=[required]&context=[optional]&origin=[optional]
        /// <summary>
        /// Build a URL with all the params as required for launching a dashboard
        /// </summary>
        /// <param name="DashboardID"></param>
        /// <param name="Context"></param>
        /// <param name="Origin"></param>
        /// <returns></returns>
        public static string URLBuilder(string DashboardID,
                                          string Context = "",
                                          SystemOrigin Origin = SystemOrigin.Milliman)
        {
            string URL = "Dashboard.aspx?dashboardid=" + DashboardID;
            if (string.IsNullOrEmpty(Context) == false)
                URL += "&enterpriseid=" + Context;
            URL += "&origin=" + Origin.ToString().ToLower();
            return URL;
        }
        public static string MapToInternalID(string ID)
        {
            if (ConfigurationManager.AppSettings[ID] == null)
            {
                return ID;
            }
            return ConfigurationManager.AppSettings[ID];
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //for sub mounted folder we need an extra level on the path, this is for Version 2 migration
            string QVSubRoot = System.Configuration.ConfigurationManager.AppSettings["QVSubRoot"];
            if ( string.IsNullOrEmpty(QVSubRoot) == false )
            {
                QVSubRoot = System.Configuration.ConfigurationManager.AppSettings["QVSubRoot"];
                QVSubRoot = QVSubRoot.Replace('/','\\');
                if ( QVSubRoot.EndsWith("\\") == false )
                    QVSubRoot += "\\";
            }
            if ( (Session["milliman"] == null) && (Request["key"] == null) ) //no identieir must be covisint user,  if parm key, then we a milliman
            {
                string DashboardID = Request["dashboardid"];
                if (string.IsNullOrEmpty(DashboardID))
                    Response.Redirect("DashboardMissingIssue.html");

                //map the Covisint string POPULATION or COSTMODEL to a real QV project name
                string QVProjectName = ConfigurationManager.AppSettings[DashboardID];
                if (string.IsNullOrEmpty(QVProjectName))
                    Response.Redirect("DashboardMissingIssue.html");

                //look in the repo to find the first qualifed path of the selected project for this user
                MembershipUser MU = Membership.GetUser();
                string QVProject = MillimanCommon.UserRepo.GetInstance().FindQualifedQVProject(MU.UserName, QVProjectName, Roles.GetRolesForUser());
                if (string.IsNullOrEmpty(QVProject))
                    Response.Redirect("NotAuthorizedIssue.html");

                if (Membership.GetUser().IsApproved == false)
                    Response.Redirect("NotApprovedIssue.html");


                //will never be in session now
                string _ContextSelection = Session["patientid"] == null ? string.Empty : Session["patientid"].ToString();
                //we accept patientid or enterpriseid
                if (string.IsNullOrEmpty(_ContextSelection) == true)    //look on the query, 
                    _ContextSelection = Request.QueryString["patientid"] == null ? string.Empty : Request.QueryString["patientid"];
                if (string.IsNullOrEmpty(_ContextSelection) == true)    //look on the query, 
                    _ContextSelection = Request.QueryString["enterpriseid"] == null ? string.Empty : Request.QueryString["enterpriseid"];

                //don't look at origin on url, look at how the user logged in
                //SystemOrigin Origin = Request.QueryString["origin"] == null ? SystemOrigin.Milliman : (SystemOrigin)Enum.Parse(typeof(SystemOrigin), Request.QueryString["origin"]);
                SystemOrigin Origin = Session["milliman"] != null ? SystemOrigin.Milliman : SystemOrigin.ExternalSSO;
                if (Launch(Membership.GetUser().UserName, QVSubRoot + QVProject, _ContextSelection, "DashboardMissingIssue.html", Origin))
                    Response.Redirect("DashboardMissingIssue.html");

                dashboard_text.Text = @"You are logged into the Milliman site as '" + Context.User.Identity.Name + "'. ";
                dashboard_text.Text += @"You will be redirected to dashboard: " + DashboardID;
                this.Title = @"Dashboard Access";
            }
            else //must be milliman user
            {
                string QVProject = Request["key"];
                if (string.IsNullOrEmpty(QVProject))
                    Response.Redirect("DashboardMissingIssue.html");

                QVProject = MillimanCommon.Utilities.ConvertHexToString(QVProject);
                if (ProjectIsValidForUser(QVProject))
                {
                    SystemOrigin Origin = Session["milliman"] != null ? SystemOrigin.Milliman : SystemOrigin.ExternalSSO;
                    if (Launch(Membership.GetUser().UserName, QVSubRoot + QVProject, "", "DashboardMissingIssue.html", Origin))
                        Response.Redirect("DashboardMissingIssue.html");

                    dashboard_text.Text = @"You are logged into the Milliman site as '" + Membership.GetUser().UserName + "'. ";
                    dashboard_text.Text += @"You will be redirected to dashboard: " + QVProject;
                    this.Title = @"Dashboard Access";
                }
                else
                {
                    Response.Redirect("NotAuthorizedIssue.html");
                }
            }
        }

        //should the link allow the user to view the QVW
        private bool ProjectIsValidForUser(string QVW)
        {
            //make sure we are still logged in, if not exit
            try
            {
                if (Membership.GetUser() == null)
                {
                    Response.Redirect("userlogin.aspx");
                    return false;
                }
                string[] UserRoles = MillimanCommon.UserAccessList.GetRolesForUser();
                if (UserRoles != null)
                {
                    MillimanCommon.UserAccessList ACL = new MillimanCommon.UserAccessList(Membership.GetUser().UserName, UserRoles, false);
                    foreach (MillimanCommon.UserAccessList.UserAccess Access in ACL.ACL)
                    {
                        if (Access.ReducedVersionNotAvailable)
                        {
                            //QVRootrelative and QVW are both relative to qv document root
                            if (string.Compare(Access.QVRootRelativeProjectPath, QVW, true) == 0)
                                return true;
                        }
                        else
                        {
                            //reducedQVW is full path,  while project is relatvie to QV root
                            if (Access.ReducedQVW.ToLower().Contains(QVW.ToLower()) == true)
                                return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(Report.ReportType.Error, "Failed validating user request", ex);
            }
            return false;
        }
    }
}