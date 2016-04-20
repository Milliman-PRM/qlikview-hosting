using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace MongoDbWrap
{
    public class MongoDbConnection
    {
        private IMongoClient _Client = null;
        private IMongoDatabase _Db = null;
        public IMongoDatabase Db
        {
            get { return _Db; }
        }

        //public MongoDbConnection()
        //{}

        public MongoDbConnection(MongoDbConnectionParameters Params)
        {
            ConnectMongo(Params);
        }

        public bool CheckOpen()
        {
            // TODO: This needs to be better, check on connection status
            return _Client != null;

        }

        public void ConnectMongo(MongoDbConnectionParameters Params)
        {
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

        public bool InsertDocument(string CollectionName, Dictionary<string, string> Content)
        {
            // Insert the MongoDB document
            try
            {
                BsonDocument NewDoc = new BsonDocument();
                foreach (String Key in Content.Keys)
                {
                    NewDoc.Add(Key, Content[Key]);  // TODO get this value string parsed into a proper Bson type
                }
                _Db.GetCollection<BsonDocument>(CollectionName).InsertOneAsync(NewDoc);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool InsertDocument(string CollectionName, string JsonContentString)
        {
            try
            {
                BsonDocument NewDoc = BsonDocument.Parse(JsonContentString);
                _Db.GetCollection<BsonDocument>(CollectionName).InsertOneAsync(NewDoc);
            }
            catch (Exception /*e*/)
            {
                return false;
            }
            return true;
        }

    }
}
