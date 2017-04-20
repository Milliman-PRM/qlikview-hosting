/*
 * CODE OWNERS: Tom Puckett, Ben Wyatt
 * OBJECTIVE: A Win Forms application to drive features implemented in the related class library
 * DEVELOPER NOTES: 
 */

using System;
using System.Windows.Forms;

namespace PRMServerMonitorGUI
{
    static class Program
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                return 0;
        }
    }
}
