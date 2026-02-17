namespace Deal.DeskOne.Domain.Aggregates.Request
{
    public sealed class RequestStatusHistory
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid RequestId { get; private set; }
        public RequestStatus FromStatus { get; private set; }
        public RequestStatus ToStatus { get; private set; }
        public Guid ChangedBy { get; private set; }
        public DateTime ChangedAt { get; private set; }
        public string? Comment { get; private set; }

        private RequestStatusHistory() { }

        public static RequestStatusHistory Create(
            Guid requestId,
            RequestStatus fromStatus,
            RequestStatus toStatus,
            Guid changedBy,
            string? comment = null)
        {
            return new RequestStatusHistory
            {
                RequestId = requestId,
                FromStatus = fromStatus,
                ToStatus = toStatus,
                ChangedBy = changedBy,
                ChangedAt = DateTime.UtcNow,
                Comment = comment
            };
        }
    }
}
