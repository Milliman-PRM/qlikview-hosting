using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CdrContext;

namespace BayClinicCernerAmbulatory
{
    [BsonIgnoreExtraElements]
    internal class MongodbPersonEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("unique_person_identifier")]
        public String UniquePersonIdentifier;

        [BsonElement("last_name")]
        public String LastName;

        [BsonElement("first_name")]
        public String FirstName;

        [BsonElement("middle_name")]
        public String MiddleName;

        [BsonElement("gender")]
        public String Gender;

        [BsonElement("birth_date_time")]
        public String BirthDateTime;

        [BsonElement("birth_date_precision")]
        public String BirthDatePrecision;

        [BsonElement("deceased")]
        public String Deceased;

        [BsonElement("deceased_date_time")]
        public String DeceasedDateTime;

        [BsonElement("deceased_date_precision")]
        public String DeceasedDatePrecision;

        [BsonElement("ethnicity")]
        public String Ethnicity;

        [BsonElement("additional_ethnicity")]
        public String AdditionalEthnicity;

        [BsonElement("marital_status")]
        public String MaritalStatus;

        [BsonElement("additional_marital_status")]
        public String AdditionalMaritalStatus;

        [BsonElement("nationality")]
        public String Nationality;

        [BsonElement("language")]
        public String Language;

        [BsonElement("additional_language")]
        public String AdditionalLanguage;

        [BsonElement("religion")]
        public String Religion;

        [BsonElement("race")]
        public String Race;

        [BsonElement("additional_race")]
        public String AdditionalRace;

        [BsonElement("active")]
        public String Active;

        [BsonElement("active_status_date_time")]
        public String ActiveStatusDateTime;

        [BsonElement("effective_begin_date_time")]
        public String EffectiveBeginDateTime;

        [BsonElement("effective_end_date_time")]
        public String EffectiveEndDateTime;

        [BsonElement("update_date_time")]
        public String UpdateDateTime;

        [BsonElement("ImportFile")]
        public String ImportFile;

        [BsonElement("ImportFileDate")]
        public String ImportFileDate;

        [BsonElement("lastaggregationrun")]
        public long LastAggregationRun;
#pragma warning restore 0649

        /// <summary>
        /// SElectively combines the attributes of this mongodb document with a supplied Patient record
        /// </summary>
        /// <param name="PatientRecord">Call with null if there is no existing Patient record, the resulting record is returned here</param>
        /// <param name="ReferencedCodes"></param>
        /// <returns>true if an existing record was modified, false if a new record was created</returns>
        internal bool MergeWithExistingPatient(ref Patient PatientRecord, CernerReferencedCodeDictionaries ReferencedCodes)
        {
            DateTime ParsedDateTime;

            if (PatientRecord == null)  // nothing to merge with
            {
                PatientRecord = new Patient();

                PatientRecord.EmrIdentifier = UniquePersonIdentifier;  // should always be the same?
                PatientRecord.NameLast = LastName;
                PatientRecord.NameFirst = FirstName;
                PatientRecord.NameMiddle = MiddleName;
                DateTime.TryParse(BirthDateTime, out ParsedDateTime);        // Will be DateTime.MinValue on parse failure
                PatientRecord.BirthDate = ParsedDateTime;
                PatientRecord.Gender = ReferencedCodes.GetCdrGenderEnum(Gender);
                DateTime.TryParse(DeceasedDateTime, out ParsedDateTime);        // Will be DateTime.MinValue on parse failure
                PatientRecord.DeathDate = ParsedDateTime;
                PatientRecord.Race = ReferencedCodes.RaceCodeMeanings[Race];  // coded
                PatientRecord.Ethnicity = ReferencedCodes.EthnicityCodeMeanings[Ethnicity];  // coded
                PatientRecord.MaritalStatus = ReferencedCodes.GetCdrMaritalStatusEnum(MaritalStatus);  // coded
                PatientRecord.LatestImportFileDate = ImportFileDate;

                return false;
            }
            else  // do merge
            {
                //ExistingPatient.EmrIdentifier = UniquePersonIdentifier;
                if (PatientRecord.NameLast.Length < LastName.Length) PatientRecord.NameLast = LastName;
                if (PatientRecord.NameFirst.Length < FirstName.Length) PatientRecord.NameFirst = FirstName;
                if (PatientRecord.NameMiddle.Length < MiddleName.Length) PatientRecord.NameMiddle = MiddleName;
                if (PatientRecord.BirthDate == DateTime.MinValue)
                {
                    DateTime.TryParse(BirthDateTime, out ParsedDateTime);        // Will be DateTime.MinValue on parse failure
                    PatientRecord.BirthDate = ParsedDateTime;
                }
                if (PatientRecord.Gender == CdrContext.Gender.Unspecified) PatientRecord.Gender = ReferencedCodes.GetCdrGenderEnum(Gender);
                if (PatientRecord.DeathDate == DateTime.MinValue)
                {
                    DateTime.TryParse(DeceasedDateTime, out ParsedDateTime);        // Will be DateTime.MinValue on parse failure
                    PatientRecord.DeathDate = ParsedDateTime;
                }
                if (PatientRecord.Race.Length < ReferencedCodes.RaceCodeMeanings[Race].Length) PatientRecord.Race = ReferencedCodes.RaceCodeMeanings[Race];  // coded
                if (PatientRecord.Ethnicity.Length < ReferencedCodes.EthnicityCodeMeanings[Ethnicity].Length) PatientRecord.Ethnicity = ReferencedCodes.EthnicityCodeMeanings[Ethnicity];  // coded
                if (PatientRecord.MaritalStatus == CdrContext.MaritalStatus.Unspecified) PatientRecord.MaritalStatus = ReferencedCodes.GetCdrMaritalStatusEnum(MaritalStatus);  // coded
                PatientRecord.LatestImportFileDate = new String[] { PatientRecord.LatestImportFileDate, ImportFileDate }.Max();
                return true;
            }
        }

    }
}
