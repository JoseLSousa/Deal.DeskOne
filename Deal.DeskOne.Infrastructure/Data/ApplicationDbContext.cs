using Deal.DeskOne.Domain.Aggregates.Request;
using Microsoft.EntityFrameworkCore;

namespace Deal.DeskOne.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {

        public DbSet<RequestAggregate> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<RequestAggregate>()
                .HasQueryFilter(r => !r.IsDeleted);

            modelBuilder.Entity<RequestAggregate>()
                .Property(r => r.IsDeleted)
                .HasDefaultValue(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
