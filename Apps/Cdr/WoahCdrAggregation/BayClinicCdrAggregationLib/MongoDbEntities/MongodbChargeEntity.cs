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
    class MongodbChargeEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("unique_charge_identifier")]
        public String UniqueChargeIdentifier;

        [BsonElement("parent_charge_identifier;")]
        public String ParentChargeIdentifier;

        [BsonElement("unique_charge_item_identifier")]
        public String UniqueChargeItemIdentifier;

        [BsonElement("unique_person_identifier")]
        public String UniquePersonIdentifier;

        [BsonElement("unique_visit_identifier")]
        public String UniqueVisitIdentifier;

        [BsonElement("posted_date_time")]
        public String PostedDateTime;

        [BsonElement("service_date_time")]
        public String ServiceDateTime;

        [BsonElement("description")]
        public String Description;

        [BsonElement("state")]
        public String State;

        [BsonElement("type")]
        public String Type;

        [BsonElement("offset_charge_identifier")]
        public String OffsetChargeIdentifier;

        [BsonElement("ordering_physician_identifier")]
        public String OrderingPhysicianIdentifier;

        [BsonElement("performing_physician_identifier")]
        public String PerformingPhysicianIdentifier;

        [BsonElement("referring_physician_identifier")]
        public String ReferringPhysicianIdentifier;

        [BsonElement("verifying_physician_identifier")]
        public String VerifyingPhysicianIdentifier;

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

        [BsonElement("lastaggregationrun")]
        public long LastAggregationRun;
#pragma warning restore 0649
    }
}
