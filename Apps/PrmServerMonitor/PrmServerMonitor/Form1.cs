/*
 * CODE OWNERS: Tom Puckett
 * OBJECTIVE: Main form used when this application is run as an interactive GUI application
 * DEVELOPER NOTES: <What future developers need to know.>
 */

using System;
using System.Windows.Forms;

namespace PrmServerMonitor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Button handler that initiates cleanup of orphaned documents
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRemoveOrphanTasks_Click(object sender, EventArgs e)
        {
            // the project setup for a QMS client application can be found at https://community.qlik.com/docs/DOC-2639

            Cursor OriginalCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                OrphanQlikTaskRemover Worker = new OrphanQlikTaskRemover();
                Worker.RemoveOrphanTasks();
            }
            finally
            {
                this.Cursor = OriginalCursor;
            }
        }

        /// <summary>
        /// Button handler that initiates the enumeration of assorted current Qlikview CAL statistics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCalReport_Click(object sender, EventArgs e)
        {
            QlikviewCalManager Manager = new QlikviewCalManager();
            Manager.EnumerateAllCals();
        }
    }
}
