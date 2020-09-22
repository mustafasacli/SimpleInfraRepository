namespace SimpleInfra.Data
{
    using System.Data.Entity;

    /// <summary>
    /// Defines the <see cref="SimpleBaseRepository{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleBaseRepository<T> : SimpleBaseDataRepository<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleBaseRepository{T}"/> class.
        /// </summary>
        /// <param name="context">DbContext instance.</param>
        public SimpleBaseRepository(DbContext context) : base(context)
        {
        }
    }
}