using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrmServerMonitor;

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
    }
}
