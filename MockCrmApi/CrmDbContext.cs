using Microsoft.EntityFrameworkCore;

namespace MockCrmApi
{
    public class CrmClient
    {
        public string Code { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int Status { get; set; }
    }

    public class CrmDbContext : DbContext
    {
        public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options) { }

        public DbSet<CrmClient> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CrmClient>().HasKey(c => c.Code);
        }
    }
}