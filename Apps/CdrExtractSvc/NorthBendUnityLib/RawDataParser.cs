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

namespace NorthBendUnityLib
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


            foreach (string Zip in Directory.GetFiles(zipFolder, @"*.zip").OrderBy(name => Directory.GetLastWriteTime(name)))
            {
                using (ZipArchive archive = ZipFile.OpenRead(Zip))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries.OrderBy(entry => entry.LastWriteTime))
                    {
                        if (!entry.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                            continue;  // don't process a file that is not named .json

                        using (StreamReader reader = new StreamReader(entry.Open()))
                        {
                            NorthBendImportFile ImportFileContent = JsonConvert.DeserializeObject<NorthBendImportFile>(reader.ReadToEnd());

                            if (ImportFileContent.Data == null) 
                                continue;

                            for (int PatientCounter = 0; PatientCounter < ImportFileContent.Data.Count; PatientCounter++ )
                            {
                                string EmrExportId = entry.Name + "{" + PatientCounter.ToString() + "}";

                                if (ImportFileContent.Data[PatientCounter].Type != JTokenType.Object)
                                {
                                    //System.Diagnostics.Debug.Write("Top level Data array element should be JObject type but found unexpected type: " + ImportFileContent.Data[PatientCounter].Type.ToString());
                                    throw new Exception("Top level Data array element should be JObject type but found unexpected type: " + ImportFileContent.Data[PatientCounter].Type.ToString());
                                }
                                JObject FullPatientRecord = (JObject)ImportFileContent.Data[PatientCounter];

                                if (FullPatientRecord.Children().Count() != 18)
                                {
                                    //System.Diagnostics.Debug.Write("EmrExportId " + EmrExportId + " expected to have 18 children but had " + FullPatientRecord.Children().Count().ToString());
                                    throw new Exception("EmrExportId " + EmrExportId + " expected to have 18 children but had " + FullPatientRecord.Children().Count().ToString());
                                }

                                foreach (JProperty PatientChildProperty in FullPatientRecord.Properties())
                                {
                                    Dictionary<string, string> NewDocDictionary = new Dictionary<string, string>();
                                    string CollectionName = PatientChildProperty.Name;

                                    switch (PatientChildProperty.Name)
                                    {
                                        case "patientDemographics":
                                            ProcessPatientDemographics(PatientChildProperty, EmrExportId);
                                            break;
                                        case "encounters":
                                            ProcessEncounters(PatientChildProperty, EmrExportId);
                                            break;
                                        case "chartVitals":
                                            ProcessArrayOfLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "chartProblems":
                                            ProcessArrayOfLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "medicalHistory":
                                            ProcessArrayOfLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "surgicalHistory":
                                            ProcessArrayOfLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "familyHistory":
                                            ProcessArrayOfLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "socialHistory":
                                            ProcessArrayOfLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "medications":
                                            ProcessArrayOfLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "immunizations":
                                            ProcessArrayOfLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "providers":
                                            ProcessObjectContainingLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "medicationDetails":
                                            ProcessObjectContainingLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "resultDetails":
                                            ProcessObjectContainingLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "vitalsActivities":
                                            ProcessArrayOfLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "orders":
                                            ProcessArrayOfLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "problems":
                                            ProcessProblems(PatientChildProperty, EmrExportId);
                                            break;
                                        case "results":
                                            ProcessArrayOfLeafObjects(PatientChildProperty, EmrExportId);
                                            break;
                                        case "completeExtract":
                                            break;

                                        default:
                                            //System.Diagnostics.Debug.Write("Unsupported top level property with name: " + PatientChildProperty.Name);
                                            throw new Exception("Unsupported top level property with name: " + PatientChildProperty.Name);
                                    }  // switch (PatientChildProperty.Name)
                                }  // foreach (JProperty PatientChildProperty ...
                            }  // for (int PatientCounter ...
                        }  // using (StreamReader ...
                    }
                }
            }

            MongoCxn.Disconnect();
            MongoCxn = null;
        }

        /// <summary>
        /// Expects that the supplied property value is Object type and contains leaf properties.  
        /// </summary>
        /// <param name="PatientDemographicsProperty"></param>
        /// <param name="EmrExportId"></param>
        /// <returns></returns>
        private bool ProcessPatientDemographics(JProperty PatientDemographicsProperty, string EmrExportId) 
        {
            bool ReturnValue = true;

            // Validate
            if (PatientDemographicsProperty.Value.Type != JTokenType.Object || PatientDemographicsProperty.Name != "patientDemographics")
            {
                //System.Diagnostics.Debug.WriteLine("Failed validation of patientDemographics property");
                throw new Exception("Failed validation of patientDemographics property");
            }

            JToken Contents = PatientDemographicsProperty.Value;
            Dictionary<string, string> NewDocDictionary = new Dictionary<string, string>();

            foreach (JProperty Field in Contents.Children())
            {
                NewDocDictionary.Add(Field.Name, Field.Value.ToString());
            }
            NewDocDictionary.Add("EmrExportId", EmrExportId);

            if (DoMongoInsert)
            {
                ReturnValue = MongoCxn.InsertDocument("patientdemographics", NewDocDictionary);
            }

            return ReturnValue;
        }

        /// <summary>
        /// Expects that the supplied property value is JArray type, with each element containing an encounterSummary 
        /// object with only leaf properties, and a charges array containing 0 or more objects with leaf values.  
        /// </summary>
        /// <param name="EncountersProperty"></param>
        /// <param name="EmrExportId"></param>
        /// <returns></returns>
        private bool ProcessEncounters(JProperty EncountersProperty, string EmrExportId)
        {
            bool ReturnValue = true;

            // Validate
            if (EncountersProperty.Value.Type != JTokenType.Array || EncountersProperty.Name != "encounters")
            {
                System.Diagnostics.Debug.WriteLine("Failed validation of encounters property");
                return false;
            }

            // The value was validated to be an array type so this is safe
            JEnumerable<JToken> EnumerableEncounters = EncountersProperty.Value.Children();
            Dictionary<string, string> NewDocDictionary;

            foreach (JObject EncounterObject in EnumerableEncounters)
            {
                foreach (JProperty EncounterObjectProperty in EncounterObject.Children())
                {
                    switch (EncounterObjectProperty.Name)
                    {
                        case "encounterSummary":
                            if (EncounterObjectProperty.Value.Type != JTokenType.Object)
                            {
                                //System.Diagnostics.Debug.WriteLine("encounterSummary property value is not JTokenType.Object");
                                throw new Exception("encounterSummary property value is not JTokenType.Object");
                            }

                            NewDocDictionary = new Dictionary<string, string>();
                            foreach (JToken EncounterSummaryChildToken in EncounterObjectProperty.Value.Children())
                            {
                                if (EncounterSummaryChildToken.Type != JTokenType.Property)
                                {
                                    //System.Diagnostics.Debug.WriteLine("In ProcessEncounters, encountered EncounterSummaryChildToken with unsupported type " + EncounterSummaryChildToken.Type.ToString());
                                    throw new Exception("In ProcessEncounters, encountered EncounterSummaryChildToken with unsupported type " + EncounterSummaryChildToken.Type.ToString());
                                }
                                NewDocDictionary.Add(((JProperty)EncounterSummaryChildToken).Name, ((JProperty)EncounterSummaryChildToken).Value.ToString());
                            }
                            NewDocDictionary.Add("EmrExportId", EmrExportId);

                            if (DoMongoInsert) 
                            {
                                MongoCxn.InsertDocument("encounter", NewDocDictionary);
                            }

                            break;

                        case "charges":
                            if (EncounterObjectProperty.Value.Type != JTokenType.Array)
                            {
                                System.Diagnostics.Debug.WriteLine("charges property value is not JTokenType.Array");
                                throw new Exception("charges property value is not JTokenType.Array");
                            }
                            JArray ArrayOfCharges = (JArray)EncounterObjectProperty.Value;
                            foreach (JObject ChargeObject in ArrayOfCharges)
                            {
                                NewDocDictionary = new Dictionary<string, string>();

                                foreach (JProperty ChargeObjectProperty in ChargeObject.Children())
                                {
                                    NewDocDictionary.Add(ChargeObjectProperty.Name, ChargeObjectProperty.Value.ToString());
                                }
                                NewDocDictionary.Add("EmrExportId", EmrExportId);

                                if (DoMongoInsert)
                                {
                                    ReturnValue = MongoCxn.InsertDocument("charge", NewDocDictionary);
                                }
                            }
                            break;

                        default:
                            System.Diagnostics.Debug.WriteLine("Unsupported child of encounter object: " + EncounterObjectProperty.Name);
                            break;
                    }
                }
            }

            return ReturnValue;
        }

        private bool ProcessProblems(JProperty TheProperty, string EmrExportId)
        {
            bool ReturnValue = true;

            // Validate
            if (TheProperty.Value.Type != JTokenType.Array)
            {
                System.Diagnostics.Debug.WriteLine("In ProcessArrayOfLeafObjects(), failed validation of " + TheProperty.Name + " property");
                return false;
            }

            // The value was validated to be an array type so this is safe
            JArray ArrayOfObjects = (JArray)TheProperty.Value;
            Dictionary<string, string> NewDocDictionary;

            foreach (JObject OneObject in ArrayOfObjects)
            {
                NewDocDictionary = new Dictionary<string, string>();
                foreach (JProperty ProblemProperty in OneObject.Children())
                {
                    switch (ProblemProperty.Value.Type)
                    {
                        case JTokenType.Object:
                            if (ProblemProperty.Name != "detail")
                            {
                                throw new Exception("Unsupported property encountered while processing problems data, name: " + ProblemProperty.Name);
                            }
                            foreach (JProperty ProblemDetailProperty in ProblemProperty.Value.Children())
                            {
                                NewDocDictionary.Add("detail_" + ProblemDetailProperty.Name, ProblemDetailProperty.Value.ToString());
                            }
                            break;

                        case JTokenType.String:
                            NewDocDictionary.Add(ProblemProperty.Name, ProblemProperty.Value.ToString());
                            break;

                        case JTokenType.Date:
                            NewDocDictionary.Add(ProblemProperty.Name, ProblemProperty.Value.ToString());
                            break;

                        case JTokenType.Boolean:
                            NewDocDictionary.Add(ProblemProperty.Name, ProblemProperty.Value.ToString());
                            break;

                        default:
                            throw new Exception("Unsupported property value type: " + ProblemProperty.Value.Type.ToString() + " encountered while processing problem object: " + EmrExportId);
                    }
                }
                NewDocDictionary.Add("EmrExportId", EmrExportId);

                if (DoMongoInsert)
                {
                    ReturnValue = MongoCxn.InsertDocument(TheProperty.Name, NewDocDictionary);
                }
            }

            return ReturnValue;
        }

        /// <summary>
        /// Expects that the supplied property value is JArray type, with each element containing an 
        /// object with leaf properties.  Uses the property name as the MongoDB collection name.  
        /// </summary>
        /// <param name="TheProperty"></param>
        /// <param name="EmrExportId"></param>
        /// <returns></returns>
        private bool ProcessArrayOfLeafObjects(JProperty TheProperty, string EmrExportId)
        {
            bool ReturnValue = true;

            // Validate
            if (TheProperty.Value.Type != JTokenType.Array)
            {
                System.Diagnostics.Debug.WriteLine("In ProcessArrayOfLeafObjects(), failed validation of " + TheProperty.Name + " property");
                return false;
            }

            // The value was validated to be an array type so this is safe
            JArray ArrayOfObjects = (JArray)TheProperty.Value;
            Dictionary<string, string> NewDocDictionary;

            foreach (JObject OneObject in ArrayOfObjects)
            {
                NewDocDictionary = new Dictionary<string, string>();
                foreach (JProperty IndividualLeafProperty in OneObject.Children())
                {
                    NewDocDictionary.Add(IndividualLeafProperty.Name, IndividualLeafProperty.Value.ToString());
                }
                NewDocDictionary.Add("EmrExportId", EmrExportId);

                if (DoMongoInsert)
                {
                    ReturnValue = MongoCxn.InsertDocument(TheProperty.Name, NewDocDictionary);
                }
            }

            return ReturnValue;
        }

        /// <summary>
        /// Expects that the supplied property value is JObject type, with each property value being an 
        /// object with leaf properties.  Uses the property name as the MongoDB collection name.  
        /// </summary>
        /// <param name="TheProperty"></param>
        /// <param name="EmrExportId"></param>
        /// <returns></returns>
        private bool ProcessObjectContainingLeafObjects(JProperty TheProperty, string EmrExportId)
        {
            bool ReturnValue = true;

            // Validate
            if (TheProperty.Value.Type != JTokenType.Object)
            {
                System.Diagnostics.Debug.WriteLine("In ProcessObjectContainingLeafObjects(), failed validation of " + TheProperty.Name + " property");
                return false;
            }

            // The value was validated to be an array type so this is safe
            Dictionary<string, string> NewDocDictionary;

            foreach (JProperty PropertyContainingLeafObject in TheProperty.Value.Children())
            {
                NewDocDictionary = new Dictionary<string, string>();
                foreach (JProperty IndividualLeafProperty in PropertyContainingLeafObject.Value.Children())
                {
                    NewDocDictionary.Add(IndividualLeafProperty.Name, IndividualLeafProperty.Value.ToString());
                }
                NewDocDictionary.Add("EmrExportId", EmrExportId);

                if (DoMongoInsert)
                {
                    ReturnValue = MongoCxn.InsertDocument(TheProperty.Name, NewDocDictionary);
                }
            }

            return ReturnValue;
        }

        /// <summary>
        /// Somebody's idea from the internet, supposed to be good, read this.  
        /// </summary>
        /// <param name="jo"></param>
        /// <returns></returns>
        private Dictionary<string, object> deserializeToDictionary(string jo)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(jo);
            var values2 = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> d in values)
            {
                // if (d.Value.GetType().FullName.Contains("Newtonsoft.Json.Linq.JObject"))
                if (d.Value is JObject)
                {
                    values2.Add(d.Key, deserializeToDictionary(d.Value.ToString()));
                }
                else
                {
                    values2.Add(d.Key, d.Value);
                }
            }
            return values2;
        }
    }

    public class NorthBendImportFile
    {
        public JObject Job;
        public JArray Data;
    }

}

