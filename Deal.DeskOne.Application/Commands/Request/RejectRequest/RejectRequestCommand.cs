using Deal.DeskOne.Application.Abstractions.Mediator;

namespace Deal.DeskOne.Application.Commands.Request.RejectRequest
{
    public record RejectRequestCommand(Guid Id, Guid RejectedBy, string Reason) : ICommand;
}
