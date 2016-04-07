using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanClientUserAdmin
{
    public partial class UserDownloads : System.Web.UI.Page
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
                    List<string> ColumnNames = null;
                    List<List<string>> DownloadItems = DisplayProcessor( RBP.UserDownloads(LocalPS, out ColumnNames));
                    if (DownloadItems.Count > 0)
                    {
                        MillimanCommon.UI.DynamicTable DynTable = new MillimanCommon.UI.DynamicTable();
                        DynTable.CreateTable2DBinder("User Downloadable Items", ColumnNames, DownloadItems, PlaceHolder1, System.Drawing.Color.FromArgb(120, 240, 240, 240));
                    }
                    else
                    {  //no new items
                        Label UserMessage = new Label();
                        UserMessage.Style.Add("width", "100%");
                        UserMessage.Text = "<br><br><center>No items can be downloaded by users.</center>";
                        PlaceHolder1.Controls.Add(UserMessage);
                    }

                }
            }
        }

        /// <summary>
        /// Process the values such that no value is repeated on multiple rows concurrently
        /// </summary>
        /// <param name="DisplayItems"></param>
        /// <returns></returns>
        private List<List<string>> DisplayProcessor(List<List<string>> DisplayItems)
        {
            if (DisplayItems.Count > 0)
            {
                List<string> CurrentValues = new List<string>();
                foreach (string Current in DisplayItems[0])
                    CurrentValues.Add(Current);

                for (int Index = 1; Index < DisplayItems.Count(); Index++)
                {
                    List<string> RowItems = DisplayItems[Index];
                    for (int TokenIndex = 0; TokenIndex < RowItems.Count(); TokenIndex++)
                    {
                        if (string.Compare(CurrentValues[TokenIndex], RowItems[TokenIndex], true) == 0)
                        {
                            //we need to check the cell to our left, if it has a value, we need to keep our value
                            if ((TokenIndex - 1 >= 0) && (string.IsNullOrEmpty(RowItems[TokenIndex - 1]) == true))
                                RowItems[TokenIndex] = "";
                            else if (TokenIndex == 0)
                                RowItems[TokenIndex] = "";
                        }
                        else
                        {
                            CurrentValues[TokenIndex] = RowItems[TokenIndex];
                        }
                    }
                }
            }
            return DisplayItems;
        }
    }
}