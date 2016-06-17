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
    internal class MongodbDiagnosisEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("record_identifier")]
        public String RecordIdentifier;

        [BsonElement("unique_diagnosis_identifier")]
        public String UniqueDiagnosisIdentifier;

        [BsonElement("unique_person_identifier")]
        public String UniquePersonIdentifier;

        [BsonElement("unique_visit_identifier")]
        public String UniqueVisitIdentifier;

        [BsonElement("unique_originating_terminology_identifier")]
        public String UniqueOriginatingTerminologyIdentifier;

        [BsonElement("unique_terminology_identifier")]
        public String UniqueTerminologyIdentifier;

        [BsonElement("display")]
        public String Display;

        [BsonElement("class")]
        public String Class;

        [BsonElement("type")]
        public String Type;

        [BsonElement("diagnosis_date_time")]
        public String DiagnosisDateTime;

        [BsonElement("diagnosis_by")]
        public String DiagnosisBy;

        [BsonElement("diagnosis_by_name")]
        public String DiagnosisByName;

        [BsonElement("priority")]
        public String Priority;

        [BsonElement("certainty")]
        public String Certainty;

        [BsonElement("classification")]
        public String Classification;

        [BsonElement("clinical_service")]
        public String ClinicalService;

        [BsonElement("conditional_qualifier")]
        public String ConditionalQualifier;

        [BsonElement("probability")]
        public String Probability;

        [BsonElement("ranking")]
        public String Ranking;

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

        [BsonElement("ImportFileDate")]
        public String ImportFileDate;

        [BsonElement("lastaggregationrun")]
        public long LastAggregationRun;
#pragma warning restore 0649
    }
}
