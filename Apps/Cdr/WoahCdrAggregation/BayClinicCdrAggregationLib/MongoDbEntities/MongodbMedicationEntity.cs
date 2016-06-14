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
    class MongodbMedicationEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("unique_medication_identifier")]
        public string UniqueMedicationIdentifier;

        [BsonElement("unique_person_identifier")]
        public string UniquePersonIdentifier;

        [BsonElement("unique_visit_identifier")]
        public string UniqueVisitIdentifier;

        [BsonElement("code")]
        public string Code;

        [BsonElement("type")]
        public string Type;

        [BsonElement("unique_synonym_identifier")]
        public string UniqueSynonymIdentifier;

        [BsonElement("ordered_as")]
        public string OrderedAs;

        [BsonElement("placed_order_as")]
        public string PlacedOrderAs;

        [BsonElement("original_ordered_date_time")]
        public string OriginalOrderedDateTime;

        [BsonElement("start_date_time")]
        public string StartDateTime;

        [BsonElement("suspend_date_time")]
        public string SuspendDateTime;

        [BsonElement("resume_date_time")]
        public string ResumeDateTime;

        [BsonElement("discontinue_date_time")]
        public string DiscontinueDateTime;

        [BsonElement("discontinue_type")]
        public string DiscontinueType;

        [BsonElement("stop_date_time")]
        public string StopDateTime;

        [BsonElement("stop_type")]
        public string StopType;

        [BsonElement("status")]
        public string Status;

        [BsonElement("active")]
        public string Active;

        [BsonElement("active_status_date_time")]
        public string ActiveStatusDateTime;

        [BsonElement("update_date_time")]
        public string UpdateDateTime;

        [BsonElement("ImportFile")]
        public String ImportFile;

        [BsonElement("lastaggregationrun")]
        public long LastAggregationRun;
#pragma warning restore 0649
    }
}
