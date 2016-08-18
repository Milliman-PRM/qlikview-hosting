using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Win = System.Windows.Forms;

namespace Milliman.Reduction.Application {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Win.Application.EnableVisualStyles();
            Win.Application.SetCompatibleTextRenderingDefault(false);
            Win.Application.Run(new Form1());
        }
    }
}
