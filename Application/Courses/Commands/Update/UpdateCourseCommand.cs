using Application.Courses.Common;
using FluentValidation.Results;

namespace Application.Courses.Commands.Update
{
    public class UpdateCourseCommand : CourseModel, IRequest
    {
        public int Id { get; set; }
    }

    public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand>
    {
        private readonly IApplicationDbContext context;
        private readonly IDateTime dateTime;

        public UpdateCourseCommandHandler(IApplicationDbContext context, IDateTime dateTime)
        {
            this.context = context;
            this.dateTime = dateTime;
        }

        public async Task Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            Course? course = await context.Courses.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (course is null)
            {
                throw new NotFoundException(nameof(Course), id);
            }

            List<ValidationFailure> failures = [];

            List<Session> sessionsScheduled = await context.Sessions
                .Where(s => s.CourseId == id)
                .ToListAsync(cancellationToken);

            DateTime startDateInput = request.StartDate;

            if (startDateInput != course.StartDate)
            {
                DateTime? firstSessionDate = sessionsScheduled.Select(ss => ss.ScheduledTime)
                    .Order()
                    .FirstOrDefault();

                if (firstSessionDate.HasValue && startDateInput >= firstSessionDate)
                {
                    failures.Add(new ValidationFailure(nameof(Course.StartDate), "Cannot reschedule course's starting date after the earliest session"));
                }

                course.StartDate = request.StartDate;
            }

            DateTime? endDateInput = request.EndDate;

            if (endDateInput != course.EndDate && endDateInput.HasValue)
            {
                DateTime? lastSessionDate = sessionsScheduled.Select(ss => ss.ScheduledTime)
                    .Order()
                    .LastOrDefault();

                endDateInput = endDateInput.Value.AddDays(1).AddTicks(-1);

                if (lastSessionDate.HasValue && endDateInput <= lastSessionDate)
                {
                    failures.Add(new ValidationFailure(nameof(Course.EndDate), "Cannot reschedule course's ending date before the latest scheduled session"));
                }

                course.EndDate = request.EndDate;
            }

            if (failures.Count > 0)
            {
                throw new ValidationException(failures);
            }

            course.Name = request.Name;
            course.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
