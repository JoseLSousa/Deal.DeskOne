using Dapper;
using Deal.DeskOne.Application.Abstractions;
using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Application.Queries.Request.GetRequests;

namespace Deal.DeskOne.Infrastructure.Queries
{
    public class GetRequestsQueryHandler(IDbConnectionFactory connectionFactory) : IQueryHandler<GetRequestsQuery, IEnumerable<RequestResponseDto>>
    {
        public async Task<IEnumerable<RequestResponseDto>> HandleAsync(GetRequestsQuery query, CancellationToken cancellationToken = default)
        {
            using var connection = connectionFactory.CreateConnection();

            const string sql = """
                               SELECT "Id", "Title", "CreatedAt"
                               FROM "Requests"
                               ORDER BY "CreatedAt" DESC
                               """;

            return await connection.QueryAsync<RequestResponseDto>(sql);
        }
    }
}
