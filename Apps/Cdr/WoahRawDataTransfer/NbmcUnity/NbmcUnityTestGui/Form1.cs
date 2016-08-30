using System;
using System.Configuration;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOHSQLInterface;
using MongoDbWrap;
using System.IO;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using NbmcUnityQueryLib;

namespace NbmcUnityTestGui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ButtonBuildMrnList_Click(object sender, EventArgs e)
        {
            DateTime LaunchTimestamp = DateTime.Now;

            // database connections
            MongoDbConnection MongoDb = new MongoDbConnection();
            MongoDb.InitializeWithIni(@"H:\.prm_config\.mongodb", "MongoCredentials");
            IMongoCollection<BsonDocument> PatientCollection = MongoDb.Db.GetCollection<BsonDocument>("patient");

            /*
            MongoDbConnection RawDataDb = new MongoDbConnection();
            RawDataDb.InitializeWithIni(@"H:\.prm_config\.mongodb", "MongoNbmcRawDataCredentials");
            IMongoCollection<BsonDocument> RawDataDbPatientIndexCollection = RawDataDb.Db.GetCollection<BsonDocument>(TextPatIndexCollectionName.Text);

            // Delete all existing documents
            RawDataDbPatientIndexCollection.DeleteMany("{}");
            */

            // Instantiate the the interface to the SQLite membership dataset
            WOHSQLiteInterface WOHMembershipData = new WOHSQLiteInterface();

            DirectoryInfo SupportFilesFolder = new DirectoryInfo(@"\\indy-netapp\prm_phi\phi\0273WOH\3.005-0273WOH06\5-Support_files");
            DirectoryInfo LatestSubfolder = SupportFilesFolder.GetDirectories().OrderByDescending(f => f.Name).First();
            String WoahMembershipDataFile = Path.Combine(LatestSubfolder.FullName, @"035_Staging_Membership\Members_3.005-0273WOH06.sqlite");
            Trace.WriteLine("Using membership data file: " + WoahMembershipDataFile);

            //Connect to the SQLite membership database
            WOHMembershipData.ConnectToMembershipData(WoahMembershipDataFile);

            int WoahIdCounter = 0, EmrIdCounter = 0, MrnCounter = 0, MaxPerWoahIdMrnCounter = 0, MaxPerWoahIdEmrIdCounter = 0;

            String CsvFileName = "NBMCWoahMembers_" + LaunchTimestamp.ToString("yyyyMMdd-HHmmss") + ".csv";
            Trace.WriteLine("Output of identifier mapping will be written to file: " + CsvFileName);
            StreamWriter CsvWriter = new StreamWriter(CsvFileName);
            CsvWriter.AutoFlush = true;
            CsvWriter.WriteLine("Woah ID|NBMC MRN|Allscripts ID");

            foreach (String WoahId in WOHMembershipData.GetWoahIds())
            {
                WoahIdCounter++;
                HashSet<String> MrnSet = new HashSet<string>();
                HashSet<String> PIdSet = new HashSet<string>();
                int PerWoahIdMrnCounter = 0, PerWoahIdEmrIdCounter = 0;

                PipelineDefinition<BsonDocument, BsonDocument> pipeline = new BsonDocument[]
                {
                    //new BsonDocument { { "$match", new BsonDocument("identifiers.root", "2.16.124.113635.1.4.1.104") } },  // Has a WOAH ID
                    new BsonDocument { { "$match", new BsonDocument("identifiers.extension", WoahId) } },
                    new BsonDocument { { "$project", new BsonDocument { {"identifiers", 1}, {"names", 1}, { "birthDate", 1 } } } },
                    new BsonDocument { { "$unwind", new BsonDocument("path", "$identifiers") } },
                    new BsonDocument { { "$match", new BsonDocument("$or", new BsonArray {
                                                                                          { new BsonDocument("identifiers.root", "1.3.6.1.4.1.22812.3.7498501.3") }, // NBMC MRN
                                                                                          { new BsonDocument("identifiers.root", "2.16.124.113635.1.4.1.105") }  // Allscripts record ID
                                                                                         } ) } },
                };

                var results = PatientCollection.Aggregate(pipeline);

                // The pipeline will yield one match per NBMC MRN or Allscripts record ID
                foreach (BsonDocument OneResult in results.ToEnumerable())
                {
                    BsonDocument IdentifiersDoc = OneResult["identifiers"].AsBsonDocument;
                    //Trace.WriteLine("\n_id is " + OneResult["_id"].AsString);
                    string root = IdentifiersDoc["root"].AsString;
                    string ext = IdentifiersDoc["extension"].AsString;
                    //Trace.WriteLine(root + " -> " + ext);

                    switch (root)
                    {
                        case "1.3.6.1.4.1.22812.3.7498501.3":  // NBMC MRN
                            MrnCounter++;
                            PerWoahIdMrnCounter++;
                            MaxPerWoahIdMrnCounter = Math.Max(MaxPerWoahIdMrnCounter, PerWoahIdMrnCounter);
                            MrnSet.Add(ext);
                            break;
                        case "2.16.124.113635.1.4.1.105":  // Allscripts record ID
                            EmrIdCounter++;
                            PerWoahIdEmrIdCounter++;
                            MaxPerWoahIdEmrIdCounter = Math.Max(MaxPerWoahIdEmrIdCounter, PerWoahIdEmrIdCounter);
                            PIdSet.Add(ext);
                            break;
                        default:
                            // oopsie (probably impossible)
                            break;
                    }
                }

                /*
                BsonDocument NewDoc = new BsonDocument
                {
                    { "Woah ID", WoahId },
                    { "NBMC MRN", new BsonArray(NbmcMrns[WoahId].ToArray()) },
                    { "Allscripts ID", new BsonArray(NbmcPIds[WoahId].ToArray()) }
                };
                RawDataDbPatientIndexCollection.InsertOne(NewDoc);
                */

                CsvWriter.WriteLine(WoahId + "|" + String.Join(",", MrnSet.ToArray()) + "|" + String.Join(",", PIdSet.ToArray()));
                Trace.WriteLine("WOAH ID " + WoahId + " corresponds to MRNs: " + String.Join(",", MrnSet.ToArray()) + " and EMR IDs: " + String.Join(",", PIdSet.ToArray()));
            }

            CsvWriter.Close();

            Trace.WriteLine("WOAH ID  count: " + WoahIdCounter);
            Trace.WriteLine("NBMC MRN count: " + WoahIdCounter);
            Trace.WriteLine("EMR  ID  count: " + WoahIdCounter);
            Trace.WriteLine("MaxPerWoahIdEmrIdCounter: " + MaxPerWoahIdEmrIdCounter);
            Trace.WriteLine("MaxPerWoahIdMrnCounter: " + MaxPerWoahIdMrnCounter);
        }

        private void ButtonExtractDiagnoses_Click(object sender, EventArgs e)
        {
            DateTime LaunchTimestamp = DateTime.Now;
            String TraceFileName = "TraceLog_" + LaunchTimestamp.ToString("yyyyMMdd-HHmmss") + ".txt";

            TraceListener ThisTraceListener = new TextWriterTraceListener(TraceFileName);
            Trace.Listeners.Add(ThisTraceListener);
            Trace.WriteLine("Operation launched " + LaunchTimestamp.ToString("yyyyMMdd-HHmmss"));

            PatientExplorer PatExplorer = new PatientExplorer();

            #region Declare and initialize variables to limit the run duration
            int MrnCounter = 0;
            int MrnCountLimit;
            int.TryParse(ConfigurationManager.AppSettings["MrnCountLimit"], out MrnCountLimit);

            int[] OutInt = new int[4] { 0, 0, 0, 0 };
            String[] RunDurationLimitSettingStrings = ConfigurationManager.AppSettings["RunDurationLimit"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (RunDurationLimitSettingStrings.Count() == 4)
            {
                OutInt[0] = int.Parse(RunDurationLimitSettingStrings[0]);
                OutInt[1] = int.Parse(RunDurationLimitSettingStrings[1]);
                OutInt[2] = int.Parse(RunDurationLimitSettingStrings[2]);
                OutInt[3] = int.Parse(RunDurationLimitSettingStrings[3]);
            }
            TimeSpan RunDurationLimit = new TimeSpan(OutInt[0], OutInt[1], OutInt[2], OutInt[3]);
            #endregion

            openFileDialog1.InitialDirectory = ".";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                Trace.WriteLine("ID map .csv file not found, not processing");
                MessageBox.Show("ID map .csv file not found, not processing");
                return;
            }

            String OutputPath = @".\Output";
            String CsvFileName = "Diagnoses_" + LaunchTimestamp.ToString("yyyyMMdd-HHmmss") + ".csv";
            CsvFileName = Path.Combine(OutputPath, CsvFileName);
            Directory.CreateDirectory(OutputPath);
            StreamWriter CsvWriter = new StreamWriter(CsvFileName);
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

            using (StreamReader IdMapStream = new StreamReader(openFileDialog1.OpenFile()))
            {
                String[] FieldNames = IdMapStream.ReadLine().Split(new char[]{ '|'}, StringSplitOptions.RemoveEmptyEntries);

                while (!IdMapStream.EndOfStream)
                {
                    String[] Fields = IdMapStream.ReadLine().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (Fields.Count() != 3)
                    {
                        continue;
                    }
                    String WoahId = Fields[0];
                    String[] Mrns = Fields[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    String[] EmrIds = Fields[2].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (Mrns.Count() == 0 || EmrIds.Count() == 0)
                    {
                        continue;
                    }

                    if (Mrns.Count() != EmrIds.Count())
                    {
                        Trace.WriteLine("For WOAH ID " + WoahId + " Mrn count " + Mrns.Count() + " and EMRId count " + EmrIds.Count() + " are not the same");
                        continue;
                    }

                    for (int EmrIdCounter = 0; EmrIdCounter < EmrIds.Count(); EmrIdCounter++)
                    {
                        MrnCounter++;
                        Trace.WriteLine("Starting Unity operations on WOAH ID " + WoahId + ", Mrn " + Mrns[EmrIdCounter] + ", EmrId " + EmrIds[EmrIdCounter]);
                        PatExplorer.ExplorePatientEmrId(EmrIds[EmrIdCounter], false, true, false, CsvWriter, Mrns[EmrIdCounter], WoahId);
                    }

                    if (MrnCountLimit > 0 && MrnCounter >= MrnCountLimit)
                    {
                        Trace.WriteLine("Completed the configured limit of " + MrnCounter + " MRN operations, breaking");
                        break;
                    }
                    if (RunDurationLimit.TotalSeconds > 0.0 && (DateTime.Now-LaunchTimestamp) > RunDurationLimit)
                    {
                        Trace.WriteLine("Run duration reached the configured limit of " + RunDurationLimit.ToString() + ", breaking");
                        break;
                    }
                }
            }
            Trace.WriteLine("Total extracted PatientMrn counter is " + MrnCounter);

            CsvWriter.Close();

            Trace.WriteLine("Extract operation completed at " + DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            Trace.Listeners.Remove(ThisTraceListener);
        }

        private void ButtonExploreSystem_Click(object sender, EventArgs e)
        {
            DateTime LaunchTimestamp = DateTime.Now;
            String TraceFileName = "TraceLog_" + LaunchTimestamp.ToString("yyyyMMdd-HHmmss") + ".txt";
            TraceListener ThisTraceListener = new TextWriterTraceListener(TraceFileName);
            Trace.Listeners.Add(ThisTraceListener);
            Trace.WriteLine("Operation launched " + LaunchTimestamp.ToString("yyyyMMdd-HHmmss"));

            PatientExplorer PatExplorer = new PatientExplorer();

            PatExplorer.ExploreServer();

            Trace.WriteLine("ExploreSystem operation completed at " + DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            Trace.Listeners.Remove(ThisTraceListener);
        }
    }
}
