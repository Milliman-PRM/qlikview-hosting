using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrmServerMonitor.Qms;
using PrmServerMonitor.ServiceSupport;

namespace PrmServerMonitor
{
    public class OrphanQlikTaskRemover
    {
        /// <summary>
        /// Button handler that initiates cleanup of orphaned documents
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveOrphanTasks()
        {
            // the project setup for a QMS client application can be found at https://community.qlik.com/docs/DOC-2639

            TextWriterTraceListener TraceFile = new TextWriterTraceListener("Trace_OrphanTaskRemoval_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".log");
            Trace.AutoFlush = true;
            Trace.Listeners.Add(TraceFile);

            try
            {
                QMSClient Client;

                string QMS = "http://localhost:4799/QMS/Service";
                Client = new QMSClient("BasicHttpBinding_IQMS", QMS);
                string key = Client.GetTimeLimitedServiceKey();
                ServiceKeyClientMessageInspector.ServiceKey = key;

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
                TraceFile.Flush();
                Trace.Listeners.Remove(TraceFile);
                TraceFile.Close();
                TraceFile = null;
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
                Trace.WriteLine("No document nodes found in path " + SourceDocumentFolder.General.Path + Path.DirectorySeparatorChar + RelativePath);
            }

            foreach (DocumentNode SourceDocumentNode in SourceDocumentNodes.OrderByDescending(x => x.IsSubFolder).ThenBy(x => x.Name))
            {
                Trace.WriteLine("Evaluating path " + SourceDocumentNode.Name);

                if (SourceDocumentNode.IsSubFolder)
                {
                    Trace.WriteLine("Node is a folder, recursing...");

                    DeleteTasksForOrphanDocuments(Client, SourceDocumentFolder, RelativePath + "\\" + SourceDocumentNode.Name);
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
