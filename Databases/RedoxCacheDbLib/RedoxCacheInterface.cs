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
        RedoxCacheContext Db;

        public RedoxCacheInterface(string ConnectionString = null)
        {
            if (ConnectionString == null)
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["DefaultRedoxCacheConnectionString"].ConnectionString;
            }

            Db = new RedoxCacheContext(ConnectionString);

            ConnectionString = null;
        }

        public long InsertSchedulingRecord(long TransmissionNumber, string ContentString)
        {
            Scheduling S = new Scheduling
            {
                TransmissionId = TransmissionNumber,
                Content = ContentString
            };

            Db.Schedulings.InsertOnSubmit(S);
            Db.SubmitChanges();

            return S.dbid;
        }

        public List<Scheduling> GetSchedulingRecords(bool Oldest = true, int MaxCount = 1)
        {
            List<Scheduling> ReturnList = Oldest ?
                Db.Schedulings.OrderBy(x => x.TransmissionId).Take(MaxCount).ToList() :
                Db.Schedulings.OrderByDescending(x => x.TransmissionId).Take(MaxCount).ToList();

            return ReturnList;
        }

        public bool RemoveSchedulingRecord(Scheduling S)
        {
            Db.Schedulings.DeleteOnSubmit(S);
            Db.SubmitChanges();
            return true;
        }
    }
}
