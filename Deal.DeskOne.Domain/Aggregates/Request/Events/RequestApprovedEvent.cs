using Deal.DeskOne.Domain.Common;

namespace Deal.DeskOne.Domain.Aggregates.Requests.Events
{
    public sealed record RequestApprovedEvent(
        Guid RequestId,
        Guid ApprovedBy) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}