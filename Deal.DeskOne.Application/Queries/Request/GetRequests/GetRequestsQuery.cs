using Deal.DeskOne.Application.Abstractions.Mediator;

namespace Deal.DeskOne.Application.Queries.Request.GetRequests
{
    public record GetRequestsQuery(
        string? Status = null,
        string? Search = null)
        : IQuery<IEnumerable<RequestResponseDto>>;
}
