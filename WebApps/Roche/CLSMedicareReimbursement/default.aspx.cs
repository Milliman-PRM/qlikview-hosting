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
        private string SessionKey_Selections = @"CurrentSelections";
        private string SessionKey_DataSet    = @"CurrentDataSet";

        protected void Page_Load(object sender, EventArgs e)
        {
            if ( !IsPostBack)
            {

                //make a session holder for the users selections 
                if ( Session[SessionKey_Selections] == null )
                    Session[SessionKey_Selections] = new CLSBusinessLogic.BusinessLogicManager.CurrentSelections();

                CLSBusinessLogic.BusinessLogicManager BLM = CLSBusinessLogic.BusinessLogicManager.GetInstance();
                //get the footer data and bind it
                FooterList.DataSource = BLM.FootNotes;
                FooterList.DataTextField = "Footnote1";
                FooterList.DataValueField = "Id";
                FooterList.DataBind();

                FooterLink.Text = "[MISSING]" + "(" + BLM.WebURL[0].Webaddressurl + ")";
                FooterLink.NavigateUrl = BLM.WebURL[0].Webaddressurl;

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

                YearDropdown.DataSource = BLM.UniqueYears.OrderByDescending(i => i);
                YearDropdown.DataBind();
                YearDropdown.SelectedIndex = 0;

                //set current working set in memory "DataSet" for main view grid
                Session[SessionKey_DataSet] = BLM.DataByYear[YearDropdown.SelectedItem.Text];
                RatesGrid.VirtualItemCount = BLM.DataByYear[YearDropdown.SelectedItem.Text].Rows.Count;
            }
        }


        protected void RatesGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            System.Data.DataTable DataSet = Session[SessionKey_DataSet] as System.Data.DataTable;
            if (DataSet != null)
            {
                //DataSet.Columns[0].ColumnName = "Analyzer";
                //DataSet.Columns[1].ColumnName = "Assay Description";
                //DataSet.Columns[2].ColumnName = "CPT Descriptor";
                //DataSet.Columns[3].ColumnName = "Notes";
                //DataSet.Columns[4].ColumnName = "Locality";
                //DataSet.Columns[5].ColumnName = "Medicare Reimbursement Rate";

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
            string SortOnColumn = e.CommandName;
            string SortMethod = "DESC";
            if (e.NewSortOrder == Telerik.Web.UI.GridSortOrder.Ascending)
                SortMethod = "ASC";

            //System.Data.DataTable CurrentTable = Session[SessionKey_DataSet] as System.Data.DataTable;

            //CurrentTable.DefaultView.Sort = SortOnColumn + " " + SortMethod;

        }

        protected void AnalyzerCheckList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //get user selections
            CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;

            List<string> SelectedAnalyzerNames = AnalyzerCheckList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Text).ToList();
            //update the users selection object instance
            CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.ANALYZERNAMES);  
            foreach (string Selected in SelectedAnalyzerNames)
                CurrentSels.AddToList(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.ANALYZERNAMES, Selected);

            AssayDescriptionList.Items.Clear();

            List<string> SelectedAnalyzerIDs = AnalyzerCheckList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Value).ToList();
            List<CLSdbContext.SearchTerm> AssociatedAssayDescriptions = CLSBusinessLogic.BusinessLogicManager.GetInstance().FindAssayDescriptionForAnalyzer(SelectedAnalyzerIDs);
            AssayDescriptionList.DataSource = AssociatedAssayDescriptions;
            AssayDescriptionList.DataTextField = "SearchDesc";
            AssayDescriptionList.DataValueField = "Id";
            AssayDescriptionList.DataBind();
        }

        protected void AssayDescriptionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //get user selections
            CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;
            List<string> SearchTermIDs= AssayDescriptionList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Value).ToList();

            if ( CurrentSels.NoSelectionsMade())
            {    //no selections have been made, so we need to look up the appropriate analyziers and set to checked

            }
            else
            {   //selections have been made, we are narrowing the search down
                CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMIDS);
                foreach (string SearchTermID in SearchTermIDs)
                    CurrentSels.AddToList(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMIDS, SearchTermID);
            }
        }

        protected void LocalityList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //get user selections
            CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;
            List<string> SelectedLocalityIDs = LocalityList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Value).ToList();
            CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.LOCALATIESIDS);
            //update the users selection object instance
            foreach (string SelectedLocalityID in SelectedLocalityIDs)
                CurrentSels.AddToList(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.LOCALATIESIDS, SelectedLocalityID);

        }

        protected void YearDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            //should we requery against new year that has been selected
            CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;

            //first check if are not changing selections do nothing
            if (CurrentSels.Years.Contains(YearDropdown.SelectedItem.Text) == true)
                return;  //we are already showing data for this year

            if ( CurrentSels.NoSelectionsMade() )
            {
                System.Data.DataTable ResultSet = CLSBusinessLogic.BusinessLogicManager.GetInstance().DataByYear[YearDropdown.SelectedItem.Text];
                RatesGrid.VirtualItemCount = ResultSet.Rows.Count;
                RatesGrid.DataSource = ResultSet;
                RatesGrid.DataBind();
            }
            else
            {
                //change year, and requery
                CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.YEARS);
                CurrentSels.AddToList(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.YEARS, YearDropdown.SelectedItem.Text);
                System.Data.DataTable ResultSet = CLSBusinessLogic.BusinessLogicManager.GetInstance().FetchDataForSelections(CurrentSels);
                Session[SessionKey_DataSet] = ResultSet;
                RatesGrid.VirtualItemCount = ResultSet.Rows.Count;
                RatesGrid.DataSource = ResultSet;
                RatesGrid.DataBind();
            }
        }

        protected void RadGridPanel_AjaxRequest(object sender, Telerik.Web.UI.AjaxRequestEventArgs e)
        {
            CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;

            switch ( e.Argument)
            {
                case "refresh":
                    System.Data.DataTable ResultSet = CLSBusinessLogic.BusinessLogicManager.GetInstance().FetchDataForSelections(CurrentSels);
                    Session[SessionKey_DataSet] = ResultSet;
                    RatesGrid.VirtualItemCount = ResultSet.Rows.Count;
                    RatesGrid.DataSource = ResultSet;
                    RatesGrid.DataBind();
                    break;
            }
        }
    }
}