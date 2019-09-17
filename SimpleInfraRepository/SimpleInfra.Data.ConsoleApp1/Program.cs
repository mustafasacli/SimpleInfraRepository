using SimpleInfra.Data.ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Configuration;
using Mst.Dexter.Extensions;

namespace SimpleInfra.Data.ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            int? repositoryType = null;
            repositoryType = ConfigurationManager.AppSettings["repoType"].ToIntNullable();

            using (var repo = GetRepository<PersonalFile>(repositoryType)
                /*new OraRepository<PersonalFile>()*/)
            {
                var files = repo
                    .GetAll(q => q.Id > 100, asNoTracking: true)
                    .ToList() ?? new List<PersonalFile>();

                foreach (var item in files)
                {
                    Console.WriteLine(item.ToString());
                    Console.WriteLine("-------------------------------------");
                }
            }

            Console.ReadKey();
        }

        static SimpleBaseDataRepository<T> GetRepository<T>(int? repositoryType = null) where T : class
        {
            switch (repositoryType.GetValueOrDefault(0))
            {
                case 1:
                    return new OraRepository<T>();

                case 2:
                    return new MsSqlRepository<T>();

                case 3:
                    return new PostgreRepository<T>();

                default:
                    return new OraRepository<T>();
            }
            // return new OraRepository<T>();
        }
    }
}
