using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BayClinicCdrAggregationLib;
using BayClinicCernerAmbulatory;

namespace WoahCdrAggregationTestGui
{
    public partial class Form1 : Form
    {
        public List<BayClinicAggregationWorkerThreadManager> BayClinicProcesses = new List<BayClinicCdrAggregationLib.BayClinicAggregationWorkerThreadManager>();
        private DateTime StartDateTime;

        public Form1()
        {
            InitializeComponent();
            timerUiUpdate.Interval = 1000;
        }

        private void buttonAggregate_Click(object sender, EventArgs e)
        {
            StartDateTime = DateTime.Now;

            RadioButton SelectedRadio = groupFeedSelection.Controls.OfType<RadioButton>().Where(Button => Button.Checked == true).FirstOrDefault();
            switch (SelectedRadio.Name)
            {
                case "radioBayClinicCernerAmbulatory":
                    BayClinicAggregationWorkerThreadManager BcLib = new BayClinicAggregationWorkerThreadManager();

                    BcLib.StartThread(checkBoxResetMongoDB.Checked);
                    BayClinicProcesses.Add(BcLib);

                    Thread.Sleep(200);  // milliseconds
                    BcLib.EndThread();

                    break;

                case "radioNBMCAllscriptsViaIntelliware":
                    break;

                default:
                    break;
            }

            timerUiUpdate.Start();
        }

        private void buttonEndAllThreads_Click(object sender, EventArgs e)
        {
            foreach (BayClinicAggregationWorkerThreadManager B in BayClinicProcesses)
            {
                B.EndThread();
            }
        }

        private void timerUiUpdate_Tick(object sender, EventArgs e)
        {
            String ProgressSummary = BayClinicProcesses[0].GetProgressSummary();
            if (!String.IsNullOrWhiteSpace(ProgressSummary))
            {
                labelBcPatientsCompleted.Text = ProgressSummary;
                labelElapsedTime.Text = (DateTime.Now - StartDateTime).ToString();
            }
            //labelNbmcPatientsCompleted.Text = NbmcProcesses[0].GetNumberOfPatientsDone().ToString();
        }

        private void ButtonAddICD10CodeMeanings_Click(object sender, EventArgs e)
        {
            BayClinicSpecialTasks TaskObject = new BayClinicSpecialTasks();

            TaskObject.AddAllIcd10CodeMeanings();
        }
    }
}
