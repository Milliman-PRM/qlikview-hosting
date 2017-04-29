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

            if (ConfigurationManager.AppSettings.AllKeys.Contains("MinimumAgeToDelete"))
            {
                int MinimumAgeToDelete = Convert.ToInt32(ConfigurationManager.AppSettings["MinimumAgeToDelete"]);
                MinimumAgeToDelete = Math.Max(MinimumAgeToDelete, 72);
                NumericUpDownMinAge.Value = MinimumAgeToDelete;
            }
        }

        /// <summary>
        /// Handler to initiate removal of all orphan Qlikview server tasks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRemoveOrphanTasks_Click(object sender, EventArgs e)
        {
            OrphanQlikTaskRemover Worker = new OrphanQlikTaskRemover(ComboBoxServer.Text);
            Worker.RemoveOrphanTasks();
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
        private void ButtonDeleteTest_Click(object sender, EventArgs e)
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
        private void ButtonEnumerateDocCALs_Click(object sender, EventArgs e)
        {
            Cursor StartCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            int SelectLimit;
            if (!int.TryParse(ConfigurationManager.AppSettings["MaxDocumentCALsToDelete"], out SelectLimit))
            {
                SelectLimit = 10;  // only if config value could not be parsed
            }

            QlikviewCalManager Worker = new QlikviewCalManager(ComboBoxServer.Text);
            List<DocCalEntry> CalEntries = Worker.EnumerateDocumentCals(true, SelectLimit, CheckBoxAllowUndatedCalSelection.Checked, (int)NumericUpDownMinAge.Value);

            DataGridViewDocCals.Rows.Clear();
            foreach (var x in CalEntries.OrderBy(c => c.DocumentName + c.LastUsedDateTime))
            {
                DataGridViewDocCals.Rows.Add(new object[] { Path.Combine(x.RelativePath, x.DocumentName), x.UserName, x.LastUsedDateTime.ToString("yyyy-MM-dd HH:mm:ss"), x.DeleteFlag });
            }

            UpdateCheckCountLabel();

            this.Cursor = StartCursor;
        }

        /// <summary>
        /// This call back provides line numbers in the header of each row in the DataGridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewDocCals_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
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
        private void CheckBoxAllowUndatedCalSelection_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox TypedSender = sender as CheckBox;
            if (!TypedSender.Checked)
            {
                for (int RowIndex = 0; RowIndex < DataGridViewDocCals.Rows.Count; RowIndex++)
                {
                    if (DateTime.Parse(DataGridViewDocCals.Rows[RowIndex].Cells["ColumnLastAccessDateTime"].Value.ToString()) == new DateTime())
                    {
                        DataGridViewDocCals.Rows[RowIndex].Cells["ColumnDelete"].Value = TypedSender.Checked;
                    }
                }
            }
        }

        /// <summary>
        /// Clear all checks in the Delete column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClearAllDeleteChecks_Click(object sender, EventArgs e)
        {
            CheckAllDeleteCells(false);
        }

        /// <summary>
        /// Internal function to set or clear all checkboxes.  
        /// </summary>
        /// <param name="DoCheck"></param>
        private void CheckAllDeleteCells(bool DoCheck)
        {
            for (int RowIndex = 0; RowIndex < DataGridViewDocCals.Rows.Count; RowIndex++)
            {
                DataGridViewDocCals.Rows[RowIndex].Cells["ColumnDelete"].Value = DoCheck;
            }
        }

        /// <summary>
        /// A button click handler that deletes each document CAL signaled by a checked row in the DataViewGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeleteSelectedCals_Click(object sender, EventArgs e)
        {
            Cursor StartCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            QlikviewCalManager Worker = new QlikviewCalManager(ComboBoxServer.Text, true);
            for (int RowIndex = DataGridViewDocCals.Rows.Count-1; RowIndex >=0; RowIndex--)
            {
                DataGridViewRow Row = DataGridViewDocCals.Rows[RowIndex];
                if (Convert.ToBoolean(Row.Cells["ColumnDelete"].Value) == true)
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

            Trace.WriteLine("about to invalidate the grid control");

            // redraw the grid
            DataGridViewDocCals.Invalidate();

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
            if (DataGridViewDocCals.Columns[e.ColumnIndex].Name == "ColumnDelete" && e.RowIndex != -1)
            {
                if (Convert.ToBoolean(DataGridViewDocCals.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) == true &&
                    Convert.ToDateTime(DataGridViewDocCals.Rows[e.RowIndex].Cells["ColumnLastAccessDateTime"].Value) == new DateTime() &&
                    !CheckBoxAllowUndatedCalSelection.Checked)
                {
                    DataGridViewDocCals.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                    DataGridViewDocCals.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = false;
                    DataGridViewDocCals.Rows[e.RowIndex].Cells["ColumnLastAccessDateTime"].Selected = true;
                }

                UpdateCheckCountLabel();
            }
        }

        /// <summary>
        /// Updates the label control with the appropriate count of checkboxes that are checked in the "ColumnDelete" column
        /// </summary>
        private void UpdateCheckCountLabel()
        {
            int CheckCounter = 0;
            foreach (DataGridViewRow Row in DataGridViewDocCals.Rows)
            {
                if (Convert.ToBoolean(Row.Cells["ColumnDelete"].Value))
                {
                    CheckCounter++;
                }
            }
            LabelCheckedCount.Text = CheckCounter.ToString() + " Rows Checked";
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
            if (DataGridViewDocCals.Columns[e.ColumnIndex].Name == "ColumnDelete" && e.RowIndex != -1)
            {
                DataGridViewDocCals.EndEdit();
            }
        }

    }
}
