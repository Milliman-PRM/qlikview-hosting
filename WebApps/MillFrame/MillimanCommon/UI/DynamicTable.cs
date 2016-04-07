using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace MillimanCommon.UI
{
    public class DynamicTable
    {
        private System.Drawing.Color _EmptyCellBackground { get; set; }

        static public bool CreateTable(string Caption, List<string> NewValues, System.Web.UI.WebControls.PlaceHolder Container )
        {
            RadGrid grid = new RadGrid();
            grid.ViewStateMode = ViewStateMode.Disabled;
            grid.ID = Guid.NewGuid().ToString();
            grid.Skin = "Silk";
            grid.Width = Unit.Percentage(100);
            grid.PageSize = 15;
            grid.AllowPaging = false;
            grid.PagerStyle.Mode = GridPagerMode.NumericPages;
            grid.AutoGenerateColumns = false;
            //Add Customers table  
            grid.MasterTableView.Width = Unit.Percentage(100);
            grid.MasterTableView.DataKeyNames = new string[] { "Names" };
            GridBoundColumn boundColumn = new GridBoundColumn();
            boundColumn.DataField = "Names";
            boundColumn.HeaderText = Caption;
            boundColumn.Reorderable = true;
            boundColumn.ReadOnly = true;
            boundColumn.HeaderStyle.Width = Unit.Percentage(100);
            boundColumn.HeaderStyle.Font.Bold = true;
            boundColumn.HeaderStyle.Font.Italic = true;
            grid.MasterTableView.Columns.Add(boundColumn);
            // Add the grid to the placeholder  
            Container.Controls.Add(grid);
            grid.Visible = true;
            var bindableNames = from name in NewValues
                                select new { Names = name };
            grid.DataSource = bindableNames.ToList();
            grid.DataBind();
            return true;
        }

    /// <summary>
    /// Creates a dynamic table based on the requested column names and values provided
    /// </summary>
    /// <param name="Caption"></param>
    /// <param name="ColumnNames">Names of column headers</param>
    /// <param name="NewValues">Values to display</param>
    /// <param name="Container">Placeholder items used to contain the grid</param>
    /// <returns></returns>
        public bool CreateTable2DBinder(string Caption, List<string> ColumnNames, List<List<string>> NewValues, System.Web.UI.WebControls.PlaceHolder Container, System.Drawing.Color EmptyCellBK )
        {
            _EmptyCellBackground = EmptyCellBK;
            //verify and process grid, if grid contains null, return false
            //all column lenghts must be the same as lenght of ColumnNames
            int NumColumnNames = ColumnNames.Count;
            foreach( List<string> InnerList in NewValues )
            {
                if (InnerList == null)
                    return false;  //can't do anything with null lists
                while (InnerList.Count < NumColumnNames)
                    InnerList.Add("");
                while (InnerList.Count > NumColumnNames)
                    InnerList.RemoveAt(InnerList.Count - 1);
            }

            RadGrid grid = new RadGrid();
            grid.ViewStateMode = ViewStateMode.Disabled;
            grid.ID = Guid.NewGuid().ToString();
            grid.Skin = "Silk";
            grid.Width = Unit.Percentage(100);
            grid.PageSize = 15;
            grid.AllowPaging = false;
            grid.PagerStyle.Mode = GridPagerMode.NumericPages;
            grid.AutoGenerateColumns = true;
            //Add Customers table  
            grid.MasterTableView.Width = Unit.Percentage(100);
            // Add the grid to the placeholder  
            Container.Controls.Add(grid);
            grid.Visible = true;
            grid.ItemDataBound += grid_ItemDataBound;
            //create a data table for the data and bind it
            System.Data.DataTable dt = new System.Data.DataTable();
            foreach (string ColName in ColumnNames)
                dt.Columns.Add(ColName, Type.GetType("System.String"));
            for (int Index = 0; Index < NewValues.Count(); Index++)
            {
                dt.Rows.Add();
                for (int ColIndex = 0; ColIndex < ColumnNames.Count; ColIndex++)
                    dt.Rows[dt.Rows.Count - 1][ColumnNames[ColIndex]] = NewValues[Index][ColIndex];
            }
            grid.DataSource = dt;
            grid.DataBind();
            return true;
        }


        void grid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = e.Item as GridDataItem;
                int columnCount = ((System.Data.DataRowView)dataItem.DataItem).Row.Table.Columns.Count;
                string cellValue = string.Empty;
                string uniqueColumnname = string.Empty;

                for (int x = 0; x < columnCount; x++)
                {
                    uniqueColumnname = ((System.Data.DataRowView)dataItem.DataItem).Row.Table.Columns[x].ToString();
                    cellValue = ((System.Data.DataRowView)dataItem.DataItem)[uniqueColumnname].ToString();

                    if (string.IsNullOrEmpty(cellValue.ToString()))
                    {
                        TableCell cell = dataItem[uniqueColumnname];
                        cell.BackColor = _EmptyCellBackground;
                    }
                }

            }
        }
    }
}
