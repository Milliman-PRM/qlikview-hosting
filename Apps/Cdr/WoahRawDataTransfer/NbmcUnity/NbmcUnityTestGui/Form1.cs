using System;
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
            MongoDbConnection MongoDb = new MongoDbConnection();
            MongoDb.InitializeWithIni(@"H:\.prm_config\.mongodb", "MongoCredentials");
            IMongoCollection<BsonDocument> PatientCollection = MongoDb.Db.GetCollection<BsonDocument>("patient");
            Dictionary<String, HashSet<String>> NbmcMrns = new Dictionary<string, HashSet<string>>();

            // Instantiate the the interface to the SQLite membership dataset
            WOHSQLiteInterface WOHMembershipData = new WOHSQLiteInterface();

            /*  This block works well for authenticated access to network resources when running as SYSTEM user
             *  uses reference to project "NetworkAccess" in Apps/AppsCommon
             *  
            MembershipDataFileUsed = null;
            if (!Environment.ExpandEnvironmentVariables("<%ephi_username%><%ephi_password%>").Contains("<%"))
            {
                NetworkCredential KDriveCredentials = new NetworkCredential
                {
                    UserName = EphiUserName,
                    Password = EphiPassword,
                    Domain = "ROOT_MILLIMAN"
                };

                using (new NetworkConnection(@"\\indy-netapp\prm_phi", KDriveCredentials))
                {
                    DirectoryInfo SupportFilesFolder = new DirectoryInfo(@"\\indy-netapp\prm_phi\phi\0273WOH\3.005-0273WOH06\5-Support_files");
                    DirectoryInfo LatestSubfolder = SupportFilesFolder.GetDirectories().OrderByDescending(f => f.Name).First();
                    String WoahMembershipDataFile = Path.Combine(LatestSubfolder.FullName, @"035_Staging_Membership\Members_3.005-0273WOH06.sqlite");
                    File.Copy(WoahMembershipDataFile, @".\Members_3.005-0273WOH06.sqlite", true);
                    Trace.WriteLine("File.Copy returned at " + DateTime.Now);

                    MembershipDataFileUsed = Path.GetFullPath(@".\Members_3.005-0273WOH06.sqlite");
                }
            }
            */

            DirectoryInfo SupportFilesFolder = new DirectoryInfo(@"\\indy-netapp\prm_phi\phi\0273WOH\3.005-0273WOH06\5-Support_files");
            DirectoryInfo LatestSubfolder = SupportFilesFolder.GetDirectories().OrderByDescending(f => f.Name).First();
            String WoahMembershipDataFile = Path.Combine(LatestSubfolder.FullName, @"035_Staging_Membership\Members_3.005-0273WOH06.sqlite");

            Trace.WriteLine("Using membership data file: " + WoahMembershipDataFile);

            //Connect to the SQLite membership database
            WOHMembershipData.ConnectToMembershipData(WoahMembershipDataFile);

            int WoahIdCounter = 0, MaxPerIdMatchCounter = 0, MaxMrnCounter = 0;
            StreamWriter CsvWriter = new StreamWriter("NBMCWoahMembers_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".csv");
            CsvWriter.WriteLine("Woah ID|NBMC MRN");

            foreach (String WoahId in WOHMembershipData.GetWoahIds())
            {
                WoahIdCounter++;
                HashSet<String> MrnSet = NbmcMrns.ContainsKey(WoahId) ? NbmcMrns[WoahId] : new HashSet<string>();
                PipelineDefinition<BsonDocument, BsonDocument> pipeline = new BsonDocument[]
                {
                    //new BsonDocument { { "$match", new BsonDocument("identifiers.root", "2.16.124.113635.1.4.1.104") } },  // WOAH ID
                    new BsonDocument { { "$match", new BsonDocument("identifiers.extension", WoahId) } },
                    new BsonDocument { { "$project", new BsonDocument { {"identifiers", 1}, {"names", 1}, { "birthDate", 1 } } } },
                    new BsonDocument { { "$unwind", new BsonDocument("path", "$identifiers") } },
                    new BsonDocument { { "$match", new BsonDocument("identifiers.root", "1.3.6.1.4.1.22812.3.7498501.3") } },  // NBMC MRN
                };

                int PerWoahIdMatchCounter = 0; 

                var results = PatientCollection.Aggregate(pipeline);

                // The pipeline will yield one match per NBMC MRN
                foreach (BsonDocument OneResult in results.ToEnumerable())
                {
                    PerWoahIdMatchCounter++;
                    MaxPerIdMatchCounter = Math.Max(MaxPerIdMatchCounter, PerWoahIdMatchCounter);
                    BsonDocument IdentifiersDoc = OneResult["identifiers"].AsBsonDocument;
                    Trace.WriteLine("\n_id is " + OneResult["_id"].AsString);
                    string root = IdentifiersDoc["root"].AsString;
                    string ext = IdentifiersDoc["extension"].AsString;
                    Trace.WriteLine(root + " -> " + ext);

                    MaxMrnCounter = Math.Max(MaxMrnCounter, PerWoahIdMatchCounter);
                    MrnSet.Add(ext);
                    NbmcMrns[WoahId] = MrnSet;
                }

                if (NbmcMrns.ContainsKey(WoahId))
                {
                    CsvWriter.WriteLine(WoahId + "|" + String.Join(",", NbmcMrns[WoahId].ToArray()));
                    CsvWriter.FlushAsync();
                }
                Trace.Write("WOAH ID " + WoahId + " was matched " + PerWoahIdMatchCounter + " times in MongoDB");
                Trace.WriteLine(NbmcMrns.ContainsKey(WoahId) ? ", Mrns are: " + String.Join(", ", NbmcMrns[WoahId].ToArray()) : "");
            }

            for (int i = 0; i<2; i++)
            {
                try  // the close can throw, I think because the asynchronous flush is not completed
                {
                    CsvWriter.Close();
                }
                catch (Exception)
                {
                    Thread.Sleep(200);
                    continue;
                }
                break;
            }
            Trace.WriteLine("WOAH ID count: " + WoahIdCounter);
            Trace.WriteLine("MaxPerIdMatchCounter: " + MaxPerIdMatchCounter);
            Trace.WriteLine("MaxMrnCounter: " + MaxMrnCounter);
        }
    }
}
