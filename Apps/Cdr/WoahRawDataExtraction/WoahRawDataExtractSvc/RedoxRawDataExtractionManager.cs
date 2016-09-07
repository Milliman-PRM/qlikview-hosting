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
        ExtractionThreadParameters ThdParams;

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
        public bool StartThread(ExtractionThreadParameters Params = null)
        {
            Mutx.WaitOne();

            ThdParams = (Params == null) ?
                new ExtractionThreadParameters
                {
                    RawFilePath = ConfigurationManager.AppSettings["RedoxRawFilePath"],
                    ArchiveFilePath = ConfigurationManager.AppSettings["RedoxArchiveFilePath"],
                    MongoIniFileName = ConfigurationManager.AppSettings["RedoxMongoIniFileName"],
                    MongoIniFileSection = ConfigurationManager.AppSettings["RedoxMongoIniFileSection"],
                    SleepTimeMs = int.Parse(ConfigurationManager.AppSettings["RedoxSleepTimeMs"])
                }
                :
                Params ;

            try {
                Directory.CreateDirectory(ThdParams.RawFilePath);
                Directory.CreateDirectory(ThdParams.ArchiveFilePath);
            }
            catch (Exception /*e*/)
            {
                Mutx.ReleaseMutex();
                return false;
            }

            // validate settings
            if ( !Directory.Exists(ThdParams.RawFilePath) ||
                 !Directory.Exists(ThdParams.ArchiveFilePath) ||
                 !File.Exists(ThdParams.MongoIniFileName) )
            {
                Mutx.ReleaseMutex();
                return false;
            }
            Mutx.ReleaseMutex();

            Thd.Start();

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
        public void ThreadMain()
        {
            Mutx.WaitOne();
            RedoxExtractLib.RawDataParser Parser = new RedoxExtractLib.RawDataParser(ThdParams.MongoIniFileName, ThdParams.MongoIniFileSection);
            Mutx.ReleaseMutex();

            for (Mutx.WaitOne() ; !EndThreadSignal ; Thread.Sleep(ThdParams.SleepTimeMs), Mutx.WaitOne())
            {
                Mutx.ReleaseMutex();

                Parser.MigrateRawToMongo(ThdParams.RawFilePath, ThdParams.ArchiveFilePath, true);
            }
            Mutx.ReleaseMutex();
        }
    }
}
