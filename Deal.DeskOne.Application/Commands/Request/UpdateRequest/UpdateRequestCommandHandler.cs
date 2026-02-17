using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Domain.Abstractions;
using Deal.DeskOne.Domain.Abstractions.Repositories;

namespace Deal.DeskOne.Application.Commands.Request.UpdateRequest
{
    public class UpdateRequestCommandHandler(IRequestRepository requestRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateRequestCommand>
    {
        public async Task HandleAsync(UpdateRequestCommand command, CancellationToken cancellationToken = default)
        {
            var request = await requestRepository.GetByIdAsync(command.Id, cancellationToken);

            if (request is null)
                throw new InvalidOperationException($"Request with id {command.Id} not found.");

            if (!string.IsNullOrWhiteSpace(command.Title))
                request.UpdateTitle(command.Title);

            if (!string.IsNullOrWhiteSpace(command.Description))
                request.UpdateDescription(command.Description);

            if (command.Category.HasValue)
                request.UpdateCategory(command.Category.Value);

            if (command.Priority.HasValue)
                request.UpdatePriority(command.Priority.Value);

            requestRepository.Update(request);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
