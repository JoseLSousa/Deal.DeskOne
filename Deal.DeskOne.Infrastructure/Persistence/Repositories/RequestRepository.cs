using Deal.DeskOne.Domain.Abstractions.Repositories;
using Deal.DeskOne.Domain.Aggregates.Request;
using Deal.DeskOne.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Deal.DeskOne.Infrastructure.Persistence.Repositories
{
    public class RequestRepository(ApplicationDbContext context) : IRequestRepository
    {
        public async Task AddAsync(RequestAggregate request, CancellationToken cancellationToken)
        {
            await context.Requests.AddAsync(request, cancellationToken);
        }

        public async Task<RequestAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await context.Requests.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<RequestAggregate>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await context.Requests.ToListAsync(cancellationToken);
        }

        public void Update(RequestAggregate request)
        {
            context.Requests.Update(request);
        }

        public void Delete(RequestAggregate request, Guid deletedBy)
        {
            request.SoftDelete(deletedBy);
            context.Requests.Update(request);
        }
    }
}
