namespace Deal.DeskOne.Application.Queries.Request.GetRequestHistory;

public record RequestHistoryDto(
    Guid Id,
    string FieldName,
    string OldValue,
    string NewValue,
    string Comment,
    DateTime ChangedAt,
    Guid ChangedBy
)
{
    // Adicionado para compatibilidade com o Dapper
    public RequestHistoryDto() : this(Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, default, default) { }
}