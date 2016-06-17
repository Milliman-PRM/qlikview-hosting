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
    internal class MongodbVisitEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("unique_visit_identifier")]
        public String UniqueVisitIdentifier;

        [BsonElement("unique_person_identifier")]
        public String UniquePersonIdentifier;

        [BsonElement("unique_organization_identifier")]
        public String UniqueOrganizationIdentifier;

        [BsonElement("place_of_service_type")]
        public String PlaceOfServiceType;

        [BsonElement("place_of_service")]
        public String PlaceOfService;

        [BsonElement("facility_code")]
        public String FacilityCode;

        [BsonElement("building_code")]
        public String BuildingCode;

        [BsonElement("unit_code")]
        public String UnitCode;

        [BsonElement("location_code")]
        public String LocationCode;

        [BsonElement("registration_date_time")]
        public String RegistrationDateTime;

        [BsonElement("discharge_date_time")]
        public String DischargeDateTime;

        [BsonElement("service")]
        public String Service;
        
        [BsonElement("financial_class")]
        public String FinancialClass;
        
        [BsonElement("class_type")]
        public String ClassType;
        
        [BsonElement("class")]
        public String Class;
        
        [BsonElement("type")]
        public String Type;
        
        [BsonElement("reason_for_visit")]
        public String ReasonForVisit;
        
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

