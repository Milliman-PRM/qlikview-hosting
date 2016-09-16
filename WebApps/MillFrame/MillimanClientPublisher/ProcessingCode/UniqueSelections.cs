using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientPublisher.ProcessingCode
{
    public class UniqueSelections
    {
        public class UniqueSelection
        {
            //absolute path to selection file
            public string SelectionFile { get; set; }
            //user accounts that need access to SelectionFile
            public List<string> Accounts;

            //reduced QVW name as utilized in current cache
            public string RequestedReducedQVWName { get; set; }

            public UniqueSelection()
            {
                Accounts = new List<string>();
            }

            public MillimanCommon.ReduceConfig ReductionConfiguration { get; set; }

        }

        public Dictionary<string, UniqueSelection> UniqueSelectionDictionary { get; set; }

        public Dictionary<string, List<string>> ReducedReportToAccountMapping()
        {
            Dictionary<string, List<string>> Map = new Dictionary<string, List<string>>();
            if (UniqueSelectionDictionary != null)
            {
                foreach (KeyValuePair<string, UniqueSelection> KVP in UniqueSelectionDictionary)
                {
                    Map.Add(KVP.Key, KVP.Value.Accounts);
                }
            }
            return Map;
        }

        /// <summary>
        /// Construct this class, and it will loop over the selections file and create a list
        /// in which identical selection files will be grouped, this can then be passed to the 
        /// reduction server for the miniminal reduction required
        /// </summary>
        /// <param name="WorkingDirectory"></param>
        /// <param name="CurrentProject"></param>
        public UniqueSelections( string WorkingDirectory, ProjectSettingsExtension CurrentProject, string DataModelFile )
        {
            //data model file should always be in local working directory
            DataModelFile = System.IO.Path.Combine(WorkingDirectory, DataModelFile);

            UniqueSelectionDictionary = new Dictionary<string,UniqueSelection>();

            string[] Selections = System.IO.Directory.GetFiles(WorkingDirectory, CurrentProject.ProjectName + ".selections", System.IO.SearchOption.AllDirectories);
            foreach( string Selection in Selections )
            {
                string Hash = MillimanCommon.Utilities.CalculateMD5Hash(Selection, true);  //calcuate the hash value
                if ( UniqueSelectionDictionary.ContainsKey(Hash) )
                {
                    //add directory to list, this selection matches one already found
                    UniqueSelectionDictionary[Hash].Accounts.Add(System.IO.Path.GetDirectoryName(Selection));
                }
                else
                {
                    UniqueSelection US = new UniqueSelection();
                    US.SelectionFile = Selection;
                    US.Accounts.Add(System.IO.Path.GetDirectoryName(Selection));
                    string ReferenceFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Selection), "[REFERENCE].redirect");
                    string NewReferenceFilename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Selection), CurrentProject.QVName + ".redirect");
                    string ReferenceFileContents = string.Empty;
                    if (System.IO.File.Exists(NewReferenceFilename))  //look to see if the new un-ambigious reference name is avaialble, if so use it
                        ReferenceFileContents = System.IO.File.ReadAllText(NewReferenceFilename); //relative path to a QVW
                    else
                        ReferenceFileContents = System.IO.File.ReadAllText(ReferenceFileName);  //otherwise use the old way of doing things

                    US.RequestedReducedQVWName = System.IO.Path.GetFileNameWithoutExtension(ReferenceFileContents);  //should be a GUID looking ID
                    UniqueSelectionDictionary.Add(Hash, US);
                }
            }

            string MasterQVWName = CurrentProject.QVName + @".qvw";
            string RequestedMasterStatusLog = CurrentProject.QVName + "_master.log";
            List<MillimanCommon.NVPair> RequestedUniqueValuesFromReducedColumns = null; //GetDMColumns(CurrentProject, DataModelFile, );
            string RequestedReducedQVWStatusLog = "";
            int Index = 0;
            //now loop over entries and create the reduce configurationf files
            foreach( KeyValuePair<string, UniqueSelection> Current in UniqueSelectionDictionary )
            {
                //this isn't very efficient, but we need the unique values per column file name to change on each iteration
                RequestedUniqueValuesFromReducedColumns = GetDMColumns(CurrentProject, DataModelFile, Current.Value.RequestedReducedQVWName);
                //create a new processor for each item, slower but safer
                Processor Proc = new Processor();
                RequestedReducedQVWStatusLog = CurrentProject.QVName + "_" + Index.ToString() + ".log" ;
                Index++;
                Current.Value.ReductionConfiguration = Proc.ReductionConfigurationFromSelectionFile(MasterQVWName,
                                                                    RequestedMasterStatusLog,
                                                                    Current.Value.RequestedReducedQVWName,
                                                                    RequestedReducedQVWStatusLog,
                                                                    string.Empty,
                                                                    string.Empty,
                                                                    string.Empty,
                                                                    string.Empty,
                                                                    null,
                                                                    null,
                                                                    true,
                                                                    null,
                                                                    RequestedUniqueValuesFromReducedColumns,
                                                                    Current.Value.SelectionFile,
                                                                    DataModelFile,
                                                                    true);
            }

        }

        /// <summary>
        /// Create the list of data model column names and unique file ids to pass into reduction server, this
        /// will give back the list of items to verify against
        /// </summary>
        /// <param name="CurrentProject"></param>
        /// <param name="SelectionFile"></param>
        /// <returns></returns>
        private List<MillimanCommon.NVPair> GetDMColumns(ProjectSettingsExtension CurrentProject, string SelectionFile, string BaseFile)
        {
            List<MillimanCommon.NVPair> Selections = new List<MillimanCommon.NVPair>();
            Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
            List<string> DeserializedItems = SS.Deserialize(SelectionFile) as List<string>;
            if ( DeserializedItems != null )
            {
                foreach( string DMItem in DeserializedItems )
                {
                    string[] DMTokens = DMItem.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if ( DMTokens != null)
                    {
                        foreach( string Token in DMTokens )
                        {
                            MillimanCommon.NVPair TokenItem = new MillimanCommon.NVPair();
                            TokenItem.FieldName = Token;
                            TokenItem.Value = BaseFile + ".uniquevalues_" + Token.Replace(' ', '_');  //just in case get rid of blanks
                            Selections.Add(TokenItem);
                        }
                    }
                }
            }
            return Selections;
        }

    }
}