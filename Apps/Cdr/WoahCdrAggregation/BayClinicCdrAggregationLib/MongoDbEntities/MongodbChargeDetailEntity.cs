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
    internal class MongodbChargeDetailEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("record_identifier")]
        public String RecordIdentifier;

        [BsonElement("unique_charge_item_identifier")]
        public String UniqueChargeItemIdentifier;

        [BsonElement("type")]
        public String Type;

        [BsonElement("sequence")]
        public String Sequence;

        [BsonElement("code")]
        public String Code;

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
        internal bool MergeWithExistingChargeCodes(ref ChargeCode ChargeDetailRecord, ref Charge ChargeRecord, CernerReferencedCodeDictionaries ReferencedCodes)
        {
            DateTime UpdateTime;
            DateTime.TryParse(UpdateDateTime, out UpdateTime);
            if (ChargeDetailRecord != null)
            {
                ChargeCode NewPgRecord = new ChargeCode
                {
                    EmrIdentifier = UniqueChargeItemIdentifier,
                    Charge = ChargeRecord,
                    Code = new CodedEntry
                    {
                        Code = Code,
                        CodeSystem = ReferencedCodes.ChargeDetailTypeCodeMeanings[Type],
                    },
                    UpdateTime = UpdateTime,
                    LastImportFileDate = ImportFileDate
                };
                return true;
            }
            
            else if(ChargeDetailRecord.UpdateTime < UpdateTime)
            {
                if (ChargeDetailRecord.Code.Code != Code && !String.IsNullOrEmpty(Code)) ChargeDetailRecord.Code.Code = Code;
                if (ChargeDetailRecord.Code.CodeSystem != ReferencedCodes.ChargeDetailTypeCodeMeanings[Type] && !String.IsNullOrEmpty(Type))
                    ChargeDetailRecord.Code.CodeSystem = ReferencedCodes.ChargeDetailTypeCodeMeanings[Type];


                if (ChargeDetailRecord.UpdateTime != UpdateTime && !String.IsNullOrEmpty(UpdateDateTime)) ChargeDetailRecord.UpdateTime = UpdateTime;

                ChargeDetailRecord.LastImportFileDate = new string[] { ChargeDetailRecord.LastImportFileDate, ImportFileDate }.Max();

                return false;
            }

            else {
                return false;
            }
            
        }
    }
}
