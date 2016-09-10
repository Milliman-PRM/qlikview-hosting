using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CdrContext;
using CdrDbLib;

namespace BayClinicCernerAmbulatory
{
    [BsonIgnoreExtraElements]
    internal class MongodbResultEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("record_identifier")]
        public String RecordIdentifier;

        [BsonElement("unique_result_identifier")]
        public String UniqueResultIdentifier;

        [BsonElement("parent_result_identifier")]
        public String ParentResultIdentifier;

        [BsonElement("unique_person_identifier")]
        public String UniquePersonIdentifier;

        [BsonElement("unique_visit_identifier")]
        public String UniqueVisitIdentifier;

        [BsonElement("code")]
        public String Code;

        [BsonElement("start_date_time")]
        public String StartDateTime;

        [BsonElement("end_date_time")]
        public String EndDateTime;

        [BsonElement("tag")]
        public String Tag;

        [BsonElement("title")]
        public String Title;

        [BsonElement("normal_code")]
        public String NormalCode;

        [BsonElement("normal_high")]
        public String NormalHigh;

        [BsonElement("normal_low")]
        public String NormalLow;

        [BsonElement("critial_high")]
        public String CriticalHigh;

        [BsonElement("critial_low")]
        public String CriticalLow;

        [BsonElement("performed_date_time")]
        public String PerformedDateTime;

        [BsonElement("performed_by")]
        public String PerformedBy;

        [BsonElement("verified_date_time")]
        public String VerifiedDateTime;

        [BsonElement("verified_by")]
        public String VerifiedBy;

        [BsonElement("status")]
        public String Status;

        [BsonElement("units")]
        public String Units;

        [BsonElement("result_value")]
        public String ResultValue;

        [BsonElement("administration_dosage")]
        public String AdministrationDosage;

        [BsonElement("dosage_units")]
        public String DosageUnits;

        [BsonElement("administration_method")]
        public String AdministrationMethod;

        [BsonElement("administration_by")]
        public String AdministrationBy;

        [BsonElement("administration_start_date_time")]
        public String AdministrationStartDateTime;

        [BsonElement("administration_end_date_time")]
        public String AdministrationEndDateTime;

        [BsonElement("route")]
        public String Route;

        [BsonElement("site")]
        public String Site;

        [BsonElement("strength")]
        public String Strength;

        [BsonElement("strength_units")]
        public String StrengthUnits;

        [BsonElement("expire_date_time")]
        public String ExpireDateTime;

        [BsonElement("lot_number")]
        public String LotNumber;

        [BsonElement("manufacturer")]
        public String Manufacturer;

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
        /// Selectively combines the attributes of this mongodb document with a supplied Measurment record
        /// </summary>
        /// <param name="MeasurementRecord">Call with null if there is no existing Measurment record, the resulting record is returned here</param>
        /// <param name="PatientRecord"></param>
        /// <param name="VisitRecord"></param>
        /// <param name="ReferencedCodes"></param>
        /// <returns>true if an existing record was modified, false if a new record was created</returns>
        internal bool MergeWithExistingMeasurement(ref Measurement MeasurementRecord, ref Patient PatientRecord, VisitEncounter VisitRecord, CernerReferencedCodeDictionaries ReferencedCodes)
        {
            DateTime ParsedDateTime;
            DateTime PerformedDate, UpdateTime;
            DateTime.TryParse(PerformedDateTime, out PerformedDate);
            DateTime.TryParse(UpdateDateTime, out UpdateTime);
            if (MeasurementRecord == null)
            {
                MeasurementRecord = new Measurement
                {
                    Patientdbid = PatientRecord.dbid,
                    VisitEncounterdbid = VisitRecord.dbid,
                    EmrIdentifier = UniqueResultIdentifier,
                    Name = ReferencedCodes.ResultCodeCodeMeanings[Code],
                    Description = Title,  // TODO get this right
                    Comments = "",    // TODO get this right
                    MeasurementCode = new CodedEntry { },    // TODO get this right
                    AssessmentDateTime = PerformedDate,
                    Value = ResultValue,
                    Units = ReferencedCodes.ResultUnitsCodeMeanings[Units],
                    NormalRangeLow = NormalLow,
                    NormalRangeHigh = NormalHigh,
                    NormalType = ReferencedCodes.GetResultNormalCodeEnum(NormalCode),
                    UpdateTime = UpdateTime,
                    LastImportFileDate = ImportFileDate,

                    // TODO The following are exploratory attributes.  This should be revised for long term production.  
                    BC_StartDateTime = DateTime.TryParse(StartDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null,
                    BC_EndDateTime = DateTime.TryParse(EndDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null,
                    BC_PerformedDateTime = DateTime.TryParse(PerformedDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null,
                    BC_VerifiedDateTime = DateTime.TryParse(VerifiedDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null,
                    BC_AdministrationStartDateTime = DateTime.TryParse(AdministrationStartDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null,
                    BC_AdministrationEndDateTime = DateTime.TryParse(AdministrationEndDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null,
                    BC_ExpireDateTime = DateTime.TryParse(ExpireDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null,
                    BC_EffectiveBeginDateTime = DateTime.TryParse(EffectiveBeginDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null,
                    BC_EffectiveEndDateTime = DateTime.TryParse(EffectiveEndDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null,
                    BC_UpdateDateTime = DateTime.TryParse(UpdateDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null,
                };
                return false;
            }
            else if(MeasurementRecord.UpdateTime < UpdateTime)
            {
                if (MeasurementRecord.Name != ReferencedCodes.ResultCodeCodeMeanings[Code] && !String.IsNullOrEmpty(ReferencedCodes.ResultCodeCodeMeanings[Code]))
                    MeasurementRecord.Name = ReferencedCodes.ResultCodeCodeMeanings[Code];

                if (MeasurementRecord.Description != Title && !String.IsNullOrEmpty(Title)) MeasurementRecord.Description = Title;
                if (MeasurementRecord.AssessmentDateTime != PerformedDate && !String.IsNullOrEmpty(PerformedDateTime)) MeasurementRecord.AssessmentDateTime = PerformedDate;
                if (MeasurementRecord.Value != ResultValue && !String.IsNullOrEmpty(ResultValue)) MeasurementRecord.Value = ResultValue;

                if (MeasurementRecord.Units != ReferencedCodes.ResultUnitsCodeMeanings[Units] && !String.IsNullOrEmpty(ReferencedCodes.ResultUnitsCodeMeanings[Units]))
                    MeasurementRecord.Units = ReferencedCodes.ResultUnitsCodeMeanings[Units];

                if (MeasurementRecord.NormalRangeLow != NormalLow && !String.IsNullOrEmpty(NormalLow)) MeasurementRecord.NormalRangeLow = NormalLow;
                if (MeasurementRecord.NormalRangeHigh != NormalHigh && !String.IsNullOrEmpty(NormalHigh)) MeasurementRecord.NormalRangeHigh = NormalHigh;

                if (MeasurementRecord.NormalType != ReferencedCodes.GetResultNormalCodeEnum(NormalCode) && !String.IsNullOrEmpty(NormalCode))
                    MeasurementRecord.NormalType = ReferencedCodes.GetResultNormalCodeEnum(NormalCode);

                if (MeasurementRecord.UpdateTime != UpdateTime && !String.IsNullOrEmpty(UpdateDateTime)) MeasurementRecord.UpdateTime = UpdateTime;

                MeasurementRecord.LastImportFileDate = new string[] { MeasurementRecord.LastImportFileDate, ImportFileDate }.Max();

                // TODO The following are exploratory attributes.  This should be revised for long term production.  
                MeasurementRecord.BC_StartDateTime = DateTime.TryParse(StartDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null;
                MeasurementRecord.BC_EndDateTime = DateTime.TryParse(EndDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null;
                MeasurementRecord.BC_PerformedDateTime = DateTime.TryParse(PerformedDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null;
                MeasurementRecord.BC_VerifiedDateTime = DateTime.TryParse(VerifiedDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null;
                MeasurementRecord.BC_AdministrationStartDateTime = DateTime.TryParse(AdministrationStartDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null;
                MeasurementRecord.BC_AdministrationEndDateTime = DateTime.TryParse(AdministrationEndDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null;
                MeasurementRecord.BC_ExpireDateTime = DateTime.TryParse(ExpireDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null;
                MeasurementRecord.BC_EffectiveBeginDateTime = DateTime.TryParse(EffectiveBeginDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null;
                MeasurementRecord.BC_EffectiveEndDateTime = DateTime.TryParse(EffectiveEndDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null;
                MeasurementRecord.BC_UpdateDateTime = DateTime.TryParse(UpdateDateTime, out ParsedDateTime) ? ParsedDateTime : (DateTime?)null;
            }

            return true;
        }

    }
}
