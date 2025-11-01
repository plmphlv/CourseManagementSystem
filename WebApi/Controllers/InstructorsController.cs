using Application.Instructors.Commands.Create;
using Application.Instructors.Queries.GetInstructorDetails;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class InstructorsController : ApiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<int>> CreateInstructorAccount([FromBody] CreateInstructorCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InstructorOutputModel>> GetInstructorDetails([FromRoute] int id)
        {
            return await Mediator.Send(new GetInstructorDetailsQuery { Id = id });
        }
    }
}
