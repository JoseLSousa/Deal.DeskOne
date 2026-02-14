using Deal.DeskOne.Application.Abstractions.Mediator;

namespace Deal.DeskOne.Application.Queries.Request.GetRequests
{
    public record GetRequestsQuery : IQuery<IEnumerable<RequestResponseDto>>;
}
