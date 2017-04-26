/*
 * CODE OWNERS: Tom Puckett, Ben Wyatt
 * OBJECTIVE: Dedicated class to process tracking of Qlikview CALs.
 * DEVELOPER NOTES: Note that this is derived from ServerMonitorProcessingBase, as all future processing classes should be
 */

using System;
using System.IO;
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
        public enum CalStatisticField
        {
            NamedCalAssigned,
            NamedCalLimit,
            DocumentCalAssigned,
            DocumentCalLimit
        }

        /// <summary>
        /// Dumps a single caller selectable CAL related statistic value to trace log.  Intended mainly for use in non-interactive mode.  
        /// </summary>
        /// <param name="FieldToReport"></param>
        public void ReportCalStatistic(CalStatisticField FieldToReport)
        {
            EstablishTraceLogFile();

            CALConfiguration CalConfig = EnumerateAllCals(false);
            if (CalConfig == null)
            {
                Trace.WriteLine("QlikviewCalManager.EnumerateAllCals() returned null");
                return;
            }

            switch (FieldToReport)
            {
                case CalStatisticField.DocumentCalAssigned:
                    Trace.WriteLine(CalConfig.DocumentCALs.Assigned);
                    break;
                case CalStatisticField.DocumentCalLimit:
                    Trace.WriteLine(CalConfig.DocumentCALs.Limit);
                    break;
                case CalStatisticField.NamedCalAssigned:
                    Trace.WriteLine(CalConfig.NamedCALs.Assigned);
                    break;
                case CalStatisticField.NamedCalLimit:
                    Trace.WriteLine(CalConfig.NamedCALs.Limit);
                    break;
            }

            CloseTraceLogFile();
        }

        /// <summary>
        /// Obtains the CALConfiguration object representing the server configuration and optionally reports certain statistics to the trace log.  
        /// </summary>
        /// <param name="TraceOutput">true if trace log is to be written with CAL statistics</param>
        /// <returns>null if operation failed</returns>
        public CALConfiguration EnumerateAllCals(bool TraceOutput = false)
        {
            CALConfiguration CalConfig = null;

            try
            {
                if (!ConnectClient("BasicHttpBinding_IQMS", @"http://localhost:4799/QMS/Service"))
                {
                    if (TraceOutput)
                    {
                        Trace.WriteLine("In " + this.GetType().Name + ".EnumerateAllCals(): Failed to connect to web service");
                    }
                    return null;
                }

                ServiceInfo QvsService = Client.GetServices(ServiceTypes.QlikViewServer).First();

                CalConfig = Client.GetCALConfiguration(QvsService.ID, CALConfigurationScope.All);

                if (CalConfig != null && TraceOutput)
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
            }

            return CalConfig;
        }

        /// <summary>
        /// Removes all named CALs for one user from the Qlikview server if it exists
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="TraceFile"></param>
        /// <returns>Indicates whether >=1 CAL was removed</returns>
        public bool RemoveOneNamedCal(string UserName, bool TraceFile = false)
        {
            if (TraceFile)
            {
                EstablishTraceLogFile();
            }

            bool ReturnValue = false;
            try
            {
                if (!ConnectClient("BasicHttpBinding_IQMS", @"http://localhost:4799/QMS/Service"))
                {
                    Trace.WriteLine("In " + this.GetType().Name + ".EnumerateAllCals(): Failed to connect to web service");
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

        public bool RemoveOneDocumentCal(string UserName, string DocumentName, bool TraceFile = false)
        {
            if (TraceFile)
            {
                EstablishTraceLogFile();
            }

            bool ReturnValue = false;
            try
            {
                if (!ConnectClient("BasicHttpBinding_IQMS", @"http://localhost:4799/QMS/Service"))
                {
                    Trace.WriteLine("In " + this.GetType().Name + ".EnumerateAllCals(): Failed to connect to web service");
                    return false;
                }

                ServiceInfo QvServerInstance = Client.GetServices(ServiceTypes.QlikViewServer).First();
                Client.ClearQVSCache(QVSCacheObjects.All);

                DocumentNode[] AllDocNodes = Client.GetUserDocuments(QvServerInstance.ID);

                // Look for the desired DocumentNode
                foreach (DocumentNode DocNode in AllDocNodes)
                {
                    if (DocNode.Name == DocumentName)
                    {
                        Trace.WriteLine("Found DocNode for document " + DocNode.Name);

                        // This is the document I want to manage.  Get the meta data.
                        DocumentMetaData Meta = Client.GetDocumentMetaData(DocNode, DocumentMetaDataScope.Licensing);

                        //Extract the current list of Document CALs for this document
                        List<AssignedNamedCAL> CurrentCALs = Meta.Licensing.AssignedCALs.ToList();

                        Trace.WriteLine("Document has " + CurrentCALs.Count + " CALS");

                        for (int CalIndex = CurrentCALs.Count-1 ; CalIndex >= 0 ; CalIndex--)
                        {
                            //If the user matches the name then remove it from the list
                            if (CurrentCALs[CalIndex].UserName == UserName)
                            {
                                Trace.WriteLine("Found user " + CurrentCALs[CalIndex].UserName + " with a document CAL");

                                bool RemoveSuccess = CurrentCALs.Remove(CurrentCALs[CalIndex]);

                                Trace.WriteLine("Removed user " + CurrentCALs[CalIndex].UserName + " with result " + RemoveSuccess.ToString());
                            }
                        }

                        //Add the updated CALs list back to the meta data object
                        Meta.Licensing.AssignedCALs = CurrentCALs.ToArray();
                        //Save the metadata back to the server
//                        Client.SaveDocumentMetaData(Meta);
                    }

                }

            }
            catch /*anything*/
            {
                Trace.WriteLine("Failed to delete named user license for account:" + UserName);
            }
            finally
            {
                DisconnectClient();
                CloseTraceLogFile();
            }

            return ReturnValue;
        }

        public bool EnumerateDocumentCals(bool TraceFile)
        {
            if (TraceFile)
            {
                EstablishTraceLogFile();
            }

            try
            {
                if (!ConnectClient("BasicHttpBinding_IQMS", @"http://localhost:4799/QMS/Service"))
                {
                    if (TraceFile)
                    {
                        Trace.WriteLine("In " + this.GetType().Name + ".EnumerateAllCals(): Failed to connect to web service");
                    }
                    return false;
                }

                ServiceInfo QvServerInstance = Client.GetServices(ServiceTypes.QlikViewServer).First();

                DocumentNode[] AllDocNodes = Client.GetUserDocuments(QvServerInstance.ID);

                // TODO DeleteMe
                System.Xml.Serialization.XmlSerializer S = new System.Xml.Serialization.XmlSerializer(typeof(DocumentMetaData));

                // Look for the desired DocumentNode
                foreach (DocumentNode DocNode in AllDocNodes)
                {
                    // TODO Delete
                    if (DocNode.IsSubFolder) Trace.WriteLine("!!!Subfolder! " + DocNode.Name);

                    Trace.WriteLine("Evaluating document " + Path.Combine(DocNode.RelativePath, DocNode.Name));

                    // Get the document meta data.
                    DocumentMetaData Meta = Client.GetDocumentMetaData(DocNode, DocumentMetaDataScope.Licensing);

                    // TODO DeleteMe
                    using (StreamWriter W = new StreamWriter(DocNode.Name + "_Meta.txt", false))
                    {
                        S.Serialize(W, Meta);
                    }

                    //Extract the current list of Document CALs for this document
                    List<AssignedNamedCAL> CurrentCALs = Meta.Licensing.AssignedCALs.ToList();

                    for (int CalIndex = CurrentCALs.Count - 1; CalIndex >= 0; CalIndex--)
                    {
                        Trace.WriteLine("    Document CAL last used " + CurrentCALs[CalIndex].LastUsed.ToString("yyyy-MM-dd HH:mm:ss") + " found for user " + CurrentCALs[CalIndex].UserName);
                    }

                }
            }
            catch  /*(anything)*/
            { /*Do nothing*/}
            finally
            {
                DisconnectClient();
                CloseTraceLogFile();
            }

            return true;
        }
    }
}
