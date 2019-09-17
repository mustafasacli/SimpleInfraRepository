namespace SimpleInfra.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Interface for simple data async repository. </summary>
    ///
    /// <remarks>   Msacli, 30.04.2019. </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public interface ISimpleDataAsyncRepository<T> : ISimpleDataRepository<T> where T : class
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a t using the given predicate. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   A T. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get First the given predicate. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   A T. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        Task<T> FirstAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get First or default. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
        /// <param name="asNoTracking"> asNoTracking parameter. </param>
        ///
        /// <returns>   A T. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Counts the given predicate. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   check Any the given predicate. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

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
        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);

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
        Task<int> ExecuteSqlCommandAsync(TransactionalBehavior transactionalBehavior, string sql, params object[] parameters);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets by id. </summary>
        ///
        /// <param name="oid">  The oid. </param>
        ///
        /// <returns>   The by id. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        Task<T> GetByIdAsync(params object[] oid);
    }
}
