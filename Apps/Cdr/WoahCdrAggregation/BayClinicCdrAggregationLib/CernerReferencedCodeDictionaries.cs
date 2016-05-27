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

namespace BayClinicCernerAmbulatory
{
    class CernerReferencedCodeDictionaries
    {
        IMongoCollection<MongodbRefCodeEntity> RefCodeCollection;

        public Dictionary<String, String> GenderCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> DeceasedCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> BirthDatePrecisionCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> DeceasedDatePrecisionCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> MaritalStatusCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> RaceCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> LanguageCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> EthnicityCodeMeanings = new Dictionary<string, string>();
        //public Dictionary<String, String> ...CodeMeanings = new Dictionary<string, string>();

        #region temporary validation functions

        private void ValidateRefCodeFieldList(String FileName, int TestCount)
        {
            List<String> Fields = new List<string>();

            var q = RefCodeCollection.AsQueryable()
                .Where(x => x.FileName.ToUpper() == FileName)
                .GroupBy(id => id.Field)
                ;
            foreach (var record in q)
            {
                Fields.Add(record.Key);
            }

            Trace.WriteLineIf(Fields.Count > 0, FileName + " field list has fields: " + String.Join(", ", Fields));
            if (Fields.Count != TestCount)
                throw new Exception();
        }

        #endregion

        public bool Initialize(IMongoCollection<MongodbRefCodeEntity> CollectionArg)
        {
            RefCodeCollection = CollectionArg;

            ValidateRefCodeFieldList("PERSON", 12);

            bool Success =
                   InitializeCodeDictionary("PERSON", new String[] {"GENDER" },                  ref GenderCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"DECEASED" },                ref DeceasedCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"BIRTH_DATE_PRECISION" },    ref BirthDatePrecisionCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"DECEASED_DATE_PRECISION" }, ref DeceasedDatePrecisionCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"MARITAL_STATUS", "ADDITIONAL_MARITAL_STATUS" }, ref MaritalStatusCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"RACE",           "ADDITIONAL_RACE"           }, ref RaceCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"LANGUAGE",       "ADDITIONAL_LANGUAGE"       }, ref LanguageCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"ETHNICITY",      "ADDITIONAL_ETHNICITY"      }, ref EthnicityCodeMeanings)

                // Likely these won't be present in the ambulatory extract?
                //...&& InitializeCodeDictionary("PERSON", "HOME_ADDRESS", ref CodeMeanings)
                //...&& InitializeCodeDictionary("PERSON", "IDENTIFIERS", ref CodeMeanings)  // ???
                //...&& InitializeCodeDictionary("PERSON", "STATUS", ref CodeMeanings)  // ???
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
        private bool InitializeCodeDictionary(String FileName, String[] Fields, ref Dictionary<String, String> Dict, bool AddZeroUnspecified = true)
        {
            try
            {
                var q = RefCodeCollection.AsQueryable()
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
                Trace.WriteLine("Exception in InitializeCodeDictionary.CernerReferencedCodeDictionaries: " + e.Message);
                return false;
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
                        Trace.WriteLine("Unsupported Patient-Gender encountered: " + GenderCodeMeanings[CernerCode]);
                        return Gender.Unspecified;
                }
            }
            else
            {
                return Gender.Unspecified;
            }
        }

        public MaritalStatus GetCdrMaritalStatusEnum(String CernerCode)
        {
            if (MaritalStatusCodeMeanings.ContainsKey(CernerCode))
            {
                switch (MaritalStatusCodeMeanings[CernerCode].ToLower())
                {
                    case "single":
                        return MaritalStatus.Single;
                    case "married":
                        return MaritalStatus.Married;
                    case "divorced":
                        return MaritalStatus.Divorced;
                    case "partner deceased":
                        return MaritalStatus.Widowed;  // is this right?
                    case "widowed":
                        return MaritalStatus.Widowed;
                    case "life partner":
                        return MaritalStatus.DomesticPartner;
                    case "separated":
                        return MaritalStatus.Separated;
                    case "never married":
                        return MaritalStatus.Single;
                    case "other":
                        return MaritalStatus.Other;
                    case "annulled":
                        return MaritalStatus.Divorced;

                    default:
                        Trace.WriteLine("Unsupported Patient-MaritalStatus encountered: " + MaritalStatusCodeMeanings[CernerCode]);
                        return MaritalStatus.Unspecified;
                }
            }
            else
            {
                return MaritalStatus.Unspecified;
            }
        }

    }
}
