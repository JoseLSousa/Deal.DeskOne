using Deal.DeskOne.Domain.Aggregates.Request;
using Deal.DeskOne.Domain.Common;

namespace Deal.DeskOne.Domain.Aggregates.Requests.Events
{
    public sealed record RequestCreatedEvent(
        Guid RequestId,
        string Title,
        RequestCategory Category,
        Guid CreatedBy) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}