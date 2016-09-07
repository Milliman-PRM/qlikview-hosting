using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MongoDbWrap;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedoxExtractLib
{
    public class RawDataParser
    {
        Dictionary<String,MongoDbConnection> MongoConnections = new Dictionary<string, MongoDbConnection>();
        MongoDbConnectionParameters CxParams;
        bool DoMongoInsert;

        public RawDataParser(string IniFile, string MongoCredentialSectionName)
        {
            // Instantiate Mongo connection parameters
            CxParams = new MongoDbConnectionParameters();
            CxParams.ReadFromIni(IniFile, MongoCredentialSectionName);
        }

        public void MigrateRawToMongo(String SearchFolder, String ArchiveFolder, bool InsertToMongo = false)
        {
            DoMongoInsert = InsertToMongo;

            foreach (string ClinicalSummaryFileName in Directory.GetFiles(SearchFolder, @"*.json").OrderBy(name => Directory.GetLastWriteTime(name)))
            {
                ProcessClinicalSummaryFile(ClinicalSummaryFileName, ArchiveFolder, InsertToMongo);
            }

        }

        private bool ProcessClinicalSummaryFile(String JsonFileName, String ArchiveFolder, bool InsertToMongo)
        {
            String SourceName;
            JObject FileContentObj;
            JObject SchedulingObj;
            JObject ClinicalSumObj;

            try
            {
                FileContentObj = JObject.Parse(File.ReadAllText(JsonFileName));
                SchedulingObj = FileContentObj.Value<JObject>("Scheduling");
                ClinicalSumObj = FileContentObj.Value<JObject>("ClinicalSummary");
                SourceName = SchedulingObj["Meta"]["Source"]["Name"].ToString().Replace(" ", "");
            }
            catch (Exception /*e*/)
            {
                return false;
            }

            // establish a MongoDB connection
            if (!MongoConnections.ContainsKey(SourceName))
            {
                CxParams._Db = SourceName;
                MongoConnections[SourceName] = new MongoDbConnection(CxParams);
            }

            if (!MongoConnections[SourceName].TestDatabaseAccess())
            {
                MongoConnections.Remove(SourceName);
                return false;
            }

            bool OverallSuccess = true;
            if (InsertToMongo) {
                List<bool> IndividualSuccesses = new List<bool>();
                IndividualSuccesses.Add(MongoConnections[SourceName].InsertDocument("Scheduling", JsonConvert.SerializeObject(SchedulingObj)));
                IndividualSuccesses.Add(MongoConnections[SourceName].InsertDocument("ClinicalSummary", JsonConvert.SerializeObject(ClinicalSumObj)));

                OverallSuccess = IndividualSuccesses.All(x => x);

                if (OverallSuccess)
                {
                    MoveFileToArchive(JsonFileName, ArchiveFolder);
                }
            }

            return OverallSuccess;
        }

        private void MoveFileToArchive(String JsonFileName, String ArchiveFolder)
        {
            String NewFileName = Path.Combine(ArchiveFolder, Path.GetFileName(JsonFileName));
            File.Move(JsonFileName, NewFileName);
        }
    }
}
