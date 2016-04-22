using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ProjectSettings
/// </summary>
public class ProjectSettings
{

    //private string _QVProject;

    //public string QVProject
    //{
    //    get { return _QVProject; }
    //    set { _QVProject = value; }
    //}
    //private string _QVResources;

    //public string QVResources
    //{
    //    get { return _QVResources; }
    //    set { _QVResources = value; }
    //}
    //private string _QVThumbnail;

    //public string QVThumbnail
    //{
    //    get { return _QVThumbnail; }
    //    set { _QVThumbnail = value; }
    //}
    //private string _QVDescription;

    //public string QVDescription
    //{
    //    get { return _QVDescription; }
    //    set { _QVDescription = value; }
    //}

    //private string _CovisintFieldName;

    //public string CovisintFieldName
    //{
    //    get { return _CovisintFieldName; }
    //    set { _CovisintFieldName = value; }
    //}
    //private string _MillimanControlName;

    //public string MillimanControlName
    //{
    //    get { return _MillimanControlName; }
    //    set { _MillimanControlName = value; }
    //}
    //private string _FriendlyDBName;

    //public string FriendlyDBName
    //{
    //    get { return _FriendlyDBName; }
    //    set { _FriendlyDBName = value; }
    //}
    //private string _DBConnectionString;

    //public string DBConnectionString
    //{
    //    get { return _DBConnectionString; }
    //    set { _DBConnectionString = value; }
    //}

    //private string _LoadedFrom;

    //public string LoadedFrom
    //{
    //    get { return _LoadedFrom; }
    //    set { _LoadedFrom = value; }
    //}


    //public ProjectSettings()
    //{
    //    //
    //    // TODO: Add constructor logic here
    //    //
    //}

    //static public ProjectSettings Load(string PathFilename)
    //{
    //    PathFilename = PathFilename.ToLower().Replace(@".qvw", @".xml");
    //    if (System.IO.File.Exists(PathFilename) == false)
    //        return null;

    //    Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
    //    ProjectSettings Settings = SS.Deserialize(PathFilename) as ProjectSettings;
    //    if (Settings != null)
    //        Settings._LoadedFrom = PathFilename;
    //    return Settings;
    //}

    //public bool Save(string PathFilename = "")
    //{
    //    try
    //    {
    //        Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
    //        if (string.IsNullOrEmpty(PathFilename) == true)
    //            PathFilename = this._LoadedFrom;
            
    //        if ( string.Compare( System.IO.Path.GetExtension( PathFilename ), ".xml", true ) != 0 )
    //            PathFilename = PathFilename.Replace( System.IO.Path.GetExtension( PathFilename ), ".xml");
    //        SS.Serialize(this, PathFilename);

    //        return true;
    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //    return false;
    //}
}