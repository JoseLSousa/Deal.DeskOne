using Deal.DeskOne.Application.Abstractions.Mediator;

namespace Deal.DeskOne.Application.Queries.Request.GetRequests
{
    public record GetRequestsQuery(
        int? Status = null,
        string? Search = null)
        : IQuery<IEnumerable<RequestResponseDto>>;
}
