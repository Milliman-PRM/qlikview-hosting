using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        #region Add/Update/Delete
        void Add(T obj);
        void Update(T obj);
        void Delete(object id);
        void Attach(T obj);
        void Save();
        void Commit();
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
