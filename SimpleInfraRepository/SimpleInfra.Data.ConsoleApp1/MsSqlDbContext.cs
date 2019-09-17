namespace SimpleInfra.Data.ConsoleApp1
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MsSqlDbContext : DbContext
    {
        public MsSqlDbContext()
            : base("name=MsSqlDbContext")
        {
        }

        public virtual DbSet<PersonalFile> PersonalFiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
