using System;
using System.Net;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CdrContext;
using CdrDbLib;
using MongoDbWrap;
using MongoDB.Driver;
using System.Data;
using System.IO;

namespace BayClinicCernerAmbulatory
{
    class BayClinicCernerAmbulatoryBatchAggregator
    {
        public static readonly String WoahBayClinicOrganizationIdentity = "WOAH Bay Clinic";
        private const String FeedIdentity = "bayclinictest";  // TODO move this to config shared between aggregation and extraction applications

        private Mutex Mutx;
        private bool _EndThreadSignal;
        private String _PgConnectionName;
        //private String MembershipDataFileUsed;

        //private StreamWriter CsvWriter;
        private DateTime PatientLoopStart;
        long WriteCounter;

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
        private String NewBayClinicEmrMongoCredentialConfigFile = ConfigurationManager.AppSettings["NewBayClinicEmrMongoCredentialConfigFile"];
        private String NewBayClinicEmrMongoCredentialSection = ConfigurationManager.AppSettings["NewBayClinicEmrMongoCredentialSection"];

        private CdrDbInterface CdrDb;
        private long OrganizationDbid;
        private long ThisAggregationRunDbid;
        private MongoDbConnection MongoCxn;
        private CernerReferencedCodeDictionaries ReferencedCodes;
        private CernerReferenceHealthPlanDictionaries ReferencedHealthPlans;
        private CernerReferencedTerminologyDictionaries ReferenceTerminology;
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
        IMongoCollection<MongodbProcedureEntity> ProcedureCollection;

        public int NewPatientCounter;
        public int MergedPatientCounter;
        public int PersonCounter;
        public int NonMemberCounter;
        public int RollbackCounter;
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
        public int ProcedureCounter;

        /// <summary>
        /// Constructor, instantiates member objects and stores the connection string name (default if not provided by caller)
        /// </summary>
        /// <param name="PgConnectionName"></param>
        public BayClinicCernerAmbulatoryBatchAggregator(String PgConnectionName = null)
        {
            _PgConnectionName = String.IsNullOrEmpty(PgConnectionName) ? 
                                    PgConnectionStringName : 
                                    PgConnectionName;

            ReferencedCodes = new CernerReferencedCodeDictionaries();
            ReferencedHealthPlans = new CernerReferenceHealthPlanDictionaries();
            ReferenceTerminology = new CernerReferencedTerminologyDictionaries();

            Mutx = new Mutex();
        }

        /// <summary>
        /// Initializes member instance variables to support this aggregation run
        /// </summary>
        /// <returns>boolean indicating success</returns>
        private bool InitializeRun(bool ClearRunNumbers)
        {
            String EphiUserName = Environment.GetEnvironmentVariable("ephi_username");  // should be available if launched from Jenkins
            String EphiPassword = Environment.GetEnvironmentVariable("ephi_password");  // should be available if launched from Jenkins

            if (!Environment.ExpandEnvironmentVariables("<%ephi_username%><%ephi_password%><%Pg.Host%><%Pg.Db%>").Contains("<%"))
            {
                String PgHost = Environment.GetEnvironmentVariable("Pg.Host");  // should be available if launched from Jenkins
                String PgDb = Environment.GetEnvironmentVariable("Pg.Db");  // should be available if launched from Jenkins
                String PgPort = Environment.GetEnvironmentVariable("Pg.Port");  // optional
                String PgSchema = Environment.GetEnvironmentVariable("Pg.Schema");  // optional

                String PgConnectionString = "User Id=" + EphiUserName.ToLower() + 
                                           ";password=" + EphiPassword + 
                                           ";Host=" + PgHost + 
                                           ";Port=" + (String.IsNullOrEmpty(PgPort) ? "5432" : PgPort) +
                                           ";Database=" + PgDb +
                                           ";Initial Schema=" + (String.IsNullOrEmpty(PgSchema) ? "public" : PgSchema) ;
                //Trace.WriteLine("PostgreSQL connection string is: " + PgConnectionString);
                CdrDb = new CdrDbInterface(PgConnectionString, ConnectionArgumentType.ConnectionString);
            }
            else
            {
                // TODO validate _PgConnectionName
                CdrDb = new CdrDbInterface(_PgConnectionName, ConnectionArgumentType.ConnectionStringName);
            }
            try  // Validate the context connection
            {
                int RecordCount = CdrDb.Context.AggregationRuns.Count();
                Trace.WriteLine("PostgreSQL connection complete to database " + CdrDb.Context.Connection.Database + " on host " + CdrDb.Context.Connection.DataSource);
            }
            catch (Exception e)
            {
                Trace.WriteLine("PostgreSQL connection failed: " + e.Message);
            }

            MongoCxn = new MongoDbConnection();
            // Mongo.DbSuffix is optional
            if (!Environment.ExpandEnvironmentVariables("<%ephi_username%><%ephi_password%><%Mongo.UserDomain%><%Mongo.Host%>").Contains("<%"))
            {
                // all required environment exists
                if (MongoCxn.InitializeWithEnvironment(FeedIdentity))
                {
                    Trace.WriteLine("MongoCxn.InitializeWithEnvironment complete");
                }
                else
                {
                    Trace.WriteLine("MongoCxn.InitializeWithEnvironment failed");
                }
            }
            else
            {
                // TODO validate config file?
                if (MongoCxn.InitializeWithIni(NewBayClinicEmrMongoCredentialConfigFile, NewBayClinicEmrMongoCredentialSection))
                {
                    Trace.WriteLine("MongoCxn.InitializeWithIni complete");
                }
                else
                {
                    Trace.WriteLine("MongoCxn.InitializeWithIni failed");
                }
            }

            if (MongoCxn.GetCollectionNames().Count > 1)
            {
                Trace.WriteLine("MongoDB connection opened, collection(s) found");
            }
            else
            {
                Trace.WriteLine("MongoDB connection failed");
            }

            bool Initialized;

            //CsvWriter = new StreamWriter("Performance_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".csv");
            //CsvWriter.AutoFlush = true;
            WriteCounter = 0;

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
            ProcedureCollection = MongoCxn.Db.GetCollection<MongodbProcedureEntity>("procedure");

            // reset all counters to 0
            PersonCounter = NewPatientCounter = NonMemberCounter = MergedPatientCounter = RollbackCounter = PhoneCounter =
            AddressCounter = IdentifierCounter = ChargeCounter = ChargeDetailCounter = ResultCounter = DiagnosisCounter =
            VisitCounter = InsuranceCoverageCounter = ImmunizationCounter = MedicationCounter = ProblemCounter = ProcedureCounter =
                0;

            ThisAggregationRunDbid = GetNewAggregationRunDbid();
            Trace.WriteLine("Aggregating Bay Clinic Cerner data.  ThisAggregationRunDbid is " + ThisAggregationRunDbid);

            Initialized = ReferencedCodes.Initialize(RefCodeCollection);
            Initialized &= ReferencedHealthPlans.Initialize(ReferenceHealthPlanCollection, RefCodeCollection);
            Initialized &= ReferenceTerminology.Initialize(ReferenceTerminologyCollection);
            Initialized &= ThisAggregationRunDbid > 0;

            MongoRunUpdater = new MongoAggregationRunUpdater(ThisAggregationRunDbid, MongoCxn.Db);

            if (ClearRunNumbers)
            {
                // TODO This would truncate patients and all tables directly or indirectly referencing patients through foreign keys.  
                // CdrDb.Context.ExecuteCommand("TRUNCATE TABLE public.patient RESTART IDENTITY CASCADE");

                MongoRunUpdater.ClearAllMongoAggregationRunNumbers();
            }

            return Initialized;
        }

        /// <summary>
        /// Determines whether data for a patient, identified by a MongoDB person document, should be aggregated
        /// </summary>
        /// <param name="PersonDocument"></param>
        /// <returns></returns>
        public bool IsWOAHMember(MongodbPersonEntity PersonDocument)
        {
            // First just evaluate whether this patient was already aggregated before, if so then return true. 
            if (CdrDb.Context.Patients.Count(p => p.EmrIdentifier == PersonDocument.UniquePersonIdentifier) > 0)
            {
                //Trace.WriteLine("Found member " + PersonDocument.UniquePersonIdentifier + " in PostgreSQL");
                return true;
            }
            else
            {
                var InsuranceCoverageQuery = InsuranceCollection.AsQueryable()
                                                   .Where(x => x.EntityType == "PERSON"
                                                            && x.UniqueEntityIdentifier == PersonDocument.UniquePersonIdentifier);

                foreach (MongodbInsuranceEntity InsuranceDoc in InsuranceCoverageQuery)
                {
                    if (ReferencedCodes.InsuranceTypeCodeMeanings[InsuranceDoc.Type].ToUpper() == "MEDICAID")
                    {
                        //Trace.WriteLine("Found member " + PersonDocument.UniquePersonIdentifier + " in MongoDB");
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

            if (!InitializeRun(ClearRunNumbers))
            {
                Trace.WriteLine("Initialization failed, not proceeding");
                return false;
            }

            PatientLoopStart = DateTime.Now;

            var query = from Run in CdrDb.Context.AggregationRuns
                        where Run.dbid == ThisAggregationRunDbid
                        select Run;
            AggregationRun AggRun = query.FirstOrDefault();  // Returns existing record or null
            AggRun.StatusFlags = AggregationRunStatus.InProcess;
            CdrDb.Context.SubmitChanges();

            FilterDefinition<MongodbPersonEntity> PatientFilterDef = Builders<MongodbPersonEntity>.Filter.Where(x =>
                           x.UniquePersonIdentifier != ""  // has an identifier to be referenced from other txt files
                        && !(x.LastAggregationRun > 0)     // not previously aggregated
                      );

            FindOptions PersonFindOptions = new FindOptions { BatchSize = 5, NoCursorTimeout = true };
            using (var PersonCursor = PersonCollection.Find<MongodbPersonEntity>(PatientFilterDef, PersonFindOptions)
                                                      .SortBy(x => x.ImportFileDate)
                                                      .ToCursor())
            {
                while (PersonCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    // generate a new instance of the context wrapper
                    CdrDb = null;
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                    Thread.Sleep(1);  // not sure if this is really required (or good), but sometimes the CdrDbInterface constructor runs before the destructor
                    CdrDb = new CdrDbInterface(_PgConnectionName, ConnectionArgumentType.ConnectionStringName);

                    foreach (MongodbPersonEntity PersonDocument in PersonCursor.Current)
                    {
                        // TODO The following is temporary to aggregate all patients and skip filtering out non-members.  
                        bool IsMember = true;   // bool IsMember = IsWOAHMember(PersonDocument);

                        if (IsMember)
                        {
                            DateTime AggregateStart = DateTime.Now;
                            bool ThisPatientAggregationResult = AggregateOnePatient(PersonDocument);
                            TimeSpan AggregateTime = DateTime.Now - AggregateStart;

                            OverallResult &= ThisPatientAggregationResult;
                        }
                        else
                        {
                            NonMemberCounter++;
                        }

                        PersonCounter++;
                        if (PersonCounter % 1000 == 0)
                        {
                            Trace.Write(".");
                            if (PersonCounter % 50000 == 0)
                            {
                                Trace.WriteLine("Evaluated " + String.Format("{0,8:#}", PersonCounter) + " person documents for eligibility");
                            }
                        }

                        if (EndThreadSignal)
                        {
                            goto EndProcessing;
                        }
                    }
                }

            }

EndProcessing:
            query = from Run in CdrDb.Context.AggregationRuns
                    where Run.dbid == ThisAggregationRunDbid
                    select Run;
            AggRun = query.FirstOrDefault();  // Returns existing record or null
            AggRun.StatusFlags = AggregationRunStatus.Complete;
            CdrDb.Context.SubmitChanges();

            CdrDb = null;

            Trace.WriteLine("Processed   " + PersonCounter + " person documents from MongoDB");
            Trace.WriteLine("Inserted    " + NewPatientCounter + " new patient records");
            Trace.WriteLine("Merged      " + MergedPatientCounter + " existing patient records");
            Trace.WriteLine("Non-members " + NonMemberCounter + " non-member documents evaluated");
            Trace.WriteLine("Rolled back " + RollbackCounter + " patient transactions");
            Trace.WriteLine("Aggregation process completed at " + DateTime.Now);
            return OverallResult;
        }

        /// <summary>
        /// Aggregate all data related to one patient document from Mongo and manage the PostgreSQL storage transaction
        /// </summary>
        /// <param name="PersonDocument"></param>
        /// <returns></returns>
        private bool AggregateOnePatient(MongodbPersonEntity PersonDocument)
        {
            bool OverallSuccess = true;

            // Figure out whether this patient is already in the database
            var query = from Pat in CdrDb.Context.Patients
                        where Pat.EmrIdentifier == PersonDocument.UniquePersonIdentifier
                        select Pat;
            Patient PatientRecord = query.FirstOrDefault();  // Returns existing record or null

            #region PostgreSQL transaction to process all data for one patient
            //CdrDbInterface Db = new CdrDbInterface(_PgConnectionName, ConnectionArgumentType.ConnectionStringName);

            CdrDb.Context.Connection.Open();
            CdrDb.Context.Transaction = CdrDb.Context.Connection.BeginTransaction();

            // Merge fields with existing record or create new object in the case that no existing record was found
            bool Merged = PersonDocument.MergeWithExistingPatient(ref PatientRecord, ReferencedCodes);

            // Aggregate entities that are linked to this patient (entities linked to patient and visit are called from the visit aggregation method)
            OverallSuccess &= AggregateTelephoneNumbers(PersonDocument, PatientRecord);
            OverallSuccess &= AggregateAddresses(PersonDocument, PatientRecord);
            OverallSuccess &= AggregateIdentifiers(PersonDocument, PatientRecord);
            OverallSuccess &= AggregateVisits(PersonDocument, PatientRecord);
            OverallSuccess &= AggregateInsuranceCoverages(PersonDocument, PatientRecord);
            OverallSuccess &= AggregateProblems(PersonDocument, PatientRecord);  // Bay Clinic does not have links between problem and a visit

            if (Merged)
            {
                MergedPatientCounter++;
            }
            else
            {
                CdrDb.Context.Patients.InsertOnSubmit(PatientRecord);
                NewPatientCounter++;
            }

            DateTime StoreStart = DateTime.Now;
            CdrDb.Context.SubmitChanges();
            DateTime StoreEnd = DateTime.Now;
            //CsvWriter.WriteLine((WriteCounter++).ToString() + " , " + (StoreEnd - PatientLoopStart).TotalSeconds + " , " + (StoreEnd - StoreStart).TotalSeconds);

            MongoRunUpdater.PersonIdList.Add(PersonDocument.Id);

            // TODO maybe do a quality check on PatientRecord before proceeding to aggregate the referencing entities.  

            if (OverallSuccess)
            {
                CdrDb.Context.Transaction.Commit();
                MongoRunUpdater.UpdateAll();
            }
            else
            {
                RollbackCounter++;
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
                                                            where PhoneDoc.RecordIdentifier == Phone.EmrIdentifier
                                                            select Phone;

                        TelephoneNumber NewPgRecord = DuplicateTelephoneNumberQuery.FirstOrDefault();

                        bool Merged = PhoneDoc.MergeWithExistingTelephoneNumber(ref NewPgRecord, ref PgPatient, ReferencedCodes);

                        if (!Merged)
                        {
                            PhoneCounter++;
                            PgPatient.TelephoneNumbers.Add(NewPgRecord);
                        }

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

                        var DuplicateAddressrQuery = from Address in PgPatient.PhysicalAddresses
                                                     where Address.EmrIdentifier == AddressDoc.RecordIdentifier
                                                     select Address;

                        PhysicalAddress NewPgRecord = DuplicateAddressrQuery.FirstOrDefault();

                        bool Merged = AddressDoc.MergeWithExistingPhysicalAddress(ref NewPgRecord, ref PgPatient, ReferencedCodes);
                        if (!Merged)
                        {
                            AddressCounter++;
                            PgPatient.PhysicalAddresses.Add(NewPgRecord);
                        }

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
                                                              where IdentifierDoc.RecordIdentifier == Identifier.EmrIdentifier
                                                              select Identifier;

                        PatientIdentifier NewPgRecord = DuplicatePatientIdentifierQuery.FirstOrDefault();

                        bool Merged = IdentifierDoc.MergeWithExistingPatientIdentifier(ref NewPgRecord, ref PgPatient, ReferencedCodes, OrganizationDbid);

                        if (!Merged)
                        {
                            IdentifierCounter++;
                            PgPatient.PatientIdentifiers.Add(NewPgRecord);
                        }

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

            FindOptions VisitFindOptions = new FindOptions { BatchSize = 5, NoCursorTimeout = true };
            using (var VisitCursor = VisitCollection.Find<MongodbVisitEntity>(VisitFilterDef, VisitFindOptions).ToCursor())
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

                        bool Merged = VisitDoc.MergeWithExistingVisit(ref NewPgRecord, ref PatientRecord, ReferencedCodes, ref CdrDb);

                        // Aggregate entities that are linked to this visit
                        OverallSuccess &= AggregateCharges(VisitDoc, NewPgRecord);
                        OverallSuccess &= AggregateResults(PersonDoc, PatientRecord, VisitDoc, NewPgRecord);
                        OverallSuccess &= AggregateDiagnosis(PersonDoc, PatientRecord, VisitDoc, NewPgRecord);
                        OverallSuccess &= AggregateImmunizations(PersonDoc, PatientRecord, VisitDoc, NewPgRecord);
                        OverallSuccess &= AggregateMedications(PersonDoc, PatientRecord, VisitDoc, NewPgRecord);

                        OverallSuccess &= AggregateProcedures(PersonDoc, PatientRecord, VisitDoc, NewPgRecord);

                        if (!Merged)
                        {
                            VisitCounter++;
                            PatientRecord.VisitEncounters.Add(NewPgRecord);
                        }

                        MongoRunUpdater.VisitIdList.Add(VisitDoc.Id);

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

                        bool Merged = ChargeDoc.MergeWithExistingCharge(ref NewPgRecord, ref PgVisit);
                        // A legitimate NewPgRecord now exists

                        AggregateChargeDetails(ChargeDoc, NewPgRecord);  // Choose this one or the one below

                        if (!Merged)
                        {
                            ChargeCounter++;
                            PgVisit.Charges.Add(NewPgRecord);
                        }

                        MongoRunUpdater.ChargeIdList.Add(ChargeDoc.Id);
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
                                                         where ChargeDetail.EmrIdentifier == ChargeDetailDoc.UniqueChargeItemIdentifier  // Maybe this line is redundant with ChargeRecord.ChargeCodes
                                                         select ChargeDetail;
                        ChargeCode NewPgRecord = DuplicateChargeDetailQuery.FirstOrDefault();

                        bool Merged = ChargeDetailDoc.MergeWithExistingChargeCode(ref NewPgRecord, ref ChargeRecord, ReferencedCodes);

                        if (!Merged)
                        {
                            ChargeDetailCounter++;
                            ChargeRecord.ChargeCodes.Add(NewPgRecord);
                        }

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

                        bool Merged = ResultDoc.MergeWithExistingMeasurement(ref NewPgRecord, ref PatientRecord, VisitRecord, ReferencedCodes);

                        if (!Merged)
                        {
                            ResultCounter++;
                            VisitRecord.Results.Add(NewPgRecord);
                            PatientRecord.Measurements.Add(NewPgRecord);
                        }

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

                        bool Merged = DiagnosisDoc.MergeWithExistingDiagnosis(ref NewPgRecord, ref PatientRecord, VisitRecord, ReferencedCodes, ReferenceTerminology);
                        if (!Merged)
                        {
                            DiagnosisCounter++;
                            VisitRecord.Diagnoses.Add(NewPgRecord);
                            PatientRecord.Diagnoses.Add(NewPgRecord);
                        }

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
                                                              where Insurance.EmrIdentifier == InsuranceCoverageDoc.RecordIdentifier
                                                              select Insurance;
                        InsuranceCoverage NewPgRecord = DuplicateInsuranceCoverageQuery.FirstOrDefault();

                        bool Merged = InsuranceCoverageDoc.MergeWithExistingInsuranceCoverage(ref NewPgRecord, ref PgPatient, ReferencedCodes, ReferencedHealthPlans);

                        if (!Merged)
                        {
                            InsuranceCoverageCounter++;
                            PgPatient.InsuranceCoverages.Add(NewPgRecord);
                        }

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

                        bool Merged = ImmunizationDoc.MergeWithExistingImmunization(ref NewPgRecord, ref PatientRecord, VisitRecord, ReferencedCodes);

                        if (!Merged)
                        {
                            ImmunizationCounter++;
                            PatientRecord.Immunizations.Add(NewPgRecord);
                            VisitRecord.Immunizations.Add(NewPgRecord);
                        }

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

                        String MedicationInstructions = GetMedicationInstructions(MedicationDoc);

                        MongodbReferenceMedicationEntity ReferenceMedicationRecord = GetReferenceMedicationRecord(MedicationDoc);

                        bool Merged = MedicationDoc.MergeWithExistingMedication(ref NewPgRecord, ref PatientRecord, VisitRecord, ReferenceMedicationRecord, MedicationInstructions);

                        if (!Merged)
                        {
                            MedicationCounter++;
                            PatientRecord.Medications.Add(NewPgRecord);
                            VisitRecord.Medications.Add(NewPgRecord);
                        }

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

                        bool Merged = ProblemDoc.MergeWithExistingProblem(ref NewPgRecord, ref PatientRecord);
                        if (!Merged)
                        {
                            ProblemCounter++;
                            PatientRecord.Problems.Add(NewPgRecord);
                            //VisitRecord.Problems.Add(NewPgRecord);  // not for Cerner
                        }

                        MongoRunUpdater.ProblemIdList.Add(ProblemDoc.Id);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Aggregate the procedure data associated with a specified patient
        /// </summary>
        /// <param name="PersonDoc"></param>
        /// <param name="PatientRecord"></param>
        /// <param name="VisitDoc"></param>
        /// <param name="VisitRecord"></param>
        /// <returns></returns>
        private bool AggregateProcedures(MongodbPersonEntity PersonDoc, Patient PatientRecord, MongodbVisitEntity VisitDoc, VisitEncounter VisitRecord)
        {
            FilterDefinition<MongodbProcedureEntity> ProcedureFilterDef = Builders<MongodbProcedureEntity>.Filter
                .Where(x =>
                        x.UniqueVisitIdentifier == VisitDoc.UniqueVisitIdentifier
                    && !(x.LastAggregationRun > 0)     // not previously aggregated
                    && x.ImportFileDate == GetImportDateFromFileName(PersonDoc.ImportFile)
                      //&& (x.ImportFile.StartsWith(PersonDoc.ImportFile.Substring(0, 12)))  // match the extract date from PersonDoc.ImportFile 
                      );

            using (var ProcedureCursor = ProcedureCollection.Find<MongodbProcedureEntity>(ProcedureFilterDef).ToCursor())
            {
                while (ProcedureCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbProcedureEntity ProcedureDoc in ProcedureCursor.Current)
                    {
                        var DuplicateProcedureQuery = from Procedure in PatientRecord.Procedures
                                                    where Procedure.EmrIdentifier == ProcedureDoc.UniqueProcedureIdentifier
                                                    select Procedure;
                        Procedure NewPgRecord = DuplicateProcedureQuery.FirstOrDefault();

                        bool Merged = ProcedureDoc.MergeWithExistingProcedure(ref NewPgRecord, ref PatientRecord, VisitRecord, ReferenceTerminology);

                        if (!Merged)
                        {
                            ProcedureCounter++;
                            PatientRecord.Procedures.Add(NewPgRecord);
                            VisitRecord.Procedures.Add(NewPgRecord);
                        }

                        MongoRunUpdater.ProcedureIdList.Add(ProcedureDoc.Id);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MedicationDoc"></param>
        /// <returns></returns>
        private MongodbReferenceMedicationEntity GetReferenceMedicationRecord(MongodbMedicationEntity MedicationDoc)
        {
            MongodbReferenceMedicationEntity ReferenceMedicationRecord;
            //Get rxnorm codes
            var ReferenceMedicationQuery = ReferenceMedicationCollection.AsQueryable()
                                                       .Where(x => x.UniqueSynonymIdentifier == MedicationDoc.UniqueSynonymIdentifier);

            ReferenceMedicationRecord = ReferenceMedicationQuery.FirstOrDefault();

            if (ReferenceMedicationRecord == null)
            {
                ReferenceMedicationRecord = new MongodbReferenceMedicationEntity { RxNorm = "", CatalogCKI = "", Dnum = "", NDC = "" };
            }

            return ReferenceMedicationRecord;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MedicationDoc"></param>
        /// <returns></returns>
        private String GetMedicationInstructions(MongodbMedicationEntity MedicationDoc)
        {
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
            return MedicationInstructions;
        }


        /// <summary>
        /// Convert an extract file name to a chronologically sortable date of extract (yyyymmdd)
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private String GetImportDateFromFileName(String FileName)
        {
            return FileName.Substring(8, 4) + FileName.Substring(4, 2) + FileName.Substring(6, 2);
        }

        /// <summary>
        /// Establishes and stores a new aggregation run record for the current run
        /// </summary>
        /// <returns>The database key value of the new record</returns>
        private long GetNewAggregationRunDbid()
        {
            Organization Org = CdrDb.EnsureOrganizationRecord(WoahBayClinicOrganizationIdentity);
            OrganizationDbid = Org.dbid;  // Assignment of member variable.
            DataFeed FeedObj = CdrDb.EnsureFeedRecord(FeedIdentity, Org);

            AggregationRun NewRun = new AggregationRun
            {
                DataFeeddbid = FeedObj.dbid,
                StatusFlags = AggregationRunStatus.RunNumberReserved,
                StartDateTime = DateTime.Now
            };

            CdrDb.Context.AggregationRuns.InsertOnSubmit(NewRun);
            try
            {
                CdrDb.Context.SubmitChanges();
            }
            catch (Exception e)
            {
                string Msg = e.Message;
                Trace.WriteLine("Exception caught in GetNewAggregationRun, message:\n" + Msg + "\nCall Stack:\n" + e.StackTrace);
            }

            return NewRun.dbid;
        }

    }
}

