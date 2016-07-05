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
    internal class MongodbIdentifierEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("record_identifier")]
        public String RecordIdentifier;

        [BsonElement("entity_identifier")]
        public String EntityIdentifier;

        [BsonElement("entity_type")]
        public String EntityType;

        [BsonElement("identifier")]
        public String Identifier;

        [BsonElement("identifier_type")]
        public String IdentifierType;

        [BsonElement("identifier_group")]
        public String IdentifierGroup;

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
        internal bool MergeWithExistingPatientIdentifiers(ref PatientIdentifier IdentifierRecord, ref Patient PatientRecord, CernerReferencedCodeDictionaries ReferencedCodes, Organization OrganizationObject)
        {
            DateTime ActiveStatusDT, UpdateTime;
            DateTime.TryParse(ActiveStatusDateTime, out ActiveStatusDT);
            DateTime.TryParse(UpdateDateTime, out UpdateTime);
            if (IdentifierRecord == null)
            {

                IdentifierRecord = new PatientIdentifier
                {
                    Identifier = Identifier,
                    IdentifierType = ReferencedCodes.IdentifierTypeCodeMeanings[IdentifierType],
                    Organization = OrganizationObject,
                    Patient = PatientRecord,
                    DateFirstReported = ActiveStatusDT,
                    DateLastReported = ActiveStatusDT,
                    UpdateTime = UpdateTime,
                    LastImportFileDate = ImportFileDate
                };
                return true;
            }

            else if(IdentifierRecord.UpdateTime < UpdateTime)
            { 
                if (IdentifierRecord.IdentifierType != ReferencedCodes.IdentifierTypeCodeMeanings[IdentifierType] && !String.IsNullOrEmpty(IdentifierType)) IdentifierType = ReferencedCodes.IdentifierTypeCodeMeanings[IdentifierType];

                if (IdentifierRecord.DateLastReported < ActiveStatusDT) IdentifierRecord.DateLastReported = ActiveStatusDT;
                if (IdentifierRecord.DateFirstReported > ActiveStatusDT) IdentifierRecord.DateFirstReported = ActiveStatusDT;

                if (IdentifierRecord.UpdateTime != UpdateTime && !String.IsNullOrEmpty(UpdateDateTime)) IdentifierRecord.UpdateTime = UpdateTime;

                IdentifierRecord.LastImportFileDate = new string[] { IdentifierRecord.LastImportFileDate, ImportFileDate }.Max();

                return false;
            }

            else
            {
                return false;
            }
            
        }
    }
}
