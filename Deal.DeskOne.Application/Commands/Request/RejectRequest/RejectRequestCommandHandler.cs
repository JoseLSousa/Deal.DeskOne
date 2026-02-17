using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Domain.Abstractions;
using Deal.DeskOne.Domain.Abstractions.Repositories;

namespace Deal.DeskOne.Application.Commands.Request.RejectRequest
{
    public class RejectRequestCommandHandler(IRequestRepository requestRepository, IUnitOfWork unitOfWork) : ICommandHandler<RejectRequestCommand>
    {
        public async Task HandleAsync(RejectRequestCommand command, CancellationToken cancellationToken = default)
        {
            var request = await requestRepository.GetByIdAsync(command.Id, cancellationToken);

            if (request is null)
                throw new InvalidOperationException($"Request with id {command.Id} not found.");

            request.Reject(command.RejectedBy, command.Reason);

            requestRepository.Update(request);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
