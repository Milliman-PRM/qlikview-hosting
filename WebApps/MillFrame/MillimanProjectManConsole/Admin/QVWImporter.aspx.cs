using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.Admin
{
    public partial class QVWImporter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string Msg = string.Empty;
                string Key = Request["key"];
                if (string.IsNullOrEmpty(Key) == false)
                {
                    //first hit, we create role and directory and start the file copy thread
                    string TempFile = MillimanCommon.Utilities.ConvertHexToString(Key);
                    MillimanCommon.FileSignature FS = new MillimanCommon.FileSignature();
                    if (FS.IsSigned(TempFile) == true)
                    {
                        string Group = string.Empty;
                        MillimanCommon.XMLFileSignature XMLFS = new MillimanCommon.XMLFileSignature(TempFile);
                        foreach (KeyValuePair<string, string> Item in XMLFS.SignatureDictionary)
                        {
                            if (Item.Key.StartsWith("@") == true)
                            {
                                if ((string.IsNullOrEmpty(Group) == false) && (string.IsNullOrEmpty(Item.Value) == false) )
                                    Group += "_";
                                Group += Item.Value;
                            }
                        }
                        string Path = Group.Replace('_', '\\');
                        //bool CanEmit = false;

                        //to get here we need have already checked it does not exist
                        Global.GetInstance().CreateGroup(Group);

                        string FullPath = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"], Path);
                        System.IO.Directory.CreateDirectory(FullPath);

                        string FullPathWithName = System.IO.Path.Combine(FullPath, Guid.NewGuid().ToString("N") + ".imp");
                        System.Threading.ParameterizedThreadStart ts = new System.Threading.ParameterizedThreadStart(CopyQVWToProjectDirectory);
                        System.Threading.Thread thd = new System.Threading.Thread(ts);
                        thd.IsBackground = true;
                        thd.Start(TempFile + "~" + FullPathWithName);

                        string Semaphore = Guid.NewGuid().ToString("N");
                        Session[Semaphore] = thd;   //lets use the session, to get this done
                        Response.Redirect("QVWImporter.aspx?wait=" + Semaphore + "&loc=" + MillimanCommon.Utilities.ConvertStringToHex( FullPathWithName ));
                    }
                }
                    //this will get pinged over and over until done, or failed
                else if (Request["wait"] != null)
                {
                    System.Threading.Thread BW = Session[Request["wait"].ToString()] as System.Threading.Thread;
                    if (BW != null)
                    {
                        if (BW.IsAlive == false)
                        {
                            Session["wait"] = null;  //clear it out of session
                            Response.Redirect("EnhancedUploadView.aspx?loc=" + Request["loc"]);
                            return;
                        }
                        else
                        {  //still processing
                            Response.Redirect("QVWImporter.aspx?wait=" + Request["wait"] + "&loc=" + Request["loc"]);
                            return;
                        }
                    }
                    else
                    {
                        MillimanCommon.Alert.Show("QVW mover thread failed - please close this window and try again.");
                    }
                }
            }
        }

        /// <summary>
        /// if results is empty we are good to go, otherwise it has an error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CopyQVWToProjectDirectory(object parms)
        {
            try
            {
                string Action = parms.ToString();
                string[] Args = Action.Split(new char[] { '~' });
                //do some cleanup
                System.IO.File.Delete(Args[1]);

                //move the file
                System.IO.File.Move(Args[0], Args[1] );
            }
            catch (Exception ex)
            {
                MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, ex.ToString());
            }
        }

      
    }
}