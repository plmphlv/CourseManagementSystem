using Application.Schedules.Command.Cancel;
using Application.Schedules.Command.Confirm;
using Application.Schedules.Queries.GetSchedules;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class SchedulesController : ApiControllerBase
    {
        [HttpGet("GetSchedules")]
        public async Task<ActionResult<List<ScheduleDto>>> GetSchedules([FromQuery] GetSchedulesQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpPut("CancelSchedule/{id}")]
        public async Task<ActionResult<List<ScheduleDto>>> CancelSchedule([FromRoute] int id, [FromBody] CancelScheduleCommand command)
        {
            if(id != command.Id)
            {
                return BadRequest("Mismatched schedule ID");
            }

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpPut("ConfirmSchedule/{id}")]
        public async Task<ActionResult<List<ScheduleDto>>> ConfirmSchedule([FromRoute] int id, [FromQuery] ConfirmScheduleCommand command)
        {
            await Mediator.Send(command);

            return NoContent();
        }
    }
}
