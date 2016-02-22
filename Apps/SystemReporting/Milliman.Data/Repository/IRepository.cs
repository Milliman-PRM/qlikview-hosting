using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SystemReporting.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        DbContext GetContext();
        void Commit();
        void Dispose();

        #region CRUD Methods
        void Add(T obj);
        void Update(T obj);
        void Delete(object id);
        void Attach(T obj);
        void Save();
        #endregion

        #region Search

        IEnumerable<T> SelectAll();
        T SelectByID(object id);

        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindAll();
        T First(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        #endregion
    }
}
