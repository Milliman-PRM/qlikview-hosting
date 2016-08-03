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
    internal class MongodbInsuranceEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("record_identifier")]
        public String RecordIdentifier;

        [BsonElement("unique_entity_identifier")]
        public String UniqueEntityIdentifier;

        [BsonElement("entity_type")]
        public String EntityType;

        [BsonElement("entity_relationship_to_plan")]
        public String EntityRelationShipToPlan;

        [BsonElement("sequence")]
        public String Sequence;

        [BsonElement("unique_subscriber_identifier")]
        public String UniqueSubscriberIdentifier;

        [BsonElement("unique_health_plan_identifier")]
        public String UniqueHealthPlanIdentifier;

        [BsonElement("class")]
        public String Class;

        [BsonElement("type")]
        public String Type;

        [BsonElement("unique_organization_identifier")]
        public String UniqueOrganizationIdentifier;

        [BsonElement("policy_number")]
        public String PolicyNumber;

        [BsonElement("member_number")]
        public String MemberNumber;

        [BsonElement("group_name")]
        public String GroupName;

        [BsonElement("group_number")]
        public String GroupNumber;

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
        /// Selectively combines the attributes of this mongodb document with a supplied Insurance Coverage record
        /// </summary>
        /// <param name="InsuranceCoverageRecord">Call with null if there is no existing Insurance Coverage record, the resulting record is returned here</param>
        /// <param name="PgPatient"></param>
        /// <param name="ReferencedCodes"></param>
        /// <param name="ReferencedHealthPlans"></param>
        /// <returns>true if an existing record was modified, false if a new record was created</returns>
        internal bool MergeWithExistingInsuranceCoverage(ref InsuranceCoverage InsuranceCoverageRecord, ref Patient PgPatient, CernerReferencedCodeDictionaries ReferencedCodes, CernerReferenceHealthPlanDictionaries ReferencedHealthPlans)
        {
            string CernerMaxDateTime = "2100-12-31 00:00:00";
            DateTime StartDate, EndDate, UpdateTime;
            DateTime.TryParse(UpdateDateTime, out UpdateTime);
            DateTime.TryParse(EffectiveBeginDateTime, out StartDate);        // Will be DateTime.MinValue on parse failure
            DateTime.TryParse(EffectiveEndDateTime, out EndDate);        // Will be DateTime.MinValue on parse failure

            if(InsuranceCoverageRecord == null)
            {
                InsuranceCoverageRecord = new InsuranceCoverage
                {
                    EmrIdentifier = RecordIdentifier,
                    CoverageType = ReferencedCodes.InsuranceTypeCodeMeanings[Type],
                    MemberID = MemberNumber,
                    Payer = UniqueOrganizationIdentifier,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    PlanName = ReferencedHealthPlans.PlanNameCodeMeanings[UniqueHealthPlanIdentifier],
                    Patient = PgPatient,            //Just adding the patientdbid might may improve runtime
                    UpdateTime = UpdateTime,
                    LastImportFileDate = ImportFileDate
                };
                return false;
            }

            else if (InsuranceCoverageRecord.UpdateTime < UpdateTime)
            {
                if (InsuranceCoverageRecord.Payer != UniqueOrganizationIdentifier && !String.IsNullOrEmpty(UniqueOrganizationIdentifier)) InsuranceCoverageRecord.Payer = UniqueOrganizationIdentifier;
                //Extra logic here to have as many real dates as possible instead of max dates
                if (InsuranceCoverageRecord.EndDate.ToString() == CernerMaxDateTime)
                {
                    if (UpdateDateTime != CernerMaxDateTime)
                        InsuranceCoverageRecord.EndDate = EndDate;
                }
                if(InsuranceCoverageRecord.EndDate.ToString() != CernerMaxDateTime)
                {
                    if(InsuranceCoverageRecord.EndDate < EndDate)
                        InsuranceCoverageRecord.EndDate = EndDate;
                }
                if (InsuranceCoverageRecord.StartDate > StartDate) InsuranceCoverageRecord.StartDate = StartDate;

                if (InsuranceCoverageRecord.PlanName != UniqueHealthPlanIdentifier && !String.IsNullOrEmpty(UniqueHealthPlanIdentifier)) InsuranceCoverageRecord.PlanName = UniqueHealthPlanIdentifier;

                if (InsuranceCoverageRecord.UpdateTime != UpdateTime && !String.IsNullOrEmpty(UpdateDateTime)) InsuranceCoverageRecord.UpdateTime = UpdateTime;

                InsuranceCoverageRecord.LastImportFileDate = new string[] { InsuranceCoverageRecord.LastImportFileDate, ImportFileDate }.Max();
            }

            return true;
        }
    }
}
