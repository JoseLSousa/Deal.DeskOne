using Deal.DeskOne.Application.Abstractions.Mediator;

namespace Deal.DeskOne.Application.Queries.Request.GetRequestHistory
{
    public record GetRequestHistoryQuery(Guid RequestId) : IQuery<IEnumerable<RequestHistoryDto>>;
}
