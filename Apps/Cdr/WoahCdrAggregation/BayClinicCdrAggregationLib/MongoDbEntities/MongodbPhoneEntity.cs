using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CdrDbLib;
using CdrContext;

namespace BayClinicCernerAmbulatory
{
    [BsonIgnoreExtraElements]
    internal class MongodbPhoneEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("record_identifier")]
        public String RecordIdentifier;

        [BsonElement("type")]
        public String Type;

        [BsonElement("sequence")]
        public String Sequence;

        [BsonElement("entity_identifier")]
        public String EntityIdentifier;

        [BsonElement("entity_type")]
        public String EntityType;

        [BsonElement("contact_method")]
        public String ContactMethod;

        [BsonElement("extension")]
        public String Extension;

        [BsonElement("phone_number")]
        public String PhoneNumber;

        [BsonElement("description")]
        public String Description;

        [BsonElement("contact")]
        public String Contact;

        [BsonElement("instruction")]
        public String Instruction;

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
        /// Selectively combines the attributes of this mongodb document with a supplied Phone Record record
        /// </summary>
        /// <param name="PhoneRecord">Call with null if there is no existing Phone Record record, the resulting record is returned here</param>
        /// <param name="PatientRecord"></param>
        /// <param name="ReferencedCodes"></param>
        /// <returns>true if an existing record was modified, false if a new record was created</returns>
        internal bool MergeWithExistingTelephoneNumber(ref TelephoneNumber PhoneRecord, ref Patient PatientRecord, CernerReferencedCodeDictionaries ReferencedCodes)
        {
            DateTime ActiveStatusDT, UpdateTime;
            DateTime.TryParse(ActiveStatusDateTime, out ActiveStatusDT);        // Will be DateTime.MinValue on parse failure
            DateTime.TryParse(UpdateDateTime, out UpdateTime);        // Will be DateTime.MinValue on parse failure
            if (PhoneRecord == null)
            {
                PhoneRecord = new TelephoneNumber
                {
                    EmrIdentifier = RecordIdentifier,
                    Number = new PhoneNumber
                    {
                        Number = PhoneNumber,
                        PhoneType = ReferencedCodes.GetCdrPhoneTypeEnum(Type)
                    },
                    Patient = PatientRecord,
                    UpdateTime = UpdateTime,
                    DateFirstReported = ActiveStatusDT,
                    DateLastReported = ActiveStatusDT,
                    LastImportFileDate = ImportFileDate
                };
                return false;
            }
            else if(PhoneRecord.UpdateTime < UpdateTime)
            {
                if (PhoneRecord.Number.Number != PhoneNumber && !String.IsNullOrEmpty(PhoneNumber)) PhoneRecord.Number.Number = PhoneNumber;
                if (PhoneRecord.Number.PhoneType != ReferencedCodes.GetCdrPhoneTypeEnum(Type) && !String.IsNullOrEmpty(ReferencedCodes.GetCdrPhoneTypeEnum(Type).ToString()))
                {
                    PhoneRecord.Number.PhoneType = ReferencedCodes.GetCdrPhoneTypeEnum(Type);
                }

                if (PhoneRecord.DateLastReported < ActiveStatusDT) PhoneRecord.DateLastReported = ActiveStatusDT;
                if (PhoneRecord.DateFirstReported > ActiveStatusDT) PhoneRecord.DateFirstReported = ActiveStatusDT;

                if (PhoneRecord.UpdateTime != UpdateTime && !String.IsNullOrEmpty(UpdateDateTime)) PhoneRecord.UpdateTime = UpdateTime;

                PhoneRecord.LastImportFileDate = new string[] { PhoneRecord.LastImportFileDate, ImportFileDate }.Max();
            }

            return true;
        }
    }
}
