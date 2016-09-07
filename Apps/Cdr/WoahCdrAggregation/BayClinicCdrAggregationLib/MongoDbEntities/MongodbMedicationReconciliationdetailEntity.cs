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
    internal class MongodbMedicationReconciliationDetailEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("record_identifier")]
        public String RecordIdentifier;

        [BsonElement("unique_medication_reconciliation_identifier")]
        public String UniqueMedicationReconciliationIdentifier;

        [BsonElement("unique_medication_identifier")]
        public String UniqueMedicationIdentifier;

        [BsonElement("clinical_display")]
        public String ClinicalDisplay;

        [BsonElement("simplified_display")]
        public String SimplifiedDisplay;

        [BsonElement("mnemonic")]
        public String Mnemonic;

        [BsonElement("action")]
        public String Action;

        [BsonElement("note")]
        public String Note;

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
