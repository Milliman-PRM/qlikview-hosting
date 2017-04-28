/*
 * CODE OWNERS: Tom Puckett, Ben Wyatt
 * OBJECTIVE: Form control callbacks.  
 * DEVELOPER NOTES: Functionality that is common to the GUI and console versions should be implemented in the class library project instead of here
 */

using System;
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
        }

        private void ButtonRemoveOrphanTasks_Click(object sender, EventArgs e)
        {
            OrphanQlikTaskRemover Worker = new OrphanQlikTaskRemover();
            Worker.RemoveOrphanTasks();
        }

        private void ButtonCalReport_Click(object sender, EventArgs e)
        {
            QlikviewCalManager Worker = new QlikviewCalManager();
            Worker.EnumerateAllCals(true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ComboBoxServer.SelectedIndex = 0;  // initialize
        }

        private void ButtonDeleteTest_Click(object sender, EventArgs e)
        {
            QlikviewCalManager Worker = new QlikviewCalManager();
            Worker.RemoveOneNamedCal(TextBoxUserName.Text, true);
        }

        private void ButtonDeleteDocCalTest_Click(object sender, EventArgs e)
        {
            Cursor StartCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            QlikviewCalManager Worker = new QlikviewCalManager();
            Worker.RemoveOneDocumentCal(TextBoxDocUserName.Text, TextBoxPath.Text, TextBoxDocName.Text, true);

            this.Cursor = StartCursor;
        }

        private void ButtonEnumerateDocCALs_Click(object sender, EventArgs e)
        {
            Cursor StartCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            QlikviewCalManager Worker = new QlikviewCalManager();
            List<QlikviewCalManager.DocCalEntry> CalEntries = Worker.EnumerateDocumentCals(true);

            DataGridViewDocCals.Rows.Clear();
            foreach (var x in CalEntries.OrderBy(c => c.DocumentName + c.LastUsedDateTime))
            {
                DataGridViewDocCals.Rows.Add(new object[] { Path.Combine(x.RelativePath, x.DocumentName), x.UserName, x.LastUsedDateTime.ToString("yyyy-MM-dd HH:mm:ss") });
            }

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

        private void ButtonClearAllDeleteChecks_Click(object sender, EventArgs e)
        {
            CheckAllDeleteCells(false);
        }

        private void CheckAllDeleteCells(bool DoCheck)
        {
            for (int RowIndex = 0; RowIndex < DataGridViewDocCals.Rows.Count; RowIndex++)
            {
                DataGridViewDocCals.Rows[RowIndex].Cells["ColumnDelete"].Value = DoCheck;
            }
        }

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
            }
        }

        private void DataGridViewDocCals_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (DataGridViewDocCals.Columns[e.ColumnIndex].Name == "ColumnDelete" && e.RowIndex != -1)
            {
                DataGridViewDocCals.EndEdit();
            }
        }
    }
}
