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
    internal class MongodbImmunizationEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("record_identifier")]
        public String RecordIdentifier;

        [BsonElement("unique_result_identifier")]
        public String UniqueResultIdentifier;

        [BsonElement("unique_person_identifier")]
        public String UniquePersonIdentifier;

        [BsonElement("unique_visit_identifier")]
        public String UniqueVisitIdentifier;

        [BsonElement("unique_order_identifier")]
        public String UniqueOrderIdentifier;

        [BsonElement("code")]
        public String Code;

        [BsonElement("cvx")]
        public String Cvx;

        [BsonElement("start_date_time")]
        public String StartDateTime;

        [BsonElement("end_date_time")]
        public String EndDateTime;

        [BsonElement("status")]
        public String Status;

        [BsonElement("performed_by")]
        public String PerformedBy;

        [BsonElement("performed_date_time")]
        public String PerformedDateTime;

        [BsonElement("modifier_type")]
        public String ModifierType;

        [BsonElement("modifier_reason")]
        public String ModifierReason;

        [BsonElement("modifier_reason_meaning")]
        public String ModifierReasonMeaning;

        [BsonElement("dose_sequence")]
        public String DoseSequence;

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

        [BsonElement("type")]
        public String Type;

        [BsonElement("expire_date_time")]
        public String ExpireDateTime;

        [BsonElement("lot_number")]
        public String LotNumber;

        [BsonElement("manufacturer")]
        public String Manufacturer;

        [BsonElement("product_name")]
        public String ProductName;

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

        internal bool MergeWithExistingImmunizations(ref Immunization ImmunizationRecord, ref Patient PatientRecord, VisitEncounter VisitRecord, CernerReferencedCodeDictionaries ReferencedCodes)
        {
            DateTime PerformedDate, UpdateTime;
            DateTime.TryParse(PerformedDateTime, out PerformedDate);
            DateTime.TryParse(UpdateDateTime, out UpdateTime);
            if (ImmunizationRecord == null)
            {
                Immunization NewPgRecord = new Immunization
                {
                    Patientdbid = PatientRecord.dbid,
                    EmrIdentifier = UniqueOrderIdentifier,  // TODO This is probably not the right value to assign
                    VisitID = UniqueVisitIdentifier,
                    ResultID = UniqueResultIdentifier,
                    Description = "",
                    PerformedDateTime = PerformedDate,
                    ImmunizationCode = new CodedEntry
                    {
                        Code = Code,
                        CodeMeaning = ReferencedCodes.ImmunizationCodeMeanings[Code],
                    },
                    VisitEncounterdbid = VisitRecord.dbid,
                    UpdateTime = UpdateTime,
                    LastImportFileDate = ImportFileDate
                };
                return true;
            }
            else if(ImmunizationRecord.UpdateTime < UpdateTime)
            {
                if (ImmunizationRecord.PerformedDateTime != PerformedDate && !String.IsNullOrEmpty(PerformedDateTime)) ImmunizationRecord.PerformedDateTime = PerformedDate;

                if (ImmunizationRecord.ImmunizationCode.Code != Code && !String.IsNullOrEmpty(Code)) ImmunizationRecord.ImmunizationCode.Code = Code;
                if(ImmunizationRecord.ImmunizationCode.CodeMeaning != ReferencedCodes.ImmunizationCodeMeanings[Code] && !String.IsNullOrEmpty(Code))
                    ImmunizationRecord.ImmunizationCode.CodeMeaning = ReferencedCodes.ImmunizationCodeMeanings[Code];

                if (ImmunizationRecord.UpdateTime != UpdateTime && !String.IsNullOrEmpty(UpdateDateTime)) ImmunizationRecord.UpdateTime = UpdateTime;

                ImmunizationRecord.LastImportFileDate = new string[] { ImmunizationRecord.LastImportFileDate, ImportFileDate }.Max();

                return false;
            }
            else
            {
                return false;
            }



        }
    }
}
