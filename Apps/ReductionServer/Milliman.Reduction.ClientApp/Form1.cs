using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Milliman.Reduction.ClientApp {
    public partial class Form1: Form {
        //private ReductionService.ReductionServiceClient client;
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            
            //client.StopService();
        }

        private void btnReduce_Click(object sender, EventArgs e) {
            using( var client = new ReductionService.ReductionServiceClient() ) {
                client.EnqueueReductionFolder(txtPath.Text);
            }
        }

        private void btnStop_Click(object sender, EventArgs e) {
            //client.StopService();
        }
    }
}
