using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.Admin
{
    public partial class QVWImportVerify : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string Msg = string.Empty;
                string Key = Request["key"];
                if (string.IsNullOrEmpty(Key) == false)
                {
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
                                if ((string.IsNullOrEmpty(Group) == false) && (string.IsNullOrEmpty(Item.Value) == false))
                                    Group += "_";
                                Group += Item.Value;
                            }
                        }
                        string Path = Group.Replace('_', '\\');

                        //get the groups from the server
                        List<string> ServerGroups = Global.GetInstance().GetGroups().ToList();
                        bool GroupExists = false;
                        if (ServerGroups != null)
                        {
                            foreach (string G in ServerGroups)
                            {
                                if (string.Compare(G, Group, true) == 0)
                                    GroupExists = true;
                            }
                        }

                        string FullPath = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"], Path);
                        bool DirExists = System.IO.Directory.Exists(FullPath);
                        bool CanEmit = false;
                        if (XMLFS.SignatureDictionary.ContainsKey("can_emit"))
                            CanEmit = System.Convert.ToBoolean(XMLFS.SignatureDictionary["can_emit"]);
                        
                        if ((GroupExists == false) && (DirExists == false))
                        {
                            Msg = "Continuing will result in a group called<br><b> '" + Group + "'</b><br> being created along with it's associated directory structure<br><b> '" + Path + "'</b>";
                            if (CanEmit)
                                Msg += "<br><br>The QVW has been signed to allow client user administration<br>";
                            else
                                Msg += "<br><br>The QVW has NOT been signed in regard to client user administration. Client user administration will not be available.<br>";
                            VerificationAction.Value = "QVWImporter.aspx";
                            VerifiedClick.Visible = true;
                            VerifiedClick.Text = "Continue";
                        }
                        else if ((GroupExists == true) && (DirExists == false))
                        {
                            Msg = "The requested group<br> <b>'" + Group + "'</b> <br>already exists.<br><br> QVW importing is allowed only when the requested group and directory do not exist.";
                        }
                        else if ((GroupExists == false) && (DirExists == true))
                        {
                            Msg = "The requested directory <br><b>'" + Path + "'</b><br> already exists.<br><br> QVW importing is allowed only when the requested group and directory do not exist.";
                        }
                        else if ((GroupExists == true) && (DirExists == true))
                        {
                            Msg = "The requested group <br><b>'" + Group + "'</b><br> and directory <br>'" + Path + "'<br> already exist.<br><br> QVW importing is allowed only when the requested group and directory do not exist.";
                        }
                    }
                    else
                    {
                        Msg = "The selected QVW is <b>not</b> signed. <br><br> Only signed QVWs may be used with import";
                    }
                }
                else  //error display
                {
                    Msg = "No QVW was provided for import?"; 
                }

                Verify.Text = "<center>" + Msg + "</center>";
            }
        }

        protected void VerifiedClick_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(VerificationAction.Value) == false)
            {
                //send them on to the importer
                Response.Redirect(VerificationAction.Value.ToString() + "?key=" + Request["key"]);
            }
            else
            {
                ///close the window, nothing else to do
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "closeScript", "clientClose('');", true);
            }


        }
    }
}