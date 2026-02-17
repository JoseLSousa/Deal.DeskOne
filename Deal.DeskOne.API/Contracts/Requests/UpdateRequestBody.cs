using Deal.DeskOne.Domain.Aggregates.Request;

namespace Deal.DeskOne.API.Contracts.Requests
{
    public sealed record UpdateRequestBody(
        string? Title = null,
        string? Description = null,
        RequestCategory? Category = null,
        RequestPriority? Priority = null);
}
