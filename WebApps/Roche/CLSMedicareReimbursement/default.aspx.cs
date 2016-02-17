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

                //get the footer data and bind it
                FooterList.DataSource = Data.FooterData.FooterItems;
                FooterList.DataBind();
                FooterLink.Text = Data.FooterData.FooterLink + "(" + Data.FooterData.FootLinkURI + ")";
                FooterLink.NavigateUrl = Data.FooterData.FootLinkURI;

                RatesGrid.VirtualItemCount = Data.RatesData.AllRates.Count();

                AnalyzerCheckList.DataSource = Data.AnalyzerData.UniqueAnalyzers;
                AnalyzerCheckList.DataBind();

                AssayDescriptionList.DataSource = Data.AssayDescriptionData.AssayDescriptions;
                AssayDescriptionList.DataBind();

                LocalityList.DataSource = Data.LocalityData.Localities;
                LocalityList.DataBind();

            }
        }

        protected void RatesGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            CLSPOCO.CLSPOCO Data = CLSPOCO.CLSPOCO.GetCLSDomainData();
            int PageIndex = RatesGrid.CurrentPageIndex;
            int PageSize = RatesGrid.PageSize;
            int StartAt = PageIndex * PageSize;
            int ItemCount = PageSize;
            if (StartAt + ItemCount >= Data.RatesData.AllRates.Count())
                ItemCount = (Data.RatesData.AllRates.Count() - StartAt);
            RatesGrid.DataSource = Data.RatesData.AllRates.GetRange(StartAt, ItemCount );
         
          
        }

        private System.Drawing.Color Selected = System.Drawing.Color.LightGreen;
        private System.Drawing.Color UnSelected = System.Drawing.SystemColors.Control;

        protected void CurrentYear_Click(object sender, EventArgs e)
        {
            CurrentYear.BackColor = Selected;
            LastYear.BackColor = UnSelected;
            
        }

        protected void LastYear_Click(object sender, EventArgs e)
        {
            CurrentYear.BackColor = UnSelected;
            LastYear.BackColor = Selected;
        }

        protected void RatesGrid_SortCommand(object sender, Telerik.Web.UI.GridSortCommandEventArgs e)
        {
            int index = 0;
        }

        protected void AnalyzerCheckList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Test(ref AssayDescriptionList);
            Test(ref LocalityList);
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