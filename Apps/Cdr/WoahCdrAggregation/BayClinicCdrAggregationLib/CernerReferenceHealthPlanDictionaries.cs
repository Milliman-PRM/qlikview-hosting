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

        public Dictionary<String, String> GenderCodeMeanings = new Dictionary<string, string>();
        //public Dictionary<String, String> ...CodeMeanings = new Dictionary<string, string>();

        #region temporary validation functions


        #endregion

        public bool Initialize(IMongoCollection<MongodbReferenceHealthPlanEntity> CollectionArg)
        {
            ReferenceHealthPlanCollection = CollectionArg;

            bool Success =
                   InitializeReferenceHealthPlanDictionary(ref GenderCodeMeanings)
                ;

            return Success;
        }

        /// <summary>
        /// Initializes a dictionary of codes and corresponding display values from the MongoDB referencecode collection, eliminating redundancy
        /// </summary>
        /// <param name="FileName">The value of the 'file_name' field to use for grouping</param>
        /// <param name="Fields">The values of 0 or more case sensitive 'field' values to search</param>
        /// <param name="Dict">The dictionary<string,string> to be populated with query results</param>
        /// <returns></returns>
        private bool InitializeReferenceHealthPlanDictionary(ref Dictionary<String, String> Dict, bool AddZeroUnspecified = true)
        {
            // TODO get this function right
            try
            {
                var q = ReferenceHealthPlanCollection.AsQueryable()
                    .Where(x => x.FileName.ToUpper() == FileName && Fields.Contains(x.Field))
                    .GroupBy(id => new { ElementCode = id.ElementCode, Display = id.Display })
                    .Select(x => new { x.Key.ElementCode, x.Key.Display });

                foreach (var record in q)
                {
                    Dict[record.ElementCode] = record.Display;
                }
                if (AddZeroUnspecified)
                {
                    Dict["0"] = "Unspecified";
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception in CernerReferencedCodeDictionaries.InitializeCodeDictionary: " + e.Message);
                return false;
            }

            return true;
        }

    }
}
