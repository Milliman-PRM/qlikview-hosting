using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CLSMedicareReimbursement
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ( !IsPostBack)
            {
                //get the data from the factory
                CLSPOCO.CLSPOCO Data = CLSPOCO.CLSPOCO.GetCLSDomainData();

                CLSBusinessLogic.BusinessLogicManager BLM = CLSBusinessLogic.BusinessLogicManager.GetInstance();
                //get the footer data and bind it
                FooterList.DataSource = BLM.FootNotes;
                FooterList.DataTextField = "Footnote1";
                FooterList.DataValueField = "Id";
                FooterList.DataBind();

                FooterLink.Text = "[MISSING]" + "(" + BLM.WebURL[0].Webaddressurl + ")";
                FooterLink.NavigateUrl = BLM.WebURL[0].Webaddressurl;

                //RatesGrid.VirtualItemCount = Data.RatesData.AllRates.Count();
                RatesGrid.VirtualItemCount = BLM.ReimbursementRate.ToList().Count();

                AnalyzerCheckList.DataSource = BLM.UniqueAnalyzers;
                AnalyzerCheckList.DataTextField = "AnalyzerName";
                AnalyzerCheckList.DataValueField = "Id";
                AnalyzerCheckList.DataBind();

                AssayDescriptionList.DataSource = BLM.UniqueAssayDescriptions;
                AssayDescriptionList.DataTextField = "SearchDesc";
                AssayDescriptionList.DataValueField = "Id";
                AssayDescriptionList.DataBind();

                LocalityList.DataSource = BLM.UniqueLocalities;
                LocalityList.DataTextField = "LocalityDescLong";
                LocalityList.DataValueField = "Id";
                LocalityList.DataBind();

                var year = BLM.Year;
                foreach (var y in year)
                {
                    YearDropdown.Items.Add(y.ToString());
                }

            }
        }

        protected void RatesGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {

            CLSBusinessLogic.BusinessLogicManager BLM = CLSBusinessLogic.BusinessLogicManager.GetInstance();
            CLSPOCO.CLSPOCO Data = CLSPOCO.CLSPOCO.GetCLSDomainData();
            int PageIndex = RatesGrid.CurrentPageIndex;
            int PageSize = RatesGrid.PageSize;
            int StartAt = PageIndex * PageSize;
            int ItemCount = PageSize;
            if (StartAt + ItemCount >= BLM.ReimbursementRate.ToList().Count())
                ItemCount = (BLM.ReimbursementRate.ToList().Count() - StartAt);
            RatesGrid.DataSource = Data.RatesData.AllRates.GetRange(StartAt, ItemCount );

            //Added to get data from the accurate source
            //RatesGrid.DataSource = BLM.ReimbursementRatesAllData;
        }

        private System.Drawing.Color Selected = System.Drawing.Color.LightGreen;
        private System.Drawing.Color UnSelected = System.Drawing.SystemColors.Control;

        protected void RatesGrid_SortCommand(object sender, Telerik.Web.UI.GridSortCommandEventArgs e)
        {
            
        }

        protected void AnalyzerCheckList_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> SelectedAnalyzerIDs = AnalyzerCheckList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Value).ToList();
            AssayDescriptionList.Items.Clear();
            List<CLSdbContext.SearchTerm> AssociatedAssayDescriptions = CLSBusinessLogic.BusinessLogicManager.GetInstance().FindAssayDescriptionForAnalyzer(SelectedAnalyzerIDs);
            AssayDescriptionList.DataSource = AssociatedAssayDescriptions;
            AssayDescriptionList.DataTextField = "SearchDesc";
            AssayDescriptionList.DataValueField = "Id";
            AssayDescriptionList.DataBind();
        }

        protected void AssayDescriptionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Test(ref LocalityList);
        }

        protected void LocalityList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Test(ref AssayDescriptionList);
        }

        private void Test(ref ListBox LB)
        {
            //clear all
            foreach (ListItem LI in LB.Items)
                LI.Selected = false;
            List<ListItem> Moving = new List<ListItem>();
            for (int Index = 0; Index < 5; Index++)
            {
                Moving.Add(LB.Items[LB.Items.Count-1]);
                LB.Items.RemoveAt(LB.Items.Count - 1);
            }
            foreach (ListItem LI in Moving)
            {
                LI.Selected = true;
                LB.Items.Insert(0,LI);
            }
        }
    }
}