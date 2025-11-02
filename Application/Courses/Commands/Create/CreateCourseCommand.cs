using Application.Courses.Common;
using FluentValidation.Results;

namespace Application.Courses.Commands.Create
{
    public class CreateCourseCommand : CourseModel, IRequest<int>
    {
        public int InstructorId { get; set; }

        public ICollection<SessionInputModel> Sessions { get; set; } = new List<SessionInputModel>();
    }

    public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, int>
    {
        private readonly IApplicationDbContext context;

        public CreateCourseCommandHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<int> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            int instructorId = request.InstructorId;

            bool instructorExists = await context
                .Instructors
                .AnyAsync(i => i.Id == instructorId, cancellationToken);

            if (!instructorExists)
            {
                throw new NotFoundException(nameof(Instructor), instructorId);
            }

            string name = request.Name;
            bool isCourseNameTaken = await context
                .Courses
                .AnyAsync(i => i.Name == name, cancellationToken);

            if (isCourseNameTaken)
            {
                throw new ValidationException([new ValidationFailure(nameof(Course.Name), "Name is already taken")]);
            }

            Course course = new Course
            {
                InstructorId = instructorId,
                Name = request.Name,
                Description = request.Description,
                MemberLimit = request.MemberLimit,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
            };

            foreach (SessionInputModel s in request.Sessions)
            {
                Session session = new Session
                {
                    ScheduledTime = s.ScheduledDate,
                    Notes = s.Notes,
                    Course = course,
                    InstructorId = instructorId,
                    IsConfirmed = true,
                    DurationMinutes = s.DurationMinutes
                };

                course.Sessions.Add(session);
            }

            await context.Courses.AddAsync(course, cancellationToken);

            return course.Id;
        }
    }
}
