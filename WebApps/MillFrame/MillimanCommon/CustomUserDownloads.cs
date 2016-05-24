using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MillimanCommon
{
    public class CustomUserDownloads
    {
        public bool LoadedSuccessfully { get; set;  }

        private long FileSizeLastLoad = 0L;

        private static CustomUserDownloads instance;
        private static object instance_lock = new object();
        public static CustomUserDownloads GetInstance()
        {
            if (instance == null)
            {
                instance = new CustomUserDownloads();
                instance.Load();
            }
            else  //check to see if file changed since last load, if so reload it
            {
                string PathFilename = System.Configuration.ConfigurationManager.AppSettings["UserSpecificDownloads"];
                if (System.IO.File.Exists(PathFilename))
                {
                    System.IO.FileInfo FI = new System.IO.FileInfo(PathFilename);
                    if (instance.FileSizeLastLoad != FI.Length)
                    {
                        lock (instance_lock)
                        {
                            instance = null;
                            instance = new CustomUserDownloads();
                            instance.Load();
                        }
                    }
                }
            }
            return instance;
        }
        public static void KillInstance()
        {
            if (instance != null)
                instance = null;
        }

        public class CustomDownloads
        {

            //used for search critria
            private string _AccountName;

            public string AccountName
            {
                get { return _AccountName; }
                set { _AccountName = value; }
            }
            //used for search criteria
            private string _VirtualReport;

            public string VirtualReport
            {
                get { return _VirtualReport; }
                set { _VirtualReport = value; }
            }

            private string _VirtualItemPath; //path relative to CustomUserDownloads folder

            public string VirtualItemPath
            {
                get { return _VirtualItemPath; }
                set { _VirtualItemPath = value; }
            }
            private string _VirtualItemIcon; //path relative to CustomUserDownloads folder 16x16

            public string VirtualItemIcon
            {
                get { return _VirtualItemIcon; }
                set { _VirtualItemIcon = value; }
            }
            private string _MimeType;

            public string MimeType
            {
                get { return _MimeType; }
                set { _MimeType = value; }
            }
            private string _Tooltip;

            public string Tooltip
            {
                get { return _Tooltip; }
                set { _Tooltip = value; }
            }

            public CustomDownloads() { }
            public CustomDownloads(string Account, string Report, string ItemPath, string ItemIcon, string Mime, string aTooltip)
            {
                _AccountName = Account;
                _VirtualReport = Report;
                _VirtualItemPath = ItemPath;
                _VirtualItemIcon = ItemIcon;
                _MimeType = Mime;
                _Tooltip = aTooltip;
            }
        }

        private List<CustomDownloads> _DownloadItems;

        public List<CustomDownloads> DownloadItems
        {
            get { return _DownloadItems; }
            set { _DownloadItems = value; }
        }

        /// <summary>
        /// Return a list of custom downloads that are for the requested QVW
        /// </summary>
        /// <param name="QualifiedQVW"></param>
        /// <returns></returns>
        public List<CustomDownloads> FindItemsForQVW(string QualifiedQVW)
        {
            List<CustomDownloads> Downloads = new List<CustomDownloads>();
            lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
            {
                try
                {
                    foreach (CustomDownloads CD in DownloadItems)
                    {
                        if (string.Compare(CD.VirtualReport, QualifiedQVW, true) == 0)
                            Downloads.Add(CD);
                    }
                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(Report.ReportType.Error, "Add or Update custom download item failed", ex);
                }
            }
            return Downloads;
        }
        

        public bool AddorUpdateItem(string AccountName,
                             string VirtualReportName,
                             string VirtualDownloadItem,
                             string Icon,
                             string Mime,
                             string Tooltip)
        {
            lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
            {
                try
                {
                    bool Updated = false;
                    foreach (CustomDownloads CD in DownloadItems)
                    {
                        if ((string.Compare(AccountName, CD.AccountName, true) == 0) &&
                             (string.Compare(VirtualReportName, CD.VirtualReport, true) == 0) &&
                             (string.Compare(VirtualDownloadItem, CD.VirtualItemPath, true) == 0))
                        {
                            CD.VirtualItemIcon = Icon;
                            CD.MimeType = Mime;
                            CD.Tooltip = Tooltip;
                            Updated = true;
                            break;
                        }
                    }
                    if (Updated == false)
                    {
                        CustomDownloads CD = new CustomDownloads(AccountName, VirtualReportName, VirtualDownloadItem, Icon, Mime, Tooltip);
                        DownloadItems.Add(CD);
                    }
                    return Save();
                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(Report.ReportType.Error, "Add or Update custom download item failed", ex);
                }
                return false;
            }
        }

        public bool DeleteItem( string AccountName,
                                string VirtualReportName,
                                string VirtualDownloadItem)
        {
            lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
            {
                try
                {
                    int Index = 0;
                    foreach (CustomDownloads CD in DownloadItems)
                    {
                        if ((string.Compare(AccountName, CD.AccountName, true) == 0) &&
                             (string.Compare(VirtualReportName, CD.VirtualReport, true) == 0) &&
                             (string.Compare(VirtualDownloadItem, CD.VirtualItemPath, true) == 0))
                        {
                            DownloadItems.RemoveAt(Index);
                            break;
                        }
                        Index++;
                    }
                    return Save();
                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(Report.ReportType.Error, "Delete Item from custom download list failed", ex);
                }
                return false;
            }
        }

        /// <summary>
        /// for the account and virtual report, clear the associations
        /// </summary>
        /// <param name="AccountName"></param>
        /// <param name="VirtualReportName"></param>
        /// <returns></returns>
        public bool ClearItems(   string AccountName,
                                  string VirtualReportName )
        {
            lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
            {
                try
                {
                    for( int Index = DownloadItems.Count-1; Index >= 0; Index-- )
                    {
                        CustomDownloads CD = DownloadItems[Index];
                        if ((string.Compare(AccountName, CD.AccountName, true) == 0) &&
                             (string.Compare(VirtualReportName, CD.VirtualReport, true) == 0))
                        {
                            DownloadItems.RemoveAt(Index);
                        }
                    }
                    return Save();
                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(Report.ReportType.Error, "Clear item from custom download list failed", ex);
                }
                return false;
            }
        }

        /// <summary>
            /// for now we use this to save the template, there is not UI for updating
            /// </summary>
            /// <returns></returns>
            //public bool Save()
            //{
            //    try
            //    {
            //        List<CustomDownloads> Temp = new List<CustomDownloads>();
            //        Temp.Add(new CustomDownloads("myaccount", "myreport", "myitempath", "myitemicon", "mymime", "mytooltip"));
            //        Temp.Add(new CustomDownloads("myaccount", "myreport", "myitempath", "myitemicon", "mymime", "mytooltip"));
            //        Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
            //        string PathFilename = System.Configuration.ConfigurationManager.AppSettings["UserSpecificDownloads"];
            //        if (System.IO.File.Exists(PathFilename) == true)
            //            System.IO.File.Delete(PathFilename);
            //        SS.Serialize(Temp, PathFilename);
            //        return true;
            //    }
            //    catch (Exception)
            //    {

            //    }
            //    return false;
            //}


        /// <summary>
        /// used by client admin to get list for updates
        /// </summary>
        /// <returns></returns>
            public bool Load()
            {
                lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
                {
                    try
                    {
                        LoadedSuccessfully = true;
                        string PathFilename = System.Configuration.ConfigurationManager.AppSettings["UserSpecificDownloads"];
                        if (System.IO.File.Exists(PathFilename))
                        {
                            Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                            List<CustomDownloads> DownloadInfoList = SS.Deserialize(PathFilename) as List<CustomDownloads>;
                            _DownloadItems = DownloadInfoList;
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "", ex);
                    }
                    //set downloads to an empty list, other things depend on this list
                    _DownloadItems = new List<CustomDownloads>();
                    MillimanCommon.Report.Log(Report.ReportType.Error, "Failed to open custom download list");
                    LoadedSuccessfully = false;
                    return false;
                }
            }

        /// <summary>
        /// push all the downloads back to the file
        /// </summary>
        /// <param name="AllItems"></param>
        /// <returns></returns>
            public bool Save()
            {
                try
                {
                    Polenter.Serialization.SharpSerializer SS = new Polenter.Serialization.SharpSerializer(false);
                    string PathFilename = System.Configuration.ConfigurationManager.AppSettings["UserSpecificDownloads"];
                    if (System.IO.File.Exists(PathFilename) == true)
                        System.IO.File.Delete(PathFilename);
                    SS.Serialize(DownloadItems, PathFilename);
                    return true;
                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "", ex);
                }
                return false;
            }




            //for a specific user and specific report return an special downloads required
            public List<CustomDownloads> GetUserSpecficDownloads( string AccountName, string ReportAndVirtualPath )
            {
                lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
                {
                    try
                    {

                        //hate doing this but since our XML is current created by hand we have to
                        ReportAndVirtualPath = ReportAndVirtualPath.Replace('/', '\\');
                        foreach (CustomDownloads CD in DownloadItems)
                        {
                            CD.VirtualReport = CD.VirtualReport.Replace('/', '\\');
                            CD.VirtualItemPath = CD.VirtualItemPath.Replace('/', '\\');
                            CD.VirtualItemIcon = CD.VirtualItemIcon.Replace('/', '\\');
                        }
                        //end slash check
                        return DownloadItems.FindAll(x => ((string.Compare(x.AccountName, AccountName, true) == 0) && (string.Compare(x.VirtualReport, ReportAndVirtualPath, true) == 0)));
                    }
                    catch (Exception ex)
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "", ex);
                    }
                    return null;
                }
            }

            /// <summary>
            /// Check to see if item is referenced by the custom user downloads - called by GC
            /// </summary>
            /// <param name="ItemAndVirtualPath">Report icon and download item</param>
            /// <returns></returns>
            public bool ItemIsReferenced(string ItemAndVirtualPath)
            {
                lock (instance_lock) //lock on gettting instance to protect against threading issues( good enough for now)
                {
                    try
                    {

                        //hate doing this but since our XML is current created by hand we have to
                        ItemAndVirtualPath = ItemAndVirtualPath.Replace('/', '\\');
                        foreach (CustomDownloads CD in DownloadItems)
                        {
                            if (string.Compare(CD.VirtualItemIcon, ItemAndVirtualPath, true) == 0)
                                return true;
                            else if (string.Compare(CD.VirtualItemPath, ItemAndVirtualPath, true) == 0)
                                return true;
                        }
                       
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "", ex);
                    }
                    return true; //return true, so we don't GC any items away if an error occurred
                }
            }
    }
}