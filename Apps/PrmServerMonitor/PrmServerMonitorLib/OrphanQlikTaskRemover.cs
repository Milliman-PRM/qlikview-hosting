/*
 * CODE OWNERS: Tom Puckett, Ben Wyatt
 * OBJECTIVE: A dedicated class for removing orphaned Qlikview server tasks from the local server
 * DEVELOPER NOTES: Note that this is derived from ServerMonitorProcessingBase, as all future processing classes should be
 */

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using PrmServerMonitorLib.Qms;
using PrmServerMonitorLib.ServiceSupport;

namespace PrmServerMonitorLib
{
    /// <summary>
    /// Constructor, passes arguments to the base class
    /// </summary>
    public class OrphanQlikTaskRemover : QlikviewProcessingBase
    {
        public OrphanQlikTaskRemover(string ServerNameArg = "localhost", bool LifetimeTraceArg = false) : base(ServerNameArg, LifetimeTraceArg)
        { }

        /// <summary>
        /// Destructor
        /// </summary>
        ~OrphanQlikTaskRemover()
        {
            CloseTraceLogFile(true);
        }

        /// <summary>
        /// Button handler that initiates cleanup of orphaned documents
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveOrphanTasks(bool LogToFile)
        {
            // the project setup for a QMS client application can be found at https://community.qlik.com/docs/DOC-2639

            if (LogToFile)
            {
                EstablishTraceLogFile();
            }

            try
            {
                if (!ConnectClient("BasicHttpBinding_IQMS", ServerName))
                {
                    Trace.WriteLine("In " + this.GetType().Name + ".RemoveOrphanTasks(): Failed to connect to web service");
                    return;
                }

                ServiceInfo[] myServices = Client.GetServices(ServiceTypes.QlikViewDistributionService);
                foreach (ServiceInfo service in myServices)
                {
                    DocumentFolder[] sourceDocumentsFolders = Client.GetSourceDocumentFolders(service.ID, DocumentFolderScope.General | DocumentFolderScope.Services);

                    // loop through all source document folders (expecting 1 but handle any number)
                    foreach (DocumentFolder sourceDocumentFolder in sourceDocumentsFolders.OrderBy(x => x.General.Path))
                    {
                        Trace.WriteLine("Starting to process Source Document Folder " + sourceDocumentFolder.General.Path);
                        DeleteTasksForOrphanDocuments(Client, sourceDocumentFolder, string.Empty);
                    }

                }
            }
            catch (System.Exception e)
            {
                Trace.WriteLine("Exception caught:\r\n" + e.Message + "\r\n" + e.StackTrace);
            }
            finally
            {
                DisconnectClient();
                CloseTraceLogFile();
            }

        }

        /// <summary>
        /// Recursively finds all orphan DocumentNodes under the specified path and deletes all tasks for those nodes.  
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="SourceDocumentFolder"></param>
        /// <param name="RelativePath"></param>
        private void DeleteTasksForOrphanDocuments(IQMS Client, DocumentFolder SourceDocumentFolder, string RelativePath)
        {
            // retrieve all source document nodes of the given folder and under the specified relative path
            List<DocumentNode> SourceDocumentNodes = Client.GetSourceDocumentNodes(SourceDocumentFolder.Services.QDSID, SourceDocumentFolder.ID, RelativePath).Where(node => node.IsOrphan).ToList();

            if (SourceDocumentNodes.Count == 0)
            {
                Trace.WriteLine("No document nodes found in path " + SourceDocumentFolder.General.Path + RelativePath);
            }

            foreach (DocumentNode SourceDocumentNode in SourceDocumentNodes.OrderByDescending(x => x.IsSubFolder).ThenBy(x => x.Name))
            {
                Trace.WriteLine("Evaluating path " + SourceDocumentNode.Name);

                if (SourceDocumentNode.IsSubFolder)
                {
                    Trace.WriteLine("Node is a folder, recursing...");

                    DeleteTasksForOrphanDocuments(Client, SourceDocumentFolder, RelativePath + Path.DirectorySeparatorChar + SourceDocumentNode.Name);
                }
                else
                {
                    TaskInfo[] DocumentTasks = Client.GetTasksForDocument(SourceDocumentNode.ID);
                    Trace.WriteLine("Found " + DocumentTasks.Length + " tasks in this path");

                    foreach (TaskInfo Task in DocumentTasks)
                    {
                        Trace.WriteLine("Deleting task named " + Task.Name + ", ID " + Task.ID);
                        bool Success = Client.DeleteTask(Task.ID, Task.Type);
                        if (!Success)
                        {
                            Trace.WriteLine("DeleteTask failed!");
                        }
                    }
                }
            }
        }
    }
}
