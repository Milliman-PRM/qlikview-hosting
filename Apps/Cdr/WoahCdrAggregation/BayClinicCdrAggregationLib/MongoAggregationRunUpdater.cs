using System;
using System.Diagnostics;
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

        public HashSet<ObjectId> AddressIdList;
        public HashSet<ObjectId> AllergyIdList;
        public HashSet<ObjectId> AllergyReactionIdList;
        public HashSet<ObjectId> AllergyReviewIdList;
        public HashSet<ObjectId> ChargeIdList;
        public HashSet<ObjectId> ChargeDetailIdList;
        public HashSet<ObjectId> DiagnosisIdList;
        public HashSet<ObjectId> FamilyHistoryIdList;
        public HashSet<ObjectId> IdentifiersIdList;
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
        public HashSet<ObjectId> PersonIdList;
        public HashSet<ObjectId> PersonEmploymentIdList;
        public HashSet<ObjectId> PersonInfoIdList;
        public HashSet<ObjectId> PhoneIdList;
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
        public HashSet<ObjectId> VisitIdList;

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
            UpdateResult Result;

            DateTime UpdateStart = DateTime.Now;

            foreach (ObjectId DocId in PersonIdList)
            {
                FilterDefinition<MongodbPersonEntity> FilterDef = Builders<MongodbPersonEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbPersonEntity> UpdateDef = Builders<MongodbPersonEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbPersonEntity>("person").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in PhoneIdList)
            {
                FilterDefinition<MongodbPhoneEntity> FilterDef = Builders<MongodbPhoneEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbPhoneEntity> UpdateDef = Builders<MongodbPhoneEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbPhoneEntity>("phone").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in AddressIdList)
            {
                FilterDefinition<MongodbAddressEntity> FilterDef = Builders<MongodbAddressEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbAddressEntity> UpdateDef = Builders<MongodbAddressEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbAddressEntity>("address").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in IdentifiersIdList)
            {
                FilterDefinition<MongodbIdentifierEntity> FilterDef = Builders<MongodbIdentifierEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbIdentifierEntity> UpdateDef = Builders<MongodbIdentifierEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbIdentifierEntity>("identifiers").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in VisitIdList)
            {
                FilterDefinition<MongodbVisitEntity> FilterDef = Builders<MongodbVisitEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbVisitEntity> UpdateDef = Builders<MongodbVisitEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbVisitEntity>("visit").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in ChargeIdList)
            {
                FilterDefinition<MongodbChargeEntity> FilterDef = Builders<MongodbChargeEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbChargeEntity> UpdateDef = Builders<MongodbChargeEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbChargeEntity>("charge").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in ChargeDetailIdList)
            {
                FilterDefinition<MongodbChargeDetailEntity> FilterDef = Builders<MongodbChargeDetailEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbChargeDetailEntity> UpdateDef = Builders<MongodbChargeDetailEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbChargeDetailEntity>("chargedetail").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in ResultIdList)
            {
                FilterDefinition<MongodbResultEntity> FilterDef = Builders<MongodbResultEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbResultEntity> UpdateDef = Builders<MongodbResultEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbResultEntity>("result").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in DiagnosisIdList)
            {
                FilterDefinition<MongodbDiagnosisEntity> FilterDef = Builders<MongodbDiagnosisEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbDiagnosisEntity> UpdateDef = Builders<MongodbDiagnosisEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbDiagnosisEntity>("diagnosis").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in InsuranceIdList)
            {
                FilterDefinition<MongodbInsuranceEntity> FilterDef = Builders<MongodbInsuranceEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbInsuranceEntity> UpdateDef = Builders<MongodbInsuranceEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbInsuranceEntity>("insurance").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in ImmunizationIdList)
            {
                FilterDefinition<MongodbImmunizationEntity> FilterDef = Builders<MongodbImmunizationEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbImmunizationEntity> UpdateDef = Builders<MongodbImmunizationEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbImmunizationEntity>("immunization").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in MedicationsIdList)
            {
                FilterDefinition<MongodbMedicationEntity> FilterDef = Builders<MongodbMedicationEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbMedicationEntity> UpdateDef = Builders<MongodbMedicationEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbMedicationEntity>("medications").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in ProblemIdList)
            {
                FilterDefinition<MongodbProblemEntity> FilterDef = Builders<MongodbProblemEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbProblemEntity> UpdateDef = Builders<MongodbProblemEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbProblemEntity>("problem").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            foreach (ObjectId DocId in ProcedureIdList)
            {
                FilterDefinition<MongodbProcedureEntity> FilterDef = Builders<MongodbProcedureEntity>.Filter.Where(x => x.Id == DocId);
                UpdateDefinition<MongodbProcedureEntity> UpdateDef = Builders<MongodbProcedureEntity>.Update.Set(p => p.LastAggregationRun, AggregationRunNumber);
                Result = Db.GetCollection<MongodbProcedureEntity>("procedure").UpdateOne(FilterDef, UpdateDef);  // record to MongoDB that this document has been aggregated
            }

            Reset();

            // TODO remove// Trace.WriteLine(DateTime.Now + ":     MongoDB updater took time: " + (DateTime.Now - UpdateStart));
        }

        /// <summary>
        /// Clears all aggregation run numbers from documents in MongoDB in order to permit a full database aggregation
        /// </summary>
        public void ClearAllMongoAggregationRunNumbers()
        {
            UpdateResult Result;

            FilterDefinition<MongodbPersonEntity> PersonFilterDef = Builders<MongodbPersonEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbPersonEntity> PersonUpdateDef = Builders<MongodbPersonEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Db.GetCollection<MongodbPersonEntity>("person").UpdateMany(PersonFilterDef, PersonUpdateDef);

            FilterDefinition <MongodbPhoneEntity> PhoneFilterDef = Builders<MongodbPhoneEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbPhoneEntity> PhoneUpdateDef = Builders<MongodbPhoneEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Db.GetCollection<MongodbPhoneEntity>("phone").UpdateMany(PhoneFilterDef, PhoneUpdateDef);

            FilterDefinition<MongodbAddressEntity> AddressFilterDef = Builders<MongodbAddressEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbAddressEntity> AddressUpdateDef = Builders<MongodbAddressEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Result = Db.GetCollection<MongodbAddressEntity>("address").UpdateMany(AddressFilterDef, AddressUpdateDef);

            FilterDefinition<MongodbIdentifierEntity> IdentifierFilterDef = Builders<MongodbIdentifierEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbIdentifierEntity> IdentifierUpdateDef = Builders<MongodbIdentifierEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Db.GetCollection<MongodbIdentifierEntity>("identifiers").UpdateMany(IdentifierFilterDef, IdentifierUpdateDef);

            FilterDefinition<MongodbVisitEntity> VisitFilterDef = Builders<MongodbVisitEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbVisitEntity> VisitUpdateDef = Builders<MongodbVisitEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Db.GetCollection<MongodbVisitEntity>("visit").UpdateMany(VisitFilterDef, VisitUpdateDef);

            FilterDefinition<MongodbChargeEntity> ChargeFilterDef = Builders<MongodbChargeEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbChargeEntity> ChargeUpdateDef = Builders<MongodbChargeEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Db.GetCollection<MongodbChargeEntity>("charge").UpdateMany(ChargeFilterDef, ChargeUpdateDef);

            FilterDefinition<MongodbChargeDetailEntity> ChargeDetailFilterDef = Builders<MongodbChargeDetailEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbChargeDetailEntity> ChargeDetailUpdateDef = Builders<MongodbChargeDetailEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Db.GetCollection<MongodbChargeDetailEntity>("chargedetail").UpdateMany(ChargeDetailFilterDef, ChargeDetailUpdateDef);

            FilterDefinition<MongodbResultEntity> ResultFilterDef = Builders<MongodbResultEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbResultEntity> ResultUpdateDef = Builders<MongodbResultEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Db.GetCollection<MongodbResultEntity>("result").UpdateMany(ResultFilterDef, ResultUpdateDef);

            FilterDefinition<MongodbDiagnosisEntity> DiagnosisFilterDef = Builders<MongodbDiagnosisEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbDiagnosisEntity> DiagnosisUpdateDef = Builders<MongodbDiagnosisEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Db.GetCollection<MongodbDiagnosisEntity>("diagnosis").UpdateMany(DiagnosisFilterDef, DiagnosisUpdateDef);

            FilterDefinition<MongodbInsuranceEntity> InsuranceFilterDef = Builders<MongodbInsuranceEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbInsuranceEntity> InsuranceUpdateDef = Builders<MongodbInsuranceEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Result = Db.GetCollection<MongodbInsuranceEntity>("insurance").UpdateMany(InsuranceFilterDef, InsuranceUpdateDef);

            FilterDefinition<MongodbImmunizationEntity> ImmunizationFilterDef = Builders<MongodbImmunizationEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbImmunizationEntity> ImmunizationUpdateDef = Builders<MongodbImmunizationEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Db.GetCollection<MongodbImmunizationEntity>("immunization").UpdateMany(ImmunizationFilterDef, ImmunizationUpdateDef);

            FilterDefinition<MongodbMedicationEntity> MedicationFilterDef = Builders<MongodbMedicationEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbMedicationEntity> MedicationUpdateDef = Builders<MongodbMedicationEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Db.GetCollection<MongodbMedicationEntity>("medications").UpdateMany(MedicationFilterDef, MedicationUpdateDef);

            FilterDefinition<MongodbProblemEntity> ProblemFilterDef = Builders<MongodbProblemEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbProblemEntity> ProblemUpdateDef = Builders<MongodbProblemEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Db.GetCollection<MongodbProblemEntity>("problem").UpdateMany(ProblemFilterDef, ProblemUpdateDef);

            FilterDefinition<MongodbProcedureEntity> ProcedureFilterDef = Builders<MongodbProcedureEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbProcedureEntity> ProcedureUpdateDef = Builders<MongodbProcedureEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = Db.GetCollection<MongodbProcedureEntity>("procedure").UpdateMany(ProcedureFilterDef, ProcedureUpdateDef);

        }

    }
}
