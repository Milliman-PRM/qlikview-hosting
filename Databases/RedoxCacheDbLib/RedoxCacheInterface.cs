using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using RedoxCacheDbContext;

namespace RedoxCacheDbLib
{
    public class RedoxCacheInterface
    {
        RedoxCacheContext Db = new RedoxCacheContext();

        public RedoxCacheInterface(string ConnectionString = null)
        {
            if (ConnectionString == null)
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["DefaultRedoxCacheConnectionString"].ConnectionString;
            }

            Db = new RedoxCacheContext(ConnectionString);

            ConnectionString = null;
        }

        public long InsertSchedulingRecord(long TransmissionNumber, string MetaString, string ContentString)
        {
            Scheduling S = new Scheduling
            {
                TransmissionNumber = TransmissionNumber,
                Meta = MetaString,
                Content = ContentString
            };

            Db.Schedulings.InsertOnSubmit(S);
            Db.SubmitChanges();

            return S.TransmissionNumber;
        }
    }
}
