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
        private string SessionKey_DataSet = @"CurrentDataSet";

        protected void Page_Load(object sender, EventArgs e)
        {
            CLSBusinessLogic.BusinessLogicManager BLM = CLSBusinessLogic.BusinessLogicManager.GetInstance();

            if (!IsPostBack)
            {

                //make a session holder for the users selections 
                CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSelections = new CLSBusinessLogic.BusinessLogicManager.CurrentSelections();
                if (Session[SessionKey_Selections] == null)
                    Session[SessionKey_Selections] = CurrentSelections;

                //to remove special character "Â" from footnote
                var footNotes = new List<CLSdbContext.Footnote>();
                var newFootNote = new CLSdbContext.Footnote();
                foreach (var item in BLM.FootNotes)
                {
                    if (item.Footnote1.Contains(@"Â"))
                    {
                        newFootNote.Id = item.Id;
                        newFootNote.Footnote1 = item.Footnote1.Replace(@"Â", string.Empty);
                    }
                    else
                    {
                        newFootNote.Id = item.Id;
                        newFootNote.Footnote1 = item.Footnote1;
                    }
                    footNotes.Add(newFootNote);
                    newFootNote = new CLSdbContext.Footnote();
                }


                FooterList.DataSource = footNotes;
                FooterList.DataTextField = "Footnote1";
                FooterList.DataValueField = "Id";
                FooterList.DataBind();

                if (BLM.WebURL.Count > 0)
                {
                    lblFooterLink.Text = BLM.WebURL[0].Displaytext;
                    FooterLink.Text = BLM.WebURL[0].Webaddressurl;
                    FooterLink.NavigateUrl = BLM.WebURL[0].Webaddressurl;
                }

                AnalyzerCheckList.DataSource = BLM.UniqueAnalyzers;
                AnalyzerCheckList.DataTextField = "AnalyzerName";
                AnalyzerCheckList.DataValueField = "Id";
                AnalyzerCheckList.DataBind();

                AssayDescriptionList.DataSource = BLM.UniqueAssayDescriptions;
                AssayDescriptionList.DataTextField = "SearchDesc";
                AssayDescriptionList.DataValueField = "Id";
                AssayDescriptionList.DataBind();

                LocalityList.DataSource = BLM.UniqueLocalities;
                LocalityList.DataTextField = "LocalityDescription";
                LocalityList.DataValueField = "Id";
                LocalityList.DataBind();

                YearDropdown.DataSource = BLM.UniqueYears.OrderByDescending(i => i);
                YearDropdown.DataBind();
                YearDropdown.SelectedIndex = 0;
                CurrentSelections.AddToList(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.YEARS, YearDropdown.SelectedItem.Text);

                //set current working set in memory "DataSet" for main view grid
                Session[SessionKey_DataSet] = BLM.DataByYear[YearDropdown.SelectedItem.Text];
                RatesGrid.VirtualItemCount = BLM.DataByYear[YearDropdown.SelectedItem.Text].Rows.Count;

            }
            else
            {
                //since the year drop down is not in a ajax panel, it causes a postback which results
                //in the menu being displayed, do check, if triggred by year drop down, then hide the menu
                Control C = GetControlThatCausedPostBack(this);
                if ((C != null) && (string.Compare(C.ClientID, "yeardropdown", true) == 0))
                    menu.Visible = false; //keep the menu hidden on postback from drop, its not in an ajax panel
            }


            //these items have to be bound on each call
            AnalyzerSearch.DataSource = BLM.UniqueAnalyzers;
            AnalyzerSearch.DataBind();
            //we always allow searching across all entries
            AssayDescriptionSearch.DataSource = BLM.UniqueAssayDescriptions;
            AssayDescriptionSearch.DataBind();

            LocalitySearch.DataSource = BLM.UniqueLocalities;
            LocalitySearch.DataBind();
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

        private void CheckForPreviousSelections()
        {
            Toast.Text = "Hi Van";
            Toast.VisibleOnPageLoad = true;
        }


        protected void RatesGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            System.Data.DataTable DataSet = Session[SessionKey_DataSet] as System.Data.DataTable;
            if (DataSet != null)
            {
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
            //string SortOnColumn = e.CommandArgument.ToString();
            //string SortMethod = "DESC";
            //if (e.NewSortOrder == Telerik.Web.UI.GridSortOrder.Ascending)
            //    SortMethod = "ASC";

            //System.Data.DataTable CurrentTable = Session[SessionKey_DataSet] as System.Data.DataTable;

            //CurrentTable.DefaultView.Sort = SortOnColumn + " " + SortMethod;

        }

        /// <summary>
        /// user clicked on analyzer list, see what changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AnalyzerCheckList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //get old user selections 
            CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;
            //update user selections
            List<string> SelectedAnalyzerNames = AnalyzerCheckList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Text).ToList();
            //update the users selection object instance
            CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.ANALYZERNAMES);  
            foreach (string Selected in SelectedAnalyzerNames)
                CurrentSels.AddToList(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.ANALYZERNAMES, Selected);
            AssayDescriptionList.Items.Clear();
            //need to get all the analyzers ids
            List<string> SelectedAnalyzerIDs = new List<string>();
            foreach (string AnalyzerName in SelectedAnalyzerNames)
                SelectedAnalyzerIDs.AddRange(CLSBusinessLogic.BusinessLogicManager.GetInstance().FindAnalyzerIDsFromName(AnalyzerName));

            //SelectedAnalyzerIDs = new List<string>() { "12", "8", "9", "10", "11", "13", "14", "15", "16", "17" };
            if (SelectedAnalyzerIDs.Count() > 0)  //user selected at least 1 analyzer, so update assay list
            {
                List<CLSdbContext.SearchTerm> AssociatedAssayDescriptions = CLSBusinessLogic.BusinessLogicManager.GetInstance().FindAssayDescriptionForAnalyzer(SelectedAnalyzerIDs);
                AssayDescriptionList.DataSource = AssociatedAssayDescriptions;
                AssayDescriptionList.DataTextField = "SearchDesc";
                AssayDescriptionList.DataValueField = "Id";
                AssayDescriptionList.DataBind();

                UnHighlightAnalyzers(); //something selected so turn off highlights
            }
            else  //user un-selected, there are no analyzers selected, so re-show entire list, but highlight analyzer list items if 1 assay description is selected
            {
                //reset everything here to clear out system
                CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.ALL);
                LocalityList.ClearSelection();  //clear out all selections

                //rebind to full list for display
                AssayDescriptionList.DataSource = CLSBusinessLogic.BusinessLogicManager.GetInstance().UniqueAssayDescriptions;
                AssayDescriptionList.DataTextField = "SearchDesc";
                AssayDescriptionList.DataValueField = "Id";
                AssayDescriptionList.DataBind();

                //no analyzer, check to see if SINGLE assay description highlighted
                NoAnalyzerSingleAssayDescriptionSelected();
                if (CurrentSels.SearchTermDescs.Count() == 1)
                    AssayDescriptionList.Items.FindByText(CurrentSels.SearchTermDescs[0]).Selected = true;
            }

        }

        protected void AssayDescriptionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //get user selections
            CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;
            List<string> SearchTermIDs = AssayDescriptionList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Value).ToList();
            List<string> SearchTermTexts = AssayDescriptionList.Items.Cast<ListItem>().Where(n => n.Selected).Select(n => n.Text).ToList();

            //test behavior that when we are here there can be only 1 selection
            CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS);

            if (CurrentSels.NoSelectionsMade())
            {    //no selections have been made, so we need to look up the appropriate analyziers and set to checked
                List<string> Analyzers = CLSBusinessLogic.BusinessLogicManager.GetInstance().FindAnalyzersForAssayDescription(AssayDescriptionList.SelectedItem.Text);
                CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.ALL);
                CurrentSels.AddToList(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS, AssayDescriptionList.SelectedItem.Text);
                HighlightAnalyzers(Analyzers);
            }
            else
            {
                UnHighlightAnalyzers();  //turn off any entries highlighted
                //selections have been made, we are narrowing the search down
                CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMIDS);
                CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS);

                foreach (string SearchTermText in SearchTermTexts)
                    CurrentSels.AddToList(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS, SearchTermText);
            }
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

            if (CurrentSels.NoSelectionsMade())
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
                RebindPrimaryGrid(CurrentSels);
            }
        }

        protected void RadGridPanel_AjaxRequest(object sender, Telerik.Web.UI.AjaxRequestEventArgs e)
        {
            CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;

            switch (e.Argument)
            {
                case "refresh":
                    RebindPrimaryGrid(CurrentSels);
                    break;
            }
        }

        protected void RatesGrid_SelectedCellChanged(object sender, EventArgs e)
        {
            if (RatesGrid.SelectedCells.Count == 1)
            {
                CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;

                string SelectedValue = RatesGrid.SelectedCells[0].Text;
                string SelectedColumn = RatesGrid.SelectedCells[0].Column.UniqueName;
                //analyzer_name description locality_description
                switch (SelectedColumn.ToLower())
                {
                    case "analyzer_name":
                        CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.ANALYZERNAMES);
                        CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.ANALYZERIDS);
                        CurrentSels.AddToList(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.ANALYZERNAMES, SelectedValue);
                        RebindPrimaryGrid(CurrentSels);
                        break; 
                    case "description":
                        CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMBYCODEIDS);
                        CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS);
                        CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMIDS);
                        CurrentSels.AddToList(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS, SelectedValue);
                        RebindPrimaryGrid(CurrentSels);
                        break;
                    case "locality_description":
                        CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.LOCALATIES);
                        CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.LOCALATIESBYDESCSHRT);
                        CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.LOCALATIESIDS);
                        CurrentSels.AddToList(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.LOCALATIESBYDESCSHRT, SelectedValue);
                        RebindPrimaryGrid(CurrentSels);
                        break;
                    //case "code":  we are supposed to query on this field too, but not yet, no way to clear it really
                    //    break;
                    default:
                        RatesGrid.SelectedIndexes.Clear();  //not valid search columns
                        break;
                }
            }
        }

        private bool RebindPrimaryGrid(CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSet)
        {
            try
            {
                CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;
                if (CurrentSels.NoSelectionsMade() == false)
                {
                    System.Data.DataTable ResultSet = CLSBusinessLogic.BusinessLogicManager.GetInstance().FetchDataForSelections(CurrentSet);
                    Session[SessionKey_DataSet] = ResultSet;
                    RatesGrid.VirtualItemCount = ResultSet.Rows.Count;
                    RatesGrid.DataSource = ResultSet;
                    RatesGrid.DataBind();
                    return true;
                }
                else //no selection return to original state
                {
                    Session[SessionKey_DataSet] = CLSBusinessLogic.BusinessLogicManager.GetInstance().DataByYear[YearDropdown.SelectedItem.Text];
                    RatesGrid.VirtualItemCount = CLSBusinessLogic.BusinessLogicManager.GetInstance().DataByYear[YearDropdown.SelectedItem.Text].Rows.Count;
                    RatesGrid.DataSource = CLSBusinessLogic.BusinessLogicManager.GetInstance().DataByYear[YearDropdown.SelectedItem.Text];
                    RatesGrid.DataBind();
                }
            }
            catch (Exception ex)
            {
                //report error
            }
            return false;
        }

        protected void LaunchMenu_Click(object sender, ImageClickEventArgs e)
        {
            menu.Visible = true;
            CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;

            List<CLSdbContext.SearchTerm> AssociatedAssayDescriptions = new List<CLSdbContext.SearchTerm>();
            foreach (string Analyzer in CurrentSels.AnalyzerNames)
            {
                ListItem LI = AnalyzerCheckList.Items.FindByText(Analyzer);
                if (LI != null)
                {
                    LI.Selected = true;
                }
                //create a running list of all available
                string AnalyzerID = CLSBusinessLogic.BusinessLogicManager.GetInstance().FindAnalyzerIDFromName(Analyzer);
                AssociatedAssayDescriptions.AddRange(CLSBusinessLogic.BusinessLogicManager.GetInstance().FindAssayDescriptionForAnalyzer(new List<string>() { AnalyzerID }));
            }

            foreach (string SearchTerm in CurrentSels.SearchTermDescs)
            { 
                ListItem LI = AssayDescriptionList.Items.FindByText(SearchTerm);
                if (LI != null)
                    LI.Selected = true;
                else
                    AssayDescriptionList.Items.Add(SearchTerm);  //just add it
            }

            //set highlights if necessary
            NoAnalyzerSingleAssayDescriptionSelected();
            LocalityList.ClearSelection();  //clear out and restore localaties each time
            foreach (string LocalityID in CurrentSels.LocalatiesIDs)
            {
                string LocalityText = CLSBusinessLogic.BusinessLogicManager.GetInstance().FindLocalityByID(LocalityID);
                ListItem LI = LocalityList.Items.FindByText(LocalityText);
                if (LI != null)
                    LI.Selected = true;
            }
        }

        private void NoAnalyzerSingleAssayDescriptionSelected()
        {
            CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;
            if ((CurrentSels.AnalyzerIDs.Count() == 0) && (CurrentSels.AnalyzerNames.Count() == 0) && (CurrentSels.AnalyzersByCodeIDs.Count() == 0) && (CurrentSels.SearchTermDescs.Count() == 1))
            {
                string SelectedValue = CurrentSels.SearchTermDescs[0];
                List<string> Analyzers = CLSBusinessLogic.BusinessLogicManager.GetInstance().FindAnalyzersForAssayDescription(SelectedValue);
                HighlightAnalyzers(Analyzers);
            }
            else
                UnHighlightAnalyzers();
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
            if ( LI != null )
            {
                LI.Selected = true;
                AssayDescriptionList_SelectedIndexChanged(null, null);
            }
            else
            {
                //special case,  they have selected a search item that is not in the current list so we clear search and reset back
                //to a full search 

                CLSBusinessLogic.BusinessLogicManager.CurrentSelections CurrentSels = Session[SessionKey_Selections] as CLSBusinessLogic.BusinessLogicManager.CurrentSelections;
                CurrentSels.Clear(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.ALL);
                CurrentSels.AddToList(CLSBusinessLogic.BusinessLogicManager.CurrentSelections.QueryFieldNames.SEARCHTERMDESCS, AssayDescriptionSearch.Text);
                AssayDescriptionList.DataSource = CLSBusinessLogic.BusinessLogicManager.GetInstance().UniqueAssayDescriptions;
                AssayDescriptionList.DataBind();
                LI = AssayDescriptionList.Items.FindByText(AssayDescriptionSearch.Text);
                LI.Selected = true;
                AnalyzerCheckList.ClearSelection();
                LocalityList.ClearSelection();
                AssayDescriptionList_SelectedIndexChanged(null, null);
            }
            AssayDescriptionSearch.Text = "";
        }

        protected void LocalitySearch_Search(object sender, Telerik.Web.UI.SearchBoxEventArgs e)
        {
            ListItem LI = LocalityList.Items.FindByText(LocalitySearch.Text);
            if ( LI != null )
            {
                LI.Selected = true;
                LocalityList_SelectedIndexChanged(null, null);
            }
            LocalitySearch.Text = "";
        }
    }
}