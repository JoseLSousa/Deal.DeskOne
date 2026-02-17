namespace Deal.DeskOne.Application.Queries.Request.GetRequests
{
    public record RequestResponseDto(
        Guid Id,
        string Title,
        string Description,
        int Category,
        int Priority,
        int Status,
        DateTime CreatedAt,
        Guid CreatedBy,
        Guid? ApprovedBy,
        Guid? RejectedBy,
        string? RejectionReason,
        Guid? DeletedBy
    );
}
