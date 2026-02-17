using Deal.DeskOne.API.Contracts.Requests;
using Deal.DeskOne.API.Security;
using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Application.Commands.Request.ApproveRequest;
using Deal.DeskOne.Application.Commands.Request.CreateRequest;
using Deal.DeskOne.Application.Commands.Request.DeleteRequest;
using Deal.DeskOne.Application.Commands.Request.RejectRequest;
using Deal.DeskOne.Application.Commands.Request.UpdateRequest;
using Deal.DeskOne.Application.Queries.Request.GetRequestById;
using Deal.DeskOne.Application.Queries.Request.GetRequests;
using Deal.DeskOne.Application.Queries.Request.GetRequestHistory;
using Deal.DeskOne.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deal.DeskOne.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        : ControllerBase
    {
        [Authorize(Policy = AuthorizationPolicies.RequestReadCreate)]
        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequestBody body,
            CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = new CreateRequestCommand(
                body.Title,
                body.Description,
                body.Category,
                body.Priority,
                userId);

            await commandDispatcher.DispatchAsync(command, cancellationToken);
            return Ok();
        }

        [Authorize(Policy = AuthorizationPolicies.RequestReadCreate)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequestResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequests(CancellationToken cancellationToken)
        {
            var query = new GetRequestsQuery();

            var result = await queryDispatcher.DispatchAsync(query, cancellationToken);

            return Ok(result);
        }

        [Authorize(Policy = AuthorizationPolicies.RequestReadCreate)]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RequestResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRequestById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetRequestByIdQuery(id);

            var result = await queryDispatcher.DispatchAsync(query, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [Authorize(Policy = AuthorizationPolicies.RequestReadCreate)]
        [HttpGet("{id:guid}/history")]
        [ProducesResponseType(typeof(IEnumerable<RequestHistoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequestHistory(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetRequestHistoryQuery(id);

            var result = await queryDispatcher.DispatchAsync(query, cancellationToken);

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateRequest(Guid id, [FromBody] UpdateRequestBody body,
            CancellationToken cancellationToken)
        {
            var command = new UpdateRequestCommand(
                id,
                body.Title,
                body.Description,
                body.Category,
                body.Priority);

            await commandDispatcher.DispatchAsync(command, cancellationToken);

            return NoContent();
        }

        [Authorize(Policy = AuthorizationPolicies.RequestApproveReject)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRequest(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = new DeleteRequestCommand(id, userId);

            await commandDispatcher.DispatchAsync(command, cancellationToken);

            return NoContent();
        }

        [Authorize(Policy = AuthorizationPolicies.RequestApproveReject)]
        [HttpPost("{id:guid}/approve")]
        public async Task<IActionResult> ApproveRequest(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var commandWithUser = new ApproveRequestCommand(Id: id, ApprovedBy: userId);

            await commandDispatcher.DispatchAsync(commandWithUser, cancellationToken);

            return NoContent();
        }

        [Authorize(Policy = AuthorizationPolicies.RequestApproveReject)]
        [HttpPost("{id:guid}/reject")]
        public async Task<IActionResult> RejectRequest(Guid id, [FromBody] RejectRequestBody body,
            CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var commandWithUser = new RejectRequestCommand(id, userId, body.Reason);

            await commandDispatcher.DispatchAsync(commandWithUser, cancellationToken);

            return NoContent();
        }
    }
}
