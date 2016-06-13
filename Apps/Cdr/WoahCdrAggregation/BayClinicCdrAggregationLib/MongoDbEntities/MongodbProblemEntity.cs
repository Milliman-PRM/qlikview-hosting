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
    class MongodbProblemEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("record_identifier")]
        public String RecordIdentifier;

        [BsonElement("unique_problem_identifier")]
        public String UniqueProblemIdentifier;

        [BsonElement("unique_person_identifier")]
        public String UniquePersonIdentifier;

        [BsonElement("unique_originating_terminology_identifier")]
        public String UniqueOriginatingTerminologyIdentifier;

        [BsonElement("unique_terminology_identifier")]
        public String UniqueTerminologyIdentifier;

        [BsonElement("display")]
        public String Display;

        [BsonElement("estimated_resolution_date_time")]
        public String EstimatedResolutionDateTime;

        [BsonElement("actual_resolution_date_time")]
        public String ActualResolutionDateTime;

        [BsonElement("type")]
        public String Type;

        [BsonElement("onset_date_time")]
        public String OnsetDateTime;

        [BsonElement("onset_date_time_precision")]
        public String OnsetDateTimePrecision;

        [BsonElement("onset_date_time_precision_code")]
        public String OnsetDateTimePrecisionCode;

        [BsonElement("status")]
        public String Status;

        [BsonElement("status_date_time")]
        public String StatusDateTime;

        [BsonElement("status_date_time_precision")]
        public String StatusDateTimePrecision;

        [BsonElement("status_date_time_precision_code")]
        public String StatusDateTimePrecisionCode;

        [BsonElement("cancel_reason")]
        public String CancelReason;

        [BsonElement("certainty")]
        public String Certainty;

        [BsonElement("classification")]
        public String Classification;

        [BsonElement("additional_qualifier")]
        public String AdditionalQualifier;

        [BsonElement("probability")]
        public String Probability;

        [BsonElement("ranking")]
        public String Ranking;

        [BsonElement("sensitivity")]
        public String Sensitivity;

        [BsonElement("prognosis")]
        public String Prognosis;

        [BsonElement("persistence")]
        public String Persistence;

        [BsonElement("severity")]
        public String Severity;

        [BsonElement("severity_class")]
        public String SeverityClass;

        [BsonElement("severity_description_freetext")]
        public String SeverityDescriptionFreetext;

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
