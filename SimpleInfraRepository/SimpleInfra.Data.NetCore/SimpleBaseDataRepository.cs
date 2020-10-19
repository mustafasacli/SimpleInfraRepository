////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	SimpleBaseDataRepository.cs
//
// summary:	Implements the simple base data repository class
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace SimpleInfra.Data.NetCore
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.SqlServer;
    using Microsoft.EntityFrameworkCore.SqlServer.Query;
    using Microsoft.EntityFrameworkCore.Query;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A simple base data repository. </summary>
    ///
    /// <remarks>   Msacli, 30.04.2019. </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public abstract partial class SimpleBaseDataRepository<T> : ISimpleDataRepository<T> where T : class
    {
        /// <summary>
        ///
        /// </summary>
        public event ContextDisposedHandler DisposedHandler;

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
        /// <remarks>   Mustafa SAÇLI, 5.05.2019. </remarks>
        ///
        /// <param name="dbContext">        Context for the database. </param>
        /// <param name="simpleRepoLogger"> (Optional) The simple repo logger. </param>
        /// <param name="errorLogEnable">   (Optional) True if error log enable. </param>
        /// <param name="lazyLoadingEnabled">   (Optional) True if Lazy Load enable. </param>
        /// <param name="autoDetectChangesEnabled">   (Optional) True if Auto Detect Changes Enabled. </param>
        /// <param name="proxyCreationEnabled">   (Optional) True if Proxy Creation Enabled. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected SimpleBaseDataRepository(
            DbContext dbContext, ISimpleRepoLogger simpleRepoLogger = null, bool errorLogEnable = true)
        {
            this.LogError = errorLogEnable;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<T>();
            this.SimpleRepoLogger = simpleRepoLogger;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the simple repo logger. </summary>
        ///
        /// <value> The simple repo logger. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ISimpleRepoLogger SimpleRepoLogger
        { get; protected set; }

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
        /// <summary>   Includes the given entity type name. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="path"> Full path name of the entity type name. </param>
        ///
        /// <returns>   An IQueryable&lt;T&gt; </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual IQueryable<T> Include(string path)
        {
            return dbSet.Include(path);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Includes the given entity property. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <typeparam name="TProperty">    Type of the property. </typeparam>
        /// <param name="path"> Full path name of the entity. </param>
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
        public virtual IQueryable<T> GetAll(bool asNoTracking = false)
        {
            IQueryable<T> iq = null;

            if (asNoTracking)
            {
                iq = dbSet
                        .AsNoTracking();
            }
            else
            {
                iq = dbSet;
            }

            return iq;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="predicate">    The predicate. </param>
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   all. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            IQueryable<T> iq = null;

            if (asNoTracking)
            {
                iq = dbSet
                        .AsNoTracking()
                        .Where(predicate);
            }
            else
            {
                iq = dbSet
                        .Where(predicate);
            }

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
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   The with page. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual IQueryable<T> GetWithPage<TKey>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, TKey>> keySelectorForOrder = null,
            bool isOrderByDesc = false,
            uint pageNumber = 1, uint pageItemCount = 1, bool asNoTracking = false)
        {
            uint pageNo = pageNumber < 1 ? 1 : pageNumber;
            uint itemCount = pageItemCount < 1 ? 1 : pageItemCount;

            IQueryable<T> iq = null;
            if (asNoTracking)
            {
                iq = dbSet
                        .AsNoTracking()
                        .Where(predicate);
            }
            else
            {
                iq = dbSet
                        .Where(predicate);
            }

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
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   all with page. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual IQueryable<T> GetAllWithPage<TKey>(
            Expression<Func<T, TKey>> keySelectorForOrder = null,
            bool isOrderByDesc = false,
            uint pageNumber = 1, uint pageItemCount = 1, bool asNoTracking = false)
        {
            uint pageNo = pageNumber < 1 ? 1 : pageNumber;
            uint itemCount = pageItemCount < 1 ? 1 : pageItemCount;

            IQueryable<T> iq = null;
            if (asNoTracking)
            {
                iq = dbSet
                        .AsNoTracking();
            }
            else
            {
                iq = dbSet;
            }

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
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   A T instance. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual T Get(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            T instance = null;

            if (asNoTracking)
            {
                instance = dbSet
                        .Where(predicate)
                .AsNoTracking()
                .SingleOrDefault();
            }
            else
            {
                instance = dbSet
                        .Where(predicate)
                .SingleOrDefault();
            }

            return instance;
        }

        /// <summary>   Gets a t using the given predicate. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="predicate">    The predicate. </param>
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   A T instance. </returns>
        public virtual T Single(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            T instance = null;

            if (asNoTracking)
            {
                instance = dbSet
                        .Where(predicate)
                .AsNoTracking()
                .Single();
            }
            else
            {
                instance = dbSet
                        .Where(predicate)
                .Single();
            }

            return instance;
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
        public virtual T GetById(params object[] oid)
        {
            if (oid == null)
                return null;

            if (oid.All(q => CheckObjectIsNull(q)))
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

        /// <summary>
        ///  Finds an entity with the given primary key values. If an entity with the given
        ///     primary key values exists in the context, then it is returned immediately without
        ///     making a request to the store. Otherwise, a request is made to the store for
        ///     an entity with the given primary key values and this entity, if found, is attached
        ///     to the context and returned. If no entity is found in the context or the store,
        ///     then null is returned.
        /// </summary>
        /// <remarks>
        /// The ordering of composite key values is as defined in the EDM, which is in turn
        ///     as defined in the designer, by the Code First fluent API, or by the DataMember
        ///     attribute.
        /// </remarks>
        /// <exception cref="T:System.InvalidOperationException">
        /// Thrown if multiple entities exist in the context with the primary key values given.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// Thrown if the type of entity is not part of the data model for this context.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///  Thrown if the types of the key values do not match the types of the key values
        ///     for the entity type to be found.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///  Thrown if the context has been disposed.
        /// </exception>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        public virtual T Find(params object[] keyValues)
        {
            return dbSet.Find(keyValues);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get First the given predicate. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="predicate">    The predicate. </param>
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   A T instance. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual T First(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            T instance = null;

            if (asNoTracking)
            {
                instance = dbSet
                    .AsNoTracking()
                    .First(predicate);
            }
            else
            {
                instance = dbSet
                    .First(predicate);
            }

            return instance;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get First or default. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="predicate">    The predicate. </param>
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   A T instance. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            T instance = null;

            if (asNoTracking)
            {
                instance = dbSet
                    .AsNoTracking()
                    .FirstOrDefault(predicate);
            }
            else
            {
                instance = dbSet
                    .FirstOrDefault(predicate);
            }

            return instance;
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
        /// <summary>   Has any with the given predicate. </summary>
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
        /// <summary>   Adds an entity range. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="entities"> An IEnumerable&lt;T&gt; of items to append to.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void AddRange(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
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
        public virtual IQueryable<T> SqlQuery(string sql, params object[] parameters)
        {
            return dbSet.FromSqlRaw<T>(sql: sql, parameters: parameters);
        }

        /// <summary>
        ///     Executes the given DDL/DML command against the database. As with any API that
        ///     accepts SQL it is important to parameterize any user input to protect against
        ///     a SQL injection attack. You can include parameter place holders in the SQL query
        ///     string and then supply parameter values as additional arguments. Any parameter
        ///     values you supply will automatically be converted to a DbParameter.
        /// </summary>
        /// <param name="sql">The command string.</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns> The result returned by the database after executing the command.</returns>
        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            var result = -1;

            result = dbContext.Database.ExecuteSqlRaw(sql: sql, parameters: parameters);

            return result;
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

        /// <summary>
        /// Delete entity array.
        /// </summary>
        /// <param name="entities"> entity array</param>
        /// <returns>IEnumrable entity</returns>
        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            if (entities != null && entities.Count() >= 1)
            {
                foreach (T entity in entities)
                {
                    Update(entity);
                }
            }
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

        /// <summary>
        /// Delete entity array.
        /// </summary>
        /// <param name="entities"> entity array</param>
        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            if (entities != null && entities.Count() >= 1)
            {
                dbSet.RemoveRange(entities);
            }
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
            catch (Exception e)
            {
                try
                {
                    if (LogError)
                    {
                        SimpleRepoLogger?.Error(e);
                    }
                }
                catch { }

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
            if (!disposed)
            {
                if (disposing)
                {
                    if (this.IsContextDisposedOrNull() == false)
                    {
                        dbContext?.Dispose();
                        try
                        {
                            if (this.DisposedHandler != null)
                            { this.DisposedHandler.Invoke(this, new DbContextDisposingEventArgs(dbContext)); }
                        }
                        catch (Exception ee)
                        { this.SimpleRepoLogger?.Error(ee); }
                    }
                }
            }

            disposed = true;
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


        /// <summary>
        /// Checks Dbcontext is disposed.
        /// </summary>
        public bool IsContextDisposedOrNull()
        {
            if (dbContext == null)
                return true;

            var isContextDisposed = false;

            try
            { var database = dbContext.Database; }
            catch (ObjectDisposedException dex)
            {
                isContextDisposed = true;
                SimpleRepoLogger?.Error(dex,
                    $"Entity: {typeof(T).FullName}");
            }

            return isContextDisposed;
        }
    }
}