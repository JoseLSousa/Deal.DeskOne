using Deal.DeskOne.Domain.Aggregates.Request;
using Microsoft.EntityFrameworkCore;

namespace Deal.DeskOne.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<RequestAggregate> Requests { get; set; }
        public DbSet<RequestStatusHistory> RequestStatusHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<RequestAggregate>()
                .HasQueryFilter(r => !r.IsDeleted);

            modelBuilder.Entity<RequestAggregate>()
                .Property(r => r.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<RequestAggregate>()
                .HasMany(r => r.StatusHistory)
                .WithOne()
                .HasForeignKey(h => h.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RequestStatusHistory>()
                .HasKey(h => h.Id);

            modelBuilder.Entity<RequestStatusHistory>()
                .Property(h => h.FromStatus)
                .HasConversion<int>();

            modelBuilder.Entity<RequestStatusHistory>()
                .Property(h => h.ToStatus)
                .HasConversion<int>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
