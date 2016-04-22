using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.IO;

namespace MillimanProjectManConsole
{
    public class ProductionPublisher
    {
        private static object LockingVar = new object();
        public class StatusClass
        {

            public string _Project;
            public string _TaskID;
            public string _StatusPercentage;
            public string _Status;
            public string _EmailCache;
            public string _DetailsMessage;

            public bool _isGarbage = false;

            public StatusClass(string MyProject)
            {
                _Project = MyProject;
                _TaskID = Guid.NewGuid().ToString("N");
                _StatusPercentage = "0";
            }
        }

        private Dictionary<string, StatusClass> Tasks = new Dictionary<string, StatusClass>();


        private MPMCServices.MPMCServicesSoapClient _RemoteServer;
        public ProductionPublisher(MPMCServices.MPMCServicesSoapClient RemoteServer)
        {
            _RemoteServer = RemoteServer;

        }

        public string GetError(string TaskID)
        {
            lock (LockingVar)
            {
                if (Tasks.ContainsKey(TaskID))
                    return Tasks[TaskID]._Status;
                return "0";
            }
        }
        public string GetPercentComplete(string TaskID)
        {
            lock (LockingVar)
            {
                if (Tasks.ContainsKey(TaskID))
                    return Tasks[TaskID]._StatusPercentage;
                return "0";
            }
        }
        public string GetDetailMessage(string TaskID)
        {
            lock (LockingVar)
            {
                if (Tasks.ContainsKey(TaskID))
                    return Tasks[TaskID]._DetailsMessage;
                return "";
            }
        }
        public bool IsFinished(string TaskID)
        {
            lock (LockingVar)
            {
                if (Tasks.ContainsKey(TaskID))
                    return Tasks[TaskID]._isGarbage == true;
            }
            return false;
        }
        public void SetStatus(string TaskID, string Status)
        {
            lock (LockingVar)
            {  //dont' create one, just move on
                if (Tasks.ContainsKey(TaskID))
                    Tasks[TaskID]._StatusPercentage = Status;
            }
        }
        public void DeleteStatus(string TaskID)
        {
            lock (LockingVar)
            {  //dont' create one, just move on
                if (Tasks.ContainsKey(TaskID))
                {
                    if (Tasks[TaskID]._isGarbage)
                        Tasks.Remove(TaskID);
                }
            }
        }
        
        public string PublishProject(string Project, string EmailCache)
        {
            StatusClass SC = new StatusClass(Project);
            SC._EmailCache = EmailCache;

            Tasks.Add(SC._TaskID, SC);

            ParameterizedThreadStart ts = new ParameterizedThreadStart(PublishToServer);
            Thread thd = new Thread(ts);
            thd.IsBackground = true;
            thd.Start(SC);

            return SC._TaskID;
        }

        /// <summary>
        /// if the thumbnail is empty we are using the global default - this is for backward compatability
        /// going forward there will always be a thumbnail,  even if its the same as the default
        /// </summary>
        /// <param name="PS"></param>
        /// <returns></returns>
        private string GetThumbnailFile(MillimanCommon.ProjectSettings PS)
        {
            if (string.IsNullOrEmpty(PS.QVThumbnail) == true)
                return HttpContext.Current.Server.MapPath("~/images/DefaultProjectImage.gif");
            else
                return System.IO.Path.Combine(PS.LoadedFromPath, PS.QVThumbnail);
        }

        /// <summary>
        /// we now transfer all files in the group directory
        /// </summary>
        /// <param name="PS"></param>
        /// <returns></returns>
        public long GetTotalBytesInTransfer(List<string> FilesToMove)
        {
            long Total = 0L;
            foreach (string File in FilesToMove)
            {
                FileInfo FI = new FileInfo(File);
                Total += FI.Length;
            }
            return Total;
        }

        /// <summary>
        /// we rename all the files listed as "_new" to thier respective file
        /// </summary>
        /// <param name="PS"></param>
        /// <returns></returns>
        private List<string> RenameToMove(MillimanCommon.ProjectSettings PS)
        {
            List<string> Renamed = new List<string>();
            bool MasterQVWNew = false;
            try
            {
                List<string> AllFiles = System.IO.Directory.GetFiles(PS.AbsoluteProjectPath, "*", SearchOption.AllDirectories).ToList();
                string MasterQVW = System.IO.Path.Combine(PS.AbsoluteProjectPath, PS.ProjectName + ".qvw");
                string Extension = string.Empty;
                string OrginFile = string.Empty;
                foreach (string Filename in AllFiles)
                {
                    if (Filename.ToLower().EndsWith("_new") == true)
                    {
                        OrginFile = Filename.Substring(0, Filename.Length - 4);
                        System.IO.File.Delete(OrginFile);
                        System.IO.File.Move(Filename, OrginFile);
                        Renamed.Add(OrginFile);
                        if (string.Compare(MasterQVW, OrginFile, true) == 0)
                            MasterQVWNew = true;
                    }
                }
                //done as 2 seperate loops since this loop depends on variable MasterQVNNew, which may get set in first loop
                foreach (string Filename in AllFiles)
                {
                    if ((Filename.ToLower().Contains(".selections")) && (Filename.ToLower().Contains("_old") == false) && (Filename.ToLower().Contains("_del") == false) && (MasterQVWNew == true))
                    {
                        Renamed.Add(Filename); //add all selections file, they may have been processed
                    }
                }
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to access '" + PS.LoadedFrom + "'", ex);
                return null;
            }
            //always upload these files
            if (Renamed.Contains(PS.LoadedFrom) == false)
                Renamed.Add(PS.LoadedFrom);  //always upload the project file
            string Thumbnailfile = System.IO.Path.Combine(PS.LoadedFromPath, PS.QVThumbnail);
            if (Renamed.Contains(Thumbnailfile) == false)
                Renamed.Add(Thumbnailfile);

            //always send user manual if it exists
            string UserManualfile = System.IO.Path.Combine(PS.LoadedFromPath, PS.UserManual);
            if ( (Renamed.Contains(UserManualfile) == false) && (System.IO.File.Exists(UserManualfile)))
                Renamed.Add(UserManualfile);

            if (MasterQVWNew)
            {
                //only move the cache items when the master qvw has been reduced
                string CacheDir = System.IO.Path.Combine(PS.AbsoluteProjectPath, "ReducedCachedQVWs");
                if (System.IO.Directory.Exists(CacheDir))
                {
                    List<string> CacheFiles = System.IO.Directory.GetFiles(CacheDir, "*", SearchOption.AllDirectories).ToList();
                    Renamed.AddRange(CacheFiles);
                }
            }

            //always resend download files, since it should be a reflection of what is here
            string DownloadDir = System.IO.Path.Combine(PS.AbsoluteProjectPath, PS.ProjectName +"_data");
            if (System.IO.Directory.Exists(DownloadDir))
            {
                List<string> DownloadFiles = System.IO.Directory.GetFiles(DownloadDir, "*", SearchOption.AllDirectories).ToList();
                Renamed.AddRange(DownloadFiles);
            }
            return Renamed;
        }

        /// <summary>
        /// Main task body that does all the work
        /// </summary>
        /// <param name="parms"></param>
        public void PublishToServer(object parms)
        {
            StatusClass Status = parms as StatusClass;
            if (Status != null)
            {
                string FullDocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                string DocumentRoot = FullDocumentRoot.Substring(0, FullDocumentRoot.LastIndexOf(@"\"));
                MillimanCommon.ProjectSettings PS = MillimanCommon.ProjectSettings.Load(System.IO.Path.Combine(DocumentRoot, Status._Project));
                if (PS != null)
                {
                    //updated the local version as a new file - always upload it
                    PS.UploadedToProduction = System.Web.Security.Membership.GetUser().UserName;
                    PS.UploadedToProductionDate = System.DateTime.Now.ToString();
                    PS.Save(PS.LoadedFrom + "_new");

                    List<string> FilesToMove = RenameToMove(PS);
                    if (FilesToMove == null)
                    {
                        string ErrorMsg = System.Web.Security.Membership.GetUser().UserName + " attempted to publish a project. Could not process project directory locally.";
                        AuditingInfo(ErrorMsg, PS.VirtualDirectory, PS.ProjectName);
                        Status._Status = ErrorMsg;
                        Status._isGarbage = true;
                        return;
                    }

                    PS.UploadedToProductionDate = DateTime.Now.ToString();
                    if (System.Web.Security.Membership.GetUser() != null)
                        PS.UploadedToProduction = System.Web.Security.Membership.GetUser().UserName;
                    long TotalBytesInTransfer = GetTotalBytesInTransfer(FilesToMove);
                    long TotalBytesTransfered = 0L;

                    //we have to update and save who is doing this first, otherwise our file diffs will be bad
                    PS.UploadedToProduction = System.Web.Security.Membership.GetUser().UserName;
                    PS.UploadedToProductionDate = DateTime.Now.ToString();
                    PS.Save();

                    //<add key="FriendlyServerName" value="STAGING"/>
                    string ServerName = System.Configuration.ConfigurationManager.AppSettings["FriendlyServerName"];
                    //upload all the files 
                    //List<string> AllFiles = System.IO.Directory.GetFiles(PS.AbsoluteProjectPath, "*", SearchOption.AllDirectories).ToList();
                    int UploadCount = 1;
                    foreach (string File in FilesToMove)
                    {
                        Status._DetailsMessage = "Uploading file " + UploadCount.ToString() + " of " + FilesToMove.Count.ToString() + " to " + ServerName;
                        if (FileUploader(Status, PS, File, ref TotalBytesTransfered, TotalBytesInTransfer, FullDocumentRoot, true) == false)
                        {
                            string ErrorMsg = System.Web.Security.Membership.GetUser().UserName + " attempted to publish a project. Publication failed due to failure in transfering the project file(.hciprj). Client access is still available to the current server project. Uploaded " + UploadCount.ToString() + " of " + FilesToMove.Count.ToString() + " files.  Failed on '" + FilesToMove[UploadCount] + "'";
                            AuditingInfo(ErrorMsg, PS.VirtualDirectory, PS.ProjectName);
                            Status._Status = ErrorMsg;
                            Status._isGarbage = true;
                            return;
                        }
                        UploadCount++;
                    }
                    ////now that the file are uploaded,  we want to delete the cached versions and user selections
                    //string CachDir = System.IO.Path.Combine(PS.AbsoluteProjectPath, "ReducedCachedQVWs");
                    //string UserQVWs = System.IO.Path.Combine(PS.AbsoluteProjectPath, "ReducedUserQVWs");
                    //System.IO.Directory.Delete(CachDir, true);
                    //System.IO.Directory.CreateDirectory(CachDir);
                    //System.IO.Directory.Delete(UserQVWs, true);
                    //System.IO.Directory.CreateDirectory(UserQVWs);


                    ////upload project file
                    //Status._DetailsMessage = "Uploading project file '" + PS.ProjectName + "'";
                    //if (FileUploader(Status, PS, System.IO.Path.Combine(DocumentRoot, Status._Project), ref TotalBytesTransfered, TotalBytesInTransfer, FullDocumentRoot, true) == false)
                    //{
                    //    string ErrorMsg = System.Web.Security.Membership.GetUser().UserName + " attempted to publish a project. Publication failed due to failure in transfering the project file(.hciprj). Client access is still available to the current server project.";
                    //    AuditingInfo( ErrorMsg, PS.VirtualDirectory, PS.ProjectName );
                    //    Status._Status = ErrorMsg;
                    //    Status._isGarbage = true;
                    //    return;
                    //}
                    ////upload qvw to temp
                    //if (FileUploader(Status, PS, PS.QVName + ".qvw", ref TotalBytesTransfered, TotalBytesInTransfer, FullDocumentRoot, true) == false)
                    //{
                    //    string ErrorMsg = System.Web.Security.Membership.GetUser().UserName + " attempted to publish a project. Publication failed due to failure in transfering the QVW file(.qvw). Client access is still available to the current server project.";
                    //    AuditingInfo(ErrorMsg, PS.VirtualDirectory, PS.ProjectName);
                    //    Status._Status = ErrorMsg;
                    //    Status._isGarbage = true;
                    //    return;
                    //}
                    ////upload Thumbnail
                    //if (FileUploader(Status, PS, PS.QVThumbnail, ref TotalBytesTransfered, TotalBytesInTransfer, FullDocumentRoot, true) == false)
                    //{
                    //    string ErrorMsg = System.Web.Security.Membership.GetUser().UserName + " attempted to publish a project. Publication failed due to failure in transfering the project thumbnail file(.png/.jpg/.gif). Client access is still available to the current server project.";
                    //    AuditingInfo(ErrorMsg, PS.VirtualDirectory, PS.ProjectName);
                    //    Status._Status = ErrorMsg;
                    //    Status._isGarbage = true;
                    //    return;
                    //    //log error or something
                    //}

                    ////upload UserManual
                    //if (FileUploader(Status, PS, PS.UserManual, ref TotalBytesTransfered, TotalBytesInTransfer, FullDocumentRoot, true) == false)
                    //{
                    //    string ErrorMsg = System.Web.Security.Membership.GetUser().UserName + " attempted to publish a project. Publication failed due to failure in transfering the project help file(.html). Client access is still available to the current server project.";
                    //    AuditingInfo(ErrorMsg, PS.VirtualDirectory, PS.ProjectName);
                    //    Status._Status = ErrorMsg;
                    //    Status._isGarbage = true;
                    //    return;
                    //}

                    ////upload resources
                    //if (FileUploader(Status, PS, PS.QVResources, ref TotalBytesTransfered, TotalBytesInTransfer, FullDocumentRoot, true) == false)
                    //{
                    //    string ErrorMsg = System.Web.Security.Membership.GetUser().UserName + " attempted to publish a project. Publication failed due to failure in transfering the project resource file. Client access is still available to the current server project.";
                    //    AuditingInfo(ErrorMsg, PS.VirtualDirectory, PS.ProjectName);
                    //    Status._Status = ErrorMsg;
                    //    Status._isGarbage = true;
                    //    return;
                    //}


                    //add autherization
                    //add to groups
                    string VirtualPath = Status._Project.Substring(FullDocumentRoot.Length).Replace('/', '\\');
                    while (VirtualPath.StartsWith(@"\"))
                        VirtualPath = VirtualPath.Substring(1);
                    Status._Status = _RemoteServer.ProcessProject(VirtualPath);

                    if (Status._Status.ToLower().IndexOf("error:") == -1)
                    {
                        AuditingInfo(Status._EmailCache, PS.VirtualDirectory, PS.ProjectName);
                    }
                    else
                    {
                        string ErrorMsg = "<html><body>Publication failed for _USER_ on _DATETIME_<br><br>_MSG_</body></html>";
                        ErrorMsg = ErrorMsg.Replace("_USER_", System.Web.Security.Membership.GetUser().UserName);
                        ErrorMsg = ErrorMsg.Replace("_DATETIME_", System.DateTime.Now.ToString());
                        ErrorMsg = ErrorMsg.Replace("_MSG_", Status._Status);
                        AuditingInfo(ErrorMsg, PS.VirtualDirectory, PS.ProjectName);
                    }
                }
            }
            else  //bad info relayed to publisher
            {
                AuditingInfo(System.Web.Security.Membership.GetUser().UserName + " attempted to publish a project.  Publication failed due to invalid/missing project file(.hciprj)", string.Empty, string.Empty);
            }
            Status._isGarbage = true;
        }

        /// <summary>
        /// create a filename that is unique based on Report_User_UniqueValue.html
        /// </summary>
        /// <param name="HistoryFolder">Folder to write the history</param>
        /// <param name="ReportName">Name of the report</param>
        /// <param name="PusherName">Person logged in and publishing</param>
        /// <returns></returns>
        private string GetUniqueFilename(string HistoryFolder, string ReportName, string PusherName)
        {
            int Index = 0;
            string RName = MillimanCommon.Utilities.CalculateMD5Hash(ReportName) + "_" + PusherName + "_";
            string History = System.IO.Path.Combine(HistoryFolder,  RName);
            string CandidateName = "";
            while(true)
            {
                CandidateName = History + Index.ToString() + ".html";
                if (System.IO.File.Exists(CandidateName) == false)
                    return RName + Index.ToString() + ".html";

                Index++;
            }
        }
        public void AuditingInfo(string Status, string VirtualDir, string ReportName)
        {
            try
            {
                System.Net.Mail.MailMessage objeto_mail = new System.Net.Mail.MailMessage();
                //write to history folder
                if ((string.IsNullOrEmpty(VirtualDir) == false) && (string.IsNullOrEmpty(ReportName) == false))
                {
                    if (VirtualDir.ToLower().IndexOf(".hciprj") > 0)
                        VirtualDir = VirtualDir.Substring(0, VirtualDir.IndexOf('\\'));

                    string Pusher = MillimanCommon.Utilities.ConvertStringToHex(System.Web.Security.Membership.GetUser().UserName);
                    string HistoryFolder = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["publishinghistory"], VirtualDir);
                    string RName = GetUniqueFilename(HistoryFolder, ReportName, Pusher);
                    System.IO.Directory.CreateDirectory(HistoryFolder);
                    HistoryFolder = System.IO.Path.Combine(HistoryFolder, RName);
                    System.IO.File.WriteAllText(HistoryFolder, Status);
                    //create an image file in the same directory
                    string OutputFilename = "";
                    MillimanCommon.HtmlConversion.CovertHTML(HistoryFolder, out OutputFilename);

                    //redirect status so stupid outlook will be happy
                    var contentID = "Image";
                    var StatusImage = new System.Net.Mail.Attachment(OutputFilename);
                    StatusImage.ContentId = contentID;
                    StatusImage.ContentDisposition.Inline = true;
                    StatusImage.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
                    objeto_mail.Attachments.Add(StatusImage);

                    Status = "<htm><body> <img src=\"cid:" + contentID + "\"> </body></html>";

                    ///
                    //Status = "<html><body><a href='~/admin/imagereflector.aspx?key=_KEY_' /></body></html>";
                    //Status = Status.Replace("_KEY_", MillimanCommon.Utilities.ConvertStringToHex(HistoryFolder));
                }
                else
                {
                    Status = "<htm><body>Auditing report was not found '" + VirtualDir + "\\" + ReportName + "' Report this issue to the system administrator</body></html>";
                }

                //send emails
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.Port = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["emailserverport"]);
                client.Host = System.Configuration.ConfigurationManager.AppSettings["emailserver"];
                client.Timeout = 10000;
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = true;
                if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["emailaccount"]) == false)
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["emailaccount"], System.Configuration.ConfigurationManager.AppSettings["emailpassword"]);
                }
                objeto_mail.From = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["email_from"]);

                string[] SendEmailsTo = System.Configuration.ConfigurationManager.AppSettings["email_to"].Split(new char[] { '~' });
                foreach (string EmailTo in SendEmailsTo)
                    objeto_mail.To.Add(new System.Net.Mail.MailAddress(EmailTo));
                objeto_mail.Subject = System.Configuration.ConfigurationManager.AppSettings["email_subject"];
                objeto_mail.IsBodyHtml = true;
  
                objeto_mail.Body = Status;
                client.Send(objeto_mail);
            }
            catch (Exception)
            {
            }
        }

        public void TestRegister()
        {
            _RemoteServer.ProcessProject("");
        }
        public bool FileUploader(StatusClass Status, MillimanCommon.ProjectSettings PS, string FileToUpload, ref long TotalBytesTransfered, long TotalBytesInTransfer, string DocumentRoot, bool UploadAsTempFile = false)
        {
            try
            {
                string UploadFile = System.IO.Path.Combine(PS.LoadedFromPath, FileToUpload).Replace('/', '\\');
                if (System.IO.File.Exists(UploadFile) == false)
                    return true; //not an error, we dont' have all the files to transfer
                string VirtualPath = UploadFile.Substring(DocumentRoot.Length);
                //no leading slashes
                while (VirtualPath.StartsWith(@"\"))
                    VirtualPath = VirtualPath.Substring(1);
                //upload as tmp file to be renamed later
                if (UploadAsTempFile)
                    VirtualPath += "_tmp";
                int chunkSize = 1048576;  //1M
                byte[] chunk = new byte[chunkSize];
                byte[] empty = new byte[chunkSize];
                double PercentTransfered = 0.0;
                string Response = "";
                bool DeleteOnFirstWrite = true;
                using (FileStream fileReader = new FileStream(UploadFile, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader binaryReader = new BinaryReader(fileReader);
                    int bytesToRead = (int)fileReader.Length;
                    do
                    {
                        Array.Copy(empty, chunk, empty.Length); //clear the array
                        chunk = binaryReader.ReadBytes(chunkSize);
                        //send data here
                        Response = _RemoteServer.PutBinarySlice(VirtualPath, chunk, chunk.Length, DeleteOnFirstWrite, true);
                        if (string.IsNullOrEmpty(Response) == false)
                            return false;  //didnt work
                        DeleteOnFirstWrite = false;  //set to false, only delete when we startup writting
                        TotalBytesTransfered += chunk.Length;
                        PercentTransfered = ((double)TotalBytesTransfered / (double)TotalBytesInTransfer) * 100.0;
                        Status._StatusPercentage = ((int)PercentTransfered).ToString();
                        System.Diagnostics.Debug.WriteLine(Status._StatusPercentage);
                        bytesToRead -= chunkSize;

                    } while (bytesToRead > 0);
                }
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }


 
    }
}