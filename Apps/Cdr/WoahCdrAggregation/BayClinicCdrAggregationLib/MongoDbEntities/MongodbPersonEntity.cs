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
    public class MongodbPersonEntity
    {
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("unique_person_identifier")]
        public String UniquePersonIdentifier;

        [BsonElement("last_name")]
        public String LastName;

        [BsonElement("first_name")]
        public String FirstName;

        [BsonElement("middle_name")]
        public String MiddleName;

        [BsonElement("gender")]
        public String Gender;

        [BsonElement("birth_date_time")]
        public String BirthDateTime;

        [BsonElement("birth_date_precision")]
        public String BirthDatePrecision;

        [BsonElement("deceased")]
        public String Deceased;

        [BsonElement("deceased_date_time")]
        public String DeceasedDateTime;

        [BsonElement("deceased_date_precision")]
        public String DeceasedDatePrecision;

        [BsonElement("ethnicity")]
        public String Ethnicity;

        [BsonElement("additional_ethnicity")]
        public String AdditionalEthnicity;

        [BsonElement("marital_status")]
        public String MaritalStatus;

        [BsonElement("additional_marital_status")]
        public String AdditionalMaritalStatus;

        [BsonElement("nationality")]
        public String Nationality;

        [BsonElement("language")]
        public String Language;

        [BsonElement("additional_language")]
        public String AdditionalLanguage;

        [BsonElement("religion")]
        public String Religion;

        [BsonElement("race")]
        public String Race;

        [BsonElement("additional_race")]
        public String AdditionalRace;

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
    }
}
