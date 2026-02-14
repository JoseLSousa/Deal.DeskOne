using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Domain.Aggregates.Request;
using Deal.DeskOne.Domain.Aggregates.Requests;

namespace Deal.DeskOne.Application.Commands.Request.CreateRequest
{
    public abstract record CreateRequestCommand(
        string Title,
        string Description,
        RequestCategory Category,
        RequestPriority Priority,
        Guid CreatedBy
        ) : ICommand<Guid>;
}
