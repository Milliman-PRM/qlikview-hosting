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
    class MongodbReferenceHealthPlanEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("unique_health_plan_identifier")]
        public String UniqueHealthPlanIdentifier;

        [BsonElement("plan_name")]
        public String PlanName;

        [BsonElement("financial_class")]
        public String FinancialClass;

        [BsonElement("type")]
        public String Type;

        [BsonElement("class")]
        public String Class;

        [BsonElement("group_name")]
        public String GroupName;

        [BsonElement("group_number")]
        public String GroupNumber;

        [BsonElement("policy_number")]
        public String PolicyNumber;

        [BsonElement("active")]
        public String Active;

        [BsonElement("active_status_date_time")]
        public String ActiveStatusDateTime;

        [BsonElement("effecive_begin_date_time")]
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
