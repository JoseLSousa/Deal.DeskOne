using Deal.DeskOne.Domain.Common;

namespace Deal.DeskOne.Domain.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void SetOriginalVersion<T>(T entity, uint version) where T : AggregateRoot;
    }
}
