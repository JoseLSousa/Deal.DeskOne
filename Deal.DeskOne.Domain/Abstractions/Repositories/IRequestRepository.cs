using Deal.DeskOne.Domain.Aggregates.Request;

namespace Deal.DeskOne.Domain.Abstractions.Repositories
{
    public interface IRequestRepository
    {
        Task AddAsync(RequestAggregate request, CancellationToken cancellationToken);
    }
}
