using Deal.DeskOne.Domain.Aggregates.Request;

namespace Deal.DeskOne.API.Contracts.Requests
{
    public record UpdateRequestBody(
        string? Title,
        string? Description,
        RequestCategory? Category,
        RequestPriority? Priority,
        uint Version);
}
