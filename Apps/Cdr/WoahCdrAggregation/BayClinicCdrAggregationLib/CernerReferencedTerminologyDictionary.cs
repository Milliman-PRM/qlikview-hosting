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
        public Dictionary<String, String> TerminologyTerminologyCode = new Dictionary<string, string>();

        public bool Initialize(IMongoCollection<MongodbReferenceTerminologyEntity> CollectionArg, bool AddZeroUnspecified = true)
        {
            RefTerminologyCollection = CollectionArg;
            try
            {
                // TODO convert this query to aggregation pipeline and group by identifier to speed up the processing of returned docs
                var TerminologyQuery = RefTerminologyCollection.AsQueryable();
                foreach(MongodbReferenceTerminologyEntity ReferenceTerminologyDoc in TerminologyQuery)
                {
                    TerminologyConceptMeaning[ReferenceTerminologyDoc.UniqueTerminologyIdentifier] = ReferenceTerminologyDoc.Concept;
                    TerminologyCodeMeaning[ReferenceTerminologyDoc.UniqueTerminologyIdentifier] = ReferenceTerminologyDoc.Code;
                    TerminologyTerminologyCode[ReferenceTerminologyDoc.UniqueTerminologyIdentifier] = ReferenceTerminologyDoc.Terminology;
                }
                if (AddZeroUnspecified)
                {
                    TerminologyConceptMeaning["0"] = "Unspecified";
                    TerminologyCodeMeaning["0"] = "Unspecified";
                    TerminologyTerminologyCode["0"] = "Unspecified";
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
