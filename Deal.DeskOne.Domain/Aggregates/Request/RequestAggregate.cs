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
        private readonly List<RequestHistory> _history = [];
        public IReadOnlyCollection<RequestHistory> History => _history.AsReadOnly();

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

            request._history.Add(RequestHistory.Create(request.Id, "Request", string.Empty, "Created", "Request created successfully.", createdBy));
            request.AddDomainEvent(new RequestCreatedEvent(request.Id, title, category, createdBy));

            return request;
        }

        public void UpdateTitle(string newTitle, Guid changedBy)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Cannot update title of approved or rejected requests.");

            if (string.IsNullOrWhiteSpace(newTitle) || Title == newTitle)
                return;

            var oldValue = Title;
            Title = newTitle;

            _history.Add(RequestHistory.Create(Id, nameof(Title), oldValue, newTitle, string.Empty, changedBy));
            MarkAsUpdated();
        }

        public void UpdateDescription(string newDescription, Guid changedBy)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Cannot update description of approved or rejected requests.");

            if (string.IsNullOrWhiteSpace(newDescription) || Description == newDescription)
                return;

            var oldValue = Description;
            Description = newDescription;

            _history.Add(RequestHistory.Create(Id, nameof(Description), oldValue, newDescription, string.Empty, changedBy));
            MarkAsUpdated();
        }

        public void UpdateCategory(RequestCategory newCategory, Guid changedBy)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Cannot update category of approved or rejected requests.");

            if (Category == newCategory)
                return;

            var oldValue = Category.ToString();
            Category = newCategory;

            _history.Add(RequestHistory.Create(Id, nameof(Category), oldValue, newCategory.ToString(), string.Empty, changedBy));
            MarkAsUpdated();
        }

        public void Approve(Guid approvedBy, string? comment = null)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be approved.");

            var previousStatus = Status;
            Status = RequestStatus.Approved;
            ApprovedBy = approvedBy;

            _history.Add(RequestHistory.Create(Id, nameof(Status), previousStatus.ToString(), Status.ToString(), comment ?? "Request approved.", approvedBy));
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

            _history.Add(RequestHistory.Create(Id, nameof(Status), previousStatus.ToString(), Status.ToString(), reason, rejectedBy));
            MarkAsUpdated();
            AddDomainEvent(new RequestRejectedEvent(Id, rejectedBy, reason));
        }

        public void UpdatePriority(RequestPriority newPriority, Guid changedBy)
        {
            if (Status != RequestStatus.Pending)
                throw new InvalidOperationException("Cannot update priority of approved or rejected requests.");

            if (Priority == newPriority)
                return;

            var oldValue = Priority.ToString();
            Priority = newPriority;

            _history.Add(RequestHistory.Create(Id, nameof(Priority), oldValue, newPriority.ToString(), string.Empty, changedBy));
            MarkAsUpdated();
        }

        public void SoftDelete(Guid deletedBy)
        {
            if (IsDeleted)
                return;

            var oldValue = IsDeleted;
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;

            _history.Add(RequestHistory.Create(Id, nameof(IsDeleted), oldValue.ToString(), IsDeleted.ToString(), "Request deleted.", deletedBy));
            MarkAsUpdated();
        }
    }
}