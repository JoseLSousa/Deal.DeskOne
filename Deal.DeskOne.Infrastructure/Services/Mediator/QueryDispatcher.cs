using Deal.DeskOne.Application.Abstractions.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Deal.DeskOne.Infrastructure.Services.Mediator
{
    public sealed class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
    {
        public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            var handler = serviceProvider.GetRequiredService(handlerType);

            return await ((Task<TResult>)handlerType.GetMethod("HandleAsync")?.Invoke(handler, [query, ct])!)!;
        }
    }

}
