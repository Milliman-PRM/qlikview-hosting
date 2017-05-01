/*
 * CODE OWNERS: Tom Puckett, Ben Wyatt
 * OBJECTIVE: Form control callbacks.  
 * DEVELOPER NOTES: Functionality that is common to the GUI and console versions should be implemented in the class library project, not here
 */

using System;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrmServerMonitorLib;

namespace PRMServerMonitorGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            if (ConfigurationManager.AppSettings.AllKeys.Contains("MinimumDocCalAgeHoursToDelete"))
            {
                int MinimumDocCalAgeHoursToDelete = Convert.ToInt32(ConfigurationManager.AppSettings["MinimumDocCalAgeHoursToDelete"]);
                MinimumDocCalAgeHoursToDelete = Math.Max(MinimumDocCalAgeHoursToDelete, 24);
                NumericUpDownDocCalMinAge.Minimum = MinimumDocCalAgeHoursToDelete;
            }

            if (ConfigurationManager.AppSettings.AllKeys.Contains("MinimumNamedCalAgeHoursToDelete"))
            {
                int MinimumNamedCalAgeHoursToDelete = Convert.ToInt32(ConfigurationManager.AppSettings["MinimumNamedCalAgeHoursToDelete"]);
                MinimumNamedCalAgeHoursToDelete = Math.Max(MinimumNamedCalAgeHoursToDelete, 24);
                NumericUpDownNamedCalMinAge.Minimum = MinimumNamedCalAgeHoursToDelete;
            }

            UpdateCheckCountLabel(DataGridViewNamedCals, "ColumnDeleteNamedCal", LabelNamedCalCheckedCount);
            UpdateCheckCountLabel(DataGridViewDocCals, "ColumnDeleteDocCal", LabelDocCalCheckedCount);
        }

        /// <summary>
        /// Handler to initiate removal of all orphan Qlikview server tasks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRemoveOrphanTasks_Click(object sender, EventArgs e)
        {
            OrphanQlikTaskRemover Worker = new OrphanQlikTaskRemover(ComboBoxServer.Text);
            Worker.RemoveOrphanTasks(CheckBoxLogToFile.Checked);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCalReport_Click(object sender, EventArgs e)
        {
            QlikviewCalManager Worker = new QlikviewCalManager(ComboBoxServer.Text);
            Worker.EnumerateAllCals(true);
        }

        /// <summary>
        /// Initialization of run time application state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            ComboBoxServer.SelectedIndex = 0;  // initialize
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeleteNamedCalTest_Click(object sender, EventArgs e)
        {
            QlikviewCalManager Worker = new QlikviewCalManager(ComboBoxServer.Text);
            Worker.RemoveOneNamedCal(TextBoxUserName.Text, true);
        }

        /// <summary>
        /// A test handler to observe the doc CAL removal feature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeleteDocCalTest_Click(object sender, EventArgs e)
        {
            Cursor StartCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            QlikviewCalManager Worker = new QlikviewCalManager(ComboBoxServer.Text);
            Worker.RemoveOneDocumentCal(TextBoxDocUserName.Text, TextBoxPath.Text, TextBoxDocName.Text, true);

            this.Cursor = StartCursor;
        }

        /// <summary>
        /// Clears and fills the DataGridView with current state of document CALs from the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRefreshDocCALs_Click(object sender, EventArgs e)
        {
            Cursor StartCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            int SelectLimit;
            if (!int.TryParse(ConfigurationManager.AppSettings["MaxDocumentCALsToDelete"], out SelectLimit))
            {
                SelectLimit = 10;  // only if config value could not be parsed
            }

            QlikviewCalManager Worker = new QlikviewCalManager(ComboBoxServer.Text);
            List<DocCalEntry> CalEntries = Worker.EnumerateDocumentCals(CheckBoxLogToFile.Checked, SelectLimit, CheckBoxAllowUndatedDocCalSelection.Checked, (int)NumericUpDownDocCalMinAge.Value);

            DataGridViewDocCals.Rows.Clear();
            foreach (var x in CalEntries.OrderBy(c => c.DocumentName + c.LastUsedDateTime))
            {
                DataGridViewDocCals.Rows.Add(new object[] { Path.Combine(x.RelativePath, x.DocumentName), x.UserName, x.LastUsedDateTime.ToString("yyyy-MM-dd HH:mm:ss"), x.DeleteFlag });
            }

            UpdateCheckCountLabel(DataGridViewDocCals, "ColumnDeleteDocCal", LabelDocCalCheckedCount);

            this.Cursor = StartCursor;
        }

        private void ButtonRefreshNamedCALs_Click(object sender, EventArgs e)
        {
            Cursor StartCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            int SelectLimit;
            if (!int.TryParse(ConfigurationManager.AppSettings["MaxNamedCALsToDelete"], out SelectLimit))
            {
                SelectLimit = 100;  // only if config value could not be parsed
            }

            QlikviewCalManager Worker = new QlikviewCalManager(ComboBoxServer.Text);
            List<NamedCalEntry> CalEntries = Worker.EnumerateNamedCals(SelectLimit, CheckBoxAllowUndatedNamedCalSelection.Checked, (int)NumericUpDownNamedCalMinAge.Value, CheckBoxLogToFile.Checked);

            DataGridViewNamedCals.Rows.Clear();
            foreach (var x in CalEntries.OrderBy(c => c.LastUsedDateTime))
            {
                DataGridViewNamedCals.Rows.Add(new object[] { x.UserName, x.LastUsedDateTime.ToString("yyyy-MM-dd HH:mm:ss"), x.DeleteFlag });
            }

            UpdateCheckCountLabel(DataGridViewNamedCals, "ColumnDeleteNamedCal", LabelNamedCalCheckedCount);

            this.Cursor = StartCursor;
        }

        /// <summary>
        /// This call back provides line numbers in the header of each row in the DataGridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView Grid = sender as DataGridView;
            string rowIdx = (e.RowIndex + 1).ToString();

            StringFormat CenterFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            };

            Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, Grid.RowHeadersWidth - 1, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, CenterFormat);
        }

        /// <summary>
        /// When the checkbox is unchecked, clears check marks from all rows that contain a default DateTime value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAllowUndatedDocCalSelection_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox TypedSender = sender as CheckBox;
            if (!TypedSender.Checked)
            {
                for (int RowIndex = 0; RowIndex < DataGridViewDocCals.Rows.Count; RowIndex++)
                {
                    if (DateTime.Parse(DataGridViewDocCals.Rows[RowIndex].Cells["ColumnLastAccessDateTime"].Value.ToString()) == new DateTime())
                    {
                        DataGridViewDocCals.Rows[RowIndex].Cells["ColumnDeleteDocCal"].Value = TypedSender.Checked;
                    }
                }
            }
        }

        /// <summary>
        /// When the checkbox is unchecked, clears check marks from all rows that contain a default DateTime value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAllowUndatedNamedCalSelection_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox TypedSender = sender as CheckBox;
            if (!TypedSender.Checked)
            {
                for (int RowIndex = 0; RowIndex < DataGridViewNamedCals.Rows.Count; RowIndex++)
                {
                    if (DateTime.Parse(DataGridViewNamedCals.Rows[RowIndex].Cells["ColumnLastNamedCalAccess"].Value.ToString()) == new DateTime())
                    {
                        DataGridViewNamedCals.Rows[RowIndex].Cells["ColumnDeleteNamedCal"].Value = TypedSender.Checked;
                    }
                }
            }
        }

        /// <summary>
        /// Clear all checks in the Delete column of the sending DataGridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClearAllDocCalDeleteChecks_Click(object sender, EventArgs e)
        {
            SetCheckBoxes(DataGridViewDocCals, "ColumnDeleteDocCal", false);
        }

        private void ButtonClearAllNamedCalDeleteChecks_Click(object sender, EventArgs e)
        {
            SetCheckBoxes(DataGridViewNamedCals, "ColumnDeleteNamedCal", false);
        }

        /// <summary>
        /// Internal function to set or clear all checkboxes.  
        /// </summary>
        /// <param name="Checked"></param>
        private void SetCheckBoxes(DataGridView Grid, string ColumnName, bool Checked)
        {
            for (int RowIndex = 0; RowIndex < Grid.Rows.Count; RowIndex++)
            {
                Grid.Rows[RowIndex].Cells[ColumnName].Value = Checked;
            }
        }

        /// <summary>
        /// A button click handler that deletes each document CAL signaled by a checked row in the DataViewGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeleteSelectedDocCals_Click(object sender, EventArgs e)
        {
            Cursor StartCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            QlikviewCalManager Worker = new QlikviewCalManager(ComboBoxServer.Text, true);
            for (int RowIndex = DataGridViewDocCals.Rows.Count-1; RowIndex >=0; RowIndex--)
            {
                DataGridViewRow Row = DataGridViewDocCals.Rows[RowIndex];
                if (Convert.ToBoolean(Row.Cells["ColumnDeleteDocCal"].Value) == true)
                {
                    string Folder = Path.GetDirectoryName(Row.Cells["ColumnDocument"].Value.ToString());
                    string Doc = Path.GetFileName(Row.Cells["ColumnDocument"].Value.ToString());
                    string User = Row.Cells["ColumnUserId"].Value.ToString();

                    Trace.WriteLine(string.Format("Removing doc cal for doc {0} and user {1}", Path.Combine(Folder, Doc), User));
                    if (Worker.RemoveOneDocumentCal(User, Folder, Doc, true))
                    {
                        DataGridViewDocCals.Rows.RemoveAt(RowIndex);
                    }
                }
            }

            // redraw the grid
            DataGridViewDocCals.Invalidate();

            Worker = null;  // try to encourage Trace file closure (destructor execution)
            this.Cursor = StartCursor;
        }

        /// <summary>
        /// A button click handler that deletes each named CAL signaled by a checked row in the DataViewGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeleteSelectedNamedCals_Click(object sender, EventArgs e)
        {
            Cursor StartCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            QlikviewCalManager Worker = new QlikviewCalManager(ComboBoxServer.Text, true);
            for (int RowIndex = DataGridViewNamedCals.Rows.Count - 1; RowIndex >= 0; RowIndex--)
            {
                DataGridViewRow Row = DataGridViewNamedCals.Rows[RowIndex];
                if (Convert.ToBoolean(Row.Cells["ColumnDeleteNamedCal"].Value) == true)
                {
                    string User = Row.Cells["ColumnUserName"].Value.ToString();

                    Trace.WriteLine(string.Format("Removing named cal for user {0}", User));
                    if (Worker.RemoveOneNamedCal(User, true))
                    {
                        DataGridViewNamedCals.Rows.RemoveAt(RowIndex);
                    }
                }
            }

            // redraw the grid
            DataGridViewNamedCals.Invalidate();

            Worker = null;  // try to encourage Trace file closure (destructor execution)
            this.Cursor = StartCursor;
        }

        /// <summary>
        /// Callback where validation can be performed.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewDocCals_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (DataGridViewDocCals.Columns[e.ColumnIndex].Name == "ColumnDeleteDocCal" && e.RowIndex != -1)
            {
                if (Convert.ToBoolean(DataGridViewDocCals.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) == true)
                {
                    bool IsUndated = Convert.ToDateTime(DataGridViewDocCals.Rows[e.RowIndex].Cells["ColumnLastAccessDateTime"].Value) == new DateTime();
                    bool IsTooRecent = DateTime.Now - Convert.ToDateTime(DataGridViewDocCals.Rows[e.RowIndex].Cells["ColumnLastAccessDateTime"].Value) < new TimeSpan((int)NumericUpDownDocCalMinAge.Value, 0, 0);

                    if (IsTooRecent || (IsUndated && !CheckBoxAllowUndatedDocCalSelection.Checked))
                    {
                        DataGridViewDocCals.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                        DataGridViewDocCals.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = false;
                        DataGridViewDocCals.Rows[e.RowIndex].Cells["ColumnLastAccessDateTime"].Selected = true;
                    }
                }

                UpdateCheckCountLabel(DataGridViewDocCals, "ColumnDeleteDocCal", LabelDocCalCheckedCount);
            }
        }

        /// <summary>
        /// Updates the label control with the appropriate count of checkboxes that are checked in the "ColumnDeleteDocCal" column
        /// </summary>
        private void UpdateCheckCountLabel(DataGridView Grid, string DeleteColumnName, Label LabelToUpdate)
        {
            int CheckCounter = 0;
            foreach (DataGridViewRow Row in Grid.Rows)
            {
                if (Convert.ToBoolean(Row.Cells[DeleteColumnName].Value))
                {
                    CheckCounter++;
                }
            }
            LabelToUpdate.Text = CheckCounter.ToString() + " Rows Checked";
        }

        /// <summary>
        /// Inserts an immediate EndEdit() call for each click of a checkbox in the Checkbox type column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewDocCals_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Background: For all cell types, the runtime doesn't fire the CellValueChanged event until the cell edit is complete e.g. not after each keystroke 
            // for a text cell.  For a checkbox cell this happens when the cell loses focus, which is too late for interactive validation.  
            if (DataGridViewDocCals.Columns[e.ColumnIndex].Name == "ColumnDeleteDocCal" && e.RowIndex != -1)
            {
                DataGridViewDocCals.EndEdit();
            }
        }

        /// <summary>
        /// Inserts an immediate EndEdit() call for each click of a checkbox in the Checkbox type column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewNamedCals_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Background: For all cell types, the runtime doesn't fire the CellValueChanged event until the cell edit is complete e.g. not after each keystroke 
            // for a text cell.  For a checkbox cell this happens when the cell loses focus, which is too late for interactive validation.  
            if (DataGridViewNamedCals.Columns[e.ColumnIndex].Name == "ColumnDeleteNamedCal" && e.RowIndex != -1)
            {
                DataGridViewNamedCals.EndEdit();
            }
        }

        private void DataGridViewNamedCals_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (DataGridViewNamedCals.Columns[e.ColumnIndex].Name == "ColumnDeleteNamedCal" && e.RowIndex != -1)
            {
                if (Convert.ToBoolean(DataGridViewNamedCals.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) == true)
                {
                    bool IsUndated = Convert.ToDateTime(DataGridViewNamedCals.Rows[e.RowIndex].Cells["ColumnLastNamedCalAccess"].Value) == new DateTime();
                    bool IsTooRecent = DateTime.Now - Convert.ToDateTime(DataGridViewNamedCals.Rows[e.RowIndex].Cells["ColumnLastNamedCalAccess"].Value) < new TimeSpan((int)NumericUpDownNamedCalMinAge.Value, 0, 0);

                    if (IsTooRecent || (IsUndated && !CheckBoxAllowUndatedNamedCalSelection.Checked))
                    {
                        DataGridViewNamedCals.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                        DataGridViewNamedCals.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = false;
                        DataGridViewNamedCals.Rows[e.RowIndex].Cells["ColumnLastNamedCalAccess"].Selected = true;
                    }
                }

                UpdateCheckCountLabel(DataGridViewNamedCals, "ColumnDeleteNamedCal", LabelNamedCalCheckedCount);
            }
        }

    }
}
