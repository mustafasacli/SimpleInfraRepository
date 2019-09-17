namespace SimpleInfra.Data.ConsoleApp1
{
    using System.Data.Entity;
    using System.Reflection;

    internal class PostgreDbContext : DbContext
    {
        public PostgreDbContext() : base($"name={nameof(PostgreDbContext)}")
        {
            Database.SetInitializer<OracleDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<PersonalFile> PersonalFiles
        { get; set; }
    }
}