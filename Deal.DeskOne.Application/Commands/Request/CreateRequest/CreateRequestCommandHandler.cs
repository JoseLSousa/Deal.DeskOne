using Deal.DeskOne.Application.Abstractions.Mediator;

namespace Deal.DeskOne.Application.Commands.Request.CreateRequest
{
    public class CreateRequestCommandHandler(ApplicationDb) : ICommandHandler<CreateRequestCommand, Guid>
    {
        public Task<Guid> HandleAsync(CreateRequestCommand command, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
