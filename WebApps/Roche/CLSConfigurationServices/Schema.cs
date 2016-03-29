using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace CLSConfigurationServices
{
    public class Schema
    {
        public string SchemaName { get; set; }
        public string OperatorEmail { get; set; }
        public DateTime MakeLiveDateTime { get; set; }

        public Schema() { }

        public Schema ( string _SchemaName, string _OperatorEmail, DateTime _MakeLiveDateTime )
        {
            SchemaName = _SchemaName;
            OperatorEmail = _OperatorEmail;
            MakeLiveDateTime = _MakeLiveDateTime;
        }

        public bool Save( string PathFilename )
        {
            System.IO.File.Delete(PathFilename);
            JavaScriptSerializer JSS = new JavaScriptSerializer();
            System.IO.File.WriteAllText(PathFilename, JSS.Serialize(this));
            return true;
        }

        public static Schema Load( string PathFilename )
        {
            string Contents = System.IO.File.ReadAllText(PathFilename);
            JavaScriptSerializer JSS = new JavaScriptSerializer();
            Schema Current = JSS.Deserialize<Schema>(Contents);
            return Current;
        }
            
    }
}