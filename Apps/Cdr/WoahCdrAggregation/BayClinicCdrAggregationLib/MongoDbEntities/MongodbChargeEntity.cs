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
    internal class MongodbChargeEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("unique_charge_identifier")]
        public String UniqueChargeIdentifier;

        [BsonElement("parent_charge_identifier;")]
        public String ParentChargeIdentifier;

        [BsonElement("unique_charge_item_identifier")]
        public String UniqueChargeItemIdentifier;

        [BsonElement("unique_person_identifier")]
        public String UniquePersonIdentifier;

        [BsonElement("unique_visit_identifier")]
        public String UniqueVisitIdentifier;

        [BsonElement("posted_date_time")]
        public String PostedDateTime;

        [BsonElement("service_date_time")]
        public String ServiceDateTime;

        [BsonElement("description")]
        public String Description;

        [BsonElement("state")]
        public String State;

        [BsonElement("type")]
        public String Type;

        [BsonElement("offset_charge_identifier")]
        public String OffsetChargeIdentifier;

        [BsonElement("ordering_physician_identifier")]
        public String OrderingPhysicianIdentifier;

        [BsonElement("performing_physician_identifier")]
        public String PerformingPhysicianIdentifier;

        [BsonElement("referring_physician_identifier")]
        public String ReferringPhysicianIdentifier;

        [BsonElement("verifying_physician_identifier")]
        public String VerifyingPhysicianIdentifier;

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
        internal bool MergeWithExistingCharges(ref Charge ChargeRecord, ref VisitEncounter VisitRecord)
        {
            DateTime ActiveStatusDT, ServiceDT, PostDT, UpdateDT;
            DateTime.TryParse(ActiveStatusDateTime, out ActiveStatusDT);
            DateTime.TryParse(ServiceDateTime, out ServiceDT);
            DateTime.TryParse(PostedDateTime, out PostDT);
            DateTime.TryParse(UpdateDateTime, out UpdateDT);
            if (ChargeRecord == null)
            {
                String DescriptionFirstWord = Description.Split(' ').FirstOrDefault();

                // TODO may need to think about value of Type, some could be "no charge"
                // TODO may need to think about value of State, some could be "combined away" or "suspended"
                Charge NewPgRecord = new Charge
                {
                    //DentalDetails = new DentalDetail {ToothNumber=0 ,ToothSurfaceCode="" },
                    EmrIdentifier = UniqueChargeIdentifier,
                    DateOfService = ServiceDT,
                    Description = Description,  // If the ChargeDetail codes are not adequate, a cpt appears to be prepended to this field in raw data
                    Comment = "",
                    SubmittedDate = PostDT,
                    Submitter = "",   // TODO What to do with this?
                    State = State,  // TODO This is a coded reference, get the meaning string
                    DateInfoLastUpdated = UpdateDT,
                    VisitEncounter = VisitRecord,
                    LastImportFileDate = ImportFileDate
                    // Think about whether the ordering_physician_identifier or verifying_physician_identifier would be useful to add to the model
                    // TODO Should we collect the type field?
                };
                NewPgRecord.ChargeCodes.Add(new ChargeCode
                {
                    Code = new CodedEntry
                    {
                        Code = DescriptionFirstWord,
                        CodeSystem = "Charge Description Prepend",
                    }
                });
                // UniqueChargeItemIdentifier is the reference from related ChargeDetail documents
                // What is parent_charge_identifier?
                // What is offset_charge_identifier?
                return true;
            }
            else if(ChargeRecord.DateInfoLastUpdated < UpdateDT) 
            {
                if (ChargeRecord.DateOfService != ServiceDT && !String.IsNullOrEmpty(ServiceDateTime)) ChargeRecord.DateOfService = ServiceDT;
                if (!ChargeRecord.Description.Contains(Description) && !String.IsNullOrEmpty(Description)) ChargeRecord.Description += "; " + Description;
                if (ChargeRecord.SubmittedDate != PostDT && !String.IsNullOrEmpty(ServiceDateTime)) ChargeRecord.SubmittedDate = PostDT;
                if (ChargeRecord.State != State && !String.IsNullOrEmpty(State)) ChargeRecord.State = State;

                //TODO Should I add a check for the visit record too? 
                if (ChargeRecord.DateInfoLastUpdated != UpdateDT && !String.IsNullOrEmpty(UpdateDateTime)) ChargeRecord.DateInfoLastUpdated = UpdateDT;

                ChargeRecord.LastImportFileDate = new string[] { ChargeRecord.LastImportFileDate, ImportFileDate }.Max();

                return false;
            }

            else {
                return false;
            }
            
        }
    }
}
