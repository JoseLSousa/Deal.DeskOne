using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Domain.Abstractions;
using Deal.DeskOne.Domain.Abstractions.Repositories;

namespace Deal.DeskOne.Application.Commands.Request.UpdateRequest
{
    // 2. Injete o ILogger
    public class UpdateRequestCommandHandler(
        IRequestRepository requestRepository,
        IUnitOfWork unitOfWork) : ICommandHandler<UpdateRequestCommand>
    {
        public async Task HandleAsync(UpdateRequestCommand command, CancellationToken cancellationToken = default)
        {
            var request = await requestRepository.GetByIdAsync(command.Id, cancellationToken);

            if (request is null)
                throw new InvalidOperationException($"Request with id {command.Id} not found.");

            // --- INÍCIO DA DEPURAÇÃO ---
            // 3. Adicione os logs para comparar as versões
            Console.WriteLine($"Iniciando atualização para Request ID: {command.Id}");
            Console.WriteLine($"--> Versão recebida do Cliente (Comando): {command.Version}");
            Console.WriteLine($"--> Versão lida do Banco (Repositório): {request.Version}");

            if (command.Version != request.Version)
            {
                Console.WriteLine("ALERTA: A versão do cliente já está defasada no momento da leitura!");
            }
            // --- FIM DA DEPURAÇÃO ---

            unitOfWork.SetOriginalVersion(request, command.Version);

            if (!string.IsNullOrWhiteSpace(command.Title))
                request.UpdateTitle(command.Title, command.ChangedBy);

            if (!string.IsNullOrWhiteSpace(command.Description))
                request.UpdateDescription(command.Description, command.ChangedBy);

            if (command.Category.HasValue)
                request.UpdateCategory(command.Category.Value, command.ChangedBy);

            if (command.Priority.HasValue)
                request.UpdatePriority(command.Priority.Value, command.ChangedBy);

            foreach (var history in request.History)
                requestRepository.AddHistory(history);

            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                Console.WriteLine($"Atualização para Request ID: {command.Id} concluída com sucesso.");
            }
            catch (Exception)
            {
                Console.WriteLine($"Ocorreu um erro de concorrência ao salvar o Request ID: {command.Id}");
                throw new InvalidOperationException("Os dados foram alterados por outro usuário. Por favor, recarregue a página.");
            }
        }
    }
}