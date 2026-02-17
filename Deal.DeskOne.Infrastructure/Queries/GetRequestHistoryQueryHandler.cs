using Deal.DeskOne.Application.Abstractions;
using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Application.Queries.Request.GetRequestHistory;
using Dapper;

namespace Deal.DeskOne.Infrastructure.Queries
{
    public class GetRequestHistoryQueryHandler(IDbConnectionFactory dbConnectionFactory)
        : IQueryHandler<GetRequestHistoryQuery, IEnumerable<RequestHistoryDto>>
    {
        public async Task<IEnumerable<RequestHistoryDto>> HandleAsync(
            GetRequestHistoryQuery query,
            CancellationToken cancellationToken = default)
        {
            using var connection = dbConnectionFactory.CreateConnection();

            const string sql = @"
                SELECT 
                    id as Id,
                    request_id as RequestId,
                    from_status as FromStatus,
                    to_status as ToStatus,
                    changed_by as ChangedBy,
                    changed_at as ChangedAt,
                    comment as Comment
                FROM ""RequestStatusHistories""
                WHERE request_id = @RequestId
                ORDER BY changed_at DESC";

            var histories = await connection.QueryAsync<RequestHistoryDto>(
                sql,
                new { RequestId = query.RequestId });

            return histories;
        }
    }
}
