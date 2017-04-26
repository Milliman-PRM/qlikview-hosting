/*
 * CODE OWNERS: Tom Puckett, Ben Wyatt
 * OBJECTIVE: Form control callbacks.  
 * DEVELOPER NOTES: Functionality that is common to the GUI and console versions should be implemented in the class library project instead of here
 */

using System;
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
            QlikviewCalManager Worker = new QlikviewCalManager();
            Worker.RemoveOneDocumentCal(TextBoxDocUserName.Text, TextBoxUserName.Text, true);
        }

        private void ButtonEnumerateDocCALs_Click(object sender, EventArgs e)
        {
            Cursor StartCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            QlikviewCalManager Worker = new QlikviewCalManager();
            Worker.EnumerateDocumentCals(true);

            this.Cursor = StartCursor;
        }
    }
}
