using SystemReporting.Data.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SystemReporting.Data.Repository
{
    public class Repository<T> : IDisposable, IRepository<T> where T : class
    {
        private ApplicationDbContext _dbContext { get; set; }
        private DbSet<T> _entity { get; set; }

        /// <summary>
        /// Constructor opens the connection to DB
        /// </summary>
        public Repository()
        {
            this._dbContext = new ApplicationDbContext();
            //this is needed if we call the repository and not pass the db
            if (this._dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
            {
                this._dbContext.Database.Connection.Open();
            }
            _entity = _dbContext.Set<T>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        public Repository(ApplicationDbContext dbContext)
        {
            if (dbContext == null)
                GetContext();

            this._dbContext = dbContext;
            _entity = dbContext.Set<T>();
        }
       
        public void Dispose()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
            GC.SuppressFinalize(_dbContext);
        }

        public DbContext GetContext()
        {
            this._dbContext = new ApplicationDbContext();
            return _dbContext;
        }
        
        #region Functions Add/Update/Delete

        public void Add(T obj)
        {
            _entity.Add(obj);
        }

        public void Update(T obj)
        {
            _entity.Attach(obj);
            _dbContext.Entry(obj).State = EntityState.Modified;
        }

        public void Delete(object id)
        {
            T existing = _entity.Find(id);
            _entity.Remove(existing);
        }

        public void Attach(T entity)
        {
            _entity.Attach(entity);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// This method tells the current data store context to save all
        /// changes and synchronize with the data store.
        /// </summary>
        public void Commit()
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    _dbContext.Configuration.ValidateOnSaveEnabled = false;
                    _dbContext.Configuration.AutoDetectChangesEnabled = false;                    
                    using (var scope = new TransactionScope(TransactionScopeOption.Required,
                                                        new TransactionOptions
                                                        {
                                                            IsolationLevel = System.Transactions.IsolationLevel.Snapshot
                                                        }))
                    {
                        _dbContext.SaveChanges();
                        scope.Complete();
                    }
                    _dbContext.Configuration.ValidateOnSaveEnabled = true;
                    _dbContext.Configuration.AutoDetectChangesEnabled = true;
                    break;
                }
                catch (DbUpdateException ex)
                {
                    var innerEx = ex.InnerException;
                    if (innerEx.InnerException != null && innerEx.InnerException is Npgsql.NpgsqlException)
                        innerEx = innerEx.InnerException;

                    var message = innerEx != null ? innerEx.Message.ToLower() : string.Empty;

                    if (!string.IsNullOrEmpty(message)
                        && (message.Contains("deadlock victim")
                            || message.Contains("timeout")))
                    {
                        Dispose();
                        continue;                        
                    }
                    else
                    {
                        Dispose();
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region Search

        public IEnumerable<T> SelectAll()
        {
            //return _entity.ToList();
            return _entity.AsEnumerable();
        }

        public T SelectByID(object id)
        {
            return _entity.Find(id);
        }        

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return FindAll().Where(predicate);
        }

        public IQueryable<T> FindAll()
        {
            return _dbContext.Set<T>();
        }

        public T First(Expression<Func<T, bool>> predicate)
        {
            return FindAll().First(predicate);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return FindAll().FirstOrDefault(predicate);
        }

        #endregion
    }
}
