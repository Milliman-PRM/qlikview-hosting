using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanCommon
{
    static public class ReducedQVWUtilities
    {
        //beneath the QVDocumentsReduced there is a reflection of the QVDocuments root,  however at the leaf level
        //a directory is created for each user that has a reduced QVW
        static public string GetUsersReducedQVWName(string QualifiedMasterQVW, string UserID)
        {
            string QVWPath = System.IO.Path.GetDirectoryName(QualifiedMasterQVW);      
            string ValidUserID = ValidUserDirFromID(UserID);
            return System.IO.Path.Combine(QVWPath, "ReducedUserQVWs", ValidUserID, System.IO.Path.GetFileName(QualifiedMasterQVW));
        }
        static public string GetUsersSelectionsFileName(string QualifiedMasterQVW, string UserID)
        {
            string UsersReducedQVWName = GetUsersReducedQVWName(QualifiedMasterQVW, UserID);
            return UsersReducedQVWName.ToLower().Replace(".qvw", ".selections");
        }
        static public string GetUserDir(string ProjectDirectory, string UserID)
        {
            return System.IO.Path.Combine(ProjectDirectory, "ReducedUserQVWs", ValidUserDirFromID(UserID));
        }
        static public string ValidUserDirFromID(string UserID)
        {
            return MillimanCommon.Utilities.ConvertStringToHex(UserID);

            /////we need to make sure there are not invalid chars in the user is since we will create a subdir
            /////for the user, some userids are emails, other are not
            //string ValidUserChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
            //string ValidUserID = string.Empty;
            //foreach (char C in UserID)
            //{
            //    if (ValidUserChars.Contains(C))
            //        ValidUserID += C;
            //    else
            //        ValidUserID += '_';
            //}
            //return ValidUserID;
        }

        /// <summary>
        /// This will return a FULLY qualified path to the reduced/cached QVW
        /// </summary>
        /// <param name="QualifiedMasterQVW"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        static public string ReducedQVWRedirected(string QualifiedMasterQVW, string UserID)
        {
            string ReducedQV = GetUsersReducedQVWName(QualifiedMasterQVW, UserID);
            string Wildcard = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ReducedQV), "[REFERENCE].redirect");
            if (System.IO.File.Exists(Wildcard) == true)
            {
                string RedirectedQVW = System.IO.File.ReadAllText(Wildcard);
                //securiety check, make sure the path of the master is part of the cached/reduced version, otherwise it may be pointing
                //outside to another group
                string MasterQVWPath = System.IO.Path.GetDirectoryName(QualifiedMasterQVW);
                //need to convert the relative path to absolut to test it
                string QualafiedReduced = System.IO.Path.Combine(System.Web.Configuration.WebConfigurationManager.AppSettings["QVDocumentRoot"], RedirectedQVW);
                if ((QualafiedReduced.ToLower().IndexOf(MasterQVWPath.ToLower()) >= 0) && (System.IO.File.Exists(QualafiedReduced)))
                    return RedirectedQVW; //return the path to the short path
            }

            //the directory has to have at least a reduced QVW,  or a [REFERENCE].redirect file, if not no QVW access
            //should be allowed

            return string.Empty;
        }

        static public List<string> GetHierarchyFilenames(string QualifiedQVWorHCPRJ, bool NewFilesOnly = false)
        {
            List<string> Hierarchies = new List<string>();
            string QVWName = QualifiedQVWorHCPRJ.ToLower().Replace(".hciprj", ".qvw");
            string HierRootName = QVWName.ToLower().Replace(".qvw", ".hierarchy_");
            int Index = 0;
            while (true)
            {
                string HierarchyName = HierRootName + Index.ToString();
                if (NewFilesOnly)
                    HierarchyName += "_new";
                if (System.IO.File.Exists(HierarchyName) == true)
                {
                    Hierarchies.Add(HierarchyName);
                    Index++;
                }
                else
                {
                    break;
                }
            }
            return Hierarchies;
        }

        static public string GetSelectedHierarchyFilename(string QualifiedQVW, string UserName)
        {
            string ReducedQVWName = GetUsersReducedQVWName( QualifiedQVW, UserName ).ToLower();
            return ReducedQVWName.Replace( ".qvw",".selections");
        }
    }
}
