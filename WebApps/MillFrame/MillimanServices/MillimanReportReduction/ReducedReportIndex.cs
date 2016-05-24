using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanReportReduction
{
    /// <summary>
    /// Each reduced qvw will have an xml file describing the qvw 
    /// and containing keys such that it can be re-generated
    /// </summary>
    //public class ReducedReportIndex
    //{
    //    public string QualifiedMasterQVW { get; set; }
    //    public string GroupID { get; set; }
    //    public string ReducedQVW { get; set; }
    //    public string UserID { get; set; }

    //    public string ReducedByUser { get; set; }
    //    public DateTime ReducedByTime { get; set; }

    //    public DateTime ReloadTime { get; set; }

    //    public List<NVPairs> ReductionKeys { get; set; }
    //    private string _LoadedFrom;

    //    public ReducedReportIndex()
    //    {
    //        ReductionKeys = new List<NVPairs>();
    //    }

    //    static public ReducedReportIndex Load(string PathFilename)
    //    {
    //        PathFilename = PathFilename.ToLower().Replace(@".qvw", @".xml");
    //        if (System.IO.File.Exists(PathFilename) == false)
    //            return null;

    //        Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
    //        ReducedReportIndex Settings = SS.Deserialize(PathFilename) as ReducedReportIndex;
    //        if (Settings != null)
    //            Settings._LoadedFrom = PathFilename;
    //        return Settings;
    //    }

    //    public bool Save(string PathFilename = "")
    //    {
    //        try
    //        {
    //            Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
    //            if (string.IsNullOrEmpty(PathFilename) == true)
    //                PathFilename = this._LoadedFrom;

    //            if (string.Compare(System.IO.Path.GetExtension(PathFilename), ".xml", true) != 0)
    //                PathFilename = PathFilename.Replace(System.IO.Path.GetExtension(PathFilename), ".xml");
    //            SS.Serialize(this, PathFilename);

    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "ReducedReportCatIndex", ex);
    //        }
    //        return false;
    //    }
    //}
}
