using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BayClinicCdrAggregationLib;
using NorthBendCdrAggregationLib;

namespace WoahCdrAggregationTestGui
{
    public partial class Form1 : Form
    {
        public List<BayClinicCernerAmbulatoryExtractAggregator> BayClinicProcesses = new List<BayClinicCdrAggregationLib.BayClinicCernerAmbulatoryExtractAggregator>();

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonAggregate_Click(object sender, EventArgs e)
        {
            RadioButton SelectedRadio = groupFeedSelection.Controls.OfType<RadioButton>().Where(Button => Button.Checked == true).FirstOrDefault();
            switch (SelectedRadio.Name)
            {
                case "radioBayClinicCernerAmbulatory":
                    BayClinicCernerAmbulatoryExtractAggregator BcLib = new BayClinicCernerAmbulatoryExtractAggregator();
                    BcLib.StartThread();
                    BayClinicProcesses.Add(BcLib);
                    break;

                case "radioNBMCAllscriptsViaIntelliware":
                    break;

                default:
                    break;
            }
        }

        private void buttonEndAllThreads_Click(object sender, EventArgs e)
        {
            foreach (BayClinicCernerAmbulatoryExtractAggregator B in BayClinicProcesses)
            {
                B.EndThread();
            }
        }
    }
}
