using Microsoft.EntityFrameworkCore;
using Infrastructure.SQL.Database.Entities;

namespace Infrastructure.SQL.Database
{
    public class PostgreSQLDbContext : DbContext
    {
        public PostgreSQLDbContext(DbContextOptions<PostgreSQLDbContext> options) : base(options)
        {
        }

        public DbSet<CollectionEntity> Collections { get; set; }

    }
}
