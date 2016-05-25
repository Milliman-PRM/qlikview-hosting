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
    class MongodbRefCodeEntity
    {
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("file_name")]
        public String FileName;

        [BsonElement("field")]
        public String Field;

        [BsonElement("component")]
        public String Component;

        [BsonElement("element_code")]
        public String ElementCode;

        [BsonElement("display")]
        public String Display;

        [BsonElement("description")]
        public String Description;

        [BsonElement("type")]
        public String Type;

        [BsonElement("ImportFile")]
        public String ImportFile;
    }
}
