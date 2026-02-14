using Deal.DeskOne.Application.Abstractions.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Deal.DeskOne.Infrastructure.Services.Mediator
{
    public sealed class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
    {
        public async Task DispatchAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            var commandType = command.GetType();

            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);

            var handler = serviceProvider.GetService(handlerType);

            var method = handlerType.GetMethod(nameof(ICommandHandler<ICommand>.HandleAsync));

            await (Task)method!.Invoke(handler, [command, cancellationToken])!;
        }

        public async Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        {
            var commandType = command.GetType();

            // Monta o tipo do handler: ICommandHandler<TCommand, TResult>
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));

            var handler = serviceProvider.GetRequiredService(handlerType);

            // Executa e faz o cast para Task<TResult>
            var method = handlerType.GetMethod(nameof(ICommandHandler<ICommand<TResult>, TResult>.HandleAsync));
            return await (Task<TResult>)method!.Invoke(handler, [command, cancellationToken])!;
        }
    }
}
