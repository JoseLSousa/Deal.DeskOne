using Deal.DeskOne.Domain.Aggregates.Request;

namespace Deal.DeskOne.API.Contracts.Requests
{
    public sealed record CreateRequestBody(
        string Title,
        string Description,
        RequestCategory Category,
        RequestPriority Priority);
}
