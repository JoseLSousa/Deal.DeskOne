using Deal.DeskOne.Domain.Abstractions;
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
    }
}
