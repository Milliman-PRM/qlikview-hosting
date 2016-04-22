using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.Admin
{
    public partial class PushToProduction : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.AppendHeader("Refresh", "15");
            if (!IsPostBack)
            {
                if (Request["ProjectPath"] != null)
                {
                    string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                    //DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));
                    MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(System.IO.Path.Combine(DocumentRoot, Request["ProjectPath"]));
                    if (PS != null)
                    {
                        string ErrorMessages = DataIsReady(PS);
                        if (string.IsNullOrEmpty(ErrorMessages) == true )
                        {
                            string EmailCache = "";
                            if (Session["PromptView"] != null)
                                EmailCache = Session["PromptView"].ToString();

                            string TaskID = Global.Publisher.PublishProject(System.IO.Path.Combine(DocumentRoot, Request["ProjectPath"]), EmailCache);
                            Response.Redirect("PushToProduction.aspx?Task=" + TaskID);
                        }


                        Status.Text = "Project '" + PS.ProjectName + "' does not have enough information to publish.<br><br>Please correct the following issues:<br>" + ErrorMessages;  //need more info here.
                        Container.Visible = false;
                        
                    }
                }
                else if (Request["Task"] != null)
                {
                    if (Response.Headers["Refresh"] == null)
                        Response.AppendHeader("Refresh", "3");

                    string ErrorMsg = Global.Publisher.GetError(Request["Task"]);
                    if (string.IsNullOrEmpty(ErrorMsg) == true)
                    {
                        Status.Text = "Production server update is " + Global.Publisher.GetPercentComplete(Request["Task"]) + "% complete.";
                        string Percentage = Global.Publisher.GetPercentComplete(Request["Task"]);
                        if (System.Convert.ToInt32(Percentage) >= 100)
                            Percentage = "99"; //never go beyond 99
                        ProgressBar.Width = new Unit(System.Convert.ToDouble(Global.Publisher.GetPercentComplete(Request["Task"])), UnitType.Percentage);

                        ProgressBar.Height = new Unit(20.0, UnitType.Pixel);
                        DetailMessage.Text = Global.Publisher.GetDetailMessage(Request["Task"]);
                    }
                    else
                    {
                        Status.Text = ErrorMsg;
                    }
                    if (string.IsNullOrEmpty(ErrorMsg) == false)
                    {
                        Response.Headers.Set("Refresh", "1000000");
                        Global.Publisher.DeleteStatus(Request["Task"]); 
                        ProgressBar.Visible = false;
                    }
                    else if (Global.Publisher.IsFinished(Request["Task"]) == true)
                    {
                       
                        Response.Headers.Set("Refresh", "1000000");
                        Status.Text = "Production server update is complete.";
                        Global.Publisher.DeleteStatus(Request["Task"]);
                        MillimanCommon.Alert.Show(Status.Text);
                    }
                }
            }
        }

        private string DataIsReady(MillimanCommon.ProjectSettings PS)
        {
            string ErrorMessage = "";
            if (string.IsNullOrEmpty(PS.ProjectName))
            {
                ErrorMessage += "<li>Project name is required.</li>";
            }
            if (string.IsNullOrEmpty(PS.QVDescription))
            {
                ErrorMessage += "<li>Project description is required.</li>";
            }
            if (string.IsNullOrEmpty(PS.LoadedFromPath))
            {
                ErrorMessage += "<li>Publication directory is required.</li>";
            }

            if (string.IsNullOrEmpty(ErrorMessage) == false)
                ErrorMessage = "<ol>" + ErrorMessage + "</ol>";
            return ErrorMessage;
        }
    }
}