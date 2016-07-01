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
    class CernerReferenceHealthPlanDictionaries
    {
        IMongoCollection<MongodbReferenceHealthPlanEntity> ReferenceHealthPlanCollection;
        IMongoCollection<MongodbRefCodeEntity> ReferenceCodeCollection;

        public Dictionary<String, String> PlanNameCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> PlanTypeCodeMeanings = new Dictionary<string, string>();

        #region temporary validation functions
        #endregion

        public bool Initialize(IMongoCollection<MongodbReferenceHealthPlanEntity> HealthPlanCollectionArg, IMongoCollection<MongodbRefCodeEntity> RefCodeCollectionArg, bool AddZeroUnspecified = true)
        {
            ReferenceHealthPlanCollection = HealthPlanCollectionArg;
            ReferenceCodeCollection = RefCodeCollectionArg;

            try
            {
                var q = ReferenceHealthPlanCollection.AsQueryable();

                foreach (var HealthPlanRecord in q)
                {
                    PlanNameCodeMeanings[HealthPlanRecord.UniqueHealthPlanIdentifier] = HealthPlanRecord.PlanName;

                    var TypeCodeQuery = ReferenceCodeCollection.AsQueryable().Where(x =>
                               x.FileName.ToUpper() == "REFERENCEHEALTHPLAN"
                            && x.Field.ToUpper() == "TYPE"
                            && x.ElementCode == HealthPlanRecord.Type
                        )
                        .FirstOrDefault()
                        ;

                    if (TypeCodeQuery != null)
                    {

                    }
                }
                if (AddZeroUnspecified)
                {
                    PlanNameCodeMeanings["0"] = "Unspecified";
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception in CernerReferenceHealthPlanDictionaries.InitializePlanNameDictionary: " + e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes a dictionary of codes mapped to PlanName strings from the referencehealthplan collection in MongoDB
        /// </summary>
        /// <param name="AddZeroUnspecified">Optional, true to include an entry for code "0" mapped to value "Unspecified"</param>
        /// <returns></returns>
        private bool InitializePlanNameDictionary(bool AddZeroUnspecified = true)
        {
            try
            {
                var q = ReferenceHealthPlanCollection.AsQueryable();

                foreach (var record in q)
                {
                    PlanNameCodeMeanings[record.UniqueHealthPlanIdentifier] = record.PlanName;
                }
                if (AddZeroUnspecified)
                {
                    PlanNameCodeMeanings["0"] = "Unspecified";
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception in CernerReferenceHealthPlanDictionaries.InitializePlanNameDictionary: " + e.Message);
                return false;
            }

            return true;
        }

        private bool InitializePlanTypeDictionary(bool AddZeroUnspecified = true)
        {
            try
            {
                var q = ReferenceHealthPlanCollection.AsQueryable();

                foreach (var record in q)
                {
                    PlanNameCodeMeanings[record.UniqueHealthPlanIdentifier] = record.PlanName;
                }
                if (AddZeroUnspecified)
                {
                    PlanNameCodeMeanings["0"] = "Unspecified";
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception in CernerReferenceHealthPlanDictionaries.InitializePlanNameDictionary: " + e.Message);
                return false;
            }

            return true;
        }

    }
}
