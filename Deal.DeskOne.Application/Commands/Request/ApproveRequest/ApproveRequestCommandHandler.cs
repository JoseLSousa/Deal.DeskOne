using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Domain.Abstractions;
using Deal.DeskOne.Domain.Abstractions.Repositories;

namespace Deal.DeskOne.Application.Commands.Request.ApproveRequest
{
    public class ApproveRequestCommandHandler(IRequestRepository requestRepository, IUnitOfWork unitOfWork) : ICommandHandler<ApproveRequestCommand>
    {
        public async Task HandleAsync(ApproveRequestCommand command, CancellationToken cancellationToken = default)
        {
            var request = await requestRepository.GetByIdAsync(command.Id, cancellationToken);

            if (request is null)
                throw new InvalidOperationException($"Request with id {command.Id} not found.");

            request.Approve(command.ApprovedBy);

            requestRepository.Update(request);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
