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
        public Dictionary<String, String> PhoneTypeCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> AddressTypeCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> IdentifierTypeCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> IdentifierGroupCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> VisitLocationCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> ChargeDetailTypeCodeMeanings = new Dictionary<string, string>();
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

            /*
            if (Fields.Count != TestCount)
                throw new Exception();
            */
        }

        #endregion

        public bool Initialize(IMongoCollection<MongodbRefCodeEntity> CollectionArg)
        {
            RefCodeCollection = CollectionArg;

            ValidateRefCodeFieldList("PERSON", 12);

            bool Success =
                   InitializeCodeDictionary("PERSON", new String[] {"GENDER"},                  ref GenderCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"DECEASED"},                ref DeceasedCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"BIRTH_DATE_PRECISION"},    ref BirthDatePrecisionCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"DECEASED_DATE_PRECISION"}, ref DeceasedDatePrecisionCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"MARITAL_STATUS", "ADDITIONAL_MARITAL_STATUS"}, ref MaritalStatusCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"RACE",           "ADDITIONAL_RACE"          }, ref RaceCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"LANGUAGE",       "ADDITIONAL_LANGUAGE"      }, ref LanguageCodeMeanings)
                && InitializeCodeDictionary("PERSON", new String[] {"ETHNICITY",      "ADDITIONAL_ETHNICITY"     }, ref EthnicityCodeMeanings)
                // Likely these won't be present in the ambulatory extract?
                //&& InitializeCodeDictionary("PERSON", "HOME_ADDRESS", ref CodeMeanings)
                //&& InitializeCodeDictionary("PERSON", "IDENTIFIERS", ref CodeMeanings)  // ???
                //&& InitializeCodeDictionary("PERSON", "STATUS", ref CodeMeanings)  // ???

                && InitializeCodeDictionary("PHONE",   new String[] {"TYPE"}, ref PhoneTypeCodeMeanings)

                && InitializeCodeDictionary("ADDRESS", new String[] {"TYPE"}, ref AddressTypeCodeMeanings)

                && InitializeCodeDictionary("IDENTIFIERS", new String[] { "IDENTIFIER_TYPE" }, ref IdentifierTypeCodeMeanings)
                && InitializeCodeDictionary("IDENTIFIERS", new String[] { "IDENTIFIER_GROUP" }, ref IdentifierGroupCodeMeanings)

                && InitializeCodeDictionary("VISIT", new String[] { "LOCATION_CODE" }, ref VisitLocationCodeMeanings)

                && InitializeCodeDictionary("CHARGEDETAIL", new String[] { "TYPE" }, ref ChargeDetailTypeCodeMeanings)
                ;
            
            Trace.WriteLine("Identifier Typecodes dictionary has values: " + String.Join(", ", IdentifierTypeCodeMeanings));
            Trace.WriteLine("Identifier Groupcodes dictionary has values: " + String.Join(", ", IdentifierGroupCodeMeanings));
            Trace.WriteLine("ChargeDetail Type dictionary has values: " + String.Join(", ", ChargeDetailTypeCodeMeanings));

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

        public Organization GetOrganizationEntityForVisitLocationCode(String CernerCode, ref CdrDbInterface CdrDb)
        {
            switch (VisitLocationCodeMeanings[CernerCode])
            {
                case "Bay Clinic":
                    return CdrDb.EnsureOrganizationRecord(BayClinicCernerAmbulatoryBatchAggregator.WoahBayClinicOrganizationIdentity);
                default:
                    throw new Exception("Unsupported Visit location");
            }
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
                    case "unspecified":
                        return Gender.Unspecified;
                    default:
                        Trace.WriteLine("Unsupported Patient-Gender encountered: " + GenderCodeMeanings[CernerCode] + " with code " + CernerCode);
                        return Gender.Unspecified;
                }
            }

            return Gender.Unspecified;
        }

        public AddressType GetCdrAddressTypeEnum(String CernerCode)
        {
            if (AddressTypeCodeMeanings.ContainsKey(CernerCode))
            {
                switch (AddressTypeCodeMeanings[CernerCode].ToLower())
                {
                    case "unspecified":
                        return AddressType.Unspecified;
                    case "home":
                        return AddressType.Home;
                    case "business":
                        return AddressType.Business;
                    case "mailing":
                        return AddressType.Mailing;
                    case "eprescribing":
                        return AddressType.ePrescribing;
                    default:
                        SystemSounds.Beep.Play();
                        Trace.WriteLine("Unsupported Address-Type encountered: " + AddressTypeCodeMeanings[CernerCode] + " with code " + CernerCode);
                        return AddressType.Unspecified;
                }
            }

            return AddressType.Unspecified;
        }

        public PhoneType GetCdrPhoneTypeEnum(String CernerCode)
        {
            if (PhoneTypeCodeMeanings.ContainsKey(CernerCode))
            {
                switch (PhoneTypeCodeMeanings[CernerCode].ToLower())
                {
                    case "unspecified":
                        return PhoneType.Unspecified;
                    case "home":
                        return PhoneType.Home;
                    case "cell":
                        return PhoneType.Mobile;
                    case "business":
                        return PhoneType.Work;
                    case "fax business":
                        return PhoneType.Fax;
                    case "internal secure":
                        return PhoneType.Other;
                    default:
                        Trace.WriteLine("Unsupported Phone-Type encountered: " + PhoneTypeCodeMeanings[CernerCode] + " with code " + CernerCode);
                        return PhoneType.Other;
                }
            }

            return PhoneType.Unspecified;
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
                    case "unspecified":
                        return MaritalStatus.Unspecified;

                    default:
                        Trace.WriteLine("Unsupported Patient-MaritalStatus encountered: " + MaritalStatusCodeMeanings[CernerCode] + " with code " + CernerCode);
                        return MaritalStatus.Unspecified;
                }
            }

            return MaritalStatus.Unspecified;
        }

    }
}
