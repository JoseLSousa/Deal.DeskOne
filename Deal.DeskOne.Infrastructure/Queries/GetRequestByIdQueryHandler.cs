using Dapper;
using Deal.DeskOne.Application.Abstractions;
using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Application.Queries.Request.GetRequestById;
using Deal.DeskOne.Application.Queries.Request.GetRequests;

namespace Deal.DeskOne.Infrastructure.Queries
{
    public class GetRequestByIdQueryHandler(IDbConnectionFactory connectionFactory) : IQueryHandler<GetRequestByIdQuery, RequestResponseDto?>
    {
        public async Task<RequestResponseDto?> HandleAsync(GetRequestByIdQuery query, CancellationToken cancellationToken = default)
        {
            using var connection = connectionFactory.CreateConnection();

            const string sql = """
                               SELECT "Id", "Title","Description", "Category","Priority", "Status",
                                      "CreatedAt", "CreatedBy", "ApprovedBy", "RejectedBy", "RejectionReason", "DeletedBy"
                               FROM "Requests"
                               WHERE "Id" = @Id
                                 AND "IsDeleted" = false
                               """;

            return await connection.QueryFirstOrDefaultAsync<RequestResponseDto>(sql, new { Id = query.Id });
        }
    }
}
