using Application.Sessions.Commands.Cancel;
using Application.Sessions.Commands.Confirm;
using Application.Sessions.Commands.Create;
using Application.Sessions.Commands.Delete;
using Application.Sessions.Commands.Update;
using Application.Sessions.Queries.GetSessionDetails;
using Application.Sessions.Queries.GetSessions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class SessionsController : ApiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<int>> CreateSession([FromBody] CreateSessionCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSessions([FromRoute] int id, [FromBody] UpdateSessionCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSession([FromRoute] int id)
        {
            await Mediator.Send(new DeleteSessionCommand { Id = id });

            return NoContent();
        }

        [HttpPut("ConfirmSessions")]
        public async Task<ActionResult> ConfirmSessions([FromBody] ConfirmSessionsCommand command)
        {
            await Mediator.Send(command);

            return NoContent();
        }

        [HttpPut("CancelSessions")]
        public async Task<ActionResult> CancelSessions([FromBody] CancelSessionsCommand command)
        {
            await Mediator.Send(command);

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SessionOutputModel>> GetSessionDetails([FromRoute] int id)
        {
            return await Mediator.Send(new GetSessionDetailsQuery { Id = id });
        }

        [HttpGet("GetSessions")]
        public async Task<ActionResult<List<SessionDto>>> GetSessions([FromQuery] GetSessionsQuery query)
        {
            return await Mediator.Send(query);
        }
    }
}
