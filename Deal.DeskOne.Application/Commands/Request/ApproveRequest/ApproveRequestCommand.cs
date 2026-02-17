using Deal.DeskOne.Application.Abstractions.Mediator;

namespace Deal.DeskOne.Application.Commands.Request.ApproveRequest
{
    public record ApproveRequestCommand(Guid Id, Guid ApprovedBy) : ICommand;
}
