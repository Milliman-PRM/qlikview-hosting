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
    class MongodbPhoneEntity
    {
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

        [BsonElement("contact_method")]
        public String ContactMethod;

        [BsonElement("extension")]
        public String Extension;

        [BsonElement("phone_number")]
        public String PhoneNumber;

        [BsonElement("description")]
        public String Description;

        [BsonElement("contact")]
        public String Contact;

        [BsonElement("instruction")]
        public String Instruction;

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

        [BsonElement("Importfile")]
        public String Importfile;

    }
}
