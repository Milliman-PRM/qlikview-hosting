using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrmServerMonitor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length == 0)
            { // Run as GUI
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                return 0;
            }
            else if (args.Any(a => a.ToLower() == "console"))
            { // run background process (invoke with "start /wait <exename> [args]" because the OS does not wait for gui applications to return)
                foreach (string Arg in args)
                {
                    switch (Arg.ToLower())
                    {
                        case "orphantaskremoval":
                            {
                                OrphanQlikTaskRemover Worker = new OrphanQlikTaskRemover();
                                Worker.RemoveOrphanTasks();
                            }
                            break;

                        // NOTE: Only include cases for finite operations.  No ongoing monitoring activities
                        case "somethingelse":
                            {
                            }
                            break;

                        default:
                            break;
                    }
                }
                return 0;
            }
            else  // nothing to be done
            {
                return 2;
            }

        }
    }
}
