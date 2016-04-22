using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Dynamic;

namespace MillimanCommon
{
    public partial class UserRepo
    {

        /// <summary>
        /// called by assertion page, pinged by covisint,  they will provide the name of the report
        /// we need to find where that version is located that has been assigned to this user
        /// 
        /// Code will look to see if stored data is QV project,  Role, or Directory
        /// if Role - code will look up all QV projects in role
        /// if Directory - code will enumerate all files in directory for QV files
        /// 
        /// We return the first item we find thay can access by the name QVProjectName
        /// File extensions must be QVW
        ///  File paths are relative to QV root
        /// </summary>
        /// <param name="UserName">Covisint Enterprise ID</param>
        /// <param name="QVProjectName">Always a project name - comes from Covisint application</param>
        /// <returns></returns>
        public string FindQualifedQVProject(string UserName, string QVProjectName, string[] Roles )
        {
            List<ExpandoObject> AllProjects = FindAllQVProjectsForUser(UserName, Roles);
            if (AllProjects != null)
            {
                foreach (dynamic EO in AllProjects)
                {
                    string UserProject = EO.QVFilename;
                    
                    string ProjectFile = Path.GetFileNameWithoutExtension(UserProject) + @".hciprj";
                    if (string.Compare(QVProjectName, ProjectFile, true) == 0)
                        return EO.ReducedQVFilename;
                }
            }
            return "";
        }

        /// <summary>
        /// Find all the QVWS associated with a role
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        public List<string> FindAllQVProjectsForRole(string Role)
        {
            List<string> Projects = new List<string>();
            if (RoleMap.ContainsKey(Role))
            {
                string QVRoot = ConfigurationManager.AppSettings["QVDocumentRoot"];
                List<QVEntity> RoleItems = RoleMap[Role];
                foreach (QVEntity Q in RoleItems)
                {
                    string PathFile = System.IO.Path.Combine(QVRoot, Q.QualifiedPathName);
                    if (File.Exists(PathFile))
                    {
                        Projects.Add(PathFile);
                    }
                }
            }
            return Projects;
        }

        public List<string> FindAllProjectsForRole(string Role)
        {
            List<string> All = FindAllQVProjectsForRole(Role);
            //this works because of the 1 to 1 mapping between QVWs and projects, just change the extension
            return All.Select(x => x.Replace(@".qvw", @".hciprj")).ToList();
        }

        /// <summary>
        /// Get all the QV projects for this user,  file extensions must be QVW
        
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="UserRoles">User roles are now stored in the database</param>
        /// <returns>A list of expand objects with attributes: QVEntity QVFilename</returns>
        public List< ExpandoObject > FindAllQVProjectsForUser(string UserName, string[] UserRoles = null, bool GetReducedVersions = true )
        {
            
            string QVRoot = ConfigurationManager.AppSettings["QVDocumentRoot"];
            List<ExpandoObject> QVs = new List<ExpandoObject>();

            //if we get some roles,  look up the projects and master QVWs
            if (UserRoles != null)
            {
                foreach (string Role in UserRoles)
                {
                    if (RoleMap.ContainsKey(Role))
                    {
                        List<QVEntity> RoleItems = RoleMap[Role];
                        foreach (QVEntity Q in RoleItems)
                        {
                            string PathFile = System.IO.Path.Combine(QVRoot, Q.QualifiedPathName);
                            if (File.Exists(PathFile))
                            {
                                dynamic EO = new ExpandoObject();
                                EO.QVEntity = Q;
                                EO.QVFilename = PathFile;
                                EO.Role = Role;
                                EO.ReducedQVFilename = string.Empty;
                                QVs.Add(EO);
                            }
                        }
                    }
                }
            }

            if (UserMap.ContainsKey(UserName))
            {
                List<QVEntity> UserList = UserMap[UserName];
                foreach (QVEntity QVE in UserList)
                {
                    if ((QVE.IsRole == false) && (QVE.IsDirectory == false))  //must be a file
                    {
                        dynamic EO = new ExpandoObject();
                        EO.QVEntity = QVE;
                        EO.Role = "[Direct]";
                        EO.QVFilename = System.IO.Path.Combine(QVRoot, QVE.QualifiedPathName);
                        EO.ReducedQVFilename = string.Empty;
                        if ( File.Exists(EO.QVFilename) )
                            QVs.Add(EO);
                    }
                        //this was removed due to possible security issues - VWN
                    //else if (QVE.IsDirectory == true)
                    //{  //we should get rid of this, will cause security issues
                    //      string[] DirItems = Directory.GetFiles(QVE.QualifiedPathName, "*.qvw", SearchOption.AllDirectories);
                    //      foreach (string QVItem in DirItems)
                    //      {
                    //          dynamic EO = new ExpandoObject();
                    //          EO.QVEntity = QVE;
                    //          EO.Role = "[Directory]";
                    //          EO.QVFilename = QVItem.Substring(QVRoot.Length);
                    //          //because we used GetFiles should already be qualified
                    //          if (File.Exists(EO.QVFilename)) 
                    //              QVs.Add(EO); //make relative to qv root
                    //      }
                    //}
                    else if (QVE.IsRole == true)
                    {  //don't think we use this
                           List<QVEntity> RoleItems = RoleMap[QVE.QualifiedPathName];
                           foreach (QVEntity Q in RoleItems)
                           {
                               dynamic EO = new ExpandoObject();
                               EO.QVEntity = QVE;
                               EO.Role = "[FileRole]";
                               EO.QVFilename = System.IO.Path.Combine(QVRoot, QVE.QualifiedPathName);
                               EO.ReducedQVFilename = string.Empty;
                               if (File.Exists(EO.QVFilename)) 
                                   QVs.Add(EO);
                           }
                    }
                }
            }

            if (GetReducedVersions)
            {
                //now loop over QVWs and check for reduced version, if not available then remove entry
                for (int Index = QVs.Count - 1; Index >= 0; Index--)
                {
                    dynamic EOI = QVs[Index]; 
                    EOI.ReducedQVFilename = ReducedQVWUtilities.ReducedQVWRedirected(EOI.QVFilename, UserName); //returns empty if not exist
                }
            }
            return QVs;
        }

        /// <summary>
        /// return a list of all the projects that exist in this role
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        public List<string> GetProjectsInRole(string Role)
        {
            List<string> AllProjects = new List<string>();
            if (RoleMap.ContainsKey(Role))
            {
                List<QVEntity> RoleItems = RoleMap[Role];
                foreach (QVEntity Q in RoleItems)
                {
                    AllProjects.Add(Q.QualifiedPathName);
                }
            }
            return AllProjects;
        }

        /// <summary>
        /// Returns true/false if QVW is utilized by a group or directy by user
        /// </summary>
        /// <param name="FullyQualifiedQVW"></param>
        /// <returns></returns>
        public bool IsQVWUsed(string FullyQualifiedQVW)
        {
            try
            {
                string QVRoot = ConfigurationManager.AppSettings["QVDocumentRoot"];

                foreach (KeyValuePair<string, List<QVEntity>> KVP in RoleMap)
                {
                    foreach (QVEntity QVE in KVP.Value)
                    {
                        string FullPath = System.IO.Path.Combine(QVRoot, QVE.QualifiedPathName);
                        if (string.Compare(FullPath, FullyQualifiedQVW, true) == 0)
                            return true;
                    }
                }
                foreach (KeyValuePair<string, List<QVEntity>> KVP in UserMap)
                {
                    foreach (QVEntity QVE in KVP.Value)
                    {
                        string FullPath = System.IO.Path.Combine(QVRoot, QVE.QualifiedPathName);
                        if (string.Compare(FullPath, FullyQualifiedQVW, true) == 0)
                            return true;
                    }
                }
            }
            catch (Exception ex )
            {
                MillimanCommon.Report.Log(Report.ReportType.Error, "Unspecified", ex);
                throw;
            }

            return false;
        }

        /// <summary>
        /// Get a list of all the master QVWs that are in use by users
        /// Utilized by UAC to set rules
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllQVWs()
        {
            List<string> QVWList = new List<string>();

            foreach (KeyValuePair<string, List<QVEntity>> KVP in RoleMap)
            {
                foreach (QVEntity QE in KVP.Value)
                {
                    if (QVWList.Contains(QE.QualifiedPathName) == false)
                        QVWList.Add(QE.QualifiedPathName);
                }
            }
            foreach (KeyValuePair<string, List<QVEntity>> KVP in UserMap)
            {
                foreach (QVEntity QE in KVP.Value)
                {
                    if (QVWList.Contains(QE.QualifiedPathName) == false)
                        QVWList.Add(QE.QualifiedPathName);
                }
            }

            return QVWList;
        }
    }
}
