using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using CdrContext;

namespace BayClinicCernerAmbulatory
{
    class CernerReferencedCodeDictionaries
    {
        Dictionary<String, String> GenderCodeMeanings = new Dictionary<string, string>();
        IMongoCollection<MongodbRefCodeEntity> RefCodeCollection;

        public bool Initialize(IMongoCollection<MongodbRefCodeEntity> CollectionArg)
        {
            bool Success;
            RefCodeCollection = CollectionArg;

            Success = InitializeGenderCodes();

            return Success;
        }

        private bool InitializeGenderCodes()
        {
            // Extract referenced codes to 
            var q = RefCodeCollection.AsQueryable()
                .Where(x => x.FileName.ToUpper() == "PERSON" && x.Field.ToUpper() == "GENDER")
                .GroupBy(id => new { ElementCode = id.ElementCode, Display = id.Display })
                .Select(x => new { x.Key.ElementCode, x.Key.Display });
            foreach (var record in q)
            {
                GenderCodeMeanings[record.ElementCode] = record.Display;
            }
            return true;
        }

        public Gender GetCdrGenderEnum(String CernerCode)
        {
            if (GenderCodeMeanings.ContainsKey(CernerCode))
            {
                switch (GenderCodeMeanings[CernerCode].ToLower())
                {
                    case "female":
                        return Gender.Female;
                    case "male":
                        return Gender.Male;
                    default:
                        return Gender.Unspecified;
                }
            }
            else
            {
                return Gender.Unspecified;
            }
        }
    }
}
