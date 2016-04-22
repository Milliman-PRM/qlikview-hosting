using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanReportReduction
{
    public class ReducedReportMasterIndex
    {

        public Dictionary<string, string> ReportMasterIndex { get; set; }
        public string _LoadedFrom { get; set; }

        public ReducedReportMasterIndex()
        {
            ReportMasterIndex = new Dictionary<string, string>();
        }

        /// <summary>
        /// Returns the path to a previously reduced version with these keys
        /// </summary>
        /// <param name="ReductionKeys"></param>
        /// <returns></returns>
        public string IsReduced(List<NVPairs> ReductionKeys)
        {
            return "";
        }

        public bool AddToReductionCache( string QualifiedReducedQVW, List<NVPairs> ReductionKeys )
        {
            string Key = string.Empty;
            foreach( NVPairs NVP in ReductionKeys )
            {
                Key += NVP.Name + NVP.Value;
            }

            if ( ReportMasterIndex.ContainsKey(Key) == false )
                ReportMasterIndex.Add(Key, QualifiedReducedQVW);

            Save();

            return true;
        }

        static public ReducedReportMasterIndex Load(string PathFilename)
        {
            PathFilename = PathFilename.ToLower().Replace(@".qvw", @".xml");
            if (System.IO.File.Exists(PathFilename) == false)
            {
                //we don't have an index must be the first time we have run, so create an empty index
                ReducedReportMasterIndex RMI = new ReducedReportMasterIndex();
                Polenter.Serialization.SharpSerializer NewSS = new Polenter.Serialization.SharpSerializer(false);
                NewSS.Serialize(RMI, PathFilename);
                NewSS = null;
            }

            Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
            ReducedReportMasterIndex Settings = SS.Deserialize(PathFilename) as ReducedReportMasterIndex;
            if (Settings != null)
                Settings._LoadedFrom = PathFilename;
            return Settings;
        }

        public bool Save(string PathFilename = "")
        {
            try
            {
                Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                if (string.IsNullOrEmpty(PathFilename) == true)
                    PathFilename = this._LoadedFrom;

                if (string.Compare(System.IO.Path.GetExtension(PathFilename), ".xml", true) != 0)
                    PathFilename = PathFilename.Replace(System.IO.Path.GetExtension(PathFilename), ".xml");
                SS.Serialize(this, PathFilename);

                return true;
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "ReducedReportMasterIndex", ex);
            }
            return false;
        }
    }
}
