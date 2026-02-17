using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Domain.Aggregates.Request;

namespace Deal.DeskOne.Application.Commands.Request.UpdateRequest
{
    public record UpdateRequestCommand(
        Guid Id,
        string? Title = null,
        string? Description = null,
        RequestCategory? Category = null,
        RequestPriority? Priority = null) : ICommand;
}
