using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Threading;

namespace WoahRawDataExtractSvc
{
    class RedoxRawDataExtractionManager
    {
        Thread Thd;
        bool EndThreadSignal;
        Mutex Mutx;

        public RedoxRawDataExtractionManager()
        {
            EndThreadSignal = false;
            Thd = new Thread(ThreadMain);
            Mutx = new Mutex();
        }

        /// <summary>
        /// Initiates processing.  
        /// </summary>
        /// <param name="ThreadArgs">Normally not supplied, but caller can pass override values to modify behavior</param>
        /// <returns></returns>
        public bool StartThread(Dictionary<String,String> ThreadArgs = null)
        {
            if (ThreadArgs == null) {
                // normal initialization
                ThreadArgs = new Dictionary<string, string>();
                ThreadArgs["RedoxRawFilePath"] = ConfigurationManager.AppSettings["RedoxRawFilePath"];
                ThreadArgs["RedoxMongoIniFileName"] = ConfigurationManager.AppSettings["RedoxMongoIniFileName"];
                ThreadArgs["RedoxMongoIniFileSection"] = ConfigurationManager.AppSettings["RedoxMongoIniFileSection"];
                ThreadArgs["RedoxSleepTimeMs"] = ConfigurationManager.AppSettings["RedoxSleepTimeMs"];
                ThreadArgs["RedoxArchiveFilePath"] = ConfigurationManager.AppSettings["RedoxArchiveFilePath"];
            }

            if (!Directory.Exists(ThreadArgs["RedoxRawFilePath"]))
            {
                return false;
            }

            Thd.Start(ThreadArgs);

            return true;
        }

        /// <summary>
        /// Sets the signal to terminate the thread and waits the specified time for the thread to end
        /// </summary>
        /// <param name="WaitMs">Default value is -1 (wait indefinitely)</param>
        /// <returns></returns>
        public bool EndThread(int WaitMs = -1)
        {
            Mutx.WaitOne();
            EndThreadSignal = true;
            Mutx.ReleaseMutex();

            return Thd.Join(WaitMs);
        }

        /// <summary>
        /// Entry point for the worker thread managed by this class  
        /// </summary>
        /// <param name="Args">A Dictionary<string,string> with operating parameters for the thread</param>
        public void ThreadMain(Object Arg)
        {
            Dictionary<String, String> Args = (Dictionary<String, String>)Arg;
            String RedoxRawFilePath = Args["RedoxRawFilePath"];
            String RedoxMongoIniFileName = Args["RedoxMongoIniFileName"];
            String RedoxMongoIniFileSection = Args["RedoxMongoIniFileSection"];
            String RedoxArchiveFilePath = Args["RedoxArchiveFilePath"];
            int RedoxSleepTimeMs = int.Parse(Args["RedoxSleepTimeMs"]);

            RedoxExtractLib.RawDataParser Parser = new RedoxExtractLib.RawDataParser(RedoxMongoIniFileName, RedoxMongoIniFileSection);

            for (Mutx.WaitOne() ; !EndThreadSignal ; Thread.Sleep(RedoxSleepTimeMs), Mutx.WaitOne())
            {
                Mutx.ReleaseMutex();

                Parser.MigrateRawToMongo(RedoxRawFilePath, false);
                //Parser.MigrateRawToMongo(RedoxRawFilePath, true);  // true to perform insert
            }
            Mutx.ReleaseMutex();
        }
    }
}
