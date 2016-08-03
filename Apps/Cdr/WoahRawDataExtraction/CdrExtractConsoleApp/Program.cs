using System;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using BayClinicCernerExtractLib;

namespace CdrExtractConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            String RawFolder = ConfigurationManager.AppSettings["BayClinicEmrRawDataFolder"];
            String BayClinicEmrMongoCredentialConfigFile = ConfigurationManager.AppSettings["BayClinicEmrMongoCredentialConfigFile"];
            String BayClinicEmrMongoCredentialSection = ConfigurationManager.AppSettings["BayClinicEmrMongoCredentialSection"];

            // ConfigurationManager returns null when the requested key is not found.  Code using the values should be implemented to handle this case

            BayClinicCernerExtractLib.RawDataParser Parser = new BayClinicCernerExtractLib.RawDataParser(BayClinicEmrMongoCredentialConfigFile, BayClinicEmrMongoCredentialSection);
            if (!Directory.Exists(RawFolder))
            {
                Trace.WriteLine("Abort!  The configured folder for raw data was not found to exist");
                return 1;
            }

            try
            {
                Parser.MigrateFolderToMongo(RawFolder, true, Path.Combine(RawFolder, "Archive"));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Abort!  Exception caught in Program.Main while running Parser.MigrateFolderToMongo(): \n" + ex.Message + "\n" + ex.StackTrace);
                return 1;
            }

            return 0;
        }
    }
}
