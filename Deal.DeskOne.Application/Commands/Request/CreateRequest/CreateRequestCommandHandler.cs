using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Domain.Abstractions;
using Deal.DeskOne.Domain.Abstractions.Repositories;
using Deal.DeskOne.Domain.Aggregates.Request;

namespace Deal.DeskOne.Application.Commands.Request.CreateRequest
{
    public class CreateRequestCommandHandler(IRequestRepository requestsRepository, IUnitOfWork unitOfWork) : ICommandHandler<CreateRequestCommand, Guid>
    {
        public async Task<Guid> HandleAsync(CreateRequestCommand command, CancellationToken cancellationToken = default)
        {
            var request = RequestAggregate.Create(
                command.Title,
                command.Description,
                command.Category,
                command.Priority,
                command.CreatedBy
            );

            await requestsRepository.AddAsync(request, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return request.Id;
        }
    }
}
