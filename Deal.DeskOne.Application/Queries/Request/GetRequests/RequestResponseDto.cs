namespace Deal.DeskOne.Application.Queries.Request.GetRequests
{
    public record RequestResponseDto(
        Guid Id,
        string Title,
        DateTime CreatedAt
    );
}
