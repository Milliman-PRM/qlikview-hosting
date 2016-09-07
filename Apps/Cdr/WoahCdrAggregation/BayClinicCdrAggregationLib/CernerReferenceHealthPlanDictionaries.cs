using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace BayClinicCernerAmbulatory
{
    class CernerReferenceHealthPlanDictionaries
    {
        IMongoCollection<MongodbReferenceHealthPlanEntity> ReferenceHealthPlanCollection;
        IMongoCollection<MongodbRefCodeEntity> ReferenceCodeCollection;

        public Dictionary<String, String> PlanNameCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> PlanTypeCodeMeanings = new Dictionary<string, string>();

        public bool Initialize(IMongoCollection<MongodbReferenceHealthPlanEntity> HealthPlanCollectionArg, IMongoCollection<MongodbRefCodeEntity> RefCodeCollectionArg, bool AddZeroUnspecified = true)
        {
            ReferenceHealthPlanCollection = HealthPlanCollectionArg;
            ReferenceCodeCollection = RefCodeCollectionArg;

            try
            {
                var HealthPlanQuery = ReferenceHealthPlanCollection.AsQueryable();

                foreach (MongodbReferenceHealthPlanEntity HealthPlanDoc in HealthPlanQuery)
                {
                    PlanNameCodeMeanings[HealthPlanDoc.UniqueHealthPlanIdentifier] = HealthPlanDoc.PlanName;

                    var PlanTypeDoc = ReferenceCodeCollection.AsQueryable()
                        .Where(x =>
                               x.FileName.ToUpper() == "REFERENCEHEALTHPLAN"
                            && x.Field.ToUpper() == "TYPE"
                            && x.ElementCode == HealthPlanDoc.Type
                        )
                        .Take(1)    // All instances of the same code must map to the same meaning so just ask for one
                        .FirstOrDefault();

                    if (PlanTypeDoc != null)
                    {
                        PlanTypeCodeMeanings[HealthPlanDoc.UniqueHealthPlanIdentifier] = PlanTypeDoc.Display;
                    }
                }

                if (AddZeroUnspecified)
                {
                    PlanNameCodeMeanings["0"] = "Unspecified";
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception in CernerReferenceHealthPlanDictionaries.Initialize(): " + e.Message);
                return false;
            }

            return true;
        }

    }
}
