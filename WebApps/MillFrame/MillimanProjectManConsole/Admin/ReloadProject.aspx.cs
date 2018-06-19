using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Telerik.Web.UI;

namespace MillimanProjectManConsole
{
    public partial class ReloadProject : System.Web.UI.Page
    {

        public class DataItems
        {
            public string ID { get; set; }
            private string _DisplayName;

            public string DisplayName
            {
                get {
                    return _DisplayName;
                }
                set { _DisplayName = value; }
            }

            private string _Name;

            public string Name
            {
                get { return _Name; }
                set {
                    IsDirty = true;
                      _Name = value; 
                }
            }
            private string _Description;

            public string Description
            {
                get { return _Description; }
                set {
                    IsDirty = true;
                    _Description = value; 
                }
            }

            private string _TempFile;

            public string TempFile
            {
                get { return _TempFile; }
                set {
                    IsDirty = true;
                    _TempFile = value; 
                }
            }
            public bool IsDirty { get; set; }

            public DataItems() { }
            public DataItems(string _ID, string _Name, string _DisplayName, string _Description)
            {
                ID = _ID;
                Name = _Name;
                Description = _Description;
                DisplayName = _DisplayName;
                IsDirty = false;
            }
        }

        const int MaxTotalBytes = int.MaxValue;

        Int64 totalBytes;

        public bool? IsRadAsyncValid
        {

            get
            {

                if (Session["IsRadAsyncValid"] == null)
                {

                    Session["IsRadAsyncValid"] = true;

                }



                return Convert.ToBoolean(Session["IsRadAsyncValid"].ToString());

            }

            set
            {

                Session["IsRadAsyncValid"] = value;

            }

        }

        /// <summary>
        /// Scan the data directory for the project and get a list of all file( max 6 ).
        /// Each file may have an associated ".description" file describing contents
        /// </summary>
        /// <param name="Directory"></param>
        /// <returns></returns>
        static public List<DataItems> GetDataFromDirectory(string Directory)
        {
            List<DataItems> MyList = new List<DataItems>();
            if (System.IO.Directory.Exists(Directory) == false)
            {
                System.IO.Directory.CreateDirectory(Directory);
                return MyList;
            }
            string[] Files = System.IO.Directory.GetFiles(Directory);
            foreach (string F in Files)
            {
                string Description = string.Empty;
                string Ext = System.IO.Path.GetExtension(F);
                if ( (string.Compare(Ext, ".description", true) != 0) && (string.Compare(Ext, ".description_new", true)!= 0) )
                {
                    //look to see if there is a description file associate with it
                    string DescriptionFileRoot = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(F), System.IO.Path.GetFileNameWithoutExtension(F));
                    MillimanCommon.DownloadDescriptions DD = new MillimanCommon.DownloadDescriptions();
                    if (System.IO.File.Exists(DescriptionFileRoot + ".description") == true)
                        DD = MillimanCommon.DownloadDescriptions.Load(DescriptionFileRoot + ".description");
                    if (System.IO.File.Exists(DescriptionFileRoot + ".description_new") == true)
                        DD = MillimanCommon.DownloadDescriptions.Load(DescriptionFileRoot + ".description_new");

                    if ( MyList.Count < 6 )
                    {
                        string Filename = System.IO.Path.GetFileName(F);
                        //get rid of "_new" in display string - dont want user to see it
                        if (Filename.ToLower().EndsWith("_new"))
                            Filename = Filename.Substring(0, Filename.Length - 4);

                        bool found = false;
                        foreach (DataItems DI in MyList)
                        {
                            if (string.Compare(DI.DisplayName, Filename, true) == 0)
                                found = true;
                        }
                        if ( found == false )   //items my exist in pairs, original.item and original.item_new
                            MyList.Add(new DataItems(Guid.NewGuid().ToString(), F, Filename, DD.Description));
                    }
                }
            }

            return MyList;
        }

        public List<DataItems> GetData()
        {
            List<DataItems> Data = Session["DownloadItems"] as List<DataItems>;
            if (Data == null)
                Session["DownloadItems"] = new List<DataItems>();

            return Session["DownloadItems"] as List<DataItems>;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["DeletedDownloadItems"] = null;
                Session["DownloadItems"] = null;

                if (Request["ProjectPath"] != null)
                {
                    string DocumentRoot = ConfigurationManager.AppSettings["QVDocumentRoot"];
                    DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));
                    MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load( System.IO.Path.Combine(DocumentRoot, Request["ProjectPath"]));
                    if (PS != null)
                    {
                        QVWName.Text = PS.QVName;
                        UpdateLabel.Text = PS.QVName + " Project Update";

                        //when we import, we get an IMP file which is really a QVW,  if we have a QVW or QVW new get rid of any IMP file we have
                        string QualifiedQVW = System.IO.Path.Combine(PS.LoadedFromPath, PS.QVName + ".qvw");
                        string NewQualifiedQVW = QualifiedQVW + "_new";
                        if ( System.IO.File.Exists(QualifiedQVW) ||  System.IO.File.Exists(NewQualifiedQVW) )
                        {
                            string[] ImpFiles = System.IO.Directory.GetFiles(PS.LoadedFromPath, "*.imp");
                            foreach (string IF in ImpFiles)
                                System.IO.File.Delete(IF);
                        }
                    }
                    string ProjectDirectory = System.IO.Path.Combine( DocumentRoot, Request["ProjectPath"] );
                    string DataDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ProjectDirectory), PS.ProjectName + "_data");
                    Session["DownloadItems"] = GetDataFromDirectory(DataDirectory);

                }

            }

            IsRadAsyncValid = null;
        }

        protected void Upload_Click(object sender, EventArgs e)
        {
            //resolve downloads and add too!
            List<string> FilesUpdated = new List<string>();
            try
            {

                string QVWSaveTo = string.Empty;

                string SelectedPath = Request["ProjectPath"];
                string DocumentRoot = ConfigurationManager.AppSettings["QVDocumentRoot"];
                DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));
                string FullPathWithProject = System.IO.Path.Combine(DocumentRoot, SelectedPath);
                string FullPath = System.IO.Path.GetDirectoryName(FullPathWithProject);

                MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(System.IO.Path.Combine(DocumentRoot, Request["ProjectPath"]));

                if (PS == null)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Could not load project '" + Request["ProjectPath"] + "' to update");
                    return;
                }
                //load up reporting ban k
                MillimanCommon.QVWReportBank Bank = new MillimanCommon.QVWReportBank(PS.LoadedFrom);

                if (QVWUpload.UploadedFiles.Count != 0)
                {

                    //clear all 'New" files in reduced user dir
                    string[] NewFiles = System.IO.Directory.GetFiles(System.IO.Path.Combine(PS.AbsoluteProjectPath, "ReducedUserQVWs"), "*.*_new", System.IO.SearchOption.AllDirectories);
                    foreach (string File in NewFiles)
                    {
                        if (File.ToLower().EndsWith("_new") == true)
                            System.IO.File.Delete(File);
                    }
                    //clear previous generated new hierarchies
                    string[] HierFiles = System.IO.Directory.GetFiles(PS.AbsoluteProjectPath, "*.*_new");
                    foreach (string HFile in HierFiles)
                    {
                        if (HFile.ToLower().Contains("hierarchy") == true)
                            System.IO.File.Delete(HFile);
                    }

                    //clear report records
                    Bank.ClearAll();

                    UploadedFile file = QVWUpload.UploadedFiles[0];
                    byte[] fileData = new byte[file.InputStream.Length];
                    file.InputStream.Read(fileData, 0, (int)file.InputStream.Length);
                    System.IO.FileStream fs = System.IO.File.Create(GetTempFile(file.FileName ), 1000000);
                    System.IO.BinaryWriter BW = new System.IO.BinaryWriter(fs);
                    BW.Write(fileData);
                    BW.Close();

                    //create with "_new" extension for rollback
                    QVWSaveTo = System.IO.Path.Combine(FullPath, QVWName.Text + ".qvw" );
                    System.IO.File.Delete(QVWSaveTo + "_new");
                    System.IO.File.Copy(GetTempFile(file.FileName), QVWSaveTo + "_new");

                    string AssociatedGroup = VerifyGroupings(PS);
                    if (string.IsNullOrEmpty(AssociatedGroup) == false )  //make sure the groups match
                    {
                        PS.QVName = QVWName.Text;
                        if (string.IsNullOrEmpty(PS.Groups))
                            PS.Groups = AssociatedGroup;

                        PS.LastProjectRunDate = DateTime.Now.ToString();
                        PS.LastProjectRun = System.Web.Security.Membership.GetUser().UserName;
                        PS.QVProjectHash = MillimanCommon.Utilities.CalculateMD5Hash(QVWSaveTo + "_new", true);
                        PS.Save();
                        FilesUpdated.Add("QVW:" + QVWName.Text + ".qvw successfully uploaded and associated with the project.\n");
                    }
                }

                if (UserManual.UploadedFiles.Count != 0)
                {
                    UploadedFile file = UserManual.UploadedFiles[0];
                    byte[] fileData = new byte[file.InputStream.Length];
                    file.InputStream.Read(fileData, 0, (int)file.InputStream.Length);

                    if (string.IsNullOrEmpty(PS.UserManual) == false)
                     //need to get rid of old if exists
                    {
                        string QualifiedUserManual = System.IO.Path.Combine(PS.LoadedFromPath, PS.UserManual);
                        if (System.IO.File.Exists(QualifiedUserManual))
                            System.IO.File.Delete(QualifiedUserManual);
                        string NewQualifiedUserManual = QualifiedUserManual + "_new";
                        if ( System.IO.File.Exists(NewQualifiedUserManual ) )
                            System.IO.File.Delete(NewQualifiedUserManual);
                    }
                    PS.UserManual = System.Guid.NewGuid().ToString("N") + "_" + file.FileName;
                    PS.Save();
                    System.IO.FileStream fs = System.IO.File.Create(GetTempFile(PS.UserManual), 1000000);
                    System.IO.BinaryWriter BW = new System.IO.BinaryWriter(fs);
                    BW.Write(fileData);
                    BW.Close();
                    string SaveTo = System.IO.Path.Combine(FullPath, PS.UserManual);
                    System.IO.File.Delete(SaveTo);
                    System.IO.File.Copy(GetTempFile(PS.UserManual), SaveTo + "_new");
                    FilesUpdated.Add("User Manual:" + PS.UserManual + "\n");
                }

                //get rid of all the items to delete
                string DownloadPath = System.IO.Path.Combine(PS.AbsoluteProjectPath, PS.ProjectName + "_data");
                List<string> DeletedDownloadItems = Session["DeletedDownloadItems"] as List<string>;
                if (DeletedDownloadItems != null)
                {
                    foreach (string S in DeletedDownloadItems)
                    {
                        string DDItem = System.IO.Path.Combine(DownloadPath, S);
                        string DDItemDesc = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(DDItem), System.IO.Path.GetFileNameWithoutExtension(DDItem) + ".description");
                        //get rid of new items too
                        if ( System.IO.File.Exists(DDItem) )
                        {
                            System.IO.File.Delete(DDItem);
                            Bank.AddItemToList( new MillimanCommon.QVWReportBank.DownloadClass( System.IO.Path.GetFileName(DDItem), MillimanCommon.QVWReportBank.DownloadClassType.DELETED ));
                        }
                        if ( System.IO.File.Exists(DDItemDesc))
                            System.IO.File.Delete(DDItemDesc);
                        if ( System.IO.File.Exists(DDItem + "_new"))
                            System.IO.File.Delete(DDItem + "_new");
                        if ( System.IO.File.Exists( DDItemDesc + "_new"))
                            System.IO.File.Delete(DDItemDesc + "_new");
                        FilesUpdated.Add("Deleted file: " + System.IO.Path.GetFileName(DDItem));
                    }
                }
                Session["DeletedDownloadItems"] = null;

                //update all download items
                List<DataItems> DIList = Session["DownloadItems"] as List<DataItems>;
                if (DIList != null)
                {
                    foreach( DataItems DI in DIList )
                    {
                        if (DI.IsDirty)
                        {
                            if (string.IsNullOrEmpty(DI.TempFile) == false)
                            {
                                //why are special cases always so ugly,  if the user selects a file with a different name
                                //we need to check and get rid of the old name
                                if (string.Compare(DI.DisplayName, System.IO.Path.GetFileName(DI.TempFile), true) != 0)
                                {
                                    //delete old display and old display.new
                                    string OldName = System.IO.Path.Combine(DownloadPath, DI.DisplayName);
                                    if (System.IO.File.Exists(OldName))
                                        System.IO.File.Delete(OldName);
                                    OldName += "_new";
                                    if (System.IO.File.Exists(OldName))
                                        System.IO.File.Delete(OldName);
                                    //get rid of description files too
                                    string DescOldName = System.IO.Path.Combine(DownloadPath, System.IO.Path.GetFileNameWithoutExtension( DI.DisplayName) + ".description");
                                    if (System.IO.File.Exists(DescOldName))
                                        System.IO.File.Delete(DescOldName);
                                    DescOldName += "_new";
                                    if (System.IO.File.Exists(DescOldName))
                                        System.IO.File.Delete(DescOldName);                           
                                }

                                //copy of file to local dir
                                System.IO.File.Copy(DI.TempFile, System.IO.Path.Combine(DownloadPath, System.IO.Path.GetFileName(DI.TempFile) + "_new"));
                                DI.DisplayName = System.IO.Path.GetFileName(DI.TempFile);
                                DI.Name = DI.TempFile;
                                DI.DisplayName = System.IO.Path.GetFileName(DI.TempFile);
                                FilesUpdated.Add("Updated file:" + System.IO.Path.GetFileName(DI.TempFile));
                                if (System.IO.File.Exists(System.IO.Path.Combine(DownloadPath, System.IO.Path.GetFileName(DI.TempFile))) == true)
                                    Bank.AddItemToList(new MillimanCommon.QVWReportBank.DownloadClass(DI.DisplayName, MillimanCommon.QVWReportBank.DownloadClassType.MODIFIED));
                                else
                                    Bank.AddItemToList(new MillimanCommon.QVWReportBank.DownloadClass(DI.DisplayName, MillimanCommon.QVWReportBank.DownloadClassType.ADDED));
                            }

                            string LocalDescFile = System.IO.Path.Combine(DownloadPath, System.IO.Path.GetFileNameWithoutExtension(DI.TempFile) + ".description_new");
                            if (string.IsNullOrEmpty(DI.TempFile))
                                LocalDescFile = System.IO.Path.Combine(DownloadPath, System.IO.Path.GetFileNameWithoutExtension(DI.Name) + ".description_new");
                            //System.IO.File.WriteAllText(LocalDescFile, DI.Description);
                            //need to serialize it out so it can be read by client admin
                            MillimanCommon.DownloadDescriptions DD = new MillimanCommon.DownloadDescriptions();
                            //never put file with "new" in container
                            DD.DownloadItem = System.IO.Path.GetFileName(DI.Name.Replace("_new",""));
                            DD.Description = DI.Description;
                            DD.Save(LocalDescFile);

                            FilesUpdated.Add("Updated file description:" + System.IO.Path.GetFileName(DI.Name));
                        }
                    }
                }
                string ProjectDirectory = System.IO.Path.Combine(DocumentRoot, Request["ProjectPath"]);
                string DataDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ProjectDirectory), PS.ProjectName + "_data");
                Session["DownloadItems"] = GetDataFromDirectory(DataDirectory);
                Session["ProjectPath"] = System.IO.Path.Combine(DocumentRoot, Request["ProjectPath"]);
   
                //only direct here if a new QVW is available
                if (string.IsNullOrEmpty(QVWSaveTo) == false)
                    Response.Redirect("../complexupload/SignatureVerification.aspx");
                else
                    MillimanCommon.Alert.Show("Local project update completed");
            }
            catch (Exception ex)
            {
                MillimanCommon.Alert.Show("Failed to upload file - " + ex.ToString());
            }

            if (Session["DownloadItems"] != null)
            {
                RadGrid1.DataSource = Session["DownloadItems"] as List<DataItems>;
                RadGrid1.DataBind();
            }
        }

        /// <summary>
        /// Ensure the incoming QVW has the same group info in signature as current
        /// </summary>
        /// <param name="PS"></param>
        /// <returns></returns>
        private string VerifyGroupings(MillimanCommon.ProjectSettings PS)
        {
            if (PS != null)
            {
                string QVWPaths = System.IO.Path.GetDirectoryName(PS.LoadedFrom);
                bool AllowUnsignedQVW = System.Convert.ToBoolean(ConfigurationManager.AppSettings["AllowUnsignedQVWs"]);
                //we may have QVW coming thought that are not signed yet, thus
                //allow them to move into position, even though we cannot check them
                //this should be a temp situation
                string NewGroup = GetGroup( System.IO.Path.Combine(QVWPaths, PS.QVName) + ".qvw_new");
                string GroupFromPath = GetGroupFromPath(QVWPaths);
                if ((string.IsNullOrEmpty(NewGroup) == true) && (AllowUnsignedQVW) )
                {
                    return GroupFromPath;
                }

                string OriginalGroup = GetGroup(System.IO.Path.Combine(QVWPaths, PS.QVName) + ".qvw");
                if (string.IsNullOrEmpty(OriginalGroup) == true)
                    OriginalGroup = PS.Groups;

                if (string.Compare(OriginalGroup, NewGroup, true) != 0)
                {   //error condition
                    //get rid of new one
                    System.IO.File.Delete(System.IO.Path.Combine(QVWPaths, PS.QVName) + ".qvw_new");
                    string ErrorValue = OriginalGroup + "~" + NewGroup;
                    Response.Redirect("QVWReloadError.aspx?key=" + MillimanCommon.Utilities.ConvertStringToHex( ErrorValue ));
                    return string.Empty;
                }
                else
                {
                    return NewGroup;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Return the group assigned by signature, or empty string for now group signature
        /// </summary>
        /// <param name="QualifiedQVW"></param>
        /// <returns></returns>
        private string GetGroup(string QualifiedQVW)
        {
            string Group = string.Empty;
            MillimanCommon.XMLFileSignature XMLFS = new MillimanCommon.XMLFileSignature(QualifiedQVW);
            
            foreach (KeyValuePair<string, string> Item in XMLFS.SignatureDictionary)
            {
                if (Item.Key.StartsWith("@") == true)
                {
                    if ((string.IsNullOrEmpty(Group) == false) && (string.IsNullOrEmpty(Item.Value) == false))
                        Group += "_";
                    Group += Item.Value;
                }
            }
            return Group;
        }

        /// <summary>
        /// Accepts path and returns group based on path location
        /// </summary>
        /// <param name="QualifiedPath"></param>
        /// <returns></returns>
        private string GetGroupFromPath(string QualifiedPath)
        {
            string GroupName = QualifiedPath.Replace('/', '\\');
            string DocumentRoot = ConfigurationManager.AppSettings["QVDocumentRoot"];
            GroupName = GroupName.Substring(DocumentRoot.Length);
            if (GroupName.StartsWith("\\"))
                GroupName = GroupName.Substring(1);
            GroupName = GroupName.Replace('\\', '_');
            return GroupName;
        }

        protected void RadGrid1_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode)
            {
                RadAsyncUpload upload = ((GridEditableItem)e.Item)["Upload"].FindControl("AsyncUpload1") as RadAsyncUpload;
                TableCell cell = (TableCell)upload.Parent;
                CustomValidator validator = new CustomValidator();
                validator.ErrorMessage = "Please select file to be uploaded";
                validator.ClientValidationFunction = "validateRadUpload";
                validator.Display = ValidatorDisplay.Dynamic;
                cell.Controls.Add(validator);
            }
        }

        protected string TrimDescription(string description)
        {
            if (!string.IsNullOrEmpty(description) && description.Length > 200)
            {
                return string.Concat(description.Substring(0, 200), "...");
            }
            return description;
        }

        protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RadGrid1.DataSource = GetData();
        }

        protected void RadGrid1_InsertCommand(object source, GridCommandEventArgs e)
        {
            if (!IsRadAsyncValid.Value)
            {
                e.Canceled = true;
                //RadAjaxManager.Alert("The length of the uploaded file must be less than 1 MB");
                return;
            }

            GridEditFormInsertItem insertItem = e.Item as GridEditFormInsertItem;
            //string DisplayName = (insertItem["DisplayName"].FindControl("txbName") as RadTextBox).Text;
            string description = (insertItem["Description"].FindControl("txbDescription") as RadTextBox).Text;
            RadAsyncUpload radAsyncUpload = insertItem["Upload"].FindControl("AsyncUpload1") as RadAsyncUpload;
            UploadedFile file = radAsyncUpload.UploadedFiles[0];
            byte[] fileData = new byte[file.InputStream.Length];
            file.InputStream.Read(fileData, 0, (int)file.InputStream.Length);
            System.IO.FileStream fs = System.IO.File.Create(GetTempFile(file.FileName), 2048);
            System.IO.BinaryWriter BW = new System.IO.BinaryWriter(fs);
            BW.Write(fileData);
            BW.Close();
            DataItems DI = new DataItems(Guid.NewGuid().ToString(), file.FileName, System.IO.Path.GetFileName(file.FileName), description);
            DI.TempFile = GetTempFile(file.FileName);
            GetData().Add(DI);
        }

        private string GetTempFile(string RequestedName)
        {
            string Destination = System.IO.Path.Combine(System.IO.Path.GetTempPath(), RequestedName);
            return Destination;
        }

        protected void RadGrid1_UpdateCommand(object source, GridCommandEventArgs e)
        {
            if (!IsRadAsyncValid.Value)
           {
                e.Canceled = true;
                //RadAjaxManager.Alert("The length of the uploaded file must be less than 1 GB");
                return;
            }
            GridEditableItem editedItem = e.Item as GridEditableItem;
            DataItems DI = GetData()[e.Item.DataSetIndex];
            if (DI != null)
            {
                string DisplayName = (editedItem["DisplayName"].FindControl("txbName") as RadTextBox).Text;
                string description = (editedItem["Description"].FindControl("txbDescription") as RadTextBox).Text;
                DI.DisplayName = DisplayName;
                DI.Description = description;
                RadAsyncUpload radAsyncUpload = editedItem["Upload"].FindControl("AsyncUpload1") as RadAsyncUpload;
                if ( (radAsyncUpload != null) && (radAsyncUpload.UploadedFiles.Count == 1) )
                {
                    UploadedFile file = radAsyncUpload.UploadedFiles[0];
                    byte[] fileData = new byte[file.InputStream.Length];
                    file.InputStream.Read(fileData, 0, (int)file.InputStream.Length);
                    System.IO.FileStream fs = System.IO.File.Create(GetTempFile(file.FileName), 1000000);
                    System.IO.BinaryWriter BW = new System.IO.BinaryWriter(fs);
                    BW.Write(fileData);
                    BW.Close();
                    DI.Name = file.FileName;
                    DI.TempFile = GetTempFile(file.FileName);
                }
            }

        }

        protected void RadGrid1_DeleteCommand(object source, GridCommandEventArgs e)
        {
            //add the deleted item name to the deleted list and remove from in-memory list
            List<string> DeletedDownloadItems = null;
            if (Session["DeletedDownloadItems"] == null)
                Session["DeletedDownloadItems"] = new List<string>();
            DeletedDownloadItems = Session["DeletedDownloadItems"] as List<string>;
            DeletedDownloadItems.Add(GetData()[e.Item.DataSetIndex].Name);
            GetData().RemoveAt(e.Item.DataSetIndex);
        }

        protected void RadGrid1_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == RadGrid.EditCommandName)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SetEditMode", "isEditMode = true;", true);
            }
        }

        protected void AsyncUpload1_FileUploaded(object sender, FileUploadedEventArgs e)
        {
            if ((totalBytes < MaxTotalBytes) && (e.File.ContentLength < MaxTotalBytes))
            {
                e.IsValid = true;
                totalBytes += e.File.ContentLength;
                IsRadAsyncValid = true;
            }
            else
            {
                e.IsValid = false;
                IsRadAsyncValid = false;
            }
        }

    }
}