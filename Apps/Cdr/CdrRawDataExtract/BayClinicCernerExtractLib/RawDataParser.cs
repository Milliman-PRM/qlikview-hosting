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

        /// <summary>
        /// Oversees the migration of all raw data from zip files in a specified folder
        /// </summary>
        /// <param name="zipFolder">Folder to be processed</param>
        /// <param name="InsertToMongo">set true to import data to MongoDB.  Otherwise the raw data is parsed only. </param>
        /// <param name="ArchiveFolder">Destination folder to move zip files after successful import.  Default or null to skip archiving</param>
        public void MigrateFolderToMongo(string zipFolder, bool InsertToMongo = false, String ArchiveFolder = null)
        {
            MongoDbConnection MongoCxn = new MongoDbConnection(CxParams);
            String ImportedZipsCollectionName = "importedzips";

            if (!MongoCxn.TestDatabaseAccess())
            {
                throw new Exception("problem while testing database using connection parameters: " + CxParams.ToString());
            }

            foreach (string Zip in Directory.GetFiles(zipFolder, @"*.zip").OrderBy(name => Directory.GetLastWriteTime(name)))
            {
                Dictionary<String, String> InsertDict = new Dictionary<string, string>();

                if (InsertToMongo)
                {
                    InsertDict.Add("zipfile", Path.GetFileName(Zip));
                    if (MongoCxn.DocumentExists(ImportedZipsCollectionName, InsertDict))
                    {
                        continue;
                    }
                }

                MigrateZipFileToMongo(Zip, InsertToMongo, ArchiveFolder, MongoCxn);

                if (InsertToMongo)
                {
                    MongoCxn.InsertDocument(ImportedZipsCollectionName, InsertDict);
                }
            }

            MongoCxn.Disconnect();
            MongoCxn = null;
        }

        /// <summary>
        /// Oversees the parsing and import of the contents of one zip file to MongoDB, and optional archiving of the zip file. 
        /// </summary>
        /// <param name="ZipFileName">Full path of file to process.</param>
        /// <param name="InsertToMongo">Specifies whether to perform inserts, default false.</param>
        /// <param name="ArchiveFolder">Destination folder to move zip file after successful processing.  Default or null to skip archiving.</param>
        /// <param name="MongoCxn">Optional connection object.  Created internally if not provided.</param>
        public void MigrateZipFileToMongo(string ZipFileName, bool InsertToMongo = false, String ArchiveFolder = null, MongoDbConnection MongoCxn = null)
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
            if (!String.IsNullOrEmpty(ArchiveFolder) && Directory.Exists(ArchiveFolder))
            {
                File.Move(ZipFileName, Path.Combine(ArchiveFolder, Path.GetFileName(ZipFileName)));
            }

            if (CreateLocalMongoConnection)
            {
                MongoCxn.Disconnect();
                MongoCxn = null;
            }

        }

        /// <summary>
        /// Oversees processing of one text file contained in a zip
        /// </summary>
        /// <param name="Entry"></param>
        /// <param name="InsertToMongo">Specifies whether to perform inserts, default false.</param>
        /// <param name="MongoCxn"></param>
        private void MigrateZipEntryToMongo(ZipArchiveEntry Entry, bool InsertToMongo = false, MongoDbConnection MongoCxn = null)
        {
            if (Entry.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                MigrateIndividualFileContentToMongo(new StreamReader(Entry.Open()), Entry.Name, InsertToMongo, MongoCxn);

                Trace.WriteLine( "Read ZipEntry: " + Entry.FullName );
            }
        }

        /// <summary>
        /// Parses and optionally imports contents of one raw data file (via. provided StreamReader) to MongoDB
        /// </summary>
        /// <param name="Reader"></param>
        /// <param name="TxtFileName"></param>
        /// <param name="InsertToMongo">Specifies whether to perform inserts, default false.</param>
        /// <param name="MongoCxn">Optional connection object.  Created internally if not provided.</param>
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

            {  // Remove any records previously inserted from this raw data file
                Dictionary<string, string> DeleteCriteria = new Dictionary<string, string>();
                DeleteCriteria.Add("ImportFile", TxtFileName);
                MongoCxn.DeleteDocuments(CollectionName, DeleteCriteria);
            }

            // Process all data lines from the rest of the stream
            while (Reader.Peek() >= 0)
            {
                // Lines after the first seem to have one more delimiter (at the end) than the header line, but no value after the last one.  
                // So Values[] gets one additional element but with no bad consequence since the last one is not a real value.  
                String Line = Reader.ReadLine();
                if (Line.EndsWith("|"))
                {
                    Line = Line.Remove(Line.Length - 1);  // remove any one trailing '|'
                }
                if (Line.EndsWith("\""))
                {
                    Line = Line.Remove(Line.Length - 1);  // remove any one trailing '"'
                }
                if (Line.StartsWith("\""))
                {
                    Line = Line.Remove(0, 1);    // remove any one leadling '"'
                }
                string[] Values = Line.Split(new string[]{ "\"|\""}, StringSplitOptions.None);

                Dictionary<string, string> NewDocDictionary = new Dictionary<string, string>();
                for (int i = 0; i < FieldNames.Length; i++)
                {
                    NewDocDictionary.Add(FieldNames[i], Values[i]);
                }
                NewDocDictionary.Add("ImportFile", TxtFileName);
                NewDocDictionary.Add("ImportFileDate", TxtFileName.Substring(8, 4) + TxtFileName.Substring(4, 2) + TxtFileName.Substring(6, 2));

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

