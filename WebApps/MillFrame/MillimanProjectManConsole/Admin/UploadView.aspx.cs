using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Web.Configuration;

public partial class UploadView : System.Web.UI.Page
{
  
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            //if we only get a QVPath - we are creating a new entry, otherwise we are updating an existing entity
            string QVPath = Request["QVPath"].Replace(@"/", @"\");
            string QVName = string.Empty;
            if (QVPath.IndexOf(".qvw", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                QVName = Path.GetFileName(QVPath);
                QVPath = QVPath.Substring(0, QVPath.Length - QVName.Length);
            }
            string DocumentRoot = ConfigurationManager.AppSettings["QVDocumentRoot"];

            CovisintFieldID.DataSource = ConfigurationManager.AppSettings["CovisintTags"].Split(new char[] { '~' });
            CovisintFieldID.DataBind();
            QVFieldID.DataSource = ConfigurationManager.AppSettings["MillimanTags"].Split(new char[] { '~' });
            QVFieldID.DataBind();

            string[] FriendlyDBNames = ConfigurationManager.AppSettings["DBConnections"].Split(new char[] { '~' });
            for (int Index = 0; Index < FriendlyDBNames.Length; Index = Index + 2)
            {
                DatabaseConnection.Items.Add(new ListItem(FriendlyDBNames[Index], FriendlyDBNames[Index + 1]));
            }

            DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));
            string PathFilename = Path.Combine(DocumentRoot, QVPath, QVName);

            MillimanCommon.ProjectSettings _Settings;
            if (string.IsNullOrEmpty(QVName) == false)
            {
                _Settings = MillimanCommon.ProjectSettings.Load(PathFilename);
            }
            else
            {
                _Settings = new MillimanCommon.ProjectSettings();
            }

            SetSelections(_Settings);
        }
    }

    private void SetSelections(MillimanCommon.ProjectSettings Settings)
    {
        Description.Text = Settings.QVDescription;

        if (string.IsNullOrEmpty(Settings.CovisintFieldName) == false)
        {
            CovisintFieldID.SelectedValue = Settings.CovisintFieldName;
            QVFieldID.SelectedValue = Settings.MillimanControlName;
            CovisintFieldID.Enabled = true;
            QVFieldID.Enabled = true;
            DataRestrictionCheckbox.Checked = true;
        }
        else
        {
            CovisintFieldID.Enabled = false;
            QVFieldID.Enabled = false;
            DataRestrictionCheckbox.Checked = false;
        }

        if (string.IsNullOrEmpty(Settings.FriendlyDBName) == false)
        {
            DatabaseConnection.SelectedValue = Settings.FriendlyDBName;
            DatabaseConnection.Enabled = true;
            UserDatabase.Checked = true;
        }
        else
        {
            DatabaseConnection.Enabled = false;
            UserDatabase.Checked = false;
        }
    }

    private void GetSelections(ref MillimanCommon.ProjectSettings Settings)
    {
        Settings.QVDescription = Description.Text;
        if (DataRestrictionCheckbox.Checked)
        {
            Settings.CovisintFieldName = CovisintFieldID.SelectedValue;
            Settings.MillimanControlName = QVFieldID.SelectedValue;
        }
        else
        {
            Settings.CovisintFieldName = string.Empty;
            Settings.MillimanControlName = string.Empty;
        }
        if (UserDatabase.Checked)
        {
            Settings.FriendlyDBName = DatabaseConnection.SelectedItem.Text;
            Settings.DBConnectionString = DatabaseConnection.SelectedValue;
        }
        else
        {
            Settings.FriendlyDBName = string.Empty;
            Settings.DBConnectionString = string.Empty;
        }
    }

    /// <summary>
    /// save the uploaded file to disk
    /// </summary>
    /// <param name="RootDocumentPath"></param>
    /// <param name="SelectedPath"></param>
    /// <param name="FU"></param>
    /// <returns></returns>
    private string SaveUserFiles(string RootDocumentPath, string SelectedPath, FileUpload FU, bool IsPresentation = false)
    {
        string SaveTo = "";
        try
        {
            string PathFilename = Path.Combine(RootDocumentPath.ToString(), SelectedPath);
            if (FU.HasFile)
            {
                string filename = Path.GetFileName(FU.FileName);
                string extension = Path.GetExtension(filename);
                if (IsPresentation)
                {
                    if ((string.Compare(extension, @".qvw", true) != 0) || (string.IsNullOrEmpty(extension) == true))
                    {
                        if (string.IsNullOrEmpty(extension) == true)
                            filename += @".qvw";
                        else
                            filename = filename.Replace(extension, @".qvw");
                    }
                }
                SaveTo = Path.Combine( PathFilename.ToString(), filename );

                if (File.Exists(SaveTo))
                    File.Delete(SaveTo);

                FU.SaveAs(SaveTo);
                
            }
        }
        catch (Exception)
        {

        }
        return SaveTo;
    }

    protected void Upload_Click(object sender, EventArgs e)
    {
        string QVPath = Request["QVPath"].Replace(@"/", @"\");
        string QVName = string.Empty;
        if (QVPath.IndexOf(".qvw", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            QVName = Path.GetFileName(QVPath);
            QVPath = QVPath.Substring(0, QVPath.Length - QVName.Length);
        }

        string DocumentRoot = ConfigurationManager.AppSettings["QVDocumentRoot"];
        DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));

        //Note:  if QVName is missing - we are creating something new,  and thus the "QVW" file must be present
        MillimanCommon.ProjectSettings _Settings;
        if (string.IsNullOrEmpty(QVName) == false)
        {
            _Settings = MillimanCommon.ProjectSettings.Load(Path.Combine(DocumentRoot, QVPath, QVName));
        }
        else
        {
            _Settings = new MillimanCommon.ProjectSettings();
        }

        string PresentationFile = SaveUserFiles(DocumentRoot, QVPath, PresentationUploadControl, true);
        string PresentationResourcesFile = SaveUserFiles(DocumentRoot, QVPath, PresentationResoucesUploadControl);
        string PresentationThumbnailFile = SaveUserFiles(DocumentRoot, QVPath, PresentationThumbnail);

        _Settings.QVProject = string.IsNullOrEmpty(PresentationFile) ? _Settings.QVProject : PresentationFile;
        _Settings.QVResources = string.IsNullOrEmpty(PresentationResourcesFile) ? _Settings.QVResources : PresentationResourcesFile;
        _Settings.QVThumbnail = string.IsNullOrEmpty(PresentationThumbnailFile) ? _Settings.QVThumbnail : PresentationThumbnailFile;

        GetSelections(ref _Settings);

        string SaveConfigurationTo;
        if (string.IsNullOrEmpty(_Settings.LoadedFrom) == false)
            SaveConfigurationTo = _Settings.LoadedFrom;
        else
            SaveConfigurationTo = Path.Combine(DocumentRoot, QVPath, PresentationUploadControl.FileName);

        _Settings.Save(SaveConfigurationTo);

        //close the window
        Page.ClientScript.RegisterClientScriptBlock(GetType(), "CloseScript", "CloseDialog()", true);
    }
    protected void DataRestrictionCheckbox_CheckedChanged(object sender, EventArgs e)
    {
        CovisintFieldID.Enabled = DataRestrictionCheckbox.Checked;
        QVFieldID.Enabled = DataRestrictionCheckbox.Checked;
    }
    protected void UserDatabase_CheckedChanged(object sender, EventArgs e)
    {
        DatabaseConnection.Enabled = UserDatabase.Checked;
    }
}