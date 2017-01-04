using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.Admin
{
    public partial class Diff : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.AppendHeader("Refresh", "15");
            if (!IsPostBack)
            {
                if (Request["ProjectPath"] != null)
                {
                    string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                    DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));
                    MillimanCommon.ProjectSettings LocalPS = MillimanCommon.ProjectSettings.Load(System.IO.Path.Combine(DocumentRoot, Request["ProjectPath"]));
                    if (LocalPS == null)
                    {
                        MillimanCommon.Alert.Show("Did not find project '" + Request["ProjectPath"] + "'.");
                        return;
                    }
                    MillimanCommon.ProjectSettings ServerPS = ServerProject(System.IO.Path.Combine( LocalPS.VirtualDirectory, LocalPS.ProjectName + ".hciprj"));
                    SetAllToIcon("~/images/transparent16x16.gif", "");

                    string ServerThumbnailMD5 = "";
                    string LocalThumbnailMD5 = "";
                    string ServerManualMD5 = "";
                    string LocalManualMD5 = "";

                    //try and set the server PS defaults first - this could be null if never uploaded to server
                    if (ServerPS != null)
                    {
                        ServerProjectName.Text = ServerPS.ProjectName;
                        ServerProjectDescription.Text = ServerPS.QVDescription;
                        ServerPublicationDirectory.Text = LocalPS.VirtualDirectory;  //this must be the same since we load a mirror image so LocalPS is correct
                        ServerQVWGeneratedOn.Text = ServerPS.LastProjectRunDate;
                        ServerQVWGeneratedBy.Text = ServerPS.LastProjectRun;
                        ServerQVWFile.Text = ServerPS.QVName;
                        //start diffing

                        if (string.IsNullOrEmpty(ServerPS.MillimanControlName) == true)
                            ServerLoopNReduce.Text = "None";
                        else
                            ServerLoopNReduce.Text = "QVW reduced on list '" + ServerPS.MillimanControlName + "' and associated to Covisint runtime variable '" + ServerPS.CovisintFieldName + "'";

 
                        ServerServerUpdatedBy.Text = string.IsNullOrEmpty(ServerPS.UploadedToProduction) == true ? "Never" : ServerPS.UploadedToProduction;
                        ServerServerUpdatedOn.Text = string.IsNullOrEmpty(ServerPS.UploadedToProductionDate) == true ? "NA" : ServerPS.UploadedToProductionDate;

                        string ServerThumb = System.IO.Path.Combine(LocalPS.VirtualDirectory, ServerPS.QVThumbnail);

                        string URL = GetServerFile(ServerThumb);  //MillimanCommon.Utilities.ConvertStringToHex(System.IO.Path.Combine(ServerPS.LoadedFromPath, ServerPS.QVThumbnail));
                        if (string.IsNullOrEmpty(URL) == false)
                            ServerThumbnailMD5 = MillimanCommon.Utilities.CalculateMD5Hash(URL, true);

                        ServerThumbnail.ImageUrl = "ImageReflector.aspx?key=" + MillimanCommon.Utilities.ConvertStringToHex( URL );
                        if (System.IO.File.Exists(System.IO.Path.Combine(URL)) == false)
                        {
                            ThumbnailStatus.ImageUrl = "~/images/notequal.png";
                            ThumbnailStatus.ToolTip = "Server image file is missing - please reload thumbnail";
                        }

                        if (string.IsNullOrEmpty(ServerPS.UserManual) == false)
                        {
                            string ServerManualDownload = System.IO.Path.Combine( LocalPS.VirtualDirectory, ServerPS.UserManual);
                            URL = GetServerFile(ServerManualDownload); //MillimanCommon.Utilities.ConvertStringToHex(System.IO.Path.Combine(ServerPS.LoadedFromPath, ServerPS.UserManual));                      

                            //sk:VSTS item #1862; if url is empty try _new file. Part 1 of 2
                            if (URL == String.Empty)
                            {
                                ServerManualDownload = System.IO.Path.Combine(LocalPS.VirtualDirectory, ServerPS.UserManual + "_new");
                                URL = GetServerFile(ServerManualDownload); //MillimanCommon.Utilities.ConvertStringToHex(System.IO.Path.Combine(ServerPS.LoadedFromPath, ServerPS.UserManual));
                            }

                            ServerUserManual.Text = ServerPS.UserManual;
                            if (string.IsNullOrEmpty(URL) == false)
                                ServerManualMD5 = MillimanCommon.Utilities.CalculateMD5Hash(URL, true);

                            ServerUserManual.NavigateUrl = "DocumentReflector.asp?key=" + MillimanCommon.Utilities.ConvertStringToHex(URL);
                        }

                        string[] GroupList = LocalPS.Groups.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string GroupName in GroupList)
                        {
                            ServerGroups.Items.Add(GroupName);
                        }
                    }
                    else  //Server side is null, never uploaded so set defaults
                    {
                        ServerLoopNReduce.Text = "None";
                        ServerServerUpdatedBy.Text = "Never";
                        ServerServerUpdatedOn.Text = "NA";
                        LocalThumbnail.ImageUrl = "~/images/transparent16x16.gif";
                    }
                
                    if (LocalPS != null) //is ok if ServerPS is null may not have been uploaded yet
                    {
                        LocalProjectName.Text = LocalPS.ProjectName;
                        LocalProjectDescription.Text = LocalPS.QVDescription;
                        LocalPublicationDirectory.Text = LocalPS.VirtualDirectory; //trim down to virtual dir
                        LocalQVWGeneratedOn.Text = LocalPS.LastProjectRunDate;
                        LocalQVWGeneratedBy.Text = LocalPS.LastProjectRun;
                        LocalQVWFile.Text = LocalPS.QVName;
                        //start diffing

                        if (string.IsNullOrEmpty(LocalPS.MillimanControlName) == true)
                            LocalLoopNReduce.Text = "None";
                        else
                            LocalLoopNReduce.Text = "QVW reduced on list '" + LocalPS.MillimanControlName + "' and associated to Covisint runtime variable '" + LocalPS.CovisintFieldName + "'";


                        LocalServerUpdatedBy.Text = string.IsNullOrEmpty(LocalPS.UploadedToProduction) == true ? "Never" : LocalPS.UploadedToProduction;
                        LocalServerUpdatedOn.Text = string.IsNullOrEmpty(LocalPS.UploadedToProductionDate) == true ? "NA" : LocalPS.UploadedToProductionDate;

                        string URL = MillimanCommon.Utilities.ConvertStringToHex(System.IO.Path.Combine(LocalPS.LoadedFromPath, LocalPS.QVThumbnail));
                        if (string.IsNullOrEmpty(URL) == false)
                            LocalThumbnailMD5 = MillimanCommon.Utilities.CalculateMD5Hash(System.IO.Path.Combine(LocalPS.LoadedFromPath, LocalPS.QVThumbnail), true);

                        LocalThumbnail.ImageUrl = "ImageReflector.aspx?key=" + URL;
                        if (System.IO.File.Exists(System.IO.Path.Combine(LocalPS.LoadedFromPath, LocalPS.QVThumbnail)) == false)
                        {
                            ThumbnailStatus.ImageUrl = "~/images/stop-icon.png";
                            ThumbnailStatus.ToolTip = "Image file is missing - please reload thumbnail";
                        }

                        if (string.IsNullOrEmpty(LocalPS.UserManual) == false)
                        {
                            URL = System.IO.Path.Combine(LocalPS.LoadedFromPath, LocalPS.UserManual);
                            LocalUserManual.Text = LocalPS.UserManual;
                            if (string.IsNullOrEmpty(URL) == false)
                            {
                                LocalManualMD5 = MillimanCommon.Utilities.CalculateMD5Hash(URL, true);

                                //sk:VSTS item #1862; if Md5 is an empty string try _new file. Part 1 of 2
                                if (LocalManualMD5 == String.Empty)
                                {
                                    URL = System.IO.Path.Combine(LocalPS.LoadedFromPath, LocalPS.UserManual + "_new");
                                    LocalUserManual.Text = LocalPS.UserManual;
                                    if (string.IsNullOrEmpty(URL) == false)
                                    {
                                        LocalManualMD5 = MillimanCommon.Utilities.CalculateMD5Hash(URL, true);
                                    }
                                }
                            }
                            LocalUserManual.NavigateUrl = "DocumentReflector.asp?key=" + MillimanCommon.Utilities.ConvertStringToHex(URL);
                        }
                        string[] GroupList = LocalPS.Groups.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string GroupName in GroupList)
                        {
                            LocalGroups.Items.Add(GroupName);
                        }

                    }

                    
                    //we loaded all the defaults,  so attempt to diff stuff
                    if (string.Compare(LocalProjectName.Text, ServerProjectName.Text, true) != 0)
                    {
                        ProjectNameStatus.ImageUrl = "~/images/notequal.png";
                        ProjectNameStatus.ToolTip = "Project names is different";
                    }
                    else
                    {
                        ProjectNameStatus.ImageUrl = "~/images/equal.png";
                        ProjectNameStatus.ToolTip = "Project names are the same";
                    }

                    if (string.Compare(LocalProjectDescription.Text, ServerProjectDescription.Text, true) != 0)
                    {
                        ProjectDescriptionStatus.ImageUrl = "~/images/notequal.png";
                        ProjectDescriptionStatus.ToolTip = "Project descriptions are different.";
                    }
                    else
                    {
                        ProjectDescriptionStatus.ImageUrl = "~/images/equal.png";
                        ProjectDescriptionStatus.ToolTip = "Project descriptions are the same.";
                    }
                    if (string.Compare(LocalPublicationDirectory.Text, ServerPublicationDirectory.Text, true) != 0)
                    {
                        PublicationDirectoryStatus.ImageUrl = "~/images/notequal.png";
                        PublicationDirectoryStatus.ToolTip = "Publication directorys are different.";
                    }
                    else
                    {
                        PublicationDirectoryStatus.ImageUrl = "~/images/equal.png";
                        PublicationDirectoryStatus.ToolTip = "Publication directory are the same.";
                    }
                    if (string.Compare(ServerManualMD5, LocalManualMD5, true) != 0)
                    {
                        UserManualStatus.ImageUrl = "~/images/notequal.png";
                        UserManualStatus.ToolTip = "User manuals are different.";
                    }
                    else
                    {
                        UserManualStatus.ImageUrl = "~/images/equal.png";
                        UserManualStatus.ToolTip = "User manuals are the same.";
                    }

                    List<string> LGItems = LocalGroups.Items.OfType<string>().ToList();
                    List<string> RGItems = ServerGroups.Items.OfType<string>().ToList();

                    if ((LGItems.Count > 0) || (RGItems.Count > 0))
                    {
                        bool ListsSame = true;
                        foreach (string S in LGItems)
                        {
                            if (RGItems.Contains(S, StringComparer.OrdinalIgnoreCase) == false)
                                ListsSame = false;
                        }
                        foreach (string S in RGItems)
                        {
                            if (LGItems.Contains(S, StringComparer.OrdinalIgnoreCase) == false)
                                ListsSame = false;
                        }
                        if (ListsSame == false)
                        {
                            GroupsStatus.ImageUrl = "~/images/notequal.png";
                            GroupsStatus.ToolTip = "Groups have different content";
                        }
                        else
                        {
                            GroupsStatus.ImageUrl = "~/images/equal.png";
                            GroupsStatus.ToolTip = "Groups lists are the same.";
                        }
                    }
                    else
                    {
                        GroupsStatus.ImageUrl = "~/images/equal.png";
                        GroupsStatus.ToolTip = "Groups lists are the same.";
                    }
                    if (string.IsNullOrEmpty(LocalThumbnailMD5) == false)
                    {
                        if (string.Compare(LocalThumbnailMD5, ServerThumbnailMD5, true) == 0)
                        {
                            ThumbnailStatus.ImageUrl = "~/images/equal.png";
                            ThumbnailStatus.ToolTip = "Icons are the same.";
                        }
                        else
                        {
                            ThumbnailStatus.ImageUrl = "~/images/notequal.png";
                            ThumbnailStatus.ToolTip = "Icons are different.";
                        }
                    }
                    if ((string.IsNullOrEmpty(LocalQVWFile.Text) == false) && (string.IsNullOrEmpty(ServerQVWFile.Text) == false))
                    {
                        string LocalQVW = LocalPS.LoadedFrom.Replace(LocalPS.ProjectName + ".hciprj", LocalPS.QVName + ".qvw");
                        string LMD5 = MillimanCommon.Utilities.CalculateMD5Hash(LocalQVW, true);
                        //use local virtual dir below, since it's populated from a mirror on production
                        string ServerQVW = System.IO.Path.Combine( LocalPS.VirtualDirectory, LocalPS.QVName + ".qvw");
                        string SMD5 = Global.GetInstance().GetHash( ServerQVW );
                        if (string.Compare(LMD5, SMD5, true) != 0)
                        {
                            QVWFileStatus.ImageUrl = "~/images/notequal.png";
                            QVWFileStatus.ToolTip = "QVW files are not the same";
                        }
                        else
                        {
                            QVWFileStatus.ImageUrl = "~/images/equal.png";
                            QVWFileStatus.ToolTip = "QVW files are the same";
                        }
                    }

                    if (ServerPS != null)
                    {
                        if (string.Compare(LocalPS.LastProjectRunDate, ServerPS.LastProjectRunDate, true) == 0)
                        {
                            QVWGeneratedOnStatus.ImageUrl = "~/images/equal.png";
                            QVWGeneratedOnStatus.ToolTip = "Last project run dates are the same.";
                        }
                        else
                        {
                            QVWGeneratedOnStatus.ImageUrl = "~/images/notequal.png";
                            QVWGeneratedOnStatus.ToolTip = "Last project run dates are not the same.";
                        }
                        if (string.Compare(LocalPS.LastProjectRun, ServerPS.LastProjectRun, true) == 0)
                        {
                            QVWGeneratedByStatus.ImageUrl = "~/images/equal.png";
                            QVWGeneratedByStatus.ToolTip = "Last person to run the project are the same.";
                        }
                        else
                        {
                            QVWGeneratedByStatus.ImageUrl = "~/images/notequal.png";
                            QVWGeneratedByStatus.ToolTip = "Last person to run the project are different.";
                        }



                        if (string.Compare(LocalPS.UploadedToProductionDate, ServerPS.UploadedToProductionDate, true) == 0)
                        {
                            ServerUpdatedOnStatus.ImageUrl = "~/images/equal.png";
                            ServerUpdatedOnStatus.ToolTip = "Server updated status is the same";
                        }
                        else
                        {
                            ServerUpdatedOnStatus.ImageUrl = "~/images/notequal.png";
                            ServerUpdatedOnStatus.ToolTip = "Server updated status is different.";
                        }

                        if (string.Compare(LocalPS.UploadedToProduction, ServerPS.UploadedToProduction, true) == 0)
                        {
                            ServerUpdatedByStatus.ImageUrl = "~/images/equal.png";
                            ServerUpdatedByStatus.ToolTip = "Server was last updated 'by' is the same";
                        }
                        else
                        {
                            ServerUpdatedByStatus.ImageUrl = "~/images/notequal.png";
                            ServerUpdatedByStatus.ToolTip = "Server was last updated 'by' is different.";
                        }
                    }
                    //just set these, we don't use them yet
                    LoopNReduceStatus.ImageUrl = "~/images/equal.png";

                    //LocalPS should never be null,  but ServerPS can be, if so set all icons to NA
                    if (( LocalPS == null ) || (ServerPS == null) )
                    {
                        SetAllToIcon("~/images/na.png", "Project has not been uploaded to production server.");
                    }
                }
            }
        }

        private void SetAllToIcon(string Icon, string Tooltip = "")
        {
            QVWFileStatus.ImageUrl = Icon;
            QVWFileStatus.ToolTip = Tooltip;
            ProjectNameStatus.ImageUrl = Icon;
            ProjectNameStatus.ToolTip = Tooltip;
            ProjectDescriptionStatus.ImageUrl = Icon;
            ProjectDescriptionStatus.ToolTip = Tooltip;
            ThumbnailStatus.ImageUrl = Icon;
            ThumbnailStatus.ToolTip = Tooltip;
            UserManualStatus.ImageUrl = Icon;
            UserManualStatus.ToolTip = Tooltip;
            PublicationDirectoryStatus.ImageUrl = Icon;
            PublicationDirectoryStatus.ToolTip = Tooltip;
            GroupsStatus.ImageUrl = Icon;
            GroupsStatus.ToolTip = Tooltip;
            QVWGeneratedOnStatus.ImageUrl = Icon;
            QVWGeneratedOnStatus.ToolTip = Tooltip;
            QVWGeneratedByStatus.ImageUrl = Icon;
            QVWGeneratedByStatus.ToolTip = Tooltip;
            ServerUpdatedOnStatus.ImageUrl = Icon;
            ServerUpdatedOnStatus.ToolTip = Tooltip;
            ServerUpdatedByStatus.ImageUrl = Icon;
            ServerUpdatedByStatus.ToolTip = Tooltip;
            LoopNReduceStatus.ImageUrl = Icon;
            LoopNReduceStatus.ToolTip = Tooltip;
        }

        /// <summary>
        /// Retrieve the file from the server and load into project
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        private MillimanCommon.ProjectSettings ServerProject(string Path)
        {
            string RemoteProject = GetServerFile(Path);
            if (string.IsNullOrEmpty(RemoteProject) == false)
            {
                return MillimanCommon.ProjectSettings.Load(RemoteProject);
            }
            return null;
        }

        /// <summary>
        /// Fetch the requested from from server
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        private string GetServerFile(string Path)
        {
            byte[] ServerFile = Global.GetInstance().Get(Path);
            if (ServerFile != null)
            {
                string Temp = System.IO.Path.GetTempFileName();
                using (System.IO.BinaryWriter b = new System.IO.BinaryWriter(System.IO.File.Open(Temp, System.IO.FileMode.Append)))
                {
                    b.Write(ServerFile);
                }
                return Temp;
            }
            return "";
        }
        /// <summary>
        /// Diff the two files
        /// </summary>
        /// <param name="Path1"></param>
        /// <param name="Path2"></param>
        /// <returns></returns>
        private bool DiffFiles(string Path1, string Path2)
        {

            string MD51 = MillimanCommon.Utilities.CalculateMD5Hash(Path1, true);
            string MD52 = MillimanCommon.Utilities.CalculateMD5Hash(Path2, true);

            return (string.Compare(MD51, MD52, true) == 0);
        }
    }
}