using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanProjectManConsole.ComplexReporting
{
    public partial class ReductionStatus : System.Web.UI.Page
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
                    List<string> SuccessfulReductionUsers = null;
                    List<string> FailedReductionUsers = null;
                    RBP.ReductionStatus(out SuccessfulReductionUsers, out FailedReductionUsers);
                    if (FailedReductionUsers.Count > 0)
                    {
                        MillimanCommon.UI.DynamicTable.CreateTable("Failed Report Reduction", FailedReductionUsers, PlaceHolder1);
                    }
                    if (SuccessfulReductionUsers.Count > 0)
                    {
                        MillimanCommon.UI.DynamicTable.CreateTable("Successful Report Reduction", SuccessfulReductionUsers, PlaceHolder1);
                    }


                }
            }
        }
    }
}