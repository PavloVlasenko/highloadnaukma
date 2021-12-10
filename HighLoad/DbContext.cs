using HighLoad.Entities;
using Microsoft.EntityFrameworkCore;

namespace HighLoad
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Book> Books => base.Set<Book>();

        public DbContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        private readonly string? connectionString;

        public DbContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (connectionString != null)
                optionsBuilder.UseSqlServer(connectionString);
        }
    }
}