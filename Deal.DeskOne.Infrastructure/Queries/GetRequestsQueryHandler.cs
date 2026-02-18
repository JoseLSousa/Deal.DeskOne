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
                               SELECT "Id", "Title","Description", "Category","Priority", "Status",
                                      "CreatedAt", "CreatedBy", "ApprovedBy", "RejectedBy", "RejectionReason", "DeletedBy", xmin AS "Version"
                               FROM "Requests"
                               WHERE "IsDeleted" = false
                                     AND (@Status IS NULL OR "Status" = @Status)
                                     AND (@Search IS NULL OR "Title" ILIKE @Search OR "Description" ILIKE @Search)
                               ORDER BY "CreatedAt" DESC
                               """;

            var parameters = new
            {
                Status = query.Status,
                Search = string.IsNullOrEmpty(query.Search) ? null : $"%{query.Search}%"
            };

            return await connection.QueryAsync<RequestResponseDto>(sql, parameters);
        }
    }
}
