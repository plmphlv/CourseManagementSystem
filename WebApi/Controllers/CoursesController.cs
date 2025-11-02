using Application.Courses.Commands.Create;
using Application.Courses.Commands.Delete;
using Application.Courses.Commands.Update;
using Application.Courses.Queries.GetCourseDetails;
using Application.Courses.Queries.GetCourses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class CoursesController : ApiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<int>> CreateCourse([FromBody] CreateCourseCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCourse([FromRoute] int id, [FromBody] UpdateCourseCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourse([FromRoute] int id)
        {
            await Mediator.Send(new DeleteCourseCommand { Id = id });

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseOutputModel>> GetCourseDetails([FromRoute] int id)
        {
            return await Mediator.Send(new GetCourseDetailsQuery { Id = id });
        }

        [HttpGet("GetCourses")]
        public async Task<ActionResult<List<CourseDto>>> GetCourses([FromQuery] GetCoursesQuery query)
        {
            return await Mediator.Send(query);
        }
    }
}
