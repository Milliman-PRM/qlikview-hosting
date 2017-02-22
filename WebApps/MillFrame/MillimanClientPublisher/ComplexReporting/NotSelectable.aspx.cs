using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClientPublisher.ComplexReporting
{
    public partial class NotSelectable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            EnableViewState = false;

            if (!IsPostBack)
            {
                if (Session["ProjectPath"] != null)
                {
                    //string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                    //DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));

                    MillimanCommon.ProjectSettings LocalPS = MillimanCommon.ProjectSettings.Load(Session["ProjectPath"].ToString());

                    ReportingCommon RC = new ReportingCommon();
                    string WorkingPath = System.IO.Path.GetDirectoryName(Session["ProjectPath"].ToString());
                    Dictionary<string, List<string>> MissingValueMap = RC.GetMissingValues(WorkingPath);
                    if ( (MissingValueMap != null) && (MissingValueMap.Count() > 0))
                    {
                        foreach( KeyValuePair<string,List<string>> KVP in MissingValueMap)
                        {
                            if ( KVP.Value.Count() > 0 )
                               MillimanCommon.UI.DynamicTable.CreateTable("User " + KVP.Key + " un-selectable values(MISSING)", KVP.Value, PlaceHolder1);
                        }
                    }
                    else
                    {  //no new items
                        Label UserMessage = new Label();
                        UserMessage.Style.Add("width", "100%");
                        UserMessage.Text = "<br><br><center>All user selections are still valid.</center>";
                        PlaceHolder1.Controls.Add(UserMessage);
                    }

                }
            }
        }
    }
}