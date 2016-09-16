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
    internal class CernerReferencedCodeDictionaries
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
        public Dictionary<String, String> ResultCodeCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> ResultNormalCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> ResultUnitsCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> TerminologyCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> ImmunizationCodeMeanings = new Dictionary<string, string>();
        public Dictionary<String, String> InsuranceTypeCodeMeanings = new Dictionary<string, string>();
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

            //ValidateRefCodeFieldList("PERSON", 12);

            bool Success =
                   InitializeReferenceCodeDictionary("PERSON", new String[] {"GENDER"},                  ref GenderCodeMeanings)
                && InitializeReferenceCodeDictionary("PERSON", new String[] {"DECEASED"},                ref DeceasedCodeMeanings)
                && InitializeReferenceCodeDictionary("PERSON", new String[] {"BIRTH_DATE_PRECISION"},    ref BirthDatePrecisionCodeMeanings)
                && InitializeReferenceCodeDictionary("PERSON", new String[] {"DECEASED_DATE_PRECISION"}, ref DeceasedDatePrecisionCodeMeanings)
                && InitializeReferenceCodeDictionary("PERSON", new String[] {"MARITAL_STATUS", "ADDITIONAL_MARITAL_STATUS"}, ref MaritalStatusCodeMeanings)
                && InitializeReferenceCodeDictionary("PERSON", new String[] {"RACE",           "ADDITIONAL_RACE"          }, ref RaceCodeMeanings)
                && InitializeReferenceCodeDictionary("PERSON", new String[] {"LANGUAGE",       "ADDITIONAL_LANGUAGE"      }, ref LanguageCodeMeanings)
                && InitializeReferenceCodeDictionary("PERSON", new String[] {"ETHNICITY",      "ADDITIONAL_ETHNICITY"     }, ref EthnicityCodeMeanings)
                // Likely these won't be present in the ambulatory extract?
                //&& InitializeReferenceCodeDictionary("PERSON", "HOME_ADDRESS", ref CodeMeanings)
                //&& InitializeReferenceCodeDictionary("PERSON", "IDENTIFIERS", ref CodeMeanings)  // ???
                //&& InitializeReferenceCodeDictionary("PERSON", "STATUS", ref CodeMeanings)  // ???

                && InitializeReferenceCodeDictionary("PHONE",   new String[] {"TYPE"}, ref PhoneTypeCodeMeanings)

                && InitializeReferenceCodeDictionary("ADDRESS", new String[] {"TYPE"}, ref AddressTypeCodeMeanings)

                && InitializeReferenceCodeDictionary("IDENTIFIERS", new String[] { "IDENTIFIER_TYPE" }, ref IdentifierTypeCodeMeanings)
                && InitializeReferenceCodeDictionary("IDENTIFIERS", new String[] { "IDENTIFIER_GROUP" }, ref IdentifierGroupCodeMeanings)

                && InitializeReferenceCodeDictionary("VISIT", new String[] { "LOCATION_CODE" }, ref VisitLocationCodeMeanings)

                && InitializeReferenceCodeDictionary("CHARGEDETAIL", new String[] { "TYPE" }, ref ChargeDetailTypeCodeMeanings)

                && InitializeReferenceCodeDictionary("RESULT", new String[] { "CODE" }, ref ResultCodeCodeMeanings)
                && InitializeReferenceCodeDictionary("RESULT", new String[] { "NORMAL_CODE" }, ref ResultNormalCodeMeanings)
                && InitializeReferenceCodeDictionary("RESULT", new String[] { "UNITS" }, ref ResultUnitsCodeMeanings)

                && InitializeReferenceCodeDictionary("REFERENCETERMINOLOGY", new String[] { "TERMINOLOGY" }, ref TerminologyCodeMeanings)

                && InitializeReferenceCodeDictionary("IMMUNIZATION", new String[] { "CODE" }, ref ImmunizationCodeMeanings)

                && InitializeReferenceCodeDictionary("INSURANCE", new String[] { "TYPE" }, ref InsuranceTypeCodeMeanings)

                ;
            
            //Trace.WriteLine("Identifier Typecodes dictionary has values: " + String.Join(", ", IdentifierTypeCodeMeanings));
            //Trace.WriteLine("Identifier Groupcodes dictionary has values: " + String.Join(", ", IdentifierGroupCodeMeanings));
            //Trace.WriteLine("ChargeDetail Type dictionary has values: " + String.Join(", ", ChargeDetailTypeCodeMeanings));

            return Success;
        }

        /// <summary>
        /// Initializes a dictionary of codes and corresponding display values from the MongoDB referencecode collection, eliminating redundancy
        /// </summary>
        /// <param name="FileName">The value of the 'file_name' field to use for grouping</param>
        /// <param name="Fields">The values of 0 or more case sensitive 'field' values to search</param>
        /// <param name="Dict">The dictionary<string,string> to be populated with query results</param>
        /// <returns></returns>
        private bool InitializeReferenceCodeDictionary(String FileName, String[] Fields, ref Dictionary<String, String> Dict, bool AddZeroUnspecified = true)
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
                Trace.WriteLine("Exception in CernerReferencedCodeDictionaries.InitializeCodeDictionary: " + e.Message);
                return false;
            }

            return true;
        }

        public long GetOrganizationDbidForVisitLocationCode(String CernerCode, ref CdrDbInterface CdrDb)
        {
            switch (VisitLocationCodeMeanings[CernerCode])
            {
                case "Bay Clinic":
                    return CdrDb.EnsureOrganizationRecord(BayClinicCernerAmbulatoryBatchAggregator.WoahBayClinicOrganizationIdentity).dbid;
                case "Bay Clinic Pediactrics":
                    return CdrDb.EnsureOrganizationRecord(VisitLocationCodeMeanings[CernerCode]).dbid;
                default:
                    Trace.WriteLine("WARNING: From CernerReferencedCodeDictionaries.GetOrganizationDbidForVisitLocationCode(), adding to database a non-default visit location >" + VisitLocationCodeMeanings[CernerCode] + "<");
                    return CdrDb.EnsureOrganizationRecord(VisitLocationCodeMeanings[CernerCode]).dbid;
                    //throw new Exception("Unsupported Visit location encountered in GetOrganizationDbidForVisitLocationCode()" + VisitLocationCodeMeanings[CernerCode] + " with code " + CernerCode);
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
                    case "unknown":
                        return Gender.Unknown;
                    default:
                        String ErrMsg = "Unsupported Patient-Gender encountered in GetOrganizationDbidForVisitLocationCode(): " + GenderCodeMeanings[CernerCode] + " with code " + CernerCode;
                        Trace.WriteLine(ErrMsg);
                        throw new Exception(ErrMsg);
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
                    case "email":
                        return AddressType.EMail;
                    case "prescription (printer output)":
                        return AddressType.PrescriptionWritten;
                    default:
                        String ErrMsg = "Unsupported Address-Type encountered: " + AddressTypeCodeMeanings[CernerCode] + " with code " + CernerCode;
                        Trace.WriteLine(ErrMsg);
                        throw new Exception(ErrMsg);
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
                    case "external secure":
                        return PhoneType.Other;
                    default:
                        String ErrMsg = "Unsupported Phone-Type encountered: " + PhoneTypeCodeMeanings[CernerCode] + " with code " + CernerCode;
                        Trace.WriteLine(ErrMsg);
                        throw new Exception(ErrMsg);
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
                    case "married (living together)":
                        return MaritalStatus.Married;
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
                        String ErrMsg = "Unsupported Patient-MaritalStatus encountered: " + MaritalStatusCodeMeanings[CernerCode] + " with code " + CernerCode;
                        Trace.WriteLine(ErrMsg);
                        throw new Exception(ErrMsg);
                }
            }

            return MaritalStatus.Unspecified;
        }

        public ResultNormal GetResultNormalCodeEnum(String CernerCode)
        {
            if (ResultNormalCodeMeanings.ContainsKey(CernerCode))
            {
                switch (ResultNormalCodeMeanings[CernerCode].ToLower())
                {
                    case "hhi":
                        return ResultNormal.High;
                    case "hi":
                        return ResultNormal.High;
                    case "llow":
                        return ResultNormal.Low;
                    case "low":
                        return ResultNormal.Low;
                    case "abn":
                        return ResultNormal.Abnormal;
                    case "unspecified":     // TODO This may indicate normal, have not tested this exhaustively
                        return ResultNormal.Unspecified;
                    case "":                // TODO This may indicate unspecified or normal.  Need to switch RefCodes to use description field instead of display
                        if (CernerCode == "214")
                        {
                            return ResultNormal.Normal;
                        }
                        return ResultNormal.Unspecified;

                    default:
                        String ErrMsg = "Unsupported Result-Normal code encountered: " + ResultNormalCodeMeanings[CernerCode] + " with code " + CernerCode;
                        Trace.WriteLine(ErrMsg);
                        throw new Exception(ErrMsg);
                }
            }

            return ResultNormal.Unspecified;
        }
    }
}
