using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace MillimanProjectManConsole.ComplexReporting
{
    public partial class NewItems : System.Web.UI.Page
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
                    Dictionary<string, List<MillimanCommon.QVWReportBank.NewItemClass>> NewItems = RBP.NewItemsGroupByConcept();
                    if (NewItems.Count > 0)
                    {
                        foreach (KeyValuePair<string, List<MillimanCommon.QVWReportBank.NewItemClass>> NewItem in NewItems)
                        {
                            List<string> Values = new List<string>();
                            foreach (MillimanCommon.QVWReportBank.NewItemClass NI in NewItem.Value)
                                Values.Add(NI.ItemValue);
                            MillimanCommon.UI.DynamicTable.CreateTable( "New Items for " + NewItem.Key , Values, PlaceHolder1);
                        }
                    }
                    else
                    {  //no new items
                        Label UserMessage = new Label();
                        UserMessage.Style.Add("width", "100%");
                        UserMessage.Text = "<br><br><center>The newer report does not contain any new fields.</center>";
                        PlaceHolder1.Controls.Add(UserMessage);
                    }
                
                }
            }
        }
     }
}