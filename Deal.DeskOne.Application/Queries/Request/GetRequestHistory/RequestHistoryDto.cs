namespace Deal.DeskOne.Application.Queries.Request.GetRequestHistory
{
    public record RequestHistoryDto(
        Guid Id,
        Guid RequestId,
        int FromStatus,
        int ToStatus,
        Guid ChangedBy,
        DateTime ChangedAt,
        string? Comment);
}
