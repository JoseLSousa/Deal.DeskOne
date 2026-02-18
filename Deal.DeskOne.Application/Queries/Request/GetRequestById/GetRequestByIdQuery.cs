using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Application.Queries.Request.GetRequests;

namespace Deal.DeskOne.Application.Queries.Request.GetRequestById
{
    public record GetRequestByIdQuery(Guid Id) : IQuery<RequestResponseDto?>;
}
