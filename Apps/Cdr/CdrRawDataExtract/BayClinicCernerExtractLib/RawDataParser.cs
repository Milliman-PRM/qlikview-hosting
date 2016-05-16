using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using MongoDbWrap;

namespace BayClinicCernerExtractLib
{
    /// <summary>
    /// class RawDataParser reads the Bay Clinic Cerner raw data exports and inserts documents to MongoDB collections
    /// </summary>
    public class RawDataParser
    {
        MongoDbConnectionParameters CxParams;

        public RawDataParser(string IniFile, string SectionName)
        {
            CxParams = new MongoDbConnectionParameters(IniFile, SectionName);
        }

        public void MigrateFolderToMongo(string zipFolder, bool InsertToMongo = false, String ArchiveFolder = null, bool DoArchive = false)
        {
            MongoDbConnection MongoCxn = new MongoDbConnection(CxParams);

            if (!MongoCxn.TestDatabaseAccess())
            {
                throw new Exception("problem while testing database using connection parameters: " + CxParams.ToString());
            }

            foreach (string Zip in Directory.GetFiles(zipFolder, @"*.zip").OrderBy(name => Directory.GetLastWriteTime(name)))
            {
                MigrateZipFileToMongo(Zip, InsertToMongo, ArchiveFolder, MongoCxn);
            }

            MongoCxn.Disconnect();
            MongoCxn = null;
        }

        public void MigrateZipFileToMongo(string ZipFileName, bool InsertToMongo = false, String ArchiveFolder = null, MongoDbConnection MongoCxn = null, bool DoArchive = false)
        {
            bool CreateLocalMongoConnection = (MongoCxn == null && InsertToMongo);
            if (CreateLocalMongoConnection)
            {
                MongoCxn = new MongoDbConnection(CxParams);
            }

            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(ZipFileName))
                {
                    foreach (ZipArchiveEntry Entry in archive.Entries.OrderBy(entry => entry.LastWriteTime))
                    {
                        Trace.WriteLine("Processing zip entry: " + Entry.Name);
                        MigrateZipEntryToMongo(Entry, InsertToMongo, MongoCxn);
                    }
                }

            }
            catch (Exception /*e*/)
            {
                // The zip file could not be opened, it's probably locked by the writer.   (or something else happened)
            }

            // Archive zip file to specified folder name
            if (DoArchive && !String.IsNullOrEmpty(ArchiveFolder) && Directory.Exists(ArchiveFolder))
            {
                File.Move(ZipFileName, Path.Combine(ArchiveFolder, Path.GetFileName(ZipFileName)));
            }

            if (CreateLocalMongoConnection)
            {
                MongoCxn.Disconnect();
                MongoCxn = null;
            }

        }

        private void MigrateZipEntryToMongo(ZipArchiveEntry Entry, bool InsertToMongo = false, MongoDbConnection MongoCxn = null)
        {
            if (Entry.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                MigrateIndividualFileContentToMongo(new StreamReader(Entry.Open()), Entry.Name, InsertToMongo, MongoCxn);

                Trace.WriteLine( "Read ZipEntry: " + Entry.FullName );
            }
        }

        public void MigrateIndividualFileContentToMongo(StreamReader Reader, String TxtFileName, bool InsertToMongo = false, MongoDbConnection MongoCxn = null)
        {
            // Figure out the appropriate collection name
            int DotPos = TxtFileName.LastIndexOf('.');
            int UnderscorePos = TxtFileName.LastIndexOf('_', DotPos);
            string CollectionName = TxtFileName.Substring(UnderscorePos + 1, DotPos - UnderscorePos - 1).ToLower();
            if (string.IsNullOrEmpty(CollectionName))
            {
                throw new Exception("Empty collection name, can not process.");
            }

            // Read and parse field names from the first line of the file
            string[] FieldNames = Reader.ReadLine().ToLower().Replace("\"", "").Split('|');

            bool CreateLocalMongoConnection = (MongoCxn == null && InsertToMongo);
            if (CreateLocalMongoConnection)
            {
                MongoCxn = new MongoDbConnection(CxParams);
            }

            // Process all data lines from the rest of the stream
            while (Reader.Peek() >= 0)
            {
                // Lines after the first seem to have one more delimiter (at the end) than the header line, but no value after the last one.  
                // So Values[] gets one additional element but with no bad consequence since the last one is not a real value.  
                string[] Values = Reader.ReadLine().Replace("\"", "").Split('|');

                Dictionary<string, string> NewDocDictionary = new Dictionary<string, string>();
                for (int i = 0; i < FieldNames.Length; i++)
                {
                    NewDocDictionary.Add(FieldNames[i], Values[i]);
                }
                NewDocDictionary.Add("ImportFile", TxtFileName);

                // Insert the MongoDB document
                if (InsertToMongo)
                {
                    MongoCxn.InsertDocument(CollectionName, NewDocDictionary);
                }
            }

            if (CreateLocalMongoConnection)
            {
                MongoCxn.Disconnect();
                MongoCxn = null;
            }
        }

    }
}

