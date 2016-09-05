using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanClientUserAdmin
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
                    string DocumentRoot = System.Configuration.ConfigurationManager.AppSettings["QVDocumentRoot"];
                    DocumentRoot = DocumentRoot.Substring(0, DocumentRoot.LastIndexOf(@"\"));

                    MillimanCommon.ProjectSettings LocalPS = MillimanCommon.ProjectSettings.Load(Session["ProjectPath"].ToString());

                    MillimanCommon.QVWReportBankProcessor RBP = new MillimanCommon.QVWReportBankProcessor(Session["ProjectPath"].ToString());
                    Dictionary<string, List<MillimanCommon.QVWReportBank.NotSelectableClass>> UnselectableItems = RBP.NotSelectableByUser();
                    if (UnselectableItems.Count > 0)
                    {
                        foreach (KeyValuePair<string, List<MillimanCommon.QVWReportBank.NotSelectableClass>> Unselectable in UnselectableItems)
                        {
                            List<string> Values = new List<string>();
                            foreach (MillimanCommon.QVWReportBank.NotSelectableClass NI in Unselectable.Value)
                                Values.Add(NI.FieldName);
                            MillimanCommon.UI.DynamicTable.CreateTable( "User " +  Unselectable.Key  + " un-selectable values", Values, PlaceHolder1);
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