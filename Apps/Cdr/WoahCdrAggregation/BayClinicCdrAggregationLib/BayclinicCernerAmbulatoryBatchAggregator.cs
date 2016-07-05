using System;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CdrContext;
using CdrDbLib;
using Devart.Data.Linq;
using MongoDbWrap;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using BayClinicCdrAggregationLib;
using System.Data.OleDb;
using System.Data;
using System.IO;
using WOHSQLInterface;



namespace BayClinicCernerAmbulatory
{
    class BayClinicCernerAmbulatoryBatchAggregator
    {
        public static readonly String WoahBayClinicOrganizationIdentity = "WOAH Bay Clinic";
        private const String FeedIdentity = "BayClinicCernerAmbulatoryExtract";

        private Mutex Mutx;
        private bool _EndThreadSignal;

        /// <summary>
        /// Property that encapsulates thread safe access to the end thread signal
        /// </summary>
        public bool EndThreadSignal
        {
            set
            {
                Mutx.WaitOne();
                _EndThreadSignal = value;
                Mutx.ReleaseMutex();
            }
            get
            {
                Mutx.WaitOne();
                bool ReturnVal = _EndThreadSignal;
                Mutx.ReleaseMutex();
                return ReturnVal;
            }
        }

        // TODO Some of these should be arguments from the caller, or sourced another way? 
        private String PgConnectionStringName = ConfigurationManager.AppSettings["CdrPostgreSQLConnectionString"];
        private String NewBayClinicAmbulatoryMongoCredentialConfigFile = ConfigurationManager.AppSettings["NewBayClinicAmbulatoryMongoCredentialConfigFile"];
        private String NewBayClinicAmbulatoryMongoCredentialSection = ConfigurationManager.AppSettings["NewBayClinicAmbulatoryMongoCredentialSection"];

        private int WOAHPatientCounter = 0;

        //Init the the interface to the sql dataset
        WOHSQLiteInterface WOHMembershipData = new WOHSQLiteInterface();

        private CdrDbInterface CdrDb;
        private Organization OrganizationObject;
        private DataFeed FeedObject;
        private AggregationRun ThisAggregationRun;
        private MongoDbConnection MongoCxn;
        private CernerReferencedCodeDictionaries ReferencedCodes;
        private CernerReferenceHealthPlanDictionaries ReferencedHealthPlans;
        private MongoAggregationRunUpdater MongoRunUpdater;

        IMongoCollection<MongodbIdentifierEntity> IdentifierCollection;
        IMongoCollection<MongodbPersonEntity> PersonCollection;
        IMongoCollection<MongodbRefCodeEntity> RefCodeCollection;
        IMongoCollection<MongodbPhoneEntity> PhoneCollection;
        IMongoCollection<MongodbAddressEntity> AddressCollection;
        IMongoCollection<MongodbVisitEntity> VisitCollection;
        IMongoCollection<MongodbChargeEntity> ChargeCollection;
        IMongoCollection<MongodbChargeDetailEntity> ChargeDetailCollection;
        IMongoCollection<MongodbResultEntity> ResultCollection;
        IMongoCollection<MongodbDiagnosisEntity> DiagnosisCollection;
        IMongoCollection<MongodbInsuranceEntity> InsuranceCollection;
        IMongoCollection<MongodbReferenceTerminologyEntity> ReferenceTerminologyCollection;
        IMongoCollection<MongodbImmunizationEntity> ImmunizationCollection;
        IMongoCollection<MongodbProblemEntity> ProblemCollection;
        IMongoCollection<MongodbMedicationEntity> MedicationCollection;
        IMongoCollection<MongodbMedicationReconciliationDetailEntity> MedicationReconciliationDetailCollection;
        IMongoCollection<MongodbReferenceMedicationEntity> ReferenceMedicationCollection;
        IMongoCollection<MongodbReferenceHealthPlanEntity> ReferenceHealthPlanCollection;

        public int PatientCounter;
        public int PhoneCounter;
        public int AddressCounter;
        public int IdentifierCounter;
        public int VisitCounter;
        public int ChargeCounter;
        public int ChargeDetailCounter;
        public int ResultCounter;
        public int DiagnosisCounter;
        public int InsuranceCoverageCounter;
        public int ImmunizationCounter;
        public int MedicationCounter;
        public int ProblemCounter;

        /// <summary>
        /// Constructor, instantiates member objects and stores the connection string name (default if not provided by caller)
        /// </summary>
        /// <param name="PgConnectionName"></param>
        public BayClinicCernerAmbulatoryBatchAggregator(String PgConnectionName = null)
        {
            if (String.IsNullOrEmpty(PgConnectionName))
            {
                PgConnectionName = PgConnectionStringName;
            }

            CdrDb = new CdrDbInterface(PgConnectionName, ConnectionArgumentType.ConnectionStringName);
            MongoCxn = new MongoDbConnection(NewBayClinicAmbulatoryMongoCredentialConfigFile, NewBayClinicAmbulatoryMongoCredentialSection);
            ReferencedCodes = new CernerReferencedCodeDictionaries();
            ReferencedHealthPlans = new CernerReferenceHealthPlanDictionaries();

            Mutx = new Mutex();
        }

        /// <summary>
        /// Initializes member instance variables to support this aggregation run
        /// </summary>
        /// <returns>boolean indicating success</returns>
        private bool InitializeRun()
        {
            bool Initialized;

            EndThreadSignal = false;

            // Initialize MongoDb Collections
            IdentifierCollection = MongoCxn.Db.GetCollection<MongodbIdentifierEntity>("identifiers");
            PersonCollection = MongoCxn.Db.GetCollection<MongodbPersonEntity>("person");
            RefCodeCollection = MongoCxn.Db.GetCollection<MongodbRefCodeEntity>("referencecode");
            PhoneCollection = MongoCxn.Db.GetCollection<MongodbPhoneEntity>("phone");
            AddressCollection = MongoCxn.Db.GetCollection<MongodbAddressEntity>("address");
            VisitCollection = MongoCxn.Db.GetCollection<MongodbVisitEntity>("visit");
            ChargeCollection = MongoCxn.Db.GetCollection<MongodbChargeEntity>("charge");
            ChargeDetailCollection = MongoCxn.Db.GetCollection<MongodbChargeDetailEntity>("chargedetail");
            ResultCollection = MongoCxn.Db.GetCollection<MongodbResultEntity>("result");
            DiagnosisCollection = MongoCxn.Db.GetCollection<MongodbDiagnosisEntity>("diagnosis");
            InsuranceCollection = MongoCxn.Db.GetCollection<MongodbInsuranceEntity>("insurance");
            ReferenceTerminologyCollection = MongoCxn.Db.GetCollection<MongodbReferenceTerminologyEntity>("referenceterminology");
            ImmunizationCollection = MongoCxn.Db.GetCollection<MongodbImmunizationEntity>("immunization");
            ProblemCollection = MongoCxn.Db.GetCollection<MongodbProblemEntity>("problem");
            MedicationCollection = MongoCxn.Db.GetCollection<MongodbMedicationEntity>("medications");
            MedicationReconciliationDetailCollection = MongoCxn.Db.GetCollection<MongodbMedicationReconciliationDetailEntity>("medicationreconciliationdetail");
            ReferenceMedicationCollection = MongoCxn.Db.GetCollection<MongodbReferenceMedicationEntity>("referencemedication");
            ReferenceHealthPlanCollection = MongoCxn.Db.GetCollection<MongodbReferenceHealthPlanEntity>("referencehealthplan");
            // TODO initialize collection

            // reset all counters to 0
            PatientCounter = PhoneCounter = AddressCounter = IdentifierCounter = VisitCounter = ChargeCounter = ChargeDetailCounter =
                ResultCounter = DiagnosisCounter = InsuranceCoverageCounter = ImmunizationCounter = MedicationCounter = ProblemCounter =
                0;

            Initialized = ReferencedCodes.Initialize(RefCodeCollection);
            Initialized  = ReferencedCodes.Initialize(RefCodeCollection);
            Initialized &= ReferencedHealthPlans.Initialize(ReferenceHealthPlanCollection, RefCodeCollection);
            ThisAggregationRun = GetNewAggregationRun();
            Initialized &= ThisAggregationRun.dbid > 0;

            //Connect to the sql database
            WOHMembershipData.ConnectToLatestMembership();

            MongoRunUpdater = new MongoAggregationRunUpdater(ThisAggregationRun.dbid, MongoCxn.Db);

            return Initialized;
        }

        public bool IsWOAHMember(MongodbPersonEntity PersonDocument)
        {

            if (CdrDb.Context.Patients.Count(p => p.EmrIdentifier == PersonDocument.UniquePersonIdentifier) > 0)
            {
                return true;
            }

            else
            {
                var InsuranceCoverageQuery = InsuranceCollection.AsQueryable()
                                                   .Where(x => x.UniqueEntityIdentifier == PersonDocument.UniquePersonIdentifier
                                                            && x.EntityType == "PERSON");

                foreach (MongodbInsuranceEntity InsuranceDoc in InsuranceCoverageQuery)
                {
                    if (ReferencedHealthPlans.PlanNameCodeMeanings[InsuranceDoc.UniqueHealthPlanIdentifier] == "WESTERN OREGON ADVANCED HEALTH")
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// Main callable method, iterates over all available patients in MongoDB to aggregate into PostgreSQL
        /// </summary>
        /// <param name="ClearRunNumbers">if true, resets all documents in MongoDB so they are processed</param>
        /// <returns></returns>
        public bool AggregateAllAvailablePatients(bool ClearRunNumbers = false)
        {
            bool OverallResult = true;
            int PatientCounter = 0;

            if (!InitializeRun())
            {
                return false;
            }

            if (ClearRunNumbers)
            {
                ClearAllMongoAggregationRunNumbers();
            }

            ThisAggregationRun.StatusFlags = AggregationRunStatus.InProcess;
            CdrDb.Context.SubmitChanges();

            FilterDefinition<MongodbPersonEntity> PatientFilterDef = Builders<MongodbPersonEntity>.Filter.Where(x =>
                           x.UniquePersonIdentifier != ""  // has an identifier to be referenced from other txt files
                        && !(x.LastAggregationRun > 0)     // not previously aggregated
                      );

            using (var PersonCursor = PersonCollection.Find<MongodbPersonEntity>(PatientFilterDef)
                                                      .SortBy(x => x.ImportFileDate)
                                                      .ToCursor())
            {
                while (PersonCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbPersonEntity PersonDocument in PersonCursor.Current)
                    {
                        PatientCounter++;
                        if(IsWOAHMember(PersonDocument))
                        {
                            WOAHPatientCounter++;
                            bool ThisPatientAggregationResult = AggregateOnePatient(PersonDocument);
                            OverallResult &= ThisPatientAggregationResult;
                        }

                        if (EndThreadSignal)
                        {
                            goto EndProcessing;
                        }
                    }
                }
            }

        EndProcessing:
            //Disconnect after the the database is done being used
            WOHMembershipData.Disconnect();

            ThisAggregationRun.StatusFlags = AggregationRunStatus.Complete;
            CdrDb.Context.SubmitChanges();

            Trace.WriteLine("Processed " + PatientCounter + " patients");
            Trace.WriteLine("Processed " + WOAHPatientCounter + "WOAH covered patients");
            return OverallResult;
        }

        /// <summary>
        /// Aggregate all data related to one patient document from Mongo and manage the PostgreSQL storage transaction
        /// </summary>
        /// <param name="PersonDocument"></param>
        /// <returns></returns>
        private bool AggregateOnePatient(MongodbPersonEntity PersonDocument)
        {
            //DateTime ParsedDateTime;
            bool OverallSuccess = true;

            // Figure out whether this patient is already in the database
            var query = from Pat in CdrDb.Context.Patients
                        where Pat.EmrIdentifier == PersonDocument.UniquePersonIdentifier
                        select Pat;
            Patient PatientRecord = query.FirstOrDefault();  // Returns existing record or null

            #region PostgreSQL transaction to process all data for one patient
            CdrDb.Context.Connection.Open();
            CdrDb.Context.Transaction = CdrDb.Context.Connection.BeginTransaction();

            // Store to database
            if (PersonDocument.MergeWithExistingPatient(ref PatientRecord, ReferencedCodes)) { int i = 42; }//Debugging breakpoint

            else
            {
                CdrDb.Context.Patients.InsertOnSubmit(PatientRecord);
            }
            CdrDb.Context.SubmitChanges();  // TODO Is it possible that only one SubmitChanges call for the entire transaction is more efficient?  

            MongoRunUpdater.PersonIdList.Add(PersonDocument.Id);

            // TODO maybe do a quality check on PatientRecord before proceeding to aggregate the referencing entities.  

            // Aggregate entities that are linked to this patient (entities linked to patient and visit are called from the visit aggregation method)
            OverallSuccess &= AggregateTelephoneNumbers(PersonDocument, PatientRecord);
            OverallSuccess &= AggregateAddresses(PersonDocument, PatientRecord);
            OverallSuccess &= AggregateIdentifiers(PersonDocument, PatientRecord);
            OverallSuccess &= AggregateVisits(PersonDocument, PatientRecord);
            OverallSuccess &= AggregateInsuranceCoverages(PersonDocument, PatientRecord);
            OverallSuccess &= AggregateProblems(PersonDocument, PatientRecord);  // Bay Clinic does not have links between problem and a visit

            if (OverallSuccess)
            {
                WOAHPatientCounter++;
                CdrDb.Context.Transaction.Commit();
                MongoRunUpdater.UpdateAll();
            }
            else
            {
                CdrDb.Context.Transaction.Rollback();
                MongoRunUpdater.Reset();
            }

            CdrDb.Context.Connection.Close();
            #endregion  PostgreSQL transaction to process all data for one patient

            return OverallSuccess;
        }

        /// <summary>
        /// Aggregate the telephone numbers associated with a specified patient
        /// </summary>
        /// <param name="PersonDoc"></param>
        /// <param name="PgPatient"></param>
        /// <returns></returns>
         private bool AggregateTelephoneNumbers(MongodbPersonEntity PersonDoc, Patient PgPatient)
        {
            FilterDefinition<MongodbPhoneEntity> PhoneFilterDef = Builders<MongodbPhoneEntity>.Filter
                .Where(
                         x => x.EntityType == "PERSON"
                      && x.EntityIdentifier == PersonDoc.UniquePersonIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      && x.ImportFileDate == GetImportDateFromFileName(PersonDoc.ImportFile)
                      //&& (x.ImportFile.StartsWith(PersonDoc.ImportFile.Substring(0, 12)))  // match the extract date from PersonDoc.ImportFile 
                      );

            using (var PhoneCursor = PhoneCollection.Find<MongodbPhoneEntity>(PhoneFilterDef).ToCursor())
            {
                while (PhoneCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbPhoneEntity PhoneDoc in PhoneCursor.Current)
                    {

                        var DuplicateTelephoneNumberQuery = from Phone in PgPatient.TelephoneNumbers
                                                            where PhoneDoc.PhoneNumber == Phone.Number.Number
                                                            select Phone;

                        TelephoneNumber NewPgRecord = DuplicateTelephoneNumberQuery.FirstOrDefault();

                        if (PhoneDoc.MergeWithExistingTelephoneNumber(ref NewPgRecord, ref PgPatient, ReferencedCodes)) { int i = 42; }


                        CdrDb.Context.TelephoneNumbers.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.PhoneIdList.Add(PhoneDoc.Id);

                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Aggregate the address data associated with a specified patient
        /// </summary>
        /// <param name="PersonDoc"></param>
        /// <param name="PgPatient"></param>
        /// <returns></returns>
        private bool AggregateAddresses(MongodbPersonEntity PersonDoc, Patient PgPatient)
        {
            FilterDefinition<MongodbAddressEntity> AddressFilterDef = Builders<MongodbAddressEntity>.Filter
                .Where(
                         x => x.EntityType == "PERSON"
                      && x.EntityIdentifier == PersonDoc.UniquePersonIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      && x.ImportFileDate == GetImportDateFromFileName(PersonDoc.ImportFile)
                      //&& (x.ImportFile.StartsWith(PersonDoc.ImportFile.Substring(0, 12)))  // match the extract date from PersonDoc.ImportFile 
                      );

            using (var AddressCursor = AddressCollection.Find<MongodbAddressEntity>(AddressFilterDef).ToCursor())
            {
                while (AddressCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbAddressEntity AddressDoc in AddressCursor.Current)
                    {

                        var DuplicateAddressrQuery = from PatientAddress in PgPatient.PhysicalAddresses
                                                     where PatientAddress.Address.Line1 == AddressDoc.AddressLine1 && PatientAddress.Address.City == AddressDoc.CityText //Should I check more fields?
                                                     select PatientAddress;

                        PhysicalAddress NewPgRecord = DuplicateAddressrQuery.FirstOrDefault();

                        if (AddressDoc.MergeWithExistingPhysicalAddress(ref NewPgRecord, ref PgPatient, ReferencedCodes)) { int i = 42; }


                        CdrDb.Context.PhysicalAddresses.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.AddressIdList.Add(AddressDoc.Id);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Aggregate the identifiers associated with a specified patient
        /// </summary>
        /// <param name="PersonDoc"></param>
        /// <param name="PgPatient"></param>
        /// <returns></returns>
        private bool AggregateIdentifiers(MongodbPersonEntity PersonDoc, Patient PgPatient)
        {
            FilterDefinition<MongodbIdentifierEntity> IdentifierFilterDef = Builders<MongodbIdentifierEntity>.Filter
                .Where(
                         x => x.EntityType == "PERSON"
                      && x.EntityIdentifier == PersonDoc.UniquePersonIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      && x.ImportFileDate == GetImportDateFromFileName(PersonDoc.ImportFile)
                      //&& (x.ImportFile.StartsWith(MongoPerson.ImportFile.Substring(0, 12)))  // match the extract date from PersonDoc.ImportFile 
                      );

            using (var IdentifierCursor = IdentifierCollection.Find<MongodbIdentifierEntity>(IdentifierFilterDef).ToCursor())
            {
                while (IdentifierCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbIdentifierEntity IdentifierDoc in IdentifierCursor.Current)
                    {
                        var DuplicatePatientIdentifierQuery = from Identifier in PgPatient.PatientIdentifiers
                                                              where IdentifierDoc.Identifier == Identifier.Identifier
                                                              select Identifier;

                        PatientIdentifier NewPgRecord = DuplicatePatientIdentifierQuery.FirstOrDefault();

                        if (IdentifierDoc.MergeWithExistingPatientIdentifiers(ref NewPgRecord, ref PgPatient, ReferencedCodes, OrganizationObject)) { int i = 42; }

                        CdrDb.Context.PatientIdentifiers.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.IdentifiersIdList.Add(IdentifierDoc.Id);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Aggregate the visit data associated with a specified patient
        /// </summary>
        /// <param name="PersonDoc"></param>
        /// <param name="PatientRecord"></param>
        /// <returns></returns>
        private bool AggregateVisits(MongodbPersonEntity PersonDoc, Patient PatientRecord)
        {
            bool OverallSuccess = true;

            //Retrieves all of the visits in Mongo that are related to the patient and not already aggregated
            FilterDefinition<MongodbVisitEntity> VisitFilterDef = Builders<MongodbVisitEntity>.Filter
                .Where(
                         x => x.UniquePersonIdentifier == PersonDoc.UniquePersonIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      && x.ImportFileDate == GetImportDateFromFileName(PersonDoc.ImportFile)
                      //&& (x.ImportFile.StartsWith(PersonDoc.ImportFile.Substring(0, 12)))  // match the extract date from PersonDoc.ImportFile 
                      );

            using (var VisitCursor = VisitCollection.Find<MongodbVisitEntity>(VisitFilterDef).ToCursor())
            {
                while (VisitCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbVisitEntity VisitDoc in VisitCursor.Current)
                    {
                        //Should return any new visits that are already in the cdr database
                        var DuplicateVisitQuery = from Visit in PatientRecord.VisitEncounters
                                                  where VisitDoc.UniqueVisitIdentifier == Visit.EmrIdentifier
                                                  select Visit;

                        VisitEncounter NewPgRecord = DuplicateVisitQuery.FirstOrDefault();

                        if (VisitDoc.MergeWithExistingVisit(ref NewPgRecord, ref PatientRecord, ReferencedCodes, ref CdrDb))
                        {
                            // Record changes will persist by SubmitChanges()
                            int i = 42;  // debugging breakpoint
                        }
                        else
                        {
                            VisitCounter++;
                        }

                        CdrDb.Context.VisitEncounters.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.VisitIdList.Add(VisitDoc.Id);

                        // Aggregate entities that are linked to this visit
                        OverallSuccess &= AggregateCharges(VisitDoc, NewPgRecord);
                        OverallSuccess &= AggregateResults(PersonDoc, PatientRecord, VisitDoc, NewPgRecord);
                        OverallSuccess &= AggregateDiagnosis(PersonDoc, PatientRecord, VisitDoc, NewPgRecord);
                        OverallSuccess &= AggregateImmunizations(PersonDoc, PatientRecord, VisitDoc, NewPgRecord);
                        OverallSuccess &= AggregateMedications(PersonDoc, PatientRecord, VisitDoc, NewPgRecord);

                    }
                }
            }

            return OverallSuccess;
        }

        /// <summary>
        /// Aggregate the charge data associated with a specified visit
        /// </summary>
        /// <param name="VisitDoc"></param>
        /// <param name="PgVisit"></param>
        /// <returns></returns>
        private bool AggregateCharges(MongodbVisitEntity VisitDoc, VisitEncounter PgVisit)
        {
            FilterDefinition<MongodbChargeEntity> ChargeFilterDef = Builders<MongodbChargeEntity>.Filter
                .Where(
                         x => x.UniqueVisitIdentifier == VisitDoc.UniqueVisitIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      && x.ImportFileDate == GetImportDateFromFileName(VisitDoc.ImportFile)
                      //&& (x.ImportFile.StartsWith(MongoVisit.ImportFile.Substring(0, 12)))  // match the extract date from PersonDoc.ImportFile 
                      );

            using (var ChargeCursor = ChargeCollection.Find<MongodbChargeEntity>(ChargeFilterDef).ToCursor())
            {
                while (ChargeCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbChargeEntity ChargeDoc in ChargeCursor.Current)
                    {
                        var DuplicateChargeQuery = from Charge in PgVisit.Charges
                                                   where Charge.EmrIdentifier == ChargeDoc.UniqueChargeIdentifier
                                                   select Charge;

                        Charge NewPgRecord = DuplicateChargeQuery.FirstOrDefault();
                        if (ChargeDoc.MergeWithExistingCharges(ref NewPgRecord, ref PgVisit)) { int i = 42; }

                        CdrDb.Context.Charges.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.ChargeIdList.Add(ChargeDoc.Id);

                        AggregateChargeDetails(ChargeDoc, NewPgRecord);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Aggregate the charge detail data associated with a specified visit
        /// </summary>
        /// <param name="ChargeDoc"></param>
        /// <param name="ChargeRecord"></param>
        /// <returns></returns>
        private bool AggregateChargeDetails(MongodbChargeEntity ChargeDoc, Charge ChargeRecord)
        {
            FilterDefinition<MongodbChargeDetailEntity> ChargeDetailFilterDef = Builders<MongodbChargeDetailEntity>.Filter
                .Where(
                         x => x.UniqueChargeItemIdentifier == ChargeDoc.UniqueChargeItemIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      && x.ImportFileDate == GetImportDateFromFileName(ChargeDoc.ImportFile)
                      //&& (x.ImportFile.StartsWith(ChargeDoc.ImportFile.Substring(0, 12)))  // match the extract date from PersonDoc.ImportFile 
                      );

            using (var ChargeDetailCursor = ChargeDetailCollection.Find<MongodbChargeDetailEntity>(ChargeDetailFilterDef).ToCursor())
            {
                while (ChargeDetailCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbChargeDetailEntity ChargeDetailDoc in ChargeDetailCursor.Current)
                    {
                        var DuplicateChargeDetailQuery = from ChargeDetail in ChargeRecord.ChargeCodes
                                                         where ChargeDetail.EmrIdentifier == ChargeDetailDoc.UniqueChargeItemIdentifier
                                                         select ChargeDetail;
                        ChargeCode NewPgRecord = DuplicateChargeDetailQuery.FirstOrDefault();
                        if (!ChargeDetailDoc.MergeWithExistingChargeCodes(ref NewPgRecord, ref ChargeRecord, ReferencedCodes)) { int i = 42;  }

                        CdrDb.Context.ChargeCodes.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.ChargeDetailIdList.Add(ChargeDetailDoc.Id);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Aggregate the result data associated with a specified patient and visit
        /// </summary>
        /// <param name="PersonDoc"></param>
        /// <param name="PatientRecord"></param>
        /// <param name="VisitDoc"></param>
        /// <param name="VisitRecord"></param>
        /// <returns></returns>
        private bool AggregateResults(MongodbPersonEntity PersonDoc, Patient PatientRecord, MongodbVisitEntity VisitDoc, VisitEncounter VisitRecord)
        {
            bool Success = true;


            FilterDefinition<MongodbResultEntity> ResultFilterDef = Builders<MongodbResultEntity>.Filter
                .Where(x =>
                       x.UniquePersonIdentifier == PersonDoc.UniquePersonIdentifier
                    && x.UniqueVisitIdentifier == VisitDoc.UniqueVisitIdentifier
                    && !(x.LastAggregationRun > 0)     // not previously aggregated
                    && x.ImportFileDate == GetImportDateFromFileName(PersonDoc.ImportFile)
                    //&& (x.ImportFile.StartsWith(PersonDoc.ImportFile.Substring(0, 12)))  // match the extract date from PersonDoc.ImportFile 
                      );

            using (var ResultCursor = ResultCollection.Find<MongodbResultEntity>(ResultFilterDef).ToCursor())
            {
                while (ResultCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbResultEntity ResultDoc in ResultCursor.Current)
                    {
                        var DuplicateResultsQuery = from Result in PatientRecord.Measurements
                                                      where Result.EmrIdentifier == ResultDoc.UniqueResultIdentifier
                                                      select Result;
                        Measurement NewPgRecord = DuplicateResultsQuery.FirstOrDefault();
                        if (!ResultDoc.MergeWithExistingMeasurements(ref NewPgRecord, ref PatientRecord, VisitRecord, ReferencedCodes)) { int i = 42; }

                        CdrDb.Context.Measurements.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.ResultIdList.Add(ResultDoc.Id);
                    }
                }
            }

            return Success;
        }

        /// <summary>
        /// Aggregate the diagnosis data associated with a specified patient and visit
        /// </summary>
        /// <param name="PersonDoc"></param>
        /// <param name="PatientRecord"></param>
        /// <param name="VisitDoc"></param>
        /// <param name="VisitRecord"></param>
        /// <returns></returns>
        private bool AggregateDiagnosis(MongodbPersonEntity PersonDoc, Patient PatientRecord, MongodbVisitEntity VisitDoc, VisitEncounter VisitRecord)
        {
            bool Success = true;

            FilterDefinition<MongodbDiagnosisEntity> DiagnosisFilterDef = Builders<MongodbDiagnosisEntity>.Filter
                .Where(x =>
                       x.UniquePersonIdentifier == PersonDoc.UniquePersonIdentifier
                    && x.UniqueVisitIdentifier == VisitDoc.UniqueVisitIdentifier
                    && !(x.LastAggregationRun > 0)     // not previously aggregated
                    && x.ImportFileDate == GetImportDateFromFileName(PersonDoc.ImportFile)
                    //&& (x.ImportFile.StartsWith(PersonDoc.ImportFile.Substring(0, 12)))  // match the extract date from PersonDoc.ImportFile 
                      );

            using (var DiagnosisCursor = DiagnosisCollection.Find<MongodbDiagnosisEntity>(DiagnosisFilterDef).ToCursor())
            {
                while (DiagnosisCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbDiagnosisEntity DiagnosisDoc in DiagnosisCursor.Current)
                    {

                        var DuplicateDiagnosisQuery = from Diagnosis in PatientRecord.Diagnoses
                                                      where Diagnosis.EmrIdentifier == DiagnosisDoc.UniqueDiagnosisIdentifier
                                                      select Diagnosis;
                        Diagnosis NewPgRecord = DuplicateDiagnosisQuery.FirstOrDefault();

                        var Query = ReferenceTerminologyCollection.AsQueryable()
                            .Where(x => x.UniqueTerminologyIdentifier.ToUpper() == DiagnosisDoc.UniqueTerminologyIdentifier)
                            //.Select(x => new { x.Key.ElementCode, x.Key.Display })
                            ;
                        MongodbReferenceTerminologyEntity TerminologyRecord = Query.FirstOrDefault();
                        if (TerminologyRecord == null)
                        {
                            TerminologyRecord = new MongodbReferenceTerminologyEntity { Code = "", Text = "", Terminology = "0" };
                        }

                        if (!DiagnosisDoc.MergeWithExistingDiagnoses(ref NewPgRecord, ref PatientRecord, VisitRecord, ReferencedCodes, TerminologyRecord)) { int i = 42; }
                   
                        CdrDb.Context.Diagnoses.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.DiagnosisIdList.Add(DiagnosisDoc.Id);
                    }
                }
            }

            return Success;
        }


        /// <summary>
        /// Aggregate the insurance associated with a specified patient
        /// </summary>
        /// <param name="PersonDoc"></param>
        /// <param name="PgPatient"></param>
        /// <returns></returns>
        private bool AggregateInsuranceCoverages(MongodbPersonEntity PersonDoc, Patient PgPatient)
        {
            FilterDefinition<MongodbInsuranceEntity> InsuranceCoverageFilterDef = Builders<MongodbInsuranceEntity>.Filter
                .Where(
                         x => x.EntityType == "PERSON"
                      && x.UniqueEntityIdentifier == PersonDoc.UniquePersonIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      && x.ImportFileDate == GetImportDateFromFileName(PersonDoc.ImportFile)
                      //&& (x.ImportFile.StartsWith(MongoPerson.ImportFile.Substring(0, 12)))  // match the extract date from PersonDoc.ImportFile 
                      );

            using (var InsuranceCoverageCursor = InsuranceCollection.Find<MongodbInsuranceEntity>(InsuranceCoverageFilterDef).ToCursor())
            {
                while (InsuranceCoverageCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbInsuranceEntity InsuranceCoverageDoc in InsuranceCoverageCursor.Current)
                    {

                        var DuplicateInsuranceCoverageQuery = from Insurance in PgPatient.InsuranceCoverages
                                                              where Insurance.Payer == InsuranceCoverageDoc.UniqueOrganizationIdentifier && Insurance.PlanName == InsuranceCoverageDoc.UniqueHealthPlanIdentifier
                                                              select Insurance;
                        InsuranceCoverage NewPgRecord = DuplicateInsuranceCoverageQuery.FirstOrDefault();
                        if(!InsuranceCoverageDoc.MergeWithExistingInsuranceCoverage(ref NewPgRecord, ref PgPatient, ReferencedCodes)) { int i = 42; }


                        CdrDb.Context.InsuranceCoverages.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.InsuranceIdList.Add(InsuranceCoverageDoc.Id);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Aggregate the immunization data associated with a specified patient and visit
        /// </summary>
        /// <param name="PersonDoc"></param>
        /// <param name="PatientRecord"></param>
        /// <param name="VisitDoc"></param>
        /// <param name="VisitRecord"></param>
        /// <returns></returns>
        private bool AggregateImmunizations(MongodbPersonEntity PersonDoc, Patient PatientRecord, MongodbVisitEntity VisitDoc, VisitEncounter VisitRecord)
        {
            FilterDefinition<MongodbImmunizationEntity> ImmunizationFilterDef = Builders<MongodbImmunizationEntity>.Filter
                .Where(x =>
                       x.UniquePersonIdentifier == PersonDoc.UniquePersonIdentifier
                    && x.UniqueVisitIdentifier == VisitDoc.UniqueVisitIdentifier
                    && !(x.LastAggregationRun > 0)     // not previously aggregated
                    && x.ImportFileDate == GetImportDateFromFileName(PersonDoc.ImportFile)
                    //&& (x.ImportFile.StartsWith(PersonDoc.ImportFile.Substring(0, 12)))  // match the extract date from PersonDoc.ImportFile 
                      );

            using (var ImmunizationCursor = ImmunizationCollection.Find<MongodbImmunizationEntity>(ImmunizationFilterDef).ToCursor())
            {
                while (ImmunizationCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbImmunizationEntity ImmunizationDoc in ImmunizationCursor.Current)
                    {
                        var DuplicateImmunizationQuery = from Immunization in PatientRecord.Immunizations
                                                              where Immunization.ResultID == ImmunizationDoc.UniqueResultIdentifier 
                                                              && Immunization.VisitID == ImmunizationDoc.UniqueVisitIdentifier
                                                              select Immunization;
                        Immunization NewPgRecord = DuplicateImmunizationQuery.FirstOrDefault();
                        if (!ImmunizationDoc.MergeWithExistingImmunizations(ref NewPgRecord, ref PatientRecord, VisitRecord, ReferencedCodes)) { int i = 42; }

                        
                        CdrDb.Context.Immunizations.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.ImmunizationIdList.Add(ImmunizationDoc.Id);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Aggregate the medication data associated with a specified patient and visit
        /// </summary>
        /// <param name="PersonDoc"></param>
        /// <param name="PatientRecord"></param>
        /// <param name="VisitDoc"></param>
        /// <param name="VisitRecord"></param>
        /// <returns></returns>
        private bool AggregateMedications(MongodbPersonEntity PersonDoc, Patient PatientRecord, MongodbVisitEntity VisitDoc, VisitEncounter VisitRecord)
        {
            FilterDefinition<MongodbMedicationEntity> MedicationFilterDef = Builders<MongodbMedicationEntity>.Filter
                .Where(x =>
                       x.UniquePersonIdentifier == PersonDoc.UniquePersonIdentifier
                    && x.UniqueVisitIdentifier == VisitDoc.UniqueVisitIdentifier
                    && !(x.LastAggregationRun > 0)     // not previously aggregated
                    && x.ImportFileDate == GetImportDateFromFileName(PersonDoc.ImportFile)
                    //&& (x.ImportFile.StartsWith(PersonDoc.ImportFile.Substring(0, 12)))  // match the extract date from PersonDoc.ImportFile 
                      );

            using (var MedicationCursor = MedicationCollection.Find<MongodbMedicationEntity>(MedicationFilterDef).ToCursor())
            {
                while (MedicationCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbMedicationEntity MedicationDoc in MedicationCursor.Current)
                    {
                        var DuplicateMedicationsQuery = from Medicine in PatientRecord.Medications
                                                      where Medicine.EmrIdentifier == MedicationDoc.UniqueMedicationIdentifier
                                                      select Medicine;
                        Medication NewPgRecord = DuplicateMedicationsQuery.FirstOrDefault();



                        //Get instructions
                        string MedicationInstructions;
                        var MedicationReconciliationDetailQuery = MedicationReconciliationDetailCollection.AsQueryable()
                                                                    .Where(x => x.UniqueMedicationIdentifier == MedicationDoc.UniqueMedicationIdentifier);

                        MongodbMedicationReconciliationDetailEntity MedicationReconciliationDetailRecord = MedicationReconciliationDetailQuery.FirstOrDefault();

                        if (MedicationReconciliationDetailRecord == null)
                        {
                            MedicationReconciliationDetailRecord = new MongodbMedicationReconciliationDetailEntity { ClinicalDisplay = "", SimplifiedDisplay = "" };
                        }


                        if (!string.IsNullOrEmpty(MedicationReconciliationDetailRecord.ClinicalDisplay))
                        {
                            MedicationInstructions = MedicationReconciliationDetailRecord.ClinicalDisplay;
                        }
                        else if (string.IsNullOrEmpty(MedicationReconciliationDetailRecord.ClinicalDisplay) && !string.IsNullOrEmpty(MedicationReconciliationDetailRecord.SimplifiedDisplay))
                        {
                            MedicationInstructions = MedicationReconciliationDetailRecord.SimplifiedDisplay;
                        }
                        else
                        {
                            MedicationInstructions = "";
                        }

                        //Get rxnorm codes
                        var ReferenceMedicationQuery = ReferenceMedicationCollection.AsQueryable()
                                                                   .Where(x => x.UniqueSynonymIdentifier == MedicationDoc.UniqueSynonymIdentifier);

                        MongodbReferenceMedicationEntity ReferenceMedicationRecord = ReferenceMedicationQuery.FirstOrDefault();
                        if (ReferenceMedicationRecord == null)
                        {
                            ReferenceMedicationRecord = new MongodbReferenceMedicationEntity { RxNorm = "", CatalogCKI = "", Dnum = "", NDC = "" };
                        }
                        if (!MedicationDoc.MergeWithExistingMedications(ref NewPgRecord, ref PatientRecord, VisitRecord, ReferenceMedicationRecord, MedicationInstructions))
                        {
                            int i = 42;
                        }

                        /*
                       
                        MedicationCounter++;
                        


                        


                        */
                        CdrDb.Context.Medications.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.MedicationsIdList.Add(MedicationDoc.Id);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Aggregate the problem data associated with a specified patient
        /// </summary>
        /// <param name="PersonDoc"></param>
        /// <param name="PatientRecord"></param>
        /// <param name="VisitDoc"></param>
        /// <param name="VisitRecord"></param>
        /// <returns></returns>
        private bool AggregateProblems(MongodbPersonEntity PersonDoc, Patient PatientRecord, MongodbVisitEntity VisitDoc = null, VisitEncounter VisitRecord = null)
        {
            FilterDefinitionBuilder<MongodbProblemEntity> ProblemFilterBuilder = Builders<MongodbProblemEntity>.Filter;

            FilterDefinition<MongodbProblemEntity> ProblemFilterDef = ProblemFilterBuilder.Where(x =>
                       x.UniquePersonIdentifier == PersonDoc.UniquePersonIdentifier
                    && !(x.LastAggregationRun > 0)     // not previously aggregated
                    && x.ImportFileDate == GetImportDateFromFileName(PersonDoc.ImportFile)
                    //&& (x.ImportFile.StartsWith(PersonDoc.ImportFile.Substring(0,12)))  // match the extract date from PersonDoc.ImportFile
                      );

            // Following logic will be relevant for Allscripts, not Cerner
            //if (VisitDoc != null)
            //    ProblemFilterDef = ProblemFilterDef & ProblemFilterBuilder.Where(x => x.UniqueVisitIdentifier == VisitDoc.UniqueVisitIdentifier);

            using (var ProblemCursor = ProblemCollection.Find<MongodbProblemEntity>(ProblemFilterDef).ToCursor())
            {
                while (ProblemCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbProblemEntity ProblemDoc in ProblemCursor.Current)
                    {
                        var DuplicateProblemQuery = from Problems in PatientRecord.Problems
                                                              where Problems.EmrIdentifier == ProblemDoc.UniqueProblemIdentifier
                                                              select Problems;
                        Problem NewPgRecord = DuplicateProblemQuery.FirstOrDefault();
                        if (!ProblemDoc.MergeWithExistingProblems(ref NewPgRecord, ref PatientRecord)) { int i = 42; }
                       
                        CdrDb.Context.Problems.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.ProblemIdList.Add(ProblemDoc.Id);
                    }
                }
            }

            return true;
        }

        private String GetImportDateFromFileName(String FileName)
        {
            return FileName.Substring(8, 4) + FileName.Substring(4, 2) + FileName.Substring(6, 2);
        }

        /// <summary>
        /// Establishes, stores, and returns a new aggregation run record for the current run
        /// </summary>
        /// <returns></returns>
        private AggregationRun GetNewAggregationRun()
        {
            OrganizationObject = CdrDb.EnsureOrganizationRecord(WoahBayClinicOrganizationIdentity);
            FeedObject = CdrDb.EnsureFeedRecord(FeedIdentity, OrganizationObject);

            AggregationRun NewRun = new AggregationRun
            {
                DataFeeddbid = FeedObject.dbid,
                StatusFlags = AggregationRunStatus.RunNumberReserved
            };

            CdrDb.Context.AggregationRuns.InsertOnSubmit(NewRun);
            try
            {
                CdrDb.Context.SubmitChanges();
            }
            catch (Exception e)
            {
                string x = e.Message;
            }

            return NewRun;
        }

        /// <summary>
        /// Clears all aggregation run numbers from documents in MongoDB in order to permit a full database aggregation
        /// </summary>
        private void ClearAllMongoAggregationRunNumbers()
        {
            UpdateResult Result;

            FilterDefinition<MongodbPersonEntity> PersonFilterDef = Builders<MongodbPersonEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbPersonEntity> PersonUpdateDef = Builders<MongodbPersonEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = PersonCollection.UpdateMany(PersonFilterDef, PersonUpdateDef);

            FilterDefinition<MongodbPhoneEntity> PhoneFilterDef = Builders<MongodbPhoneEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbPhoneEntity> PhoneUpdateDef = Builders<MongodbPhoneEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = PhoneCollection.UpdateMany(PhoneFilterDef, PhoneUpdateDef);

            FilterDefinition<MongodbAddressEntity> AddressFilterDef = Builders<MongodbAddressEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbAddressEntity> AddressUpdateDef = Builders<MongodbAddressEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = AddressCollection.UpdateMany(AddressFilterDef, AddressUpdateDef);

            FilterDefinition<MongodbIdentifierEntity> IdentifierFilterDef = Builders<MongodbIdentifierEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbIdentifierEntity> IdentifierUpdateDef = Builders<MongodbIdentifierEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = IdentifierCollection.UpdateMany(IdentifierFilterDef, IdentifierUpdateDef);

            FilterDefinition<MongodbVisitEntity> VisitFilterDef = Builders<MongodbVisitEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbVisitEntity> VisitUpdateDef = Builders<MongodbVisitEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = VisitCollection.UpdateMany(VisitFilterDef, VisitUpdateDef);

            FilterDefinition<MongodbChargeEntity> ChargeFilterDef = Builders<MongodbChargeEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbChargeEntity> ChargeUpdateDef = Builders<MongodbChargeEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = ChargeCollection.UpdateMany(ChargeFilterDef, ChargeUpdateDef);

            FilterDefinition<MongodbChargeDetailEntity> ChargeDetailFilterDef = Builders<MongodbChargeDetailEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbChargeDetailEntity> ChargeDetailUpdateDef = Builders<MongodbChargeDetailEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = ChargeDetailCollection.UpdateMany(ChargeDetailFilterDef, ChargeDetailUpdateDef);

            FilterDefinition<MongodbResultEntity> ResultFilterDef = Builders<MongodbResultEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbResultEntity> ResultUpdateDef = Builders<MongodbResultEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = ResultCollection.UpdateMany(ResultFilterDef, ResultUpdateDef);

            FilterDefinition<MongodbInsuranceEntity> InsuranceFilterDef = Builders<MongodbInsuranceEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbInsuranceEntity> InsuranceUpdateDef = Builders<MongodbInsuranceEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = InsuranceCollection.UpdateMany(InsuranceFilterDef, InsuranceUpdateDef);

            FilterDefinition<MongodbImmunizationEntity> ImmunizationFilterDef = Builders<MongodbImmunizationEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbImmunizationEntity> ImmunizationUpdateDef = Builders<MongodbImmunizationEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = ImmunizationCollection.UpdateMany(ImmunizationFilterDef, ImmunizationUpdateDef);

            FilterDefinition<MongodbDiagnosisEntity> DiagnosisFilterDef = Builders<MongodbDiagnosisEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbDiagnosisEntity> DiagnosisUpdateDef = Builders<MongodbDiagnosisEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = DiagnosisCollection.UpdateMany(DiagnosisFilterDef, DiagnosisUpdateDef);

            FilterDefinition<MongodbProblemEntity> ProblemFilterDef = Builders<MongodbProblemEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbProblemEntity> ProblemUpdateDef = Builders<MongodbProblemEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = ProblemCollection.UpdateMany(ProblemFilterDef, ProblemUpdateDef);

            FilterDefinition<MongodbMedicationEntity> MedicationFilterDef = Builders<MongodbMedicationEntity>.Filter.Where(x => x.LastAggregationRun > 0);
            UpdateDefinition<MongodbMedicationEntity> MedicationUpdateDef = Builders<MongodbMedicationEntity>.Update.Unset(x => x.LastAggregationRun);
            Result = MedicationCollection.UpdateMany(MedicationFilterDef, MedicationUpdateDef);

        }

    }
}

