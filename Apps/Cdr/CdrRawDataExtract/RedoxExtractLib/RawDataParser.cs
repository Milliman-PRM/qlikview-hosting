using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using MongoDbWrap;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedoxExtractLib
{
    public class RawDataParser
    {
        MongoDbConnectionParameters CxParams;
        MongoDbConnection MongoCxn;
        bool DoMongoInsert;

        public RawDataParser(string IniFile, string MongoCredentialSectionName)
        {
            // Instantiate Mongo connection parameters, will throw if not available
            CxParams = new MongoDbConnectionParameters(IniFile, MongoCredentialSectionName);
        }

        public void MigrateRawToMongo(string zipFolder, bool InsertToMongo = false)
        {
            DoMongoInsert = InsertToMongo;

            MongoCxn = new MongoDbConnection(CxParams);

            if (!MongoCxn.TestDatabaseAccess())
            {
                throw new Exception("problem while testing database using connection parameters: " + CxParams.ToString());
            }
        }
    }
}
