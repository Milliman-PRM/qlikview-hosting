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
    internal class MongodbProblemEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("record_identifier")]
        public String RecordIdentifier;

        [BsonElement("unique_problem_identifier")]
        public String UniqueProblemIdentifier;

        [BsonElement("unique_person_identifier")]
        public String UniquePersonIdentifier;

        [BsonElement("unique_originating_terminology_identifier")]
        public String UniqueOriginatingTerminologyIdentifier;

        [BsonElement("unique_terminology_identifier")]
        public String UniqueTerminologyIdentifier;

        [BsonElement("display")]
        public String Display;

        [BsonElement("estimated_resolution_date_time")]
        public String EstimatedResolutionDateTime;

        [BsonElement("actual_resolution_date_time")]
        public String ActualResolutionDateTime;

        [BsonElement("type")]
        public String Type;

        [BsonElement("onset_date_time")]
        public String OnsetDateTime;

        [BsonElement("onset_date_time_precision")]
        public String OnsetDateTimePrecision;

        [BsonElement("onset_date_time_precision_code")]
        public String OnsetDateTimePrecisionCode;

        [BsonElement("status")]
        public String Status;

        [BsonElement("status_date_time")]
        public String StatusDateTime;

        [BsonElement("status_date_time_precision")]
        public String StatusDateTimePrecision;

        [BsonElement("status_date_time_precision_code")]
        public String StatusDateTimePrecisionCode;

        [BsonElement("cancel_reason")]
        public String CancelReason;

        [BsonElement("certainty")]
        public String Certainty;

        [BsonElement("classification")]
        public String Classification;

        [BsonElement("additional_qualifier")]
        public String AdditionalQualifier;

        [BsonElement("probability")]
        public String Probability;

        [BsonElement("ranking")]
        public String Ranking;

        [BsonElement("sensitivity")]
        public String Sensitivity;

        [BsonElement("prognosis")]
        public String Prognosis;

        [BsonElement("persistence")]
        public String Persistence;

        [BsonElement("severity")]
        public String Severity;

        [BsonElement("severity_class")]
        public String SeverityClass;

        [BsonElement("severity_description_freetext")]
        public String SeverityDescriptionFreetext;

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

        internal bool MergeWithExistingProblems(ref Problem ProblemRecord, ref Patient PatientRecord)
        {
            DateTime BeginDateTime, EndDateTime, ActiveStatusDateTime, UpdateTime;
            DateTime.TryParse(UpdateDateTime, out UpdateTime);
            DateTime.TryParse(EffectiveBeginDateTime, out BeginDateTime);
            DateTime.TryParse(EffectiveEndDateTime, out EndDateTime);
            DateTime.TryParse(EffectiveEndDateTime, out ActiveStatusDateTime);
            if (ProblemRecord == null)
            {
                Problem NewPgRecord = new Problem
                {
                    Patientdbid = PatientRecord.dbid,
                    EmrIdentifier = UniqueProblemIdentifier,
                    Description = Display,  // TODO Think about adding a Problem field for terminology code reference
                    BeginDateTime = BeginDateTime,
                    EndDateTime = EndDateTime,
                    UpdateTime = UpdateTime,
                    LastImportFileDate = ImportFileDate,
                    EffectiveDateTime = ActiveStatusDateTime,
                };

                // Following logic will be relevant for Allscripts, not Cerner
                //if (VisitRecord != null)  // not relevant for Cerner but probably relevant for Allscripts
                //{
                //    NewPgRecord.VisitEncounterdbid = VisitRecord.dbid;
                //}
                

                return true;
            }
            else if (ProblemRecord.UpdateTime < UpdateTime)
            {
                if (ProblemRecord.Description != Display && !String.IsNullOrEmpty(Display)) ProblemRecord.Description += "; " + Display;
                //Extra logic here to have as many real dates as possible instead of max dates
                if (ProblemRecord.EndDateTime.ToString().Contains("2100"))
                {
                    if (!UpdateDateTime.Contains("2100"))
                        ProblemRecord.EndDateTime = EndDateTime;
                }
                if (!ProblemRecord.EndDateTime.ToString().Contains("2100"))
                {
                    if (ProblemRecord.EndDateTime < EndDateTime)
                        ProblemRecord.EndDateTime = EndDateTime;
                }
                if (ProblemRecord.BeginDateTime > BeginDateTime) ProblemRecord.BeginDateTime = BeginDateTime;

                if (ProblemRecord.UpdateTime != UpdateTime && !String.IsNullOrEmpty(UpdateDateTime)) ProblemRecord.UpdateTime = UpdateTime;

                ProblemRecord.LastImportFileDate = new string[] { ProblemRecord.LastImportFileDate, ImportFileDate }.Max();

                return false;
            }
            else
            {
                return false;
            }
            
        }
    }
}
