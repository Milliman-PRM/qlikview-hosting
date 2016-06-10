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

namespace BayClinicCernerAmbulatory
{
    class BayClinicCernerAmbulatoryBatchAggregator
    {
        public static readonly String WoahBayClinicOrganizationIdentity = "WOAH Bay Clinic";
        private const String FeedIdentity = "BayClinicCernerAmbulatoryExtract";

        // TODO Some of these should be arguments from the caller, or sourced another way? 
        private String PgConnectionStringName = ConfigurationManager.AppSettings["CdrPostgreSQLConnectionString"];
        private String NewBayClinicAmbulatoryMongoCredentialConfigFile = ConfigurationManager.AppSettings["NewBayClinicAmbulatoryMongoCredentialConfigFile"];
        private String NewBayClinicAmbulatoryMongoCredentialSection = ConfigurationManager.AppSettings["NewBayClinicAmbulatoryMongoCredentialSection"];

        private CdrDbInterface CdrDb;
        private Organization OrganizationObject;
        private DataFeed FeedObject;
        private AggregationRun ThisAggregationRun;
        private MongoDbConnection MongoCxn;
        private CernerReferencedCodeDictionaries ReferencedCodes;
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
        IMongoCollection<MongodbImmunizationEntity> ImmunizationCollection;

        public BayClinicCernerAmbulatoryBatchAggregator(String PgConnectionName = null)
        {
            if (String.IsNullOrEmpty(PgConnectionName))
            {
                PgConnectionName = PgConnectionStringName;
            }

            CdrDb = new CdrDbInterface(PgConnectionName, ConnectionArgumentType.ConnectionStringName);
            MongoCxn = new MongoDbConnection(NewBayClinicAmbulatoryMongoCredentialConfigFile, NewBayClinicAmbulatoryMongoCredentialSection);
            ReferencedCodes = new CernerReferencedCodeDictionaries();
        }

        private bool InitializeRun()
        {
            bool Initialized;

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
            ImmunizationCollection = MongoCxn.Db.GetCollection<MongodbImmunizationEntity>("immunization");

            Initialized = ReferencedCodes.Initialize(RefCodeCollection);
            ThisAggregationRun = GetNewAggregationRun();
            Initialized &= ThisAggregationRun.dbid > 0;

            MongoRunUpdater = new MongoAggregationRunUpdater(ThisAggregationRun.dbid, MongoCxn.Db);

            return Initialized;
        }

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
            
            FilterDefinition<MongodbPersonEntity> PatientFilterDef = Builders<MongodbPersonEntity>.Filter
                .Where(  // TODO get criteria right
                           x => x.LastName != ""           // must have a last name
                        && x.UniquePersonIdentifier != ""  // has an identifier to be referenced from other txt files
                        && !(x.LastAggregationRun > 0)     // not previously aggregated
                      );

            using (var PersonCursor = PersonCollection.Find<MongodbPersonEntity>(PatientFilterDef)/*.Project<MongodbPersonEntity>(Proj)*/.ToCursor())
            {
                while (PersonCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbPersonEntity PersonDocument in PersonCursor.Current)
                    {
                        PatientCounter++;

                        bool ThisPatientAggregationResult = AggregateOnePatient(PersonDocument);
                        OverallResult &= ThisPatientAggregationResult;
                    }
                }
            }

            ThisAggregationRun.StatusFlags = AggregationRunStatus.Complete;
            CdrDb.Context.SubmitChanges();

            Trace.WriteLine("Processed " + PatientCounter + " patients");
            return OverallResult;
        }

        private bool AggregateOnePatient(MongodbPersonEntity PersonDocument)
        {
            DateTime ParsedDateTime;
            bool OverallSuccess = true;

            Patient ThisPatient = new Patient();
            ThisPatient.NameLast = PersonDocument.LastName;
            ThisPatient.NameFirst = PersonDocument.FirstName;
            ThisPatient.NameMiddle = PersonDocument.MiddleName;
            DateTime.TryParse(PersonDocument.BirthDateTime, out ParsedDateTime);        // Will be DateTime.MinValue on parse failure
            ThisPatient.BirthDate = ParsedDateTime;
            ThisPatient.Gender = ReferencedCodes.GetCdrGenderEnum(PersonDocument.Gender);
            DateTime.TryParse(PersonDocument.DeceasedDateTime, out ParsedDateTime);        // Will be DateTime.MinValue on parse failure
            ThisPatient.DeathDate = ParsedDateTime;
            ThisPatient.Race = ReferencedCodes.RaceCodeMeanings[PersonDocument.Race];  // coded
            ThisPatient.Ethnicity = ReferencedCodes.EthnicityCodeMeanings[PersonDocument.Ethnicity];  // coded
            ThisPatient.MaritalStatus = ReferencedCodes.GetCdrMaritalStatusEnum(PersonDocument.MaritalStatus);  // coded

            // TODO maybe do a quality check on Patient before proceeding to aggregate the referencing entities.  

            #region PostgreSQL transaction to process all data for one patient
            CdrDb.Context.Connection.Open();
            CdrDb.Context.Transaction = CdrDb.Context.Connection.BeginTransaction();

            CdrDb.Context.Patients.InsertOnSubmit(ThisPatient);
            CdrDb.Context.SubmitChanges();  // TODO Is it possible that only one SubmitChanges call for the entire transaction is more efficient?  

            MongoRunUpdater.PersonIdList.Add(PersonDocument.Id);

            // Related entities
            OverallSuccess &= AggregateTelephoneNumbers(PersonDocument, ThisPatient);
            OverallSuccess &= AggregateAddresses(PersonDocument, ThisPatient);
            OverallSuccess &= AggregateIdentifiers(PersonDocument, ThisPatient);
            OverallSuccess &= AggregateVisits(PersonDocument, ThisPatient);
            OverallSuccess &= AggregateInsuranceCoverages(PersonDocument, ThisPatient);
            
            //            EntitySet<Medication> _Medications;
            //            EntitySet<Immunization> _Immunizations;
            //            EntitySet<Problem> _Problems;

            if (OverallSuccess)
            {
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
 
        private bool AggregateTelephoneNumbers(MongodbPersonEntity MongoPerson, Patient PgPatient)
        {
            int PhoneCounter = 0;

            FilterDefinition<MongodbPhoneEntity> PhoneFilterDef = Builders<MongodbPhoneEntity>.Filter
                .Where(
                         x => x.EntityType == "PERSON"
                      && x.EntityIdentifier == MongoPerson.UniquePersonIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      // TODO do we also want to match the extract date from MongoPerson.ImportFile?
                      );

            using (var PhoneCursor = PhoneCollection.Find<MongodbPhoneEntity>(PhoneFilterDef).ToCursor())
            {
                while (PhoneCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbPhoneEntity PhoneDoc in PhoneCursor.Current)
                    {
                        PhoneCounter++;
                        DateTime ActiveStatusDT;
                        DateTime.TryParse(PhoneDoc.ActiveStatusDateTime, out ActiveStatusDT);

                        TelephoneNumber NewPgRecord = new TelephoneNumber
                        {
                            Number = new PhoneNumber
                            {
                                Number = PhoneDoc.PhoneNumber,
                                PhoneType = ReferencedCodes.GetCdrPhoneTypeEnum(PhoneDoc.Type)
                            },
                            Patient = PgPatient,
                            DateFirstReported = ActiveStatusDT,
                            DateLastReported = ActiveStatusDT
                        };

                        CdrDb.Context.TelephoneNumbers.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.PhoneIdList.Add(PhoneDoc.Id);
                    }
                }
            }

            return true;
        }

        private bool AggregateAddresses(MongodbPersonEntity MongoPerson, Patient PgPatient)
        {
            int AddressCounter = 0;

            FilterDefinition<MongodbAddressEntity> AddressFilterDef = Builders<MongodbAddressEntity>.Filter
                .Where(
                         x => x.EntityType == "PERSON"
                      && x.EntityIdentifier == MongoPerson.UniquePersonIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      // TODO do we also want to match the extract date from MongoPerson.ImportFile?
                      );

            using (var AddressCursor = AddressCollection.Find<MongodbAddressEntity>(AddressFilterDef).ToCursor())
            {
                while (AddressCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbAddressEntity AddressDoc in AddressCursor.Current)
                    {
                        AddressCounter++;
                        DateTime ActiveStatusDT;
                        DateTime.TryParse(AddressDoc.ActiveStatusDateTime, out ActiveStatusDT);

                        PhysicalAddress NewPgRecord = new PhysicalAddress
                        {
                            Address = new Address
                            {
                                City = AddressDoc.CityText,
                                State = AddressDoc.StateText,
                                PostalCode = AddressDoc.ZipCode,
                                Country = AddressDoc.CountryText,
                                Line1 = AddressDoc.AddressLine1,
                                Line2 = AddressDoc.AddressLine2,
                                Line3 = AddressDoc.AddressLine3,
                                Line4 = AddressDoc.AddressLine4
                            },
                            AddressType = ReferencedCodes.GetCdrAddressTypeEnum(AddressDoc.Type),
                            patient = PgPatient,
                            DateFirstReported = ActiveStatusDT,
                            DateLastReported = ActiveStatusDT
                        };

                        CdrDb.Context.PhysicalAddresses.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.AddressIdList.Add(AddressDoc.Id);
                    }
                }
            }

            return true;
        }

        private bool AggregateIdentifiers(MongodbPersonEntity MongoPerson, Patient PgPatient)
        {
            int IdentifierCounter = 0;

            FilterDefinition<MongodbIdentifierEntity> IdentifierFilterDef = Builders<MongodbIdentifierEntity>.Filter
                .Where(
                         x => x.EntityType == "PERSON"
                      && x.EntityIdentifier == MongoPerson.UniquePersonIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      // TODO do we also want to match the extract date from MongoPerson.ImportFile?
                      );

            using (var IdentifierCursor = IdentifierCollection.Find<MongodbIdentifierEntity>(IdentifierFilterDef).ToCursor())
            {
                while (IdentifierCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbIdentifierEntity IdentifierDoc in IdentifierCursor.Current)
                    {
                        IdentifierCounter++;
                        DateTime ActiveStatusDT;
                        DateTime.TryParse(IdentifierDoc.ActiveStatusDateTime, out ActiveStatusDT);

                        PatientIdentifier NewPgRecord = new PatientIdentifier
                        {
                            Identifier = IdentifierDoc.Identifier,
                            IdentifierType = ReferencedCodes.IdentifierTypeCodeMeanings[IdentifierDoc.IdentifierType],
                            Organization = OrganizationObject,
                            Patient = PgPatient,
                            DateFirstReported = ActiveStatusDT,
                            DateLastReported = ActiveStatusDT
                        };

                        CdrDb.Context.PatientIdentifiers.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.IdentifiersIdList.Add(IdentifierDoc.Id);
                    }
                }
            }

            return true;
        }

        private bool AggregateVisits(MongodbPersonEntity PersonDoc, Patient PatientRecord)
        {
            bool OverallSuccess = true;
            int VisitCounter = 0;

            FilterDefinition<MongodbVisitEntity> VisitFilterDef = Builders<MongodbVisitEntity>.Filter
                .Where(
                         x => x.UniquePersonIdentifier == PersonDoc.UniquePersonIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      // TODO do we also want to match the extract date from MongoPerson.ImportFile?
                      );

            using (var VisitCursor = VisitCollection.Find<MongodbVisitEntity>(VisitFilterDef).ToCursor())
            {
                while (VisitCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbVisitEntity VisitDoc in VisitCursor.Current)
                    {
                        DateTime BeginDateTime, EndDateTime, ActiveStatusDT;
                        DateTime.TryParse(VisitDoc.EffectiveBeginDateTime, out BeginDateTime);        // Will be DateTime.MinValue on parse failure
                        DateTime.TryParse(VisitDoc.EffectiveEndDateTime, out EndDateTime);        // Will be DateTime.MinValue on parse failure
                        DateTime.TryParse(VisitDoc.ActiveStatusDateTime, out ActiveStatusDT);        // Will be DateTime.MinValue on parse failure

                        VisitCounter++;

                        VisitEncounter NewPgRecord = new VisitEncounter
                        {
                            EmrIdentifier = VisitDoc.UniqueVisitIdentifier,
                            BeginDateTime = BeginDateTime,  // TODO Is this right?
                            EndDateTime = EndDateTime,  // TODO Is this right?
                            Status = VisitDoc.Active,
                            StatusDateTime = ActiveStatusDT,  // TODO Is this right?
                            Organization = ReferencedCodes.GetOrganizationEntityForVisitLocationCode(VisitDoc.LocationCode, ref CdrDb),
                            Patient = PatientRecord
                        };

                        CdrDb.Context.VisitEncounters.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.VisitIdList.Add(VisitDoc.Id);

                        OverallSuccess &= AggregateCharges(VisitDoc, NewPgRecord);
                        OverallSuccess &= AggregateResults(PersonDoc, PatientRecord, VisitDoc, NewPgRecord);
                        OverallSuccess &= AggregateDiagnoses(PersonDoc, PatientRecord, VisitDoc, NewPgRecord);
                        OverallSuccess &= AggregateImmunizations(PersonDoc, PatientRecord, VisitDoc, NewPgRecord);

                    }
                }
            }

            return OverallSuccess;
        }

        private bool AggregateCharges(MongodbVisitEntity MongoVisit, VisitEncounter PgVisit)
        {
            int ChargeCounter = 0;

            FilterDefinition<MongodbChargeEntity> ChargeFilterDef = Builders<MongodbChargeEntity>.Filter
                .Where(
                         x => x.UniqueVisitIdentifier == MongoVisit.UniqueVisitIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                      // TODO do we also want to match the extract date from MongoPerson.ImportFile?
                      );

            using (var ChargeCursor = ChargeCollection.Find<MongodbChargeEntity>(ChargeFilterDef).ToCursor())
            {
                while (ChargeCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbChargeEntity ChargeDoc in ChargeCursor.Current)
                    {
                        ChargeCounter++;
                        DateTime ActiveStatusDT, ServiceDT, PostDT, UpdateDT;
                        DateTime.TryParse(ChargeDoc.ActiveStatusDateTime, out ActiveStatusDT);
                        DateTime.TryParse(ChargeDoc.ServiceDateTime, out ServiceDT);
                        DateTime.TryParse(ChargeDoc.PostedDateTime, out PostDT);
                        DateTime.TryParse(ChargeDoc.UpdateDateTime, out UpdateDT);

                        String DescriptionFirstWord = ChargeDoc.Description.Split(' ').FirstOrDefault();

                        // TODO may need to think about value of ChargeDoc.Type, some could be "no charge"
                        // TODO may need to think about value of ChargeDoc.State, some could be "combined away" or "suspended"
                        Charge NewPgRecord = new Charge
                        {
                            //DentalDetails = new DentalDetail {ToothNumber=0 ,ToothSurfaceCode="" },
                            EmrIdentifier = ChargeDoc.UniqueChargeIdentifier,
                            DateOfService = ServiceDT,
                            Description = ChargeDoc.Description,  // If the ChargeDetail codes are not adequate, a cpt appears to be prepended to this field in raw data
                            Comment = "",
                            SubmittedDate = PostDT,
                            Submitter = "",   // TODO What to do with this?
                            State = ChargeDoc.State,  // TODO This is a coded reference, get the meaning string
                            DateInfoLastUpdated = UpdateDT,
                            VisitEncounter = PgVisit
                            // Think about whether the ordering_physician_identifier or verifying_physician_identifier would be useful to add to the model
                            // TODO Should we collect the type field?
                        };
                        NewPgRecord.ChargeCodes.Add(new ChargeCode { Code = new CodedEntry { Code = DescriptionFirstWord,
                                                                                             CodeSystem = "Charge Description Prepend", }
                                                                   });
                        // ChargeDoc.UniqueChargeItemIdentifier is the reference from related ChargeDetail documents
                        // What is parent_charge_identifier?
                        // What is offset_charge_identifier?

                        CdrDb.Context.Charges.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.ChargeIdList.Add(ChargeDoc.Id);

                        AggregateChargeDetails(ChargeDoc, NewPgRecord);
                    }
                }
            }

            return true;
        }

        private bool AggregateChargeDetails(MongodbChargeEntity ChargeDoc, Charge ChargeRecord)
        {
            int ChargeDetailCounter = 0;

            FilterDefinition<MongodbChargeDetailEntity> ChargeDetailFilterDef = Builders<MongodbChargeDetailEntity>.Filter
                .Where(
                         x => x.UniqueChargeItemIdentifier == ChargeDoc.UniqueChargeItemIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                                                         // TODO do we also want to match the extract date from MongoPerson.ImportFile?
                      );

            using (var ChargeDetailCursor = ChargeDetailCollection.Find<MongodbChargeDetailEntity>(ChargeDetailFilterDef).ToCursor())
            {
                while (ChargeDetailCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbChargeDetailEntity ChargeDetailDoc in ChargeDetailCursor.Current)
                    {
                        ChargeDetailCounter++;

                        ChargeCode NewPgRecord = new ChargeCode
                        {
                            Charge = ChargeRecord,
                            Code = new CodedEntry
                            {
                                Code = ChargeDetailDoc.Code,
                                CodeSystem = ReferencedCodes.ChargeDetailTypeCodeMeanings[ChargeDetailDoc.Type],
                            }
                        };

                        CdrDb.Context.ChargeCodes.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.ChargeDetailIdList.Add(ChargeDetailDoc.Id);
                    }
                }
            }

            return true;
        }

        private bool AggregateResults(MongodbPersonEntity PersonDoc, Patient PatientRecord, MongodbVisitEntity VisitDoc, VisitEncounter VisitRecord)
        {
            bool Success = true;
            int ResultCounter = 0;
            DateTime PerformedDateTime;

            FilterDefinition<MongodbResultEntity> ResultFilterDef = Builders<MongodbResultEntity>.Filter
                .Where(x => 
                       x.UniquePersonIdentifier == PersonDoc.UniquePersonIdentifier
                    && x.UniqueVisitIdentifier == VisitDoc.UniqueVisitIdentifier
                    && !(x.LastAggregationRun > 0)     // not previously aggregated
                      // TODO do we also want to match the extract date from MongoPerson.ImportFile?
                      );

            using (var ResultCursor = ResultCollection.Find<MongodbResultEntity>(ResultFilterDef).ToCursor())
            {
                while (ResultCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbResultEntity ResultDoc in ResultCursor.Current)
                    {
                        ResultCounter++;
                        DateTime.TryParse(ResultDoc.PerformedDateTime, out PerformedDateTime);

                        Measurement NewPgRecord = new Measurement
                        {
                            Patientdbid = PatientRecord.dbid,
                            VisitEncounterdbid = VisitRecord.dbid,
                            EmrIdentifier = ResultDoc.UniqueResultIdentifier,
                            Name = ReferencedCodes.ResultCodeCodeMeanings[ResultDoc.Code],
                            Description = ResultDoc.Title,  // TODO get this right
                            Comments = "",    // TODO get this right
                            MeasurementCode = new CodedEntry { },    // TODO get this right
                            AssessmentDateTime = PerformedDateTime,
                            Value = ResultDoc.ResultValue,
                            Units = ReferencedCodes.ResultUnitsCodeMeanings[ResultDoc.Units],
                            NormalRangeLow = ResultDoc.NormalLow,
                            NormalRangeHigh = ResultDoc.NormalHigh,
                            NormalType = ReferencedCodes.GetResultNormalCodeEnum(ResultDoc.NormalCode)
                        };

                        CdrDb.Context.Measurements.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.ResultIdList.Add(ResultDoc.Id);
                    }
                }
            }

            return Success;
        }

        private bool AggregateDiagnoses(MongodbPersonEntity PersonDoc, Patient PatientRecord, MongodbVisitEntity VisitDoc, VisitEncounter VisitRecord)
        {
            bool Success = true;
            int DiagnosisCounter = 0;

            FilterDefinition<MongodbDiagnosisEntity> DiagnosisFilterDef = Builders<MongodbDiagnosisEntity>.Filter
                .Where(x =>
                       x.UniquePersonIdentifier == PersonDoc.UniquePersonIdentifier
                    && x.UniqueVisitIdentifier == VisitDoc.UniqueVisitIdentifier
                    && !(x.LastAggregationRun > 0)     // not previously aggregated
                                                       // TODO do we also want to match the extract date from MongoPerson.ImportFile?
                      );

            using (var DiagnosisCursor = DiagnosisCollection.Find<MongodbDiagnosisEntity>(DiagnosisFilterDef).ToCursor())
            {
                while (DiagnosisCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbDiagnosisEntity ResultDoc in DiagnosisCursor.Current)
                    {
                        DiagnosisCounter++;

                        DateTime StartDateTime, EndDateTime, DiagDateTime, StatusDateTime;
                        DateTime.TryParse(ResultDoc.EffectiveBeginDateTime, out StartDateTime);
                        DateTime.TryParse(ResultDoc.EffectiveEndDateTime, out EndDateTime);
                        DateTime.TryParse(ResultDoc.DiagnosisDateTime, out DiagDateTime);
                        DateTime.TryParse(ResultDoc.ActiveStatusDateTime, out StatusDateTime);

                        Diagnosis NewPgRecord = new Diagnosis
                        {
                            Patientdbid = PatientRecord.dbid,
                            VisitEncounterdbid = VisitRecord.dbid,
                            EmrIdentifier = ResultDoc.UniqueDiagnosisIdentifier,
                            StartDateTime = StartDateTime,
                            EndDateTime = EndDateTime,
                            DeterminationDateTime = DiagDateTime,
                            ShortDescription = "",
                            LongDescription = "",
                            DiagCode = new CodedEntry { },
                            Status = "",
                            StatusDateTime = StatusDateTime
                            // TODO This block is not finished.  Get the member initializations right.  
                        };

                        CdrDb.Context.Diagnoses.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.DiagnosisIdList.Add(ResultDoc.Id);
                    }
                }
            }

            return Success;
        }


        private bool AggregateInsuranceCoverages(MongodbPersonEntity MongoPerson, Patient PgPatient)
        {
            int InsuranceCoverageCounter = 0;

            FilterDefinition<MongodbInsuranceEntity> InsuranceCoverageFilterDef = Builders<MongodbInsuranceEntity>.Filter
                .Where(
                         x => x.EntityType == "PERSON"
                      && x.UniqueEntityIdentifier == MongoPerson.UniquePersonIdentifier
                      && !(x.LastAggregationRun > 0)     // not previously aggregated
                                                         // TODO do we also want to match the extract date from MongoPerson.ImportFile?
                      );

            using (var InsuranceCoverageCursor = InsuranceCollection.Find<MongodbInsuranceEntity>(InsuranceCoverageFilterDef).ToCursor())
            {
                while (InsuranceCoverageCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbInsuranceEntity InsuranceCoverageDoc in InsuranceCoverageCursor.Current)
                    {
                        InsuranceCoverageCounter++;
                        DateTime StartDate, EndDate;
                        DateTime.TryParse(InsuranceCoverageDoc.EffectiveBeginDateTime, out StartDate);        // Will be DateTime.MinValue on parse failure
                        DateTime.TryParse(InsuranceCoverageDoc.EffectiveEndDateTime, out EndDate);        // Will be DateTime.MinValue on parse failure
                       

                        InsuranceCoverage NewPgRecord = new InsuranceCoverage
                        {
                            Payer = InsuranceCoverageDoc.UniqueOrganizationIdentifier,            
                            StartDate = StartDate,
                            EndDate = EndDate,
                            PlanName = InsuranceCoverageDoc.UniqueHealthPlanIdentifier,
                            Patient = PgPatient            //Just adding the patientdbid might may improve runtime
                        };

                        CdrDb.Context.InsuranceCoverages.InsertOnSubmit(NewPgRecord);
                        CdrDb.Context.SubmitChanges();

                        MongoRunUpdater.InsuranceIdList.Add(InsuranceCoverageDoc.Id);
                    }
                }
            }
            return true;
        }

        private bool AggregateImmunizations(MongodbPersonEntity PersonDoc, Patient PatientRecord, MongodbVisitEntity VisitDoc, VisitEncounter VisitRecord)
        {
            int ImmunizationCounter = 0;

            FilterDefinition<MongodbImmunizationEntity> ImmunizationFilterDef = Builders<MongodbImmunizationEntity>.Filter
                .Where(x =>
                       x.UniquePersonIdentifier == PersonDoc.UniquePersonIdentifier
                    && x.UniqueVisitIdentifier == VisitDoc.UniqueVisitIdentifier
                    && !(x.LastAggregationRun > 0)     // not previously aggregated
                                                       // TODO do we also want to match the extract date from MongoPerson.ImportFile?
                      );

            using (var ImmunizationCursor = ImmunizationCollection.Find<MongodbImmunizationEntity>(ImmunizationFilterDef).ToCursor())
            {
                while (ImmunizationCursor.MoveNext())  // transfer the next batch of available documents from the query result cursor
                {
                    foreach (MongodbImmunizationEntity ImmunizationDoc in ImmunizationCursor.Current)
                    {
                        ImmunizationCounter++;
                        DateTime PerformedDateTime;
                        DateTime.TryParse(ImmunizationDoc.PerformedDateTime, out PerformedDateTime);

                        Immunization NewPgRecord = new Immunization
                        {
                            Patientdbid = PatientRecord.dbid,
                            EmrIdentifier = ImmunizationDoc.UniqueOrderIdentifier,
                            Description = "",         // No description is found in the data
                            PerformedDateTime = PerformedDateTime,
                            ImmunizationCode = new CodedEntry
                            {
                                Code = ImmunizationDoc.Code,
                                CodeMeaning = ReferencedCodes.ImmunizationCodeMeanings[ImmunizationDoc.Code],
                            },
                            VisitEncounterdbid = VisitRecord.dbid
                        };
                    }
                }
            }

            return true;
        }


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
        }

    }
}
 