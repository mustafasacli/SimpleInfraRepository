namespace SimpleInfra.UoW
{
    using SimpleFileLogging;
    using SimpleFileLogging.Interfaces;
    using SimpleFileLogging.Enums;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    internal static class SimpleUnitOfWorkCache
    {
        /// <summary>
        /// Defines the contextTypes
        /// </summary>
        private static List<Type> contextTypes = new List<Type>();

        /// <summary>
        /// Defines the contextInstanceList
        /// </summary>
        private static ConcurrentDictionary<string, DbContext> contextInstanceList
            = new ConcurrentDictionary<string, DbContext>();

        /// <summary>
        /// Defines the contextTypeList
        /// </summary>
        private static ConcurrentDictionary<string, Type> contextTypeList
            = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// Defines the entityTypesList
        /// </summary>
        private static ConcurrentDictionary<string, string> entityTypesList
            = new ConcurrentDictionary<string, string>();

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

        static SimpleUnitOfWorkCache()
        {
            MapEntityToContext();
        }

        private static void MapEntityToContext()
        {
            try
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                if (Directory.Exists(path + "bin\\"))
                    path += "bin\\";
                var dataFiles = Directory.GetFiles(path, "*.dll") ?? new string[] { };
                foreach (var file in dataFiles)
                {
                    try
                    {
                        var assembly = Assembly.LoadFile(file);
                        var referencedAssemblies = assembly.GetReferencedAssemblies() ?? new AssemblyName[0];
                        referencedAssemblies.ToList().ForEach(a =>
                        {
                            try
                            { Assembly.Load(a); }
                            catch (Exception e2)
                            {
                                Console.WriteLine(e2.ToString());
                                Logger?.Error(e2, $"Asemmbly could not be loaded. Assembly: {a.FullName}");
                                throw;
                            }
                        });
                        var ctxs =
                        assembly.GetExportedTypes()
                            .Where(q => q != typeof(DbContext) && !q.Namespace.StartsWith("System.Data.Entity")
                            && q.IsPublic && q.IsClass && typeof(DbContext).IsAssignableFrom(q))
                            .ToList();

                        contextTypes.AddRange(ctxs);

                        //RegisterAssembly(assembly);

                        Logger?.Info($"\"{assembly.FullName}\" is loaded.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Logger?.Error(ex, $"File Name: {file}");
                    }
                }
            }
            catch (Exception ex2)
            {
                Console.WriteLine(ex2.ToString());
                Logger?.Error(ex2);
            }

            contextTypes = contextTypes.Distinct().ToList();
            contextTypes.ForEach(q =>
            {
                // Type Listesi Dictionary ye atılıyor.
                contextTypeList.GetOrAdd(q.FullName, q);

                // instance Dictionary is being initiliazed.
                //contextInstanceList.GetOrAdd(q.FullName, (string name) =>
                //{
                //    return Activator.CreateInstance(q) as DbContext;
                //});
                var dbSetProps = new List<Type>();

                q.GetRuntimeProperties()
             .Where(o =>
                 o.PropertyType.IsGenericType &&
                 o.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                 o.PropertyType.GenericTypeArguments.Count() > 0)
                 .Select(r => r.PropertyType.GenericTypeArguments)
                 .ToList()
                 .ForEach(r =>
                 {
                     dbSetProps.AddRange(r);
                 });
                dbSetProps = dbSetProps.Distinct().ToList();
                // Entity Name-DbContext Name mapping
                dbSetProps.ForEach(p =>
                {
                    entityTypesList.GetOrAdd(p.FullName, q.FullName);
                });
            });
        }

        /// <summary>
        /// Gets DbContext instance from cache if exist , if instance not exist create and return instance.
        /// </summary>
        /// <param name="contextTypeName">The contextTypeName<see cref="string"/></param>
        /// <returns>The DbContext instance.<see cref="DbContext"/></returns>
        internal static DbContext GetDbContext(string contextTypeName)
        {
            var context = contextInstanceList.GetOrAdd(contextTypeName,
               (string name) =>
              {
                  var typ = contextTypeList[contextTypeName];
                  return Activator.CreateInstance(typ) as DbContext;
              });

            return context;
        }

        internal static string GetDbContextName<T>() where T : class
        {
            var contextTypeName = entityTypesList[typeof(T).FullName];
            return contextTypeName;
        }

        internal static bool TryRemove(string contextTypeName, out DbContext context)
        {
            return contextInstanceList.TryRemove(contextTypeName, out context);
        }
    }
}
