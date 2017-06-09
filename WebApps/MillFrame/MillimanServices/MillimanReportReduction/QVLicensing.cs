using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MillimanReportReduction.QMSAPIService;
using MillimanReportReduction.ServiceSupport;

namespace MillimanReportReduction
{
    public class QVLicensing
    {
        public enum LicenseTypeEnum { NAMED, DOCUMENT, ANY, NONE };
        public class LicenseInfo
        {
 
            public LicenseTypeEnum License { get; set; }
            public string User { get; set; }
            public DateTime LastAccessed { get; set; }
            public string DocumentName { get; set; }
            public LicenseInfo(LicenseTypeEnum _License, string _User, DateTime _LastAccessed, string _DocumentName)
            {
                License = _License;
                User = _User;
                LastAccessed = _LastAccessed;
                DocumentName = _DocumentName;
            }
        }

        /// <summary>
        /// we have to prefix the names as custom
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private string AddUserDirCon(string UserName)
        {
            if ( UserName.ToLower().IndexOf("custom") == -1 )
                return @"CUSTOM\" + UserName.ToUpper();
            return UserName;
        }
        /// <summary>
        /// Get a list of all license in use, doc cals will contain the user and document, while name cals will
        /// just contain the user name since any number of documents could be accessed
        /// </summary>
        /// <param name="OfType"></param>
        /// <returns></returns>
        public List<LicenseInfo> GetAllCALSOfTypeInUse( LicenseTypeEnum OfType)
        {
            List<LicenseInfo> Info = new List<LicenseInfo>();
            try
            {

                QMSClient Client;
                string QMS = "http://localhost:4799/QMS/Service";
                Client = new QMSClient("BasicHttpBinding_IQMS", QMS);
                string key = Client.GetTimeLimitedServiceKey();
                ServiceKeyClientMessageInspector.ServiceKey = key;
                ServiceInfo[] MyQVS = Client.GetServices(ServiceTypes.QlikViewServer);

                if ((OfType == LicenseTypeEnum.ANY) || (OfType == LicenseTypeEnum.DOCUMENT))
                {
                    DocumentNode[] allDocs = Client.GetUserDocuments(MyQVS[0].ID);
                    DocumentMetaData Meta;
                    for (int i = 0; i < allDocs.Length; i++)
                    {
                        Meta = Client.GetDocumentMetaData(allDocs[i], DocumentMetaDataScope.Licensing);
                        List<AssignedNamedCAL> currentCALs = Meta.Licensing.AssignedCALs.ToList();
                        for (int a = 0; a < currentCALs.Count; a++)
                        {
                            Info.Add(new LicenseInfo(LicenseTypeEnum.DOCUMENT, currentCALs[a].UserName, currentCALs[a].LastUsed, System.IO.Path.Combine( Meta.UserDocument.RelativePath, Meta.UserDocument.Name)));
                        }
                    }
                }
                if ((OfType == LicenseTypeEnum.ANY) || (OfType == LicenseTypeEnum.NAMED))
                {
                    CALConfiguration myCALs = Client.GetCALConfiguration(MyQVS[0].ID, CALConfigurationScope.NamedCALs);
                    List<AssignedNamedCAL> currentCALs = myCALs.NamedCALs.AssignedCALs.ToList();
                    for (int i = 0; i < currentCALs.Count; i++)
                    {
                        Info.Add( new LicenseInfo(LicenseTypeEnum.NAMED, currentCALs[i].UserName, currentCALs[i].LastUsed, "" ));
                    }
                }
            }
            catch (System.Exception)
            {
                return null;  //return null if failed
            }

            return Info;
        }

        /// <summary>
        /// Check to see if the user has a named license already
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public bool UserHasNamedCAL(string User)
        {
            QMSClient Client = null;
            try
            {
                User = AddUserDirCon(User);

                string QMS = "http://localhost:4799/QMS/Service";
                Client = new QMSClient("BasicHttpBinding_IQMS", QMS);
                string key = Client.GetTimeLimitedServiceKey();
                ServiceKeyClientMessageInspector.ServiceKey = key;
                ServiceInfo[] MyQVS = Client.GetServices(ServiceTypes.QlikViewServer);

                CALConfiguration myCALs = Client.GetCALConfiguration(MyQVS[0].ID, CALConfigurationScope.NamedCALs);
                List<AssignedNamedCAL> currentCALs = myCALs.NamedCALs.AssignedCALs.ToList();
                foreach (AssignedNamedCAL ANC in currentCALs)
                {
                    if (string.Compare(ANC.UserName, User, true) == 0)
                    {
                        Client.Close();
                        Client = null;
                        return true; // user already has a named license
                    }
                }
                Client.Close();
                Client = null;
                return false;
            }
            catch (System.Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed license access", ex);
                if (Client != null)
                    Client.Close();
            }
            return false;  //return false to keep from giving up licenses blindly
        }

        /// <summary>
        /// Assign the user  a named license
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public bool AssignUserNamedCAL(string User)
        {
            QMSClient Client = null;
            try
            {
                //if they already have a named CAL return true
                if (UserHasNamedCAL(User) == true)
                    return true;

                User = AddUserDirCon(User); //prefix the user name with Custom i.e. Custom\van.nanney@milliman.com
                //get the service key
                string QMS = "http://localhost:4799/QMS/Service";
                Client = new QMSClient("BasicHttpBinding_IQMS", QMS);
                string key = Client.GetTimeLimitedServiceKey();
                ServiceKeyClientMessageInspector.ServiceKey = key;
                ServiceInfo[] MyQVS = Client.GetServices(ServiceTypes.QlikViewServer);
                //clear out the cache
                Client.ClearQVSCache(QVSCacheObjects.All);

                //get the named CALs
                CALConfiguration myCALs = Client.GetCALConfiguration(MyQVS[0].ID, CALConfigurationScope.NamedCALs);
                List<AssignedNamedCAL> currentCALs = myCALs.NamedCALs.AssignedCALs.ToList();
                //create a new named CAL and add to list
                AssignedNamedCAL newCAL = new AssignedNamedCAL();
                newCAL.UserName = User;

                currentCALs.Add(newCAL);
                myCALs.NamedCALs.AssignedCALs = currentCALs.ToArray();
                myCALs.NamedCALs.Assigned = currentCALs.Count();

                //save the modified configuration
                Client.SaveCALConfiguration(myCALs);

                //clear the cache again
                Client.ClearQVSCache(QVSCacheObjects.License);
                
                //verify/check and see what was saved
                CALConfiguration myCALs2 = Client.GetCALConfiguration(MyQVS[0].ID, CALConfigurationScope.NamedCALs);
                List<AssignedNamedCAL> currentCALs2 = myCALs.NamedCALs.AssignedCALs.ToList();
                
                //close out nicely
                Client.Close();
                Client = null;
                return true;
            }
            catch (System.Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed license access", ex);
                if (Client != null)
                    Client.Close();
            }
            return false;  //return false to keep from giving up licenses blindly
        }

        /// <summary>
        /// Assign the user a license to access the the specific documents requested
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Documents"></param>
        /// <returns></returns>
        public bool AssignUserDocCAL(string User, List<string> Documents)
        {
            QMSClient Client = null;
            try
            {
                //if they already have a named CAL return true
                //if (UserHasNamedCAL(User) == true)
                //    return true;

                User = AddUserDirCon(User);
                
                string QMS = "http://localhost:4799/QMS/Service";
                Client = new QMSClient("BasicHttpBinding_IQMS", QMS);
                string key = Client.GetTimeLimitedServiceKey();
                ServiceKeyClientMessageInspector.ServiceKey = key;
                Client.ClearQVSCache(QVSCacheObjects.All);
                ServiceInfo[] MyQVS = Client.GetServices(ServiceTypes.QlikViewServer);
                DocumentNode[] allDocs = Client.GetUserDocuments(MyQVS[0].ID);
                DocumentMetaData Meta;
                
                for (int i = 0; i < allDocs.Length; i++)
                {
                    foreach (string Document in Documents)
                    {
                        string QualifiedQVW = System.IO.Path.Combine(allDocs[i].RelativePath, allDocs[i].Name).ToLower();
                        //document is prefixed by mounted folder, which we cannot get via the GetUserDocuments call,
                        //thus we do a index of to find match excluding mounted folder and minus QVW prefix
                        if (Document.ToLower().IndexOf(QualifiedQVW) >= 0 )  //If the document matches the one im looking for then apply the changes
                        {
                            //Get the meta data for the current document
                            Meta = Client.GetDocumentMetaData(allDocs[i], DocumentMetaDataScope.Licensing);
                            //Extract the current list of Document CALs
                            List<AssignedNamedCAL> currentCALs = Meta.Licensing.AssignedCALs.ToList();
           
                            foreach (AssignedNamedCAL ANC in currentCALs)
                            {
                                if (string.Compare(ANC.UserName, User, true) == 0)
                                {
                                    Client.Close();
                                    Client = null;
                                    return true; //already has a doc license for this, dont assign another
                                }
                            }

                            //bump what is allowed
                            if (currentCALs.Count() >= Meta.Licensing.CALsAllocated)
                                Meta.Licensing.CALsAllocated = Meta.Licensing.CALsAllocated + 1;

                            //Create a new CAL object and set the username for it
                            AssignedNamedCAL newCAL = new AssignedNamedCAL();
                            newCAL.UserName = User;
                            newCAL.LastUsed = DateTime.Now;
                            newCAL.QuarantinedUntil = DateTime.Now.AddDays(1);
                            newCAL.MachineID = "";
                          
                            //Add the new cal to the list of CALs object
                            currentCALs.Add(newCAL);
                            //Add the updated CALs list back to the meta data object
                            Meta.Licensing.AssignedCALs = currentCALs.ToArray();
                            //Save the metadata back to the server
                            Client.SaveDocumentMetaData(Meta);
                        }
                    }
                }


                Client.Close();
                Client = null;
                return true;
            }
            catch (System.Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed license access", ex);
                if (Client == null)
                    Client.Close();
            }
            return false;  //return false to keep from giving up licenses blindly
        }

        /// <summary>
        /// Loop over all license and purge any that are beyond quarantine
        /// </summary>
        public void CleanLicenses()
        {
            QMSClient Client = null;
            try
            {
                string QMS = "http://localhost:4799/QMS/Service";
                Client = new QMSClient("BasicHttpBinding_IQMS", QMS);
                string key = Client.GetTimeLimitedServiceKey();
                ServiceKeyClientMessageInspector.ServiceKey = key;
                ServiceInfo[] MyQVS = Client.GetServices(ServiceTypes.QlikViewServer);
                Client.ClearQVSCache(QVSCacheObjects.All);
                //purge doc licenses
                DocumentNode[] allDocs = Client.GetUserDocuments(MyQVS[0].ID);
                DocumentMetaData Meta;
                for (int i = 0; i < allDocs.Length; i++)  //Loop through each document
                {
                        //Get the meta data for the current document
                        Meta = Client.GetDocumentMetaData(allDocs[i], DocumentMetaDataScope.Licensing);
                        //Extract the current list of Document CALs
                        List<AssignedNamedCAL> currentCALs = Meta.Licensing.AssignedCALs.ToList();
                        for (int x = currentCALs.Count-1; x >= 0; x--)
                        {
                            //If the user matches the name then remove it from the list
                            if (currentCALs[x].QuarantinedUntil < System.DateTime.Now)
                                currentCALs.Remove(currentCALs[x]);
                        }
                        //Add the updated CALs list back to the meta data object
                        Meta.Licensing.AssignedCALs = currentCALs.ToArray();
                        //Save the metadata back to the server
                        Client.SaveDocumentMetaData(Meta);
                }

                CALConfiguration myCALs = Client.GetCALConfiguration(MyQVS[0].ID, CALConfigurationScope.NamedCALs);
                List<AssignedNamedCAL> currentNamedCALs = myCALs.NamedCALs.AssignedCALs.ToList();
                for (int i = currentNamedCALs.Count-1; i >= 0; i--)
                {
                    if (currentNamedCALs[i].QuarantinedUntil < System.DateTime.Now)
                        currentNamedCALs.Remove(currentNamedCALs[i]);
                }
                Client.SaveCALConfiguration(myCALs);
                Client.Close();
                Client = null;
            }
            catch (System.Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to purge QV licenses", ex);
                if (Client != null)
                    Client.Close();
            }
        }

        /// <summary>
        /// Remove the user's document cal association with 'document'
        /// Note: this method should be used with data acquired via method GetAllCALSOfTypeInUse
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Document"></param>
        /// <returns></returns>
        public bool RemoveUserDocCAL( string User, string Document )
        {
            QMSClient Client = null;
            try
            {
                string QMS = "http://localhost:4799/QMS/Service";
                Client = new QMSClient("BasicHttpBinding_IQMS", QMS);
                string key = Client.GetTimeLimitedServiceKey();
                ServiceKeyClientMessageInspector.ServiceKey = key;
                ServiceInfo[] MyQVS = Client.GetServices(ServiceTypes.QlikViewServer);
                Client.ClearQVSCache(QVSCacheObjects.All);
                //purge doc licenses
                DocumentNode[] allDocs = Client.GetUserDocuments(MyQVS[0].ID);
                DocumentMetaData Meta;
                for (int i = 0; i < allDocs.Length; i++)  //Loop through each document
                {
                    //Get the meta data for the current document
                    Meta = Client.GetDocumentMetaData(allDocs[i], DocumentMetaDataScope.Licensing);
                    string CurrentDocName = System.IO.Path.Combine(Meta.UserDocument.RelativePath, Meta.UserDocument.Name);
                    //are we working on the correct document
                    if (string.Compare(CurrentDocName, Document, true) == 0)
                    {   //found correct document, now look at users
                        bool IsDirty = false;
                        //Extract the current list of Document CALs
                        List<AssignedNamedCAL> currentCALs = Meta.Licensing.AssignedCALs.ToList();
                        for (int x = currentCALs.Count - 1; x >= 0; x--)
                        {
                            //If the user matches the name then remove it from the list
                            if ((currentCALs[x].QuarantinedUntil < System.DateTime.Now) && (string.Compare(User, currentCALs[x].UserName, true) == 0))
                            {
                                currentCALs.Remove(currentCALs[x]);
                                IsDirty = true;
                            }
                        }
                        if (IsDirty)
                        {
                            //Add the updated CALs list back to the meta data object
                            Meta.Licensing.AssignedCALs = currentCALs.ToArray();
                            //dec the cals allocated, otherwise silly QV still hangs on to license
                            Meta.Licensing.CALsAllocated = currentCALs.Count();
                            //Save the metadata back to the server
                            Client.SaveDocumentMetaData(Meta);
                        }
                    }
                }
                //shutdown client connection
                Client.Close();
                Client = null;
                return true;
            }
            catch (System.Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to remove QV licenses", ex);
                if (Client != null)
                    Client.Close();
            }
            return false;
        }

        /// <summary>
        /// Remove users named CAL associated with thier account
        /// Note: this method should be used with data acquired via method GetAllCALSOfTypeInUse
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public bool RemoveNamedUserCAL(string User )
        {
            QMSClient Client = null;
            try
            {
                string QMS = "http://localhost:4799/QMS/Service";
                Client = new QMSClient("BasicHttpBinding_IQMS", QMS);
                string key = Client.GetTimeLimitedServiceKey();
                ServiceKeyClientMessageInspector.ServiceKey = key;
                ServiceInfo[] MyQVS = Client.GetServices(ServiceTypes.QlikViewServer);
                Client.ClearQVSCache(QVSCacheObjects.All);

                CALConfiguration myCALs = Client.GetCALConfiguration(MyQVS[0].ID, CALConfigurationScope.NamedCALs);
                List<AssignedNamedCAL> currentNamedCALs = myCALs.NamedCALs.AssignedCALs.ToList();
                bool IsDirty = false;
                for (int i = currentNamedCALs.Count - 1; i >= 0; i--)
                {
                    //find the user license in the list
                    if ((currentNamedCALs[i].QuarantinedUntil < System.DateTime.Now) && (string.Compare(User, currentNamedCALs[i].UserName, true) == 0))
                    {
                        currentNamedCALs.RemoveAt(i);        
                        //update the count of assigned license
                        myCALs.NamedCALs.Assigned = currentNamedCALs.Count();
                        IsDirty = true;
                    }
                }
                //if we updated, the save config
                if (IsDirty)
                {
                    myCALs.NamedCALs.AssignedCALs = currentNamedCALs.ToArray();
                    Client.SaveCALConfiguration(myCALs);
                }
                Client.Close();
                Client = null;
                return true;
            }
            catch (System.Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to delete named user license for account:" + User, ex);
                if (Client != null)
                    Client.Close();
            }
            return false;
        }

        /// <summary>
        /// Create a report of license use in the specified directory
        /// </summary>
        /// <param name="ToPathFile"></param>
        public void GenerateCSVUsageReport(string ToPathFile)
        {
            try
            {
                System.IO.File.Delete(ToPathFile);
                List<LicenseInfo> LICollection = GetAllCALSOfTypeInUse(LicenseTypeEnum.ANY);
                string Info = "";
                foreach (LicenseInfo LI in LICollection)
                {
                    if (LI.License == LicenseTypeEnum.NAMED)
                        Info = LI.License.ToString() + "," + LI.User + "," + LI.LastAccessed.ToString();
                    else
                        Info = LI.License.ToString() + "," + LI.User + "," + LI.LastAccessed.ToString() + "," + LI.DocumentName;

                    System.IO.File.AppendAllText(ToPathFile, Info);
                }
            }
            catch (System.Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to generate license report", ex);
            }
        }

        //username requiredlicensetype requiredlicense# total #NamedCALSInUse #DocCalsInUse
        public class LicenseCostInfo
        {
            public string UserName { get; set; }
            public LicenseTypeEnum RequiredLicenseType { get; set; }
            public int RequiredLicenseCount { get; set; }
            public double TotalCost { get; set; }
            
            public int NamedCalsInUse { get; set; }
            public int DocCalsInUse { get; set; }

            public LicenseCostInfo(string _UserName, LicenseTypeEnum _RequiredLicType, int _RequiredCount, double _TotalCost, int _NamedCalsInUse, int _DocCalsInUse)
            {
                UserName = _UserName;
                RequiredLicenseType = _RequiredLicType;
                RequiredLicenseCount = _RequiredCount;
                TotalCost = _TotalCost;
                NamedCalsInUse = _NamedCalsInUse;
                DocCalsInUse = _DocCalsInUse;
            }

            public string ToCSV()
            {
                return UserName + "," + RequiredLicenseType.ToString() + "," + RequiredLicenseCount.ToString() + "," + TotalCost.ToString("C2") + "," + NamedCalsInUse.ToString() + "," + DocCalsInUse.ToString();
            }
        }

       /// <summary>
        ///Create a summary of the cost of licensing, optional write to a CSV file for email attachment
       /// </summary>
       /// <param name="CleanInUseLicense"> Remove all license not in use by users ( or quaratined )</param>
       /// <param name="OptionalCSVFile"> Write CSV file of data</param>
       /// <returns></returns>
        public List<LicenseCostInfo> GenerateLicenseCostReport(bool CleanInUseLicense = false, string OptionalCSVFile = "")
        {
            List<LicenseCostInfo> Aggregated = new List<LicenseCostInfo>();

            try
            {
                int MaxDocsPerDocumentCAL = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxDocsPerDocumentCAL"]);
                double NamedCALCost = System.Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["NamedCALCost"]);
                double DocCALCost = System.Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["DocCALCost"]);

                if ( CleanInUseLicense )
                    CleanLicenses();

                //snapshot of what is in use
                List<LicenseInfo> LICollection = GetAllCALSOfTypeInUse(LicenseTypeEnum.ANY);


                List<LicenseCostInfo> ByRequiredNamedUserCALLicenseTypes = new List<LicenseCostInfo>();
                List<LicenseCostInfo> ByRequiredDocumentCALLicenseTypes = new List<LicenseCostInfo>();
                List<LicenseCostInfo> ByNoLicenseRequiredTypes = new List<LicenseCostInfo>();

                System.Web.Security.MembershipUserCollection MUC = System.Web.Security.Membership.GetAllUsers();
                foreach (System.Web.Security.MembershipUser MU in MUC)
                {
                    string[] UserRoles = MillimanCommon.UserAccessList.GetRolesForUser(MU.UserName);
                    MillimanCommon.UserAccessList ACL = new MillimanCommon.UserAccessList(MU.UserName, UserRoles, false);
                    LicenseCostInfo LCI = null;
                    if (ACL.ACL.Count() >= MaxDocsPerDocumentCAL)
                    {
                        LCI = new LicenseCostInfo(MU.UserName, LicenseTypeEnum.NAMED, ACL.ACL.Count(), ACL.ACL.Count() * NamedCALCost, 0, 0);
                        ByRequiredNamedUserCALLicenseTypes.Add(LCI);
                    }
                    else if (ACL.ACL.Count() > 0)
                    {
                        LCI = new LicenseCostInfo(MU.UserName, LicenseTypeEnum.DOCUMENT, ACL.ACL.Count(), ACL.ACL.Count() * DocCALCost, 0, 0);
                        ByRequiredDocumentCALLicenseTypes.Add(LCI);
                    }
                    else
                    {
                        LCI = new LicenseCostInfo(MU.UserName, LicenseTypeEnum.NONE, 0, 0, 0, 0);
                        ByNoLicenseRequiredTypes.Add(LCI);
                    }

                    //check to see what is actually in use by QV
                    string TempUserName = AddUserDirCon(MU.UserName);
                    foreach (LicenseInfo LI in LICollection)
                    {
                        if ( string.Compare( LI.User, TempUserName, true ) == 0)
                        {
                            if (LI.License == LicenseTypeEnum.NAMED)
                                LCI.NamedCalsInUse++;
                            else
                                LCI.DocCalsInUse++;
                        }
                    }
                }


                Aggregated.AddRange(ByRequiredNamedUserCALLicenseTypes);
                Aggregated.AddRange(ByRequiredDocumentCALLicenseTypes);
                Aggregated.AddRange(ByNoLicenseRequiredTypes);

                if (string.IsNullOrEmpty(OptionalCSVFile) == false)
                {
                    System.IO.File.Delete(OptionalCSVFile);
                    foreach (LicenseCostInfo LCI in Aggregated)
                    {
                        System.IO.File.AppendAllText(OptionalCSVFile, LCI.ToCSV());
                    }
                }
            }
            catch (System.Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to generate license cost", ex);
            }

            return Aggregated;
        }

    }
}
