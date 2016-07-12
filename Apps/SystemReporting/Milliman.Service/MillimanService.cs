using SystemReporting.Data.Repository;
using SystemReporting.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SystemReporting.Data.Database;

namespace SystemReporting.Service
{
    public partial class MillimanService : IMillimanService
    {        
        public MillimanService()
        {
            //CreateDbContext();
        }

        #region Database

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.IisLogRepository != null)     { this.IisLogRepository.Dispose();}
                if (this.AuditLogRepository != null)   { this.AuditLogRepository.Dispose();}
                if (this.SessionLogRepository != null) { this.SessionLogRepository.Dispose();}
                if (this.UserRepository != null)       { this.UserRepository.Dispose(); }
                if (this.ReportRepository != null)     { this.ReportRepository.Dispose(); }
                if (this.GroupRepository != null)      {this.GroupRepository.Dispose();}                
            }
        }

        #endregion
        
        #region Repositories
        //initiate repositories with database context
        IRepository<IisLog> IisLogRepository = new Repository<IisLog>(new ApplicationDbContext());
        IRepository<AuditLog> AuditLogRepository = new Repository<AuditLog>(new ApplicationDbContext());
        IRepository<SessionLog> SessionLogRepository = new Repository<SessionLog>(new ApplicationDbContext());
        IRepository<User> UserRepository = new Repository<User>(new ApplicationDbContext());
        IRepository<Report> ReportRepository = new Repository<Report>(new ApplicationDbContext());
        IRepository<Group> GroupRepository = new Repository<Group>(new ApplicationDbContext());
        #endregion

        #endregion

        #region IisLog
        /// <summary>
        /// Remove an object
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(IisLog obj)
        {
            if (obj.Id > 0)
                IisLogRepository.Delete(obj);

            IisLogRepository.Commit();            
        }
        /// <summary>
        /// Save an object
        /// </summary>
        /// <param name="obj"></param>
        public void Save(IisLog obj)
        {
            if (obj.Id <= 0)
            {
                IisLogRepository.Add(obj);
            }
            else
                IisLogRepository.Attach(obj);

            IisLogRepository.Commit();
        }

        /// <summary>
        /// Get list of Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetIisLogs<T>(Expression<Func<T, bool>> predicate = null) where T : IisLog
        {
            var query = IisLogRepository.FindAll().OfType<T>();
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        /// <summary>
        /// Get list of Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <param name="name"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetIisLogs<T>(int Id, string name, Expression<Func<T, bool>> predicate = null) where T : IisLog
        {
            // Look at the Repository for all types of the generic type
            var query = IisLogRepository.FindAll().OfType<T>();
            // If we have a defined predicate - than limit the query by that expression
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        /// <summary>
        /// Get list of Objects
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<IisLog> GetIisLogs(int Id, string name)
        {
            IEnumerable<IisLog> returnValues = Enumerable.Empty<IisLog>();
            returnValues = GetIisLogs<IisLog>(Id, name).ToList();
            return returnValues;
        }
        #endregion

        #region AuditLog
        /// <summary>
        /// Remove an object
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(AuditLog obj)
        {
            if (obj.Id > 0)
                AuditLogRepository.Delete(obj);

            AuditLogRepository.Commit();
        }

        /// <summary>
        /// Save an object
        /// </summary>
        /// <param name="obj"></param>
        public void Save(AuditLog obj)
        {
            if (obj.Id <= 0)
            {
                AuditLogRepository.Add(obj);
            }
            else
                AuditLogRepository.Attach(obj);

            AuditLogRepository.Commit();
        }

        /// <summary>
        /// Get list of Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetAuditLogs<T>(Expression<Func<T, bool>> predicate = null) where T : AuditLog
        {
            var query = AuditLogRepository.FindAll().OfType<T>();
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        /// <summary>
        /// Get list of Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <param name="name"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetAuditLogs<T>(int Id, string name, Expression<Func<T, bool>> predicate = null) where T : AuditLog
        {
            // Look at the Repository for all types of the generic type
            var query = AuditLogRepository.FindAll().OfType<T>();
            // If we have a defined predicate - than limit the query by that expression
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        /// <summary>
        /// Get list of Objects
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<AuditLog> GetAuditLogs(int Id, string name)
        {
            IEnumerable<AuditLog> returnValues = Enumerable.Empty<AuditLog>();
            returnValues = GetAuditLogs<AuditLog>(Id, name).ToList();
            return returnValues;
        }
        #endregion

        #region SessionLog

        /// <summary>
        /// Remove an object
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(SessionLog obj)
        {
            if (obj.Id > 0)
                SessionLogRepository.Delete(obj);

            SessionLogRepository.Commit();
        }

        /// <summary>
        /// Save an object
        /// </summary>
        /// <param name="obj"></param>
        public void Save(SessionLog obj)
        {
            if (obj.Id <= 0)
            {
                SessionLogRepository.Add(obj);
            }
            else
                SessionLogRepository.Attach(obj);

            SessionLogRepository.Commit();
        }

        /// <summary>
        /// Get list of Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetSessionLogs<T>(Expression<Func<T, bool>> predicate = null) where T : SessionLog
        {
            //var query = BlogRepository.FindAll().OfType<T>();
            var query = SessionLogRepository.FindAll().OfType<T>();
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        /// <summary>
        /// Get list of Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <param name="name"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetSessionLogs<T>(int Id, string name, Expression<Func<T, bool>> predicate = null) where T : SessionLog
        {
            // Look at the Repository for all types of the generic type
            var query = AuditLogRepository.FindAll().OfType<T>();
            // If we have a defined predicate - than limit the query by that expression
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        /// <summary>
        /// Get list of Objects
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<SessionLog> GetSessionLogs(int Id, string name)
        {
            IEnumerable<SessionLog> returnValues = Enumerable.Empty<SessionLog>();
            returnValues = GetSessionLogs<SessionLog>(Id, name).ToList();
            return returnValues;
        }
        #endregion

        #region User

        /// <summary>
        /// Remove an object
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(User obj)
        {
            if (obj.Id > 0)
                UserRepository.Delete(obj);

            UserRepository.Commit();
        }

        /// <summary>
        /// Save an object
        /// </summary>
        /// <param name="obj"></param>
        public void Save(User obj)
        {
            if (obj.Id <= 0)
            {
                UserRepository.Add(obj);
            }
            else
                UserRepository.Attach(obj);

            UserRepository.Commit();
        }

        public IQueryable<T> GetUsers<T>(Expression<Func<T, bool>> predicate = null) where T : User
        {
            var query = UserRepository.FindAll().OfType<T>();
            if (predicate != null)
                query = query.Where(predicate);            

            return query;
        }

        /// <summary>
        /// Get users using FindAll
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="userName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetUsers<T>(int id, string userName, Expression<Func<T, bool>> predicate = null) where T : User
        {
            // Look at the Repository for all types of the generic type
            var query = UserRepository.FindAll().OfType<T>();
            // If we have a defined predicate - than limit the query by that expression
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        /// <summary>
        /// Get users using Find
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetUsers<T>(string userName, Expression<Func<T, bool>> predicate = null) where T : User
        {
            // Look at the Repository for all types of the generic type
            var query = UserRepository.FindAll().OfType<T>();
            // If we have a defined predicate - than limit the query by that expression
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }
        
        /// <summary>
        /// Returns list of users
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IEnumerable<User> GetUsers(string userName)
        {
            IEnumerable<User> returnValues = Enumerable.Empty<User>();
            returnValues = GetUsers<User>(userName).ToList();
            return returnValues;
        }        
            
        #endregion

        #region Report
        /// <summary>
        /// Remove an object
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(Report obj)
        {
            if (obj.Id > 0)
                ReportRepository.Delete(obj);

            ReportRepository.Commit();
        }

        /// <summary>
        /// Save an object
        /// </summary>
        /// <param name="obj"></param>
        public void Save(Report obj)
        {
            if (obj.Id <= 0)
            {
                ReportRepository.Add(obj);
            }
            else
                ReportRepository.Attach(obj);

            ReportRepository.Commit();
        }

        /// <summary>
        /// Get list of Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetReports<T>(Expression<Func<T, bool>> predicate = null) where T : Report
        {
            var query = ReportRepository.FindAll().OfType<T>();
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        /// <summary>
        /// Get list of Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <param name="name"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetReports<T>(int Id, string name, Expression<Func<T, bool>> predicate = null) where T : Report
        {
            // Look at the Repository for all types of the generic type
            var query = ReportRepository.FindAll().OfType<T>();
            // If we have a defined predicate - than limit the query by that expression
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        /// <summary>
        /// Get list of Objects
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<Report> GetReports(int Id, string name)
        {
            IEnumerable<Report> returnValues = Enumerable.Empty<Report>();
            returnValues = GetReports<Report>(Id, name).ToList();
            return returnValues;
        }

        #endregion

        #region Group

        /// <summary>
        /// Remove an object
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(Group obj)
        {
            if (obj.Id > 0)
                GroupRepository.Delete(obj);

            GroupRepository.Commit();
        }

        /// <summary>
        /// Save an object
        /// </summary>
        /// <param name="obj"></param>
        public void Save(Group obj)
        {
            if (obj.Id <= 0)
            {
                GroupRepository.Add(obj);
            }
            else
                GroupRepository.Attach(obj);

            GroupRepository.Commit();
        }

        /// <summary>
        /// Get list of Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetGroups<T>(Expression<Func<T, bool>> predicate = null) where T : Group
        {
            var query = GroupRepository.FindAll().OfType<T>();
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        /// <summary>
        /// Get list of Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <param name="name"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetGroups<T>(int id, string name, Expression<Func<T, bool>> predicate = null) where T : Group
        {
            // Look at the Repository for all types of the generic type
            var query = GroupRepository.FindAll().OfType<T>();
            // If we have a defined predicate - than limit the query by that expression
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }
        #endregion        
    }
}
