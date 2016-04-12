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

        public long InsertSchedulingRecord(long TransmissionNumber, String SourceId, String SourceName, String ContentString)
        {
            Scheduling S = new Scheduling
            {
                TransmissionId = TransmissionNumber,
                Content = ContentString,
                SourceFeedId = new Guid(SourceId),
                SourceFeedName = SourceName
            };

            Db.Schedulings.InsertOnSubmit(S);
            Db.SubmitChanges();

            return S.dbid;
        }

        /// <summary>
        /// Performs a SELECT on the Scheduling task table in the database and returns a collection of entities representing the result
        /// </summary>
        /// <param name="Oldest">Set true to return the oldest records in the table, false for the newest</param>
        /// <param name="MaxCount">Limits the number of records to return.  Default is no limit</param>
        /// <returns></returns>
        public List<Scheduling> GetSchedulingRecords(bool Oldest = true, int MaxCount = -1)
        {
            List<Scheduling> ReturnList;

            if (MaxCount == -1)  // Return all records
            {
                ReturnList = Oldest ?
                    Db.Schedulings.OrderBy(x => x.TransmissionId).ToList() :
                    Db.Schedulings.OrderByDescending(x => x.TransmissionId).ToList();
            }
            else  // The query will limit the number of returned records
            {
                ReturnList = Oldest ?
                    Db.Schedulings.OrderBy(x => x.TransmissionId).Take(MaxCount).ToList() :
                    Db.Schedulings.OrderByDescending(x => x.TransmissionId).Take(MaxCount).ToList();
            }

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
