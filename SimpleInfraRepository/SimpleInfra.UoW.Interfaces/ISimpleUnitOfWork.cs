namespace SimpleInfra.UoW.Interfaces
{
    using SimpleInfra.Data;

    /// <summary>
    /// 
    /// </summary>
    public interface ISimpleUnitOfWork
    {
        /// <summary>
        /// Gets ISimpleDataRepository instance for giren entity class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>returns a class instance what is implemented by ISimpleDataRepository.</returns>
        ISimpleDataRepository<T> GetRepository<T>() where T : class;
    }
}
