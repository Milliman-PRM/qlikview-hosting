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

        public Dictionary<String, String> PlanNameCodeMeanings = new Dictionary<string, string>();

        #region temporary validation functions
        #endregion

        public bool Initialize(IMongoCollection<MongodbReferenceHealthPlanEntity> CollectionArg)
        {
            ReferenceHealthPlanCollection = CollectionArg;

            bool Success =
                   InitializePlanNameDictionary()
                ;

            return Success;
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

    }
}
