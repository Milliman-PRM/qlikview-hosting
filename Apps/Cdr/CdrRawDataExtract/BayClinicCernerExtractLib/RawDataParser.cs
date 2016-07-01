using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using MongoDbWrap;
using System.Threading;

namespace BayClinicCernerExtractLib
{
    /// <summary>
    /// class RawDataParser reads the Bay Clinic Cerner raw data exports and inserts documents to MongoDB collections
    /// </summary>
    public class RawDataParser
    {
        MongoDbConnectionParameters CxParams;
        Mutex Mutx;
        TextWriterTraceListener myTextListener;
        Dictionary<String, long> DocumentCounts;

        private bool _EndProcessing;
        public bool EndProcessing
        {
            get {
                Mutx.WaitOne();
                bool Ret = _EndProcessing;
                Mutx.ReleaseMutex();
                return Ret;
            }
            set {
                Mutx.WaitOne();
                _EndProcessing = value;
                Mutx.ReleaseMutex();
            }
        }

        public RawDataParser(string IniFile, string SectionName)
        {
            myTextListener = new TextWriterTraceListener("traceoutput.txt");
            Trace.Listeners.Add(myTextListener);
            Trace.AutoFlush = true;
            DocumentCounts = new Dictionary<string, long>();

            CxParams = new MongoDbConnectionParameters(IniFile, SectionName);
            Mutx = new Mutex();
            EndProcessing = false;
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

            DocumentCounts = new Dictionary<string, long>();

            if (!MongoCxn.TestDatabaseAccess())
            {
                throw new Exception("problem while testing database using connection parameters: " + CxParams.ToString());
            }

            foreach (string Zip in Directory.GetFiles(zipFolder, @"*.zip").OrderBy(name => Directory.GetLastWriteTime(name)))
            {
                Dictionary<String, String> InsertDict = new Dictionary<string, string>();

                if (EndProcessing)
                {
                    return;
                }

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

            Trace.WriteLine("Total document counts:");
            foreach (String Key in DocumentCounts.Keys)
            {
                Trace.WriteLine(Key + ": " + DocumentCounts[Key].ToString());
            }
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
                    Trace.WriteLine("Processing zip file " + ZipFileName);
                    foreach (ZipArchiveEntry Entry in archive.Entries.OrderBy(entry => entry.LastWriteTime))
                    {
                        if (EndProcessing)
                        {
                            break;
                        }

                        Trace.WriteLine("Processing ZipEntry: " + Entry.Name);
                        MigrateZipEntryToMongo(Entry, InsertToMongo, MongoCxn);
                    }
                }

            }
            catch (Exception e)
            {
                string Msg = e.Message;
                Trace.WriteLine("Exception caught while processing zip " + ZipFileName + ", Exception message is:\n " + Msg);
                // The zip file could not be opened, it's probably locked by the writer.   (or something else happened)
            }

            // Archive zip file to specified folder name
            if (!String.IsNullOrEmpty(ArchiveFolder) && Directory.Exists(ArchiveFolder))
            {
                try
                {
                    File.Move(ZipFileName, Path.Combine(ArchiveFolder, Path.GetFileName(ZipFileName)));
                }
                catch (Exception Exc)
                {
                    String Msg = Exc.Message;
                    switch (Exc.GetType().ToString())
                    {
                        case "System.IO.IOException":  // The destination file already exists.  - or -  sourceFileName was not found.
                            break;
                        case "System.IO.ArgumentNullException":  // sourceFileName or destFileName is null.
                            break;
                        case "System.IO.ArgumentException":  // sourceFileName or destFileName is a zero - length string, contains only white space, or contains invalid characters as defined in InvalidPathChars.
                            break;
                        case "System.IO.UnauthorizedAccessException":  // The caller does not have the required permission.
                            break;
                        case "System.IO.PathTooLongException":  // The specified path, file name, or both exceed the system - defined maximum length. For example, on Windows - based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
                            break;
                        case "System.IO.DirectoryNotFoundException":  // The path specified in sourceFileName or destFileName is invalid, (for example, it is on an unmapped drive).
                            break;
                        case "System.IO.NotSupportedException":  // sourceFileName or destFileName is in an invalid format.
                            break;
                    }
                }
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
            if (Entry.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) && !EndProcessing)
            {
                using (StreamReader Reader = new StreamReader(Entry.Open()))
                {
                    try
                    {
                        MigrateIndividualFileContentToMongo(Reader, Entry.Name, InsertToMongo, MongoCxn);
                    }
                    catch (Exception Exc)
                    {
                        Trace.WriteLine("Exception caught while processing content in " + Entry.Name + " Message is \n" + Exc.Message);
                    }
                }
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
            if (!DocumentCounts.ContainsKey(CollectionName))
            {
                DocumentCounts[CollectionName] = 0;
            }

            // Read and parse field names from the first line of the file
            string[] FieldNames = SplitLine(Reader.ReadLine().ToLower());

            bool CreateLocalMongoConnection = (MongoCxn == null && InsertToMongo);
            if (CreateLocalMongoConnection)
            {
                MongoCxn = new MongoDbConnection(CxParams);
            }

            // Process all data lines from the rest of the stream
            while (Reader.Peek() >= 0 && !EndProcessing)
            {
                DocumentCounts[CollectionName]++;

                string[] Values = SplitLine(Reader.ReadLine());

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
        private string[] SplitLine(String Line)
        {
            // Lines after the first seem to have one more delimiter (at the end) than the header line, but no value after the last one.  
            // So Values[] gets one additional element but with no bad consequence since the last one is not a real value.  
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

            return Line.Split(new string[] { "\"|\"" }, StringSplitOptions.None);
        }


    }
}

