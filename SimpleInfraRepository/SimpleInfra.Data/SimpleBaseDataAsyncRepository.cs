////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	SimpleBaseDataRepository.cs
//
// summary:	Implements the simple base data repository class
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace SimpleInfra.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

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
        /// <param name="predicate"></param>
        /// <param name="asNoTracking"></param>
        /// <returns></returns>
        public Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return dbSet
                        .Where(predicate)
                .AsNoTracking()
                .SingleOrDefaultAsync();
            }
            else
            {
                return dbSet
                        .Where(predicate)
                .SingleOrDefaultAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="asNoTracking"></param>
        /// <returns></returns>
        public Task<T> FirstAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return dbSet
                        .Where(predicate)
                .AsNoTracking()
                .FirstAsync();
            }
            else
            {
                return dbSet
                        .Where(predicate)
                .FirstAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="asNoTracking"></param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return dbSet
                        .Where(predicate)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            }
            else
            {
                return dbSet
                        .Where(predicate)
                .FirstOrDefaultAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return dbSet.CountAsync(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return dbContext.Database.ExecuteSqlCommandAsync(sql: sql, parameters: parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionalBehavior"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<int> ExecuteSqlCommandAsync(TransactionalBehavior transactionalBehavior, string sql, params object[] parameters)
        {
            return dbContext.Database.ExecuteSqlCommandAsync(transactionalBehavior: transactionalBehavior, sql: sql, parameters: parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Task<T> GetByIdAsync(params object[] oid)
        {
            if (oid == null)
                return null;

            if (oid.All(q => CheckObjectIsNull(q)))
            {
                return null;
            }

            var entity = dbSet.FindAsync(oid);
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<int> SaveChangesAsync()
        {
            try
            {
                return dbContext.SaveChangesAsync();
            }
            catch (DbEntityValidationException dve)
            {
                // Exception logging
                try
                {
                    if (LogError)
                    {
                        SimpleRepoLogger?.Error(dve);
                    }
                }
                catch
                { }
                finally
                { }

                try
                {
                    if (LogError)
                    {
                        var errors = GetValidationErrors(dve);
                        SimpleRepoLogger?.Error(errors.ToArray());
                    }
                }
                catch
                { }
                finally
                { }

                throw;
            }
            catch (Exception e)
            {
                try
                {
                    if (LogError)
                    {
                        // Exception logging
                        SimpleRepoLogger?.Error(e);
                    }
                }
                finally
                { }

                throw;
            }
        }
    }
}
