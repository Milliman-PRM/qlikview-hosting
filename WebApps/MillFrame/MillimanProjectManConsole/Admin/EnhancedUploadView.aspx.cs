using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using MillimanCommon;

public partial class EnhancedUploadView : System.Web.UI.Page
{
  
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            //always disable groups
            //SelectedGroups.Enabled = false;

            LoadPresets();
            string VirtualDir = string.Empty;
            string Groups = string.Empty;

             MillimanCommon.ProjectSettings _Settings =  LoadSettings(out VirtualDir, out Groups, true);  //pass in true to update display
             if (string.IsNullOrEmpty(_Settings.ProjectName) == false)
             {
                 //once a project is created it cannot be renamed
                 ProjectName.Enabled = false;
             }
            //if the user saved successfully, this will show a success message to the user
            //other we don't show anything - will only display the message 1 time
            if (Request["status"] != null)
            {
                if (Session[Request["status"]] != null)
                {
                    MillimanCommon.Alert.Show( Session[Request["status"]].ToString() );
                    Session[Request["status"]] = null;
                }
            }
        }
    }
    //load all pre-defined setttings,  like all groups, etc....
    private bool LoadPresets()
    {
        MillimanProjectManConsole.MPMCServices.ArrayOfString Groups = MillimanProjectManConsole.Global.GetInstance().GetGroups();
        List<string> GroupList = new List<string>();
        if (Groups != null)
        {
            GroupList = Groups.ToList();
            GroupList.Sort();
            //remove "Administrator" role if found
            if (GroupList.Contains("Administrator"))
                GroupList.RemoveAt(GroupList.IndexOf("Administrator"));
        }
        SelectedGroups.DataSource = GroupList;
        SelectedGroups.DataBind();
        //point image to default image
        PreviewImage.ImageUrl = "ImageReflector.aspx?key=" + Utilities.ConvertStringToHex(Server.MapPath("~/images/DefaultProjectImage.gif"));
        return true;
    }
    //load all the user settings
    private MillimanCommon.ProjectSettings LoadSettings( out string VirtualDir, out string Groups, bool UpdateDisplay=false)
    {
        MillimanCommon.ProjectSettings _Settings = null;
        VirtualDir = string.Empty;
        Groups = string.Empty;
        //see if we are creating new project or editing existing
        if (Request["QVPath"] != null)
        {
            //if we only get a QVPath - we are creating a new entry, otherwise we are updating an existing entity
            string QVPath = Request["QVPath"].Replace(@"/", @"\");
            string QVName = string.Empty;
            if (QVPath.IndexOf(".hciprj", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                QVName = Path.GetFileName(QVPath);
                QVPath = QVPath.Substring(0, QVPath.Length - QVName.Length);
            }
            string DocumentRoot = ConfigurationManager.AppSettings["QVDocumentRoot"];

            //DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));
            string PathFilename = Path.Combine(DocumentRoot, QVPath, QVName);

            if (string.IsNullOrEmpty(QVName) == false)
            {
                _Settings = MillimanCommon.ProjectSettings.Load(PathFilename);
            }
            else
            {
                _Settings = new MillimanCommon.ProjectSettings();
            }
            VirtualDir = QVPath;
            Groups = _Settings.Groups;
        }
        //otherwise check to see if we are importing
        else if (Request["loc"] != null)
        {
            string FullLocationWithName = Utilities.ConvertHexToString(Request["loc"]);
            string Group = string.Empty;
            //bool CanEmit = false;
            MillimanCommon.XMLFileSignature XMLFS = new MillimanCommon.XMLFileSignature(FullLocationWithName);
            foreach (KeyValuePair<string, string> Item in XMLFS.SignatureDictionary)
            {
                if (Item.Key.StartsWith("@") == true)
                {
                    if ((string.IsNullOrEmpty(Group) == false) && (string.IsNullOrEmpty(Item.Value) == false))
                        Group += "_";
                    Group += Item.Value;
                }
                //if (string.Compare(Item.Key, "can_emit", true) == 0)
                //    CanEmit = true;
            }

            _Settings = new MillimanCommon.ProjectSettings();
            _Settings.AbsoluteProjectPath = System.IO.Path.GetDirectoryName(FullLocationWithName);
            _Settings.Groups = Group;
            VirtualDir = Group.Replace('_', '\\');
            Groups = _Settings.Groups;

            //we are importing, show the default gif, can't save one here don't have the project name
            //yet and gif must match project name
            string DefaultImage = Server.MapPath("~/images/DefaultProjectImage.gif");
            PreviewImage.ImageUrl = "ImageReflector.aspx?key=" + Utilities.ConvertStringToHex(DefaultImage);

         }

        if ((_Settings != null) && UpdateDisplay)
            SetUserSelections(_Settings, VirtualDir);

        return _Settings;
    }

 

    //save all the user settings
    private bool SaveSettings(out string VirtualPath )
    {
        string DocumentRoot = ConfigurationManager.AppSettings["QVDocumentRoot"];
       // DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));

        string QVPath = string.Empty;
        string Groups = string.Empty;

        MillimanCommon.ProjectSettings _Settings = LoadSettings(out QVPath, out Groups);

        string PresentationThumbnailFile = SaveUserFiles(DocumentRoot, QVPath, PresentationThumbnail);
        //we need to provide a default
        if (string.IsNullOrEmpty(PresentationThumbnailFile) == true)
        {
            PresentationThumbnailFile = Server.MapPath("~/images/DefaultProjectImage.gif");
            string ImageCopytoProject = System.IO.Path.Combine(DocumentRoot, QVPath, System.IO.Path.GetFileName(PresentationThumbnailFile));
            if ( System.IO.File.Exists(ImageCopytoProject) == false)
            System.IO.File.Copy(PresentationThumbnailFile, ImageCopytoProject);
            PresentationThumbnailFile = ImageCopytoProject;
        }
        PresentationThumbnailFile = RenameFile(PresentationThumbnailFile, ProjectName.Text.Trim(), true);
        if (string.IsNullOrEmpty(PresentationThumbnailFile) == false)
        {
            //convert name to full path
            if (string.IsNullOrEmpty(_Settings.QVThumbnail) == false)
            {
                string OldImageFile = System.IO.Path.Combine(DocumentRoot, QVPath, _Settings.QVThumbnail);
                //get rid of old file
                File.Delete(OldImageFile);
            }
            PreviewImage.ImageUrl = "ImageReflector.aspx?key=" + Utilities.ConvertStringToHex(PresentationThumbnailFile);
            _Settings.QVThumbnail = System.IO.Path.GetFileName(PresentationThumbnailFile.Replace("_new","")); //save just name
            _Settings.QVThumbnailHash = Utilities.CalculateMD5Hash(PresentationThumbnailFile, true);
            //since we are updating a default, copy the new image tothe default setting
            if ( System.IO.File.Exists(PresentationThumbnailFile.Replace("_new", "")))
                System.IO.File.Delete(PresentationThumbnailFile.Replace("_new", ""));
            System.IO.File.Copy(PresentationThumbnailFile, PresentationThumbnailFile.Replace("_new", ""));
        }
        else //no presentation to save
        {
            //copy over default so the project has something
            if (string.IsNullOrEmpty(_Settings.QVThumbnail) == true)
            {
                string DefaultImage = Server.MapPath("~/images/DefaultProjectImage.gif");

                _Settings.QVThumbnail = _Settings.ProjectName + ".gif";
                _Settings.QVThumbnailHash = Utilities.CalculateMD5Hash(DefaultImage, true);
                //make one copy the old copy
                if ( System.IO.File.Exists(System.IO.Path.Combine(_Settings.AbsoluteProjectPath, _Settings.QVThumbnail)))
                    System.IO.File.Delete(System.IO.Path.Combine(_Settings.AbsoluteProjectPath, _Settings.QVThumbnail));
                System.IO.File.Copy(DefaultImage, System.IO.Path.Combine(_Settings.AbsoluteProjectPath, _Settings.QVThumbnail));
                //copy same image to the new - that way the publisher will pick it up as a new file and upload it
                if (System.IO.File.Exists(System.IO.Path.Combine(_Settings.AbsoluteProjectPath, _Settings.QVThumbnail + "_new")))
                    System.IO.File.Delete(System.IO.Path.Combine(_Settings.AbsoluteProjectPath, _Settings.QVThumbnail + "_new"));
                System.IO.File.Copy(DefaultImage, System.IO.Path.Combine(_Settings.AbsoluteProjectPath, _Settings.QVThumbnail + "_new"));
            }
        }

        _Settings.UserManual = UserManualLabel.Text;

        GetUserSelections(ref _Settings);

        //if they rename via the ui - rename the project file
        string SaveConfigurationTo = Path.Combine(DocumentRoot, QVPath, ProjectName.Text.Trim());
        if (string.IsNullOrEmpty(_Settings.LoadedFrom) == false)
        {
            if (string.Compare(SaveConfigurationTo, _Settings.LoadedFrom, true) != 0)
            {
                System.IO.File.Delete(_Settings.LoadedFrom);
                _Settings.LoadedFrom = SaveConfigurationTo;
            }
        }

        if (SaveConfigurationTo.IndexOf(".hciprj") == -1)
            SaveConfigurationTo += ".hciprj";
        _Settings.Save(SaveConfigurationTo);

        //create the hidden sub dirs
        string ReducedUserQVWsDir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SaveConfigurationTo), "ReducedUserQVWs");
        if (System.IO.Directory.Exists(ReducedUserQVWsDir) == false)
            System.IO.Directory.CreateDirectory(ReducedUserQVWsDir);
        string DataDir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SaveConfigurationTo), _Settings.ProjectName + "_data");
        if (System.IO.Directory.Exists(DataDir) == false)
            System.IO.Directory.CreateDirectory(DataDir);

        //close the window
        Page.ClientScript.RegisterClientScriptBlock(GetType(), "CloseScript", "CloseDialog()", true);

        VirtualPath = QVPath;

        //we do not copy of the QVW in the new scheme, since it may require index extraction, user must 
        //associate the QVW to the project explicitely
        //if (Request["loc"] != null)
        //{
        //    string FullPathWithName = Utilities.ConvertHexToString(Request["loc"]);
        //    System.IO.File.Move(FullPathWithName, System.IO.Path.Combine(DocumentRoot, VirtualPath, System.IO.Path.GetFileNameWithoutExtension(QVWName.Text) + ".qvw_new"));
        //}

        return true;
    }

    /// <summary>
    /// Take a file and rename it but leave the path and extension the same
    /// </summary>
    /// <param name="QualifiedPathName"></param>
    /// <param name="?"></param>
    /// <returns></returns>
    private string RenameFile( string QualifiedPathName, string NewName, bool IsNew = false )
    {
        //empty do nothing - return empty
        if (string.IsNullOrEmpty(QualifiedPathName) == true)
            return "";

        //chop off extension if present
        NewName = System.IO.Path.GetFileNameWithoutExtension(NewName);

        string NewQualifiedName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(QualifiedPathName), NewName + System.IO.Path.GetExtension(QualifiedPathName));
        if (IsNew)
            NewQualifiedName += "_new"; 
        if (System.IO.File.Exists(NewQualifiedName))
            System.IO.File.Delete(NewQualifiedName);
        System.IO.File.Move(QualifiedPathName, NewQualifiedName);

        return NewQualifiedName;
    }

    //this will update all the item in the UI,  based on project settings
    private void SetUserSelections(MillimanCommon.ProjectSettings Settings, string Path)
    {
        ProjectName.Text = Settings.ProjectName.Trim(); 
        QVWName.Text = Settings.QVName.Trim();
        Description.Text = Settings.QVDescription.Trim();
        Notes.Text = Settings.Notes.Trim();
        Tooltip.Text = Settings.QVTooltip.Trim();
        //PresentationThumbnailLabel.Text = Settings.QVThumbnail;
        string DocumentRoot = ConfigurationManager.AppSettings["QVDocumentRoot"];
        //show new one if present
        string NewImagePath = System.IO.Path.Combine(DocumentRoot.Trim(), Path.Trim(), Settings.QVThumbnail.Trim() + "_new");
        string OldImagePath = System.IO.Path.Combine(DocumentRoot.Trim(), Path.Trim(), Settings.QVThumbnail.Trim());
        if (System.IO.File.Exists(NewImagePath))
        {
            PreviewImage.ImageUrl = "imagereflector.aspx?key=" + Utilities.ConvertStringToHex(NewImagePath);
        }
        //otherwise show old one
        else if (System.IO.File.Exists(OldImagePath))
        {
            PreviewImage.ImageUrl = "imagereflector.aspx?key=" + Utilities.ConvertStringToHex(OldImagePath);
        }

        string NewManualPath = System.IO.Path.Combine(DocumentRoot.Trim(), Path.Trim(), Settings.UserManual.Trim() + "_new");
        string OldManualPath = System.IO.Path.Combine(DocumentRoot.Trim(), Path.Trim(), Settings.UserManual.Trim());

        UserManualLabel.Text = Utilities.RemoveGUIDFromString(Settings.UserManual);

        string UserManualURL = string.Empty;
        if ( System.IO.File.Exists( NewManualPath ) )
            UserManualURL = "documentreflector.aspx?key=" + Utilities.ConvertStringToHex( NewManualPath );
        else if (System.IO.File.Exists(OldManualPath))
            UserManualURL = "documentreflector.aspx?key=" + Utilities.ConvertStringToHex(OldManualPath);

        UserManualLabel.NavigateUrl = UserManualURL;
        UserManualLabel.Target = "_blank";

        if (string.IsNullOrEmpty(Settings.Groups) == false)
        {
            string[] SelectGroups = Settings.Groups.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
            foreach( string S in SelectGroups )
            {
                //what to do if not a group now?
                 ListItem LI =  SelectedGroups.Items.FindByText( S );
                 if (LI != null)
                     LI.Selected = true;
            }
            //we want to show a tooltop on selections, but do so in the order of the display list
            string GroupTooltip = "";
            foreach( ListItem item in SelectedGroups.Items)
            {
                if (item.Selected)
                {
                    if (GroupTooltip != "")
                        GroupTooltip += "\x0A";  //add a new line
                    GroupTooltip += item.Value;
                }
            }
            SelectedGroups.ToolTip = GroupTooltip;
        }
    }
    private void GetUserSelections(ref MillimanCommon.ProjectSettings Settings)
    {
        Settings.ProjectName = ProjectName.Text.Trim();
        Settings.QVName = QVWName.Text.Trim();
        Settings.QVDescription = Description.Text.Trim();
        Settings.Notes = Notes.Text.Trim();
        Settings.UserManual = UserManualLabel.Text.Trim();
        Settings.Groups = "";
        Settings.QVTooltip = Tooltip.Text.Trim();
        foreach (ListItem LI in SelectedGroups.Items)
        {
            if (LI.Selected)
            {
                if (string.IsNullOrEmpty(Settings.Groups) == false)
                    Settings.Groups += "~";
                Settings.Groups += LI.Value;
            }
        }
    }
    /// <summary>
    /// save the uploaded file to disk
    /// </summary>
    /// <param name="RootDocumentPath"></param>
    /// <param name="SelectedPath"></param>
    /// <param name="FU"></param>
    /// <returns></returns>
    private string SaveUserFiles(string RootDocumentPath, string SelectedPath,Telerik.Web.UI.RadAsyncUpload FU)
    {
        string SaveTo = "";
        try
        {
            string PathFilename = Path.Combine(RootDocumentPath.ToString(), SelectedPath);
            if (FU.UploadedFiles.Count > 0)
            {
                Telerik.Web.UI.UploadedFile TUF = FU.UploadedFiles[0];
                string filename = Path.GetFileName(TUF.FileName);
                string extension = Path.GetExtension(filename);
                SaveTo = Path.Combine( PathFilename.ToString(), filename );

                if (File.Exists(SaveTo))
                    File.Delete(SaveTo);

                byte[] fileData = new byte[TUF.InputStream.Length];
                TUF.InputStream.Read(fileData, 0, (int)TUF.InputStream.Length);
                System.IO.FileStream fs = System.IO.File.Create(SaveTo, 1000000);
                System.IO.BinaryWriter BW = new System.IO.BinaryWriter(fs);
                BW.Write(fileData);
                BW.Close();
            }
        }
        catch (Exception)
        {

        }
        return SaveTo;
    }
    protected void Upload_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ProjectName.Text) == true)
        {
            ProjectName.Enabled = true;
            ProjectName.Focus();
            MillimanCommon.Alert.Show("A project name must be provided before any information can be saved");
            return;
        }
        ProjectName.Enabled = false;
        string Msg = "Default settings saved.";
        string VirtualPath = string.Empty;
        if ( SaveSettings(out VirtualPath) == false )
            Msg = "Failed to save default settings.";
        
        MillimanCommon.Alert.Show(Msg);
    }
  
    protected void CreateGroup_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(NewGroupName.Text) == true)
        {
            MillimanCommon.Alert.Show("A group name must be entered!");
            NewGroupName.Focus();
            return;
        }

        string Results = MillimanProjectManConsole.Global.GetInstance().CreateGroup(NewGroupName.Text);

        MillimanCommon.Alert.Show(Results);
        NewGroupName.Text = "";
        NewGroupName.Focus();

        SelectedGroups.DataSource = MillimanProjectManConsole.Global.GetInstance().GetGroups();
        SelectedGroups.DataBind();
    }

    //always the same for now
    protected void ProjectName_TextChanged(object sender, EventArgs e)
    {
        QVWName.Text = ProjectName.Text.Trim();
    }

    protected void Delete_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(NewGroupName.Text) == true)
        {
            MillimanCommon.Alert.Show("A group name must be entered!");
            NewGroupName.Focus();
            return;
        }

        string Msg = MillimanProjectManConsole.Global.GetInstance().DeleteGroup(NewGroupName.Text);
        MillimanCommon.Alert.Show(Msg);
        NewGroupName.Text = "";
        NewGroupName.Focus();

        SelectedGroups.DataSource = MillimanProjectManConsole.Global.GetInstance().GetGroups();
        SelectedGroups.DataBind();
    }
}