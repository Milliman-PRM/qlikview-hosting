using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
    }
}
