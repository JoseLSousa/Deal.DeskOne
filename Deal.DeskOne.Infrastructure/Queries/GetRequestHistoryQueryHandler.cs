using Dapper;
using Deal.DeskOne.Application.Abstractions;
using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Application.Queries.Request.GetRequestHistory;

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

            const string sql = """

                                               SELECT 
                                                   "Id",
                                                   "RequestId",
                                                   "FromStatus",
                                                   "ToStatus",
                                                   "ChangedBy",
                                                   "ChangedAt",
                                                   "Comment"
                                               FROM "RequestStatusHistories"
                                               WHERE "RequestId" = @RequestId
                                               ORDER BY "ChangedAt" DESC
                               """;

            var histories = await connection.QueryAsync<RequestHistoryDto>(
                sql,
                new { RequestId = query.RequestId });

            return histories;
        }
    }
}