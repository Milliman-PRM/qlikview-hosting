using CLSBusinessLogic;
using CLSdbContext;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace CLSMedicareReimbursement
{
    public partial class _default : System.Web.UI.Page
    {
        private string SessionKey_Selections = @"CurrentSelections";
        private string SessionKey_DataSet = @"CurrentDataSet";
        private System.Drawing.Color Selected = System.Drawing.Color.LightGreen;
        private System.Drawing.Color UnSelected = System.Drawing.SystemColors.Control;

        #region Page Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var BLM = BusinessLogicManager.GetInstance();
                if (!IsPostBack)
                {
                    LoadData(BLM);
                }
                else
                {
                    //since the year drop down is not in a ajax panel, it causes a postback which results
                    //in the menu being displayed, do check, if triggred by year drop down, then hide the menu
                    Control C = GetControlThatCausedPostBack(this);
                    if ((C != null) && (string.Compare(C.ClientID, "yeardropdown", true) == 0))
                    {
                        ContainerMenuList.Visible = false; //keep the menu hidden on postback from drop, its not in an ajax panel  
                        //ClientScript.RegisterStartupScript(this.GetType(), "Popup", "closeWindow('ContainerMenuList');", true);
                        //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Popup", "closeWindow('ContainerMenuList');", true);
                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "Popup", "closeWindow('ContainerMenuList');;return false;", true);
                    }

                }

                //these items have to be bound on each call
                AnalyzerSearch.DataSource = BLM.UniqueAnalyzers;
                AnalyzerSearch.DataBind();
                //we always allow searching across all entries
                AssayDescriptionSearch.DataSource = BLM.UniqueAssayDescriptions;
                AssayDescriptionSearch.DataBind();

                LocalitySearch.DataSource = BLM.UniqueLocalities;
                LocalitySearch.DataBind();

                CptCodeSearch.DataSource = BLM.UniqueCPTCode;
                CptCodeSearch.DataBind();
            }
            catch (Exception ex)
            {
                throw new Exception("Page_Load", ex);
            }

        }

        protected void LaunchMenu_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ContainerMenuList.Visible = true;
                var CurrentSels = Session[SessionKey_Selections] as BusinessLogicManager.CurrentSelections;

                var AssociatedAssayDescriptions = new List<SearchTerm>();
                foreach (string Analyzer in CurrentSels.AnalyzerNames)
                {
                    ListItem LI = AnalyzerCheckList.Items.FindByText(Analyzer);
                    if (LI != null)
                    {
                        LI.Selected = true;
                    }
                    //create a running list of all available
                    var AnalyzerID = BusinessLogicManager.GetInstance().FindAnalyzerIDFromName(Analyzer);
                    AssociatedAssayDescriptions.AddRange(BusinessLogicManager.GetInstance().FindAssayDescriptionForAnalyzer(new List<string>() { AnalyzerID }));
                }

                foreach (string SearchTerm in CurrentSels.SearchTermDescs)
                {
                    ListItem LI = AssayDescriptionList.Items.FindByText(SearchTerm);
                    if (LI != null)
                        LI.Selected = true;
                    else
                        AssayDescriptionList.Items.Add(SearchTerm);  //just add it
                }

                LocalityList.ClearSelection();  //clear out and restore localaties each time
                foreach (string LocalityID in CurrentSels.LocalatiesByDescShrt)
                {
                    var LocalityText = LocalityID;
                    ListItem LI = LocalityList.Items.FindByText(LocalityText);
                    if (LI != null)
                        LI.Selected = true;
                }

                CptCodeList.ClearSelection();  //clear out and restore item each time
                foreach (var item in CurrentSels.CPTCodes)
                {
                    var cptCodeSearchText = item;
                    ListItem LI = CptCodeList.Items.FindByText(cptCodeSearchText);
                    if (LI != null)
                        LI.Selected = true;
                }

                ScrollSelectedAnalyzerIntoView();
                ScrollSelectedAssayDescriptionIntoView();
                ScrollSelectedCPTCodeIntoView();
                ScrollSelectedLocalityIntoView();
            }
            catch (Exception ex)
            {
                throw new Exception("LaunchMenu_Click", ex);
            }
        }

        protected void btnViewSelection_Click(object sender, EventArgs e)
        {
            try
            {
                var CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;
                RebindPrimaryGrid(CurrentSels);
            }
            catch (Exception ex)
            {
                throw new Exception("LaunchMenu_Click", ex);
            }
        }

        /// <summary>
        /// Event for Page Inintilize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClearSelection_Click(object sender, EventArgs e)
        {
            try
            {
                InitilizePageControls();
            }
            catch (Exception ex)
            {
                throw new Exception("LaunchMenu_Click", ex);
            }

        }

        /// <summary>
        /// user clicked on analyzer list, see what changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AnalyzerCheckList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //get old user selections 
                var CurrentSels = Session[SessionKey_Selections] as BusinessLogicManager.CurrentSelections;
                //update user selections
                var SelectedAnalyzerNames = AnalyzerCheckList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Text).ToList();
                //update the users selection object instance
                CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.ANALYZERNAMES);

                foreach (string Selected in SelectedAnalyzerNames)
                    CurrentSels.AddToList(BusinessLogicManager.CurrentSelections.QueryFieldNames.ANALYZERNAMES, Selected);
                AssayDescriptionList.Items.Clear();

                //need to get all the analyzers ids
                var SelectedAnalyzerIDs = new List<string>();
                foreach (string AnalyzerName in SelectedAnalyzerNames)
                    SelectedAnalyzerIDs.AddRange(BusinessLogicManager.GetInstance().FindAnalyzerIDsFromName(AnalyzerName));

                //SelectedAnalyzerIDs = new List<string>() { "12", "8", "9", "10", "11", "13", "14", "15", "16", "17" };
                if (SelectedAnalyzerIDs.Count() > 0)  //user selected at least 1 analyzer, so update assay list
                {
                    var AssociatedAssayDescriptions = BusinessLogicManager.GetInstance().FindAssayDescriptionForAnalyzer(SelectedAnalyzerIDs);
                    AssayDescriptionList.DataSource = AssociatedAssayDescriptions;
                    AssayDescriptionList.DataTextField = "SearchDesc";
                    AssayDescriptionList.DataValueField = "Id";
                    AssayDescriptionList.DataBind();

                    //get list of all cptcodes for analyzerids
                    var AssociatedCptCodess = BusinessLogicManager.GetInstance().FindCptCodesForAnalyzer(SelectedAnalyzerIDs);
                    CptCodeList.DataSource = AssociatedCptCodess;
                    CptCodeList.DataTextField = "Code1";
                    CptCodeList.DataValueField = "Id";
                    CptCodeList.DataBind();

                }
                else  //user un-selected, there are no analyzers selected, so re-show entire list, but highlight analyzer list items if 1 assay description is selected
                {
                    //reset everything here to clear out system
                    CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.ALL);
                    LocalityList.ClearSelection();  //clear out all selections                
                    CptCodeList.ClearSelection();  //clear out all selections

                    //rebind to full list for display
                    var BLM = BusinessLogicManager.GetInstance();
                    PopulateAssayDescriptionList(BLM);
                    PopulateCPTCodeList(BLM);
                    PopulateLocalityList(BLM);

                    if (CurrentSels.SearchTermDescs.Count() == 1)
                        AssayDescriptionList.Items.FindByText(CurrentSels.SearchTermDescs[0]).Selected = true;
                }

                // Do unselected stuff
                ScrollSelectedAnalyzerIntoView();
            }
            catch (Exception ex)
            {
                throw new Exception("AnalyzerCheckList_SelectedIndexChanged", ex);
            }
        }

        protected void AssayDescriptionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //get user selections
                var CurrentSels = Session[SessionKey_Selections] as BusinessLogicManager.CurrentSelections;
                var SearchTermIDs = AssayDescriptionList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Value).ToList();
                var SearchTermTexts = AssayDescriptionList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Text).ToList();

                //test behavior that when we are here there can be only 1 selection
                CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS);

                if (CurrentSels.NoSelectionsMade())
                {    //no selections have been made, so we need to look up the appropriate analyziers and set to checked
                    var Analyzers = BusinessLogicManager.GetInstance().FindAnalyzersForAssayDescription(AssayDescriptionList.SelectedItem.Text).ToList();
                    CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.ALL);
                    CurrentSels.AddToList(BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS, AssayDescriptionList.SelectedItem.Text);
                }
                else
                {
                    //selections have been made, we are narrowing the search down
                    CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMIDS);
                    CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS);

                    foreach (string SearchTermText in SearchTermTexts)
                        CurrentSels.AddToList(BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS, SearchTermText);
                }

                ScrollSelectedAssayDescriptionIntoView();
            }
            catch (Exception ex)
            {
                throw new Exception("AssayDescriptionList_SelectedIndexChanged", ex);
            }
        }

        protected void LocalityList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //get user selections
                var CurrentSels = Session[SessionKey_Selections] as BusinessLogicManager.CurrentSelections;
                var SelectedLocalityIDs = LocalityList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Value).ToList();
                var SelectedLocalityShorts = LocalityList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Text).ToList();
                CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.LOCALATIESBYDESCSHRT);
                //update the users selection object instance
                foreach (string SelectedLocalityShort in SelectedLocalityShorts)
                    CurrentSels.AddToList(BusinessLogicManager.CurrentSelections.QueryFieldNames.LOCALATIESBYDESCSHRT, SelectedLocalityShort);

                ScrollSelectedLocalityIntoView();
            }
            catch (Exception ex)
            {
                throw new Exception("LocalityList_SelectedIndexChanged", ex);
            }
        }

        protected void CptCodeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //get user selections
                var CurrentSels = Session[SessionKey_Selections] as BusinessLogicManager.CurrentSelections;
                var SelectedIDs = CptCodeList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Value).ToList();
                var SelectedText = CptCodeList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Text).ToList();
                CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.CPTCODES);
                //update the users selection object instance
                foreach (string item in SelectedText)
                    CurrentSels.AddToList(BusinessLogicManager.CurrentSelections.QueryFieldNames.CPTCODES, item);

                ScrollSelectedCPTCodeIntoView();
            }
            catch (Exception ex)
            {
                throw new Exception("CptCodeList_SelectedIndexChanged", ex);
            }
        }

        protected void AnalyzerSearch_Search(object sender, Telerik.Web.UI.SearchBoxEventArgs e)
        {
            //user selected from search
            ListItem LI = AnalyzerCheckList.Items.FindByText(AnalyzerSearch.Text);
            if (LI != null)
            {
                LI.Selected = true;
                AnalyzerCheckList_SelectedIndexChanged(null, null);
            }
            AnalyzerSearch.Text = "";
        }

        protected void AssayDescriptionSearch_Search(object sender, Telerik.Web.UI.SearchBoxEventArgs e)
        {
            ListItem LI = AssayDescriptionList.Items.FindByText(AssayDescriptionSearch.Text);
            if (LI != null)
            {
                LI.Selected = true;
                AssayDescriptionList_SelectedIndexChanged(null, null);
            }
            else
            {
                //special case,  they have selected a search item that is not in the current list so we clear search and reset back
                //to a full search 

                var CurrentSels = Session[SessionKey_Selections] as BusinessLogicManager.CurrentSelections;
                CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.ALL);
                CurrentSels.AddToList(BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS, AssayDescriptionSearch.Text);

                AssayDescriptionList.DataSource = BusinessLogicManager.GetInstance().UniqueAssayDescriptions;
                AssayDescriptionList.DataBind();

                LI = AssayDescriptionList.Items.FindByText(AssayDescriptionSearch.Text);
                LI.Selected = true;

                AnalyzerCheckList.ClearSelection();
                LocalityList.ClearSelection();
                CptCodeList.ClearSelection();

                AssayDescriptionList_SelectedIndexChanged(null, null);
            }
            AssayDescriptionSearch.Text = "";
        }

        protected void LocalitySearch_Search(object sender, Telerik.Web.UI.SearchBoxEventArgs e)
        {
            ListItem LI = LocalityList.Items.FindByText(LocalitySearch.Text);
            if (LI != null)
            {
                LI.Selected = true;
                LocalityList_SelectedIndexChanged(null, null);
            }
            LocalitySearch.Text = "";
        }

        protected void CptCodeSearch_Search(object sender, SearchBoxEventArgs e)
        {
            ListItem LI = CptCodeList.Items.FindByText(CptCodeSearch.Text);
            if (LI != null)
            {
                LI.Selected = true;
                CptCodeList_SelectedIndexChanged(null, null);
            }
            CptCodeSearch.Text = "";
        }


        protected void YearDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            //should we requery against new year that has been selected
            var CurrentSels = Session[SessionKey_Selections] as BusinessLogicManager.CurrentSelections;

            //first check if are not changing selections do nothing
            if (CurrentSels.Years.Contains(YearDropdown.SelectedItem.Text) == true)
                return;  //we are already showing data for this year

            if (CurrentSels.NoSelectionsMade())
            {
                System.Data.DataTable ResultSet = BusinessLogicManager.GetInstance().DataByYear[YearDropdown.SelectedItem.Text];
                RatesGrid.VirtualItemCount = ResultSet.Rows.Count;
                RatesGrid.DataSource = ResultSet;
                RatesGrid.DataBind();
            }
            else
            {
                //change year, and requery
                CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.YEARS);
                CurrentSels.AddToList(BusinessLogicManager.CurrentSelections.QueryFieldNames.YEARS, YearDropdown.SelectedItem.Text);
                RebindPrimaryGrid(CurrentSels);
            }
        }

        protected void RadAjaxManager1_AjaxRequest(object sender, Telerik.Web.UI.AjaxRequestEventArgs e)
        {
            RatesGrid.PageSize = 10 + RatesGrid.PageSize;
            RatesGrid.Rebind();
        }

        protected void RadGridPanel_AjaxRequest(object sender, Telerik.Web.UI.AjaxRequestEventArgs e)
        {
            var CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;
            switch (e.Argument)
            {
                case "refresh":

                    RebindPrimaryGrid(CurrentSels);
                    break;
            }
        }

        protected void RatesGrid_SelectedCellChanged(object sender, EventArgs e)
        {
            try
            {

                if (RatesGrid.SelectedCells.Count == 1)
                {
                    var CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;

                    var SelectedValue = RatesGrid.SelectedCells[0].Text;
                    var SelectedColumn = RatesGrid.SelectedCells[0].Column.UniqueName;
                    //analyzer_name description locality_description
                    switch (SelectedColumn.ToLower())
                    {
                        case "analyzer_name":
                            CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.ANALYZERNAMES);
                            CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.ANALYZERIDS);
                            CurrentSels.AddToList(BusinessLogicManager.CurrentSelections.QueryFieldNames.ANALYZERNAMES, SelectedValue);
                            RebindPrimaryGrid(CurrentSels);
                            break;
                        case "description":
                            CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMBYCODEIDS);
                            CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS);
                            CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMIDS);
                            CurrentSels.AddToList(BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS, SelectedValue);
                            RebindPrimaryGrid(CurrentSels);
                            break;
                        case "locality_description":
                            CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.LOCALATIES);
                            CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.LOCALATIESBYDESCSHRT);
                            CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.LOCALATIESIDS);
                            CurrentSels.AddToList(BusinessLogicManager.CurrentSelections.QueryFieldNames.LOCALATIESBYDESCSHRT, SelectedValue);
                            RebindPrimaryGrid(CurrentSels);
                            break;
                        case "code":  //we are supposed to query on this field too, but not yet, no way to clear it really
                            CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.CPTCODES);
                            CurrentSels.AddToList(BusinessLogicManager.CurrentSelections.QueryFieldNames.CPTCODES, SelectedValue);
                            RebindPrimaryGrid(CurrentSels);
                            break;
                        default:
                            RatesGrid.SelectedIndexes.Clear();  //not valid search columns
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("RatesGrid_SelectedCellChanged", ex);
            }
        }

        protected void RatesGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            System.Data.DataTable DataSet = Session[SessionKey_DataSet] as System.Data.DataTable;
            if (DataSet != null)
            {
                int PageIndex = RatesGrid.CurrentPageIndex;
                int PageSize = RatesGrid.PageSize;
                int StartAt = PageIndex * PageSize;
                int ItemCount = PageSize;
                if (StartAt + ItemCount >= DataSet.Rows.Count)
                    ItemCount = (DataSet.Rows.Count - StartAt);
                int EndAt = StartAt + ItemCount;

                //copies over the definations, but not the data itself
                System.Data.DataTable DataSubSet = DataSet.Clone();
                //copy over the data
                for (int Index = StartAt; Index < EndAt; Index++)
                    DataSubSet.Rows.Add(DataSet.Rows[Index].ItemArray);
                //add the subset to the view
                RatesGrid.DataSource = DataSubSet;
            }

        }

        protected void RatesGrid_SortCommand(object sender, Telerik.Web.UI.GridSortCommandEventArgs e)
        {
            //string SortOnColumn = e.CommandArgument.ToString();
            //string SortMethod = "DESC";
            //if (e.NewSortOrder == Telerik.Web.UI.GridSortOrder.Ascending)
            //    SortMethod = "ASC";

            //System.Data.DataTable CurrentTable = Session[SessionKey_DataSet] as System.Data.DataTable;

            //CurrentTable.DefaultView.Sort = SortOnColumn + " " + SortMethod;            
        }

        #endregion

        #region Custom
        private void LoadData(BusinessLogicManager BLM)
        {
            //make a session holder for the users selections 
            var CurrentSelections = new BusinessLogicManager.CurrentSelections();
            if (Session[SessionKey_Selections] == null)
                Session[SessionKey_Selections] = CurrentSelections;

            PopulateFooterList(BLM);
            if (BLM.WebURL.Count > 0)
            {
                lblFooterLink.Text = BLM.WebURL[0].Displaytext;
                FooterLink.Text = BLM.WebURL[0].Webaddressurl;
                FooterLink.NavigateUrl = BLM.WebURL[0].Webaddressurl;
            }

            PopulateAnalyzerCheckList(BLM);
            PopulateAssayDescriptionList(BLM);
            PopulateCPTCodeList(BLM);
            PopulateLocalityList(BLM);
            PopulateYearDropdown(BLM);

            YearDropdown.DataSource = BLM.UniqueYears.OrderByDescending(i => i);
            YearDropdown.DataBind();
            YearDropdown.SelectedIndex = 0;
            CurrentSelections.AddToList(BusinessLogicManager.CurrentSelections.QueryFieldNames.YEARS, YearDropdown.SelectedItem.Text);

            //set current working set in memory "DataSet" for main view grid
            Session[SessionKey_DataSet] = BLM.DataByYear[YearDropdown.SelectedItem.Text];
            RatesGrid.VirtualItemCount = BLM.DataByYear[YearDropdown.SelectedItem.Text].Rows.Count;
        }

        private void PopulateFooterList(BusinessLogicManager BLM)
        {
            //to remove special character "Â" from footnote
            var footNotes = BLM.FootNotes.ToList();
            //in the list remove the special characters
            var newFootNote = (from f in footNotes
                            select new Footnote
                            {
                                Footnote1 = f.Footnote1.Replace(@"Â", ""),
                                Id = f.Id
                            }).OrderBy(o=>o.Id).ToList();
            FooterList.DataSource = newFootNote;
            FooterList.DataTextField = "Footnote1";
            FooterList.DataValueField = "Id";
            FooterList.DataBind();
        }

        private void PopulateYearDropdown(BusinessLogicManager BLM)
        {
            YearDropdown.DataSource = BLM.UniqueYears.OrderByDescending(i => i);
            YearDropdown.DataBind();
            YearDropdown.SelectedIndex = 0;
        }

        private void PopulateLocalityList(BusinessLogicManager BLM)
        {
            LocalityList.DataSource = BLM.UniqueLocalities;
            LocalityList.DataTextField = "LocalityDescription";
            LocalityList.DataValueField = "Id";
            LocalityList.DataBind();
        }

        private void PopulateAssayDescriptionList(BusinessLogicManager BLM)
        {
            AssayDescriptionList.DataSource = BLM.UniqueAssayDescriptions;
            AssayDescriptionList.DataTextField = "SearchDesc";
            AssayDescriptionList.DataValueField = "Id";
            AssayDescriptionList.DataBind();
        }

        private void PopulateAnalyzerCheckList(BusinessLogicManager BLM)
        {
            AnalyzerCheckList.DataSource = BLM.UniqueAnalyzers;
            AnalyzerCheckList.DataTextField = "AnalyzerName";
            AnalyzerCheckList.DataValueField = "Id";
            AnalyzerCheckList.DataBind();
        }
        private void PopulateCPTCodeList(BusinessLogicManager BLM)
        {
            CptCodeList.DataSource = BLM.UniqueCPTCode;
            CptCodeList.DataTextField = "Code1";
            CptCodeList.DataValueField = "Id";
            CptCodeList.DataBind();
        }

        private bool RebindPrimaryGrid(BusinessLogicManager.CurrentSelections CurrentSet)
        {
            try
            {
                var CurrentSels = Session[SessionKey_Selections] as BusinessLogicManager.CurrentSelections;
                if (CurrentSels.NoSelectionsMade() == false)
                {
                    System.Data.DataTable ResultSet = BusinessLogicManager.GetInstance().FetchDataForSelections(CurrentSet);
                    Session[SessionKey_DataSet] = ResultSet;
                    RatesGrid.VirtualItemCount = ResultSet.Rows.Count;
                    RatesGrid.DataSource = ResultSet;
                    RatesGrid.DataBind();
                    return true;
                }
                else //no selection return to original state
                {
                    Session[SessionKey_DataSet] = BusinessLogicManager.GetInstance().DataByYear[YearDropdown.SelectedItem.Text];
                    RatesGrid.VirtualItemCount = BusinessLogicManager.GetInstance().DataByYear[YearDropdown.SelectedItem.Text].Rows.Count;
                    RatesGrid.DataSource = BusinessLogicManager.GetInstance().DataByYear[YearDropdown.SelectedItem.Text];
                    RatesGrid.DataBind();
                }
            }
            catch (Exception)
            {
                //report error
            }
            return false;
        }

        /// <summary>
        /// Method to re-organize the selected check list to appear top of list
        /// </summary>
        private void ScrollSelectedAnalyzerIntoView()
        {
            //Code to sort Analyzer list
            var dt = new DataTable();
            dt.Columns.Add("AnalyzerName", typeof(string));
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Selected", typeof(bool));
            var holder = new ArrayList();

            if (IsPostBack)
            {
                foreach (ListItem item in AnalyzerCheckList.Items)
                {
                    if (item.Selected == true)
                    {
                        dt.Rows.Add(item.Text, item.Value, true);
                        AnalyzerCheckList.Items.FindByValue(item.Value).Selected = true;
                        holder.Add(item.Value);
                    }
                    else
                    {
                        dt.Rows.Add(item.Text, item.Value, false);
                    }
                }
            }
            else
            {
                foreach (ListItem item in AnalyzerCheckList.Items)
                {
                    dt.Rows.Add(item.Text, item.Value, false);
                }
            }

            var dv = new DataView(dt);
            dv.Sort = "Selected DESC";
            AnalyzerCheckList.DataSource = dv;
            AnalyzerCheckList.DataTextField = "AnalyzerName";
            AnalyzerCheckList.DataValueField = "Id";
            AnalyzerCheckList.DataBind();

            foreach (String hItem in holder)
            {
                AnalyzerCheckList.Items.FindByValue(hItem).Selected = true;
            }

            for (int i = 0; i < AnalyzerCheckList.Items.Count; i++)
            {
                if (AnalyzerCheckList.Items[i].Selected)
                {
                    AnalyzerCheckList.Items[i].Attributes.Add("style", "font-weight: bold;");
                }
            }
        }

        private void ScrollSelectedAssayDescriptionIntoView()
        {
            //Code to sort Analyzer list
            var dt = new DataTable();
            dt.Columns.Add("SearchDesc", typeof(string));
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Selected", typeof(bool));
            var holder = new ArrayList();

            if (IsPostBack)
            {
                foreach (ListItem item in AssayDescriptionList.Items)
                {
                    if (item.Selected == true)
                    {
                        dt.Rows.Add(item.Text, item.Value, true);
                        AssayDescriptionList.Items.FindByValue(item.Value).Selected = true;
                        holder.Add(item.Value);
                    }
                    else
                    {
                        dt.Rows.Add(item.Text, item.Value, false);
                    }
                }
            }
            else
            {
                foreach (ListItem item in AssayDescriptionList.Items)
                {
                    dt.Rows.Add(item.Text, item.Value, false);
                }
            }

            var dv = new DataView(dt);
            dv.Sort = "Selected DESC";
            AssayDescriptionList.DataSource = dv;
            AssayDescriptionList.DataTextField = "SearchDesc";
            AssayDescriptionList.DataValueField = "Id";
            AssayDescriptionList.DataBind();

            foreach (String hItem in holder)
            {
                AssayDescriptionList.Items.FindByValue(hItem).Selected = true;
            }

            for (int i = 0; i < AssayDescriptionList.Items.Count; i++)
            {
                if (AssayDescriptionList.Items[i].Selected)
                {
                    AssayDescriptionList.Items[i].Attributes.Add("style", "font-weight: bold;");
                }
            }
        }

        private void ScrollSelectedCPTCodeIntoView()
        {
            //Code to sort Analyzer list
            var dt = new DataTable();
            dt.Columns.Add("Code1", typeof(string));
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Selected", typeof(bool));
            var holder = new ArrayList();

            if (IsPostBack)
            {
                foreach (ListItem item in CptCodeList.Items)
                {
                    if (item.Selected == true)
                    {
                        dt.Rows.Add(item.Text, item.Value, true);
                        CptCodeList.Items.FindByValue(item.Value).Selected = true;
                        holder.Add(item.Value);
                    }
                    else
                    {
                        dt.Rows.Add(item.Text, item.Value, false);
                    }
                }
            }
            else
            {
                foreach (ListItem item in CptCodeList.Items)
                {
                    dt.Rows.Add(item.Text, item.Value, false);
                }
            }

            var dv = new DataView(dt);
            dv.Sort = "Selected DESC";
            CptCodeList.DataSource = dv;
            CptCodeList.DataTextField = "Code1";
            CptCodeList.DataValueField = "Id";
            CptCodeList.DataBind();

            foreach (String hItem in holder)
            {
                CptCodeList.Items.FindByValue(hItem).Selected = true;
            }

            for (int i = 0; i < CptCodeList.Items.Count; i++)
            {
                if (CptCodeList.Items[i].Selected)
                {
                    CptCodeList.Items[i].Attributes.Add("style", "font-weight: bold;");
                }
            }
        }

        private void ScrollSelectedLocalityIntoView()
        {
            //Code to sort Analyzer list
            var dt = new DataTable();
            dt.Columns.Add("LocalityDescription", typeof(string));
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Selected", typeof(bool));
            var holder = new ArrayList();

            if (IsPostBack)
            {
                foreach (ListItem item in LocalityList.Items)
                {
                    if (item.Selected == true)
                    {
                        dt.Rows.Add(item.Text, item.Value, true);
                        LocalityList.Items.FindByValue(item.Value).Selected = true;
                        holder.Add(item.Value);
                    }
                    else
                    {
                        dt.Rows.Add(item.Text, item.Value, false);
                    }
                }
            }
            else
            {
                foreach (ListItem item in LocalityList.Items)
                {
                    dt.Rows.Add(item.Text, item.Value, false);
                }
            }

            var dv = new DataView(dt);
            dv.Sort = "Selected DESC";
            LocalityList.DataSource = dv;
            LocalityList.DataTextField = "LocalityDescription";
            LocalityList.DataValueField = "Id";
            LocalityList.DataBind();

            foreach (String hItem in holder)
            {
                LocalityList.Items.FindByValue(hItem).Selected = true;
            }

            for (int i = 0; i < LocalityList.Items.Count; i++)
            {
                if (LocalityList.Items[i].Selected)
                {
                    LocalityList.Items[i].Attributes.Add("style", "font-weight: bold;");
                }
            }
        }

        /// <summary>
        /// Reset the page and clear session varibales
        /// </summary>
        public void InitilizePageControls()
        {            
            var CurrentSels = Session[SessionKey_Selections] as BusinessLogicManager.CurrentSelections;
            //reset everything here to clear out system
            CurrentSels.Clear(BusinessLogicManager.CurrentSelections.QueryFieldNames.ALL);

            Session[SessionKey_Selections] = null;
            Session[SessionKey_DataSet] = null;
            YearDropdown.SelectedIndex = 0;

            var count = AnalyzerCheckList.Items.Count;
            for (int i = 0; i < count; i++)
            {
                if (AnalyzerCheckList.Items[i].Selected == true)
                    AnalyzerCheckList.Items[i].Selected = false;
            }
            UnHighlightAnalyzers();
            //CheckBoxList
            AnalyzerSearch.Text = "";

            foreach (ListItem item in AssayDescriptionList.Items)
            {
                //check anything out here
                if (item.Selected)
                    item.Selected = false;
            }
            //ListBox
            AssayDescriptionList.ClearSelection();
            AssayDescriptionList.SelectedIndex = -1;
            AssayDescriptionSearch.Text = "";

            foreach (ListItem item in LocalityList.Items)
            {
                //check anything out here
                if (item.Selected)
                    item.Selected = false;
            }
            LocalityList.ClearSelection();
            LocalityList.SelectedIndex = -1;
            LocalitySearch.Text = "";

            foreach (ListItem item in CptCodeList.Items)
            {
                //check anything out here
                if (item.Selected)
                    item.Selected = false;
            }
            CptCodeList.ClearSelection();
            CptCodeList.SelectedIndex = -1;
            CptCodeSearch.Text = "";

            var BLM = BusinessLogicManager.GetInstance();
            PopulateYearDropdown(BLM);
            PopulateAnalyzerCheckList(BLM);
            PopulateAssayDescriptionList(BLM);
            PopulateCPTCodeList(BLM);
            PopulateLocalityList(BLM);

            var ResultSet = BusinessLogicManager.GetInstance().DataByYear[YearDropdown.SelectedItem.Text];
            RatesGrid.VirtualItemCount = ResultSet.Rows.Count;            
            RatesGrid.DataSource = ResultSet;
            RatesGrid.DataBind();

            CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;
            RebindPrimaryGrid(CurrentSels);
            
           //Add the necessary AJAX setting programmatically
            RadAjaxManager1.AjaxSettings.AddAjaxSetting(RatesGrid, RadGridPanel, null);

            //ContainerMenuList.Visible = false; //keep the menu hidden on postback from drop, its not in an ajax panel  
            //ClientScript.RegisterStartupScript(this.GetType(), "Popup", "closeWindow('ContainerMenuList');", true);
            //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Popup", "closeWindow('ContainerMenuList');", true);
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "Popup", "closeWindow('ContainerMenuList');;return false;", true);
        }

        private Control GetControlThatCausedPostBack(Page page)
        {
            //initialize a control and set it to null
            Control ctrl = null;

            //get the event target name and find the control
            string ctrlName = page.Request.Params.Get("__EVENTTARGET");
            if (!String.IsNullOrEmpty(ctrlName))
                ctrl = page.FindControl(ctrlName);

            //return the control to the calling method
            return ctrl;
        }

        //Hilight or un-highlight entries in Analyzer list
        private void UnHighlightAnalyzers()
        {
            foreach (ListItem LI in AnalyzerCheckList.Items)
                LI.Attributes.Add("style", "font-weight:normal;color:black");
        }

        private void HighlightAnalyzers(List<string> ItemNames)
        {
            foreach (ListItem LI in AnalyzerCheckList.Items)
            {
                if (ItemNames.Contains(LI.Text))
                    LI.Attributes.Add("style", "font-weight:bold;color:red");
            }
        }

        #endregion

    }
}