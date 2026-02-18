using Deal.DeskOne.Domain.Common;

namespace Deal.DeskOne.Domain.Aggregates.Requests.Events
{
    public sealed record RequestRejectedEvent(
        Guid RequestId,
        Guid RejectedBy,
        string Reason) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}