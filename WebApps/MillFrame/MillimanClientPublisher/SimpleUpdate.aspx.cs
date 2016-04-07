using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClientPublisher
{
    public partial class SimpleUpdate : System.Web.UI.Page
    {
        public class SimpleUpdateClass : TaskBase
        {
            public string DisplayMessage { get; set; }
            public string NavigateTo { get; set; }
            public ClientPublisher.ProjectSettingsExtension ProjectSettings { get; set; }

            public string WorkingDirectory { get; set; }

            public string Account { get; set; }


            public override void TaskProcessor(object parms)
            {

                Process(parms);
            }

            //this code is split off from taskprocessor so we can re-use it to check the QVW signature, tis re-used by
            //reduction report.aspx since it's the first screen of publishing
            public void Process(object parms)
            {
                //we would have created a tooltip.dat, description.dat and reductionrequired.dat file if the values changed

                DisplayMessage = "Creating backup checkpoint....";

                //create backup set
                //string ZID = CreateBackupSet(PS.LoadedFromPath, System.IO.Path.Combine(PS.LoadedFromPath, "history"));
                string BackupTo = System.IO.Path.Combine(ProjectSettings.LoadedFromPath, "history");
                string BackupSet = ClientPublisher.PublisherUtilities.CreateBackupSet(ProjectSettings.AbsoluteProjectPath, BackupTo);
                if (string.IsNullOrEmpty(BackupSet))
                {
                    DisplayMessage = "Failed to create backup set, updating aborting.....";
                    NavigateTo = "html/ProjectSavedFailed.html";
                    TaskCompletionWithError = true;
                    TaskCompleted = true;
                    base.TaskProcessor(parms);
                    return;
                }

                try
                {
                    DisplayMessage = "Update starting....";
                    List<string> IconExtensions = new List<string>() { ".jpg", ".jpeg", ".gif", ".png" };
                    List<string> DocumentExtensions = new List<string>() { ".docx", ".xlsx", ".txt", ".pdf" };
                    string[] Files = System.IO.Directory.GetFiles(WorkingDirectory, "*.*");
                    if (Files != null)
                    {
                        foreach (string F in Files)
                        {
                            //Updating QVW
                            if (string.Compare(System.IO.Path.GetExtension(F), ".qvw", true) == 0)
                            {
                                DisplayMessage = "Updating Qlikview QVW....";
                                string To = System.IO.Path.Combine(ProjectSettings.AbsoluteProjectPath, ProjectSettings.QVName + ".qvw");
                                System.IO.File.Copy(F, To);
                                ProjectSettings.OriginalProjectName = System.IO.Path.GetFileName(F);
                            }

                            //Updating Icon
                            else if (IconExtensions.Contains(System.IO.Path.GetExtension(F).ToLower()))
                            {
                                DisplayMessage = "Updating icon.....";
                                string To = System.IO.Path.Combine(ProjectSettings.AbsoluteProjectPath, ProjectSettings.QVName + System.IO.Path.GetExtension(F));
                                System.IO.File.Copy(F, To);
                                ProjectSettings.QVThumbnail = System.IO.Path.GetFileName(To);
                            }
                            //Upating User Manual
                            else if (DocumentExtensions.Contains(System.IO.Path.GetExtension(F).ToLower()))
                            {
                                DisplayMessage = "Updating user manual.....";
                                string To = System.IO.Path.Combine(ProjectSettings.AbsoluteProjectPath, ProjectSettings.QVName + System.IO.Path.GetExtension(F));
                                System.IO.File.Copy(F, To);
                                ProjectSettings.UserManual = System.IO.Path.GetFileName(To);
                                ProjectSettings.OriginalUserManualName = System.IO.Path.GetFileName(F); //save original file for display to user
                            }
                            //Updating Tool Tip
                            else if (System.IO.Path.GetFileName(F).ToLower() == "tooltip.dat")
                            {
                                DisplayMessage = "Updating tool tip.....";
                                ProjectSettings.QVTooltip = System.Security.SecurityElement.Escape(System.IO.File.ReadAllText(F));
                            }
                            //Updating Description
                            else if (System.IO.Path.GetFileName(F).ToLower() == "description.dat")
                            {
                                DisplayMessage = "Updating description.....";
                                ProjectSettings.QVDescription = System.Security.SecurityElement.Escape(System.IO.File.ReadAllText(F));
                            }
                            //Updating Restricted Views
                            else if (System.IO.Path.GetFileName(F).ToLower() == "reductionrequired.dat")
                            {
                                DisplayMessage = "Updating restricted view status.....";
                                ProjectSettings.SupportsReduction = System.Convert.ToBoolean(System.IO.File.ReadAllText(F));
                            }

                            //delete each file after we process it
                            System.IO.File.Delete(F);
                        }

                        if (ProjectSettings.Save())
                        {
                            NavigateTo = "html/ProjectSaved.html";
                            TaskCompletionWithError = false;
                        }
                        else
                        {
                            NavigateTo = "html/ProjectSavedFailed.html";
                            TaskCompletionWithError = true;
                        }
                    }

                    TaskCompleted = true;
                    base.TaskProcessor(parms);
                    return;
                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Unspecified Error", ex);
          
                }

                ///try an restore from checkpoint
                try
                {
                    ClientPublisher.PublisherUtilities.RestoreBackupSet(ProjectSettings.AbsoluteProjectPath, BackupSet);

                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to recover from checkpoint", ex);
                }

                NavigateTo = "html/ProjectSavedFailed.html";
                TaskCompletionWithError = true;
                TaskCompleted = true;
                base.TaskProcessor(parms);

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["Processing"] == null)
                {
                    int Index = 0;
                    if (Request["Key"] != null)
                        Index = System.Convert.ToInt32(Request["Key"]);

                    ProjectSettingsExtension theProject = null;
                    IList<ProjectSettingsExtension> Projects = Session["Projects"] as IList<ProjectSettingsExtension>;
                    if ((Projects != null) && (Index < Projects.Count()))
                    {
                        theProject = Projects[Index];
                    }
                    if (theProject == null)
                    {
                        Response.Redirect("HTML/MissingProject.html");
                        return;
                    }
                    if (System.Web.Security.Membership.GetUser() == null)
                    {
                        Response.Redirect("HTML/NotAuthorizedIssue.html");
                        return;
                    }
                    string WorkingDirectory = PublisherUtilities.GetWorkingDirectory(System.Web.Security.Membership.GetUser().UserName, theProject.ProjectName);
                    SimpleUpdateClass SU = new SimpleUpdateClass();
                    SU.ProjectSettings = theProject;
                    SU.WorkingDirectory = WorkingDirectory;
                    SU.Account = System.Web.Security.Membership.GetUser().UserName;
                    Global.TaskManager.ScheduleTask(SU);
                    SU.StartTask();
                    Response.Redirect("SimpleUpdate.aspx?Processing=" + SU.TaskID);
                }
                else
                {
                    string ProcessID = Request["Processing"].ToString();
                    SimpleUpdateClass SU = Global.TaskManager.FindTask(ProcessID) as SimpleUpdateClass;
                    if (SU != null)
                    {
                        Status.Text = SU.DisplayMessage;
                        if (SU.TaskCompletionWithError)
                        {
                            Status.Text = SU.TaskCompletionMessage;
                            SU.AbortTask = true;
                            Global.TaskManager.DeleteTask(SU.TaskID);
                            Response.Redirect(SU.NavigateTo);
                        }
                        else if (SU.TaskCompleted) 
                        {
                            Status.Text = SU.TaskCompletionMessage;
                            Status.Visible = true;
                            Global.TaskManager.DeleteTask(SU.TaskID);
                            Response.Redirect(SU.NavigateTo);
                        }
                        else
                        {
                            Status.Text = SU.DisplayMessage;
                            MillimanCommon.Alert.Refresh(3);
                        }
                    }
                    else
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "System error - missing task");
                        Response.Redirect("errors/missingtask.aspx");
                        Status.Text = "Could not retrieve processing status.";
                    }
                }
            }
        }
    }
}