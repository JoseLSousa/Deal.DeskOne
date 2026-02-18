using Deal.DeskOne.Domain.Abstractions;
using Deal.DeskOne.Domain.Common;
using Deal.DeskOne.Infrastructure.Data;

namespace Deal.DeskOne.Infrastructure.Persistence
{
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        public void Dispose()
            => context.Dispose();

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public void SetOriginalVersion<T>(T entity, uint version) where T : AggregateRoot
        {
            context.Entry(entity).Property(e => e.Version).OriginalValue = version;
        }
    }
}
