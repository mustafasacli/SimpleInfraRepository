namespace SimpleInfra.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventargs"></param>
    public delegate void ContextDisposedHandler(object sender, DbContextDisposingEventArgs eventargs);

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Interface for simple data repository. </summary>
    ///
    /// <remarks>   Msacli, 30.04.2019. </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public interface ISimpleDataRepository<T> : ISimpleDataAsyncRepository<T>, IDisposable where T : class
    {
        /// <summary>
        ///
        /// </summary>
        event ContextDisposedHandler DisposedHandler;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the simple repo logger. </summary>
        ///
        /// <value> The simple repo logger. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ISimpleRepoLogger SimpleRepoLogger
        { get; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the log error. </summary>
        ///
        /// <value> True if log error, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool LogError
        { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the log debug. </summary>
        ///
        /// <value> True if log debug, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool LogDebug
        { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the log info. </summary>
        ///
        /// <value> True if log info, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool LogInfo
        { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Includes the given property. </summary>
        ///
        /// <param name="path"> Full path name of the property. </param>
        ///
        /// <returns>   An IQueryable&lt;T&gt; </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        IQueryable<T> Include(string path);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Includes the given property. </summary>
        ///
        /// <typeparam name="TProperty">    Type of the property. </typeparam>
        /// <param name="path"> Full path name of the property. </param>
        ///
        /// <returns>   An IQueryable&lt;T&gt; </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> path);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all. </summary>
        ///
        /// <returns>   An IQueryable&lt;T&gt; </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        IQueryable<T> GetAll(bool asNoTracking = false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   An IQueryable&lt;T&gt; </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, bool asNoTracking = false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets with page. </summary>
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
        IQueryable<T> GetWithPage<TKey>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, TKey>> keySelectorForOrder = null,
            bool isOrderByDesc = false,
            uint pageNumber = 1, uint pageItemCount = 1, bool asNoTracking = false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all with page. </summary>
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
        IQueryable<T> GetAllWithPage<TKey>(
            Expression<Func<T, TKey>> keySelectorForOrder = null,
            bool isOrderByDesc = false,
            uint pageNumber = 1, uint pageItemCount = 1, bool asNoTracking = false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a t using the given predicate. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   A T. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        T Get(Expression<Func<T, bool>> predicate, bool asNoTracking = false);

        /// <summary>   Gets a t using the given predicate. </summary>
        ///
        /// <remarks>   Msacli, 30.04.2019. </remarks>
        ///
        /// <param name="predicate">    The predicate. </param>
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   A T instance. </returns>
        T Single(Expression<Func<T, bool>> predicate, bool asNoTracking = false);

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
        T Find(params object[] keyValues);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get First the given predicate. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   A T. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        T First(Expression<Func<T, bool>> predicate, bool asNoTracking = false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get First or default. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   A T. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        T FirstOrDefault(Expression<Func<T, bool>> predicate, bool asNoTracking = false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Counts the given predicate. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        int Count(Expression<Func<T, bool>> predicate);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   check Any the given predicate. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool Any(Expression<Func<T, bool>> predicate);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds entity. </summary>
        ///
        /// <param name="entity">   The entity to delete. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void Add(T entity);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds a range. </summary>
        ///
        /// <param name="entities"> An IEnumerable&lt;T&gt; of items to append to this. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void AddRange(IEnumerable<T> entities);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates the given entity. </summary>
        ///
        /// <param name="entity">   The entity to delete. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update(T entity);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates a range. </summary>
        ///
        /// <param name="entities"> An IEnumerable&lt;T&gt; of items to update for this. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void UpdateRange(IEnumerable<T> entities);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Deletes the given oid. </summary>
        ///
        /// <param name="entity">   The entity to delete. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void Delete(T entity);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Deletes a range. </summary>
        ///
        /// <param name="entities"> An IEnumerable&lt;T&gt; of items to remove from this. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void DeleteRange(IEnumerable<T> entities);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Deletes the given oid. </summary>
        ///
        /// <param name="oid">  The oid. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void Delete(object oid);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   SQL set query. </summary>
        ///
        /// <param name="sql">          The SQL. </param>
        /// <param name="parameters">   A variable-length parameters list containing parameters. </param>
        ///
        /// <returns>   A DbSqlQuery&lt;T&gt; </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        DbSqlQuery<T> SqlSetQuery(string sql, params object[] parameters);

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
        int ExecuteSqlCommand(string sql, params object[] parameters);

        /// <summary>
        ///     Executes the given DDL/DML command against the database. As with any API that
        ///     accepts SQL it is important to parameterize any user input to protect against
        ///     a SQL injection attack. You can include parameter place holders in the SQL query
        ///     string and then supply parameter values as additional arguments. Any parameter
        ///     values you supply will automatically be converted to a DbParameter.
        /// </summary>
        /// <param name="transactionalBehavior"> TransactionalBehavior parameter. </param>
        /// <param name="sql">The command string.</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns> The result returned by the database after executing the command.</returns>
        int ExecuteSqlCommand(TransactionalBehavior transactionalBehavior, string sql, params object[] parameters);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   SQL query. </summary>
        ///
        /// <typeparam name="TElement"> Type of the element. </typeparam>
        /// <param name="sql">          The SQL. </param>
        /// <param name="parameters">   A variable-length parameters list containing parameters. </param>
        ///
        /// <returns>   A DbRawSqlQuery&lt;TElement&gt; </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        DbRawSqlQuery<TElement> SqlQuery<TElement>(string sql, params object[] parameters) where TElement : class;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   SQL query. </summary>
        ///
        /// <param name="elementType">          The Element Type. </param>
        /// <param name="sql">          The SQL. </param>
        /// <param name="parameters">   A variable-length parameters list containing parameters. </param>
        ///
        /// <returns>   A DbRawSqlQuery </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        DbRawSqlQuery SqlQuery(Type elementType, string sql, params object[] parameters);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets by id. </summary>
        ///
        /// <param name="oid">  The oid. </param>
        ///
        /// <returns>   The by id. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        T GetById(params object[] oid);

        /// <summary>
        /// save changes and returns result.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Checks Dbcontext is disposed.
        /// </summary>
        bool IsContextDisposedOrNull();
    }
}