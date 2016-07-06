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
    internal class MongodbDiagnosisEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("record_identifier")]
        public String RecordIdentifier;

        [BsonElement("unique_diagnosis_identifier")]
        public String UniqueDiagnosisIdentifier;

        [BsonElement("unique_person_identifier")]
        public String UniquePersonIdentifier;

        [BsonElement("unique_visit_identifier")]
        public String UniqueVisitIdentifier;

        [BsonElement("unique_originating_terminology_identifier")]
        public String UniqueOriginatingTerminologyIdentifier;

        [BsonElement("unique_terminology_identifier")]
        public String UniqueTerminologyIdentifier;

        [BsonElement("display")]
        public String Display;

        [BsonElement("class")]
        public String Class;

        [BsonElement("type")]
        public String Type;

        [BsonElement("diagnosis_date_time")]
        public String DiagnosisDateTime;

        [BsonElement("diagnosis_by")]
        public String DiagnosisBy;

        [BsonElement("diagnosis_by_name")]
        public String DiagnosisByName;

        [BsonElement("priority")]
        public String Priority;

        [BsonElement("certainty")]
        public String Certainty;

        [BsonElement("classification")]
        public String Classification;

        [BsonElement("clinical_service")]
        public String ClinicalService;

        [BsonElement("conditional_qualifier")]
        public String ConditionalQualifier;

        [BsonElement("probability")]
        public String Probability;

        [BsonElement("ranking")]
        public String Ranking;

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
        internal bool MergeWithExistingDiagnoses(ref Diagnosis DiagnosisRecord, ref Patient PatientRecord, VisitEncounter VisitRecord, CernerReferencedCodeDictionaries ReferencedCodes, MongodbReferenceTerminologyEntity TerminologyRecord)
        {
            DateTime StartDateTime, EndDateTime, DiagDateTime, StatusDateTime;
            DateTime.TryParse(EffectiveBeginDateTime, out StartDateTime);
            DateTime.TryParse(EffectiveEndDateTime, out EndDateTime);
            DateTime.TryParse(DiagnosisDateTime, out DiagDateTime);
            DateTime.TryParse(ActiveStatusDateTime, out StatusDateTime);

            if (DiagnosisRecord == null)
            {
                Diagnosis NewPgRecord = new Diagnosis
                {
                    Patientdbid = PatientRecord.dbid,
                    VisitEncounterdbid = VisitRecord.dbid,
                    EmrIdentifier = UniqueDiagnosisIdentifier,
                    StartDateTime = StartDateTime,
                    EndDateTime = EndDateTime,
                    DeterminationDateTime = DiagDateTime,
                    ShortDescription = Display,
                    LongDescription = "",  // TODO Can I do better?  Maybe this field doesn't need to be here if there is no source of long description.  
                    DiagCode = new CodedEntry
                    {
                        Code = TerminologyRecord.Code,
                        CodeMeaning = TerminologyRecord.Text,
                        CodeSystem = ReferencedCodes.TerminologyCodeMeanings[TerminologyRecord.Terminology]
                        // TODO Handle variability in codes (e.g. snomed codes are not correct in the "code" field, but are correct in concept. May require custom interpreter/handler
                    },
                    Status = "",  // TODO If this is just active and inactive maybe I don't need it.  Study.  
                    StatusDateTime = StatusDateTime,         //Same as the update time
                    LastImportFileDate = ImportFileDate
                    // TODO There is a coded "type" field with values Discharge and Billing.  Figure out whether this should be used/interpreted
                };
                return true;
            }
            
            else if(DiagnosisRecord.StatusDateTime < StatusDateTime)
            {
                //Extra logic here to have as many real dates as possible instead of max dates
                if (DiagnosisRecord.EndDateTime.ToString().Contains("2100"))
                {
                    if (!UpdateDateTime.Contains("2100"))
                        DiagnosisRecord.EndDateTime = EndDateTime;
                }
                if (!DiagnosisRecord.EndDateTime.ToString().Contains("2100"))
                {
                    if (DiagnosisRecord.EndDateTime < EndDateTime)
                        DiagnosisRecord.EndDateTime = EndDateTime;
                }

                if (DiagnosisRecord.DeterminationDateTime != DiagDateTime && !DiagnosisDateTime.Contains("2100") && !String.IsNullOrEmpty(DiagnosisDateTime))
                    DiagnosisRecord.DeterminationDateTime = DiagDateTime;
                if (DiagnosisRecord.ShortDescription != Display && !String.IsNullOrEmpty(Display)) DiagnosisRecord.ShortDescription += "; " + Display;
                if (DiagnosisRecord.DiagCode.Code != TerminologyRecord.Code && !String.IsNullOrEmpty(TerminologyRecord.Code)) DiagnosisRecord.DiagCode.Code = TerminologyRecord.Code;
                if (DiagnosisRecord.DiagCode.CodeMeaning != TerminologyRecord.Text && !String.IsNullOrEmpty(TerminologyRecord.Text)) DiagnosisRecord.DiagCode.CodeMeaning = TerminologyRecord.Text;
                if (DiagnosisRecord.DiagCode.CodeSystem != ReferencedCodes.TerminologyCodeMeanings[TerminologyRecord.Terminology] && !String.IsNullOrEmpty(ReferencedCodes.TerminologyCodeMeanings[TerminologyRecord.Terminology]))
                    DiagnosisRecord.DiagCode.CodeSystem = ReferencedCodes.TerminologyCodeMeanings[TerminologyRecord.Terminology];

                if (DiagnosisRecord.StatusDateTime != StatusDateTime && !String.IsNullOrEmpty(UpdateDateTime)) DiagnosisRecord.StatusDateTime = StatusDateTime;

                DiagnosisRecord.LastImportFileDate = new string[] { DiagnosisRecord.LastImportFileDate, ImportFileDate }.Max();

                return false;
            }

            else
            {
                return false;
            }
            
        }
    }
}
