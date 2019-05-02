////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	SimpleBaseDataRepository.cs
//
// summary:	Implements the simple base data repository class
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace MstInfra.Data.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Linq.Expressions;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A simple base data repository. </summary>
    ///
    /// <remarks>   Msacli, 30.04.2019. </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public abstract class SimpleBaseDataRepository<T> : SimpleDataRepository<T> where T : class
    {
        /// <summary>
        /// Context for the database.
        /// </summary>
        protected readonly DbContext dbContext;

        /// <summary>
        /// Set the database belongs to.
        /// </summary>
        protected readonly DbSet<T> dbSet;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised constructor for use only by derived class. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="dbContext">        Context for the database. </param>
        /// <param name="errorLogEnable">   (Optional) True if error log enable. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected SimpleBaseDataRepository(DbContext dbContext, bool errorLogEnable = true)
        {
            this.LogError = errorLogEnable;
            this.dbContext = dbContext;
            dbSet = dbContext.Set<T>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the log error. </summary>
        ///
        /// <value> True if logs error, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool LogError
        { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the log info. </summary>
        ///
        /// <value> True if logs info, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool LogInfo
        { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the log debug. </summary>
        ///
        /// <value> True if logs debug, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool LogDebug
        { get; set; }

        #region IRepository Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Includes the given file. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   An IQueryable&lt;T&gt; </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual IQueryable<T> Include(string path)
        {
            return dbSet.Include(path);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Includes the given file. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <typeparam name="TProperty">    Type of the property. </typeparam>
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   An IQueryable&lt;T&gt; </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> path)
        {
            return dbSet.Include(path);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <returns>   all. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual IQueryable<T> GetAll()
        {
            var iq = dbSet.AsNoTracking();

            return iq;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>   all. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            var iq = dbSet
                .AsNoTracking()
                .Where(predicate);

            return iq;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets with page. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <typeparam name="TKey"> Type of the key. </typeparam>
        /// <param name="predicate">            The predicate. </param>
        /// <param name="keySelectorForOrder">  (Optional) The key selector for order. </param>
        /// <param name="isOrderByDesc">        (Optional) True if is order by description, false if not. </param>
        /// <param name="pageNumber">           (Optional) The page number. </param>
        /// <param name="pageItemCount">        (Optional) Number of page items. </param>
        ///
        /// <returns>   The with page. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual IQueryable<T> GetWithPage<TKey>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, TKey>> keySelectorForOrder = null,
            bool isOrderByDesc = false,
            uint pageNumber = 1, uint pageItemCount = 1)
        {
            uint pageNo = pageNumber < 1 ? 1 : pageNumber;
            uint itemCount = pageItemCount < 1 ? 1 : pageItemCount;

            var iq = dbSet
            .AsNoTracking()
            .Where(predicate);

            if (keySelectorForOrder != null)
            {
                iq = isOrderByDesc ?
                    iq.OrderByDescending(keySelectorForOrder) : iq.OrderBy(keySelectorForOrder);
            }

            var result = iq
                .Skip((int)((pageNo - 1) * itemCount))
                .Take((int)itemCount);

            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all with page. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <typeparam name="TKey"> Type of the key. </typeparam>
        /// <param name="keySelectorForOrder">  (Optional) The key selector for order. </param>
        /// <param name="isOrderByDesc">        (Optional) True if is order by description, false if not. </param>
        /// <param name="pageNumber">           (Optional) The page number. </param>
        /// <param name="pageItemCount">        (Optional) Number of page items. </param>
        ///
        /// <returns>   all with page. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual IQueryable<T> GetAllWithPage<TKey>(
            Expression<Func<T, TKey>> keySelectorForOrder = null,
            bool isOrderByDesc = false,
            uint pageNumber = 1, uint pageItemCount = 1)
        {
            uint pageNo = pageNumber < 1 ? 1 : pageNumber;
            uint itemCount = pageItemCount < 1 ? 1 : pageItemCount;

            var iq = dbSet
            .AsNoTracking()
            .AsQueryable();

            if (keySelectorForOrder != null)
            {
                iq = isOrderByDesc ?
                    iq.OrderByDescending(keySelectorForOrder) : iq.OrderBy(keySelectorForOrder);
            }

            var result = iq
                .Skip((int)((pageNo - 1) * itemCount))
                .Take((int)itemCount);

            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a t using the given predicate. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>   A T instance. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual T Get(Expression<Func<T, bool>> predicate)
        {
            return dbSet
                .Where(predicate)
                .AsNoTracking()
                .SingleOrDefault();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets by id. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="oid">  The oid. </param>
        ///
        /// <returns>   The by id. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual T GetById(object oid)
        {
            if (CheckObjectIsNull(oid))
            {
                return null;
            }

            var entity = dbSet.Find(oid);
            return entity;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Check object is null. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="oid">  The oid. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected bool CheckObjectIsNull(object oid)
        {
            return oid == null || oid == (object)DBNull.Value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Firsts the given predicate. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>   A T instance. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual T First(Expression<Func<T, bool>> predicate)
        {
            return dbSet.First(predicate);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   First or default. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>   A T instance. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return dbSet.FirstOrDefault(predicate);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Counts the given predicate. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual int Count(Expression<Func<T, bool>> predicate)
        {
            return dbSet.Count(predicate);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Anies the given predicate. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return dbSet.Any(predicate);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds entity. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="entity">   The entity to add. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Add(T entity)
        {
            dbSet.Add(entity);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds a range. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="entities"> An IEnumerable&lt;T&gt; of items to append to this. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void AddRange(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a new T. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <returns>   A T. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public T Create()
        {
            return dbSet.Create<T>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   SQL query. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <typeparam name="TElement"> Type of the element. </typeparam>
        /// <param name="sql">          The SQL. </param>
        /// <param name="parameters">   A variable-length parameters list containing parameters. </param>
        ///
        /// <returns>   A DbRawSqlQuery&lt;TElement&gt; </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual DbRawSqlQuery<TElement> SqlQuery<TElement>(string sql, params object[] parameters) where TElement : class
        {
            return dbContext.Database.SqlQuery<TElement>(sql: sql, parameters: parameters);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   SQL set query. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="sql">          The SQL. </param>
        /// <param name="parameters">   A variable-length parameters list containing parameters. </param>
        ///
        /// <returns>   A DbSqlQuery&lt;T&gt; </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual DbSqlQuery<T> SqlSetQuery(string sql, params object[] parameters)
        {
            return dbSet.SqlQuery(sql: sql, parameters: parameters);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates the given entity. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="entity">   The entity to delete. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Update(T entity)
        {
            dbSet.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Deletes the given oid. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="entity">   The entity to delete. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Delete(T entity)
        {
            if (entity == null)
                return;

            dbSet.Remove(entity);
            dbContext.Entry(entity).State = EntityState.Deleted;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Deletes the given oid. if oid is null or DbNull, delete operation will be cancelled. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="oid">  The oid. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Delete(object oid)
        {
            if (CheckObjectIsNull(oid))
                return;

            var entity = dbSet.Find(oid);

            Delete(entity);
        }

        #endregion IRepository Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Saves the changes. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <exception cref="DbEntityValidationException">
        /// Thrown when a Database Entity Validation error condition occurs.
        /// </exception>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int SaveChanges()
        {
            var result = -1;

            try
            {
                result = dbContext.SaveChanges();
            }
            catch (DbEntityValidationException dve)
            {
                /// Exception logging
                try
                {
                    if (this.LogError) { }
                    /// KykFileLogger.LogError(dve);
                }
                finally
                { }

                try
                {
                    if (this.LogError)
                    {
                        var errors = GetValidationErrors(dve);
                        /// KykFileLogger.LogError(errors.ToArray());
                    }
                }
                finally
                { }

                throw;
            }
            catch (Exception e)
            {
                /// Exception logging
                try
                {
                    if (this.LogError) { }
                    /// KykFileLogger.LogError(e);
                }
                finally
                { }

                throw;
            }

            return result;
        }

        #region IDisposable Members

        /// <summary>
        /// True if disposed.
        /// </summary>
        private bool disposed = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="disposing">
        /// True to release both managed and unmanaged resources; false to release only unmanaged resources. 
        /// </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }

            this.disposed = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets validation errors. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="ex">   Details of the exception. </param>
        ///
        /// <returns>   The validation errors. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected List<string> GetValidationErrors(DbEntityValidationException ex)
        {
            var errorMessages = new List<string>();
            try
            {
                foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
                {
                    string entityName = validationResult.Entry?.Entity.GetType().Name;
                    foreach (DbValidationError error in validationResult.ValidationErrors)
                    {
                        errorMessages.Add(string.Format("{0}.{1} : {2}", entityName, error.PropertyName, error.ErrorMessage));
                    }
                }
            }
            catch (Exception e)
            {
                /// KykFileLogger.LogError(e);
            }
            return errorMessages;
        }
    }
}
