using Deal.DeskOne.Application.Abstractions.Mediator;

namespace Deal.DeskOne.Application.Commands.Request.DeleteRequest
{
    public record DeleteRequestCommand(Guid Id, Guid DeletedBy) : ICommand;
}
