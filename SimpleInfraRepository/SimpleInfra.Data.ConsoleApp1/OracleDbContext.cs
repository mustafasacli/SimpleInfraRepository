namespace SimpleInfra.Data.ConsoleApp1
{
    using System.Data.Entity;
    using System.Reflection;

    internal class OracleDbContext : DbContext
    {
        public OracleDbContext() : base($"name={nameof(OracleDbContext)}")
        {
            Database.SetInitializer<OracleDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ORACLE_SCHEMANAME");
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<PersonalFile> PersonalFiles
        { get; set; }
    }
}