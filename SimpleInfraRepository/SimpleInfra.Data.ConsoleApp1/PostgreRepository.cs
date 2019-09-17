using SimpleInfra.Data;

namespace SimpleInfra.Data.ConsoleApp1
{
    public class PostgreRepository<T> : SimpleBaseDataRepository<T> where T : class
    {
        public PostgreRepository() : base(new PostgreDbContext(), errorLogEnable: false)
        {
        }
    }
}