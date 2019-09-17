using SimpleInfra.Data;

namespace SimpleInfra.Data.ConsoleApp1
{
    public class MsSqlRepository<T> : SimpleBaseDataRepository<T> where T : class
    {
        public MsSqlRepository() : base(new MsSqlDbContext(), errorLogEnable: false)
        {
        }
    }
}