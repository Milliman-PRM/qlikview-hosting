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
                                    if (InsertToMongo)
                                    {
                                        if (DoMongoInsert)
                                        {
                                            MongoCxn.InsertDocument(CollectionName, NewDocDictionary);
                                        }
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

        private void SampleDictionaryCode()
        {
            /* Sample usage of Dictionary class
             * 
            Dictionary<string, string> openWith = new Dictionary<string, string>();

            // Add some elements to the dictionary. There are no 
            // duplicate keys, but some of the values are duplicates.
            openWith.Add("txt", "notepad.exe");
            openWith.Add("bmp", "paint.exe");
            openWith.Add("dib", "paint.exe");
            openWith.Add("rtf", "wordpad.exe");

            // The Add method throws an exception if the new key is 
            // already in the dictionary.
            try
            {
                openWith.Add("txt", "winword.exe");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("An element with Key = \"txt\" already exists.");
            }

            // The Item property is another name for the indexer, so you 
            // can omit its name when accessing elements. 
            Console.WriteLine("For key = \"rtf\", value = {0}.",
                openWith["rtf"]);

            // The indexer can be used to change the value associated
            // with a key.
            openWith["rtf"] = "winword.exe";
            Console.WriteLine("For key = \"rtf\", value = {0}.",
                openWith["rtf"]);
             * 
             */
        }
    }
}

