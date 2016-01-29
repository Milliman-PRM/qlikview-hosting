using SystemReporting.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SystemReporting.Service
{
    public interface IMillimanService
    {
        #region Services

        #region AuditLog
        void Remove(AuditLog obj);
        void Save(AuditLog obj);
        IQueryable<T> GetAuditLogs<T>(Expression<Func<T, bool>> predicate = null) where T : AuditLog;
        #endregion
        #region IisLog
        void Remove(IisLog obj);
        void Save(IisLog obj);
        IQueryable<T> GetIisLogs<T>(Expression<Func<T, bool>> predicate = null) where T : IisLog;
        #endregion
        #region SessionLog
        void Remove(SessionLog obj);
        void Save(SessionLog obj);
        IQueryable<T> GetSessionLogs<T>(Expression<Func<T, bool>> predicate = null) where T : SessionLog;
        #endregion
        #region User
        void Remove(User obj);
        void Save(User obj);
         T GetUser<T>(Expression<Func<T, bool>> predicate) where T : User;
        IQueryable<T> GetUsers<T>(Expression<Func<T, bool>> predicate = null) where T : User;
        IEnumerable<User> SelectAll();
        #endregion
        #region Report
        void Remove(Report obj);
        void Save(Report obj);
        IQueryable<T> GetReports<T>(Expression<Func<T, bool>> predicate = null) where T : Report;
        #endregion
        #region Group
        void Remove(Group obj);
        void Save(Group obj);
        IQueryable<T> GetGroups<T>(Expression<Func<T, bool>> predicate = null) where T : Group;
        #endregion
        #endregion
    }
}
