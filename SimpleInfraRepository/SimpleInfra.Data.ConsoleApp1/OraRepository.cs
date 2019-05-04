using SimpleInfra.Data;

namespace SimpleInfra.Data.ConsoleApp1
{
    public class OraRepository<T> : SimpleBaseDataRepository<T> where T : class
    {
        public OraRepository() : base(new OracleDbContext(), errorLogEnable: false)
        {
        }
    }
}