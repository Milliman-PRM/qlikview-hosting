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

                //set current working set in memory "DataSet" for main view grid
                Session["DataSet"] = BLM.AllData;
                RatesGrid.VirtualItemCount = BLM.AllData.Rows.Count;
                
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

                YearDropdown.Items.Add("2015");
                YearDropdown.Items.Add("2016");

                
            }
        }


        protected void RatesGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            System.Data.DataTable DataSet = Session["DataSet"] as System.Data.DataTable;
            if (DataSet != null)
            {
                DataSet.Columns[0].ColumnName = "Analyzer";
                DataSet.Columns[1].ColumnName = "Assay Description";
                DataSet.Columns[2].ColumnName = "CPT Descriptor";
                DataSet.Columns[3].ColumnName = "Notes";
                DataSet.Columns[4].ColumnName = "Locality";
                DataSet.Columns[5].ColumnName = "Medicare Reimbursement Rate";
                
                //copies over the definations, but not the data itself
                System.Data.DataTable DataSubSet = DataSet.Copy();

                int PageIndex = RatesGrid.CurrentPageIndex;
                int PageSize = RatesGrid.PageSize;
                int StartAt = PageIndex * PageSize;
                int ItemCount = PageSize;
                if (StartAt + ItemCount >= DataSet.Rows.Count)
                    ItemCount = (DataSet.Rows.Count - StartAt);
                int EndAt = StartAt + ItemCount;
                //copy over the data
                for (int Index = StartAt; Index < EndAt; Index++)
                    DataSubSet.Rows.Add(DataSet.Rows[Index].ItemArray);
                //add the subset to the view
                RatesGrid.DataSource = DataSubSet;

            }

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