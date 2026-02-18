using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Domain.Aggregates.Request;

namespace Deal.DeskOne.Application.Commands.Request.UpdateRequest
{
    public record UpdateRequestCommand(
        Guid Id,
        string? Title,
        string? Description,
        RequestCategory? Category,
        RequestPriority? Priority,
        Guid ChangedBy,
        uint Version) : ICommand;
}
