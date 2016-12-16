using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MillimanCommon
{
    /// <summary>
    /// Summary description for ProjectSettings
    /// </summary>
    public class ProjectSettings
    {

        //private string _ProjectSignature;
        //public string ProjectSignature
        //{
        //    get { return _ProjectSignature; }
        //    set { _ProjectSignature = value; }
        //}

        //Name of project as seen by a user
        private string _ProjectName;
        public string ProjectName
        {
            get { return _ProjectName; }
            set { _ProjectName = value; }
        }

        private string _OriginalProjectName;
        /// <summary>
        /// Name orginally provided by user to the project, however the QVW will
        /// be renamed to match the name of the project to avoid collisions
        /// </summary>
        public string OriginalProjectName
        {
            get { return _OriginalProjectName; }
            set { _OriginalProjectName = value; }
        }

        private bool _SupportsReduction;
        /// <summary>
        /// Used by client publisher to perist if this QVW
        /// last used reduction
        /// </summary>
        public bool SupportsReduction
        {
            get { return _SupportsReduction; }
            set { _SupportsReduction = value; }
        }


        ////the local path of the project
        //private string _LocalProjectPath;
        //public string LocalProjectPath
        //{
        //    get { return _LocalProjectPath; }
        //    set { _LocalProjectPath = value; }
        //}

        //actual QVW name
        private string _QVName;
        public string QVName
        {
            get { return _QVName; }
            set { _QVName = value; }
        }

        //optional so path on server could be different, typically isn't
        private string _ServerQVPath;
        public string ServerQVPath
        {
            get { return _ServerQVPath; }
            set { _ServerQVPath = value; }
        }

        //user manual for this project
        private string _UserManual;
        public string UserManual
        {
            get { return  AbsoluteToVirtual( _UserManual ); }
            set { _UserManual = value ; }
        }

        //original names givien this user manual for this project
        private string _OriginalUserManualName;

        public string OriginalUserManualName
        {
            get { return _OriginalUserManualName; }
            set { _OriginalUserManualName = value; }
        }

        //private string _UserManualHash;
        //public string UserManualHash
        //{
        //    get { return _UserManualHash; }
        //    set { _UserManualHash = value; }
        //}

        //the groups this project should be a part of
        private string _Groups;
        public string Groups
        {
            get { return _Groups; }
            set { _Groups = value; }
        }

        //people who will review this project
        //private string _ReviewerList;
        //public string ReviewerList
        //{
        //    get { return _ReviewerList; }
        //    set { _ReviewerList = value; }
        //}

        //the actual QVW uploaded
        private string _QVProject;
        public string QVProject
        {
            get { return _QVProject; }
            set { _QVProject =  value ; }
        }

        private string _QVProjectHash;
        public string QVProjectHash
        {
            get { return _QVProjectHash; }
            set { _QVProjectHash = value; }
        }

        //additional project resources
        private string _QVResources;
        public string QVResources
        {
            get { return AbsoluteToVirtual(  _QVResources ); }
            set { _QVResources = value ; }
        }

        //private string _QVResourcesHash;
        //public string QVResourcesHash
        //{
        //    get { return _QVResourcesHash; }
        //    set { _QVResourcesHash = value; }
        //}

        //project thumbnail displayed by portal
        private string _QVThumbnail;
        public string QVThumbnail
        {
            get { return AbsoluteToVirtual( _QVThumbnail ); }
            set { _QVThumbnail =  value ; }
        }

        private string _QVThumbnailHash;
        public string QVThumbnailHash
        {
            get { return _QVThumbnailHash; }
            set { _QVThumbnailHash = value; }
        }

        //description of project shown to client users
        private string _QVDescription;
        public string QVDescription
        {
            get { return _QVDescription; }
            set { _QVDescription = value; }
        }

        //project notes - not shown to user
        private string _Notes;
        public string Notes
        {
            get { return _Notes; }
            set { _Notes = value; }
        }

        private string _CovisintFieldName;
        public string CovisintFieldName
        {
            get { return _CovisintFieldName; }
            set { _CovisintFieldName = value; }
        }

        private string _MillimanControlName;
        public string MillimanControlName
        {
            get { return _MillimanControlName; }
            set { _MillimanControlName = value; }
        }

        private string _FriendlyDBName;
        public string FriendlyDBName
        {
            get { return _FriendlyDBName; }
            set { _FriendlyDBName = value; }
        }

        private string _DBConnectionString;
        public string DBConnectionString
        {
            get { return _DBConnectionString; }
            set { _DBConnectionString = value; }
        }

        private string _LoadedFrom;
        public string LoadedFrom
        {
            get { return _LoadedFrom; }
            set { _LoadedFrom = value; }
        }

        private string _UID;
        public string UID
        {
            get { return _UID; }
            set { _UID = value; }
        }

        private string _UploadedToProduction;
        public string UploadedToProduction
        {
            get { return _UploadedToProduction; }
            set { _UploadedToProduction = value; }
        }

        private string _UploadedToProductionDate;
        public string UploadedToProductionDate
        {
            get { return _UploadedToProductionDate; }
            set { _UploadedToProductionDate = value; }
        }

        private string _ProjectCreatedBy;
        public string ProjectCreatedBy
        {
            get { return _ProjectCreatedBy; }
            set { _ProjectCreatedBy = value; }
        }

        private string _ProjectCreatedDate;
        public string ProjectCreatedDate
        {
            get { return _ProjectCreatedDate; }
            set { _ProjectCreatedDate = value; }
        }

        private string _LastProjectRun;
        public string LastProjectRun
        {
            get { return _LastProjectRun; }
            set { _LastProjectRun = value; }
        }

        private string _LastProjectRunDate;
        public string LastProjectRunDate
        {
            get { return _LastProjectRunDate; }
            set { _LastProjectRunDate = value; }
        }

        private string _LastEditedBy;
        public string LastEditedBy
        {
            get { return _LastEditedBy; }
            set { _LastEditedBy = value; }
        }

        private string _LastEditedDate;
        public string LastEditedDate
        {
            get { return _LastEditedDate; }
            set { _LastEditedDate = value; }
        }

        //private string _QVTemplate;
        //public string QVTemplate
        //{
        //    get { return _QVTemplate; }
        //    set { _QVTemplate = value; }
        //}

        private string _QVDataSource;
        public string QVDataSource
        {
            get { return _QVDataSource; }
            set { _QVDataSource = value; }
        }

        private string _QVTooltip;
        public string QVTooltip
        {
            get { return _QVTooltip; }
            set { _QVTooltip = value; }
        }

        //the right hand part of path minus root directory QVDocuments(local),  Documents(remote)
        //private string _VirtualDirectory;
        public string VirtualDirectory
        {
            get 
            {
                   string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                   string _VirtualDirectory =  LoadedFromPath.Substring(DocumentRoot.Length);
                   _VirtualDirectory = _VirtualDirectory.Replace('/', '\\');
                   while (_VirtualDirectory[0] == '\\')
                        _VirtualDirectory = _VirtualDirectory.Substring(1);
                   return _VirtualDirectory; 
            
            }
            set {  }
        }

        public string AbsoluteProjectPath { get; set; }

        //gets the path to the directory the project was loaded from
        public string LoadedFromPath
        {
            get { return System.IO.Path.GetDirectoryName(_LoadedFrom); }
        }

        private bool _AutomaticInclusion;

        public bool AutomaticInclusion
        {
            get { return _AutomaticInclusion; }
            set { _AutomaticInclusion = value; }
        }

        //Issue 1842 - all fields should be given a default value
        public ProjectSettings()
        {
            //provide good defaults for all items
            _ProjectName = string.Empty;
            _OriginalProjectName = string.Empty;
            _SupportsReduction = false;
            _QVName = string.Empty;
            _ServerQVPath = string.Empty;
            _UserManual = string.Empty;
            _OriginalUserManualName = string.Empty;
            _Groups = string.Empty;
            _QVProject = string.Empty;
            _QVProjectHash = string.Empty;
            _QVResources = string.Empty;
            _QVThumbnail = string.Empty;
            _QVThumbnailHash = string.Empty;
            _QVDescription = string.Empty;
            _Notes = string.Empty;
            _CovisintFieldName = string.Empty;
            _MillimanControlName = string.Empty;
            _FriendlyDBName = string.Empty;
            _DBConnectionString = string.Empty;
            _LoadedFrom = string.Empty;
            _UID = string.Empty;
            _UploadedToProduction = string.Empty;
            _UploadedToProductionDate = string.Empty;
            _ProjectCreatedBy = string.Empty;
            _ProjectCreatedDate = string.Empty;
            _LastProjectRun = string.Empty;
            _LastProjectRunDate = string.Empty;
            _LastEditedBy = string.Empty;
            _LastEditedDate = string.Empty;
            _QVDataSource = string.Empty;
            _QVTooltip = string.Empty;

            _AutomaticInclusion = false;  //setting backing store to false by default

            _ProjectCreatedDate = DateTime.Now.ToString();
            if (System.Web.Security.Membership.GetUser() != null) //need to check when deserializing, will throw error
                _ProjectCreatedBy = System.Web.Security.Membership.GetUser().UserName;
            _UID = "hcprj" + Guid.NewGuid().ToString("N");
        }


        static public ProjectSettings Load(string PathFilename)
        {
            if (System.IO.File.Exists(PathFilename) == false)
                return null;


            Polenter.Serialization.SharpSerializerXmlSettings SX = new Polenter.Serialization.SharpSerializerXmlSettings();
            SX.IncludeAssemblyVersionInTypeName = false;
            SX.IncludeCultureInTypeName = false;
            SX.IncludePublicKeyTokenInTypeName = false;
            
            Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(SX);
            ProjectSettings Settings = SS.Deserialize(PathFilename) as ProjectSettings;
            if (Settings != null)
            {
                Settings._LoadedFrom = PathFilename;
                Settings.AbsoluteProjectPath = System.IO.Path.GetDirectoryName(PathFilename);
            }
            return Settings;
        }

        public bool Save(string PathFilename = "")
        {
            try
            {
                LastEditedBy = Membership.GetUser().UserName;
                LastEditedDate = DateTime.Now.ToString();

                Polenter.Serialization.SharpSerializerXmlSettings SX = new Polenter.Serialization.SharpSerializerXmlSettings();
                SX.IncludeAssemblyVersionInTypeName = false;
                SX.IncludeCultureInTypeName = false;
                SX.IncludePublicKeyTokenInTypeName = false;
                Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(SX);
                
                if (string.IsNullOrEmpty(PathFilename) == true)
                    PathFilename = this._LoadedFrom;

                if (string.IsNullOrEmpty(this._LoadedFrom) == true)
                    this._LoadedFrom = PathFilename;

                SS.Serialize(this, PathFilename);

                //Issue1298 - this issue is caused by client publisher deriving off the project settings
                //class.  When serializing, it uses the type of the derived class instead of project
                //settings.  This could be corrected by a copy contructor to create a new instance, or 
                //by a custom searilize - however both are alot of work to accomplish changing
                //the searlize type - when the following 3 lines below corrects it
                var fileContents = System.IO.File.ReadAllText(PathFilename);
                fileContents = fileContents.Replace("ClientPublisher.ProjectSettingsExtension, ClientPublisher", "MillimanCommon.ProjectSettings, MillimanCommon");
                System.IO.File.WriteAllText(PathFilename, fileContents);


                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        /// <summary>
        /// this is used in the transition from the old file format to the new, old format contained
        /// the full path, we only want virtual path
        /// </summary>
        /// <param name="FullPathItem"></param>
        /// <returns></returns>
        private string AbsoluteToVirtual(string FullPathItem)
        {
            if (string.IsNullOrEmpty(FullPathItem) == true)
                return "";

            if (FullPathItem.IndexOf(':') != -1)
            { //contains colon must be c: or d: or e:......
                string VirtualDir = System.IO.Path.GetDirectoryName( VirtualDirectory );
                return System.IO.Path.Combine(VirtualDir, System.IO.Path.GetFileName(FullPathItem));
            }
            return FullPathItem;
        }
    }
}