using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Domain.Abstractions;
using Deal.DeskOne.Domain.Abstractions.Repositories;

namespace Deal.DeskOne.Application.Commands.Request.DeleteRequest
{
    public class DeleteRequestCommandHandler(IRequestRepository requestRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeleteRequestCommand>
    {
        public async Task HandleAsync(DeleteRequestCommand command, CancellationToken cancellationToken = default)
        {
            var request = await requestRepository.GetByIdAsync(command.Id, cancellationToken);

            if (request is null)
                throw new InvalidOperationException($"Request with id {command.Id} not found.");

            request.SoftDelete(command.DeletedBy);

            requestRepository.Update(request);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
