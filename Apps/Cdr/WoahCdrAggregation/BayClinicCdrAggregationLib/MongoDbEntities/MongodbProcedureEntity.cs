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
    class MongodbProcedureEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("unique_procedure_identifier")]
        public String UniqueProcedureIdentifier;

        [BsonElement("unique_visit_identifier")]
        public String UniqueVisitIdentifier;

        [BsonElement("unique_terminology_identifier")]
        public String UniqueTerminologyIdentifier;

        [BsonElement("note")]
        public String Note;

        [BsonElement("procedure_date_time")]
        public String ProcedureDateTime;

        [BsonElement("procedure_date_time_precision")]
        public String ProcedureDateTimePrecision;

        [BsonElement("procedure_date_time_precision_code")]
        public String ProcedureDateTimePrecisionCode;

        [BsonElement("priority")]
        public String Priority;

        [BsonElement("type")]
        public String Type;

        [BsonElement("ranking")]
        public String Ranking;

        [BsonElement("clinical_service")]
        public String ClinicalService;

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
        /// Selectively combines the attributes of this mongodb document with a supplied Procedure record
        /// </summary>
        /// <param name="ProcedureRecord">Call with null if there is no existing Procedure record, the resulting record is returned here</param>
        /// <param name="PatientRecord"></param>
        /// <param name="VisitRecord"></param>
        /// <param name="RefTerminology"></param>
        /// <returns>true if an existing record was modified, false if a new record was created</returns>
        internal bool MergeWithExistingProcedure(ref Procedure ProcedureRecord, ref Patient PatientRecord, VisitEncounter VisitRecord, CernerReferencedTerminologyDictionaries RefTerminology)
        {
            String FullCode, CodeSystem;
            DateTime ActiveStatusDT, UpdateTime, ProcedureDT;
            DateTime.TryParse(ActiveStatusDateTime, out ActiveStatusDT);
            DateTime.TryParse(UpdateDateTime, out UpdateTime);
            DateTime.TryParse(ProcedureDateTime, out ProcedureDT);

            //Code parsing
            FullCode = RefTerminology.TerminologyConceptMeaning[UniqueTerminologyIdentifier];
            if(FullCode.Contains("{") && FullCode.Contains("}"))
            {
                CodeSystem = RefTerminology.TerminologyConceptMeaning[UniqueTerminologyIdentifier].Split('}')[0].Replace("{", "");
            }
            else
            {
                CodeSystem = "";
            }

            //Populating operations
            if (ProcedureRecord == null)
            {
                ProcedureRecord = new Procedure
                {
                    EmrIdentifier = UniqueProcedureIdentifier,
                    
                    TerminologyCode = new CodedEntry
                    {
                        Code = FullCode,
                        CodeSystem = CodeSystem,
                        CodeMeaning = RefTerminology.TerminologyCodeMeaning[UniqueTerminologyIdentifier]
                    },
                    Note = Note,
                    ProcedureDateTime = ProcedureDT,
                    UpdateTime = UpdateTime,
                    StatusDateTime = ActiveStatusDT,
                    LastImportFileDate = ImportFileDate
                };
                return false;
            }

            //Merging operations
            else if(ProcedureRecord.UpdateTime < UpdateTime)
            {
                if (ProcedureRecord.TerminologyCode.Code == "Unspecified" && UniqueTerminologyIdentifier != "0")
                {
                    ProcedureRecord.TerminologyCode.Code = RefTerminology.TerminologyConceptMeaning[UniqueTerminologyIdentifier];
                    ProcedureRecord.TerminologyCode.CodeSystem = CodeSystem;
                    ProcedureRecord.TerminologyCode.CodeMeaning = RefTerminology.TerminologyCodeMeaning[UniqueTerminologyIdentifier];
                }
                if (ProcedureRecord.Note != Note && !String.IsNullOrEmpty(Note)) { ProcedureRecord.Note = Note; }
                if (ProcedureRecord.ProcedureDateTime != ProcedureDT && !String.IsNullOrEmpty(ProcedureDateTime)) { ProcedureRecord.ProcedureDateTime = ProcedureDT; }
                if (ProcedureRecord.StatusDateTime != ActiveStatusDT && !String.IsNullOrEmpty(ActiveStatusDateTime)) { ProcedureRecord.StatusDateTime = ActiveStatusDT; }

                if (ProcedureRecord.UpdateTime != UpdateTime && !String.IsNullOrEmpty(UpdateDateTime)) ProcedureRecord.UpdateTime = UpdateTime;
                ProcedureRecord.LastImportFileDate = new string[] { ProcedureRecord.LastImportFileDate, ImportFileDate }.Max();
            }
                return true;
        }
    }
}
