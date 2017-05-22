using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClientPublisher
{
    public partial class ReductionStep3 : System.Web.UI.Page
    {

        public class ReductionReviewClass : TaskBase
        {
            //used to display progress bars
            public bool ShowProgressBars { get; set; }
            public int Uploaded { get; set; }
            public int TotalToUpload { get; set; }
            //end progress bars

            public string ReductionIndex { get; set; }
            public string DisplayMessage { get; set; }
            public string DetailedDisplayMessage { get; set; }
            public string NavigateTo { get; set; }
            public ClientPublisher.ProjectSettingsExtension ProjectSettings { get; set; }
            public MillimanCommon.ProjectSettings NewProjectSettings { get; set; }

            public string WorkingDirectory { get; set; }
            
            //the path to where the project has been published too
            public string OriginalProject { get; set; }
            public string Account { get; set; }


            public override void TaskProcessor(object parms)
            {

                try
                {
                    Process(parms);

                }
                catch (Exception)
                {
                    TaskCompletionMessage = "Project failed to publish.";
                    TaskCompletionWithError = true;
                }
            }

            //this code is split off from taskprocessor so we can re-use it to check the QVW signature, tis re-used by
            //reduction report.aspx since it's the first screen of publishing
            public void Process(object parms)
            {
                //we would have created a tooltip.dat, description.dat and reductionrequired.dat file if the values changed
                ClientPublisher.ProcessingCode.Processor Process = new ClientPublisher.ProcessingCode.Processor();

                DisplayMessage = "Setting up local working directories....";

                string ReducedCacheDir = System.IO.Path.Combine(ProjectSettings.AbsoluteProjectPath, "ReducedCachedQVWs");
                string ReducedUserDir = System.IO.Path.Combine(ProjectSettings.AbsoluteProjectPath, "ReducedUserQVWs");
                string WorkingCacheDir = System.IO.Path.Combine(WorkingDirectory, "ReducedCachedQVWs");
                string WorkingUserDir = System.IO.Path.Combine(WorkingDirectory, "ReducedUserQVWs");

                //first remove any un-needed files like legacy and "ready_to_publish.txt"
                string ReadyToPublish = System.IO.Path.Combine(WorkingDirectory, "ready_to_publish.txt");
                if (System.IO.File.Exists(ReadyToPublish))
                    System.IO.File.Delete(ReadyToPublish);

                DisplayMessage = "Removing unnecessary files before publishing";
                List<string> DeleteDirectories = new List<string>() { "legacy", "configurations" };
                foreach (string DelDir in DeleteDirectories)
                {
                    string LegacyDir = System.IO.Path.Combine(WorkingDirectory, DelDir);
                    if (System.IO.Directory.Exists(LegacyDir))
                    {
                        string[] AllLegacy = System.IO.Directory.GetFiles(LegacyDir, "*.*", System.IO.SearchOption.AllDirectories);
                        foreach (string LegacyFile in AllLegacy)
                            System.IO.File.Delete(LegacyFile);

                        System.IO.Directory.Delete(LegacyDir, true);
                    }
                }
                //open and update NEW project publishing fields
                NewProjectSettings.UploadedToProduction = System.Web.Security.Membership.GetUser().UserName;
                NewProjectSettings.UploadedToProductionDate = DateTime.Now.ToString();
                NewProjectSettings.Save();

                //archive of previsous
                string ZID = CreateBackupSet(OriginalProject, System.IO.Path.Combine(OriginalProject, "history"));

                string[] FilesToPublish = System.IO.Directory.GetFiles(WorkingDirectory, "*.*", System.IO.SearchOption.AllDirectories);
                TotalToUpload = FilesToPublish.Length;

                foreach( string PublishFile in FilesToPublish )
                {
                    string VirtualSource = PublishFile.Substring(WorkingDirectory.Length+1);  //+1 to get rid of directory slash
                    string Destination = System.IO.Path.Combine(OriginalProject, VirtualSource);
                    if (System.IO.File.Exists(Destination))
                        System.IO.File.Delete(Destination);
                    System.IO.File.Copy(PublishFile, Destination);
                    Uploaded++;
                    DetailedDisplayMessage = "Updating file " + Uploaded.ToString() + " of files " + FilesToPublish.Length.ToString() ;
                }

                TaskCompletionMessage = "Project '" + NewProjectSettings.QVName + "' published successfully.";
                TaskCompleted = true;
            }

            /// <summary>
            /// Dump a backup into a zip that is password protected
            /// </summary>
            /// <param name="DirToBackup">Full path of dir to backup</param>
            /// <param name="SaveZipToDir">Directory on where the backup should be written</param>
            /// <returns></returns>
            public string CreateBackupSet(string DirToBackup, string SaveZipToDir)
            {

                try
                {
                    System.IO.Directory.CreateDirectory(SaveZipToDir);

                    string ZID = System.Guid.NewGuid().ToString("N");
                    using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                    {
                        zip.Password = "M" + ZID;
                        zip.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;
                        string[] AllFiles = System.IO.Directory.GetFiles(DirToBackup, "*.*", System.IO.SearchOption.AllDirectories);
                        string[] InvalidExtension = new string[] { ".meta",".zip" };
                        foreach (string S in AllFiles)
                        {
                            string Extension = System.IO.Path.GetExtension(S);
                            if (InvalidExtension.Contains(Extension.ToLower()) == false)
                            {
                                string Dir = System.IO.Path.GetDirectoryName(S).Substring(DirToBackup.Length);
                                Dir = Dir.TrimStart(new char[] { '\\' });  //get rid of starting directory delimiters
                                zip.AddFile(S, Dir);
                            }
                        }
                        zip.Save(System.IO.Path.Combine(SaveZipToDir, ZID + ".zip"));
                    }
                    return ZID;
                }
                catch (Exception ex)
                {
                    string e = ex.ToString();
                }

                return "";
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
                    else
                    {
                        string NavigateTo = "HTML/GeneralIssue.aspx?msg=" + MillimanCommon.Utilities.ConvertStringToHex("Missing project index - cannot publish project.'");
                        Response.Redirect(NavigateTo);
                        return;
                    }
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
                    ReductionReviewClass SU = new ReductionReviewClass();
                    SU.ReductionIndex = Index.ToString();
                    SU.ProjectSettings = theProject;
                    SU.WorkingDirectory = WorkingDirectory;
                    SU.Account = System.Web.Security.Membership.GetUser().UserName;
                    SU.ShowProgressBars = false;
                    SU.OriginalProject = theProject.AbsoluteProjectPath;
                    SU.NewProjectSettings = MillimanCommon.ProjectSettings.Load(System.IO.Path.Combine(WorkingDirectory, theProject.ProjectName + ".hciprj"));
                    Global.TaskManager.ScheduleTask(SU);
                    SU.StartTask();
                    Response.Redirect("ReductionStep3.aspx?Processing=" + SU.TaskID);
                }
                else
                {
                    string ProcessID = Request["Processing"].ToString();
                    ReductionReviewClass SU = Global.TaskManager.FindTask(ProcessID) as ReductionReviewClass;
                    if (SU != null)
                    {
                        Status.Text = SU.DisplayMessage;
                        if (SU.TaskCompletionWithError)
                        {
                            Image1.Visible = false;  //busy spinner
                            Status.Text = SU.TaskCompletionMessage;
                            SU.AbortTask = true;
                            Global.TaskManager.DeleteTask(SU.TaskID);
                            MillimanCommon.Alert.Show("Project update failed");
                        }
                        else if (SU.TaskCompleted)
                        {
                            Image1.Visible = false;//busy spinner
                            Status.Text = SU.TaskCompletionMessage;
                            Status.Visible = true;
                            Global.TaskManager.DeleteTask(SU.TaskID);
                            MillimanCommon.Alert.Show("Project update has completed successfully.");
                        }
                        else
                        {
                            Status.Text = SU.DisplayMessage;
                            if ( string.IsNullOrEmpty( SU.DetailedDisplayMessage) == false )
                            {
                                DetailedStatus.Text = SU.DetailedDisplayMessage;
                                DetailedStatus.Visible = true;

                                Executing.Width = new Unit((((double)SU.Uploaded / (double)SU.TotalToUpload) * 100.0), UnitType.Percentage);  //yes executing + completed is correct, we want exe bar to stick out from behind completed

                                ProgressBars.Visible = true;
                            }
  
                             MillimanCommon.Alert.Refresh(3);
  
                        }
                    }
                    else
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "System error - missing task");
                        Response.Redirect("errors/missingtask.html");
                        Status.Text = "Could not retrieve processing status.";
                    }
                }
            }
        }
    }
}