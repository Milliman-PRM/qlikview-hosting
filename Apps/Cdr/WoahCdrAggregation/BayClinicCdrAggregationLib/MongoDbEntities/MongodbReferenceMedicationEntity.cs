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
    class MongodbReferenceMedicationEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("unique_synonym_identifier")]
        public String UniqueSynonymIdentifier;

        [BsonElement("ndc")]
        public String NDC;

        [BsonElement("dnum")]
        public String Dnum;

        [BsonElement("catalog_cki")]
        public String CatalogCKI;

        [BsonElement("rxnorm")]
        public String RxNorm;

        [BsonElement("ImportFile")]
        public String ImportFile;

        [BsonElement("lastaggregationrun")]
        public long LastAggregationRun;
#pragma warning restore 0649
    }
}
