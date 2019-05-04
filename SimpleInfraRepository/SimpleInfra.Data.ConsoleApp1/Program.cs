using SimpleInfra.Data.ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInfra.Data.ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var repo = new OraRepository<PersonalFile>())
            {
                var files = repo
                    .GetAll(q => q.Id > 100)
                    .ToList() ?? new List<PersonalFile>();

                foreach (var item in files)
                {
                    Console.WriteLine(item.ToString());
                    Console.WriteLine("-------------------------------------");
                }
            }

            Console.ReadKey();
        }
    }
}
