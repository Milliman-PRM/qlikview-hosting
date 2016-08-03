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
    internal class MongodbMedicationEntity
    {
#pragma warning disable 0649
        [BsonElement("_id")]
        public ObjectId Id;

        [BsonElement("unique_medication_identifier")]
        public string UniqueMedicationIdentifier;

        [BsonElement("unique_person_identifier")]
        public string UniquePersonIdentifier;

        [BsonElement("unique_visit_identifier")]
        public string UniqueVisitIdentifier;

        [BsonElement("code")]
        public string Code;

        [BsonElement("type")]
        public string Type;

        [BsonElement("unique_synonym_identifier")]
        public string UniqueSynonymIdentifier;

        [BsonElement("ordered_as")]
        public string OrderedAs;

        [BsonElement("placed_order_as")]
        public string PlacedOrderAs;

        [BsonElement("original_ordered_date_time")]
        public string OriginalOrderedDateTime;

        [BsonElement("start_date_time")]
        public string StartDateTime;

        [BsonElement("suspend_date_time")]
        public string SuspendDateTime;

        [BsonElement("resume_date_time")]
        public string ResumeDateTime;

        [BsonElement("discontinue_date_time")]
        public string DiscontinueDateTime;

        [BsonElement("discontinue_type")]
        public string DiscontinueType;

        [BsonElement("stop_date_time")]
        public string StopDateTime;

        [BsonElement("stop_type")]
        public string StopType;

        [BsonElement("status")]
        public string Status;

        [BsonElement("active")]
        public string Active;

        [BsonElement("active_status_date_time")]
        public string ActiveStatusDateTime;

        [BsonElement("update_date_time")]
        public string UpdateDateTime;

        [BsonElement("ImportFile")]
        public String ImportFile;

        [BsonElement("ImportFileDate")]
        public String ImportFileDate;

        [BsonElement("lastaggregationrun")]
        public long LastAggregationRun;
#pragma warning restore 0649

        /// <summary>
        /// Selectively combines the attributes of this mongodb document with a supplied Medication record
        /// </summary>
        /// <param name="MedicationRecord">Call with null if there is no existing Medication record, the resulting record is returned here</param>
        /// <param name="PatientRecord"></param>
        /// <param name="VisitRecord"></param>
        /// <param name="ReferenceMedicationRecord"></param>
        /// <param name="MedicationInstructions"></param>
        /// <returns>true if an existing record was modified, false if a new record was created</returns>
        internal bool MergeWithExistingMedication(ref Medication MedicationRecord, ref Patient PatientRecord, VisitEncounter VisitRecord, MongodbReferenceMedicationEntity ReferenceMedicationRecord, string MedicationInstructions)
        {
            DateTime PrescriptionDate, StartDate, StopDate, StatusDateTime, FillDate, UpdateTime;
            DateTime.TryParse(UpdateDateTime, out UpdateTime);
            DateTime.TryParse("", out FillDate);                                    //Data does not include fill date
            DateTime.TryParse(OriginalOrderedDateTime, out PrescriptionDate);
            DateTime.TryParse(StartDateTime, out StartDate);
            DateTime.TryParse(StopDateTime, out StopDate);
            DateTime.TryParse(ActiveStatusDateTime, out StatusDateTime);

            if (MedicationRecord == null)
            {
                MedicationRecord = new Medication
                {
                    EmrIdentifier = UniqueMedicationIdentifier,
                    PrescriptionDate = PrescriptionDate,
                    FillDate = FillDate,
                    Description = OrderedAs,
                    StartDate = StartDate,
                    StopDate = StopDate,
                    Status = Status,
                    StatusDateTime = StatusDateTime,
                    Patientdbid = PatientRecord.dbid,
                    VisitEncounterdbid = VisitRecord.dbid,
                    Instructions = MedicationInstructions,
                    RxNorm = ReferenceMedicationRecord.RxNorm,
                    CatalogCKI = ReferenceMedicationRecord.CatalogCKI,
                    Dnum = ReferenceMedicationRecord.Dnum,
                    NDC = ReferenceMedicationRecord.NDC,
                    UpdateTime = UpdateTime,
                    LastImportFileDate = ImportFileDate
                };
                return false;
            }

            else if (MedicationRecord.UpdateTime < UpdateTime)
            {
                if (MedicationRecord.PrescriptionDate != PrescriptionDate && !String.IsNullOrEmpty(OriginalOrderedDateTime)) MedicationRecord.PrescriptionDate = PrescriptionDate;

                if (MedicationRecord.StartDate != StartDate && !String.IsNullOrEmpty(StartDateTime)) MedicationRecord.StartDate = StartDate;
                if (MedicationRecord.StopDate != StopDate && !String.IsNullOrEmpty(StopDateTime)) MedicationRecord.StopDate = StopDate;

                if (MedicationRecord.Status != Status && !String.IsNullOrEmpty(Status)) MedicationRecord.Status = Status;
                if (MedicationRecord.StatusDateTime != StatusDateTime && !String.IsNullOrEmpty(ActiveStatusDateTime)) MedicationRecord.StatusDateTime = StatusDateTime;
                if (MedicationRecord.Instructions != MedicationInstructions && !String.IsNullOrEmpty(MedicationInstructions)) MedicationRecord.Instructions = MedicationInstructions;

                if (MedicationRecord.RxNorm != ReferenceMedicationRecord.RxNorm && !String.IsNullOrEmpty(ReferenceMedicationRecord.RxNorm)) MedicationRecord.RxNorm = ReferenceMedicationRecord.RxNorm;
                if (MedicationRecord.CatalogCKI != ReferenceMedicationRecord.CatalogCKI && !String.IsNullOrEmpty(ReferenceMedicationRecord.CatalogCKI)) MedicationRecord.CatalogCKI = ReferenceMedicationRecord.CatalogCKI;
                if (MedicationRecord.Dnum != ReferenceMedicationRecord.Dnum && !String.IsNullOrEmpty(ReferenceMedicationRecord.Dnum)) MedicationRecord.Dnum = ReferenceMedicationRecord.Dnum;
                if (MedicationRecord.NDC != ReferenceMedicationRecord.NDC && !String.IsNullOrEmpty(ReferenceMedicationRecord.NDC)) MedicationRecord.NDC = ReferenceMedicationRecord.NDC;

                if (MedicationRecord.UpdateTime != UpdateTime && !String.IsNullOrEmpty(UpdateDateTime)) MedicationRecord.UpdateTime = UpdateTime;

                MedicationRecord.LastImportFileDate = new string[] { MedicationRecord.LastImportFileDate, ImportFileDate }.Max();
            }

            return true;
        }
    }
}
