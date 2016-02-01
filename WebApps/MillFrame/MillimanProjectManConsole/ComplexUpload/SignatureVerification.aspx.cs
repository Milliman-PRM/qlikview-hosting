using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.ComplexUpload
{
    public partial class SignatureVerification : System.Web.UI.Page
    {
        public class SignatureVerificationClass : TaskBase
        {
            public string DisplayMessage { get; set; }
            public string NavigateTo { get; set; }
            public bool ShowRenameButton { get; set; }
            public bool ContainsHierarchy { get; set; }

            public override void TaskProcessor(object parms)
            {

                Reports.ClearAll();
                Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass("New project parameters provided"));

                Process(parms);
            }

            //this code is split off from taskprocessor so we can re-use it to check the QVW signature, tis re-used by
            //reduction report.aspx since it's the first screen of publishing
            public void Process(object parms)
            {
                ShowRenameButton = false;

                MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(QualafiedProjectFile);

                MillimanCommon.XMLFileSignature XMLFS = new MillimanCommon.XMLFileSignature(System.IO.Path.Combine(PS.AbsoluteProjectPath, PS.QVName + ".qvw_new"));

                DisplayMessage = "Checking QVW and project group associations.";
                string Group = string.Empty;
                foreach (KeyValuePair<string, string> Item in XMLFS.SignatureDictionary)
                {
                    if (Item.Key.StartsWith("@") == true)
                    {
                        if ( (string.IsNullOrEmpty(Group) == false) && (string.IsNullOrEmpty(Item.Value) == false))
                            Group += "_";
                        Group += Item.Value.ToLower();
                    }
                }

                bool QVWNotSigned = false;
                //special case if we allow unsigned QVW via the web config settings "AllowUnsignedQVWs", and the QVW is not signed
                //but we have a group associated with project, use the project group setting to allocate a bucket for QVW
                bool AllowUnsignedQVW = System.Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["AllowUnsignedQVWs"]);
                if ((AllowUnsignedQVW) && (string.IsNullOrEmpty(PS.Groups) == false) && (string.IsNullOrEmpty(Group) == true))
                {
                    Group = PS.Groups;
                    QVWNotSigned = true;
                }
                else if (string.IsNullOrEmpty(Group) == true)
                {
                    NavigateTo = "errors/groupnotavailable.html";
                    TaskCompleted = true;
                    TaskCompletionWithError = true;
                    TaskCompletionMessage = "Group name must be specified in the QVW signature.  The associated QVW is not signed with valid group signature information.";
                    AbortTask = true;
                    base.TaskProcessor(parms);
                    return;
                }

                List<string> Groups = null;

                try
                {
                    Groups = Global.GetInstance().GetGroups();
                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to retrieve production groups", ex);
                    NavigateTo = "errors/servergroupaccess.html";
                    TaskCompleted = true;
                    TaskCompletionWithError = true;
                    TaskCompletionMessage = "Failed to retrieve groups from production system";
                    AbortTask = true;
                    base.TaskProcessor(parms);
                    return;
                }

                //directory of current project must match group
                string DirPath = Group.Replace('_', '\\');
                string ProjectPath = QualafiedProjectFile.Replace('/', '\\');
                if (ProjectPath.ToLower().IndexOf(DirPath.ToLower()) == -1)
                {
                    string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                    string ProjectPathOnly = System.IO.Path.GetDirectoryName(ProjectPath);
                    string ProjectPathGroupOnly = ProjectPathOnly.Substring(DocumentRoot.Count());
                    ProjectPathGroupOnly = ProjectPathGroupOnly.Trim(new char[] { '\\' });
                    ProjectPathGroupOnly = ProjectPathGroupOnly.Replace('\\', '_');
                    NavigateTo = "";
                    TaskCompleted = true;
                    TaskCompletionWithError = true;
                    TaskCompletionMessage = "Attempt to associate a QVW signed for group '" + ProjectPathGroupOnly +"' to a project associated with group '" + Group + "'.  Associating acrosss groups is not allowed.<br> You can rename the group on the production server by clicking 'Rename Group' button below";
                    AbortTask = true;
                    ShowRenameButton = true;
                    base.TaskProcessor(parms);
                    return;
                }

                DisplayMessage = "Check group availability on production server";
                //check to see if group is available, if not, must import first
                bool GroupAvailable = false;
                foreach (string S in Groups)
                {
                    if (string.Compare(S, Group, true) == 0)
                    {
                        GroupAvailable = true;
                        break;
                    }
                }

                if (GroupAvailable == false)
                {
                    NavigateTo = "errors/groupnotavailable.html";
                    TaskCompleted = true;
                    TaskCompletionWithError = true;
                    TaskCompletionMessage = "Requested group '" + Group + "' is not available on the server.";
                    AbortTask = true;
                    base.TaskProcessor(parms);
                    return;
                }

                ContainsHierarchy = false;
                DisplayMessage = "Checking signature for client administration information";
                if (XMLFS.SignatureDictionary.ContainsKey("can_emit") == true)
                {
                    ContainsHierarchy = System.Convert.ToBoolean(XMLFS.SignatureDictionary["can_emit"]);
                }
                if (QVWNotSigned)
                    TaskCompletionMessage = "Association complete - QVW did not have a signature, but was associated to group - " + Group;
                else if (ContainsHierarchy)
                    TaskCompletionMessage = "Association complete - Signature verified/contains client administration information";
                else
                    TaskCompletionMessage = "Association complete - Signature verified/does not contain information for client administration";

                base.TaskProcessor(parms);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //report testing
            //Response.Redirect("../ComplexReporting/ReportingShell.aspx");
            //return;

            if (!IsPostBack)
            {
                if (Request["Processing"] == null)
                {
                    string ProjectPath = Session["ProjectPath"].ToString();
                    SignatureVerificationClass SVC = new SignatureVerificationClass();
                    SVC.QualafiedProjectFile = ProjectPath;
                    Global.TaskManager.ScheduleTask(SVC);
                    SVC.StartTask();
                    SignatureVerifications.Text = "Starting verification.....";
                    Response.Redirect("SignatureVerification.aspx?Processing=" + SVC.TaskID);
                }
                else
                {
                    string ProcessID = Request["Processing"].ToString();
                    SignatureVerificationClass SVC = Global.TaskManager.FindTask(ProcessID) as SignatureVerificationClass;
                    if (SVC != null)
                    {
                        SignatureVerifications.Text = SVC.DisplayMessage;
                        if (SVC.TaskCompletionWithError)
                        {
                            SignatureVerifications.Text = SVC.TaskCompletionMessage;
                            SVC.AbortTask = true;
                            Global.TaskManager.DeleteTask(SVC.TaskID);
                        }
                        else if ((SVC.TaskCompleted) && (SVC.ShowRenameButton))
                        {
                            SignatureVerifications.Text = SVC.TaskCompletionMessage;
                            RenameGroup.Visible = true;
                            Global.TaskManager.DeleteTask(SVC.TaskID);
                        }
                        else if ( (SVC.TaskCompleted) && (SVC.ContainsHierarchy) )
                        {
                            SignatureVerifications.Text = SVC.TaskCompletionMessage;
                            Global.TaskManager.DeleteTask(SVC.TaskID);
                            MillimanCommon.Alert.DelayedNavigation("IndexDownloader.aspx", 3);
                        }
                        else if ((SVC.TaskCompleted) && (SVC.ContainsHierarchy == false))
                        {
                            SignatureVerifications.Text = SVC.TaskCompletionMessage; //"Association complete - this QVW does not contain hierarchy information for client administration";
                            SignatureVerifications.Visible = true;
                            Global.TaskManager.DeleteTask(SVC.TaskID);
                            Image1.Visible = false;
                        }
                        else
                        {
                            SignatureVerifications.Text = SVC.DisplayMessage;
                            MillimanCommon.Alert.Refresh(3);
                        }
                    }
                    else
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "System error - missing task");
                        Response.Redirect("errors/missingtask.aspx");
                        SignatureVerifications.Text = "Could not retrieve processing status.";
                    }
                }
            }
        }

        protected void RenameGroup_Click(object sender, EventArgs e)
        {
            //VWN implement rename group
            MillimanCommon.Alert.Show("RENAME GROUP");
        }
    }
}