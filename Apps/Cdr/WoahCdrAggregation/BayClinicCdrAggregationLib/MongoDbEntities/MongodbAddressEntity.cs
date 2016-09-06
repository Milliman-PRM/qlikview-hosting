using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CdrContext;
using CdrDbLib;

namespace BayClinicCernerAmbulatory
{
    [BsonIgnoreExtraElements]
    internal class MongodbAddressEntity
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

        [BsonElement("ImportFileDate")]
        public String ImportFileDate;

        [BsonElement("lastaggregationrun")]
        public long LastAggregationRun;
#pragma warning restore 0649
        
        /// <summary>
        /// Selectively combines the attributes of this mongodb document with a supplied Address record
        /// </summary>
        /// <param name="AddressRecord">Call with null if there is no existing Address record, the resulting record is returned here</param>
        /// <param name="PatientRecord"></param>
        /// <param name="ReferencedCodes"></param>
        /// <returns>true if an existing record was modified, false if a new record was created</returns>
        internal bool MergeWithExistingPhysicalAddress(ref PhysicalAddress AddressRecord, ref Patient PatientRecord, CernerReferencedCodeDictionaries ReferencedCodes)
        {
            DateTime ActiveStatusDT, UpdateTime;
            DateTime.TryParse(ActiveStatusDateTime, out ActiveStatusDT);
            DateTime.TryParse(UpdateDateTime, out UpdateTime);
            if (AddressRecord == null)
            {
                AddressRecord = new PhysicalAddress
                {
                    EmrIdentifier = RecordIdentifier,
                    Address = new Address
                    {
                        City = CityText,
                        State = StateText,
                        PostalCode = ZipCode,
                        Country = CountryText,
                        Line1 = AddressLine1,
                        Line2 = AddressLine2,
                        Line3 = AddressLine3,
                        Line4 = AddressLine4
                    },
                    AddressType = ReferencedCodes.GetCdrAddressTypeEnum(Type),
                    patient = PatientRecord,
                    DateFirstReported = ActiveStatusDT,
                    DateLastReported = ActiveStatusDT,
                    UpdateTime = UpdateTime,
                    LastImportFileDate = ImportFileDate
                };
                return false;
            }
            else if(AddressRecord.UpdateTime < UpdateTime) 
            {
                //Updating the address object
                if (AddressRecord.Address.City != CityText && !String.IsNullOrEmpty(CityText)) AddressRecord.Address.City = CityText;
                if (AddressRecord.Address.State != StateText && !String.IsNullOrEmpty(StateText)) AddressRecord.Address.State = StateText;
                if (AddressRecord.Address.PostalCode != ZipCode && !String.IsNullOrEmpty(ZipCode)) AddressRecord.Address.PostalCode = ZipCode;
                if (AddressRecord.Address.Country != CountryText && !String.IsNullOrEmpty(CountryText)) AddressRecord.Address.Country = CountryText;
                if (AddressRecord.Address.Line1 != AddressLine1 && !String.IsNullOrEmpty(AddressLine1)) AddressRecord.Address.Line1 = AddressLine1;
                if (AddressRecord.Address.Line2 != AddressLine2 && !String.IsNullOrEmpty(AddressLine2)) AddressRecord.Address.Line2 = AddressLine2;
                if (AddressRecord.Address.Line3 != AddressLine3 && !String.IsNullOrEmpty(AddressLine3)) AddressRecord.Address.Line3 = AddressLine3;
                if (AddressRecord.Address.Line4 != AddressLine4 && !String.IsNullOrEmpty(AddressLine4)) AddressRecord.Address.Line4 = AddressLine4;

                //Updating the rest
                if (AddressRecord.AddressType != ReferencedCodes.GetCdrAddressTypeEnum(Type) && !String.IsNullOrEmpty(Type)) AddressRecord.AddressType = ReferencedCodes.GetCdrAddressTypeEnum(Type);

                if (AddressRecord.DateLastReported < ActiveStatusDT) AddressRecord.DateLastReported = ActiveStatusDT;
                if (AddressRecord.DateFirstReported > ActiveStatusDT) AddressRecord.DateFirstReported = ActiveStatusDT;

                if (AddressRecord.UpdateTime != UpdateTime && !String.IsNullOrEmpty(UpdateDateTime)) AddressRecord.UpdateTime = UpdateTime;

                AddressRecord.LastImportFileDate = new string[] { AddressRecord.LastImportFileDate, ImportFileDate }.Max();
            }

            return true;
        }
    }
}
