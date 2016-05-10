using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using RedoxCacheDbContext;
using Devart.Data.Linq;

namespace RedoxCacheDbLib
{
    /// <summary>
    /// An api class to expose C# access to the database housing the Redox message/task queue.  
    /// </summary>
    public class RedoxCacheDbInterface
    {
        RedoxCacheContext Db;

        /// <summary>
        /// static factory method to instantiate this class
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns>The new constructed object</returns>
        public static RedoxCacheDbInterface CreateNewInstance(string ConnectionStringName = null)
        {
            String ConnectionString = (ConnectionStringName != null) ?
                ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString :
                ConfigurationManager.ConnectionStrings["DefaultRedoxCacheContextConnectionString"].ConnectionString;

            return new RedoxCacheDbInterface(ConnectionString);
        }

        /// <summary>
        /// Private constructor!!  Use static CreateNewInstance(...) instead to instantiate.  
        /// </summary>
        /// <param name="ConnectionString"></param>
        private RedoxCacheDbInterface(string ConnectionString)
        {
            Db = new RedoxCacheContext(ConnectionString);

            ConnectionString = null;
        }

        /// <summary>
        /// Creates and inserts a record to the scheduling entity (table) of the encapsulated database
        /// </summary>
        /// <param name="TransmissionNumber"></param>
        /// <param name="SourceId"></param>
        /// <param name="SourceName"></param>
        /// <param name="ContentString"></param>
        /// <param name="EventType"></param>
        /// <returns>The primary key value of the inserted record upon successful INSERT or -1 if connection is not open</returns>
        public long InsertSchedulingRecord(long TransmissionNumber, String SourceId, String SourceName, String ContentString, String EventType)
        {
            Scheduling S = new Scheduling
            {
                TransmissionId = TransmissionNumber,
                Content = ContentString,
                SourceFeedId = new Guid(SourceId),
                SourceFeedName = SourceName,
                EventType = EventType
            };

            try
            {
                Db.Schedulings.InsertOnSubmit(S);
                Db.SubmitChanges();
            }
            catch (Exception /*e*/)
            {
                return -1;
            }

            return S.dbid;
        }

        /// <summary>
        /// Performs a SELECT on the Scheduling task table in the database and returns a collection of entities representing the result
        /// </summary>
        /// <param name="Oldest">Set true to return the oldest records in the table, false for the newest</param>
        /// <param name="MaxCount">Limits the number of records to return.  Default is no limit</param>
        /// <returns>List of returned Scheduling instances (empty if connection is not open)</returns>
        public List<Scheduling> GetSchedulingRecords(bool Oldest = true, int MaxCount = -1)
        {
            List<Scheduling> ReturnList;

            try
            {
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
            }
            catch (Exception /*e*/)
            {
                return new List<Scheduling>();
            }

            return ReturnList;
        }

        /// <summary>
        /// Removes a record from the Scheduling entity collection/table
        /// </summary>
        /// <param name="S">Represents the object to be deleted from the context</param>
        /// <returns>boolean indicating success of the remove operation</returns>
        public bool RemoveSchedulingRecord(Scheduling S)
        {
            try
            {
                Db.Schedulings.DeleteOnSubmit(S);
                Db.SubmitChanges(ConflictMode.FailOnFirstConflict);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

    }
}
