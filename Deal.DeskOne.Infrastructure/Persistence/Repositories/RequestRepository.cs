using Deal.DeskOne.Domain.Abstractions.Repositories;
using Deal.DeskOne.Domain.Aggregates.Request;
using Deal.DeskOne.Infrastructure.Data;

namespace Deal.DeskOne.Infrastructure.Persistence.Repositories
{
    public class RequestRepository(ApplicationDbContext context) : IRequestRepository
    {
        public async Task AddAsync(RequestAggregate request, CancellationToken cancellationToken)
        {
            await context.Requests.AddAsync(request, cancellationToken);
        }
    }
}
