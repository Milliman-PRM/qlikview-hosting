using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;

namespace NbmcUnityQueryLib
{
    public class PatientExplorer : UnitySessionBase
    {
        DateTime QueryStartDT = new DateTime(2015, 1, 1);
        string UnityEndpoint;
        string UnityUsername;
        string UnityPassword;
        string UnityAppName;
        string EhrUsername;

        public PatientExplorer()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;

            // read settings from app.config; change there to match your server and credentials
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            UnityEndpoint = appSettings["unity.endpoint"]; // URL for Unity SOAP messages
            UnityUsername = appSettings["svc.username"]; // Unity service username and password for security token
            UnityPassword = appSettings["svc.password"];
            UnityAppName = appSettings["appname"]; // application name from Allscripts
            EhrUsername = appSettings["ehr.username"]; // valid EHR login name 
        }

        public void ExplorePatientEmrId(string PatientEmrId, bool DoCharges, bool DoDiagnoses, bool DoProblems, StreamWriter CsvWriter = null, String PatientMrn = "", String WoahId = "")
        {
            Connect(UnityUsername, UnityPassword, UnityEndpoint);

            if (DoProblems)
            {
                DataSet ProblemDs = UnityClient.Magic("GetPatientProblems", EhrUsername, UnityAppName, PatientEmrId, UnitySecurityToken, "N", "ALL", "", "", "Y", "", null);
                Trace.WriteLine("Problem output is:\n" + ProblemDs.GetXml());
                Trace.Write("Allscripts problems IDs are: ");
                foreach (DataTable ProblemTable in ProblemDs.Tables)
                {
                    // There is a table returned with name "getpatientproblemsinfo1", which contains only errors
                    if (ProblemTable.TableName == "getpatientproblemsinfo")
                    {
                        foreach (DataRow ProblemRow in ProblemTable.Rows)  // for each multi-field record
                        {
                            String ProblemId = ProblemRow["ProblemID"].ToString();
                            //Trace.Write(ProblemRow["ProblemID"].ToString() + " , ");
                            Trace.Write("ProblemID to evaluate is : " + ProblemId);
                            DataSet ProblemDetailsDs = UnityClient.Magic("GetProblemDetails", EhrUsername, UnityAppName, PatientEmrId, UnitySecurityToken, ProblemId, "", "", "", "", "", null);
                            Trace.Write("GetProblemDetails returned xml:" + ProblemDetailsDs.GetXml());
                        }
                    }
                }
            }

            if (DoDiagnoses)
            {
                if (CsvWriter == null)
                {
                    string OutputPath = @".\Output";
                    String CsvFileName = "Diagnoses_EmrId_" + PatientEmrId + ".csv";
                    CsvFileName = Path.Combine(OutputPath, CsvFileName);
                    Directory.CreateDirectory(OutputPath);
                    CsvWriter = new StreamWriter(CsvFileName);
                    CsvWriter.AutoFlush = true;

                    CsvWriter.WriteLine("Patient_EmrId|" +
                                        "Patient_Mrn|" +
                                        "Patient_WoahId|" +
                                        "Encounter_EmrId|" +
                                        "Encounter_DateTime|" +
                                        "Encounter_PerformingProviderName|" +
                                        "Encounter_BillingProviderName|" +
                                        "Encounter_appointmenttype|" +
                                        "Encounter_ApptComment|" +
                                        "Diagnosis_ProblemDE|" +
                                        "Diagnosis_code|" +
                                        "Diagnosis_Diagnosis|" +
                                        "Diagnosis_ICD10code|" +
                                        "Diagnosis_ICD10diagnosis");
                }
            }

            Trace.WriteLine("\nCalling GetEncounterList with patient EHR ID: " + PatientEmrId);
            DataSet EncounterDs = UnityClient.Magic("GetEncounterList", EhrUsername, UnityAppName, PatientEmrId, UnitySecurityToken, "", "", "", "Y", "", "", null);
            Trace.WriteLine("Output from GetEncounterList:\n" + EncounterDs.GetXml());

            foreach (DataTable EncounterTable in EncounterDs.Tables)  // one table in a response
            {
                Trace.WriteLine("Encounter table named " + EncounterTable.TableName + " has " + EncounterTable.Rows.Count + " rows");
                foreach (DataRow EncounterRow in EncounterTable.Rows)  // for each multi-field record
                {
                    string EncounterId = EncounterRow["ID"].ToString();
                    Trace.WriteLine("\nFound encounter with EncounterID = " + EncounterId);
                    int TempInt;
                    if (!int.TryParse(EncounterId, out TempInt) || TempInt <= 0)
                    {
                        Trace.WriteLine("EncounterId not parsable into valid integer, skipping this encounter: " + EncounterId);
                        continue;
                    }

                    if (DoCharges)
                    {
                        HandleCharges(PatientEmrId, EncounterId, EncounterRow);
                    }

                    if (DoDiagnoses)
                    {
                        HandleDiagnoses(PatientEmrId, EncounterId, EncounterRow, CsvWriter, PatientMrn, WoahId);
                    }
                }
            }

        }

        public void ExplorePatient(string PatientMrn, bool DoCharges, bool DoDiagnoses, bool DoProblems, string WoahId)
        {
            String TraceFileName = "TraceLog_MRN_" + PatientMrn + ".txt";

            TraceListener ThisTraceListener = new TextWriterTraceListener(TraceFileName);
            Trace.Listeners.Add(ThisTraceListener);
            Trace.WriteLine("Launched " + DateTime.Now.ToString());

            Trace.WriteLine("Attempting to connect to unity service with endpoint " + UnityEndpoint);
            Connect(UnityUsername, UnityPassword, UnityEndpoint);

            // TODO until soon, catch UnityFault? 
            if (UnitySecurityToken.ToLower().StartsWith("error:"))
            {
                Trace.WriteLine("Error getting security token (" + UnitySecurityToken + "). Application will exit.");
                Environment.Exit(-1);
            }

            // GetServerInfo: Display general Unity server information
            DataSet dsServerInfo = UnityClient.Magic("GetServerInfo", EhrUsername, UnityAppName, "", UnitySecurityToken, "", "", "", "", "", "", null);
            Trace.WriteLine("\nOutput from GetServerInfo:\n" + dsServerInfo.GetXml());

            if (PatientMrn.Trim().Length > 0)
            {
                DataSet PatientDs;

                Trace.WriteLine("\nCalling GetPatientByMRN with MRN: " + PatientMrn);
                PatientDs = UnityClient.Magic("GetPatientByMRN", EhrUsername, UnityAppName, "", UnitySecurityToken, PatientMrn, "", "", "", "", "", null);
                Trace.WriteLine("\nOutput from GetPatientByMRN:\n" + PatientDs.GetXml( ));
                if (PatientDs.Tables.Count == 0)
                {
                    goto CleanUp;
                }

                string InitialPatientID = PatientDs.Tables[0].Rows[0]["ID"].ToString();

                DataSet PatientIdDs = UnityClient.Magic("GetPatientIDs", EhrUsername, UnityAppName, InitialPatientID, UnitySecurityToken, "", "", "", "", "", "", null);
                Trace.WriteLine("\nOutput from GetPatientIDs:\n" + PatientIdDs.GetXml());
                List<String> AllPatientIds = new List<string>();
                Trace.Write("Allscripts IDs for this patient are: ");
                foreach (DataTable PatientIdTable in PatientIdDs.Tables)
                {
                    foreach (DataRow PatientIdRow in PatientIdTable.Rows)  // for each multi-field record
                    {
                        AllPatientIds.Add(PatientIdRow["PatientIDs"].ToString());
                        Trace.Write(PatientIdRow["PatientIDs"].ToString() + " , ");
                    }
                }
                Trace.WriteLine("");

                // main loop for each patient id
                foreach (String PatientID in AllPatientIds)
                {
                    ExplorePatientEmrId(PatientID, DoCharges, DoDiagnoses, DoProblems, null, PatientMrn, WoahId);
                }
            }

CleanUp:
            // clean up
            ThisTraceListener.Flush();
            ThisTraceListener.Close();
            ThisTraceListener.Dispose();
            Trace.Listeners.Remove(ThisTraceListener);
        }

        void HandleCharges(String PatientID, String EncounterId, DataRow EncounterRow)
        {
            Trace.WriteLine("\nCalling GetCharges with patient EHR ID: " + PatientID + " and encounter EHR ID " + EncounterId);
            DataSet ChargeDs = UnityClient.Magic("GetCharges", EhrUsername, UnityAppName, PatientID, UnitySecurityToken, EncounterId, "", "", "", "", "", null);
            Trace.WriteLine("Output from GetCharges:\n" + ChargeDs.GetXml());

            foreach (DataTable ChargeTable in ChargeDs.Tables)
            {
                Trace.WriteLine("This ChargeTable has " + ChargeTable.Rows.Count + " charges");
                foreach (DataRow ChargeRow in ChargeTable.Rows)
                {
                    Trace.WriteLine("Starting another charge");
                    for (int i = 0; i < ChargeTable.Columns.Count; i++)
                    {
                        //Trace.WriteLine("Charge field name: " + ChargeTable.Columns[i].ColumnName + ", value: " + ChargeRow[i]);
                    }
                    String ChargeID = ChargeRow["ChargeID"].ToString();
                    String ChargeCode = ChargeRow["entrycode"].ToString();
                    Trace.WriteLine("For Encounter " + EncounterId + ", Charge ID: " + ChargeID + ", code: " + ChargeCode);
                }
            }
        }

        void HandleDiagnoses(String PatientEmrId, String EncounterEmrId, DataRow EncounterRow, StreamWriter CsvWriter, String PatientMrn = "", String WoahId = "")
        {
            DataSet DiagnosisDs;

            Trace.WriteLine("\nCalling GetPatientDiagnosis with patient EMR ID: " + PatientEmrId + " and encounter EMR ID " + EncounterEmrId);
            try
            {
                DiagnosisDs = UnityClient.Magic("GetPatientDiagnosis", EhrUsername, UnityAppName, PatientEmrId, UnitySecurityToken, "", "", "", EncounterEmrId, "", "", null);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception caught while calling GetPatientDiagnosis, " + e.Message);
                return;
            }
            Trace.WriteLine("Output from GetPatientDiagnosis:\n" + DiagnosisDs.GetXml());

            foreach (DataTable DiagnosisTable in DiagnosisDs.Tables)
            {
                Trace.WriteLine("This DiagnosisTable has " + DiagnosisTable.Rows.Count + " diagnoses");
                foreach (DataRow DiagnosisRow in DiagnosisTable.Rows)
                {
                    if (CsvWriter != null)
                    {
                        CsvWriter.WriteLine(PatientEmrId + "|" +
                                            PatientMrn + "|" +
                                            WoahId + "|" +
                                            EncounterEmrId + "|" +
                                            EncounterRow["DTTM"] + "|" +
                                            EncounterRow["PerformingProviderName"] + "|" +
                                            EncounterRow["BillingProviderName"] + "|" +
                                            EncounterRow["appointmenttype"] + "|" +
                                            EncounterRow["ApptComment"] + "|" +
                                            DiagnosisRow["ProblemDE"] + "|" +
                                            DiagnosisRow["Code"] + "|" +
                                            DiagnosisRow["Diagnosis"] + "|" +
                                            DiagnosisRow["ICD10Code"] + "|" +
                                            DiagnosisRow["ICD10Diagnosis"]);

                        /*
                        for (int i = 0; i < DiagnosisTable.Columns.Count; i++)
                        {
                            Trace.WriteLine("Diagnosis field name: " + DiagnosisTable.Columns[i].ColumnName + ", value: " + DiagnosisRow[i]);
                        }
                        Trace.WriteLine("");
                        */
                    }
                }  // foreach DiagnosisRow 
            }  // foreach DiagnosisTable
        }  // HandleDiagnoses

    }  // class
}  // namespace
