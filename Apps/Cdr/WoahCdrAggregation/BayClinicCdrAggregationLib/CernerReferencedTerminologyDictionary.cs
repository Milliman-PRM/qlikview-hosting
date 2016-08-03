using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using CdrContext;
using CdrDbLib;
using System.Media;

namespace BayClinicCernerAmbulatory
{
    class CernerReferencedTerminologyDictionaries
    {
        IMongoCollection<MongodbReferenceTerminologyEntity> RefTerminologyCollection;

        public Dictionary<String, String> TerminologyConceptMeaning = new Dictionary<string, string>();
        public Dictionary<String, String> TerminologyCodeMeaning = new Dictionary<string, string>();

        public bool Initialize(IMongoCollection<MongodbReferenceTerminologyEntity> CollectionArg, bool AddZeroUnspecified = true)
        {
            RefTerminologyCollection = CollectionArg;
            try
            {
                var TerminologyQuery = RefTerminologyCollection.AsQueryable();
                foreach(MongodbReferenceTerminologyEntity TerminologyDoc in TerminologyQuery)
                {
                    TerminologyConceptMeaning[TerminologyDoc.UniqueTerminologyIdentifier] = TerminologyDoc.Concept;
                    TerminologyCodeMeaning[TerminologyDoc.UniqueTerminologyIdentifier] = TerminologyDoc.Code;
                }
                if (AddZeroUnspecified)
                {
                    TerminologyConceptMeaning["0"] = "Unspecified";
                    TerminologyCodeMeaning["0"] = "Unspecified";
                }

            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception in CernerReferenceTerminologyCodeDictionaries.Initialize(): " + e.Message);
                return false;
            }


            return true;
        }
    }
}
