namespace Deal.DeskOne.Domain.Aggregates.Request
{
    public sealed class RequestHistory
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public string FieldName { get; private set; }
        public string OldValue { get; private set; }
        public string NewValue { get; private set; }
        public string Comment { get; private set; }
        public DateTime ChangedAt { get; private set; }
        public Guid ChangedBy { get; private set; }


        private RequestHistory() { }

        public static RequestHistory Create(
            Guid requestId,
            string fieldName,
            string oldValue,
            string newValue,
            string comment,
            Guid changedBy)
        {
            return new RequestHistory
            {
                Id = Guid.NewGuid(),
                RequestId = requestId,
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                Comment = comment,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = changedBy
            };
        }

    }
}
