using Deal.DeskOne.Domain.Aggregates.Requests.Events;
using Deal.DeskOne.Domain.Common;

namespace Deal.DeskOne.Domain.Aggregates.Request
{
    public sealed class RequestAggregate : AggregateRoot
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public RequestCategory Category { get; private set; }
        public RequestPriority Priority { get; private set; }
        public RequestStatus Status { get; private set; }
        public Guid CreatedBy { get; private set; }
        public Guid? ApprovedBy { get; private set; }
        public Guid? RejectedBy { get; private set; }
        public string? RejectionReason { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public Guid? DeletedBy { get; private set; }
        public ICollection<RequestStatusHistory> StatusHistory { get; private set; } = new List<RequestStatusHistory>();

        private RequestAggregate() { }

        public static RequestAggregate Create(
            string title,
            string description,
            RequestCategory category,
            RequestPriority priority,
            Guid createdBy)
        {
            var request = new RequestAggregate
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

        public void UpdateTitle(string newTitle)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Cannot update title of approved or rejected requests.");

            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Title cannot be empty.", nameof(newTitle));

            Title = newTitle;
            MarkAsUpdated();
        }

        public void UpdateDescription(string newDescription)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Cannot update description of approved or rejected requests.");

            if (string.IsNullOrWhiteSpace(newDescription))
                throw new ArgumentException("Description cannot be empty.", nameof(newDescription));

            Description = newDescription;
            MarkAsUpdated();
        }

        public void UpdateCategory(RequestCategory newCategory)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Cannot update category of approved or rejected requests.");

            Category = newCategory;
            MarkAsUpdated();
        }

        public void Approve(Guid approvedBy, string? comment = null)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be approved.");

            var previousStatus = Status;
            Status = RequestStatus.Approved;
            ApprovedBy = approvedBy;

            RecordStatusHistory(previousStatus, Status, approvedBy, comment);

            MarkAsUpdated();
            AddDomainEvent(new RequestApprovedEvent(Id, approvedBy));
        }

        public void Reject(Guid rejectedBy, string reason)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be rejected.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Rejection reason is required.", nameof(reason));

            var previousStatus = Status;
            Status = RequestStatus.Rejected;
            RejectedBy = rejectedBy;
            RejectionReason = reason;

            RecordStatusHistory(previousStatus, Status, rejectedBy, reason);

            MarkAsUpdated();
            AddDomainEvent(new RequestRejectedEvent(Id, rejectedBy, reason));
        }

        public void UpdatePriority(RequestPriority newPriority)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Cannot update priority of approved or rejected requests.");

            Priority = newPriority;
            MarkAsUpdated();
        }

        public void SoftDelete(Guid deletedBy)
        {
            if (IsDeleted)
                return;

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
            MarkAsUpdated();
        }

        private void RecordStatusHistory(RequestStatus fromStatus, RequestStatus toStatus, Guid changedBy, string? comment)
        {
            var history = RequestStatusHistory.Create(Id, fromStatus, toStatus, changedBy, comment);
            StatusHistory.Add(history);
        }
    }
}