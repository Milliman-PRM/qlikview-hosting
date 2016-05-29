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
    class MongodbAddressEntity
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

        [BsonElement("address_line_1")]
        public String AddressLine1;

        [BsonElement("address_line_2")]
        public String AddressLine2;

        [BsonElement("address_line_3")]
        public String AddressLine3;

        [BsonElement("address_line_4")]
        public String AddressLine4;

        [BsonElement("city_text")]
        public String CityText;

        [BsonElement("city_code")]
        public String CityCode;

        [BsonElement("state_text")]
        public String StateText;

        [BsonElement("state_code")]
        public String StateCode;

        [BsonElement("zipcode")]
        public String ZipCode;

        [BsonElement("zipcode_key")]
        public String ZipCodeKey;

        [BsonElement("country_text")]
        public String CountryText;

        [BsonElement("country_code")]
        public String CountryCode;

        [BsonElement("county_text")]
        public String CountyText;

        [BsonElement("county_code")]
        public String CountyCode;

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
