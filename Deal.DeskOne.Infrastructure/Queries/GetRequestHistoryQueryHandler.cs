using Dapper;
using Deal.DeskOne.Application.Abstractions;
using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Application.Queries.Request.GetRequestHistory;

namespace Deal.DeskOne.Infrastructure.Queries;

public class GetRequestHistoryQueryHandler(IDbConnectionFactory connectionFactory)
    : IQueryHandler<GetRequestHistoryQuery, IEnumerable<RequestHistoryDto>>
{
    public async Task<IEnumerable<RequestHistoryDto>> HandleAsync(GetRequestHistoryQuery query, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
                           SELECT "Id", "FieldName", "OldValue", "NewValue", "Comment", "ChangedAt", "ChangedBy"
                           FROM "RequestHistories"
                           WHERE "RequestId" = @RequestId
                           ORDER BY "ChangedAt" DESC
                           """;

        var parameters = new { query.RequestId };

        return await connection.QueryAsync<RequestHistoryDto>(sql, parameters);
    }
}