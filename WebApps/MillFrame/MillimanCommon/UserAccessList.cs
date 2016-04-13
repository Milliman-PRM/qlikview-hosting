using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Configuration;

namespace MillimanCommon
{
    public class UserAccessList
    {

        public class UserAccess
        {
            private UserRepo.QVEntity _QVEntity;

            public UserRepo.QVEntity QVEntity
            {
                get { return _QVEntity; }
                set { _QVEntity = value; }
            }

            private ProjectSettings _ProjectSettings;

            public ProjectSettings ProjectSettings
            {
                get { return _ProjectSettings; }
                set { _ProjectSettings = value; }
            }

           
            //this access list is specific for this qualified project
            private string _ProjectPath;

            public string ProjectPath
            {
                get { return _ProjectPath; }
                set { _ProjectPath = value; }
            }

            public string QVRootRelativeProjectPath
            {
                get
                {
                    string RepoFilePath = ConfigurationManager.AppSettings["QVDocumentRoot"];
                    if (string.IsNullOrEmpty(RepoFilePath) == false)
                    {
                        return _ProjectPath.Substring(RepoFilePath.Length+1); //remote the root to make relative
                    }
                    return _ProjectPath;
                }
            }

            private string _ReducedQVW;

            public string ReducedQVW
            {
                get { return _ReducedQVW; }
                set { _ReducedQVW = value.ToLower(); }
            }

            private bool _ReducedVersionNotAvailable;

            public bool ReducedVersionNotAvailable
            {
                get { return _ReducedVersionNotAvailable; }
                set { _ReducedVersionNotAvailable = value; }
            }

            public string QVReducedRelativeProjectPath
            {
                get
                {
                    string RepoFilePath = ConfigurationManager.AppSettings["QVDocumentRoot"];

                    if (string.IsNullOrEmpty(_ReducedQVW) == false)
                    {
                        if ( _ReducedQVW.Contains(':') == true ) //absolute path, make it relative to QV root
                            return _ReducedQVW.Substring(RepoFilePath.Length + 1);
                        return _ReducedQVW;  //already relative
                    }
    
                    return string.Empty;  //error, could not find QVW
                }
            }
            public UserAccess( string QVFile, UserRepo.QVEntity _QV, string ReducedQVFile )
            {
                _ProjectPath = QVFile.ToLower();;
                _QVEntity = _QV;
                _ReducedVersionNotAvailable = false;
                ReducedQVW = ReducedQVFile;
                string Settings = _ProjectPath.Replace(".qvw", ".hciprj");
                if (System.IO.File.Exists(Settings) == true)
                    _ProjectSettings = ProjectSettings.Load(Settings);
                else
                    Report.Log(Report.ReportType.Error, "Missing QVW file descriptor - " + Settings);
            }

            /// <summary>
            /// called when reduced version is not available
            /// </summary>
            /// <param name="QVFile"></param>
            /// <param name="_QV"></param>
            public UserAccess(string QVFile, UserRepo.QVEntity _QV)
            {
                _ProjectPath = QVFile.ToLower(); ;
                _QVEntity = _QV;
                _ReducedVersionNotAvailable = true;
                string Settings = _ProjectPath.Replace(".qvw", ".hciprj");
                if (System.IO.File.Exists(Settings) == true)
                    _ProjectSettings = ProjectSettings.Load(Settings);
                else
                    Report.Log(Report.ReportType.Error, "Missing QVW file descriptor - " + Settings);
            }
        }


        private List<UserAccess> _ACL = new List<UserAccess>();

        public List<UserAccess> ACL
        {
            get { return _ACL; }
            set { _ACL = value; }
        }

        public UserAccessList(string UserName, string[] UserRoles, bool IsClientAdmin )
        {
            ResolveACL(UserName, UserRoles, IsClientAdmin );
        }

        /// <summary>
        /// get a list of all the objects the user can access, this list will
        /// have 1 entry for each QVW file accessable
        /// </summary>
        /// <param name="UserName"></param>
        private void ResolveACL(string UserName, string[] UserRoles, bool IsClientAdmin )
        {
            UserRepo UR = UserRepo.GetInstance();
            List<ExpandoObject> QVFiles = UR.FindAllQVProjectsForUser(UserName, UserRoles, !IsClientAdmin);
            //Any group that exists in strictreductionrulegroups requires each user have a reduced/linked QVW, otherwise
            //users can see the master QVW
            string StrictGroups = System.Configuration.ConfigurationManager.AppSettings["StrictReductionRuleGroups"].ToLower();
            foreach (dynamic EO in QVFiles)
            {
                string QVWRole = EO.Role;
                if (StrictGroups.IndexOf(QVWRole.ToLower()) == -1)  //NOT strict rules
                {
                    if ( string.IsNullOrEmpty( EO.ReducedQVFilename ) == false )
                        _ACL.Add(new UserAccess(EO.QVFilename, EO.QVEntity, EO.ReducedQVFilename)); //show reduced version
                    else
                        _ACL.Add(new UserAccess(EO.QVFilename, EO.QVEntity, EO.QVFilename));      //show master version
                }
                else  //STRICT RULES ENFORCED, Must have reduced version of link
                {
                    if (string.IsNullOrEmpty(EO.ReducedQVFilename) == false)
                        _ACL.Add(new UserAccess(EO.QVFilename, EO.QVEntity, EO.ReducedQVFilename)); //show reduced version
                    else
                        _ACL.Add(new UserAccess(EO.QVFilename, EO.QVEntity)); //show nothing
                }
                //client admins always see the master version, if a reduced version is not available
                //if ((IsClientAdmin) && (string.IsNullOrEmpty(EO.ReducedQVFilename)==true))
                //    _ACL.Add(new UserAccess(EO.QVFilename, EO.QVEntity, EO.QVFilename));
                //else
                //    _ACL.Add(new UserAccess(EO.QVFilename, EO.QVEntity, EO.ReducedQVFilename));
            }
        }

        static public string[] GetRolesForUser(string UserName = "")
        {
            //if UserName is empty, get roles for current logged in user, all that in 1 method call, WOW
            string[] UserRoles;
            if (string.IsNullOrEmpty(UserName))
                UserRoles = System.Web.Security.Roles.GetRolesForUser(); //roles for currently logged in user
            else
                UserRoles = System.Web.Security.Roles.GetRolesForUser(UserName); //roles for specified user

            bool IsAdmin = false;
            foreach (string Role in UserRoles)
            {
                if (string.Compare(Role, "Administrator", true) == 0)
                {
                    IsAdmin = true;
                    break;
                }
            }

            if (IsAdmin == true)
            {
                UserRoles = System.Web.Security.Roles.GetAllRoles();
            }
            return UserRoles;
        }
    }
}
