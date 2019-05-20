namespace SimpleInfra.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Interface for simple data repository. </summary>
    ///
    /// <remarks>   Msacli, 30.04.2019. </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public interface ISimpleDataRepository<T> : IDisposable where T : class
    {
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
        ///
        /// <returns>   A T. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        T Get(Expression<Func<T, bool>> predicate, bool asNoTracking = false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get First the given predicate. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>   A T. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        T First(Expression<Func<T, bool>> predicate, bool asNoTracking = false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get First or default. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
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
        /// <summary>   Deletes the given oid. </summary>
        ///
        /// <param name="entity">   The entity to delete. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void Delete(T entity);

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
        /// <summary>   Gets by id. </summary>
        ///
        /// <param name="oid">  The oid. </param>
        ///
        /// <returns>   The by id. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        T GetById(object oid);
    }
}
