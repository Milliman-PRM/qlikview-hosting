using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbmcUnityTestGui
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            String TraceFileName = "TraceLog_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new TextWriterTraceListener(TraceFileName));
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
            Trace.WriteLine("Application launched " + DateTime.Now.ToString());

            Application.Run(new Form1());
        }
    }
}
