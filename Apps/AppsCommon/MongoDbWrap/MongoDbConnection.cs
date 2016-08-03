using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoDbWrap
{
    public class MongoDbConnection
    {
        private IMongoClient _Client = null;
        private IMongoDatabase _Db = null;

        public bool IsInitialized
        {
            get { return TestDatabaseAccess(); }
        }

        public IMongoDatabase Db
        {
            get { return _Db; }
        }

        public MongoDbConnection()
        {}

        public bool InitializeWithIni(String IniFileName, String SectionName)
        {
            if (!File.Exists(IniFileName) || String.IsNullOrEmpty(SectionName))
            {
                return false;
            }

            MongoDbConnectionParameters Params = new MongoDbConnectionParameters();
            if (!Params.ReadFromIni(IniFileName, SectionName))
            {
                return false;
            }

            return ConnectMongo(Params);
        }

        public bool InitializeWithEnvironment(String MongoDbNameBase)
        {
            MongoDbConnectionParameters Params = new MongoDbConnectionParameters();
            Params.ReadFromEnvironment(MongoDbNameBase);
            // Trace.WriteLine("Mongo connection params: " + Params.ToString());  // Don't leave this on, password is logged
            return ConnectMongo(Params);
        }

        public MongoDbConnection(MongoDbConnectionParameters Params)
        {
            ConnectMongo(Params);
        }

        public bool CheckOpen()
        {
            // TODO: This needs to be better, check on connection status
            return _Client != null;

        }

        public bool ConnectMongo(MongoDbConnectionParameters Params)
        {
            if (!Params.IsValid())
            {
                return false;
            }

            var credential = MongoCredential.CreateCredential(Params._UserDomain, Params._User, Params._Password);
            var settings = new MongoClientSettings
            {
                Credentials = new[] { credential },
                MaxConnectionLifeTime = new TimeSpan(2,0,0)
            };
            settings.Server = (Params._Port == 0) ? new MongoServerAddress(Params._Host) :
                                                    new MongoServerAddress(Params._Host, Params._Port);

            string ConnectionString = @"mongodb://" + Params._User + ":" + Params._Password + "@" + Params._Host;
            if (Params._Port > 0)
            {
                ConnectionString += ":" + Params._Port.ToString();
            }

            var Settings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));
            Settings.MaxConnectionLifeTime = new TimeSpan(2, 0, 0);

            _Client = new MongoClient(settings);
            //MongoClientSettings ClientSettings = Client.Settings;

            if (!string.IsNullOrEmpty(Params._Db))
            {
                AccessMongoDatabase(Params._Db);
            }

            return _Client != null;
        }

        public void Disconnect()
        {
            _Db = null;
            _Client = null;
        }

        public IMongoDatabase AccessMongoDatabase(string DbName)
        {
            _Db = _Client.GetDatabase(DbName);
            return _Db;
        }

        public bool TestDatabaseAccess()
        {
            try
            {
                GetCollectionNames();
            }
            catch
            {
                return false;
            }
            return true;
        }

#if false
        public List<Dictionary<String, String>> GetDocuments(String CollectionName, Dictionary<String,String> SearchFilter)
        {
            List<Dictionary<String, String>> ReturnDict = new List<Dictionary<String, String>>();
            List<MongodbPersonEntity> ReturnPersons = new List<MongodbPersonEntity>();

            if (_Db != null)
            {
                BsonDocument Filter = new BsonDocument(SearchFilter);
                Filter = new BsonDocument ( );
                //Filter = new BsonDocument { new BsonElement("last_name", new BsonString(SearchFilter["last_name"])) };
                ProjectionDefinition<Dictionary<String, String>> Proj = Builders<Dictionary<String, String>>.Projection.Exclude("_id");

                //ReturnValb = _Db.GetCollection<BsonDocument>(CollectionName).Find(x => x["last_name"] != "").ToList();
                ReturnPersons = _Db.GetCollection<Person>(CollectionName).Find("{last_name: {$ne: \"\"}}").ToList();
                ReturnDict = _Db.GetCollection<Dictionary<String, String>>(CollectionName).Find("{last_name: {$ne: \"\"}}").Project<Dictionary<String, String>>(Proj).ToList();
            }

            return ReturnDict;
        }
#endif

        public List<string> GetCollectionNames()
        {
            List<string> CollectionNames = new List<string>();

            if (_Db != null)
            {
                List<BsonDocument> Collections;
                try
                {
                    Collections = _Db.ListCollectionsAsync().Result.ToListAsync().Result;
                }
                catch (Exception e) // typically happens if the database server is down
                {
                    string Error = e.ToString();
                    throw;
                }

                foreach (var Collection in Collections)
                {
                    CollectionNames.Add(Collection.GetValue("name", "").AsString);
                }
            }

            return CollectionNames;
        }

        public bool DeleteDocuments(string CollectionName, Dictionary<string, string> Match, bool Async = false)
        {
            try
            {
                DeleteResult Result;

                BsonDocument MatchDoc = new BsonDocument();
                foreach (String Key in Match.Keys)
                {
                    MatchDoc.Add(Key, Match[Key]);
                }
                if (Async)
                {
                    _Db.GetCollection<BsonDocument>(CollectionName).DeleteManyAsync(MatchDoc);
                }
                else
                {
                    Result = _Db.GetCollection<BsonDocument>(CollectionName).DeleteMany(MatchDoc);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Builds a bson document and inserts to MongoDB.
        /// </summary>
        /// <param name="CollectionName"></param>
        /// <param name="Content"></param>
        /// <param name="Async">Frequent use of async can result in lost inserts, beware</param>
        /// <returns></returns>
        public bool InsertDocument(string CollectionName, Dictionary<string, string> Content, bool Async = false)
        {
            // Insert the MongoDB document
            try
            {
                BsonDocument NewDoc = new BsonDocument();
                foreach (String Key in Content.Keys)
                {
                    NewDoc.Add(Key, Content[Key]);  // TODO get this value string parsed into a proper Bson type
                }

                if (Async)
                {
                    _Db.GetCollection<BsonDocument>(CollectionName).InsertOneAsync(NewDoc);
                }
                else
                {
                    _Db.GetCollection<BsonDocument>(CollectionName).InsertOne(NewDoc);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Builds a bson document and inserts to MongoDB.
        /// </summary>
        /// <param name="CollectionName"></param>
        /// <param name="JsonContentString"></param>
        /// <param name="Async">Frequent use of async can result in lost inserts, beware</param>
        /// <returns></returns>
        public bool InsertDocument(string CollectionName, string JsonContentString, bool Async = false)
        {
            try
            {
                BsonDocument NewDoc = BsonDocument.Parse(JsonContentString);
                if (Async)
                {
                    _Db.GetCollection<BsonDocument>(CollectionName).InsertOneAsync(NewDoc);
                }
                else
                {
                    _Db.GetCollection<BsonDocument>(CollectionName).InsertOne(NewDoc);
                }
            }
            catch (Exception e)
            {
                String Msg = e.Message;
                return false;
            }
            return true;
        }

        public bool DocumentExists(string CollectionName, Dictionary<string, string> Match)
        {
            long HowManyMatches;

            try
            {
                //FilterDefinition<BsonDocument> FindFilterDef = Builders<BsonDocument>.Filter.Empty;
                FilterDefinition<BsonDocument> FindFilterDef = null;
                foreach (KeyValuePair<String,String> Criterion in Match)
                {
                    if (FindFilterDef == null)
                    {
                        FindFilterDef = Builders<BsonDocument>.Filter.AnyEq(Criterion.Key, Criterion.Value);
                    }
                    else
                    {
                        FindFilterDef = FindFilterDef & Builders<BsonDocument>.Filter.AnyEq(Criterion.Key, Criterion.Value);
                    }
                }

                HowManyMatches = _Db.GetCollection<BsonDocument>(CollectionName).Find(FindFilterDef).Count();
            }
            catch (Exception e)
            {
                String Msg = e.Message;
                return false;
            }
            return HowManyMatches > 0;
        }

    }

}
