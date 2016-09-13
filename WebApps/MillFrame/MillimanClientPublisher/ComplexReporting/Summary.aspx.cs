using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClientPublisher
{
    public partial class Summary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.AppendHeader("Refresh", "15");
            if (!IsPostBack)
            {
                if (Session["ProjectPath"] != null)
                {
                    //string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                    //DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));

                    MillimanCommon.ProjectSettings LocalPS = MillimanCommon.ProjectSettings.Load(Session["ProjectPath"].ToString());
                    string QVWQualfiedPath = System.IO.Path.Combine(LocalPS.AbsoluteProjectPath, LocalPS.QVName + ".qvw");
                  
                    if (LocalPS != null) 
                    {
                        ProjectName.Text = LocalPS.ProjectName;
                        ProjectDescription.Text = LocalPS.QVDescription;

                        //PublicationDirectory.Text = LocalPS.Groups.Replace('_', '\\');  //Groups and PublicationDirectory are the same
                        QVWGeneratedOn.Text = LocalPS.LastProjectRunDate;
                        QVWGeneratedBy.Text = LocalPS.LastProjectRun;
                        QVWFile.Text = LocalPS.QVName;

                        MillimanCommon.XMLFileSignature XMLFS = new MillimanCommon.XMLFileSignature(QVWQualfiedPath);
                        bool ContainsHierarchy = false;
                        if (XMLFS.SignatureDictionary.ContainsKey("can_emit") == true)
                        {
                            ContainsHierarchy = System.Convert.ToBoolean(XMLFS.SignatureDictionary["can_emit"]);
                        }
                        if (ContainsHierarchy == false)
                        {
                            LoopNReduce.Text = "Off";
                        }
                        else
                        {
                            int TotalUsers = 0;
                            int ReducedUsers = 0;
                            MillimanCommon.QVWReportBankProcessor RBP = new MillimanCommon.QVWReportBankProcessor(Session["ProjectPath"].ToString());
                            if (RBP.SummaryReductionStats(out TotalUsers, out ReducedUsers) == true)
                                LoopNReduce.Text = "QVW reduced for " + ReducedUsers.ToString() + " out of " + TotalUsers.ToString() + " users";
                            else
                                LoopNReduce.Text = "No reduction statisics available";
                        }
                        ServerUpdatedBy.Text = LocalPS.UploadedToProduction == "" ? "Never" : LocalPS.UploadedToProduction;
                        ServerUpdatedOn.Text = LocalPS.UploadedToProductionDate == "" ? "NA" : LocalPS.UploadedToProductionDate;

                        string URL =  System.IO.Path.Combine(LocalPS.LoadedFromPath, LocalPS.QVThumbnail);

                        Thumbnail.ImageUrl = "../ImageReflector.aspx?key=" + MillimanCommon.Utilities.ConvertStringToHex(URL);
                        if (System.IO.File.Exists(System.IO.Path.Combine(LocalPS.LoadedFromPath, LocalPS.QVThumbnail)) == false)
                        {
                            ThumbnailStatus.ImageUrl = "~/images/stop-icon.png";
                            ThumbnailStatus.ToolTip = "Image file is missing - please reload thumbnail";
                        }
                        URL = MillimanCommon.Utilities.ConvertStringToHex(System.IO.Path.Combine(LocalPS.LoadedFromPath, LocalPS.UserManual));

                        UserManual.Text = LocalPS.UserManual;
                        UserManual.NavigateUrl = "DocumentReflector.asp?key=" + URL;

                        //string[] GroupList= LocalPS.Groups.Split(new char[] {'~'}, StringSplitOptions.RemoveEmptyEntries );
                        //foreach (string GroupName in GroupList)
                        //{
                        //    Groups.Items.Add(GroupName);
                        //}

                        //now look at data and hightlight
                        if (string.IsNullOrEmpty(ProjectName.Text))
                        {
                            ProjectNameStatus.ImageUrl = "~/images/stop-icon.png";
                            ProjectNameStatus.ToolTip = "Project name is required.";
                        }
                        if (string.IsNullOrEmpty(ProjectDescription.Text))
                        {
                            ProjectDescriptionStatus.ImageUrl = "~/images/stop-icon.png";
                            ProjectDescriptionStatus.ToolTip = "Project description is required.";
                        }
                        //if (string.IsNullOrEmpty(PublicationDirectory.Text))
                        //{
                        //    PublicationDirectoryStatus.ImageUrl = "~/images/stop-icon.png";
                        //    PublicationDirectoryStatus.ToolTip = "Publication directory is required";
                        //}
                        if (string.IsNullOrEmpty(UserManual.Text))
                        {
                            UserManualStatus.ImageUrl = "~/images/warning-icon.png";
                            UserManualStatus.ToolTip = "No user manual was specified - a users manual is not required.";
                        }
                        else if (System.IO.File.Exists(System.IO.Path.Combine(LocalPS.LoadedFromPath, LocalPS.UserManual)) == false)
                        {
                            UserManualStatus.ImageUrl = "~/images/stop-icon.png";
                            UserManualStatus.ToolTip = "The user manual file is missing, please reload the user manual.";                   
                        }
                        //if (Groups.Items.Count == 0)
                        //{
                        //    GroupsStatus.ImageUrl = "~/images/warning-icon.png";
                        //    GroupsStatus.ToolTip = "Groups are not required, but are typically provided.";
                        //}
                        if (string.IsNullOrEmpty(QVWFile.Text))
                        {
                            QVWFileStatus.ImageUrl = "~/images/stop-icon.png";
                            QVWFileStatus.ToolTip = "A QVW file is not available to upload - please 'Re-Load' the project with data to create a QVW";
                        }
                        //this is a new project so no "QVW" but just a "QVW_NEW"
                        else if (System.IO.File.Exists(System.IO.Path.Combine(LocalPS.LoadedFromPath, LocalPS.QVName + ".qvw")) == false)
                        {
                            QVWFileStatus.ImageUrl = "~/images/stop-icon.png";
                            QVWFileStatus.ToolTip = "The QVW file is missing - please 'Re-Load' the project with data to create a QVW";
                        }
                    }
                }
            }
        }

        protected void PushAccepted_Click(object sender, EventArgs e)
        {
            Response.Redirect("PushToProduction.aspx?ProjectPath=" + Session["ProjectPath"].ToString());

        }

        protected override void Render(HtmlTextWriter writer)
        {
            
            //save html out to cache
            System.Text.StringBuilder sbOut = new System.Text.StringBuilder();
            try
            {
                System.IO.StringWriter swOut = new System.IO.StringWriter(sbOut);
                HtmlTextWriter htwOut = new HtmlTextWriter(swOut);
                base.Render(htwOut);
                string HTMLOutput = sbOut.ToString();
                Session["PromptView"] = ProcessHTMLForEmail(HTMLOutput);
                // Send sOut as an Email
            }
            catch (Exception)
            {
            }

            writer.Write(sbOut);

        }

        private string ProcessHTMLForEmail(string HTML)
        {
            int    StyleIndex = HTML.IndexOf("<style");
            int    StyleLength = (HTML.IndexOf("</style>") - StyleIndex) + "</style>".Length;
            string StyleInfo = HTML.Substring(StyleIndex, StyleLength);

            int TableIndex = HTML.IndexOf("<table");
            int TableLength = (HTML.IndexOf("</table>") - TableIndex) + "</table>".Length;
            string TableInfo = HTML.Substring(TableIndex, TableLength);

            //replace caption and reduce size
            TableInfo = TableInfo.Replace("Verify the publishing information before you continue!", "Uploaded to [" + System.Configuration.ConfigurationManager.AppSettings["FriendlyServerName"] + "] server on " + DateTime.Now.ToString() + " by " + System.Web.Security.Membership.GetUser().UserName);
            TableInfo = TableInfo.Replace("16px;", "11px;");

            //Thumbnail.ImageUrl = "ImageReflector.aspx?key=" + URL;
            string ThumbnailURL = Thumbnail.ImageUrl.Substring(Thumbnail.ImageUrl.IndexOf('=')+1);
            ThumbnailURL = MillimanCommon.Utilities.ConvertHexToString(ThumbnailURL);
            string ThumbnailImage = MillimanCommon.InlineImage.Inline(ThumbnailURL,false);

            string OKImage = MillimanCommon.InlineImage.Inline("../images/OK-icon.png");
            string WarningImage = MillimanCommon.InlineImage.Inline("../images/warning-icon.png");
            string StopImage = MillimanCommon.InlineImage.Inline("../images/stop-icon.png");

            TableInfo = TableInfo.Replace(Thumbnail.ImageUrl, ThumbnailImage);
            TableInfo = TableInfo.Replace("../images/OK-icon.png", OKImage);
            TableInfo = TableInfo.Replace("../images/warning-icon.png", WarningImage);
            TableInfo = TableInfo.Replace("../images/stop-icon.png", StopImage);

            string NewHTML = "<!DOCTYPE html><html xmlns=\"http://www.w3.org/1999/xhtml\"><head>";
            NewHTML += StyleInfo;
            NewHTML += "</head><body bgcolor='white'>";
            NewHTML += TableInfo;
            NewHTML += "</body></html>";

            return NewHTML;
        }
    }
}