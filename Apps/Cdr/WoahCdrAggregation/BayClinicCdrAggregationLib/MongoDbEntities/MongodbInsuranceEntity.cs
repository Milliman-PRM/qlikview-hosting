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
    class MongodbInsuranceEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("record_identifier")]
        public String RecordIdentifier;

        [BsonElement("unique_entity_identifier")]
        public String UniqueEntityIdentifier;

        [BsonElement("entity_type")]
        public String EntityType;

        [BsonElement("entity_relationship_to_plan")]
        public String EntityRelationShipToPlan;

        [BsonElement("sequence")]
        public String Sequence;

        [BsonElement("unique_subscriber_identifier")]
        public String UniqueSubscriberIdentifier;

        [BsonElement("unique_health_plan_identifier")]
        public String UniqueHealthPlanIdentifier;

        [BsonElement("class")]
        public String Class;

        [BsonElement("type")]
        public String Type;

        [BsonElement("unique_organization_identifier")]
        public String UniqueOrganizationIdentifier;

        [BsonElement("policy_number")]
        public String PolicyNumber;

        [BsonElement("member_number")]
        public String MemberNumber;

        [BsonElement("group_name")]
        public String GroupName;

        [BsonElement("group_number")]
        public String GroupNumber;

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
