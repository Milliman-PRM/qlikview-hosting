using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.IO;

namespace ClientPublisher
{
    public partial class ProjectEditor : System.Web.UI.Page
    {


        public string WorkingDirectory
        {
            get {


                ProjectSettingsExtension theProject = CurrentProject;
                if (theProject == null)
                    return "";

                if (System.Web.Security.Membership.GetUser() == null)
                {
                    Response.Redirect("HTML/NotAuthorizedIssue.html");
                    return "";
                }
                return PublisherUtilities.GetWorkingDirectory(System.Web.Security.Membership.GetUser().UserName, theProject.ProjectName);
            }
        }

        public ProjectSettingsExtension CurrentProject
        {
            get
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
                    return null;
                }
                return theProject;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //clear out an residual files that might be there
                string[] AllFiles = System.IO.Directory.GetFiles(WorkingDirectory);
                foreach (string aFile in AllFiles)
                    System.IO.File.Delete(aFile);

                LoadDataToUI();
            }
         
        }

        /// <summary>
        /// Merge data from local dir and QVW project
        /// </summary>
        private void LoadDataToUI( )
        {
            //all this to check all params, if something is wrong this properity will redirect us
            string WD = WorkingDirectory;
            ProjectSettingsExtension PSE = CurrentProject;

            if ((string.IsNullOrEmpty(WD) == false) && (PSE != null))
            {
                string QVRoot = System.Web.Configuration.WebConfigurationManager.AppSettings["QVDocumentRoot"];
                string StrictReductionGroups = System.Web.Configuration.WebConfigurationManager.AppSettings["StrictReductionRuleGroups"];
                string AutoInclusionGroups = System.Web.Configuration.WebConfigurationManager.AppSettings["AutoInclusionGroups"];
                
                if (string.IsNullOrEmpty(StrictReductionGroups) == false)
                    StrictReductionGroups = StrictReductionGroups.ToLower();
                //first we look to see if there is a version in the local publisher root, otherwise we use the value from the
                //original project

                QVW.Text = System.IO.Path.GetFileName(PSE.OriginalProjectName);
                if (string.IsNullOrEmpty(QVW.Text))
                    QVW.Text = System.IO.Path.GetFileName(PSE.QVName);

                string LocalIcon = Path.Combine(WD, PSE.QVThumbnail);
                if ( System.IO.File.Exists(LocalIcon))
                {
                    //local version
                    string Key = MillimanCommon.Utilities.ConvertStringToHex(System.IO.Path.Combine(WD, PSE.QVThumbnail));
                    Icon.ImageUrl = "reflector.ashx?key=" + Key;
                }
                else
                {
                    //published version
                    Icon.ImageUrl = PSE.QVIconReflector;
                }

                //show original upload name, if empty try default name used by project
                ManualLabel.Text = PSE.OriginalUserManualName;
                if (string.IsNullOrEmpty(ManualLabel.Text))
                    ManualLabel.Text = PSE.UserManual;

                DescriptionTextBox.Text = PSE.QVDescription == null ? "" : PSE.QVDescription;
                ToolTipTextBox.Text = PSE.QVTooltip == null ? "" : PSE.QVTooltip;

                if (AutoInclusionGroups.Contains(CurrentProject.Groups.ToLower()) == true)
                {
                    PSE.AutomaticInclusion = true;
                    AutoInclusionMsg.Visible = true;
                    AutoInclusion.Enabled = false;  //user cannot change it
                }

                //check, if group is in strict reduction group, then client publisher cannot change restictrion settings
                if (StrictReductionGroups.Contains(CurrentProject.Groups.ToLower()) == true)
                {
                    PSE.SupportsReduction = true;
                    RestrictedViewsMsg.Visible = true;
                    RestrictedViews.Enabled = false;  //user cannot change it
                }

                AutoInclusion.SelectedValue = PSE.AutomaticInclusion.ToString();

                RestrictedViews.SelectedValue = PSE.SupportsReduction.ToString();
            }
        }

        

        protected void ICONbtnUpload_Click(object sender, EventArgs e)
        {
            if (ICONImage.UploadedFiles.Count > 0)
            {
                foreach (Telerik.Web.UI.UploadedFile file in ICONImage.UploadedFiles)
                {
                    //icon must be saved with same name as project and QV
                   
                    string targetFileName = System.IO.Path.Combine(WorkingDirectory, CurrentProject.QVName + file.GetExtension());
                    System.IO.File.Delete(targetFileName);
                    file.SaveAs(targetFileName);

                    //save a know file that contains the new icon filename to pick up for accepting changes
                    string ICONFILE = System.IO.Path.Combine(WorkingDirectory, "icon");
                    System.IO.File.Delete(ICONFILE);
                    System.IO.File.WriteAllText(ICONFILE, CurrentProject.QVName + file.GetExtension());

                    Icon.ImageUrl = "reflector.ashx?key=" + MillimanCommon.Utilities.ConvertStringToHex(targetFileName);
                }
            }
        }

        protected void QVWbtnUpload_Click(object sender, EventArgs e)
        {
            if (QVWImage.UploadedFiles.Count > 0)
            {
                foreach (Telerik.Web.UI.UploadedFile file in QVWImage.UploadedFiles)
                {
                    string[] QVWFiles = Directory.GetFiles(WorkingDirectory, "*.qvw");
                    foreach (string QVWFile in QVWFiles)
                        System.IO.File.Delete(QVWFile);

                    string targetFileName = System.IO.Path.Combine(WorkingDirectory, CurrentProject.QVName + file.GetExtension());
                    System.IO.File.Delete(targetFileName);
                    file.SaveAs(targetFileName);

                    //save a know file that contains the new qvw filename to pick up for accepting changes
                    string QVWFILE = System.IO.Path.Combine(WorkingDirectory, "qvw");
                    System.IO.File.Delete(QVWFILE);
                    System.IO.File.WriteAllText(QVWFILE, CurrentProject.QVName + file.GetExtension());

                    QVW.Text = file.GetName();
                }
            }
        }

        protected void ManualbtnUpload_Click(object sender, EventArgs e)
        {
            if (ManualImage.UploadedFiles.Count > 0)
            {
                foreach (Telerik.Web.UI.UploadedFile file in ManualImage.UploadedFiles)
                {

                    string targetFileName = System.IO.Path.Combine(WorkingDirectory, CurrentProject.QVName + file.GetExtension());
                    System.IO.File.Delete(targetFileName);
                    file.SaveAs(targetFileName);

                    //save a know file that contains the new manual filename to pick up for accepting changes
                    string DOCFILE = System.IO.Path.Combine(WorkingDirectory, "document");
                    System.IO.File.Delete(DOCFILE);
                    System.IO.File.WriteAllText(DOCFILE, CurrentProject.QVName + file.GetExtension());

                    ManualLabel.Text = file.GetName();
                }
            }
        }

        protected void ApplyChanges_Click(object sender, EventArgs e)
        {
            ProjectSettingsExtension PSE = CurrentProject;

            //we will create a tooltip.dat, description.dat and reductionrequired.dat file if the values changed
            if (CurrentProject.QVTooltip == null) CurrentProject.QVTooltip = "";  //make empty string if null for comparison
            
            //write out so next step can prompt user to changes made
            if (CurrentProject.QVTooltip != ToolTipTextBox.Text)
                System.IO.File.WriteAllText(System.IO.Path.Combine(WorkingDirectory, "tooltip.dat"), ToolTipTextBox.Text);
            if (CurrentProject.QVDescription != DescriptionTextBox.Text)
                System.IO.File.WriteAllText(System.IO.Path.Combine(WorkingDirectory, "description.dat"), DescriptionTextBox.Text);
            
            CurrentProject.QVTooltip = ToolTipTextBox.Text;
            CurrentProject.QVDescription = DescriptionTextBox.Text;
            CurrentProject.Notes = Notes.Text;

            //load up the doc and icon files if available
            string DOCFILE = System.IO.Path.Combine(WorkingDirectory, "document");
            string ICONFILE = System.IO.Path.Combine(WorkingDirectory, "icon");
            string QVWFILE = System.IO.Path.Combine(WorkingDirectory, "qvw");
            if (System.IO.File.Exists(DOCFILE))
            {
                PSE.UserManual = System.IO.File.ReadAllText(DOCFILE);
                System.IO.File.Delete(DOCFILE);
            }
            if (System.IO.File.Exists(ICONFILE))
            {
                PSE.QVThumbnail = System.IO.File.ReadAllText(ICONFILE);
                System.IO.File.Delete(ICONFILE);
            }
            //if no QVW is in local directory and restriced views did not change, this is just
            //a simple server update
            //check for QVW changes first  - complex scenario

            bool RestrictionValueChanged = false;
            if ((CurrentProject.SupportsReduction == true) && (System.Convert.ToBoolean(RestrictedViews.SelectedValue) == false))
                RestrictionValueChanged = true;
            else if ((CurrentProject.SupportsReduction == false) && (System.Convert.ToBoolean(RestrictedViews.SelectedValue) == true))
                RestrictionValueChanged = true;

            if ( RestrictionValueChanged )
            {
                PSE.SupportsReduction = System.Convert.ToBoolean(RestrictedViews.SelectedValue);
                string ReductionFile = Path.Combine(WorkingDirectory, "reductionrequired.dat");
                File.WriteAllText(ReductionFile, RestrictionValueChanged.ToString());
            }

            //go ahead and save the project file, since we know there must be a QVW in the works
            PSE.Save(System.IO.Path.Combine(WorkingDirectory, PSE.QVName + ".hciprj"));

            //we should at this point compare the new PSE with the old project reloaded, if nothing changed, we do nothing

            if (System.IO.File.Exists(QVWFILE))//new QVW must be signed accordingly
            {
                string NewQVW = System.IO.Path.Combine(WorkingDirectory, System.IO.File.ReadAllText(QVWFILE));
                System.IO.File.Delete(QVWFILE);
                MillimanCommon.XMLFileSignature Signature = new MillimanCommon.XMLFileSignature(NewQVW);
                Signature.SignatureDictionary.Clear();
                string[] SignatureTokens = CurrentProject.Groups.Split(new char[] { '_' });
                int Index = 0;
                foreach( string SigToken in SignatureTokens)
                {
                    Signature.SignatureDictionary.Add("@PubClientSign_" + Index.ToString(), SigToken);
                    Index++;
                }
                //update emit flag
                bool CanEmit = System.Convert.ToBoolean(RestrictedViews.SelectedValue);
                Signature.SignatureDictionary.Add("can_emit", CanEmit.ToString());
                Signature.SignatureDictionary.Add("SecurityArbiter","Milliman");
                if (Signature.SaveChanges() == false )
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to sign new QVW for project -'" + CurrentProject.ProjectName + "'");
                    Response.Redirect("FailedSignature.html");
                    return;
                }
                if ( CanEmit )
                    Response.Redirect("ReductionVerify.aspx?Key=" + Request["key"]);
                else
                    Response.Redirect("NoReductionVerify.aspx?Key=" + Request["key"]);
            }
            else if (RestrictionValueChanged) //otherwise, we need to copy over original and resign
            {
                string CopiedQVW = Path.Combine(WorkingDirectory, CurrentProject.OriginalProjectName + ".qvw");
                string SourceQVW = Path.Combine( CurrentProject.AbsoluteProjectPath, CurrentProject.QVName + ".qvw");
                File.Copy(SourceQVW, CopiedQVW);

                bool CanEmit = System.Convert.ToBoolean(RestrictedViews.SelectedValue);

                MillimanCommon.XMLFileSignature Signature = new MillimanCommon.XMLFileSignature(CopiedQVW);
                Signature.SignatureDictionary["can_emit"] = CanEmit.ToString();
                Signature.SaveChanges();

                if ( System.Convert.ToBoolean( Signature.SignatureDictionary["can_emit"] ))
                    Response.Redirect("ReductionVerify.aspx?Key=" + Request["key"]);
                else
                    Response.Redirect("NoReductionVerify.aspx?Key=" + Request["key"]);
            }
            else //don't need to do anything with QVW, just a simple project update(no reduction)
            {
                Response.Redirect("SimpleUpdateVerify.aspx?key=" + Request["key"]);
                return;
            }


        }
    }
}