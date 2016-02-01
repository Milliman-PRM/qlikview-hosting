using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRMValidationGC
{
    static class Program
    {
        private static bool DisplayUI = false;
        private static bool RunGC = false;
        private static bool RunLicenseReport = false;
        private static bool AutoArchive = false;
        private static string RootDir = string.Empty;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //   /LicenseReport /GC /Root="" /AutoArchive
            string[] Args = Environment.GetCommandLineArgs();
            foreach (string Arg in Args)
            {
                if (string.Compare(Arg, "/licensereport", true) == 0)
                    RunLicenseReport = true;
            }

            if (DisplayUI)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }

            if (RunLicenseReport)
            {
                LicenseReport LR = new LicenseReport();
                LR.Process();
            }
        }
    }
}
