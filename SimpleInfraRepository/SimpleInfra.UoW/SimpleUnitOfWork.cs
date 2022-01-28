namespace SimpleInfra.UoW
{
    using SimpleInfra.Data;
    using SimpleInfra.UoW.Interfaces;
    using SimpleFileLogging;
    using SimpleFileLogging.Interfaces;
    using SimpleFileLogging.Enums;
    using System;
    using System.Data.Entity;

    /// <summary>
    /// Defines the <see cref="SimpleUnitOfWork" />
    /// </summary>
    public class SimpleUnitOfWork : ISimpleUnitOfWork
    {
        /// <summary>
        ///
        /// </summary>
        public SimpleUnitOfWork()
        {
        }

        private static Lazy<ISimpleLogger> lazyLogger = new Lazy<ISimpleLogger>(
            () =>
            {
                var logger = SimpleFileLogger.Instance;
                logger.LogDateFormatType = SimpleLogDateFormats.Day;
                return logger;
            }, isThreadSafe: true);

        /// <summary>
        /// Gets Logger instance.
        /// </summary>
        internal static ISimpleLogger Logger
        { get { return lazyLogger.Value; } }

        /// <summary>
        /// Gets DbContext intsance for given mapped entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns ISimpleDataRepository instance.</returns>
        public ISimpleDataRepository<T> GetRepository<T>() where T : class
        {
            var contextTypeName = SimpleUnitOfWorkCache.GetDbContextName<T>();
            var context = SimpleUnitOfWorkCache.GetDbContext(contextTypeName);

            var isContextDisposedOrNull = IsContextDisposedOrNull(context);

            if (isContextDisposedOrNull)
            {
                SimpleUnitOfWorkCache.TryRemove(contextTypeName, out context);
                context = SimpleUnitOfWorkCache.GetDbContext(contextTypeName);
            }

            var repo = new SimpleBaseRepository<T>(context);
            repo.DisposedHandler += Repo_DisposedHandler;

            return repo;
        }

        private void Repo_DisposedHandler(object sender, DbContextDisposingEventArgs eventargs)
        {
            try
            {
                var contextTypeName = eventargs.Context.GetType().FullName;
                var context = SimpleUnitOfWorkCache.GetDbContext(contextTypeName);
                var result = SimpleUnitOfWorkCache.TryRemove(contextTypeName, out context);
                if (!result)
                {
                    Logger?.Debug(string.Format($"{contextTypeName} could not be removed."));
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
                Logger?.Error(ee);
            }
        }

        /// <summary>
        /// Checks dbcontext is nul or disposed.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        protected bool IsContextDisposedOrNull(DbContext dbContext)
        {
            if (dbContext == null)
                return true;

            var isContextDisposed = false;

            try
            { var config = dbContext.Configuration; }
            catch (ObjectDisposedException dex)
            {
                isContextDisposed = true;
                Logger?.Error(dex,
                    $"Entity: {dbContext.GetType().FullName}");
            }

            return isContextDisposed;
        }
    }
}
