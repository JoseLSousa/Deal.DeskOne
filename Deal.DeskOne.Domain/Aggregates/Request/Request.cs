using Deal.DeskOne.Domain.Aggregates.Requests;
using Deal.DeskOne.Domain.Aggregates.Requests.Events;
using Deal.DeskOne.Domain.Common;

namespace Deal.DeskOne.Domain.Aggregates.Request
{
    public sealed class Request : AggregateRoot
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public RequestCategory Category { get; private set; }
        public Priority Priority { get; private set; }
        private RequestStatus Status { get; set; }
        public Guid CreatedBy { get; private set; }
        public Guid? ApprovedBy { get; private set; }
        public Guid? RejectedBy { get; private set; }
        public string? RejectionReason { get; private set; }

        private Request() { }

        public static Request Create(
            string title,
            string description,
            RequestCategory category,
            Priority priority,
            Guid createdBy)
        {
            var request = new Request
            {
                Title = title,
                Description = description,
                Category = category,
                Priority = priority,
                Status = RequestStatus.Pending,
                CreatedBy = createdBy
            };

            request.AddDomainEvent(new RequestCreatedEvent(request.Id, title, category, createdBy));

            return request;
        }

        public void Approve(Guid approvedBy)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be approved.");

            Status = RequestStatus.Approved;
            ApprovedBy = approvedBy;

            MarkAsUpdated();
            AddDomainEvent(new RequestApprovedEvent(Id, approvedBy));
        }

        public void Reject(Guid rejectedBy, string reason)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be rejected.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Rejection reason is required.", nameof(reason));

            Status = RequestStatus.Rejected;
            RejectedBy = rejectedBy;
            RejectionReason = reason;

            MarkAsUpdated();
            AddDomainEvent(new RequestRejectedEvent(Id, rejectedBy, reason));
        }

        public void UpdatePriority(Priority newPriority)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Cannot update priority of approved or rejected requests.");

            Priority = newPriority;
            MarkAsUpdated();
        }
    }
}