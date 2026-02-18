using Deal.DeskOne.Domain.Aggregates.Request;

namespace Deal.DeskOne.Domain.Abstractions.Repositories
{
    public interface IRequestRepository
    {
        Task AddAsync(RequestAggregate request, CancellationToken cancellationToken);
        Task<RequestAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<RequestAggregate>> GetAllAsync(CancellationToken cancellationToken);
        void AddHistory(RequestHistory history);
        void Update(RequestAggregate request);
        void Delete(RequestAggregate request, Guid deletedBy);
    }
}