using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Application.Commands.Request.CreateRequest;
using Deal.DeskOne.Application.Queries.Request.GetRequests;
using Microsoft.AspNetCore.Mvc;

namespace Deal.DeskOne.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequestCommand command,
            CancellationToken cancellationToken)
        {
            await commandDispatcher.DispatchAsync(command, cancellationToken);
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequestResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequests(CancellationToken cancellationToken)
        {
            var query = new GetRequestsQuery();

            var result = await queryDispatcher.DispatchAsync(query, cancellationToken);

            return Ok(result);
        }
    }
}
