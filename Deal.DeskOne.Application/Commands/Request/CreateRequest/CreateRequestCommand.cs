using Deal.DeskOne.Application.Abstractions.Mediator;

namespace Deal.DeskOne.Application.Commands.Request.CreateRequest
{
    public record CreateRequestCommand : ICommand<Guid>
    {
    }
}
