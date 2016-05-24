using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.ComplexReporting
{
    public partial class ReductionAudit : System.Web.UI.Page
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
                    List<MillimanCommon.QVWReportBank.AuditClass> AuditItems = RBP.AuditData();
                    if (AuditItems.Count > 0)
                    {
                        List<string> Values = new List<string>();
                        foreach (MillimanCommon.QVWReportBank.AuditClass Audit in AuditItems)
                        {
                            Values.Add(Audit.LogMessage);
                        }
                        MillimanCommon.UI.DynamicTable.CreateTable("Process Audit/Trace", Values, PlaceHolder1);
                    }
                    else
                    {  //no new items
                        Label UserMessage = new Label();
                        UserMessage.Style.Add("width", "100%");
                        UserMessage.Text = "<br><br><center>No audit/trace information is available</center>";
                        PlaceHolder1.Controls.Add(UserMessage);
                    }

                }
            }

        }
    }
}