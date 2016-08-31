using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using MongoDbWrap;

namespace BayClinicEmrLib
{
    /// <summary>
    /// class RawDataParser reads the Bay Clinic Cerner raw data exports and inserts documents to MongoDB collections
    /// </summary>
    public class RawDataParser
    {
        MongoDbConnectionParameters CxParams;
        bool DoMongoInsert;

        public RawDataParser(string IniFile, string SectionName)
        {
            CxParams = new MongoDbConnectionParameters(IniFile, SectionName);
        }

        public void MigrateRawToMongo(string zipFolder, bool InsertToMongo = false)
        {
            DoMongoInsert = InsertToMongo;

            MongoDbConnection MongoCxn = new MongoDbConnection(CxParams);

            if (!MongoCxn.TestDatabaseAccess())
            {
                throw new Exception("problem while testing database using connection parameters: " + CxParams.ToString());
            }

            foreach (string Zip in Directory.GetFiles(zipFolder, @"*.zip").OrderBy(name => Directory.GetLastWriteTime(name)))
            {
                using (ZipArchive archive = ZipFile.OpenRead(Zip))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries.OrderBy(entry => entry.LastWriteTime))
                    {
                        //Console.WriteLine("File: {0}", entry.FullName);

                        if (entry.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                        {
                            int DotPos = entry.Name.LastIndexOf('.');
                            int UnderscorePos = entry.Name.LastIndexOf('_', DotPos);

                            string CollectionName = entry.Name.Substring(UnderscorePos+1, DotPos - UnderscorePos - 1).ToLower();
                            if (string.IsNullOrEmpty(CollectionName))
                            {
                                throw new Exception("Empty collection name, can not process.");
                            }

                            //string extractPath = @"C:\Users\Tom.Puckett\Desktop\Bay Clinic archive";
                            //entry.ExtractToFile(Path.Combine(extractPath, entry.FullName));
                            using (StreamReader reader = new StreamReader(entry.Open()))
                            {
                                string[] FieldNames = reader.ReadLine().ToLower().Replace("\"","").Split('|');

                                while (reader.Peek() >= 0) 
                                {
                                    string[] Values = reader.ReadLine().Replace("\"", "").Split('|');
                                    Dictionary<string,string> NewDocDictionary = new Dictionary<string,string>();
                                    for (int i=0 ; i<FieldNames.Length ; i++) 
                                    {
                                        NewDocDictionary.Add(FieldNames[i], Values[i]);
                                    }
                                    NewDocDictionary.Add("ImportFile", entry.Name);

                                    // Insert the MongoDB document
                                    if (DoMongoInsert)
                                    {
                                        MongoCxn.InsertDocument(CollectionName, NewDocDictionary);
                                    }
                                }
                                Console.WriteLine(
                                "Read File {0}", entry.FullName
                                //reader.ReadToEnd();
                                );
                            }
                        }
                    }
                }
            }

            MongoCxn.Disconnect();
        }

    }
}

