using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoahRawDataExtractSvc
{
    public class ExtractionThreadParameters
    {
        public String RawFilePath;
        public String ArchiveFilePath;
        public String MongoIniFileName;
        public String MongoIniFileSection;
        public int SleepTimeMs;
    }
}
