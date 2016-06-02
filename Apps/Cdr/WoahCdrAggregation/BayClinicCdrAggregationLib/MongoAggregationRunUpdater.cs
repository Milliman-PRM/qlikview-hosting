using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BayClinicCernerAmbulatory
{
    class MongoAggregationRunUpdater
    {
        private long AggregationRunNumber;
        private IMongoDatabase Db;

        public HashSet<ObjectId> AddressIdList;     ////
        public HashSet<ObjectId> AllergyIdList;
        public HashSet<ObjectId> AllergyReactionIdList;
        public HashSet<ObjectId> AllergyReviewIdList;
        public HashSet<ObjectId> ChargeIdList;
        public HashSet<ObjectId> ChargeDetailIdList;
        public HashSet<ObjectId> DiagnosisIdList;
        public HashSet<ObjectId> FamilyHistoryIdList;
        public HashSet<ObjectId> IdentifiersIdList;    ////
        public HashSet<ObjectId> ImmunizationIdList;
        public HashSet<ObjectId> ImmunizationExpectIdList;
        public HashSet<ObjectId> ImmunizationPersonnelIdList;
        public HashSet<ObjectId> InsuranceIdList;
        public HashSet<ObjectId> ManualHealthMaintenanceIdList;
        public HashSet<ObjectId> MedicationComplianceIdList;
        public HashSet<ObjectId> MedicationDetailsIdList;
        public HashSet<ObjectId> MedicationReconciliationIdList;
        public HashSet<ObjectId> MedicationReconciliationDetailIdList;
        public HashSet<ObjectId> MedicationsIdList;
        public HashSet<ObjectId> OrderDetailsIdList;
        public HashSet<ObjectId> OrdersIdList;
        public HashSet<ObjectId> PatientInfoIdList;
        public HashSet<ObjectId> PersonIdList;    ////
        public HashSet<ObjectId> PersonEmploymentIdList;
        public HashSet<ObjectId> PersonInfoIdList;
        public HashSet<ObjectId> PhoneIdList;    ////
        public HashSet<ObjectId> ProblemIdList;
        public HashSet<ObjectId> ProcedureIdList;
        public HashSet<ObjectId> ProcedurePersonnelIdList;
        public HashSet<ObjectId> ReferenceCodeIdList;
        public HashSet<ObjectId> ReferenceHealthPlanIdList;
        public HashSet<ObjectId> ReferenceLocationIdList;
        public HashSet<ObjectId> ReferenceMedicationIdList;
        public HashSet<ObjectId> ReferenceOrganizationIdList;
        public HashSet<ObjectId> ReferencePersonnelIdList;
        public HashSet<ObjectId> ReferenceTerminologyIdList;
        public HashSet<ObjectId> RelationIdList;
        public HashSet<ObjectId> ResultIdList;
        public HashSet<ObjectId> ResultCodedIdList;
        public HashSet<ObjectId> ResultPersonnelIdList;
        public HashSet<ObjectId> ScheduleActionIdList;
        public HashSet<ObjectId> SchedulePersonAppointmentIdList;
        public HashSet<ObjectId> ScheduleResourceappointmentIdList;
        public HashSet<ObjectId> SchedulesIdList;
        public HashSet<ObjectId> SocialHistoryIdList;
        public HashSet<ObjectId> SocialHistoryResponseIdList;
        public HashSet<ObjectId> SocialHistoryResponseDetailIdList;
        public HashSet<ObjectId> VisitIdList;    ////

        public MongoAggregationRunUpdater(long RunNumber, IMongoDatabase Database)
        {
            AggregationRunNumber = RunNumber;
            Db = Database;
            Reset();
        }

        public void Reset() {
            AddressIdList = new HashSet<ObjectId>();
            AllergyIdList = new HashSet<ObjectId>();
            AllergyReactionIdList = new HashSet<ObjectId>();
            AllergyReviewIdList = new HashSet<ObjectId>();
            ChargeIdList = new HashSet<ObjectId>();
            ChargeDetailIdList = new HashSet<ObjectId>();
            DiagnosisIdList = new HashSet<ObjectId>();
            FamilyHistoryIdList = new HashSet<ObjectId>();
            IdentifiersIdList = new HashSet<ObjectId>();
            ImmunizationIdList = new HashSet<ObjectId>();
            ImmunizationExpectIdList = new HashSet<ObjectId>();
            ImmunizationPersonnelIdList = new HashSet<ObjectId>();
            InsuranceIdList = new HashSet<ObjectId>();
            ManualHealthMaintenanceIdList = new HashSet<ObjectId>();
            MedicationComplianceIdList = new HashSet<ObjectId>();
            MedicationDetailsIdList = new HashSet<ObjectId>();
            MedicationReconciliationIdList = new HashSet<ObjectId>();
            MedicationReconciliationDetailIdList = new HashSet<ObjectId>();
            MedicationsIdList = new HashSet<ObjectId>();
            OrderDetailsIdList = new HashSet<ObjectId>();
            OrdersIdList = new HashSet<ObjectId>();
            PatientInfoIdList = new HashSet<ObjectId>();
            PersonIdList = new HashSet<ObjectId>();
            PersonEmploymentIdList = new HashSet<ObjectId>();
            PersonInfoIdList = new HashSet<ObjectId>();
            PhoneIdList = new HashSet<ObjectId>();
            ProblemIdList = new HashSet<ObjectId>();
            ProcedureIdList = new HashSet<ObjectId>();
            ProcedurePersonnelIdList = new HashSet<ObjectId>();
            ReferenceCodeIdList = new HashSet<ObjectId>();
            ReferenceHealthPlanIdList = new HashSet<ObjectId>();
            ReferenceLocationIdList = new HashSet<ObjectId>();
            ReferenceMedicationIdList = new HashSet<ObjectId>();
            ReferenceOrganizationIdList = new HashSet<ObjectId>();
            ReferencePersonnelIdList = new HashSet<ObjectId>();
            ReferenceTerminologyIdList = new HashSet<ObjectId>();
            RelationIdList = new HashSet<ObjectId>();
            ResultIdList = new HashSet<ObjectId>();
            ResultCodedIdList = new HashSet<ObjectId>();
            ResultPersonnelIdList = new HashSet<ObjectId>();
            ScheduleActionIdList = new HashSet<ObjectId>();
            SchedulePersonAppointmentIdList = new HashSet<ObjectId>();
            ScheduleResourceappointmentIdList = new HashSet<ObjectId>();
            SchedulesIdList = new HashSet<ObjectId>();
            SocialHistoryIdList = new HashSet<ObjectId>();
            SocialHistoryResponseIdList = new HashSet<ObjectId>();
            SocialHistoryResponseDetailIdList = new HashSet<ObjectId>();
            VisitIdList = new HashSet<ObjectId>();
        }

        public void UpdateAll()
        {
            foreach (ObjectId DocId in PersonIdList)
            {
                FilterDefinition<MongodbPersonEntity> FilterDef = Builders<MongodbPersonEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbPersonEntity> UpdateDef = Builders<MongodbPersonEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Db.GetCollection<MongodbPersonEntity>("person").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in PhoneIdList)
            {
                FilterDefinition<MongodbPhoneEntity> FilterDef = Builders<MongodbPhoneEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbPhoneEntity> UpdateDef = Builders<MongodbPhoneEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Db.GetCollection<MongodbPhoneEntity>("phone").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in AddressIdList)
            {
                FilterDefinition<MongodbAddressEntity> FilterDef = Builders<MongodbAddressEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbAddressEntity> UpdateDef = Builders<MongodbAddressEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Db.GetCollection<MongodbAddressEntity>("address").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in IdentifiersIdList)
            {
                FilterDefinition<MongodbIdentifierEntity> FilterDef = Builders<MongodbIdentifierEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbIdentifierEntity> UpdateDef = Builders<MongodbIdentifierEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Db.GetCollection<MongodbIdentifierEntity>("identifiers").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in VisitIdList)
            {
                FilterDefinition<MongodbVisitEntity> FilterDef = Builders<MongodbVisitEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbVisitEntity> UpdateDef = Builders<MongodbVisitEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Db.GetCollection<MongodbVisitEntity>("visit").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in ChargeIdList)
            {
                FilterDefinition<MongodbChargeEntity> FilterDef = Builders<MongodbChargeEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbChargeEntity> UpdateDef = Builders<MongodbChargeEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Db.GetCollection<MongodbChargeEntity>("charge").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            Reset();
        }
    }
}
