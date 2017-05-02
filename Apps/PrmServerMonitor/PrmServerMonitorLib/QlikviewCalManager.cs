/*
 * CODE OWNERS: Tom Puckett, Ben Wyatt
 * OBJECTIVE: Dedicated class to process tracking of Qlikview CALs.
 * DEVELOPER NOTES: Note that this is derived from ServerMonitorProcessingBase, as all future processing classes should be
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PrmServerMonitorLib.Qms;
using PrmServerMonitorLib.ServiceSupport;

namespace PrmServerMonitorLib
{
    public class QlikviewCalManager : QlikviewProcessingBase
    {
        /// <summary>
        /// Used by a caller to identify a specific supported type of CAL statistic
        /// </summary>
        public enum CalStatisticEnum
        {
            NamedCalAssigned,
            NamedCalLimit,
            DocumentCalAssigned,
            DocumentCalLimit
        }

        /// <summary>
        /// Constructor.  Passes optional arguments to the base class
        /// </summary>
        /// <param name="ServerNameArg"></param>
        /// <param name="LifetimeTraceArg"></param>
        public QlikviewCalManager(string ServerNameArg = "localhost", bool LifetimeTraceArg = false) : base(ServerNameArg, LifetimeTraceArg)
        { }

        /// <summary>
        /// Destructor
        /// </summary>
        ~QlikviewCalManager()
        {
            CloseTraceLogFile(true);
        }

        /// <summary>
        /// Dumps a single caller selectable CAL related statistic value to trace log.  Intended mainly for use in non-interactive mode.  
        /// </summary>
        /// <param name="FieldToReport"></param>
        /// <param name="UseTraceFile">Only has an effect if this is instantiated without LifetimeTraceFile argument</param>
        public void ReportCalStatistic(CalStatisticEnum FieldToReport, bool UseTraceFile)
        {
            if (UseTraceFile)
            {
                EstablishTraceLogFile();
            }

            CALConfiguration CalConfig = EnumerateAllCals(false);
            if (CalConfig == null)
            {
                Trace.WriteLine("QlikviewCalManager.EnumerateAllCals() returned null");
                return;
            }

            switch (FieldToReport)
            {
                case CalStatisticEnum.DocumentCalAssigned:
                    Trace.WriteLine(CalConfig.DocumentCALs.Assigned);
                    break;
                case CalStatisticEnum.DocumentCalLimit:
                    Trace.WriteLine(CalConfig.DocumentCALs.Limit);
                    break;
                case CalStatisticEnum.NamedCalAssigned:
                    Trace.WriteLine(CalConfig.NamedCALs.Assigned);
                    break;
                case CalStatisticEnum.NamedCalLimit:
                    Trace.WriteLine(CalConfig.NamedCALs.Limit);
                    break;
            }

            CloseTraceLogFile();
        }

        /// <summary>
        /// Obtains the CALConfiguration object representing the server configuration and optionally reports certain statistics to the trace log.  
        /// </summary>
        /// <param name="UseTraceFile">Only has an effect if this is instantiated without LifetimeTraceFile argument</param>
        /// <returns>null if operation failed</returns>
        public CALConfiguration EnumerateAllCals(bool UseTraceFile = false)
        {
            CALConfiguration CalConfig = null;

            try
            {
                if (!ConnectClient("BasicHttpBinding_IQMS", ServerName))
                {
                    if (UseTraceFile)
                    {
                        Trace.WriteLine("In " + this.GetType().Name + ".EnumerateAllCals(): Failed to connect to web service");
                    }
                    return null;
                }

                ServiceInfo QvsService = Client.GetServices(ServiceTypes.QlikViewServer).First();

                CalConfig = Client.GetCALConfiguration(QvsService.ID, CALConfigurationScope.All);

                if (CalConfig != null && UseTraceFile)
                {
                    EstablishTraceLogFile();

                    //Document CALs
                    Trace.WriteLine("\r\nDocument CALs:");
                    Trace.WriteLine("# assigned document CALs:" + CalConfig.DocumentCALs.Assigned);
                    Trace.WriteLine("# in license document CALs:" + CalConfig.DocumentCALs.InLicense);
                    Trace.WriteLine("# limit document CALs:" + CalConfig.DocumentCALs.Limit);

                    //Named CALs
                    Trace.WriteLine("\r\nNamed CALs:");
                    Trace.WriteLine("Identification mode:" + CalConfig.NamedCALs.IdentificationMode);
                    Trace.WriteLine("# assigned named CALs:" + CalConfig.NamedCALs.Assigned);
                    Trace.WriteLine("# in license named CALs:" + CalConfig.NamedCALs.InLicense);
                    Trace.WriteLine("# limit document CALs:" + CalConfig.NamedCALs.Limit);

                    Trace.WriteLine("Assigned named CALs:");
                    foreach (var Cal in CalConfig.NamedCALs.AssignedCALs.OrderBy(c => c.LastUsed))
                    {
                        Trace.WriteLine(String.Format("Named CAL last used {0} assigned to user {1} ", Cal.LastUsed.ToString("yyyy-MM-dd HH:mm:ss"), Cal.UserName));
                    }

                    Trace.WriteLine("Leased named CALs:");
                    foreach (var Cal in CalConfig.NamedCALs.LeasedCALs.OrderBy(c => c.LastUsed))
                    {
                        Trace.WriteLine(String.Format("Named CAL last used {0} leased for user {1} ", Cal.LastUsed.ToString("yyyy-MM-dd HH:mm:ss"), Cal.UserName));
                    }

                    Trace.WriteLine("Removed CALs:");
                    foreach (var Cal in CalConfig.NamedCALs.RemovedAssignedCALs.OrderBy(c => c.LastUsed))
                    {
                        Trace.WriteLine(String.Format("Removed CAL last used {0} for user {1} ", Cal.LastUsed.ToString("yyyy-MM-dd HH:mm:ss"), Cal.UserName));
                    }
                    

                    //Session CALs
                    Trace.WriteLine("\r\nSession CALs:");
                    Trace.WriteLine("# available session CALs:" + CalConfig.SessionCALs.Available);
                    Trace.WriteLine("# in license session CALs:" + CalConfig.SessionCALs.InLicense);
                    Trace.WriteLine("# limit session CALs:" + CalConfig.SessionCALs.Limit);

                    CloseTraceLogFile();
                }
            }
            catch  /*(anything)*/
            { /*Do nothing*/}
            finally
            {
                DisconnectClient();
                CloseTraceLogFile();
            }

            return CalConfig;
        }

        /// <summary>
        /// Removes all named CALs for one user from the Qlikview server if it exists
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="UseTraceFile">Only has an effect if this is instantiated without LifetimeTraceFile argument</param>
        /// <returns>Indicates whether >=1 CAL was removed</returns>
        public bool RemoveOneNamedCal(string UserName, bool UseTraceFile = false)
        {
            if (UseTraceFile)
            {
                EstablishTraceLogFile();
            }

            bool ReturnValue = false;
            try
            {
                if (!ConnectClient("BasicHttpBinding_IQMS", ServerName))
                {
                    Trace.WriteLine("In " + this.GetType().Name + ".RemoveOneNamedCal(): Failed to connect to web service");
                    return false;
                }

                ServiceInfo QvServerInstance = Client.GetServices(ServiceTypes.QlikViewServer).First();
                Client.ClearQVSCache(QVSCacheObjects.All);

                CALConfiguration CalConfig = Client.GetCALConfiguration(QvServerInstance.ID, CALConfigurationScope.NamedCALs);

                List<AssignedNamedCAL> CurrentNamedCALs = CalConfig.NamedCALs.AssignedCALs.ToList();
                List<AssignedNamedCAL> RemovedNamedCALs = new List<AssignedNamedCAL>();

                bool IsDirty = false;
                for (int i = CurrentNamedCALs.Count - 1; i >= 0; i--)
                {
                    //find the user license in the list
                    if ((CurrentNamedCALs[i].QuarantinedUntil < System.DateTime.Now) && (string.Compare(UserName, CurrentNamedCALs[i].UserName, true) == 0))
                    {
                        RemovedNamedCALs.Add(CurrentNamedCALs[i]);
                        CurrentNamedCALs.RemoveAt(i);
                        IsDirty = true;
                    }
                }
                //if we updated, the save config
                if (IsDirty)
                {
                    CalConfig.NamedCALs.Assigned = CurrentNamedCALs.Count();
                    CalConfig.NamedCALs.AssignedCALs = CurrentNamedCALs.ToArray();
                    CalConfig.NamedCALs.RemovedAssignedCALs = RemovedNamedCALs.ToArray();
                    Client.SaveCALConfiguration(CalConfig);
                    ReturnValue = true;
                }
            }
            catch /*anything*/
            {
                Trace.WriteLine("Exception caught while trying to delete named user license for account:" + UserName);
            }
            finally
            {
                DisconnectClient();
                CloseTraceLogFile();
            }

            return ReturnValue;
        }

        /// <summary>
        /// Removes any document CAL assigned to the specified user for the specified document
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="RelativePath">as it is stored in the Qlikview server document meta data</param>
        /// <param name="DocumentName">File name of the document</param>
        /// <param name="UseTraceFile">Only has an effect if this is instantiated without LifetimeTraceFile argument</param>
        /// <returns></returns>
        public bool RemoveOneDocumentCal(string UserName, string RelativePath, string DocumentName, bool UseTraceFile = false)
        {
            if (UseTraceFile)
            {
                EstablishTraceLogFile();
            }

            Trace.WriteLine(string.Format("Attempting to remove user {0} from document {1}", UserName, DocumentName));

            bool ReturnValue = false;
            try
            {
                if (!ConnectClient("BasicHttpBinding_IQMS", ServerName))
                {
                    Trace.WriteLine("In " + this.GetType().Name + ".EnumerateAllCals(): Failed to connect to web service");
                    return false;
                }

                ServiceInfo QvServerInstance = Client.GetServices(ServiceTypes.QlikViewServer).First();
                Client.ClearQVSCache(QVSCacheObjects.All);

                DocumentNode[] AllDocNodes = Client.GetUserDocuments(QvServerInstance.ID);

                int DocTestCounter = 0;

                // Look for the desired DocumentNode
                foreach (DocumentNode DocNode in AllDocNodes)
                {
                    if (DocNode.Name == DocumentName && DocNode.RelativePath == RelativePath)
                    {
                        // This is the document I want to manage.  Get the meta data.
                        DocumentMetaData Meta = Client.GetDocumentMetaData(DocNode, DocumentMetaDataScope.Licensing);

                        //Extract the current list of Document CALs for this document
                        List<AssignedNamedCAL> CurrentCALs = Meta.Licensing.AssignedCALs.ToList();
                        List<AssignedNamedCAL> RemovableCALs = new List<AssignedNamedCAL>();

                        for (int CalIndex = CurrentCALs.Count-1 ; CalIndex >= 0 ; CalIndex--)
                        {
                            //If the user matches the name then remove it from the list
                            if (CurrentCALs[CalIndex].UserName == UserName)
                            {
                                RemovableCALs.Add(CurrentCALs[CalIndex]);  // This line has to come before Remove
                                CurrentCALs.Remove(CurrentCALs[CalIndex]);
                            }
                        }

                        if (RemovableCALs.Count > 0)
                        {
                            // Update the Meta object for this document with Document CAL changes
                            Meta.Licensing.AssignedCALs = CurrentCALs.ToArray();
                            Meta.Licensing.RemovedAssignedCALs = RemovableCALs.ToArray();
                            Meta.Licensing.CALsAllocated = CurrentCALs.Count;

                            //Save the metadata back to the server
                            Client.SaveDocumentMetaData(Meta);

                            ReturnValue = true;
                        }
                    }
                    DocTestCounter++;
                }

            }
            catch (System.Exception e)
            {
                Trace.WriteLine("Exception in QlikviewCalManager.RemoveOneDocumentCal()!  Failed to delete named user license for account:" + UserName + "\r\n" + e.Message + "\r\n" + e.StackTrace);
            }
            finally
            {
                DisconnectClient();
                CloseTraceLogFile();
            }

            return ReturnValue;
        }

        /// <summary>
        /// Returns a collection of all document cals known to the Qlikview server. 
        /// </summary>
        /// <param name="UseTraceFile">Only has an effect if this is instantiated without LifetimeTraceFile argument</param>
        /// <param name="MaxNumber"></param>
        /// <param name="AllowSelectionOfUndated"></param>
        /// <param name="MinimumAgeHours"></param>
        /// <returns></returns>
        public List<DocCalEntry> EnumerateDocumentCals(bool UseTraceFile, int MaxNumber, bool AllowSelectionOfUndated, int MinimumAgeHours)
        {
            List<DocCalEntry> ReturnValue = null;

            if (UseTraceFile)
            {
                EstablishTraceLogFile();
            }

            try
            {
                Trace.WriteLine("connecting with servername " + ServerName);
                if (!ConnectClient("BasicHttpBinding_IQMS", ServerName))
                {
                    if (UseTraceFile)
                    {
                        Trace.WriteLine("In " + this.GetType().Name + ".EnumerateAllCals(): Failed to connect to web service at URI " + ServerName);
                        CloseTraceLogFile();
                        return ReturnValue;
                    }
                }

                ReturnValue = new List<DocCalEntry>();

                ServiceInfo QvServerInstance = Client.GetServices(ServiceTypes.QlikViewServer).First();
                DocumentNode[] AllDocNodes = Client.GetUserDocuments(QvServerInstance.ID);

                // Look for the desired DocumentNode
                foreach (DocumentNode DocNode in AllDocNodes)
                {
                    // Get the document meta data.
                    DocumentMetaData Meta = Client.GetDocumentMetaData(DocNode, DocumentMetaDataScope.Licensing);

                    //Extract the current list of Document CALs for this document
                    List<AssignedNamedCAL> CurrentCALs = Meta.Licensing.AssignedCALs.ToList();

                    for (int CalIndex = CurrentCALs.Count - 1; CalIndex >= 0; CalIndex--)
                    {
                        ReturnValue.Add(new DocCalEntry { DocumentName = DocNode.Name, RelativePath = DocNode.RelativePath, UserName = CurrentCALs[CalIndex].UserName, LastUsedDateTime = CurrentCALs[CalIndex].LastUsed, DeleteFlag = false });
                    }
                }

                ReturnValue = FlagDocCalsForDelete(ReturnValue, MaxNumber, AllowSelectionOfUndated, new TimeSpan(MinimumAgeHours, 0, 0));
            }
            catch 
            {
                Trace.WriteLine("Exception caught in QlikviewCalManager.EnumerateDocumentCals()");
            }
            finally
            {
                DisconnectClient();
                CloseTraceLogFile();
            }

            return ReturnValue;
        }

        /// <summary>
        /// Adds delete flags to a supplied List<DocCalEntry> with bounding rules supplied by arguments
        /// </summary>
        /// <param name="CandidateList"></param>
        /// <param name="MaxNumber"></param>
        /// <param name="AllowSelectionOfUndated"></param>
        /// <param name="MustBeAtLeastThisOld"></param>
        /// <returns></returns>
        private List<DocCalEntry> FlagDocCalsForDelete(List<DocCalEntry> CandidateList, int MaxNumber, bool AllowSelectionOfUndated, TimeSpan MustBeAtLeastThisOld)
        {
            List<DocCalEntry> ReturnList = CandidateList;

            List<DocCalEntry> FlaggedItems = new List<DocCalEntry>();
            foreach (DocCalEntry Entry in CandidateList.OrderBy(e => e.LastUsedDateTime))
            {
                if (FlaggedItems.Count >= MaxNumber)
                {
                    break;
                }
                if (AllowSelectionOfUndated || Entry.LastUsedDateTime != new DateTime())
                {
                    if (DateTime.Now - Entry.LastUsedDateTime > MustBeAtLeastThisOld)
                    {
                        FlaggedItems.Add(Entry);
                    }
                }
            }

            foreach (DocCalEntry FlaggableEntry in FlaggedItems)
            {
                int Index = ReturnList.IndexOf(FlaggableEntry);
                DocCalEntry ReplacementEntry = FlaggableEntry;
                ReplacementEntry.DeleteFlag = true;
                ReturnList.RemoveAt(Index);
                ReturnList.Insert(Index, ReplacementEntry);
            }

            return ReturnList;
        }

        /// <summary>
        /// Returns a list of named CALs known to the Qlikview server
        /// </summary>
        /// <param name="SelectLimit"></param>
        /// <param name="AllowUndatedCalSelection"></param>
        /// <param name="MinimumAgeHours"></param>
        /// <param name="UseTraceFile">Only has an effect if this is instantiated without LifetimeTraceFile argument</param>
        /// <returns></returns>
        public List<NamedCalEntry> EnumerateNamedCals(int SelectLimit, bool AllowUndatedCalSelection, int MinimumAgeHours, bool UseTraceFile)
        {
            CALConfiguration CalConfig = null;
            List<NamedCalEntry> ReturnList = new List<NamedCalEntry>();

            if (UseTraceFile)
            {
                EstablishTraceLogFile();
            }

            try
            {
                if (!ConnectClient("BasicHttpBinding_IQMS", ServerName))
                {
                    Trace.WriteLine("In " + this.GetType().Name + ".EnumerateAllCals(): Failed to connect to web service");
                    CloseTraceLogFile();
                    return null;
                }

                ServiceInfo QvsService = Client.GetServices(ServiceTypes.QlikViewServer).First();
                CalConfig = Client.GetCALConfiguration(QvsService.ID, CALConfigurationScope.All);

                if (CalConfig != null)
                {
                    foreach (var Cal in CalConfig.NamedCALs.AssignedCALs.OrderBy(c => c.LastUsed))
                    {
                        ReturnList.Add(new NamedCalEntry { UserName = Cal.UserName, LastUsedDateTime = Cal.LastUsed });
                        Trace.WriteLine("Found NamedCAL for UserName " + Cal.UserName + " last used " + Cal.LastUsed.ToString("yyyy-MM-dd HH:mm:ss"));
                    }

                    ReturnList = FlagNamedCalsForDelete(ReturnList, SelectLimit, AllowUndatedCalSelection, new TimeSpan(MinimumAgeHours, 0, 0));
                }
            }
            catch  /*(anything)*/
            { /*Do nothing*/}
            finally
            {
                DisconnectClient();
                CloseTraceLogFile();
            }

            return ReturnList;
        }

        /// <summary>
        /// Adds delete flags to a supplied List<NamedCalEntry> with bounding rules supplied by arguments
        /// </summary>
        /// <param name="CandidateList"></param>
        /// <param name="MaxNumber"></param>
        /// <param name="AllowSelectionOfUndated"></param>
        /// <returns></returns>
        private List<NamedCalEntry> FlagNamedCalsForDelete(List<NamedCalEntry> CandidateList, int MaxNumber, bool AllowSelectionOfUndated, TimeSpan MustBeAtLeastThisOld)
        {
            List<NamedCalEntry> ReturnList = CandidateList;

            List<NamedCalEntry> FlaggedItems = new List<NamedCalEntry>();
            foreach (NamedCalEntry Entry in CandidateList.OrderBy(e => e.LastUsedDateTime))
            {
                if (FlaggedItems.Count >= MaxNumber)
                {
                    break;
                }
                if (AllowSelectionOfUndated || Entry.LastUsedDateTime != new DateTime())
                {
                    if (DateTime.Now - Entry.LastUsedDateTime > MustBeAtLeastThisOld)
                    {
                        FlaggedItems.Add(Entry);
                    }
                }
            }

            foreach (NamedCalEntry FlaggableEntry in FlaggedItems)
            {
                Trace.WriteLine("Flagging entry for delete, user " + FlaggableEntry.UserName + " last used " + FlaggableEntry.LastUsedDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                int Index = ReturnList.IndexOf(FlaggableEntry);
                NamedCalEntry ReplacementEntry = FlaggableEntry;
                ReplacementEntry.DeleteFlag = true;
                ReturnList.RemoveAt(Index);
                ReturnList.Insert(Index, ReplacementEntry);
            }

            return ReturnList;
        }

    }
}
