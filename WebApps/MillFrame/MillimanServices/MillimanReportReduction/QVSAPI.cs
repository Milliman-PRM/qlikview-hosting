using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MillimanReportReduction.QMSAPIService;
using MillimanReportReduction.ServiceSupport;


namespace MillimanReportReduction
{
    public class QVSAPI
    {

        public string AuthorizeAllDocuments(string VerifyForQVW, out bool Verified)
        {
            Verified = false;
            try
            {
                QMSClient Client;
                string QMS = "http://localhost:4799/QMS/Service";
                Client = new QMSClient("BasicHttpBinding_IQMS", QMS);

                string key = Client.GetTimeLimitedServiceKey();
                ServiceKeyClientMessageInspector.ServiceKey = key;
               
                ServiceInfo[] MyQVS = Client.GetServices(ServiceTypes.QlikViewServer);
                Client.ClearQVSCache(QVSCacheObjects.UserDocumentList);
                DocumentNode[] allDocs = Client.GetUserDocuments(MyQVS[0].ID);
                
                DocumentMetaData Meta;
                for (int i = 0; i < allDocs.Length; i++)
                {
                    Meta = Client.GetDocumentMetaData(allDocs[i], DocumentMetaDataScope.Authorization);
                    //don't keep adding the same authorization if already authorized
                    if (Meta.Authorization.Access.Any(x => x.UserName == "") == false)
                    {
                        DocumentAccessEntry user = new DocumentAccessEntry();
                        user.UserName = "";
                        user.AccessMode = DocumentAccessEntryMode.Always;
                        user.DayOfWeekConstraints = new DayOfWeek[0];
                        List<DocumentAccessEntry> DAL = Meta.Authorization.Access.ToList();
                        DAL.Add(user);
                        Meta.Authorization.Access = DAL.ToArray();
                        Client.SaveDocumentMetaData(Meta);
                        if ((Meta.UserDocument != null) && (string.IsNullOrEmpty(Meta.UserDocument.Name) == false))
                        {
                            MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Info, "Document '" + Meta.UserDocument.Name + "' has been authorized");
                            if ((Meta.UserDocument.Name.IndexOf(VerifyForQVW) != -1) || (string.IsNullOrEmpty(VerifyForQVW)))
                                Verified = true;
                        }
                        //List<AssignedNamedCAL> currentCALs = Meta.Licensing.AssignedCALs.ToList();
                        //for (int a = 0; a < currentCALs.Count; a++)
                        //{
                        //    string S = Meta.UserDocument.Name + ", " + currentCALs[a].UserName + ", " + currentCALs[a].LastUsed;
                        //}
                    }
                    else if ((Meta.UserDocument.Name.IndexOf(VerifyForQVW) != -1) || (string.IsNullOrEmpty(VerifyForQVW)))
                        Verified = true;  //we have already set permissions
                }
                Client.Close();
                Client = null;
            }
            catch (System.Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, ex.ToString());
                return ex.ToString();
            }
            return "";
        }
    }
}