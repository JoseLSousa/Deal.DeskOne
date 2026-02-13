using Deal.DeskOne.Application.Abstractions.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Deal.DeskOne.Application.Services.Mediator
{
    public class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
    {
        public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) where TQuery : IQuery<TResult>
        {
            var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            return await handler.HandleAsync(query, cancellationToken);
        }
    }
}
