using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanCommon
{
    public class CacheEntry
    {
        private string _DBFriendlyName;

        public string DBFriendlyName
        {
            get { return _DBFriendlyName; }
            set { _DBFriendlyName = value; }
        }
        private string _ConnectionString;

        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }
        private string _SchemaDiagram;

        public string SchemaDiagram
        {
            get { return _SchemaDiagram; }
            set { _SchemaDiagram = value; }
        }

        private string _UserKey;

        public string UserKey
        {
            get { return _UserKey; }
            set { _UserKey = value; }
        }
        private string _UserName;

        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        private string _MillimanGroupName;

        public string MillimanGroupName
        {
            get { return _MillimanGroupName; }
            set { _MillimanGroupName = value; }
        }
        private DateTime _Expires;

        public DateTime Expires
        {
            get { return _Expires; }
            set { _Expires = value; }
        }

        public CacheEntry()
        {
           
        }
        public CacheEntry(string __DBFriendlyName, string __ConnectionString, string __UserKey, string __UserName, string __MillimanGroupName, DateTime __ExpiresOn)
        {
            _DBFriendlyName = __DBFriendlyName;
            _ConnectionString = __ConnectionString;
            _UserKey = __UserKey;
            _UserName = __UserName;
            _MillimanGroupName = __MillimanGroupName;
            _Expires = __ExpiresOn;

        }
        public CacheEntry(string __UserKey, string __UserName, string __MillimanGroupName, DateTime __ExpiresOn)
        {
            _DBFriendlyName = string.Empty;
            _ConnectionString = string.Empty;
            _UserKey = __UserKey;
            _UserName = __UserName;
            _MillimanGroupName = __MillimanGroupName;
            _Expires = __ExpiresOn;

        }
        public void Save(string QualifiedCacheFilename)
        {
            Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
            SS.Serialize(this, QualifiedCacheFilename +  @".xml");
        }

        public static CacheEntry Load(string QualifiedCacheFilename )
        {
            if (System.IO.File.Exists(QualifiedCacheFilename + @".xml") == false)
                return null;

            Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
            CacheEntry CE = SS.Deserialize(QualifiedCacheFilename  + @".xml") as CacheEntry;
            //System.IO.File.Delete(QualifiedCacheFilename + @".xml"); //cannot get rid of it - user may click button multiple times to view database
            return CE;
        }
    }
}
