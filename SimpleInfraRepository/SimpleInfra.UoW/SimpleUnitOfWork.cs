namespace SimpleInfra.UoW
{
    using SimpleInfra.Data;
    using SimpleInfra.UoW.Interfaces;
    using SimpleFileLogging;
    using SimpleFileLogging.Interfaces;
    using SimpleFileLogging.Enums;
    using System;

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
            var isContextDisposed = false;

            try
            { var config = context.Configuration; }
            catch (ObjectDisposedException dex)
            {
                isContextDisposed = true;
                Logger?.Error(dex,
                    $"Entity: {typeof(T).FullName}",
                    $"contextTypeName: {contextTypeName}");
            }

            if (context == null || isContextDisposed)
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
                SimpleUnitOfWorkCache.TryRemove(contextTypeName, out context);
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
                Logger?.Error(ee);
            }
        }
    }
}
