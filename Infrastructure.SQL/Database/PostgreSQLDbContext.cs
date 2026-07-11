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
        public DbSet<FlashCardEntity> FlashCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CollectionEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<FlashCardEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.OwnsOne(e => e.Question);
                entity.OwnsOne(e => e.Answer);

                entity.HasOne<CollectionEntity>()
                .WithMany(c => c.FlashCards)
                .HasForeignKey(e => e.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }

    }
}
