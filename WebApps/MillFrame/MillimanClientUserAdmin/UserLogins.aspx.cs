using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace MillimanClientUserAdmin
{
    public partial class UserLogins : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            EnableViewState = false;

            if (!IsPostBack)
            {
                if (Session["ProjectPath"] != null)
                {
                    string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                    DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));

                    MillimanCommon.ProjectSettings LocalPS = MillimanCommon.ProjectSettings.Load(Session["ProjectPath"].ToString());

                    List<string> ColumnNames = null;
                    List<List<string>> SelectedItems = DisplayProcessor(LocalPS, out ColumnNames);
                    if (SelectedItems.Count > 0)
                    {
                        MillimanCommon.UI.DynamicTable DynTable = new MillimanCommon.UI.DynamicTable();
                        DynTable.CreateTable2DBinder("User View Selection Criteria", ColumnNames, SelectedItems, PlaceHolder1, System.Drawing.Color.FromArgb(120, 240, 240, 240));
                    }
                    else
                    {  //no new items
                        Label UserMessage = new Label();
                        UserMessage.Style.Add("width", "100%");
                        UserMessage.Text = "<br><br><center>No selections have been made to limit user views within the report.</center>";
                        PlaceHolder1.Controls.Add(UserMessage);
                    }

                }
            }
        }

        /// <summary>
        /// Process the values such that no value is repeated on multiple rows concurrently
        /// </summary>
        /// <param name="DisplayItems"></param>
        /// <returns></returns>
        private List<List<string>> DisplayProcessor(MillimanCommon.ProjectSettings ProjectSettings, out List<string> ColumnNames )
        {
            string QVLogDirectory = System.Configuration.ConfigurationManager.AppSettings["QVLogDirectory"];
            string ShowLoginsForLastXDays = System.Configuration.ConfigurationManager.AppSettings["ShowLoginsForLastXDays"];
            int ShowLogsFor = System.Convert.ToInt32(ShowLoginsForLastXDays);
            List<string> LogFiles = System.IO.Directory.GetFiles(QVLogDirectory, "Session*.*").ToList();
            DateTime OldestCandidateDate = DateTime.Now.AddDays(-1 * ShowLogsFor);
            DateTime FileDate;
            for( int Index = LogFiles.Count()-1; Index >= 0; Index--)
            {
                string FilenameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(LogFiles[Index]);
                string sFileDate = FilenameWithoutExt.Substring(FilenameWithoutExt.LastIndexOf('_')+1);
                if (DateTime.TryParse(sFileDate, out FileDate))
                {
                    if (DateTime.Compare(OldestCandidateDate, FileDate) > 0)
                        LogFiles.RemoveAt(Index);  //too old get rid of it
                }
                else
                {  //don't know format, get rid of it
                    LogFiles.RemoveAt(Index);
                }
            }
            LogFiles.Sort();  //sort files

            //D:\InstalledApplications\PRM\FULLBackups_Logs\QVLogs\Sessions_INDY-PRM-1_2015-08-12.txt

            Dictionary<string, List<string>> ProcessedFileItems = new Dictionary<string, List<string>>();

            List<string> UsersInRoles =  Roles.GetUsersInRole(ProjectSettings.Groups).ToList();
            for (int Index = UsersInRoles.Count - 1; Index >= 0; Index-- )
            {  //remove all the admin people
                if (Roles.IsUserInRole(UsersInRoles[Index], "Administrator"))
                    UsersInRoles.RemoveAt(Index);
                else
                    ProcessedFileItems.Add(UsersInRoles[Index].ToLower(), new List<string>());  //add the user, and allocate a list
            }

            string ProcessedGroup = ProjectSettings.Groups.Replace('_', '\\').ToLower();
            string AccessTime = "";
            string Report = "";
            string Duration = "";
            string Account = "";
            foreach (string LogFile in LogFiles.Reverse<string>())  //make current times at top of lists
            {
                List<string> LogFileData = System.IO.File.ReadAllLines(LogFile).ToList();
                LogFileData.RemoveAt(0);  //get rid of header
                foreach (string LogEntry in LogFileData)
                {
                    string[] LogTokens = LogEntry.Split(new char[] { '\t' });
                    AccessTime = LogTokens[3];
                    Report = LogTokens[4];
                    Duration = LogTokens[9];
                    Account = LogTokens[15].ToLower();
                    Account = Account.Substring(Account.IndexOf('\\') + 1);
                    if ( (ProcessedFileItems.ContainsKey(Account)) && (Report.ToLower().Contains(ProcessedGroup)))
                    {
                        ProcessedFileItems[Account].Add(AccessTime + "|" + Duration);
                    }
                }
            }

            //let's flatten the dictionary to something we can render into a table
            ColumnNames = new List<string>() { "Account (access for last " + ShowLoginsForLastXDays + " days)", "Login Time(EST)", "Duration(minutes)" };
            List<List<string>> LoginInfo = new List<List<string>>();
            foreach (KeyValuePair<string, List<string>> UserItem in ProcessedFileItems.Reverse())
            {
                for (int Index = 0; Index < UserItem.Value.Count; Index++)
                {
                    List<string> ReportItems = new List<string>();
                    if (Index == 0)
                    {
                        ReportItems.Add(UserItem.Key);
                    }
                    else
                    {
                        ReportItems.Add("");
                    }
                    string[] ReportSubItems = UserItem.Value[Index].ToString().Split(new char[] { '|' });
                    ReportItems.Add(ReportSubItems[0]);
                    ReportItems.Add(ReportSubItems[1]);
                    LoginInfo.Add(ReportItems);
                }
            }

            return LoginInfo;
        }
    }
}