using Application.Sessions.Common;
using FluentValidation.Results;

namespace Application.Sessions.Commands.Update
{
    public class UpdateSessionCommand : SessionModel, IRequest
    {
        public int Id { get; set; }
    }

    public class UpdateSessionCommandHandler : IRequestHandler<UpdateSessionCommand>
    {
        private readonly IApplicationDbContext context;

        public UpdateSessionCommandHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            List<string> missingEntityMessages = new List<string>();

            Session? session = await context.Sessions
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (session is null)
            {
                throw new NotFoundException(nameof(Session), id);
            }

            int courseId = request.CourseId;

            Course? course = await context.Courses
               .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

            if (course is null)
            {
                missingEntityMessages.Add($"{nameof(Course)} with Id:{courseId} was not found");
            }

            int instructorId = request.InstructorId;

            bool instructorExists = await context.Instructors
               .AnyAsync(i => i.Id == instructorId, cancellationToken);

            if (!instructorExists)
            {
                missingEntityMessages.Add($"{nameof(Instructor)} with Id:{instructorId} was not found");
            }

            int? substituteInstructorId = request.SubstituteInstructorId;

            if (substituteInstructorId.HasValue)
            {
                bool substituteInstructorExists = await context.Instructors
                   .AnyAsync(i => i.Id == substituteInstructorId, cancellationToken);

                if (!substituteInstructorExists)
                {
                    missingEntityMessages.Add($"{nameof(Instructor)} with Id:{substituteInstructorId} was not found");
                }
            }

            if (missingEntityMessages.Count > 0)
            {
                throw new NotFoundException(missingEntityMessages);
            }

            List<ValidationFailure> validationFailures = new List<ValidationFailure>(2);

            DateTime scheduledTime = request.ScheduledTime;

            if (scheduledTime > course.EndDate)
            {
                validationFailures.Add(new ValidationFailure(nameof(Session.ScheduledTime), $"Sessions cannot be scheduled after course end date"));
            }

            bool sessionExists = await context.Sessions
                .AnyAsync(s => s.CourseId == courseId &&
                s.ScheduledTime == scheduledTime, cancellationToken);

            if (sessionExists)
            {
                validationFailures.Add(new ValidationFailure(nameof(Session.ScheduledTime), $"Session with Scheduled Time:{scheduledTime.ToString()} already exists"));
            }

            if (validationFailures.Count > 0)
            {
                throw new ValidationException(validationFailures);
            }

            session.ScheduledTime = scheduledTime;
            session.DurationMinutes = request.DurationMinutes;
            session.Notes = request.Notes;
            session.CourseId = courseId;
            session.InstructorId = instructorId;
            session.SubstituteInstructorId = substituteInstructorId;

            await context.SaveChangesAsync(cancellationToken);

            if (session.IsConfirmed)
            {
                List<Schedule> schedules = await context.Schedules
                    .Where(s => s.SessionId == id)
                    .ToListAsync(cancellationToken);

                foreach (Schedule schedule in schedules)
                {
                    schedule.ScheduleDate = scheduledTime;
                }

                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
